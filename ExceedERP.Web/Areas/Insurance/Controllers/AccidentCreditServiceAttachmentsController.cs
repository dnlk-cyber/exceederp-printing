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
using ExceedERP.Core.Domain.Insurance.InsuranceNew;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
    public class AccidentCreditServiceAttachmentsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult AccidentCreditServiceAttachments_Read([DataSourceRequest]DataSourceRequest request, int eACSId)
        {
            var attachments = db.AccidentCreditServiceAttachments.Where(x => x.EmployeeAccidentCreditServiceId == eACSId).ToList();
            DataSourceResult result = attachments.ToDataSourceResult(request, attachment => new
            {
                AccidentCreditServiceAttachmentId = attachment.AccidentCreditServiceAttachmentId,
                EmployeeAccidentCreditServiceId = attachment.EmployeeAccidentCreditServiceId,
                FilePath = attachment.FilePath,
                FileName = attachment.FileName,
                ShortPath = attachment.ShortPath,
                FileExtension = attachment.FileExtension,
                Description = attachment.Description
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccidentCreditServiceAttachments_Create([DataSourceRequest]DataSourceRequest request, AccidentCreditServiceAttachment attachment, int eACSId)
        {
            if (ModelState.IsValid)
            {
                attachment.EmployeeAccidentCreditServiceId = eACSId;
                var entity = GetMappedObject(attachment);
                db.AccidentCreditServiceAttachments.Add(entity);
                db.SaveChanges();
                attachment.AccidentCreditServiceAttachmentId = entity.AccidentCreditServiceAttachmentId;
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccidentCreditServiceAttachments_Update([DataSourceRequest]DataSourceRequest request, AccidentCreditServiceAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(attachment);
                db.AccidentCreditServiceAttachments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccidentCreditServiceAttachments_Destroy([DataSourceRequest]DataSourceRequest request, AccidentCreditServiceAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(attachment);
                db.AccidentCreditServiceAttachments.Attach(entity);
                db.AccidentCreditServiceAttachments.Remove(entity);
                db.SaveChanges();
                RemoveFiles(attachment.FilePath);
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        private AccidentCreditServiceAttachment GetMappedObject(AccidentCreditServiceAttachment attachment)
        {
            var entity = new AccidentCreditServiceAttachment
            {
                AccidentCreditServiceAttachmentId = attachment.AccidentCreditServiceAttachmentId,
                EmployeeAccidentCreditServiceId = attachment.EmployeeAccidentCreditServiceId,
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
                var physicalPath = Path.Combine(Server.MapPath("/Content/Health/MedicalFiles/"), fileName);

                if (System.IO.File.Exists(physicalPath))
                {
                    // Deleting the File from the Location
                    System.IO.File.Delete(physicalPath);
                }
            }

            return Content("");
        }
     
        public ActionResult UploadIncomingMedicalFiles(int masterId, HttpPostedFileBase file, string Description)
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
                var fileExtension = Path.GetExtension(file.FileName);
                string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                newFileName = name + newFileName;
                string filePath = path + newFileName;

                file.SaveAs(filePath);

                var entity = new AccidentCreditServiceAttachment
                {
                    EmployeeAccidentCreditServiceId = masterId,
                    FilePath = filePath,
                    FileName = newFileName,
                    Description = Description,
                    ShortPath = shortPath,
                    FileExtension = fileExtension
                };

                db.AccidentCreditServiceAttachments.Add(entity);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "EmployeeAccidentCreditServices");
        }
        public ActionResult Upload_MultipleMedicalFile(int masterId,  HttpPostedFileBase[] letter, string description)
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
                    var fileExtension = Path.GetExtension(oneFile.FileName);
                    string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                    newFileName = newFileName + name + fileExtension;
                    string filePath = path + newFileName;

                    oneFile.SaveAs(filePath);

                    var entity = new AccidentCreditServiceAttachment
                    {
                        EmployeeAccidentCreditServiceId = masterId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.AccidentCreditServiceAttachments.Add(entity);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "EmployeeAccidentCreditServices");
        }
        public ActionResult DownloadAttachment(int attachmentId)
        {
            
            var record = db.AccidentCreditServiceAttachments.Find(attachmentId);
            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
                   
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
