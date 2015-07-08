<?php 

$result = array(
	'status' => 'success',
	'data' => array(
		'POST' => $_POST,
		'GET' => $_GET
	)
);

echo json_encode($result);