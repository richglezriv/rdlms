/*

Simple Uploaders by victmo

Dependencies: 
 * Fine Uploader 2.1.2 (Released October 25)
   https://github.com/FineUploader/fine-uploader/tree/2.1.2

Include:
 * jQuery 1.8+
 * Bootstrap 3
 * FineUploader/file-uploader/client/fileuploader.js (from Github)
 * simple-uploaders.js
 * simple-uploaders.css

Markup:

	<div class="form-group">
		<label>Image upload</label>
		
		<div id="upload-3" class="fileinput" data-provides="fileinput">
			<div class="fileinput-preview"></div>
			<!-- <div class="fileinput-preview panel panel-default"></div> -->
			<div>
				<button href="#" class="fileinput-action btn btn-default">
					<span class="fileinput-select">Select image</span>
					<span class="fileinput-change">Change image</span>
					<span class="fileinput-uploading">Uploading image...</span>
				</button>
				<button class="btn btn-danger fileinput-remove">Remove image</button>
				<!-- <button class="btn btn-danger fileinput-remove"><span class="glyphicon glyphicon-trash"></span></button> -->
			</div>
		</div>
		
		<p class="help-block">Example block-level help text here.</p>
	</div>

Javascript:

	var myUploader = new SimpleUploader('#upload-1', {
		type: "image",
		uploadPath: 'uploads/',
		onSuccess: function(id, fileName, response){},
		action: 'php/upload.php'
	});

Methods:

	myUploader.val() - Gets the widget value
	myUploader.val('') - Empty the value (removes preview)
	myUploader.val('file.jpg') - Sets the value to file.jpg and show the preview as {uploadPath}/{file}

Parameters:

	ImageUploader(el, opts)
	el: Element or selector (string) for the element that hold the widget (#upload-1 in the above example)
	opts:
	  - debug (bool): Shows verbose info in the console
	  - messages: Error message codes (check the code :P)
	  - showFeedback: The function to be executed when the widget needs to show a message. Default: alert()
	  - sizeLimit: 0 (default) = no limit, in bytes
	  - minSizeLimit: 0 (default) = no limit, in bytes
	  - allowedExtensions: ['jpg','jpeg','png'] by default
	  * action: the backend upload script i.e. upload.php
	  * uploadPath: the upload folder path
	  - type: file (default) or image. Controls what is shown in the preview (an img or a filename link)
	  - data: Additional POST data to be sent with the request The same as jQuery.ajax data param.
*/



function SimpleUploader(el, opts){
	// Required params: action
	// Optional: ... uploadPath (with trailing slash!)
	var self = this;
	var value = '';
	var widget = $(el).eq(0);
	var button = widget.find('.fileinput-action').eq(0);
	var preview = widget.find('.fileinput-preview');
	var remove = widget.find('.fileinput-remove');
	var currentUploadId = null;
	var messages = {typeError: "{file} tiene una extensión inválida. Extensiones válidas: {extensions}.",sizeError: "{file} es muy grande, el tamaño máximo del archivo debe ser {sizeLimit}.",minSizeError: "{file} es muy pequeño, el tamaño minimo del archivo debe ser {minSizeLimit}.",emptyError: "{file} está vacio, selecciona otro archivo.",noFilesError: "No hay archivos que subir.",onLeave: "Los archivos se están subiendo, si abandonas esta pantalla, los archivos se perderán."};
		messages = $.extend(messages, opts.messages);
	var showFeedback = opts.showFeedback || function(msg){ alert(msg); };
	var isLoading = false;
	var type = opts.type || 'file';
	var uploader = new qq.FileUploaderBasic({
		params: opts.data || {},
		messages: messages,
		showMessage: showFeedback,
		button: button[0], 
		multiple: false, 
		autoUpload: true, 
		debug: opts.debug || false, 
		sizeLimit: opts.sizeLimit || 0, 
		minSizeLimit: opts.minSizeLimit || 0,
		allowedExtensions: opts.allowedExtensions || (type==='image') ? ['jpg','jpeg','png'] : ['doc','docx','xls','xlsx','pdf','txt','zip','rar'],
		action: opts.action,
		onSubmit: function(id, fileName) {
			if(currentUploadId !== null) uploader.cancel(currentUploadId);
			currentUploadId = id;
			isLoading = true;
			updateLabel();
		},
		onProgress: function(id, fileName, loaded, total) {
			isLoading = true;
			updateLabel();
		},
		onComplete: function(id, fileName, response) {
			currentUploadId = null;
			isLoading = false;
			if(response.status && response.status === 'success'){
				setValue(fileName);
				if(typeof(opts.onSuccess)==='function') onSuccess(id, fileName, response.data);
			}else{
				// showFeedback(messages[response.error].replace(/\{file\}/g, fileName).replace(/\{(\w+)\}/g, function(a,match){ 
				// 	console.log(match);
				// 	return uploader._options[match]; 
				// }));
				if(response.message) showFeedback(response.message);
				else if(response.data.message) showFeedback(response.data.message);
				else showFeedback('Ocurrió un error al intentar subir el archivo.');
			}
			updateLabel();
		}
		// TODO: Handle cases where server response is NOT json or not expected i.e. a php error or warning...
	});
	function updateLabel(){
		widget.removeClass('fileinput-is-loaded fileinput-is-loading fileinput-is-empty');
		if(isLoading) widget.addClass('fileinput-is-loading');
		else if(value) widget.addClass('fileinput-is-loaded');
		else widget.addClass('fileinput-is-empty');
	}
	function setValue(val){
		value = val;
		preview.empty();
		if(val !== ''){
			if(type === 'image') preview.append('<img alt="" src="' + opts.uploadPath + value + '">');
			else preview.append('<a target="_blank" href="' + opts.uploadPath + value + '">' + value + '</a>');
		}
		updateLabel();
	}
	remove.on('click', function(e){
		e.preventDefault();
		setValue('');
	});
	self.val = function(img){
		if(typeof(img) === 'string'){
			setValue(img);
			return self;
		}else{
			return value;
		}
	};

	updateLabel();
	return self;
}