using BSoft.MailProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RD.Business
{
    public class NotificationController
    {
        public void SendConfirmationMail(Entities.User user)
        {
            String acctConfirmation = "Confirmacion de cuenta";
            String confirmUri = GetConfirmationUri(user);
            String body = GetConfirmationEmailBody(confirmUri);
            

            try
            {
                SendEmail(acctConfirmation, user.FirstName, body, user.Email);
            }
            catch (BSoft.MailProvider.MailControlException ex)
            {
                throw;

            }
            
        }

        private String GetConfirmationUri(Entities.User user) {
            String uri = String.Format("{0}External/Confirmation/?confirm={1}&ser={2}",
                Properties.Settings.Default.confirmationUri, user.Id.ToString(), Business.Utilities.GetSerialHash(user.Id.ToString()));
            return uri;
        }

        private string GetConfirmationEmailBody(String confirmUri)
        {
            StringBuilder builder = new StringBuilder();
            String imageSource = String.Format("{0}branding/pleca_uxns.jpg", Properties.Settings.Default.confirmationUri);
            builder.AppendLine(@"<!DOCTYPE html>
                                    <html>
                                     <head>
                                      <title>Home</title>
                                      <style>
	                                    .version.index {	color: #0000FD;	background-color: #AEF28B;}
	                                    html {	background-color: #FFFFFF;	font-family: Arial;}
	                                    #page {	z-index: 1;	width: 500px;	min-height: 699.9999999999999px;	background-image: none;	border-style: none;	   border-color: #000000;	background-color: transparent;	padding-bottom: 0px;	margin-left: auto;	margin-right: auto;}
	                                    #page_position_content {	width: 0.01px;}
	                                    #u81 {	z-index: 2;	width: 500px;}
	                                    #u115-4 {	z-index: 11;	width: 392px;	min-height: 62px;	background-color: transparent;	color: #1ABC9C;	line-height: 49px;	font-size: 41px;	margin-left: 20px;	margin-top: 31px;	position: relative;}
	                                    #u130-22 {	z-index: 15;	width: 460px;	min-height: 311px;	background-color: transparent;	margin-left: 20px;	margin-top: 28px;	position: relative;}
	                                    #u130-7 {	text-align: center;	color: #1ABC9C;}
	                                    #u130-17 {	color: #1ABC9C;}
	                                    #u130-19 {	color: #1ABC9C;	font-weight: bold;}
	                                    #u84 {	z-index: 4;	width: 500px;	padding-bottom: 20px;	margin-top: 151px;}
	                                    #u93-6 {	z-index: 5;	width: 441px;	min-height: 28px;	margin-right: -10000px;	margin-top: 14px;	left: 30px;}
	                                    body {	position: relative;	min-width: 500px;	padding-top: 36px;	padding-bottom: 36px;}
	                                    #page .verticalspacer {	clear: both;}
                                      </style>
                                       </head>
                                     <body>
                                      <div class='clearfix' id='page'><div class='position_content' id='page_position_content'><div class='clip_frame olelem' id='u81'>
                                         <img class='block' id='u81_img' src='" + imageSource + @"' alt='' width='500' height='55'/></div><div class='clearfix colelem' id='u115-4'>     <p>Correo de Activaci&oacute;n</p></div>
                                        <div class='clearfix colelem' id='u130-22'>
                                         <p>Te damos la bienvenida a nuestra plataforma de cursos en l&iacute;nea.</p>
                                         <p>Para terminar con tu proceso de registro debes de seguir esta liga para activar tu cuenta nueva:</p><p>&nbsp;</p>");
            builder.Append(String.Format("<p id='u130-7'><a href='{0}'>activacion de cuenta</a></p><p>&nbsp;</p>", confirmUri));
            builder.AppendLine(@"<p>(Si al dar click no te conecta a nuestro sitio te pedimos copiar la direcci&oacute;n a tu navegador e intentar de manera directa)</p><p>" + confirmUri + @"</p><p>&nbsp;</p>
                                         <p>Gracias por tu participaci&oacute;n.</p><p>&nbsp;</p>
                                        <p id='u130-17'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Atentamente,</p>
                                         <p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <span id='u130-19'>Equipo de Unidos por Ni&ntilde;os Saludables</span></p>
                                        </div>
                                        <div class='verticalspacer'></div><div class='clearfix colelem' id='u84'><div class='clearfix grpelem' id='u93-6'>
                                          <p>&#64;2014 Nestl&eacute; Todas las marcas registradas son propiedad de</p>
                                          <p>Soci&eacute;t&eacute; des Produits Nestl&eacute; S.A., o utilizadas con el debido permiso.</p>
                                         </div></div></div></div></body></html>");

            return builder.ToString();
        }

        private void SendEmail(string subject, string name, string mailBody, string emailAddress)
        {
            MailControl control = BSoft.MailProvider.MailControl.GetInstance();
            DestinationEntity destination = new DestinationEntity()
            {
                IsHtmlEncoded = true,
                MailAdress = emailAddress,
                MailBody = mailBody,
                Name = name,
                Subject = subject
            };

            try
            {
                control.SendEmail(destination);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SendPasswordRecoveryMail(Entities.User user)
        {
            String subject = "Restablecer Contraseña";
            String confirmUri = GetPasswordRecoveryUri(user);
            String mailBody = GetPasswordRecoveryEmailBody(confirmUri);

            try
            {
                SendEmail(subject, user.FirstName, mailBody, user.Email);
            }
            catch (BSoft.MailProvider.MailControlException ex)
            {
                throw new Exception("Imposible enviar restablecimiento de contraseña");
            }
        }

        private String GetPasswordRecoveryUri(Entities.User user)
        {
            String uri = String.Format("{0}Home/ResetPassword/?id={1}&ser={2}",
                Properties.Settings.Default.confirmationUri, user.Id.ToString(), Business.Utilities.GetSerialHash(user.Id.ToString()));
            return uri;
        }

        private String GetPasswordRecoveryEmailBody(String resetUri)
        {
            StringBuilder builder = new StringBuilder();
            String imageSource = String.Format("{0}branding/pleca_uxns.jpg", Properties.Settings.Default.confirmationUri);
            builder.AppendLine(@"<!DOCTYPE html><html><head><title>Restablecer Contraseña</title><style>
                                .version.index{color:#0000E3;background-color:#3AFF43}html{background-color:#FFFFFF;font:medium Arial;}#page{z-index:1;width:500px;min-height:699.9999999999999px;background-image:none;border-style:none;border-color:#000000;background-color:transparent;padding-bottom:0px;margin-left:auto;margin-right:auto}#page_position_content{width:0.01px}#u81{z-index:2;width:500px}#u115-4{z-index:11;width:460px;min-height:62px;background-color:transparent;color:#1ABC9C;font-size:30px;line-height:36px;margin-left:20px;margin-top:31px;position:relative}#u130-25{z-index:15;width:460px;min-height:311px;background-color:transparent;margin-left:20px;margin-top:36px;position:relative}#u130-7{text-align:center;color:#1ABC9C}#u130-20{color:#1ABC9C}#u130-22{color:#1ABC9C;font-weight:bold}#u84{z-index:4;width:500px;padding-bottom:20px;margin-top:133px}#u93-6{z-index:5;width:441px;min-height:28px;margin-right:-10000px;margin-top:14px;left:30px}body{position:relative;min-width:500px;padding-top:36px;padding-bottom:36px}#page .verticalspacer{clear:both}
                                </style>
                                </head><body><div id='page' class='clearfix'><div id='page_position_content' class='position_content'><div id='u81' class='clip_frame colelem'>");
            builder.AppendLine(String.Format("<img id='u81_img' alt='' class='block' height='55' src='{0}' width='500' /> </div><div id='u115-4' class='clearfix colelem'><p>Correo de Recuperación de contraseña</p></div><div id='u130-25' class='clearfix colelem'><p>Hemos recibido tu solicitud para restablecer tu contraseña en nuestro sistema de cursos en l&iacute;nea. Para hacer esto debes seguir esta liga:</p><p>&nbsp;</p>", imageSource));
            builder.AppendLine(String.Format("<p id='u130-7'><a href='{0}'>restablecer contrase&ntilde;a</a></p><p>&nbsp;</p><p>(Si al dar click no te conecta a nuestro sitio te pedimos copiar la</p><p>direcci&oacute;n a tu navegador e intentar de manera directa)</p>", resetUri));
            builder.AppendLine(String.Format("<p>[{0}]</p><p>Este enlace es para un &uacute;nico inicio de sesi&oacute;n y te llevar&aacute; a una p&aacute;gina donde podr&aacute;s establecer tu contrase&ntilde;a.</p><p>&nbsp;</p><p id='u130-20'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Atentamente,</p><p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <span id='u130-22'>Equipo de Unidos por Ni&ntilde;os Saludables</span></p></div><div class='verticalspacer'></div><div id='u84' class='clearfix colelem'><div id='u93-6' class='clearfix grpelem'><p>&#64;2014 Nestl&eacute; Todas las marcas registradas son propiedad de</p><p>Soci&eacute;t&eacute; des Produits Nestl&eacute; S.A., o utilizadas con el debido permiso.</p></div></div></div></div></body></html>", resetUri));
            return builder.ToString();
        }
    }
}
