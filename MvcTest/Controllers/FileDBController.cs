using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using Numeria.IO;
using System.Text;

namespace MvcTest.Controllers
{
    public class FileDBController : Controller
    {
        private string pathDB = @"C:\Temp\MvcData.dat";

        public ActionResult Index()
        {
            FileDB.CreateEmptyFile(pathDB, true);

            var files = FileDB.ListFiles(pathDB);

            ViewData["TotalFiles"] = files.Length;

            return View(files.Take(200).ToArray());
        }

        public ActionResult Structure()
        {
            using (var db = new FileDB(pathDB, FileAccess.Read))
            {
                ViewData["DebugInfo"] = db.Debug.DisplayPages();
            }
            return View();
        }

        [HttpPost]
        public ActionResult Upload()
        {
            HttpPostedFileBase file = Request.Files[0] as HttpPostedFileBase;

            if (file.ContentLength > 0)
                FileDB.Store(pathDB, file.FileName, file.InputStream);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UploadJSON()
        {
            var fileName = Request["qqfile"];

            if (string.IsNullOrEmpty(fileName)) // IE
            {
                HttpPostedFileBase file = Request.Files[0] as HttpPostedFileBase;
                FileDB.Store(pathDB, file.FileName, file.InputStream);
            }
            else
            {
                FileDB.Store(pathDB, fileName, Request.InputStream);
            }

            return Content("{ success : true }");
        }

        public ActionResult Download(string id)
        {
            // Using a classic way to download a file, insted of FileContentResult mode. 
            // Optimize for big files (download on demand)

            using (var db = new FileDB(pathDB, FileAccess.Read))
            {
                var info = db.Search(Guid.Parse(id));

                Response.Buffer = false;
                Response.BufferOutput = false;
                Response.ContentType = info.MimeType;
                Response.AppendHeader("Content-Length", info.FileLength.ToString());
                Response.AppendHeader("content-disposition", "attachment; filename=" + info.FileName);

                db.Read(info.ID, Response.OutputStream);

                return new EmptyResult();
            }
        }

        public ActionResult Thumbnail(string id)
        {
            using (var db = new FileDB(pathDB, FileAccess.Read))
            {
                var info = db.Search(Guid.Parse(id));

                if (!info.MimeType.StartsWith("image", StringComparison.InvariantCultureIgnoreCase))
                    return File(Server.MapPath("~/Content/no-picture.jpg"), "image/jpg");

                using (MemoryStream output = new MemoryStream())
                {
                    db.Read(info.ID, output);

                    Image image = Image.FromStream(output);
                    Image thumbnailImage = image.GetThumbnailImage(64, 64, new Image.GetThumbnailImageAbort(delegate { return true; }), IntPtr.Zero);

                    using (MemoryStream imageStream = new MemoryStream())
                    {
                        thumbnailImage.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);

                        return File(imageStream.ToArray(), "image/png");
                    }
                }
            }
        }

        public ActionResult Delete(string id)
        {
            FileDB.Delete(pathDB, Guid.Parse(id));

            return RedirectToAction("Index");
        }

        public ActionResult DeleteAll()
        {
            using (var db = new FileDB(pathDB, FileAccess.ReadWrite))
            {
                var ent = db.ListFiles();

                foreach(var e in ent)
                    db.Delete(e.ID);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Shrink()
        {
            FileDB.Shrink(pathDB);

            return RedirectToAction("Index");
        }
    }

    public static class HtmlHelperExtensions
    {
        public static string FormatFileLength(this HtmlHelper html, uint length)
        {
            if (length < 1024)
                return string.Format("{0} B", length);
            else if (length < (1024 * 1024))
                return string.Format("{0:#,##0.00} KB", (double)length / (double)1024);
            else if (length < (1024 * 1024 * 1024))
                return string.Format("{0:#,##0.00} MB", (double)length / (double)(1024 * 1024));
            else
                return string.Format("{0:#,##0.00} GB", (double)length / (double)(1024 * 1024 * 1024));
        }
    }
}
