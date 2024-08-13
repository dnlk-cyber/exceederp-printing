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
    public class MedicalExpenseRefundAttachmentsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult MedicalExpenseRefundAttachments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var healthAttachments = db.MedicalExpenseRefundAttachments.Where(x => x.EmployeeMedicalExpenseRefundId == id ).ToList();
            DataSourceResult result = healthAttachments.ToDataSourceResult(request, healthAttachment => new
            {
                MedicalExpenseRefundAttachmentId = healthAttachment.MedicalExpenseRefundAttachmentId,
                EmployeeMedicalExpenseRefundId = healthAttachment.EmployeeMedicalExpenseRefundId,
                FilePath = healthAttachment.FilePath,
                FileName = healthAttachment.FileName,
                ShortPath = healthAttachment.ShortPath,
                FileExtension = healthAttachment.FileExtension,
                Description = healthAttachment.Description
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MedicalExpenseRefundAttachments_Create([DataSourceRequest]DataSourceRequest request, MedicalExpenseRefundAttachment medicalAttachment, int id)
        {
            if (ModelState.IsValid)
            {
                medicalAttachment.EmployeeMedicalExpenseRefundId = id;
                var entity = GetMappedObject(medicalAttachment);
                db.MedicalExpenseRefundAttachments.Add(entity);
                db.SaveChanges();
                medicalAttachment.MedicalExpenseRefundAttachmentId = entity.MedicalExpenseRefundAttachmentId;
            }

            return Json(new[] { medicalAttachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MedicalExpenseRefundAttachments_Update([DataSourceRequest]DataSourceRequest request, MedicalExpenseRefundAttachment medicalAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(medicalAttachment);
                db.MedicalExpenseRefundAttachments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { medicalAttachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MedicalExpenseRefundAttachments_Destroy([DataSourceRequest]DataSourceRequest request, MedicalExpenseRefundAttachment medicalAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(medicalAttachment);
                db.MedicalExpenseRefundAttachments.Attach(entity);
                db.MedicalExpenseRefundAttachments.Remove(entity);
                db.SaveChanges();
                RemoveFiles(medicalAttachment.FilePath);
            }

            return Json(new[] { medicalAttachment }.ToDataSourceResult(request, ModelState));
        }
        private MedicalExpenseRefundAttachment GetMappedObject(MedicalExpenseRefundAttachment medicalAttachment)
        {
            var entity = new MedicalExpenseRefundAttachment
            {
                MedicalExpenseRefundAttachmentId = medicalAttachment.MedicalExpenseRefundAttachmentId,
                EmployeeMedicalExpenseRefundId = medicalAttachment.EmployeeMedicalExpenseRefundId,
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

                var entity = new MedicalExpenseRefundAttachment
                {
                    EmployeeMedicalExpenseRefundId = masterId,
                    FilePath = filePath,
                    FileName = newFileName,
                    Description = description,
                    ShortPath = shortPath,
                    FileExtension = fileExtension
                };

                db.MedicalExpenseRefundAttachments.Add(entity);
                db.SaveChanges();
            }

            return RedirectToAction("index", "EmployeeMedicalExpenseRefundPersonals");
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
                    var fileExtension = System.IO.Path.GetExtension(oneFile.FileName);
                    string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                    newFileName = newFileName + name + fileExtension;
                    string filePath = path + newFileName;

                    oneFile.SaveAs(filePath);

                    var entity = new MedicalExpenseRefundAttachment
                    {
                        EmployeeMedicalExpenseRefundId = masterId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.MedicalExpenseRefundAttachments.Add(entity);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("index", "EmployeeMedicalExpenseRefundPersonals");
        }
        public ActionResult DownloadAttachment(int medicalAttachmentId)
        {


            var record = db.MedicalExpenseRefundAttachments.Find(medicalAttachmentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
                   
           
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
