jQuery(function($){


	var settings = null,
		loading = false,
		body = $('body'),
		win = $(window),
		saveBtn = $('#profile-submit'),
		cancelBtn = $('#profile-cancel'),
		pwd = $('#input-newPassword'),
		pwdBar = $('#pwd-bar'),
		already = $('#already')
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

		if(session.type != 'student' && session.type != 'admin'){
			RDLMS.handleFailure('session-expired');
			return false;
		}

		settings = RDLMS.settings;

		saveBtn.on('click', function(e){ e.preventDefault(); saveUser(); });
		cancelBtn.on('click', function(e){ e.preventDefault(); cancelUser(); });
		setFormData(session.user);

		$('#input-birthday').datetimepicker({
			locale: 'es',
			viewMode: 'years',
			format: 'YYYY-MM-DD',
			useCurrent: false,
			widgetPositioning: {
				horizontal: 'auto',
				vertical: 'bottom'
			}
		});

	}
	

	function cancelUser(){
		document.location.href = RDLMS.session.type == 'admin' ? 'admin-courses.html' : 'courses.html';
	}

	
	// Save user
	function saveUser(){
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
			url: settings.lms.profile,
			dataType: "json", method: 'POST',
			data: {
				data: JSON.stringify(jsonData),
				csrftoken: RDLMS.csrftoken
			}
		})
			.done(function(r){
				if(r.status && r.status == 'success'){
					stopLoading();
					showFeedback('Tus cambios han sido guardados con éxito.');
					document.location.href = RDLMS.session.type == 'admin' ? 'admin-courses.html' : 'courses.html';
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
		};
		
		if(!(patterns.name).test(data.name)){
			errors.name = 'El nombre proporcionado es inválido.';
		}
		if(!(patterns.name).test(data.lastName)){
			errors.lastName = 'El apellido paterno proporcionado es inválido.';
		}
		if(!(patterns.name).test(data.secondLastName)){
			errors.secondLastName = 'El apellido materno proporcionado es inválido.';
		}
		if(!(patterns.birthday).test(data.birthday)){
			errors.birthday = 'La fecha de nacimiento proporcionada es inválida.';
		}
		if(!(patterns.gender).test(data.gender)){
			errors.gender = 'Por favor, selecciona un género de la lista.';
		}
		if(!(patterns.integer).test(data.occupation)){
			errors.occupation = 'Por favor, selecciona una ocupación de la lista.';
		}
		if(!(patterns.integer).test(data.organization)){
			errors.organization = 'Por favor, selecciona una organización de la lista.';
		}
		if(data.newPassword !== ''){
			//console.log(data.newPassword);
			if(!(patterns.password).test(data.newPassword)){
				errors.newPassword = 'La contraseña no cumple con los lineamientos especificados.';
			}
			if(data.newPasswordCheck !== data.newPassword){
				errors.newPasswordCheck = 'La confirmación de tu contraseña es diferente a tu contraseña.';
			}
		}

		return $.isEmptyObject(errors) ? null : errors;
	}

	function getFormData(){
		var data = {
			name: $.trim($('#input-name').val()),
			lastName: $.trim($('#input-lastName').val()),
			secondLastName: $.trim($('#input-secondLastName').val()),
			birthday: $('#input-birthday').val(),
			gender: $.trim($('#input-gender').val()),
			occupation: $.trim($('#input-occupation').val()),
			organization: $.trim($('#input-organization').val()),
			oldPassword: $('#input-oldPassword').val(),
			newPassword: $('#input-newPassword').val(),
			newPasswordCheck: $('#input-newPasswordCheck').val(),
			captcha: $('#input-CaptchaInputText').val()
		};
		return data;
	}
	function setFormData(data){
		console.log(data);
		$.each([
			'name', 'lastName', 'secondLastName', 'email',
			'birthday', 'gender', 'occupation', 'organization'
		], function(i, v){
			$('#input-' + v).val(data[v]);
		});
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
