/*
	SCORM 1.2 Reference
	http://scorm.com/scorm-explained/technical-scorm/run-time/run-time-reference/

	LMSInitialize(""):bool – Begins a communication session with the LMS.
	LMSFinish(""):bool – Ends a communication session with the LMS.
	LMSGetValue(element:CMIElement) :String – Retrieves a value from the LMS.
	LMSSetValue(element:CMIElement, value:String) : string – Saves a value to the LMS.
	LMSCommit(""):bool – Indicates to the LMS that all data should be persisted (not required).
	LMSGetLastError():CMIErrorCode – Returns the error code that resulted from the last API call.
	LMSGetErrorString(errorCode:CMIErrorCode):string – Returns a short string describing the specified error code.
	LMSGetDiagnostic(errorCode:CMIErrorCode):string – Returns detailed information about the last error that occurred.

	ScormApi
	@author: Victor Marquez
	@email: victmo@gmail.com
*/

(function ($) {
	

	window.ScormApi = function(options){
		var commitURL = options.commitURL;
		var cmiData = $.parseJSON(options.cmiData);//options.cmiData; RAGR: temp patch
		var isFinished = false;
		var isInitialized = false;
		var lastError = "0";
		var errorCodes = {"0": "No error", "101": "General Exception", "201": "Invalid argument error", "202": "Element cannot have children", "203": "Element not an array. Cannot have count.", "301": "Not initialized", "401": "Not implemented error", "402": "Invalid set value, element is a keyword", "403": "Element is read only.", "404": "Element is write only", "405": "Incorrect Data Type"};
		var startTime, endTime;

		var supportedData = {
			cmi: {
				suspend_data: {r: true, w: true, type: ['CMIString4096']},
				launch_data: {r: true, w: false, type: ['CMIString4096']},
				core: {
					student_id: {r: true, w: false, type: ['CMIIdentifier']},
					student_name: {r: true, w: false, type: ['CMIString255']},
					lesson_location: {r: true, w: true, type: ['CMIString255']},
					credit: {r: true, w: false, type: ['CMIVocabulary.Credit']},
					lesson_status: {r: true, w: true, type: ['CMIVocabulary.Status']},
					entry: {r: true, w: false, type: ['CMIVocabulary.Entry']},
					total_time: {r: true, w: false, type: ['CMITimespan']},
					exit: {r: false, w: true, type: ['CMIVocabulary.Exit']},
					session_time: {r: false, w: true, type: ['CMITimespan']},
					score: {	
						raw: {r: true, w: true, type: ['CMIDecimal', 'CMIBlank']},
						min: {r: true, w: true, type: ['CMIDecimal', 'CMIBlank']},
						max: {r: true, w: true, type: ['CMIDecimal', 'CMIBlank']}
					}
				},
				student_data: {
					mastery_score: {r: true, w: false, type: ['CMIDecimal']}
				}
			}
		};

		var dataTypes = {
			'CMIBlank': (/^$/),
			'CMIBoolean': (/^(true|false)$/),
			'CMIDecimal': (/^\-?\d*\.?\d+$/),
			'CMIIdentifier': (/^(\w|\d){1,255}$/),
			'CMIInteger': (/^\d{1,5}$/),
			'CMISInteger': (/^\-?\d{1,5}$/),
			'CMIString255': (/^(\n|.){0,255}$/),
			'CMIString4096': (/^(\n|.){0,4096}$/),
			'CMITime': (/^([0-1][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\.\d{1,2})?$/),
			'CMITimespan': (/^\d{1,4}:[0-5][0-9]:[0-5][0-9](\.\d{1,2})?$/),
			'CMIVocabulary.Credit': (/^(credit|no\-credit)$/),
			'CMIVocabulary.Status': (/^(passed|failed|completed|incomplete|browsed|not attempted)$/),
			'CMIVocabulary.Entry': (/^(ab\-initio|resume)?$/),
			'CMIVocabulary.Mode': (/^(browse|normal|review)$/),
			'CMIVocabulary.Exit': (/^(time\-out|suspend|logout)?$/)
		};

		function init() {
			lastError = "0";
			if(isInitialized || isFinished){ lastError = "101"; return "false"; }
			isInitialized = true;
			startTime = new Date();
			return "true";
		}

		function getValue(varName) {
			lastError = "0";
			if(!isInitialized || isFinished){ lastError = "301"; return "false"; }
			var err = '0', p;
			if(varName.substr(-10) === '._children'){
				p = getProp(supportedData, varName.substr(0,varName.length-10));
				if(p){
					if(p.hasOwnProperty('type')) err = "202";
					else return Object.keys(p).join(',');
				} else err = "401";
				lastError = err;
				return "";
			} else if(varName.substr(-7) === '._count'){
				lastError = "401";
				return "0";
			} else {
				p = getProp(supportedData, varName);
				if(p){
					if(p.r){
						return (cmiData[varName] || "");
					}
					else err = "404";
				} else err = "401";
				lastError = err;
				return "";
			}
		}

		function setValue(varName, varValue) {
			lastError = "0";
			if(!isInitialized || isFinished){ lastError = "301"; return "false"; }
			varValue = ""+varValue;
			var err = '0', match = false, p = getProp(supportedData, varName);
			if(p){
				if(p.w) $.each(p.type, function(i, dataType){
					if(dataTypes[dataType].test(varValue)){
						cmiData[varName] = varValue;
						match = true;
					} else err = "405";
				}); else err = "403";
			} else err = "401";
			if(match || err === "0") return "true";
			lastError = err; return "false";
		}

		function getLastError(){ return lastError; }
		function getDiagnostic(errorCode){ return getErrorString(errorCode) /*+ ' (diagnostics not yet implemented)'*/; }
		function getErrorString(errorCode){ return errorCodes[errorCode] || errorCodes["101"]; }

		function commit() {
			lastError = "0";
			if(!isInitialized || isFinished){ lastError = "301"; return "false"; }
			var cleanData = getSanitizedCmiData(cmiData);
			$.ajax({
				url: commitURL,	method: 'POST',
				data: {data: cleanData}
			})
				.done(function(response){
					console.log('Successfully commited data!');
					//console.log(cleanData);
					//console.log(response);
				})
				.fail(function(){
					console.error('Could not commit data!');
					//console.log(cleanData);
				})
			;
			if(false) {
				alert('Problem with AJAX Request in LMSCommit()');
				return "false";
			}
			return "true";
		}


		function finish() {
			lastError = "0";
			if((!isInitialized) || (isFinished)) return "false";
			var status = getValue('cmi.core.lesson_status');
			if(typeof(status) === 'undefined' || status === 'not attempted') setValue('cmi.core.lesson_status', 'incomplete');
			if($.inArray(status, ["passed", "failed", "completed"]) === -1) setValue('cmi.core.exit', 'suspend');
			endTime = new Date();
			status = setValue('cmi.core.session_time', milisecondsToHHMMSS(endTime - startTime));
			commit();
			isFinished = true;
			return "true";
		}


		function getSanitizedCmiData(cmiData){
			// Serialize only writable data
			var data = {}, p;
			$.each(cmiData, function(key, val){
				//console.log('Checking ' + key + ': ' + val);
				p = getProp(supportedData, key);
				if(p && p.w === true) data[key] = val; 
			});
			return data;
		}
		function hasProp(ref, names) {
			var name;
			if(typeof(names) === 'string') names = names.split('.');
			while((name = names.shift())){        
				if(!ref.hasOwnProperty(name)) return false;
				ref = ref[name];
			} 
			return true;
		}
		function getProp(ref, names) {
			var name;
			if(typeof(names) === 'string') names = names.split('.');
			while((name = names.shift())){        
				if(!ref.hasOwnProperty(name)) return undefined;
				ref = ref[name];
			} 
			return ref;
		}

		function milisecondsToHHMMSS(miliseconds){
			var totalSeconds = Math.round(miliseconds/1000);
			var hours = Math.floor(totalSeconds / 3600);
			var minutes = Math.floor((totalSeconds - (hours * 3600)) / 60);
			var seconds = totalSeconds - (hours * 3600) - (minutes * 60);        
			if (hours < 10) hours = "0" + hours;
			if (minutes < 10) minutes = "0" + minutes;
			if (seconds < 10) seconds = "0" + seconds;
			return hours + ':' + minutes + ':' + seconds;
		}




		return {
			LMSInitialize: init,
			LMSFinish: finish,
			LMSGetValue: getValue,
			LMSSetValue: setValue,
			LMSCommit: commit,
			LMSGetLastError: getLastError,
			LMSGetErrorString: getErrorString,
			LMSGetDiagnostic: getDiagnostic
		};
	};



}(opener.jQuery)); // jQuery is required
