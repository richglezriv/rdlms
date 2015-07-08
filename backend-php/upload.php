<?php
require_once("modules/qqFileUploader/qqfileuploader.class.php");

// TODO: user session validation... Is user logged in?

$uploadPath = '../frontend/uploads/'; // relative to getcwd() [aka Working directory];
$allowedExtensions = array();//'jpg', 'jpeg', 'png');
$sizeLimit = 1000 * 1024; // bytes * 1024 = KB
$messages = array(
	"php_settings" => 'Error de configuración en el servidor ("post_max_size" | "upload_max_filesize" < {{max_size}}).',
	"dir_not_writable" => "Error del servidor. No se pudo escribir el archivo.",
	"no_files" => "No se recibió ningún archivo.",
	"size_error" => "El archivo que se subió es demasiado grande.", // {{size_limit}}
	"type_error" => "La extensión del archivo es incorrecta, debe ser una de estas: {{extensions}}",
	"unknown_error" => "No se pudo guardar el archivo. La subida fue cancelada o se encontró un problema en el servidor."
);

$uploader = new qqFileUploader($allowedExtensions, $sizeLimit, $messages);
$result = $uploader->handleUpload($uploadPath);
die(json_encode($result));
