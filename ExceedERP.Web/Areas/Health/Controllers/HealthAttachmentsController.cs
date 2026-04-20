using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using System.IO;
using ExceedERP.Core.Domain.Health;

namespace ExceedERP.Web.Areas.Health.Controllers
{
    public class HealthAttachmentsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HealthAttachments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var healthAttachments = db.HealthAttachments.Where(x => x.HealthRankAndValueId == id ).ToList();
            DataSourceResult result = healthAttachments.ToDataSourceResult(request, healthAttachment => new
            {
                HealthAttachmentId = healthAttachment.HealthAttachmentId,
                HealthRankAndValueId = healthAttachment.HealthRankAndValueId,
                FilePath = healthAttachment.FilePath,
                FileName = healthAttachment.FileName,
                ShortPath = healthAttachment.ShortPath,
                FileExtension = healthAttachment.FileExtension,
                Description = healthAttachment.Description
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HealthAttachments_Create([DataSourceRequest]DataSourceRequest request, HealthAttachment healthAttachment, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new HealthAttachment
                {
                    HealthRankAndValueId = id,
                    FilePath = healthAttachment.FilePath,
                    FileName = healthAttachment.FileName,
                    ShortPath = healthAttachment.ShortPath,
                    FileExtension = healthAttachment.FileExtension,
                    Description = healthAttachment.Description
                };

                db.HealthAttachments.Add(entity);
                db.SaveChanges();
                healthAttachment.HealthAttachmentId = entity.HealthAttachmentId;
            }

            return Json(new[] { healthAttachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HealthAttachments_Update([DataSourceRequest]DataSourceRequest request, HealthAttachment healthAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new HealthAttachment
                {
                    HealthAttachmentId = healthAttachment.HealthAttachmentId,
                    HealthRankAndValueId = healthAttachment.HealthRankAndValueId,
                    FilePath = healthAttachment.FilePath,
                    FileName = healthAttachment.FileName,
                    ShortPath = healthAttachment.ShortPath,
                    FileExtension = healthAttachment.FileExtension,
                    Description = healthAttachment.Description
                };

                db.HealthAttachments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { healthAttachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HealthAttachments_Destroy([DataSourceRequest]DataSourceRequest request, HealthAttachment healthAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new HealthAttachment
                {
                    HealthAttachmentId = healthAttachment.HealthAttachmentId,
                    HealthRankAndValueId = healthAttachment.HealthRankAndValueId,
                    FilePath = healthAttachment.FilePath,
                    FileName = healthAttachment.FileName,
                    ShortPath = healthAttachment.ShortPath,
                    FileExtension = healthAttachment.FileExtension,
                    Description = healthAttachment.Description
                };

                db.HealthAttachments.Attach(entity);
                db.HealthAttachments.Remove(entity);
                db.SaveChanges();
                RemoveFiles(healthAttachment.FilePath);
            }

            return Json(new[] { healthAttachment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RemoveFiles(string fileName)
        {
            if (fileName != null)
            {
                var physicalPath = Path.Combine(Server.MapPath("/Content/Files/Archive/"), fileName);

                if (System.IO.File.Exists(physicalPath))
                {
                    // Deleting the File from the Location
                    System.IO.File.Delete(physicalPath);
                }
            }

            return Content("");
        }
     
        public ActionResult UploadIncomingHealthFiles(int masterId, HttpPostedFileBase file, string description)
        {
            if (file != null && file.ContentLength > 0)
            {
                string path = HttpContext.Server.MapPath("~/Content/Files/Health/");
                string shortPath = "/Content/Files/Health/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string newFileName = Path.GetExtension(file.FileName);
                var fileExtension = System.IO.Path.GetExtension(file.FileName);
                string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                newFileName = name + newFileName;
                string filePath = path + newFileName;

                file.SaveAs(filePath);

                var entity = new HealthAttachment
                {
                    HealthRankAndValueId = masterId,
                    FilePath = filePath,
                    FileName = newFileName,
                    Description = description,
                    ShortPath = shortPath,
                    FileExtension = fileExtension
                };

                db.HealthAttachments.Add(entity);
                db.SaveChanges();
            }

            return View("Success");
        }
        public ActionResult Upload_MultipleHealthFile(int masterId,  HttpPostedFileBase[] letter, string description)
        {
            for (int i = 0; i < letter.Count(); i++)
            {
                HttpPostedFileBase oneFile = letter[i];
                if (oneFile != null && oneFile.ContentLength > 0)
                {
                    string path = HttpContext.Server.MapPath("~/Content/Files/Health/");
                    string shortPath = "/Content/Files/Health/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string newFileName = Path.GetFileNameWithoutExtension(oneFile.FileName);
                    var fileExtension = System.IO.Path.GetExtension(oneFile.FileName);
                    string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                    newFileName = newFileName + name + fileExtension;
                    string filePath = path + newFileName;

                    oneFile.SaveAs(filePath);

                    var entity = new HealthAttachment
                    {
                        HealthRankAndValueId = masterId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.HealthAttachments.Add(entity);
                    db.SaveChanges();
                }
            }

            return View("Success");
        }
        public ActionResult DownloadAttachment(int healthAttachmentId)
        {


            var record = db.HealthAttachments.Find(healthAttachmentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
                   
           
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
