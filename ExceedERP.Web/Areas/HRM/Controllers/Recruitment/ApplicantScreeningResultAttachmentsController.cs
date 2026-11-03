using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using System.IO;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class ApplicantScreeningResultAttachmentsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult ApplicantScreeningResultAttachments_Read([DataSourceRequest]DataSourceRequest request, int id, string applicantType)
        {

            var attachments = db.ApplicantScreeningResultAttachments.Where(x => x.InternalApplicantScreeningResultId == id ).AsQueryable();
            if(applicantType == "External")
            {
                attachments = db.ApplicantScreeningResultAttachments.Where(x => x.ExternalApplicantScreeningResultId == id).AsQueryable();
            }
            DataSourceResult result = attachments.ToDataSourceResult(request, attachment => new
            {
                ApplicantScreeningResultAttachmentId = attachment.ApplicantScreeningResultAttachmentId,
                InternalApplicantScreeningResultId = attachment.InternalApplicantScreeningResultId,
                IsInternal = attachment.IsInternal,
                FilePath = attachment.FilePath,
                FileName = attachment.FileName,
                ShortPath = attachment.ShortPath,
                FileExtension = attachment.FileExtension,
                Description = attachment.Description
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantScreeningResultAttachments_Create([DataSourceRequest] DataSourceRequest request,
                                    IEnumerable<HttpPostedFileBase> files,int id, string applicantType)
        {
            int? internalApplicantScreeningResultId = null;
            int? externalApplicantScreeningResultId = null;
            if (files == null || !files.Any())
            {
                ModelState.AddModelError("File", "No file selected.");
                return Json(new[] { files }.ToDataSourceResult(request, ModelState));
            }

            string[] allowedExtensions = { ".jpg", ".png", ".pdf" };
            int maxFileSize = 2 * 1024 * 1024; // 2 MB

            string subPath = "~/App_Data/HRM/RecruitmentFiles/";
            string shortPath = "/App_Data/HRM/RecruitmentFiles/";
            string physicalFolder = Server.MapPath(subPath);

            if (!Directory.Exists(physicalFolder))
                Directory.CreateDirectory(physicalFolder);

            string uploadedFileName = null;
            if (applicantType == "External")
                externalApplicantScreeningResultId = id;
            else
                internalApplicantScreeningResultId = id;


            foreach (var file in files)
            {
                if (file == null || file.ContentLength == 0)
                    continue;

                var fileExtension = Path.GetExtension(file.FileName)?.ToLowerInvariant();

                // Validate extension
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("File", "Invalid file type. Only .jpg, .png, .pdf are allowed.");
                    continue;
                }

                // Validate size
                if (file.ContentLength > maxFileSize)
                {
                    ModelState.AddModelError("File", "File size exceeds the 2 MB limit.");
                    continue;
                }
              

                if (ModelState.IsValid)
                {
                    uploadedFileName = Path.GetFileName(file.FileName);

                    // To avoid overwriting, add unique suffix if file exists
                    string safeFileName = Path.GetFileNameWithoutExtension(uploadedFileName);
                    string finalFileName = uploadedFileName;
                    string physicalPath = Path.Combine(physicalFolder, finalFileName);

                    int counter = 1;
                    while (System.IO.File.Exists(physicalPath))
                    {
                        finalFileName = $"{safeFileName}_{counter}{fileExtension}";
                        physicalPath = Path.Combine(physicalFolder, finalFileName);
                        counter++;
                    }

                    file.SaveAs(physicalPath);

                    db.ApplicantScreeningResultAttachments.Add(new ApplicantScreeningResultAttachment
                    {
                        FileName = finalFileName,
                        InternalApplicantScreeningResultId = internalApplicantScreeningResultId,
                        ExternalApplicantScreeningResultId = externalApplicantScreeningResultId,
                        ShortPath = shortPath,
                        FileExtension = fileExtension,
                        Description = "Attachment",
                        FilePath = physicalPath
                    });

                    db.SaveChanges();
                }
            }

            // Log operation if at least one file was uploaded
            if (!string.IsNullOrEmpty(uploadedFileName))
            {
                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "ApplicantScreeningResultAttachments Attachment Upload",
                    "Success",
                    User.Identity.Name,
                    $"Uploaded Applicant Screening Result Attachment: {uploadedFileName}",
                    Modules.Insurance);
            }

            return Json(new[] { files }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantScreeningResultAttachments_CreateExisted([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file, int id)
        {
            if (file != null)
            {
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                int MaxFileSize = 2 * 1024 * 1024;

                //string subPath = "~/Content/HRM/Recruitment/Files/";
                string subPath = "~/App_Data/HRM/RecruitmentFiles/";
                string shortPath = "/App_Data/HRM/RecruitmentFiles/";
                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
                string fileName = "";

                foreach (var f in file)
                {

                    if (f != null)
                    {
                        #region file validation
                        var fileExtension = Path.GetExtension(f.FileName).ToLower();
                        if (!AllowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("File", "Invalid file type.");
                        }

                        if (f.ContentLength > MaxFileSize)
                        {
                            ModelState.AddModelError("File", "File size exceeds the 2 MB limit.");
                        }
                        #endregion
                        if (ModelState.IsValid)
                        {
                            fileName = Path.GetFileName(f.FileName);
                            string physicalPath = Server.MapPath(subPath + "/" + fileName);
                            //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                            f.SaveAs(physicalPath);
                            db.ApplicantScreeningResultAttachments.Add(new ApplicantScreeningResultAttachment()
                            {
                                FileName = Path.GetFileName(f.FileName),
                                InternalApplicantScreeningResultId = id,
                                ShortPath = shortPath,
                                FileExtension = fileExtension,
                                Description = "Attachment",
                                FilePath = physicalPath

                            });
                            db.SaveChanges();
                        }
                    }
                }


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "ApplicantScreeningResultAttachments Attachment template", "Success", User.Identity.Name, $"uploaded Applicant Screening Result Attachments template {fileName} ", Modules.Insurance);
                #endregion

            }
            return Json(new[] { file }.ToDataSourceResult(request, ModelState));

        }

       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantScreeningResultAttachments_Update([DataSourceRequest]DataSourceRequest request, ApplicantScreeningResultAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(attachment);
                db.ApplicantScreeningResultAttachments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantScreeningResultAttachments_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantScreeningResultAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(attachment);
                db.ApplicantScreeningResultAttachments.Attach(entity);
                db.ApplicantScreeningResultAttachments.Remove(entity);
                db.SaveChanges();
                RemoveFiles(attachment.FilePath);
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        private ApplicantScreeningResultAttachment GetMappedObject(ApplicantScreeningResultAttachment attachment)
        {
            var entity = new ApplicantScreeningResultAttachment
            {
                ApplicantScreeningResultAttachmentId = attachment.ApplicantScreeningResultAttachmentId,
                InternalApplicantScreeningResultId = attachment.InternalApplicantScreeningResultId,
                IsInternal = attachment.IsInternal,
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
     
        public ActionResult UploadFiles(int masterId, HttpPostedFileBase file, string description)
        {
            if (file != null && file.ContentLength > 0)
            {
                string path = HttpContext.Server.MapPath("~/Content/HRM/Recruitment/Files/");
                string shortPath = "/Content/HRM/Recruitment/Files/";
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

                var entity = new ApplicantScreeningResultAttachment
                {
                    InternalApplicantScreeningResultId = masterId,
                    FilePath = filePath,
                    FileName = newFileName,
                    Description = description,
                    ShortPath = shortPath,
                    FileExtension = fileExtension
                };

                db.ApplicantScreeningResultAttachments.Add(entity);
                db.SaveChanges();
            }

            return RedirectToAction("index", "InsurancePolicyRenewalPersonals");
            //return Json("Successfull");
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
                    var fileExtension = Path.GetExtension(oneFile.FileName);
                    string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                    newFileName = newFileName + name + fileExtension;
                    string filePath = path + newFileName;

                    oneFile.SaveAs(filePath);

                    var entity = new ApplicantScreeningResultAttachment
                    {
                        InternalApplicantScreeningResultId = masterId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.ApplicantScreeningResultAttachments.Add(entity);
                    db.SaveChanges();
                }
            }           

            return Json("Successfull");
        }
        public ActionResult DownloadAttachment(int attachmentId)
        {
            var file = db.ApplicantScreeningResultAttachments.FirstOrDefault(x => x.ApplicantScreeningResultAttachmentId == attachmentId);
            if (file == null || !System.IO.File.Exists(file.FilePath))
                return HttpNotFound();

            string contentType = MimeMapping.GetMimeMapping(file.FileName);
            return File(file.FilePath, contentType, file.FileName);
            //var record = db.ApplicantScreeningResultAttachments.Find(attachmentId);
            //return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));

        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
