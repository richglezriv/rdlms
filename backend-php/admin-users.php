<?php 

$data = json_decode($_POST['data'], true);
$q = $data['query'];

$users = array(
	array(
		"userId" => "100",
		"name" => "Victor",
		"lastName" => "Márquez",
		"secondLastName" => "Ortiz",
		"email" => "victmo-$q@gmail.com",
		"extra" => array()
	),
	array(
		"userId" => "200",
		"name" => "Ricardo",
		"lastName" => "Adams",
		"secondLastName" => "Smith",
		"email" => "ric@gmail.com",
		"extra" => array()
	),
	array(
		"userId" => "300",
		"name" => "Edgar",
		"lastName" => "Johnson",
		"secondLastName" => "Lopez",
		"email" => "edjohnson@gmail.com",
		"extra" => array()
	)
);

$response = array(
	'status' => "success",
	'data' => $users
);

echo json_encode($response);