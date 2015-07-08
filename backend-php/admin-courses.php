<?php 

$courses = array(
	array(
		"id" => "100",
		"name" => "Reacción Digital",
		"description" => "Curso de prueba de reacción digital.",
		"thumbnail" => 'thumb1.png',
		"scorm" => "scorm_100.zip",
		"conditions" => array()
	),
	array(
		"id" => "200",
		"name" => "Simple SCORM Diagnostic",
		"description" => "SCO para probar el intercambio de variables entre un SCO y este LMS.",
		"thumbnail" => 'thumb2.png',
		"scorm" => "scorm_200.zip",
		"conditions" => array("100")
	),
	array(
		"id" => "300",
		"name" => "Osty Diagnostic",
		"description" => "SCO más avanzado para probar la comunicación entre el SCO y el LMS.",
		"thumbnail" => 'thumb3.png',
		"scorm" => "scorm_300.zip",
		"conditions" => array()
	),
	array(
		"id" => "400",
		"name" => "Otro curso",
		"description" => "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Rem, consequatur, neque. Veniam accusamus.",
		"thumbnail" => 'thumb2.png',
		"scorm" => "scorm_400.zip",
		"conditions" => array("100","300")
	)
);

$response = array(
	'status' => "success",
	'data' => $courses
);

echo json_encode($response);