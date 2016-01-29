jQuery(function($){


	var settings = null,
		loading = false,
		body = $('body'),
		win = $(window),
		list = $('.list.courses'),
		add = $('#add-new'),
		modal = $('#course-modal'),
		stats = $('#stats-modal'),
		conditionsInput = $('#input-conditions'),
		uploadThumb, uploadScorm,
		courses = null
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




	// Courses loading ____________________________________________________________

	function onLMSInitialized(session){
		//alert('admin-courses: ' + session.type);
		if(session.type != 'admin'){
			if(session.type == 'student') RDLMS.handleFailure('admins-only');
			else RDLMS.handleFailure('session-expired');
			return false;
		}

		// Init
		add.on('click', function(e){ e.preventDefault(); if(!loading) addCourse(); });
		uploadThumb = new SimpleUploader('#input-thumbnail', { 
			uploadPath: RDLMS.settings.lms.uploadPath, 
			action: RDLMS.settings.admin.course.uploadThumb, 
			type: 'image',
			data: { csrftoken: RDLMS.csrftoken }
		});
		uploadScorm = new SimpleUploader('#input-scorm', {
			uploadPath: RDLMS.settings.lms.uploadPath, 
			action: RDLMS.settings.admin.course.uploadScorm, 
			type: 'file',
			data: { csrftoken: RDLMS.csrftoken }
		});
		
		// Fetch courses
		settings = RDLMS.settings;
		fetchCourses();
	}


	function fetchCourses(){
		if(loading) return;
		startLoading();
		$.ajax({
			url: settings.admin.course.list,
			dataType: 'json', method: 'POST',
			data: {
				csrftoken: RDLMS.csrftoken
			}
		})
			.done(function(response){
				if(response.status && response.status === 'success'){
					list.empty();
					conditionsInput.empty();
					courses = response.data;
					if(courses.length){
						$.each(courses, function(i, course){
							list.append(createCourse(
								course.id,
								course.name,
								course.description,
								course.thumbnail
							));
							conditionsInput.append('<option value="' + course.id + '">' + course.name + '</option>');
						});
					}else{
						list.append('<div><h4 class="text-info">No hay ningún curso en el sistema.</h4><p>Haz clic en "Agregar curso" para crear un nuevo curso.</p></div>');
					}
				}else if(response.status && response.status === 'fail' && response.data.reason){
					var failResult = RDLMS.handleFailure(response.data.reason);
					if(!handleFailure) showFeedback('No fue posible cargar la lista de cursos. Ocurrió una falla con el servidor');
					console.error(response.data.reason);
				}else{
					showFeedback('No fue posible cargar la lista de cursos. Ocurrió un error al comunicarse con el servidor');
				}
			})
			.fail(function(){ showFeedback('No fue posible cargar la lista de cursos'); })
			.always(stopLoading)
		;
	}


	function createCourse(id, name, description, thumb, status, time, active){
		var course = $('<div class="media course"><div class="media-left"><img class="media-object" alt="" /></div><div class="media-body"/></div>')
			//.addClass(active?'active':'not-active')
		;
		course.data('data', {id:id, name:name, description:description, thumb:thumb, status:status, time:time, active:active});
		course.find('.media-left > img').attr('src', RDLMS.settings.lms.uploadPath + '/' + thumb);
		course.find('.media-body')
			.append('<h4 class="media-heading">' + htmlEntities(name) + '</h4>')
			.append('<div class="actions"><button class="btn btn-default btn-sm btn-stats" aria-label="Estadísticas"><span class="glyphicon glyphicon-stats" aria-hidden="true"></span></button> <button class="btn btn-default btn-sm btn-edit" aria-label="Editar"><span class="glyphicon glyphicon-pencil" aria-hidden="true"></span></button> <button class="btn btn-danger btn-sm btn-delete" aria-label="Borrar"><span class="glyphicon glyphicon-trash" aria-hidden="true"></span></button></div>')
			.append('<div class="description">' + htmlEntities(description) + '</div>')
		;
		course.on('click', '.btn-stats', function(e){ showStats(course); });
		course.on('click', '.btn-edit', function(e){ editCourse(course); });
		course.on('click', '.btn-delete', function(e){ deleteCourse(course); });
		return course;
	}


	function editCourse(c){
		if(loading) return;
		showModal(c.data('data').id);
	}


	function addCourse(){
		if(loading) return;
		showModal();
	}


	function deleteCourse(c){
		if(loading) return;
		if(confirm("Esta acción borrará el curso y toda la información asociada a éste, incluyendo calificaciones y avance de los usuarios.\n¿Estás seguro que deseas continuar?")){
			c.css('background-color', '#fffaee').fadeOut();
			var jsonData = {courseId: c.data('data').id};
			$.ajax({
				url: settings.admin.course.delete,
				dataType: "json", method: 'POST',
				data: {
					data: JSON.stringify(jsonData),
					csrftoken: RDLMS.csrftoken
				}
			})
				.done(function(r){
					if(!r.status || r.status != 'success') cancelDeleteCourse(c);
				})
				.fail(function(){
					cancelDeleteCourse(c);
				})
				.always(function(r){
					console.log(r);
				})
			;
		}
	}
	function cancelDeleteCourse(c){
		showFeedback('No se pudo borrar el curso: "' + c.data('data').name + '"');
		c.fadeIn(function(){ c.css('background-color', ''); });
	}


	function showModal(id){
		course = $.grep(courses, function(o){ return o.id == id; })[0] || null;
		
		// Clean modal
		hideErrors();
		modal.data('id', null);
		modal.find('input, textarea').val('');
		modal.find('select').val([]);
		uploadThumb.val('');
		uploadScorm.val('');
		modal.find('.modal-title').text('Nuevo curso');
		modal.find('.help-block').eq(2).hide();
		
		// Load course info into modal (when id is provided)
		if(course){
			modal.data('id', course.id);
			modal.find('#input-name').val(course.name);
			modal.find('#input-description').val(course.description);
			uploadThumb.val(course.thumbnail);
			uploadScorm.val(course.scorm);
			modal.find('#input-conditions').val(course.conditions);
			modal.find('.modal-title').text('Editar curso');
			modal.find('.help-block').eq(2).show();
			modal.find('#input-conditions option')
				.show()
				.filter('[value="' + course.id + '"]')
				.prop('selected', false)
				.hide()
			;
		}
		modal.modal('show');
	}


	// Cancel modal
	modal.find('.btn-cancel').on('click', function(e){
		e.preventDefault();
		modal.modal('hide');
	});
	

	// Save modal
	modal.find('.btn-save').on('click', function(e){
		e.preventDefault();
		if(loading) return;
		startLoading();
		
		var jsonData = getFormData();

		var validationErrors = validateData(jsonData);
		if(validationErrors){
			showFeedback('Algunos de los datos que especificaste son inválidos. Por favor revisa los campos marcados.');
			stopLoading();
			showErrors(validationErrors);
			return false;
		}

		if(modal.data('id')) jsonData.courseId = modal.data('id');
		
		$.ajax({
			url: settings.admin.course.save,
			dataType: "json", method: 'POST',
			data: {
				data: JSON.stringify(jsonData), 
				csrftoken: RDLMS.csrftoken
			}
		})
			.done(function(r){
				if(r.status && r.status == 'success'){
					modal.modal('hide');
					stopLoading();
					fetchCourses();
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
				stopLoading();
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

	function getFormData(){
		var data = {
			name: modal.find('#input-name').val(),
			description: modal.find('#input-description').val(),
			thumbnail: uploadThumb.val(),
			scorm: uploadScorm.val(),
			conditions: modal.find('#input-conditions').val() || []
		};
		return data;
	}

	function validateData(data){
		var errors = {};
		var patterns = {
			notEmpty: /[^\s]+/,
			name: /^[^0-9!\@\#\$\%\*\+=\\\/\?<>\{\}\[\]:;]{2,60}$/i,
			email: /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i,
			birthday: /^((19[0-9]{2})|(20[0-2][0-9]))\-((0\d)|(1[0-2]))\-(([0-2]\d)|(3[0-1]))$/, //1900-2029
			gender: /^[mf]$/i,
			integer: /^\d+$/,
			bool: /^[01]$/,
			password: /^(?=.*?[A-Z])(?=(.*[a-z]){1,})(?=(.*[\d]){1,})(?=(.*[\W]){1,})(?!.*\s).{8,}$/
		};
		
		if(!(patterns.notEmpty).test(data.name)){
			errors.name = 'El nombre proporcionado es inválido.';
		}
		if(!(patterns.notEmpty).test(data.description)){
			errors.description = 'La descripción proporcionada es inválida.';
		}
		if(!(patterns.notEmpty).test(data.scorm)){
			errors.scorm = 'Por favor agrega un paquete SCORM para continuar.';
		}

		return $.isEmptyObject(errors) ? null : errors;
	}





	function showStats(c){
		if(loading) return;
		stats.find('.stat').text('');
		stats.find('#stats-modal-title').text(c.data('data').name);
		stats.modal('show');

		var jsonData = {courseId: c.data('data').id};
		$.ajax({
			url: settings.admin.course.stats,
			dataType: "json", method: 'POST',
			data: {
				data: JSON.stringify(jsonData), 
				csrftoken: RDLMS.csrftoken
			}
		})
			.done(function(response){
				if(response.status && response.status == 'success'){
					var r = response.data;
					var total = r.passed + r.failed + r.completed + r.incomplete + r.notAttempted;

					stats.find('tr.total .stat').text(numeral(total).format('0,0'));
					stats.find('tr.passed .stat').text(numeral(r.passed).format('0,0'));
					stats.find('tr.failed .stat').text(numeral(r.failed).format('0,0'));
					stats.find('tr.completed .stat').text(numeral(r.completed).format('0,0'));
					stats.find('tr.incomplete .stat').text(numeral(r.incomplete).format('0,0'));
					stats.find('tr.notAttempted .stat').text(numeral(r.notAttempted).format('0,0'));

					stats.find('tr.total .percentage').text(numeral(total/total).format('0.00%'));
					stats.find('tr.passed .percentage').text(numeral(r.passed/total).format('0.00%'));
					stats.find('tr.failed .percentage').text(numeral(r.failed/total).format('0.00%'));
					stats.find('tr.completed .percentage').text(numeral(r.completed/total).format('0.00%'));
					stats.find('tr.incomplete .percentage').text(numeral(r.incomplete/total).format('0.00%'));
					stats.find('tr.notAttempted .percentage').text(numeral(r.notAttempted/total).format('0.00%'));
				}else{
					showFeedback('No se pudieron cargar los datos del curso.');
					stats.modal('hide');
				}
			})
			.fail(function(){
				showFeedback('No se pudieron cargar los datos del curso.');
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