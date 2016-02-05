jQuery(function($){


	var settings = null,
		loading = false,
		body = $('body'),
		win = $(window),
		saveBtn = $('#reset-password-submit'),
		pwd = $('#input-password'),
		pwdBar = $('#pwd-bar')
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
		saveBtn.prop('disabled', true);
	}

	function stopLoading(){
		loading = false;
		body.css('cursor', '');
		saveBtn.prop('disabled', false);
	}




	// Courses loading ____________________________________________________________

	function onLMSInitialized(session){
		settings = RDLMS.settings;

		if(session.type != 'reset-password'){
			showFeedback('Tu contraseña ya ha sido cambiada o tu sesión ha caducado. Por favor, reinicia el proceso.');
			document.location.href = settings.session.logoutRedirect || 'login.html';
			return false;
		}
		saveBtn.on('click', function(e){ e.preventDefault(); saveNewPassword(); });
	}
	

	// Save user
	function saveNewPassword(){
		if(loading) return;
		startLoading();
		
		var jsonData = getFormData();

		var validationErrors = validateData(jsonData);
		if(validationErrors){
			showFeedback('Algunos de los datos que especificaste son inválidos. Por favor revisa los campos marcados.');
			stopLoading();
			return showErrors(validationErrors);
		}
	
		$.ajax({
			url: settings.session.resetPassword,
			dataType: "json", method: 'POST',
			data: {data: JSON.stringify(jsonData)}
		})
			.done(function(r){
				if(r.status && r.status == 'success'){
					stopLoading();
					showFeedback('Tu contraseña ha sido cambiada con éxito.');
					document.location.href = self.settings.logoutRedirect || 'login.html';
				}else if(r.status && r.status == 'fail' && r.data.reason == 'validation-error'){
					showFeedback('Algunos de los datos que especificaste son inválidos. Por favor revisa los campos marcados.');
					showErrors(r.data.fields);
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
	}

	function validateData(data){
		var errors = {};
		var patterns = {
			name: /^[^0-9!\@\#\$\%\*\+=\\\/\?<>\{\}\[\]:;]{2,60}$/i,
			email: /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i,
			birthday: /^((19[0-9]{2})|(20[0-2][0-9]))\-((0\d)|(1[0-2]))\-(([0-2]\d)|(3[0-1]))$/, //1900-2029
			gender: /^[mf]$/i,
			integer: /^\d+$/,
			bool: /^[01]$/,
			password: /^(?=.*?[A-Z])(?=(.*[a-z]){1,})(?=(.*[\d]){1,})(?=(.*[\W]){1,})(?!.*\s).{8,}$/
		}
		
		if(!(patterns.password).test(data.password)){
			errors.password = 'La contraseña no cumple con los lineamientos especificados.';
		}
		if(data.passwordCheck !== data.password){
			errors.passwordCheck = 'La confirmación de tu contraseña es diferente a tu contraseña.';
		}

		return $.isEmptyObject(errors) ? null : errors;
	}

	function getFormData(){
		var data = {
			password: $('#input-password').val(),
			passwordCheck: $('#input-passwordCheck').val()
		}
		return data;
	}

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


	pwd.on('keyup', function(e){
		var score = zxcvbn(this.value.substr(0,100)).score;
		var vals = [
			['10%', 'Débil', 'danger'],
			['50%', 'Media', 'warning'],
			['80%', 'Fuerte', 'success'],
			['100%', 'Muy fuerte', 'success'],
			['100%', 'Muy fuerte', 'success']
		];
		var v = vals[score];
		pwdBar.width(v[0]).text(v[1]).attr('class', 'progress-bar progress-bar-' + v[2]);
	});



	// Init everything _____________________________________________________________

	RDLMS.showFeedback = showFeedback;
	RDLMS.init(onLMSInitialized);



});