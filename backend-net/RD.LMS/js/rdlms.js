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

	function validateSession(){
	    $.ajax({
	        url: settings.session.validate,
	        dataType: 'json'
	    })
            .done(function (response) {
                if (response.status == "success") {
                    console.info('sesion is still enabled');
                }
                else if (response.status == "fail"){
                    window.alert('no tiene una sesión activa');
                    window.location.href = 'login.html';
                }
            })
            .fail(console.error('unable to validate user session'))
	    ;
	}


	function loadLMSSettings(){
		$.ajax({
			url: "configuration/lms-settings.json",
			dataType: "json", method: 'GET'
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

	// Public methods
	self.init = initialize;
	self.showFeedback = showFeedback;
	self.isInitialized = function(){ return isInitialized; };
	self.validateSession = function(){ return validateSession(); };

	return self;


})(jQuery);