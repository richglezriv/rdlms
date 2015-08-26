<?php 

session_start();
if(!isset($_SESSION['user'])){ die(json_encode(array('status'=>'fail','data'=>array('reason'=>'session-expired')))); }
if($_SESSION['user']['isAdmin']){ die(json_encode(array('status'=>'fail','data'=>array('reason'=>'users-only')))); }

$sts = ['not attempted', 'incomplete', 'browsed', 'completed'];

$courses = array(
	array(
		"id" => "200",
		"name" => "Simple SCORM Diagnostic",
		"description" => "SCO para probar el intercambio de variables entre un SCO y este LMS.",
		"thumbnail" => 'thumb2.png',
		"status" => $sts[rand(0,3)],
		"totalTime" => "0".rand(1,9).":".rand(10,59).":".rand(10,59),
		"active" => true
	),
	array(
		"id" => "100",
		"name" => "Reacción Digital",
		"description" => "Curso de prueba de reacción digital.",
		"thumbnail" => 'thumb1.png',
		"status" => "passed",
		"score" => rand(60, 100),
		"totalTime" => "0".rand(1,9).":".rand(10,59).":".rand(10,59),
		"active" => false
	),
	array(
		"id" => "300",
		"name" => "Osty Diagnostic",
		"description" => "SCO más avanzado para probar la comunicación entre el SCO y el LMS.",
		"thumbnail" => 'thumb3.png',
		"status" => $sts[rand(0,3)],
		"totalTime" => "0".rand(1,9).":".rand(10,59).":".rand(10,59),
		"active" => true
	)
);

$response = array(
	'status' => "success",
	'data' => $courses
);

echo json_encode($response);