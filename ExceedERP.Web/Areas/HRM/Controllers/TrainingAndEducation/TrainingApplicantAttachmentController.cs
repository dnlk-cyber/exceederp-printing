using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingEducation
{
    public class TrainingApplicantAttachmentController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TrainingApplicantAttachments_Read([DataSourceRequest] DataSourceRequest request, int trainingApplicantId)
        {
            IQueryable<TrainingApplicantAttachment> attachments = db.TrainingApplicantAttachments
                .Where(a => a.TrainingApplicantId == trainingApplicantId);

            DataSourceResult result = attachments.ToDataSourceResult(request, attachment => new
            {
                TrainingApplicantAttachmentId = attachment.TrainingApplicantAttachmentId,
                TrainingApplicantId = attachment.TrainingApplicantId,
                FileName = attachment.FileName,
                Description = attachment.Description,
                FilePath = attachment.FilePath,
                ShortPath = attachment.ShortPath,
                FileExtension = attachment.FileExtension,
                CreatedBy = attachment.CreatedBy,
                DateCreated = attachment.DateCreated,
                LastModified = attachment.LastModified,
                ModifiedBy = attachment.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingApplicantAttachments_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file, int id)
        {
            if (file != null)
            {
                string[] allowedExtensions = { ".jpg", ".png", ".pdf" };
                int maxFileSize = 50 * 1024 * 1024; // 50 MB to match globalOnSelect parameter

                string folderVirtualPath = "~/Content/Images/HRMFiles/TrainingApplicant/";
                string folderPhysicalPath = Server.MapPath(folderVirtualPath);
                string shortPath = "/Content/Images/HRMFiles/TrainingApplicant/";

                if (!Directory.Exists(folderPhysicalPath))
                {
                    Directory.CreateDirectory(folderPhysicalPath);
                }

                string fileName = "";

                foreach (var f in file)
                {
                    if (f != null)
                    {
                        var fileExtension = Path.GetExtension(f.FileName).ToLower();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("File", "Invalid file format. Allowed formats: " + string.Join(", ", allowedExtensions));
                            return Json(new[] { f }.ToDataSourceResult(request, ModelState));
                        }

                        if (f.ContentLength > maxFileSize)
                        {
                            ModelState.AddModelError("File", $"File exceeds the maximum limit of {maxFileSize / (1024 * 1024)} MB.");
                            return Json(new[] { f }.ToDataSourceResult(request, ModelState));
                        }

                        if (ModelState.IsValid)
                        {
                            // Avoid overwriting by prefixing a unique timestamp
                            string rawFileName = Path.GetFileNameWithoutExtension(f.FileName);
                            fileName = $"{rawFileName}_{DateTime.Now:yyyyMMddHHmmssfff}{fileExtension}";
                            string fullPhysicalSavePath = Path.Combine(folderPhysicalPath, fileName);

                            f.SaveAs(fullPhysicalSavePath);

                            var attachment = new TrainingApplicantAttachment
                            {
                                TrainingApplicantId = id,
                                FileName = fileName,
                                Description = "Training Applicant Attachment",
                                FilePath = fullPhysicalSavePath,
                                ShortPath = shortPath,
                                FileExtension = fileExtension,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now,
                                ModifiedBy = User.Identity.Name,
                                LastModified = DateTime.Now
                            };

                            db.TrainingApplicantAttachments.Add(attachment);
                            db.SaveChanges();

                            #region Operation Log
                            UserHelper.OperationLog(
                                Request.UserHostAddress,
                                "HRM TrainingApplicant Attachment",
                                "Success",
                                User.Identity.Name,
                                $"Uploaded Training Applicant Attachment: {fileName} for Applicant ID: {id}",
                                Modules.HRM
                            );
                            #endregion
                        }
                    }
                }
            }

            return Json(new[] { file }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingApplicantAttachments_Update([DataSourceRequest] DataSourceRequest request, TrainingApplicantAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = db.TrainingApplicantAttachments.Find(attachment.TrainingApplicantAttachmentId);
                if (entity != null)
                {
                    entity.Description = attachment.Description;
                    entity.LastModified = DateTime.Now;
                    entity.ModifiedBy = User.Identity.Name;

                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingApplicantAttachments_Destroy([DataSourceRequest] DataSourceRequest request, TrainingApplicantAttachment attachment)
        {
            var entity = db.TrainingApplicantAttachments.Find(attachment.TrainingApplicantAttachmentId);
            if (entity != null)
            {
                // Remove file from disk
                if (!string.IsNullOrEmpty(entity.FilePath) && System.IO.File.Exists(entity.FilePath))
                {
                    try
                    {
                        System.IO.File.Delete(entity.FilePath);
                    }
                    catch (Exception) { /* Log file access errors if required */ }
                }

                #region Operation Log
                UserHelper.DeleteLog(
                    Request.UserHostAddress,
                    "Delete",
                    "Success",
                    User.Identity.Name,
                    $"Deleted Training Applicant Attachment: {entity.FileName} (ID: {entity.TrainingApplicantAttachmentId})"
                );
                #endregion

                db.TrainingApplicantAttachments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult DownloadAttachment(int attachmentId)
        {
            var record = db.TrainingApplicantAttachments.FirstOrDefault(x => x.TrainingApplicantAttachmentId == attachmentId);
            if (record == null || string.IsNullOrEmpty(record.FilePath) || !System.IO.File.Exists(record.FilePath))
            {
                return HttpNotFound("Attachment file not found.");
            }

            return File(record.FilePath, "application/force-download", record.FileName);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}