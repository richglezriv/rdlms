(function($, RDLMS){


	// Course Id & private vars

	var courseId = document.location.hash.substr(1), //LMS.SCO.courseId,
		commitURL = RDLMS.settings.sco.commit,
		fetchURL = RDLMS.settings.sco.fetch
	;
		
	// Some caching...

	var loadingStr = document.title,
		iframe = $(document.body).find('iframe')
	;

	var scormPkgFolder = 'scorm-packages'; // We'll later decide if this needs to on the config




	// SCO Settings & Data __________________________________________________________________

	function loadSCOSettings(courseId, fetchURL){
		console.log('Loading ' + fetchURL);
		var jsonData = {courseId: courseId};
		$.ajax({
			url: fetchURL,
			dataType: "json", method: 'POST',
			data: {data: JSON.stringify(jsonData)}
		})
			.done(onSCOSettingsLoaded)
			.fail(onSCOSettingsError)
			.always(function(r){
				if(document.title === loadingStr) document.title = '';
				console.log(r);
			})
		;
	}
	function onSCOSettingsError(){
		alert('No se pudo cargar la configuración del SCO. Por favor, intenta más tarde.');
		window.close();
	}
	function onSCOSettingsLoaded(response){
		if(response.status === 'success'){
			courseId = response.data.id; // Again just in case
			document.title = response.data.name;
			iframe.attr('src', scormPkgFolder + '/' + response.data.scoPath + '/' + response.data.scoIndex);
			window.API = new ScormApi({
				commitURL: commitURL,
				cmiData: response.data.dataModel
			});
			window.onbeforeunload = function(){
				window.API.LMSFinish();
			};
		}else{
			alert(response.message);
			window.close();
		}
	}




	// Initialize _____________________________________________________________________________

	loadSCOSettings(courseId, fetchURL);


})(opener.jQuery, opener.RDLMS);