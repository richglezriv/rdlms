(function($, RDLMS){


	// Course Id & private vars

	var courseId = document.location.hash.substr(1), //LMS.SCO.courseId,
		commitURL = RDLMS.settings.sco.commit,
		fetchURL = RDLMS.settings.sco.fetch,
		scormPkgFolder = RDLMS.settings.sco.basePath
	;
		
	// Some caching...

	var loadingStr = document.title;
	var iframe = $(document.body).find('iframe');


	// SCO Settings & Data __________________________________________________________________

	function loadSCOSettings(courseId, fetchURL){
		console.log('Loading ' + fetchURL);
		var jsonData = {courseId: courseId};
		$.ajax({
			url: fetchURL,
			dataType: "json", method: 'POST',
			data: {
				data: JSON.stringify(jsonData),
				csrftoken: RDLMS.csrftoken
			}
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
		console.error('Could not load course settings.');
		onSCOClosed();
		window.close();
	}
	function onSCOSettingsLoaded(response){
		if(response.status === 'success'){
			courseId = response.data.id; // Again just in case
			document.title = response.data.name;
			var scosrc = scormPkgFolder + response.data.scoPath + '/' + response.data.scoIndex;
			console.log('Loading: ' + scosrc);
			iframe.attr('src', scosrc);
			window.API = new ScormApi({
				commitURL: commitURL,
				cmiData: response.data.dataModel,
				commitExtraData: {  // Additional POST variables that will be sent to the LMS
					csrftoken: RDLMS.csrftoken
				}
			});
			
			// Placed this code on <body onbeforeunload=""> for compatibility
			
			window.onbeforeunload = function(){
				//try{ window.API.LMSFinish(); }catch(err){ 
				//	opener.console.log('API.LMSFinish error:');
				//	opener.console.log(err);
				//};
				//try{ onSCOClosed(); }catch(err){
				//	opener.console.log('window.onSCOClosed error:');
				//	opener.console.log(err);
				//};
				API.LMSFinish();
				onSCOClosed();
			};

		}else{
			alert(response.message || 'No se pudo cargar la lección. Por favor intenta mas tarde.');
			console.log(response);
			onSCOClosed();
			window.close();
		}
	}




	// Initialize _____________________________________________________________________________

	loadSCOSettings(courseId, fetchURL);


})(jQuery, opener.RDLMS);