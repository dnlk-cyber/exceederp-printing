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
using ExceedERP.Core.Domain.ArchiveManagement.DepartmentToStaff;
using ExceedERP.DataAccess.Context;
using System.IO;

namespace ExceedERP.Web.Areas.Archive.Controllers.DepartmentToStaff
{
    public class Attachment_DepartmentToStaffsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Attachment_DepartmentToStaffs_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var attachment_departmenttostaffs = db.Attachment_DepartmentToStaffs.Where(x => x.OutGoing_DepartmentToStaffId == id).ToList();

            DataSourceResult result = attachment_departmenttostaffs.ToDataSourceResult(request, attachment_DepartmentToStaff => new
            {
                Attachment_DepartmentToStaffId = attachment_DepartmentToStaff.Attachment_DepartmentToStaffId,
                OutGoing_DepartmentToStaffId = attachment_DepartmentToStaff.OutGoing_DepartmentToStaffId,
                FilePath = attachment_DepartmentToStaff.FilePath,
                ShortPath = attachment_DepartmentToStaff.ShortPath,
                FileName = attachment_DepartmentToStaff.FileName,
                FileExtension = attachment_DepartmentToStaff.FileExtension
            });

            return Json(result);
        }
        public ActionResult DownloadDepartmentToDepartmentAttachment(int attachmentId)
        {
            try
            {
                var record = db.Attachment_DepartmentToStaffs.Find(attachmentId);

                if (record != null)
                {
                    string filePath = record.FilePath;

                    if (System.IO.File.Exists(filePath))
                    {
                        return File(filePath, "application/force-download", Path.GetFileName(filePath));
                    }
                    else
                    {
                        // Handle the case where the file does not exist
                        return HttpNotFound("File not found");
                    }
                }
                else
                {
                    // Handle the case where the record with the given ID is not found
                    return HttpNotFound("Record not found");
                }
            }
            catch (Exception ex)
            {
                // Handle other exceptions that might occur
                return Content($"An error occurred: {ex.Message}");
            }
        }

        //public ActionResult DownloadDepartmentToDepartmentAttachment(int attachmentId)
        //{
        //    var record = db.Attachment_DepartmentToStaffs.Find(attachmentId);

        //    return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        //}
        public JsonResult UploadDepartment_MultipleFile(int masterId, HttpPostedFileBase[] letter)
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

                    var entity = new Attachment_DepartmentToStaff
                    {
                        OutGoing_DepartmentToStaffId = masterId,
                        FilePath = filePath,
                        FileName = newFileName,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.Attachment_DepartmentToStaffs.Add(entity);
                    db.SaveChanges();
                }
            }

            return Json("Successfull");
        }
        public ActionResult Attachment_DepartmentToStaffs_Destroy([DataSourceRequest]DataSourceRequest request, Attachment_DepartmentToStaff attachment)
        {
            var file = db.Attachment_DepartmentToStaffs.Find(attachment.Attachment_DepartmentToStaffId);
            if (file != null)
            {
                db.Entry(file).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }

            return Json(attachment);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
