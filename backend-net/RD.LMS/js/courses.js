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

	function onLMSInitialized(){
		fetchCourses(RDLMS.settings);
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
							list.append(createCourse(
								course.id,
								course.name,
								course.description,
								course.thumbnail,
								course.status,
								course.totalTime,
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

	function createCourse(id, name, description, thumb, status, time, active){
		var course = $('<div class="media course"><div class="media-left"><img class="media-object" alt="" /></div><div class="media-body"/></div>')
			.addClass(active?'active':'not-active')
		;
		course.find('.media-left > img').attr('src', RDLMS.settings.lms.uploadPath + thumb);
		course.find('.media-body')
			.append('<h4 class="media-heading">' + htmlEntities(name) + '</h4>')
			.append('<span class="status label label-' + statusMap[status][1] + '">' + statusMap[status][0] + '</span>')
			.append('<span class="time">' + time + '</span>')
			.append('<div class="description">' + htmlEntities(description) + '</div>')
		;
		course.on('click', function(e){
			launchSCO(id);
		});
		return course;
	}

	function launchSCO(id){
		if(currentSCO === null || typeof(currentSCO) === 'undefined' || currentSCO.closed){
			currentSCO = window.open('launch-scorm.html#' + id, 'sco', '');
		}else{
			showFeedback('Sólo puedes ver un curso a la vez.');
		}
	}




	// Init everything _____________________________________________________________

	RDLMS.showFeedback = showFeedback;
	RDLMS.init(onLMSInitialized);


});
