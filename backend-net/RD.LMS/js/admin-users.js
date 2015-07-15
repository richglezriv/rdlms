jQuery(function($){


	var settings = null,
		loading = false,
		body = $('body'),
		win = $(window),
		list = $('.list.users'),
		add = $('#add-new'),
		modal = $('#user-modal'),
		stats = $('#stats-modal'),
		users = null,
		searchForm = $('#user-search').hide()
	;




	// Utils ______________________________________________________________________

	function htmlEntities(str){
		return String(str).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace("\n", '<br>');
	}




	// GUI helpers ________________________________________________________________

	function showFeedback(msg){
		alert(msg);
	}

	function startLoading(){
		loading = true;
		body.css('cursor', 'progress');
		modal.find('.modal-footer button').prop('disabled', true);
	}

	function stopLoading(){
		loading = false;
		body.css('cursor', '');
		modal.find('.modal-footer button').prop('disabled', false);
	}




	// Users loading ____________________________________________________________

	function onLMSInitialized(){
		//add.on('click', function(e){ e.preventDefault(); if(!loading) addUser(); });

		// Fetch users
		settings = RDLMS.settings;
		searchForm.show();
		searchForm.on('submit', function(e){
			e.preventDefault();
			findUsers(searchForm.find('input').val());
		});
	}


	function findUsers(q){
		if(loading) return;
		startLoading();
		var jsonData = {query: q};
		$.ajax({
			url: settings.admin.user.find,
			dataType: "json", method: 'POST',
			data: {data: JSON.stringify(jsonData)}
		})
			.done(function(response){
				if(response.status && response.status === 'success'){
					list.empty();
					users = response.data;
					if(users.length){
						$.each(users, function(i, user){
							list.append(renderUser(
								user.id,
								user.name,
								user.lastName,
								user.secondLastName,
								user.email,
								user.extra
							));
						});
					}else{
						list.append('<div><h4 class="text-info">No se encontró ningún usuario.</h4></div>');
					}
				}else{
					showFeedback(response.message);
				}
			})
			.fail(function(){ showFeedback('No fue posible cargar la lista de usuarios'); })
			.always(stopLoading)
		;
	}


	function renderUser(id, name, lastName, secondLastName, email, extra){
		var user = $('<div class="media user"><div class="media-body"/></div>');
		user.data('data', {id:id, name:name, lastName: lastName, secondLastName: secondLastName, email: email, extra: extra});
		user.find('.media-body')
			.append('<h4 class="media-heading">' + htmlEntities(([name, lastName, secondLastName]).join(' ')) + '</h4>')
			.append('<div class="email">' + htmlEntities(email) + '</div>')
			.append('<div class="actions"><button class="btn btn-default btn-sm btn-stats" aria-label="Cursos"><span class="glyphicon glyphicon-stats" aria-hidden="true"></span></button> <button class="btn btn-danger btn-sm btn-delete" aria-label="Borrar"><span class="glyphicon glyphicon-trash" aria-hidden="true"></span></button></div>')
		;
		user.on('click', '.btn-stats', function(e){ showStats(id); });
		//user.on('click', '.btn-edit', function(e){ editUser(user); });
		user.on('click', '.btn-delete', function(e){ deleteUser(user); });
		return user;
	}


	function editUser(u){
		showModal(u.data('data').id);
	}


	function addUser(){
		showModal();
	}


	function clearScorm(uid, cid){
		if(confirm(
			"Esta acción borrará el avance del usuario para este curso. \nEstás seguro que deseas continuar?"
		)){
			var jsonData = {userId: uid, courseId: cid};
			$.ajax({
				url: settings.admin.user.clearScorm,
				dataType: "json", method: 'POST',
				data: {data: JSON.stringify(jsonData)}
			})
				.done(function(r){
					if(r.status && r.status == 'success'){
						//showFeedback('El avance del usuario fue borrado con éxito.');
						showStats(uid);
					}else{
						showFeedback('No se pudo borrar el avance del usuario, por favor intenta más tarde.');
					}
				})
				.fail(function(){
					showFeedback('Ocurrió un error al intentar borrar el avance. Por favor intenta más tarde.');
				})
				.always(function(r){
					console.log(r);
				})
			;
		}
	}


	function deleteUser(u){
		if(confirm("Esta acción borrará el usuario y toda su información relacionada.\n¿Estás seguro que deseas continuar?")){
			u.css('background-color', '#fffaee').fadeOut();
			var jsonData = {userId: u.data('data').id};
			$.ajax({
				url: settings.admin.user.delete,
				dataType: "json", method: 'POST',
				data: {data: JSON.stringify(jsonData)}
			})
				.done(function(r){
					if(!r.status || r.status != 'success') cancelDeleteUser(u);
				})
				.fail(function(){
					cancelDeleteUser(u);
				})
				.always(function(r){
					console.log(r);
				})
			;
		}
	}
	function cancelDeleteUser(u){
		showFeedback('No se pudo borrar el usuario: "' + u.data('data').name + ' ' + u.data('data').lastName + '"');
		u.fadeIn(function(){ u.css('background-color', ''); });
	}


	function showModal(id){
		user = $.grep(users, function(o){ return o.id == id; })[0] || null;
		
		// Clean modal
		hideErrors();
		modal.data('id', null);
		modal.find('input, textarea').val('');
		modal.find('select').val([]);
		uploadThumb.val('');
		uploadScorm.val('');
		modal.find('.modal-title').text('Nuevo usuario');
		
		// Load user info into modal (when id is provided)
		if(user){
			modal.data('id', user.id);
			// TODO: More values
		}
		modal.modal('show');
	}


	// Cancel modal
	modal.find('.btn-cancel').on('click', function(e){
		e.preventDefault();
		if(loading) return;
		modal.modal('hide');
	});
	

	// Save modal
	modal.find('.btn-save').on('click', function(e){
		e.preventDefault();
		if(loading) return;
		startLoading();
		
		var jsonData = {
			// Data to be sent...
		};
		if(modal.data('id')) jsonData.userId = modal.data('id');
		
		$.ajax({
			url: settings.admin.user.save,
			dataType: "json", method: 'POST',
			data: {data: JSON.stringify(jsonData)}
		})
			.done(function(r){
				if(r.status && r.status == 'success'){
					// TODO
				}else if(r.status && r.status == 'fail'){
					showFeedback('Algunos de los datos que especificaste son inválidos. Por favor revisa los campos marcados.');
					showErrors(r.status.data);
				}else{
					showFeedback('Ocurrió un error al intentar guardar. Por favor intenta más tarde.');
				}
			})
			.fail(function(){
				showFeedback('Ocurrió un error al intentar guardar. Por favor intenta más tarde.');
			})
			.always(function(r){
				console.log(r);
			})
		;
	});


	function showErrors(errors){
		hideErrors();
		var input;
		$.each(errors, function(k, v){
			input = modal.find('#input-' + k);
			input.parent().addClass('has-error');
			input.after('<p class="help-block error-feedback"><i class="glyphicon glyphicon-exclamation-sign"></i> ' + v + '<p>');
		});
	}
	
	function hideErrors(){
		modal.find('.has-error').removeClass('has-error');
		modal.find('.error-feedback').remove();
	}





	function showStats(id){
		stats.find('.stat').text('');
		stats.modal('show');
		var list = stats.find('tbody');
		list.html('<tr><td>Cargando...</td></tr>');
		var jsonData = {userId: id};
		$.ajax({
			url: settings.admin.user.stats,
			dataType: "json", method: 'POST',
			data: {data: JSON.stringify(jsonData)}
		})
			.done(function(response){
				if(response.status && response.status == 'success'){
					list.empty();
					$.each(response.data, function(i, c){
						var course = $('<tr><td>' + c.name + '</td><td>' + c.status + '</td><td class="text-center">' + (c.score==null ? '' : c.score) + '</td><td class="text-right">' + (c.totalTime==null ? '' : c.totalTime) + '</td><td><button class="btn btn-sm btn-danger"><i class="glyphicon glyphicon-floppy-remove"></i></button></td></tr>');
						list.append(course);
						course.find('button').on('click', function(e){
							clearScorm(id, c.id);
						});
					});
				}else{
					showFeedback('No se pudieron cargar los cursos del usuario.');
					stats.modal('hide');
				}
			})
			.fail(function(){
				showFeedback('No se pudieron cargar los cursos del usuario.');
				stats.modal('hide');
			})
			.always(function(r){
				console.log(r);
			})
		;
	}




	// Init everything _____________________________________________________________

	RDLMS.showFeedback = showFeedback;
	RDLMS.init(onLMSInitialized);



});