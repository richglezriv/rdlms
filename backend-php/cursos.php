<?php 

// Se conecta a la bd
// Saca lista de cursos
// Muestra cursos como json

$cursos = array(
	array(
		"id" => "1",
		"nombre" => "Primer curso",
		"descripcion" => "Lorem ipsum dolor sit amet",
		"path" => "cursos/123123"
	),
	array(
		"id" => "2",
		"nombre" => "Segundo curso",
		"descripcion" => "Segundo lotem ipsum dolor sit amet",
		"path" => "cursos/123123"
	),
	array(
		"id" => "3",
		"nombre" => "Último cuasdasdadsrso",
		"descripcion" => "Ttres tres ipsum dolor sit amet",
		"path" => "cursos/123123"
	)
);

$response = array(
	'status' => "success",
	'data' => $cursos
);

echo json_encode($response);