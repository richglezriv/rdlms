RDLMS = (function($){

	// Private vars
	var self = this,
		isInitialized = false,
		initSuccessCallback = null,
		initErrorCallback = null,
		user = null
	;
	
	// Public vars
	self.settings = null;




	function loadLMSSettings(){
		$.ajax({
			url: "configuration/lms-settings.json",
			dataType: "json", method: 'POST'
		})
			.done(onLMSSettingsLoaded)
			.fail(onLMSSettingsError)
			//.always()
		;
	}


	function onLMSSettingsError(){
		self.showFeedback('No se pudo cargar la configuración del LMS. Por favor, intenta más tarde.');
		console.error('Unable to load LMS settings. Check that configuration/lms-settings.json exists and is not malformed.');
		if(initErrorCallback) initErrorCallback();
	}


	function onLMSSettingsLoaded(response){
		isInitialized = true;
		self.settings = response;
		// TODO: Check if lms-settings has all the required attributes.
		setBranding(self.settings);
		if(initSuccessCallback) initSuccessCallback();
	}


	function showFeedback(message){
		console.log(message);
	}


	function setBranding(settings){
		if(settings.branding){
			var b = settings.branding;
			var h = $('#branded-header');
			if(b.headerCaption){
				h.find('.navbar-brand').hide();
				h.find('.navbar-branding').text(b.headerCaption);
			}
			if(b.headerImage){
				h.find('.navbar-brand').hide();
				h.find('.navbar-branding').prepend('<img src="'+b.headerImage+'" />');
			}
			if(b.headerHeight){
				h.find('.navbar-branding').css({
					lineHeight: b.headerHeight + 'px',
					fontSize: (b.headerHeight * 0.3) + 'px',
					height: b.headerHeight
				});
				h.find('.dropdown > a').css('padding', ((b.headerHeight-20) / 2) + 'px 15px');
				h.find('.navbar-toggle').css('margin', ((b.headerHeight-34) / 2) + 'px 15px');
				if(h.length) $('body').css('padding-top', b.headerHeight + 20);
			}
			if(b.headerCaptionColor){
				h.find('.dropdown > *, .navbar-brand, .navbar-branding').css('color', b.headerCaptionColor);
				h.find('.navbar-toggle').css('border-color', b.headerCaptionColor);
			}
			if(b.headerBackgroundColor) h.css({ background: b.headerBackgroundColor, border: 'none' });
			if(b.headerHideAccount) h.find('.navbar-toggle, .navbar-user').hide();
			if(b.courseListTitle) $('#course-list-title').html(b.courseListTitle);
			if(b.courseListIntro) $('#course-list-intro').html(b.courseListIntro);
		}
	}


	function initialize(onSuccess, onError){
		if(typeof(onSuccess) === 'function') initSuccessCallback = onSuccess;
		if(typeof(onError) === 'function') initErrorCallback = onError;
		loadLMSSettings();
	}

	function handleFailure(code){
		if(code === 'session-expired'){
			showFeedback('Tu sesión ha expirado');
			document.location.href = self.settings.logoutRedirect || 'login.html';
			return true;
		}
		if(code === 'admins-only'){
			document.location.href = 'courses.html';
			return true;
		}
		if(code === 'users-only'){
			document.location.href = 'admin-courses.html';
			return true;
		}
		return false;
	}

	function logout(){
		if(!isInitialized) return;
		$.ajax({
			url: self.settings.session.logout,
			dataType: "json", method: 'POST'
		})
			.done(function(response){
				if(response.status && response.status === 'success'){
					document.location.href = self.settings.logoutRedirect || 'login.html';
				}else{
					console.error('Could not logout.');
				}
			})
			.fail(function(){
				console.error('Could not logout: Invalid response from server.');
			})
			//.always()
		;
	}

	// Public methods
	self.init = initialize;
	self.showFeedback = showFeedback;
	self.handleFailure = handleFailure;
	self.logout = logout;
	self.isInitialized = function(){ return isInitialized; };


	return self;


})(jQuery);