<?php 

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