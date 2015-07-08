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
		if(initSuccessCallback) initSuccessCallback();
	}

	function showFeedback(message){
		console.log(message);
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


	return self;


})(jQuery);