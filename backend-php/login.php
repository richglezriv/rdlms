<?php 

session_start();

$_SESSION['tries'] = $_SESSION['tries'] * 1;
$_SESSION['tries']++;

$json = $_POST['data'];
$data = json_decode($json, true);




$students = array(

	'victor' => array(
		'id' => 222,
		'isAdmin' => false,
		'name' => "Víctor",
		'lastName' => "Márquez",
		'secondLastName' => "Ortiz",
		'email' => "victmo@gmail.com",
		'birthday' => '1982-04-06',
		'gender' => 'M',
		'occupation' => 2, // Profesional de la salud
		'organization' => 3 // Independiente
	),

	'edgar' => array(
		'id' => 333,
		'isAdmin' => false,
		'name' => "Edgar",
		'lastName' => "Rosas",
		'secondLastName' => "Vicente",
		'email' => "edgar@reacciondigital.com",
		'birthday' => '1980-06-18',
		'gender' => 'M',
		'occupation' => 4, // Profesor/Maestro
		'organization' => 2 // ONG
	)

);

$admins = array(

	'admin' => array(
		'id' => 111,
		'isAdmin' => true,
		'name' => "Ricardo",
		'lastName' => "González",
		'secondLastName' => "Rivera",
		'email' => "ricardo@reacciondigital.com",
		'extra' => array(
			"DF", 
			"1982-02-01"
		)
	)

);




$response = array(
	'status' => "fail",
	'data' => array(
		'reason' => ($_SESSION['tries'] < 4) ? "credentials-error" : "too-many-tries"
	)
);

$csrftoken = uniqid();

if(isset($students[$data['email']]) && $data['password'] === 'user'){
	$_SESSION['tries'] = 0;
	$_SESSION['user'] = $students[$data['email']];
	$_SESSION['csrftoken'] = $csrftoken;
	$response = array(
		'status' => "success",
		'data' => array(
			'isAdmin' => false,
			'csrftoken' => $csrftoken
		)
	);
}

if(isset($admins[$data['email']]) && $data['password'] === 'admin'){
	$_SESSION['tries'] = 0;
	$_SESSION['user'] = $admins[$data['email']];
	$_SESSION['csrftoken'] = $csrftoken;
	$response = array(
		'status' => "success",
		'data' => array(
			'isAdmin' => true,
			'csrftoken' => $csrftoken
		)
	);
}

echo json_encode($response);