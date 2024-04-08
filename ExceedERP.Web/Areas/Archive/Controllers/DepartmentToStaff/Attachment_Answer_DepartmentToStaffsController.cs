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
using ExceedERP.Core.Domain.ArchiveManagement.DepartmentToDepartment;
using ExceedERP.Core.Domain.ArchiveManagement.DepartmentToStaff;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Archive.Controllers.DepartmentToDepartment
{
    public class Attachment_Answer_DepartmentToStaffsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Attachment_Answer_DepartmentToStaffs_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var attachments = db.Attachment_Answer_DepartmentToStaffs.Where(x => x.Answer_DepartmentToStaffId == id).ToList();

            DataSourceResult result = attachments.ToDataSourceResult(request, attachment_DepartmentToStaff => new
            {
                Answer_DepartmentToStaffId = attachment_DepartmentToStaff.Answer_DepartmentToStaffId,
                Attachment_Answer_DepartmentToStaffId = attachment_DepartmentToStaff.Attachment_Answer_DepartmentToStaffId,
                FilePath = attachment_DepartmentToStaff.FilePath,
                ShortPath = attachment_DepartmentToStaff.ShortPath,
                FileName = attachment_DepartmentToStaff.FileName,
                FileExtension = attachment_DepartmentToStaff.FileExtension
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attachment_Answer_DepartmentToStaffs_Create([DataSourceRequest]DataSourceRequest request, Attachment_Answer_DepartmentToStaff attachment_Answer_DepartmentToStaff)
        {
            if (ModelState.IsValid)
            {
                var entity = new Attachment_Answer_DepartmentToStaff
                {
                    Answer_DepartmentToStaffId = attachment_Answer_DepartmentToStaff.Answer_DepartmentToStaffId,
                    FilePath = attachment_Answer_DepartmentToStaff.FilePath,
                    ShortPath = attachment_Answer_DepartmentToStaff.ShortPath,
                    FileName = attachment_Answer_DepartmentToStaff.FileName,
                    FileExtension = attachment_Answer_DepartmentToStaff.FileExtension
                };

                db.Attachment_Answer_DepartmentToStaffs.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { attachment_Answer_DepartmentToStaff }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attachment_Answer_DepartmentToStaffs_Destroy([DataSourceRequest]DataSourceRequest request, Attachment_Answer_DepartmentToStaff attachment_Answer_DepartmentToStaff)
        {
            if (ModelState.IsValid)
            {
                var attach = db.Attachment_Answer_DepartmentToStaffs.Find(attachment_Answer_DepartmentToStaff.Attachment_Answer_DepartmentToStaffId);
                if(attach != null)
                {
                    db.Entry(attach).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
            }

            return Json(new[] { attachment_Answer_DepartmentToStaff }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DownloadDepartmentToStaffAttachment_Answer(int attachmentId)
        {
            var record = db.Attachment_Answer_DepartmentToStaffs.Find(attachmentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
        public ActionResult AttachmentDepToStaffAnswer_MultipleFile(int masterId, HttpPostedFileBase[] AnswerFile)
        {
            var answer = db.Answer_DepartmentToStaffs.Find(masterId);
            if (answer != null && !answer.IsAnswered)
            {
                for (int i = 0; i < AnswerFile.Count(); i++)
                {
                    HttpPostedFileBase oneFile = AnswerFile[i];
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

                        var entity = new Attachment_Answer_DepartmentToStaff
                        {
                            Answer_DepartmentToStaffId = masterId,
                            FilePath = filePath,
                            FileName = newFileName,
                            ShortPath = shortPath,
                            FileExtension = fileExtension
                        };

                        db.Attachment_Answer_DepartmentToStaffs.Add(entity);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index_Receiver", "OutGoing_DepartmentToStaff");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
