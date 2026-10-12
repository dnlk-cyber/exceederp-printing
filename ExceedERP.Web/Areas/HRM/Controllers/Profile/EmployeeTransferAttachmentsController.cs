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
using ExceedERP.Core.Domain.Health;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    public class EmployeeTransferAttachmentsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        
        public ActionResult EmployeeTransferAttachments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var transferAttachments = db.EmployeeTransferAttachments.Where(x => x.EmployeeTransferRequestId == id ).ToList();
            DataSourceResult result = transferAttachments.ToDataSourceResult(request, transferAttachment => new
            {
                EmployeeTransferAttachmentId = transferAttachment.EmployeeTransferAttachmentId,
                EmployeeTransferRequestId = transferAttachment.EmployeeTransferRequestId,
                FilePath = transferAttachment.FilePath,
                FileName = transferAttachment.FileName,
                ShortPath = transferAttachment.ShortPath,
                FileExtension = transferAttachment.FileExtension,
                Description = transferAttachment.Description
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferAttachments_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file, int id)
        {
            if (file != null)
            {
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                int MaxFileSize = 2 * 1024 * 1024;

                //string subPath = "~/App_Data/HealthFiles/";
                //string shortPath = "/App_Data/HealthFiles/";
                string subPath = "~/Content/HRM/EmployeeTransferFile/";
                string shortPath = "/Content/HRM/EmployeeTransferFile/";
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
                        #endregion
                        if (ModelState.IsValid)
                        {
                            fileName = Path.GetFileName(f.FileName);
                            string physicalPath = Server.MapPath(subPath + "/" + fileName);
                            //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                            f.SaveAs(physicalPath);
                            db.EmployeeTransferAttachments.Add(new EmployeeTransferAttachment()
                            {
                                FileName = Path.GetFileName(f.FileName),
                                EmployeeTransferRequestId = id,
                                ShortPath = shortPath,
                                FileExtension = fileExtension,
                                Description = "Employee Transfer  Attachment",
                                FilePath = physicalPath

                            });
                            db.SaveChanges();
                        }
                    }
                }


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Employee Transfer template", "Success", User.Identity.Name, $"uploaded Employee Transfer Attachment template {fileName} ", Modules.Health);
                #endregion

            }
            return Json(new[] { file }.ToDataSourceResult(request, ModelState));

        }

        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferAttachments_Update([DataSourceRequest]DataSourceRequest request, EmployeeTransferAttachment transferAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeTransferAttachment
                {
                    EmployeeTransferAttachmentId = transferAttachment.EmployeeTransferAttachmentId,
                    EmployeeTransferRequestId = transferAttachment.EmployeeTransferRequestId,
                    FilePath = transferAttachment.FilePath,
                    FileName = transferAttachment.FileName,
                    ShortPath = transferAttachment.ShortPath,
                    FileExtension = transferAttachment.FileExtension,
                    Description = transferAttachment.Description
                };

                db.EmployeeTransferAttachments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { transferAttachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferAttachments_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeTransferAttachment transferAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeTransferAttachment
                {
                    EmployeeTransferAttachmentId = transferAttachment.EmployeeTransferAttachmentId,
                    EmployeeTransferRequestId = transferAttachment.EmployeeTransferRequestId,
                    FilePath = transferAttachment.FilePath,
                    FileName = transferAttachment.FileName,
                    ShortPath = transferAttachment.ShortPath,
                    FileExtension = transferAttachment.FileExtension,
                    Description = transferAttachment.Description
                };

                db.EmployeeTransferAttachments.Attach(entity);
                db.EmployeeTransferAttachments.Remove(entity);
                db.SaveChanges();
                RemoveFiles(transferAttachment.FilePath);
            }

            return Json(new[] { transferAttachment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RemoveFiles(string fileName)
        {
            if (fileName != null)
            {
                var physicalPath = Path.Combine(Server.MapPath("/Content/Files/"), fileName);

                if (System.IO.File.Exists(physicalPath))
                {
                    // Deleting the File from the Location
                    System.IO.File.Delete(physicalPath);
                }
            }

            return Content("");
        }
        public JsonResult UploadIncomingHealthFiles(int masterId, HttpPostedFileBase file, string description)
        {
            try
            {
                // Ensure file is not null
                if (file == null)
                {
                    return Json(new { success = false, message = "No file selected for upload." });
                }

                // Define allowed extensions and max size
                string[] allowedExtensions = { ".jpg", ".png", ".pdf" };
                int maxFileSize = 2 * 1024 * 1024; // 2 MB
                string subPath = "~/Content/Files/Health/";
                string fileName = Path.GetFileName(file.FileName);
                string fileExtension = Path.GetExtension(fileName).ToLower();

                // Validate file extension
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { success = false, message = "Invalid file type. Allowed types: .jpg, .png, .pdf" });
                }

                // Validate file size
                if (file.ContentLength > maxFileSize)
                {
                    return Json(new { success = false, message = "File size exceeds the 20 MB limit." });
                }

                // Check if the directory exists, if not, create it
                string directoryPath = Server.MapPath(subPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Save the file
                string physicalPath = Path.Combine(directoryPath, fileName);
                file.SaveAs(physicalPath);

                // Save the file metadata to the database
                var healthAttachment = new HealthAttachment()
                {
                    FileName = fileName,
                    HealthRankAndValueId = masterId,
                    ShortPath = physicalPath,
                    FileExtension = fileExtension,
                    Description = description ?? "Health Attachment", // Use provided description or default
                    FilePath = physicalPath
                };

                db.HealthAttachments.Add(healthAttachment);
                db.SaveChanges();

                // Log the operation
                UserHelper.OperationLog(Request.UserHostAddress, "Health Attachment upload", "Success", User.Identity.Name,
                    $"Uploaded health attachment {fileName}", Modules.Health);

                // Return success response
                return Json(new { success = true, message = "File uploaded successfully!" });
            }
            catch (Exception ex)
            {
                // Log the exception and return a failure message
                UserHelper.OperationLog(Request.UserHostAddress, "Health Attachment upload", "Error", User.Identity.Name,
                    $"Error uploading health attachment: {ex.Message}", Modules.Health);

                return Json(new { success = false, message = "File upload failed: " + ex.Message });
            }
        }


        public JsonResult Upload_MultipleHealthFile(int masterId,  HttpPostedFileBase[] letter, string description)
        {
            for (int i = 0; i < letter.Count(); i++)
            {
                HttpPostedFileBase oneFile = letter[i];
                if (oneFile != null && oneFile.ContentLength > 0)
                {
                    string path = HttpContext.Server.MapPath("~/Content/Files/");
                    string shortPath = "/Content/Files/";
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

            return Json("Successfull");
        }
        public ActionResult DownloadAttachment(int attachmentId)
        {


            var record = db.EmployeeTransferAttachments.Find(attachmentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
                   
           
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
