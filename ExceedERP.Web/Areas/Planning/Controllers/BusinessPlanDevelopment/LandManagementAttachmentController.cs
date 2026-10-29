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
using ExceedERP.Core.Domain.Planning.BusinessPlanDevelopment;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    public class LandManagementAttachmentController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult LandManagementAttachment_Read([DataSourceRequest]DataSourceRequest request, int eACSId)
        {
            var attachments = db.LandManagementAttachment.Where(x => x.LandManagementId == eACSId).ToList();
            DataSourceResult result = attachments.ToDataSourceResult(request, attachment => new
            {

                LandManagementAttachmentId = attachment.LandManagementAttachmentId,
                LandManagementId = attachment.LandManagementId,
                FilePath = attachment.FilePath,
                FileName = attachment.FileName,
                ShortPath = attachment.ShortPath,
                FileExtension = attachment.FileExtension,
                Description = attachment.Description

            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LandManagementAttachment_Create([DataSourceRequest]DataSourceRequest request, LandManagementAttachment attachment, int eACSId)
        {
            if (ModelState.IsValid)
            {
                attachment.LandManagementId = eACSId;
                var entity = GetMappedObject(attachment);
                db.LandManagementAttachment.Add(entity);
                db.SaveChanges();
                attachment.LandManagementAttachmentId = entity.LandManagementAttachmentId;
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LandManagementAttachment_Update([DataSourceRequest]DataSourceRequest request, LandManagementAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(attachment);
                db.LandManagementAttachment.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LandManagementAttachment_Destroy([DataSourceRequest]DataSourceRequest request, LandManagementAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(attachment);
                db.LandManagementAttachment.Attach(entity);
                db.LandManagementAttachment.Remove(entity);
                db.SaveChanges();
                RemoveFiles(attachment.FilePath);
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        private LandManagementAttachment GetMappedObject(LandManagementAttachment attachment)
        {
            var entity = new LandManagementAttachment
            {
                LandManagementAttachmentId = attachment.LandManagementAttachmentId,
                LandManagementId = attachment.LandManagementId,
                FilePath = attachment.FilePath,
                FileName = attachment.FileName,
                ShortPath = attachment.ShortPath,
                FileExtension = attachment.FileExtension,
                Description = attachment.Description

            };
            return entity;
        }
        public ActionResult RemoveFiles(string fileName)
        {
            if (fileName != null)
            {
                var physicalPath = Path.Combine(Server.MapPath("/Content/Planning/BusinessPlan/MedicalFiles/"), fileName);

                if (System.IO.File.Exists(physicalPath))
                {
                    // Deleting the File from the Location
                    System.IO.File.Delete(physicalPath);
                }
            }

            return Content("");
        }
        public ActionResult UploadFiles(int masterId, HttpPostedFileBase file, string Description)
        {
            if (file != null && file.ContentLength > 0)
            {
                string path = HttpContext.Server.MapPath("~/Content/Planning/LandManagementFiles/");
                string shortPath = "/Content/Planning/BusinessPlan/Files/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string newFileName = Path.GetExtension(file.FileName);
                var fileExtension = Path.GetExtension(file.FileName);
                string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                newFileName = name + newFileName;
                string filePath = path + newFileName;

                file.SaveAs(filePath);

                var entity = new LandManagementAttachment
                {
                    LandManagementId = masterId,
                    FilePath = filePath,
                    FileName = newFileName,
                    Description = Description,
                    ShortPath = shortPath,
                    FileExtension = fileExtension
                };

                db.LandManagementAttachment.Add(entity);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "BusinessPlans");
        }
        public ActionResult Upload_MultipleFile(int masterId,  HttpPostedFileBase[] letter, string description)
        {
            for (int i = 0; i < letter.Count(); i++)
            {
                HttpPostedFileBase oneFile = letter[i];
                if (oneFile != null && oneFile.ContentLength > 0)
                {
                    string path = HttpContext.Server.MapPath("~/Content/Planning/LandManagementFiles/");
                    string shortPath = "/Content/Planning/LandManagementFiles/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string newFileName = Path.GetFileNameWithoutExtension(oneFile.FileName);
                    var fileExtension = Path.GetExtension(oneFile.FileName);
                    string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                    newFileName = newFileName + name + fileExtension;
                    string filePath = path + newFileName;
                    oneFile.SaveAs(filePath);
                    var entity = new LandManagementAttachment
                    {
                        LandManagementId = masterId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };
                    db.LandManagementAttachment.Add(entity);
                    db.SaveChanges();

                }
            }

            return RedirectToAction("Index", "LandManagements");
        }
        public ActionResult DownloadAttachment(int attachmentId)
        {
            
            var record = db.LandManagementAttachment.Find(attachmentId);
            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
                   
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
