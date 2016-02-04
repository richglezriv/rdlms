// XFS client protection...
if(window.self !== window.top && document.referrer.indexOf(document.location.origin) !== 0){ 
	var XFSMessage = 'Por razones de seguridad, el contenido de esta página no puede ser visto en un frame.';
	alert(XFSMessage); 
	window.top.location = document.location;
	throw new Error(XFSMessage); // prevents further execution
}


window.RDLMS = new (function($){

	// Private vars
	var self = this,
		isInitialized = false,
		initSuccessCallback = null,
		initErrorCallback = null,
		user = null
	;
	
	// Public vars
	self.settings = null;
	self.csrftoken = Cookies.get('__token') || null;


	function loadLMSSettings(){
		$.ajax({
			url: "configuration/lms-settings.json",
			dataType: "json", method: 'GET',
			cache: false
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
		loadLMSUser();
	}


	function loadLMSUser(){
		$.ajax({
			url: self.settings.session.user,
			dataType: "json", method: 'POST',
			cache: false, // No necessary but just in case (IE)
			data: {
				csrftoken: self.csrftoken
			}
		})
			.done(onLMSUserLoaded)
			.fail(onLMSUserError)
			//.always()
		;
	}


	function onLMSUserError(){
		self.showFeedback('No se pudo cargar la información de la sesión. Por favor, intenta más tarde.');
		console.error('Unable to load session info.');
		if(initErrorCallback) initErrorCallback();
	}


	function onLMSUserLoaded(response){
		if(response.status && response.status === 'success'){
			var session = {
				type: response.data.sessionType,
				user: response.data.user
			};
		
			self.session = session;
			setBranding(self.settings, self.session);
			$(window).on('hashchange', onHashChange);
			
			if(initSuccessCallback) initSuccessCallback(session); // sending `sessionType` and `user`
		
		}else{
			onLMSUserError();
		}
	}


	function onHashChange(e){
		var hash = location.hash;
		if(hash === '#lms-logout'){
			logout();
		}
		if(hash === '#lms-profile'){
			location.href = 'profile.html';
		}
		location.hash = '';
	}


	function showFeedback(message){
		console.log(message);
	}


	function setBranding(settings){
		var left = -1;
		if(settings.branding){
			var hi = $('.logged-in #branded-header');
			if(hi.length) hi.load(settings.branding.headerLoggedIn || '_header-logged-in.html', paintBrandingVariables);
			
			var ho = $('.logged-out #branded-header');
			if(ho.length) ho.load(settings.branding.headerLoggedOut || '_header-logged-out.html');
			
			var f = $('#branded-footer');
			if(f.length && settings.branding.footer) f.load(settings.branding.footer);
		}
	}


	function paintBrandingVariables(){
		if(!self.session.user) return;
		$('.lms-user-name').text(self.session.user.name);
		$('.lms-user-lastName').text(self.session.user.lastName);
		$('.lms-user-secondLastName').text(self.session.user.secondLastName);
	}


	function initialize(onSuccess, onError){
		if(typeof(onSuccess) === 'function') initSuccessCallback = onSuccess;
		if(typeof(onError) === 'function') initErrorCallback = onError;
		loadLMSSettings();
	}

	function handleFailure(code){
		//alert(code);
		if(code === 'session-expired'){
			showFeedback('Tu sesión ha expirado');
			//alert('Redirecting to login...');
			document.location.href = self.settings.session.logoutRedirect || 'login.html';
			return true;
		}
		if(code === 'admins-only'){
			//alert('Redirecting to student module...');
			document.location.href = 'courses.html';
			return true;
		}
		if(code === 'students-only'){
			//alert('Redirecting to admin module...');
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
					Cookies.remove('__token');
					document.location.href = self.settings.session.logoutRedirect || 'login.html';
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

	function ping(){
		$.ajax({
			url: self.settings.session.ping,
			dataType: "json", method: 'POST',
			data: {
				csrftoken: self.csrftoken
			}
		})
			.done(function(response){
				if(!response.status || response.status !== 'success'){
					console.error('Server responded with an error when pinging.');
					// TODO: Logout?
				}
			})
			.fail(function(){
				console.error('Could not ping server.');
			})
		;
	}

	function delayedRedirect(uri, milliseconds){
		milliseconds = milliseconds || 2000;
		setTimeout(function(){
			window.location = uri;
		}, milliseconds);
	}

	// Public methods
	self.init = initialize;
	self.showFeedback = showFeedback;
	self.handleFailure = handleFailure;
	self.logout = logout;
	self.ping = ping;
	self.delayedRedirect = delayedRedirect;
	self.isInitialized = function(){ return isInitialized; };


	return self;


})(jQuery);