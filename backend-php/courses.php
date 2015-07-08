<?php 

$courses = array(
	array(
		"id" => "100",
		"name" => "Reacción Digital",
		"description" => "Curso de prueba de reacción digital.",
		"thumbnail" => 'thumb1.png',
		"status" => "passed",
		"score" => 85,
		"totalTime" => "02:15:25",
		"active" => true
	),
	array(
		"id" => "200",
		"name" => "Simple SCORM Diagnostic",
		"description" => "SCO para probar el intercambio de variables entre un SCO y este LMS.",
		"thumbnail" => 'thumb2.png',
		"status" => "not attempted",
		"totalTime" => "00:00:00",
		"active" => false
	),
	array(
		"id" => "300",
		"name" => "Osty Diagnostic",
		"description" => "SCO más avanzado para probar la comunicación entre el SCO y el LMS.",
		"thumbnail" => 'thumb3.png',
		"status" => "incomplete",
		"totalTime" => "01:10:05",
		"active" => true
	)
);

$response = array(
	'status' => "success",
	'data' => $courses
);

echo json_encode($response);