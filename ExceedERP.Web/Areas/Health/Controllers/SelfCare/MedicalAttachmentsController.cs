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
    public class MedicalAttachmentsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult MedicalAttachments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var healthAttachments = db.MedicalAttachments.Where(x => x.MedicalRequestId == id ).ToList();
            DataSourceResult result = healthAttachments.ToDataSourceResult(request, healthAttachment => new
            {
                MedicalAttachmentId = healthAttachment.MedicalAttachmentId,
                MedicalRequestId = healthAttachment.MedicalRequestId,
                FilePath = healthAttachment.FilePath,
                FileName = healthAttachment.FileName,
                ShortPath = healthAttachment.ShortPath,
                FileExtension = healthAttachment.FileExtension,
                Description = healthAttachment.Description
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MedicalAttachments_Create([DataSourceRequest]DataSourceRequest request, MedicalAttachment medicalAttachment, int id)
        {
            if (ModelState.IsValid)
            {
                medicalAttachment.MedicalRequestId = id;
                var entity = GetMappedObject(medicalAttachment);
                db.MedicalAttachments.Add(entity);
                db.SaveChanges();
                medicalAttachment.MedicalAttachmentId = entity.MedicalAttachmentId;
            }

            return Json(new[] { medicalAttachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MedicalAttachments_Update([DataSourceRequest]DataSourceRequest request, MedicalAttachment medicalAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(medicalAttachment);
                db.MedicalAttachments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { medicalAttachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MedicalAttachments_Destroy([DataSourceRequest]DataSourceRequest request, MedicalAttachment medicalAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(medicalAttachment);
                db.MedicalAttachments.Attach(entity);
                db.MedicalAttachments.Remove(entity);
                db.SaveChanges();
                RemoveFiles(medicalAttachment.FilePath);
            }

            return Json(new[] { medicalAttachment }.ToDataSourceResult(request, ModelState));
        }
        private MedicalAttachment GetMappedObject(MedicalAttachment medicalAttachment)
        {
            var entity = new MedicalAttachment
            {
                MedicalAttachmentId = medicalAttachment.MedicalAttachmentId,
                MedicalRequestId = medicalAttachment.MedicalRequestId,
                FilePath = medicalAttachment.FilePath,
                FileName = medicalAttachment.FileName,
                ShortPath = medicalAttachment.ShortPath,
                FileExtension = medicalAttachment.FileExtension,
                Description = medicalAttachment.Description
            };
            return entity;
        }
        public ActionResult RemoveFiles(string fileName)
        {
            if (fileName != null)
            {
                var physicalPath = Path.Combine(Server.MapPath("/Content/Health/MedicalFiles/"), fileName);

                if (System.IO.File.Exists(physicalPath))
                {
                    // Deleting the File from the Location
                    System.IO.File.Delete(physicalPath);
                }
            }

            return Content("");
        }
     
        public ActionResult UploadIncomingMedicalFiles(int masterId, HttpPostedFileBase file, string description)
        {
            if (file != null && file.ContentLength > 0)
            {
                string path = HttpContext.Server.MapPath("~/Content/Health/MedicalFiles/");
                string shortPath = "/Content/Health/MedicalFiles/";
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

                var entity = new MedicalAttachment
                {
                    MedicalRequestId = masterId,
                    FilePath = filePath,
                    FileName = newFileName,
                    Description = description,
                    ShortPath = shortPath,
                    FileExtension = fileExtension
                };

                db.MedicalAttachments.Add(entity);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public JsonResult Upload_MultipleMedicalFile(int masterId,  HttpPostedFileBase[] letter, string description)
        {
            for (int i = 0; i < letter.Count(); i++)
            {
                HttpPostedFileBase oneFile = letter[i];
                if (oneFile != null && oneFile.ContentLength > 0)
                {
                    string path = HttpContext.Server.MapPath("~/Content/Health/MedicalFiles/");
                    string shortPath = "/Content/Health/MedicalFiles/";
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

                    var entity = new MedicalAttachment
                    {
                        MedicalRequestId = masterId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.MedicalAttachments.Add(entity);
                    db.SaveChanges();
                }
            }           

            return Json("Successfull");
        }
        public ActionResult DownloadAttachment(int medicalAttachmentId)
        {


            var record = db.MedicalAttachments.Find(medicalAttachmentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
                   
           
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
