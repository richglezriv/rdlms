<?php 

session_start();

$user = null;
$type = "logged-out";
if(isset($_SESSION['user'])){
	$user = $_SESSION['user'];
	$type = $user['isAdmin'] ? 'admin' : 'student';
}

$response = array(
	'status' => "success",
	'data' => array(
		'sessionType' => $type,
		'user' => $user
	)
);

echo json_encode($response);