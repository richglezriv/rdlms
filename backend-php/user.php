<?php 

session_start();
if(!isset($_SESSION['user'])){ die(json_encode(array('status'=>'fail','data'=>array('reason'=>'session-expired')))); }


$response = array(
	'status' => "success",
	'data' => $_SESSION['user']
);

echo json_encode($response);