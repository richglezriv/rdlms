jQuery(function($){


	var loading = false,
		body = $('body'),
		win = $(window),
		list = $('.list.courses'),
		currentSCO = null
	;

	var statusMap = {
		"passed": ['Aprobado', 'success'],
		"failed": ['Reprobado', 'danger'],
		"completed": ['Completado', 'success'],
		"incomplete": ['Incompleto', 'warning'],
		"browsed": ['Visto', 'warning'],
		"not attempted": ['Nuevo', 'info']
	};




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
	}

	function stopLoading(){
		loading = false;
		body.css('cursor', '');
	}




	// Courses loading ____________________________________________________________

	function onLMSInitialized() {
	    RDLMS.validateSession();
		fetchCourses(RDLMS.settings);
		window.onbeforeunload = function(e){
			if(!(currentSCO === null || typeof(currentSCO) === 'undefined' || currentSCO.closed)){
				// e = e || window.event;
				// var message = 'Al cerrar esta ventana se cerrará la actividad en curso.';
				// if(e) e.returnValue = message; // IE7-
				// return message;
				currentSCO.close();
			}
		};
	}

	function fetchCourses(settings){
		if(loading) return;
		$.ajax({
			url: settings.lms.courses,
			dataType: 'json'
		})
			.done(function(response){
				if(response.status && response.status === 'success'){
					list.empty();
					if(response.data.length){
						$.each(response.data, function(id, course){
							list.append(renderCourse(
								course.id,
								course.name,
								course.description,
								course.thumbnail,
								course.status,
								(course.totalTime == null) ? '' : course.totalTime,
								(course.score == null) ? '' : course.score,
								course.active
							));
						});
					}else{
						list.append('<div><h4 class="text-info">Por el momento no tienes asignado ningún curso.</h4><p>Por favor regresa más tarde.</p></div>');
					}
				}else{
					showFeedback(response.message);
				}
			})
			.fail(function(){
				showFeedback('No fue posible cargar la lista de cursos');
			})
			.always(stopLoading)
		;
	}

	function renderCourse(id, name, description, thumb, status, time, score, active){
		var course = $('<div class="media course"><div class="media-left"><img class="media-object" alt="" /></div><div class="media-body"/></div>')
			.addClass(active?'active':'not-active')
		;
		course.find('.media-left > img').attr('src', RDLMS.settings.lms.uploadPath + thumb);
		course.find('.media-body')
			.append('<h4 class="media-heading">' + htmlEntities(name) + '</h4>')
			.append('<span class="status label label-' + statusMap[status][1] + '">' + statusMap[status][0] + '</span>')
			.append('<span class="score badge">' + score + '</span>')
			.append('<span class="time">' + time + '</span>')
			.append('<div class="description">' + htmlEntities(description) + '</div>')
		;
		if(active){
			course.on('click', function(e){
				launchSCO(id);
			});
		}
		return course;
	}

	function launchSCO(id){
		if(currentSCO === null || typeof(currentSCO) === 'undefined' || currentSCO.closed){
		    currentSCO = window.open('launch-scorm.html#' + id, 'scowin', '');
		    currentSCO.onSCOClosed = function () {
		        console.log('onSCOClosed()');
		        setTimeout(function () {
		            fetchCourses(RDLMS.settings);
		        }, 1000);
		        $("#in-course").modal('hide');
		    };
		    $("#in-course").modal({ backdrop: 'static', keyboard: false });
		    window.scowin = currentSCO;
		}else{
			showFeedback('Sólo puedes ver un curso a la vez.');
		}
	}




	// Init everything _____________________________________________________________

	RDLMS.showFeedback = showFeedback;
	RDLMS.init(onLMSInitialized);


});
