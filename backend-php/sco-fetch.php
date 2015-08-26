<?php 

$formData = json_decode($_POST['data']);
$courseId = $formData->courseId;

$courses = array(
	'100' => array(
		'id' => '100',
		'name' => 'Reacción Digital',
		'scoPath' => 'reacciondigital',
		'scoIndex' => 'player.html',
		'dataModel' => array(
			'cmi.core.student_id' => '123',
			'cmi.core.student_name' => 'Victor Márquez',
			'cmi.core.lesson_location' => '{lastIndex:1, maxIndex:2}',
			'cmi.core.credit' => 'no-credit',
			'cmi.core.lesson_status' => 'passed',
			'cmi.core.entry' => '',
			'cmi.core.score.raw' => '',
			'cmi.core.score.min' => '0',
			'cmi.core.score.max' => '10',
			'cmi.core.total_time' => '02:15:25',
			'cmi.core.exit' => 'logout',
			'cmi.core.session_time' => '00:45:00',
			'cmi.suspend_data' => '',
			'cmi.launch_data' => '',
			'cmi.student_data.mastery_score' => '65'
		)
	),


	'200' => array(
		'id' => '100',
		'name' => 'SCORM Diagnostics',
		'scoPath' => 'diagnostics',
		'scoIndex' => 'default.htm',
		'dataModel' => array(
			'cmi.core.student_id' => '123',
			'cmi.core.student_name' => 'Victor Márquez',
			'cmi.core.lesson_location' => '',
			'cmi.core.credit' => 'no-credit',
			'cmi.core.lesson_status' => 'not attempted',
			'cmi.core.entry' => '',
			'cmi.core.score.raw' => '',
			'cmi.core.score.min' => '0',
			'cmi.core.score.max' => '100',
			'cmi.core.total_time' => '00:00:00',
			'cmi.core.exit' => '',
			'cmi.core.session_time' => '00:00:00',
			'cmi.suspend_data' => '',
			'cmi.launch_data' => '',
			'cmi.student_data.mastery_score' => '65'
		)
	),


	'300' => array(
		'id' => '300',
		'name' => 'Osty Diagnostic',
		'scoPath' => 'osty',
		'scoIndex' => 'proddingsco.htm',
		'dataModel' => array(
			'cmi.core.student_id' => '123',
			'cmi.core.student_name' => 'Victor Márquez',
			'cmi.core.lesson_location' => '',
			'cmi.core.credit' => 'no-credit',
			'cmi.core.lesson_status' => 'incomplete',
			'cmi.core.entry' => '',
			'cmi.core.score.raw' => '',
			'cmi.core.score.min' => '0',
			'cmi.core.score.max' => '10',
			'cmi.core.total_time' => '01:10:05',
			'cmi.core.exit' => 'suspend',
			'cmi.core.session_time' => '00:15:00',
			'cmi.suspend_data' => '',
			'cmi.launch_data' => 'Test launch data!',
			'cmi.student_data.mastery_score' => ''
		)
	)
);

if(array_key_exists($courseId, $courses)){
	echo json_encode(array(
		'status' => 'success',
		'data' => $courses[$courseId]
	));
}else{
	echo json_encode(array(
		'status' => 'fail',
		'message' => 'El curso especificado no existe'
	));
}
