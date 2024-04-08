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

namespace ExceedERP.Web.Areas.Archive.Controllers.DepartmentToDepartment
{
    public class Attachment_Answer_DepartmentToDepartmentsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Attachment_Answer_DepartmentToDepartments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var attachments = db.Attachment_Answer_DepartmentToDepartments.Where(x => x.OutGoing_DepartmentToDepartment_AnswerId == id).ToList();

            DataSourceResult result = attachments.ToDataSourceResult(request, attachment_DepartmentToDepartment => new
            {
                OutGoing_DepartmentToDepartment_AnswerId = attachment_DepartmentToDepartment.OutGoing_DepartmentToDepartment_AnswerId,
                Attachment_Answer_DepartmentToDepartmentId = attachment_DepartmentToDepartment.Attachment_Answer_DepartmentToDepartmentId,
                FilePath = attachment_DepartmentToDepartment.FilePath,
                ShortPath = attachment_DepartmentToDepartment.ShortPath,
                FileName = attachment_DepartmentToDepartment.FileName,
                FileExtension = attachment_DepartmentToDepartment.FileExtension
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attachment_Answer_DepartmentToDepartments_Create([DataSourceRequest]DataSourceRequest request, Attachment_DepartmentToDepartment attachment_DepartmentToDepartment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Attachment_DepartmentToDepartment
                {
                    OutGoing_DepartmentToDepartmentId = attachment_DepartmentToDepartment.OutGoing_DepartmentToDepartmentId,
                    FilePath = attachment_DepartmentToDepartment.FilePath,
                    ShortPath = attachment_DepartmentToDepartment.ShortPath,
                    FileName = attachment_DepartmentToDepartment.FileName,
                    FileExtension = attachment_DepartmentToDepartment.FileExtension
                };

                db.Attachment_DepartmentToDepartments.Add(entity);
                db.SaveChanges();
                attachment_DepartmentToDepartment.Attachment_DepartmentToDepartmentId = entity.Attachment_DepartmentToDepartmentId;
            }

            return Json(new[] { attachment_DepartmentToDepartment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attachment_Answer_DepartmentToDepartments_Destroy([DataSourceRequest]DataSourceRequest request, Attachment_DepartmentToDepartment attachment_DepartmentToDepartment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Attachment_DepartmentToDepartment
                {
                    Attachment_DepartmentToDepartmentId = attachment_DepartmentToDepartment.Attachment_DepartmentToDepartmentId,
                    OutGoing_DepartmentToDepartmentId = attachment_DepartmentToDepartment.OutGoing_DepartmentToDepartmentId,
                    FilePath = attachment_DepartmentToDepartment.FilePath,
                    ShortPath = attachment_DepartmentToDepartment.ShortPath,
                    FileName = attachment_DepartmentToDepartment.FileName,
                    FileExtension = attachment_DepartmentToDepartment.FileExtension
                };

                db.Attachment_DepartmentToDepartments.Attach(entity);
                db.Attachment_DepartmentToDepartments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attachment_DepartmentToDepartment }.ToDataSourceResult(request, ModelState));
        }
        //public ActionResult DownloadDepartmentToDepartmentAttachment_Answer(int attachmentId)
        //{
        //    try
        //    {
        //        var record = db.Attachment_Answer_DepartmentToDepartments.Find(attachmentId);

        //        if (record != null)
        //        {
        //            string filePath = record.FilePath;

        //            if (System.IO.File.Exists(filePath))
        //            {
        //                return File(filePath, "application/force-download", Path.GetFileName(filePath));
        //            }
        //            else
        //            {
        //                // Handle the case where the file does not exist
        //                return HttpNotFound("File not found");
        //            }
        //        }
        //        else
        //        {
        //            // Handle the case where the record with the given ID is not found
        //            return HttpNotFound("Record not found");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle other exceptions that might occur
        //        return Content($"An error occurred: {ex.Message}");
        //    }
        //}

        public ActionResult DownloadDepartmentToDepartmentAttachment_Answer(int attachmentId)
        {
            var record = db.Attachment_Answer_DepartmentToStaffs.Find(attachmentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
        //public ActionResult DownloadDepartmentToDepartmentAttachment_Answer2(int aId)
        //{
        //    var record = db.Attachment_Answer_DepartmentToDepartments.Find(aId);

        //    return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        //}
        public ActionResult AttachmentDepToDepAnswer_MultipleFile(int masterId, HttpPostedFileBase[] AnswerFile)
        {
            var answer = db.OutGoing_DepartmentToDepartment_Answers.Find(masterId);
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

                        var entity = new Attachment_Answer_DepartmentToDepartment
                        {
                            OutGoing_DepartmentToDepartment_AnswerId = masterId,
                            FilePath = filePath,
                            FileName = newFileName,
                            ShortPath = shortPath,
                            FileExtension = fileExtension
                        };

                        db.Attachment_Answer_DepartmentToDepartments.Add(entity);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index_Receiver", "OutGoing_DepartmentToDepartments");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
