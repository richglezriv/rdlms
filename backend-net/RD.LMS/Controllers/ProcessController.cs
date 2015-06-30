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
        

        public ActionResult UploadThumbnail(String qqfile)
        {
            List<String> thumbs = Session[Utilities.THUMBS] == null ? new List<string>() : (List<String>)Session[Utilities.THUMBS];

            HttpRequestBase hfc = Request;
            var img = Bitmap.FromStream(hfc.InputStream);
            string path = Server.MapPath("/uploads/" + qqfile);
            img.Save(path);

            JSonModel model = new JSonModel() { status = "success" };
            thumbs.Add(qqfile);

            return Json(model);
        }

        public ActionResult UploadScorm(string qqfile)
        {
            List<String> scorms = Session[Utilities.SCORMS] == null ? new List<string>() : (List<String>)Session[Utilities.SCORMS];

            HttpRequestBase hfc = Request;
            System.IO.Stream stream = hfc.InputStream;
            string path = Server.MapPath("/uploads/" + qqfile);
            System.IO.FileStream newFile = System.IO.File.Create(path);
            stream.CopyTo(newFile);

            stream.Close();
            stream.Dispose();
            
            newFile.Close();
            newFile.Dispose();

            JSonModel model = new JSonModel() { status = "success" };
            scorms.Add(qqfile);

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
