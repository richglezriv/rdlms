<?php 

session_start();

unset($_SESSION['user']);
unset($_SESSION['tries']);
session_unset();

$response = array(
	'status' => "success",
	'data' => null
);

echo json_encode($response);