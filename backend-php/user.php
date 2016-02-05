<?php 

session_start();

$data = json_decode($_POST['data']);
$token = $_POST['csrftoken'];

$user = null;
$type = "logged-out";
if(isset($_SESSION['user']) && $_SESSION['csrftoken'] && $_SESSION['csrftoken'] === $token){
	$user = $_SESSION['user'];
	$type = $user['isAdmin'] ? 'admin' : 'student';
}

$response = array(
	'status' => "success",
	'data' => array(
		'sessionType' => $type,
		'user' => $user,
		'csrftokenMatch' => ($_SESSION['csrftoken'] === $token)
	)
);

echo json_encode($response);