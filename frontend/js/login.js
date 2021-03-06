jQuery(function($){


	var settings = null,
		loading = false,
		body = $('body'),
		win = $(window),
		loginForm = $('#login-form').hide(),
		forgotForm = $('#forgot-form').hide(),
		registrationForm = $('#registration-form').hide(),
		forgotBtns = $('.forgot-btn')
	;




	// Utils ______________________________________________________________________

	function htmlEntities(str){
		return String(str).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace("\n", '<br>');
	}




	// GUI helpers ________________________________________________________________

	function showLoginFeedback(msg){
		//alert(msg);
		loginForm.find('.alert').remove();
		var alert = $('<div class="alert alert-warning alert-dismissible fade in" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span></button><i class="glyphicon glyphicon-exclamation-sign"></i> <span class="message"></span></div>');
		loginForm.children().first().prepend(alert);
		alert.find('.message').text(msg);
	}
	function showForgotFeedback(msg){
		//alert(msg);
		forgotForm.find('.alert').remove();
		var alert = $('<div class="alert alert-warning alert-dismissible fade in" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span></button><i class="glyphicon glyphicon-exclamation-sign"></i> <span class="message"></span></div>');
		forgotForm.children().first().prepend(alert);
		alert.find('.message').text(msg);
	}

	function startLoading(){
		loading = true;
		body.css('cursor', 'progress');
		$('.btn').prop('disabled', true);
	}

	function stopLoading(){
		loading = false;
		body.css('cursor', '');
		$('.btn').prop('disabled', false);
	}




	// Courses loading ____________________________________________________________

	function onLMSInitialized(session){

		if(session.type == 'student') return RDLMS.handleFailure('students-only');
		if(session.type == 'admin') return RDLMS.handleFailure('admins-only');

		settings = RDLMS.settings;
		loginForm.show();
		if(settings.lms.registration) registrationForm.show();

		forgotBtns.on('click', function(e){
			e.preventDefault();
			var hash = $(this).attr('href');
			if(hash == '#forgot'){
				loginForm.hide();
				forgotForm.show();
			}
			if(hash == '#login'){
				loginForm.show();
				forgotForm.hide();
			}
		});
	}





	loginForm.on('submit', function(e){
		e.preventDefault();
		if(loading) return;
		startLoading();
		
		var jsonData = {
			email: $('#input-email').val(),
			password: $('#input-password').val()
		};
		$.ajax({
			url: settings.session.login,
			dataType: "json", method: 'POST',
			data: {
				data: JSON.stringify(jsonData)
			}
		})
			.done(function(r){
				//r.status='fail'; r.data.reason='credentials-error';
				if(r.status && r.status == 'success'){
					if(!r.data.csrftoken){
						showLoginFeedback('No se pudo crear una sesión en el servidor. Por favor intenta más tarde.');
					}else{
						Cookies.set('__token', r.data.csrftoken);
						document.location.href = r.data.isAdmin ? "admin-courses.html" : "courses.html";
					}
				}else if(r.status && r.status == "fail" && r.data.reason === 'validation-error'){
					showErrors(r.data.fields);
				}else if(r.status && r.status == "fail" && r.data && r.data.reason == "credentials-error"){
					showLoginFeedback('Nombre de usuario o contraseña incorrectos');
				}else if(r.status && r.status == "fail" && r.data && r.data.reason == "too-many-tries"){
					showLoginFeedback('Demasiados intentos fallidos. Tu acceso se ha bloqueado por una hora.');
				}else{
					showLoginFeedback('No se pudo establecer conexión con el servidor. Por favor, intenta más tarde.');
				}
			})
			.fail(function(){
				showLoginFeedback('No se pudo establecer conexión con el servidor. Por favor, intenta más tarde.');
			})
			.always(function(r){
				console.log(r);
				stopLoading();
			})
		;
	});




	forgotForm.on('submit', function(e){
		e.preventDefault();
		if(loading) return;
		startLoading();
		
		var jsonData = {
			username: $('#input-email').val()
		};
		$.ajax({
			url: settings.session.forgot,
			dataType: "json", method: 'POST',
			data: {data: JSON.stringify(jsonData)}
		})
			.done(function(r){
				//r.status='fail'; r.data.reason='credentials-error';
				if(r.status){
					if(r.status === 'fail' && r.data.reason === 'validation-error'){
						showErrors(r.data.fields);
					}
					showForgotFeedback('Te hemos enviado un correo con los siguientes pasos.');
				}else{
					showForgotFeedback('Ocurrió un error al intentar procesar tu solicitud, por favor intenta mas tarde.');
				}
			})
			.fail(function(){
				showForgotFeedback('No se pudo establecer conexión con el servidor. Por favor, intenta más tarde.');
			})
			.always(function(r){
				console.log(r);
				stopLoading();
			})
		;
	});



	function showErrors(errors){
		hideErrors();
		var input;
		$.each(errors, function(k, v){
			input = $('#input-' + k);
			input.parent().addClass('has-error');
			input.after('<p class="help-block error-feedback"><i class="glyphicon glyphicon-exclamation-sign"></i> ' + v + '<p>');
		});
	}
	
	function hideErrors(){
		$('.has-error').removeClass('has-error');
		$('.error-feedback').remove();
	}




	// Init everything _____________________________________________________________

	RDLMS.showFeedback = function(msg){
		// Si recibimos un mensaje en general del servidor, lo mistramos
		// en ambos formularios.
		showLoginFeedback(msg);
		showForgotFeedback(msg);
	};

	RDLMS.init(onLMSInitialized);



});