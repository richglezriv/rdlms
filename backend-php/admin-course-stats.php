<?php 

session_start();
if(!isset($_SESSION['user'])){ die(json_encode(array('status'=>'fail','data'=>array('reason'=>'session-expired')))); }
if(!$_SESSION['user']['isAdmin']){ die(json_encode(array('status'=>'fail','data'=>array('reason'=>'admins-only')))); }

$stats = array(
	"passed" => 323,
	"failed" => 23,
	"incomplete" => 692,
	"completed" => 0,
	"notAttempted" => 1793
);

$response = array(
	'status' => "success",
	'data' => $stats
);

echo json_encode($response);