using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.DataAccess.Context;
using System.IO;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class EmployeeFolderAttachmentsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

      
        public ActionResult EmployeeFolderAttachments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var employeefolderattachments = db.EmployeeFolderAttachments.Where(x => x.PersonalArchiveNoId == id).ToList();

            DataSourceResult result = employeefolderattachments.ToDataSourceResult(request, employeeFolderAttachment => new
            {
                EmployeeFolderAttachmentId = employeeFolderAttachment.EmployeeFolderAttachmentId,
                PersonalArchiveNoId = employeeFolderAttachment.PersonalArchiveNoId,
                FileName = employeeFolderAttachment.FileName,
                FilePath = employeeFolderAttachment.FilePath,
                ShortPath = employeeFolderAttachment.ShortPath,
                FileExtension = employeeFolderAttachment.FileExtension,
                Description = employeeFolderAttachment.Description,
                Remark = employeeFolderAttachment.Remark
            });

            return Json(result);
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeFolderAttachments_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeFolderAttachment employeeFolderAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeFolderAttachment
                {
                    EmployeeFolderAttachmentId = employeeFolderAttachment.EmployeeFolderAttachmentId,
                    PersonalArchiveNoId = employeeFolderAttachment.PersonalArchiveNoId,
                    FileName = employeeFolderAttachment.FileName,
                    FilePath = employeeFolderAttachment.FilePath,
                    Remark = employeeFolderAttachment.Remark
                };

                db.EmployeeFolderAttachments.Attach(entity);
                db.EmployeeFolderAttachments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeFolderAttachment }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult UploadEmployeeFolder(int personalArchiveNoId, string description, HttpPostedFileBase[] employeeFile)
        {
            bool isUploaded = false;

            for (int i = 0; i < employeeFile.Count(); i++)
            {
                var filePath = "";
                var newFileName = "";

                HttpPostedFileBase empFile = employeeFile[i];
                if (empFile.ContentLength > 0)
                {
                    string path = HttpContext.Server.MapPath("~/Content/Files/Archive/EmployeeFolderFiles/");
                    string shortPath = "/Content/Files/Archive/EmployeeFolderFiles/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    newFileName = Path.GetFileNameWithoutExtension(empFile.FileName);
                    var fileExtension = System.IO.Path.GetExtension(empFile.FileName);
                    string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                    newFileName = newFileName + name + fileExtension;
                    filePath = path + newFileName;

                    empFile.SaveAs(filePath);

                    var entity = new EmployeeFolderAttachment();

                    entity.PersonalArchiveNoId = personalArchiveNoId;
                    entity.FileName = newFileName;
                    entity.FilePath = filePath;
                    entity.Description = description;
                    entity.ShortPath = shortPath;
                    entity.FileExtension = fileExtension;

                    db.EmployeeFolderAttachments.Add(entity);
                    db.SaveChanges();

                    isUploaded = true;
                }
            }

            if(isUploaded)
            {
                return Json(new
                {
                    Message = "Successfully Uploaded",
                    Status = "Ok"
                });
            }           

            return Json(new
            {
                Message = "Faild To Upload, Please try again",
                Status = "Failed"
            });
        }
        public ActionResult DownloadEmployeeFolderFiles(int EmployeeFolderAttachmentId)
        {
            var record = db.EmployeeFolderAttachments.Find(EmployeeFolderAttachmentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
