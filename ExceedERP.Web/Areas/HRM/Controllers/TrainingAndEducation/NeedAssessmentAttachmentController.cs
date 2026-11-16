using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using System.IO;
using ExceedERP.Core.Domain.Global;
using System.Web;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    public class NeedAssessmentAttachmentController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult NeedAssessmentAttachment_Read([DataSourceRequest] DataSourceRequest request,int id)
        {
            var attachments = db.NeedAssessmentAttachments
                .Where(x => x.NeedAssessmentId == id)
                .ToList();
            DataSourceResult result = attachments.ToDataSourceResult(request, attachment => new
            {
                NeedAssessmentAttachmentId = attachment.NeedAssessmentAttachmentId,

                NeedAssessmentId =  attachment.NeedAssessmentId,

                FileExtension = attachment.FileExtension,

                Description = attachment.Description,

                FilePath = attachment.FilePath,

                FileName = attachment.FileName,

                ShortPath = attachment.ShortPath,

                ContentType = attachment.ContentType,

                FileSize = attachment.FileSize
            });

            return Json(result);
           
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NeedAssessmentAttachment_Create(
            [DataSourceRequest] DataSourceRequest request,
            IEnumerable<HttpPostedFileBase> file,
            int id)
        {
            if (file != null)
            {
                string[] allowedExtensions =
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".pdf",
                    ".doc",
                    ".docx",
                    ".xls",
                    ".xlsx"
                };

                int maxFileSize = 10 * 1024 * 1024; // 10 MB

                string subPath =
                    "~/Content/HRM/NeedAssessmentFile/";

                string shortPath =
                    "/Content/HRM/NeedAssessmentFile/";

                string physicalDirectory =
                    Server.MapPath(subPath);

                if (!Directory.Exists(physicalDirectory))
                {
                    Directory.CreateDirectory(physicalDirectory);
                }

                string fileName = "";

                foreach (var f in file)
                {
                    if (f == null)
                        continue;

                    #region File Validation

                    var fileExtension =
                        Path.GetExtension(f.FileName)?.ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError(
                            "File",
                            "Invalid file type.");

                        return Json(
                            new[] { file }
                                .ToDataSourceResult(
                                    request,
                                    ModelState));
                    }

                    if (f.ContentLength > maxFileSize)
                    {
                        ModelState.AddModelError(
                            "File",
                            $"The uploaded file exceeds the maximum allowed size of {maxFileSize / (1024 * 1024)} MB.");

                        return Json(
                            new[] { file }
                                .ToDataSourceResult(
                                    request,
                                    ModelState));
                    }

                    #endregion

                    if (ModelState.IsValid)
                    {
                        fileName =
                            Path.GetFileName(f.FileName);

                        string physicalPath =
                            Path.Combine(
                                physicalDirectory,
                                fileName);

                        f.SaveAs(physicalPath);

                        var attachment =
                            new NeedAssessmentAttachment
                            {
                                NeedAssessmentId = id,

                                FileName = fileName,

                                FilePath = physicalPath,

                                ShortPath = shortPath,

                                FileExtension =
                                    fileExtension,

                                Description =
                                    "Need Assessment Attachment",

                                ContentType =
                                    f.ContentType,

                                FileSize =
                                    f.ContentLength
                            };

                        db.NeedAssessmentAttachments
                            .Add(attachment);

                        db.SaveChanges();
                    }
                }

                #region Operation Log

                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Need Assessment Attachment",
                    "Success",
                    User.Identity.Name,
                    $"Uploaded Need Assessment attachment {fileName}",
                    Modules.HRM);

                #endregion
            }

            return Json(
                new[] { file }
                    .ToDataSourceResult(
                        request,
                        ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NeedAssessmentAttachment_Update(
            [DataSourceRequest] DataSourceRequest request,
            NeedAssessmentAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity =
                    new NeedAssessmentAttachment
                    {
                        NeedAssessmentAttachmentId =
                            attachment.NeedAssessmentAttachmentId,

                        NeedAssessmentId =
                            attachment.NeedAssessmentId,

                        FileExtension =
                            attachment.FileExtension,

                        Description =
                            attachment.Description,

                        FilePath =
                            attachment.FilePath,

                        FileName =
                            attachment.FileName,

                        ShortPath =
                            attachment.ShortPath,

                        ContentType =
                            attachment.ContentType,

                        FileSize =
                            attachment.FileSize
                    };

                db.NeedAssessmentAttachments.Attach(entity);

                db.Entry(entity).State =
                    EntityState.Modified;

                db.SaveChanges();
            }

            return Json(
                new[] { attachment }
                    .ToDataSourceResult(
                        request,
                        ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NeedAssessmentAttachment_Destroy(
            [DataSourceRequest] DataSourceRequest request,
            NeedAssessmentAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity =
                    db.NeedAssessmentAttachments
                        .Find(
                            attachment.NeedAssessmentAttachmentId);

                if (entity != null)
                {
                    db.NeedAssessmentAttachments.Remove(entity);

                    db.SaveChanges();

                    RemoveFiles(entity.FileName);
                }
            }

            return Json(
                new[] { attachment }
                    .ToDataSourceResult(
                        request,
                        ModelState));
        }

        public ActionResult RemoveFiles(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                var physicalPath =
                    Path.Combine(
                        Server.MapPath(
                            "~/Content/HRM/NeedAssessmentFile/"),
                        fileName);

                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }

            return Content("");
        }

        public ActionResult DownloadAttachment(int attachmentId)
        {
            var record =
                db.NeedAssessmentAttachments
                    .Find(attachmentId);

            if (record == null ||
                string.IsNullOrEmpty(record.FilePath) ||
                !System.IO.File.Exists(record.FilePath))
            {
                return HttpNotFound();
            }

            return File(
                record.FilePath,
                record.ContentType ?? "application/octet-stream",
                Path.GetFileName(record.FilePath));
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
