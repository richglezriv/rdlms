// Load query string vars before I do anything else....
//window.$_GET;
//(window.onpopstate = function(){
//	var match, pl = /\+/g, search = /([^&=]+)=?([^&]*)/g, decode = function(s){ return decodeURIComponent(s.replace(pl, " ")); }, query  = window.location.search.substring(1);
//	$_GET = {}; while (match = search.exec(query)) $_GET[decode(match[1])] = decode(match[2]);
//})();


jQuery(function($){
	
	// Mobile detect _______________________________________________________________________________________
	
	var isMobile = (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))?true:false;
	console.log('Device IS' + (isMobile ? ' ' : ' NOT ') + 'mobile');





	// Private vars _______________________________________________________________________________________

	var win = $(window);
	var title = $('#title');
	var topic = $('#topic');
	var slideContainer = $('#slide');
	var prev = $('#prev-btn');
	var next = $('#next-btn');
	var overlay = $('#overlay');
	var temario = $('#temario');
	//var glosario = $('#glosario');
	//var guia = $('#guia');
	var currSlide = null;
	var maxSlide = 0;
	var courseCompleted = false;
	var slides = null;
	var slidesLen = 0;
	var currentComposition = null;
	var currNarration = null;
	var playOverlay = $('#play-overlay').hide();
	var narrationAudio = $('#narration');
	var narrationTimeout = null;
	var completeTimer = null;

	var audioLoaded = false;
	var slideLoaded = false;
	var slideTimingEnd = null;
	var slideTimingStart = null;
	
	var token = null;
	var endpoint = '/WSReaccionDigitalELearning/WSReaccionDigitalELearning.asmx/';
	var playerBasePath = document.location.protocol + '//' + document.location.host + document.location.pathname.split('/').slice(0,-1).join('/');

	var slideBgColor = slideContainer.css('background-color');

	var config = {}; // Stores SCO configuration




	// Tracking API _____________________________________________________________
	
	var popupTitles = {'#glosario':'glosario', '#temario':'temario', '#guia':'guia'};
	function slideFullTitle(n){ return 'Slide ' + (n+1) + ': ' + slides[n].title; }
	
	function track(ops){ // GA Send high level interface ;)
		var verboseOutput = JSON.stringify(ops);
		if(ga){ 
			ga('send', ops);
			console.log('Tracked ' + ops.hitType + ': ' + verboseOutput); 
		}else{ console.log('GA not initialized: ' + verboseOutput); }
	}
	function trackPageView(page, title){ var ops = { hitType: 'pageview', page: page, title: title }; track(ops); }
	function trackEvent(category, action, label, value){ var ops = { hitType: 'event', eventCategory: category, eventAction: action }; if(typeof(value) != 'undefined') ops.eventValue = value; if(typeof(label) != 'undefined') ops.eventLabel = label; track(ops); }
	function trackTiming(category, action, label, value){ var ops = { hitType: 'timing', timingCategory: category, timingVar: action, timingLabel: label, timingValue: value }; track(ops); }





	// Unsupported browser ______________________________________________________________________________

	function showBrowserWarning(){
		$('#controls').hide();
		prev.hide(); next.hide(); 
		slideContainer.attr('src', "unsupported.html");
		// Shall I track an unsupported browser event?
	}





	// Server API _______________________________________________________________________________________
	// open /Applications/Google\ Chrome.app --args --disable-web-security

	function ws(options){ // options: *method, data, success, error
		if(typeof(options.success) !== "function") options.success = function(r){ console.log(r); };
		if(typeof(options.error) !== "function") options.error = function(r){ console.error(r); };
		$.ajax({ 
			url: (endpoint + options.method), data: (JSON.stringify((options.data || {}))), type: "POST", dataType: "json", contentType: "application/json; charset=utf-8", 
			success: function(r){ if(typeof(r.d != 'undefined') && r.d.substr(0,3) === '0. ') options.error(r.d.substr(3)); else options.success(r.d || r); },
			error:  function(){ options.error(null); }
		});
	}
	
	function wsGetMaxSlide(s, e){ ws({ method: 'GetMaxSlide', data: {pToken: token}, success: s, error: e }); }
	function wsSetMaxSlide(slide, s, e){ ws({ method: 'SetMaxSlide', data: {pToken: token, pSlideNumber: slide}, success: s, error: e }); }	
	function wsGetCurrentSlide(s, e){ ws({ method: 'GetCurrentSlide', data: {pToken: token}, success: s, error: e }); }
	function wsSetCurrentSlide(slide, s, e){ ws({ method: 'SetCurrentSlide', data: {pToken: token, pSlideNumber: slide}, success: s, error: e }); }	
	function wsGetFeedBack(module, s, e){ ws({ method: 'GetFeedBack', data: {pToken: token, pModulo: module}, success: s, error: e }); }
	function wsSetFeedBack(module, score, s, e){ ws({ method: 'SetFeedBack', data: {pToken: token, pModulo: module, pCalificacion: score}, success: s, error: e }); }
	function wsSetEndingUsr(ended, s, e){ ws({ method: 'SetEndingUsr', data: {pToken: token, pFinalizado: (ended ? '1' : '0')}, success: s, error: e }); }
	function logOut(){
		console.log('Logging out...');
		//document.location.href = './../e-learning.aspx';
	}

	function setProgress(current, max){
		if(typeof(current) != 'number') current = currSlide;
		if(typeof(max) != 'number') max = maxSlide;
		if(current > max) max = current;
		currSlide = current;
		maxSlide = max;
		
		console.log('Progress saved: ' + current + ' - ' + max);
		wsSetCurrentSlide(current);
		wsSetMaxSlide(max);
		if(maxSlide >= slidesLen) markAsComplete();
		
		renderArrows();
	}
	
	function fetchProgress(){
		wsGetMaxSlide(function(max){
			if((/\d+/).test(max)){
				console.log('Max slide fetched: ' + max);
				maxSlide = ~~max;
				if(maxSlide >= slidesLen) courseCompleted = true;
			}else{
				console.error("Max slide defaulted to zero. Server returned invalid data: \n" + max);
				maxSlide = 0;
			}
			
			wsGetCurrentSlide(function(current){
				if((/\d+/).test(current)){
					console.log('Current slide fetched: ' + current);
					showSlide(~~current); // currSlide is set here
				}else{
					console.error("Current slide defaulted to zero. Server returned invalid data: \n" + current);
					showSlide(0); // currSlide is set here
				}
			}, function(){
				if(response && response.substr(0,6) === 'No fue') return logOut();
				console.warn('Current slide error, defaulted to zero');
				showSlide(0); // currSlide is set here
			});
		
		}, function(response){
			if(response && response.substr(0,6) === 'No fue') return logOut();
			console.warn('Could not fetch progress, starting from the beginning');
			showSlide(0);
		});
	}

	function saveScore(module, score){
		if(typeof(module) !== 'number') return console.error('Incorret score input');
		score = ~~score;
		console.log('Score saved: ' + module + ' - ' + score);
		wsSetFeedBack((module + 1), score);
	}

	function markAsComplete(){
		if(courseCompleted){
			console.log('Course already marked as completed');
			return;
		}
		console.log('Course marked as completed!');
		wsSetEndingUsr(true);
		trackEvent('elearning_temas', 'send', 'curso_completado_send_open');
		courseCompleted = true;
	}





	// Player functions __________________________________________________________________________________
	
	function loadSettings(){
		if(browser.sucks) return showBrowserWarning();
		
		token = document.location.hash.substr(1);
		document.location.hash = '';
		
		//token += (('at' in $_GET) ? $_GET['at'] : ''); // Query string based token, we decided to use hash
		
		//if(token === '') return logOut();
		console.log('Current session: ' + token);
		$.ajax({
			url: "sco-settings.json",
			dataType: "json",
			error: settingsError,
			success: settingsLoaded
		});

		trackEvent('elearning_temas', 'open', 'home_ventana_open');
	}

	function settingsError(){
		alert('No se pudo cargar la configuración del curso. Por favor intente más tarde.');
		logOut();
	}


	function settingsLoaded(response){
		if(response.settings && response.slides){
			title.text(response.settings.courseName);
			slides = response.slides;
			slidesLen = slides.length;
			if(slidesLen === 0) return settingsError();
			generateTOC();
			fetchProgress();
		}else{
			settingsError();
		}
	}


	function generateTOC(){
		var item, list = $('<ol />');
		$.each(slides, function(i,slide){
			item = $('<li><a href="#">' + slide.title + '</a></li>');
			list.append(item);
			item.on('click', 'a', {i: i}, function(e){
				e.preventDefault();
				if(e.data.i <= maxSlide || isRDA()){
					hidePopups();
					showSlide(e.data.i);
				}
			});
		});
		temario.find('.content').empty().append(list);
	}


	function updateTOC(){
		temario.find('.content li').each(function(i){
			$(this).css('opacity', (i > maxSlide) ? 0.3 : 1);
		});
	}


	function removeSlides(indexes){
		indexes.sort(function(a, b){return b-a;}); //descending
		for(var i = 0, len = indexes.length; i < len; i++){
			slides.splice(indexes[i], 1);
		}
		generateTOC();
		updateTOC();
	}


	function hidePopups(){
		overlay.hide();
		$('.popup').hide();
	}


	function showPopup(id){
		var popup = $(id);
		if(popup.length){
			overlay.show();
			popup.fadeIn('fast');
			updateTOC();
			
			// Tracking
			trackEvent(
				'elearning_temas', 
				'open', 
				'navegacion_' + popupTitles[id] + '_boton_click'
			);
		}
	}


	function EDGE_initComposition(compId){ // Check end of timeline on Edge
		var Edge = slideContainer[0].contentWindow.AdobeEdge, Composition = Edge.Composition, Symbol = Edge.Symbol;
		Symbol.bindTimelineAction(compId, "stage", "Default Timeline", "complete", function(sym, e){
			if(e.elapsed){
				console.log('Edge animation completed!');
				if(typeof(slide.completeAfter) !== 'number') onSlideEnded();
			}
		});
		slideLoaded = true;
		syncNarration();
	}
	
	function FLASHCANVAS_onTick(event) { // Check end of timeline on Flash Canvas
		var exportRoot = event.currentTarget;
		if(exportRoot.timeline.position >= exportRoot.timeline.duration-1){
			exportRoot.removeEventListener("tick", FLASHCANVAS_onTick);
			console.log('Flash Canvas animation completed!');
			if(typeof(slide.completeAfter) !== 'number') onSlideEnded();
		}
	}


	function syncNarration(){
		if(isMobile && currNarration !== null){
			//alert('audio: ' + audioLoaded + ' - slide: ' + slideLoaded);
			if(audioLoaded && slideLoaded){
				
				var slide = slides[currSlide];
				if(typeof(slide.mobileNarrationDelay) === "number" && slide.mobileNarrationDelay > 0){
					narrationTimeout = setTimeout(function(){
						narrationAudio.play();
						narrationAudio.currentTime = 0;
						narrationTimeout = null;
					}, slide.mobileNarrationDelay * 1000);
				

				}else{
					narrationAudio.play();
					narrationAudio.currentTime = 0;
				}
			}
		}
	}


	function destroyNarration(){
		if(narrationAudio && narrationAudio.pause){
			narrationAudio.pause();
			//delete(narrationAudio);
			narrationAudio = null;
		}
	}


	function showSlide(n){
		n = parseInt(n);
		console.log('Loading slide ' + n);
		if(n >= slidesLen || n < 0 || n === currSlide) return;
		
		// Tracking
		slideTimingEnd = new Date().getTime();
		
		if(slideTimingStart !== null) trackTiming(
			'elearning_temas', 
			'duration', 
			'duracion_diapositiva_' + (currSlide + 1), 
			Math.round((slideTimingEnd - slideTimingStart) / 1000)
		);
		
		slideTimingStart = new Date().getTime();
		
		var slide = slides[n], max;
		if(completeTimer !== null){
			clearTimeout(completeTimer);
			completeTimer = null;
		}
		slideContainer.attr('src', ((/^(https?:)?\/\//).test(slide.filename)?'':playerBasePath+'/slides/') + slide.filename);
		console.log('Loading ' + slide.filename);
		topic.text(slide.title);
		setProgress(n);
		
		if(narrationTimeout !== null){
			clearTimeout(narrationTimeout);
			narrationTimeout = null;
		}
		destroyNarration();
		
		slideLoaded = false;
		audioLoaded = false;
		if(isMobile) loadCurrNarration();
		if(typeof(slide.completeAfter) === 'number') completeTimer = setTimeout(function(){
			onSlideEnded();
		}, slide.completeAfter * 1000);
		
		// Tracking
		trackPageView(
			slideContainer.attr('src'), 
			slideFullTitle(n)
		); // Slide 1: Instrucciones de Uso
	}


	function renderArrows(){
		prev.show(); if(currSlide === 0) prev.hide();
		next.show(); if(currSlide == slidesLen-1) next.hide();
		next.css('opacity', (maxSlide <= currSlide) ? 0.3 : 1);
	}


	function onSlideEnded(){
		var max = (currSlide+1 > maxSlide) ? currSlide+1 : maxSlide;
		
		// Tracking
		if(currSlide == slidesLen-1 && slideTimingStart !== null) trackTiming(
			'elearning_temas', 
			'duration', 
			'duracion_diapositiva_' + (currSlide + 1), 
			Math.round((new Date().getTime() - slideTimingStart) / 1000)
		);
		
		setProgress(null, max);
		// Auto-advance will not work on mobile devices because a user action is required to autoplay narration.
		if(slides[currSlide].autoadvance && !isMobile) showNextSlide();
	}


	function onSlideLoaded(){ // Called everytime an iframe finishes loading...
		var slide = slideContainer[0].contentWindow;
		slide.Player = Player;
		
		var $slide = $(slide.document);
		$slide.find('body').css({
			overflow: 'hidden',
			margin: 0,
			backgroundColor: slideBgColor
		});

		currentComposition = $slide.find('#Stage').attr('class');
		
		if(slide.AdobeEdge){ // Adobe Edge
			console.log('Loaded Adobe Edge slide');
			slide.AdobeEdge.bootstrapCallback(EDGE_initComposition);
		

		}else if(slide.hasOwnProperty('exportRoot') && slide.hasOwnProperty('stage')){ // Adobe Flash HTML5 Canvas
			console.log('Loaded Adobe Flash Canvas slide');
			var callbackTimer = setInterval(function() {
				if(slide.exportRoot !== undefined) {
					clearInterval(callbackTimer);
					slide.exportRoot.addEventListener("tick", FLASHCANVAS_onTick);
					slideLoaded = true;
					syncNarration();
				}
			}, 100);
		

		}else{ // Generic HTML
			console.log('Loaded HTML slide');
			slideLoaded = true;
			syncNarration();
		}
	}





	function loadCurrNarration(){
		var slide = slides[currSlide];
		if(typeof(slide.mobileNarration) === "string"){
			currNarration = ((/^(https?:)?\/\//).test(slide.filename)?'':'slides/') + escape(slide.mobileNarration);
			//alert(currNarration);
			narrationAudio = new Audio(currNarration);
			narrationAudio.play();
			narrationAudio.addEventListener('play', function () {
				narrationAudio.removeEventListener('play', arguments.callee, false);
				narrationAudio.pause(); // When the audio is ready to play, immediately pause.
				audioLoaded = true;
				//slideLoaded = true; // Uncomments to force start
				syncNarration(); // In case current slide loaded before audio
			}, false);
		}
	}
	

	playOverlay.on('click', function(e){
		e.preventDefault();
		EDGE_initComposition(currentComposition);
		playOverlay.hide();
	});


	function showNextSlide(){
		var n = currSlide + 1;
		
		// Tracking
		trackEvent(
			'elearning_temas', 
			'action', 
			'flecha_derecha_dispositiva_' + (n + 1)
		);
		
		showSlide(n);
	}


	function showPrevSlide(){
		var n = currSlide - 1;
		
		// Tracking
		trackEvent(
			'elearning_temas', 
			'action', 
			'flecha_izquierda_dispositiva_' + (n + 1)
		);
		
		showSlide(n);
	}


	function log(m){ // to test communication
		console.log('Slide says:');
		console.log(m);
	}




	// Admin bypass _______________________________________________________________________________________

	var kp = {r:0,d:0,a:0};
	function keyHandler(e){
		var k = e.which, t = e.type, d = 'keydown';
		if(k == 82) kp.r = (t == d) ? 1 : 0;
		if(k == 68) kp.d = (t == d) ? 1 : 0;
		if(k == 65) kp.a = (t == d) ? 1 : 0;
	}
	function isRDA(){
		//return true;
		return (kp.r&&kp.d&&kp.a);
	}




	// Events ____________________________________________________________________________________________

	next.on('click', function(e){
		e.preventDefault();
		if(maxSlide > currSlide || isRDA()) showNextSlide();
	});
	
	prev.on('click', function(e){
		e.preventDefault();
		showPrevSlide();
	});
	
	$('.show-popup').on('click', function(e){
		e.preventDefault();
		showPopup(this.hash);
	});
	
	overlay.on('click', hidePopups);
	slideContainer.on('load', onSlideLoaded);
	win.on('keydown keyup', keyHandler);





	// Init ______________________________________________________________________________________________

	var Player = {
		version: '201503162221',
		slideEnded: onSlideEnded,
		setScore: saveScore,
		trackPageView: trackPageView,
		trackEvent: trackEvent,
		trackTiming: trackTiming,
		config: config,
		removeSlides: removeSlides,
		log: log // to test communication
	};

	// Print player version
	try{ console.log('%cRD Player version %s', 'color:#AA7A2B; background:#FCF8C8', Player.version); }catch(err){} $('#player-version').text(Player.version);
	
	// Constructor
	loadSettings();

});