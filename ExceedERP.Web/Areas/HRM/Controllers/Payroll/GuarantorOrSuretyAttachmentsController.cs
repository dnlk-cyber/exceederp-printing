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
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    public class GuarantorOrSuretyAttachmentsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

      
        public ActionResult GuarantorOrSuretyAttachments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var suretyAttachments = db.GuarantorOrSuretyAttachments
                                        .Where(x => x.GuarantorOrSuretyId == id )
                                        .ToList();
            DataSourceResult result = suretyAttachments.ToDataSourceResult(request, suretyAttachment => new
            {
                GuarantorOrSuretyAttachmentId = suretyAttachment.GuarantorOrSuretyId,
                GuarantorOrSuretyId = suretyAttachment.GuarantorOrSuretyId,
                ShortPath = suretyAttachment.ShortPath,
                FilePath = suretyAttachment.FilePath,
                FileExtension = suretyAttachment.FileExtension,
                FileName = suretyAttachment.FileName,
                Description = suretyAttachment.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuarantorOrSuretyAttachments_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file, int id)
        {
            if (file != null)
            {
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                int MaxFileSize = 50 * 1024 * 1024;

                string subPath = "~/Content/Files/Archive/";
                string shortPath = "/Content/Files/Archive/";
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
                            ModelState.AddModelError("File", $"The uploaded file exceeds the maximum allowed size of {MaxFileSize / (1024 * 1024)} MB.");
                        }
                        if (db.ArchiveAttachements.Any(x => x.FileName == fileName
                                && x.InstitutionIncomingLetterId == id))
                        {
                            ModelState.AddModelError("File", $"This filename : {fileName } already exists.");
                            return Json(new[] { file }.ToDataSourceResult(request, ModelState));


                        }
                        #endregion
                        if (ModelState.IsValid)
                        {
                            fileName = Path.GetFileName(f.FileName);
                            string physicalPath = Server.MapPath(subPath + "/" + fileName);
                            //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                            f.SaveAs(physicalPath);
                            db.GuarantorOrSuretyAttachments.Add(new GuarantorOrSuretyAttachment()
                            {
                                FileName = Path.GetFileName(f.FileName),
                                GuarantorOrSuretyId = id,
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
                UserHelper.OperationLog(Request.UserHostAddress, "Attachment template", "Success", User.Identity.Name, $"uploaded  Attachment template {fileName} ", Modules.HRM);
                #endregion
            }
            return Json(new[] { file }.ToDataSourceResult(request, ModelState));

        }

      

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuarantorOrSuretyAttachments_Update([DataSourceRequest]DataSourceRequest request, GuarantorOrSuretyAttachment suretyAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuarantorOrSuretyAttachment
                {
                    GuarantorOrSuretyAttachmentId = suretyAttachment.GuarantorOrSuretyId,
                    GuarantorOrSuretyId = suretyAttachment.GuarantorOrSuretyId,
                    ShortPath = suretyAttachment.ShortPath,
                    FilePath = suretyAttachment.FilePath,
                    FileExtension = suretyAttachment.FileExtension,
                    FileName = suretyAttachment.FileName,
                    Description = suretyAttachment.Description
                };

                db.GuarantorOrSuretyAttachments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { suretyAttachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuarantorOrSuretyAttachments_Destroy([DataSourceRequest]DataSourceRequest request, GuarantorOrSuretyAttachment suretyAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuarantorOrSuretyAttachment
                {
                    GuarantorOrSuretyAttachmentId = suretyAttachment.GuarantorOrSuretyId,
                    GuarantorOrSuretyId = suretyAttachment.GuarantorOrSuretyId,
                    ShortPath = suretyAttachment.ShortPath,
                    FilePath = suretyAttachment.FilePath,
                    FileExtension = suretyAttachment.FileExtension,
                    FileName = suretyAttachment.FileName,
                    Description = suretyAttachment.Description
                };


                db.GuarantorOrSuretyAttachments.Attach(entity);
                db.GuarantorOrSuretyAttachments.Remove(entity);
                db.SaveChanges();
                RemoveFiles(suretyAttachment.FileName);
            }

            return Json(new[] { suretyAttachment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RemoveFiles(string fileName)
        {
            if (fileName != null)
            {

                var physicalPath = Path.Combine(Server.MapPath("~/Content/Files/Archive/"), fileName);

                if (System.IO.File.Exists(physicalPath))
                {
                    // Deleting the File from the Location
                    System.IO.File.Delete(physicalPath);
                }
            }

            return Content("");
        }

        public JsonResult UploadIncomingLetters(int masterId, HttpPostedFileBase file, string description)
        {
            if (file != null && file.ContentLength > 0)
            {
                string path = HttpContext.Server.MapPath("~/Content/Files/Archive/");
                string shortPath = "/Content/Files/Archive/";
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

                var entity = new GuarantorOrSuretyAttachment
                {
                    GuarantorOrSuretyId = masterId,
                    FilePath = filePath,
                    FileName = newFileName,
                    Description = description,
                    ShortPath = shortPath,
                    FileExtension = fileExtension
                };

                db.GuarantorOrSuretyAttachments.Add(entity);
                db.SaveChanges();
            }

            return Json("Successfull");
        }
        public JsonResult UploadIncomingLetters_MultipleFile(int masterId,  HttpPostedFileBase[] letter, string description)
        {
            for (int i = 0; i < letter.Count(); i++)
            {
                HttpPostedFileBase oneFile = letter[i];
                if (oneFile != null && oneFile.ContentLength > 0)
                {
                    string path = HttpContext.Server.MapPath("~/Content/Files/Archive/");
                    string shortPath = "/Content/Files/Archive/";
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

                    var entity = new GuarantorOrSuretyAttachment
                    {
                        GuarantorOrSuretyId = masterId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.GuarantorOrSuretyAttachments.Add(entity);
                    db.SaveChanges();
                }
            }           

            return Json("Successfull");
        }
        //public ActionResult DownloadAttachment(int attachmentId)
        //{
        //    var record = db.GuarantorOrSuretyAttachments.Find(attachmentId);

        //    return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        //}
        public ActionResult DownloadAttachment(int attachmentId)
        {
            var record = db.GuarantorOrSuretyAttachments.Find(attachmentId);
            if (record == null || string.IsNullOrEmpty(record.FilePath))
            {
                return HttpNotFound("Attachment not found.");
            }

            var fullPath = Server.MapPath(record.FilePath); // Ensure path is resolved properly
            if (!System.IO.File.Exists(fullPath))
            {
                return HttpNotFound("File does not exist on the server.");
            }

            var fileName = Path.GetFileName(fullPath);
            var fileBytes = System.IO.File.ReadAllBytes(fullPath);

            return File(fileBytes, "application/octet-stream", fileName);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
