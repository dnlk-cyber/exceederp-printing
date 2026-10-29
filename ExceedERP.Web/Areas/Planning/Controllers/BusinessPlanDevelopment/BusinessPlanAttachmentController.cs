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

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    public class BusinessPlanAttachmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult BusinessPlanAttachment_Read([DataSourceRequest]DataSourceRequest request, int eACSId)
        {
            var attachments = db.BusinessPlanAttachment.Where(x => x.BusinessPlanId == eACSId).ToList();
            DataSourceResult result = attachments.ToDataSourceResult(request, attachment => new
            {
                BusinessPlanAttachmentId = attachment.BusinessPlanAttachmentId,
                BusinessPlanId = attachment.BusinessPlanId,
                FilePath = attachment.FilePath,
                FileName = attachment.FileName,
                ShortPath = attachment.ShortPath,
                FileExtension = attachment.FileExtension,
                Description = attachment.Description
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BusinessPlanAttachment_Create([DataSourceRequest]DataSourceRequest request, BusinessPlanAttachment attachment, int eACSId)
        {
            if (ModelState.IsValid)
            {
                attachment.BusinessPlanId = eACSId;
                var entity = GetMappedObject(attachment);
                db.BusinessPlanAttachment.Add(entity);
                db.SaveChanges();
                attachment.BusinessPlanAttachmentId = entity.BusinessPlanAttachmentId;
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BusinessPlanAttachment_Update([DataSourceRequest]DataSourceRequest request, BusinessPlanAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(attachment);
                db.BusinessPlanAttachment.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BusinessPlanAttachment_Destroy([DataSourceRequest]DataSourceRequest request, BusinessPlanAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(attachment);
                db.BusinessPlanAttachment.Attach(entity);
                db.BusinessPlanAttachment.Remove(entity);
                db.SaveChanges();
                RemoveFiles(attachment.FilePath);
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        private BusinessPlanAttachment GetMappedObject(BusinessPlanAttachment attachment)
        {
            var entity = new BusinessPlanAttachment
            {
                BusinessPlanAttachmentId = attachment.BusinessPlanAttachmentId,
                BusinessPlanId = attachment.BusinessPlanId,
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
                string path = HttpContext.Server.MapPath("~/Content/Planning/BusinessPlan/Files/");
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

                var entity = new BusinessPlanAttachment
                {
                    BusinessPlanId = masterId,
                    FilePath = filePath,
                    FileName = newFileName,
                    Description = Description,
                    ShortPath = shortPath,
                    FileExtension = fileExtension
                };

                db.BusinessPlanAttachment.Add(entity);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "BusinessPlans");
        }
        public ActionResult Upload_MultipleMedicalFile(int masterId,  HttpPostedFileBase[] letter, string description)
        {
            for (int i = 0; i < letter.Count(); i++)
            {
                HttpPostedFileBase oneFile = letter[i];
                if (oneFile != null && oneFile.ContentLength > 0)
                {
                    string path = HttpContext.Server.MapPath("~/Content/Planning/BusinessPlan/Files/");
                    string shortPath = "/Content/Planning/BusinessPlan/Files/";
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

                    var entity = new BusinessPlanAttachment
                    {
                        BusinessPlanId = masterId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.BusinessPlanAttachment.Add(entity);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "BusinessPlans");
        }
        public ActionResult DownloadAttachment(int attachmentId)
        {
            
            var record = db.BusinessPlanAttachment.Find(attachmentId);
            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
                   
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
