jQuery(function($){


	var settings = null,
		loading = false,
		body = $('body'),
		win = $(window),
		loginForm = $('#login-form').hide()
	;




	// Utils ______________________________________________________________________

	function htmlEntities(str){
		return String(str).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace("\n", '<br>');
	}




	// GUI helpers ________________________________________________________________

	function showFeedback(msg){
		//alert(msg);
		loginForm.find('.alert').remove();
		var alert = $('<div class="alert alert-warning alert-dismissible fade in" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span></button><i class="glyphicon glyphicon-exclamation-sign"></i> <span class="message"></span></div>');
		loginForm.children().first().prepend(alert);
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

	function onLMSInitialized(){
		settings = RDLMS.settings;
		loginForm.show();
	}





	loginForm.on('submit', function(e){
		e.preventDefault();
		if(loading) return;
		startLoading();
		
		var jsonData = {
			username: $('#input-username').val(),
			password: $('#input-password').val()
		};
		$.ajax({
			url: settings.session.login,
			dataType: "json", method: 'POST',
			data: {data: JSON.stringify(jsonData)}
		})
			.done(function(r){
				//r.status='fail'; r.data.reason='credentials-error';
				if(r.status && r.status == 'success'){
					document.location.href = r.data.isAdmin ? "admin-courses.html" : "courses.html";
				}else if(r.status && r.status == "fail" && r.data && r.data.reason == "credentials-error"){
					showFeedback('Nombre de usuario o contraseña incorrectos');
				}else if(r.status && r.status == "fail" && r.data && r.data.reason == "too-many-tries"){
					showFeedback('Demasiados intentos fallidos. El acceso al sistema se ha bloqueado por 60 minutos');
				}else{
					showFeedback('No se pudo establecer conexión con el servidor. Por favor, intenta más tarde.');
				}
			})
			.fail(function(){
				showFeedback('No se pudo establecer conexión con el servidor. Por favor, intenta más tarde.');
			})
			.always(function(r){
				console.log(r);
				stopLoading();
			})
		;
	});




	// Init everything _____________________________________________________________

	RDLMS.showFeedback = showFeedback;
	RDLMS.init(onLMSInitialized);



});