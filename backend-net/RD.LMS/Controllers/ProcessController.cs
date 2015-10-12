using RD.LMS.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RD.LMS.Controllers
{
    public class ProcessController : Controller
    {
        public ActionResult Index()
        {
            Response.Redirect("~/login.html");

            return View();
        }

        public ActionResult UploadThumbnail(String qqfile)
        {
            List<String> thumbs = Session[Utilities.THUMBS] == null ? new List<string>() : (List<String>)Session[Utilities.THUMBS];
            JSonModel model = new JSonModel() { status = "success" };
            HttpRequestBase hfc = Request;
            string path = Server.MapPath("~/uploads/" + qqfile);

            try
            {
                if (hfc.ContentLength > 500000)
                {
                    MessageData message = new MessageData();
                    message.message = "el archivo excede tamaño permitido";
                    model.status = "fail";
                    model.data = message;
                    return Json(model);
                }
                    
                var img = Bitmap.FromStream(hfc.InputStream);
                

                img.Save(path);
                thumbs.Add(qqfile);
            }
            catch (Exception ex) {

                MessageData message = new MessageData();
                message.message = ex.Message + " with file " + path;
                model.status = "fail";
                model.data = message;
            }
            

            return Json(model);
        }

        public ActionResult UploadScorm(string qqfile)
        {
            List<String> scorms = Session[Utilities.SCORMS] == null ? new List<string>() : (List<String>)Session[Utilities.SCORMS];
            JSonModel model = new JSonModel();

            try
            {
                HttpRequestBase hfc = Request;
                System.IO.Stream stream = hfc.InputStream;
                string path = Server.MapPath("~/uploads/" + qqfile);
                System.IO.FileStream newFile = System.IO.File.Create(path);
                stream.CopyTo(newFile);

                stream.Close();
                stream.Dispose();

                newFile.Close();
                newFile.Dispose();

                scorms.Add(qqfile);
            }
            catch (Exception)
            {
                model.status = "fail";
                MessageData m = new MessageData("error al cargar archivo scorm");
                model.data = m;
            }
            

            return Json(model);
        }

        public static void UnzipFile(String path)
        {
            System.IO.FileInfo info = new System.IO.FileInfo(path);
            string fullName = info.FullName;
            String folderName = fullName.Remove(fullName.Length - info.Extension.Length);
            if (!System.IO.Directory.Exists(folderName))
                System.IO.Directory.CreateDirectory(folderName);

            Ionic.Zip.ZipFile file = new Ionic.Zip.ZipFile(path);
            file.ExtractAll(folderName, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
            file.Dispose();

            System.IO.File.Delete(path);
        }

    }
}
