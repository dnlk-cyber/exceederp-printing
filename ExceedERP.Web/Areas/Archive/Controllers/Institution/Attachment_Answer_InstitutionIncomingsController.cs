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
using ExceedERP.Core.Domain.ArchiveManagement.Institution;

namespace ExceedERP.Web.Areas.Archive.Controllers.DepartmentToDepartment
{
    public class Attachment_Answer_InstitutionIncomingsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Attachment_Answer_InstitutionIncomings_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var attachments = db.Attachment_Answer_InstitutionIncomings.Where(x => x.InstitutionIncomingAnswerId == id).ToList();

            DataSourceResult result = attachments.ToDataSourceResult(request, attachment_DepartmentToStaff => new
            {
                InstitutionIncomingAnswerId = attachment_DepartmentToStaff.InstitutionIncomingAnswerId,
                Attachment_Answer_InstitutionIncomingId = attachment_DepartmentToStaff.Attachment_Answer_InstitutionIncomingId,
                FilePath = attachment_DepartmentToStaff.FilePath,
                ShortPath = attachment_DepartmentToStaff.ShortPath,
                FileName = attachment_DepartmentToStaff.FileName,
                FileExtension = attachment_DepartmentToStaff.FileExtension
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attachment_Answer_InstitutionIncomings_Create([DataSourceRequest]DataSourceRequest request, Attachment_Answer_InstitutionIncoming attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Attachment_Answer_InstitutionIncoming
                {
                    InstitutionIncomingAnswerId = attachment.InstitutionIncomingAnswerId,
                    FilePath = attachment.FilePath,
                    ShortPath = attachment.ShortPath,
                    FileName = attachment.FileName,
                    FileExtension = attachment.FileExtension
                };

                db.Attachment_Answer_InstitutionIncomings.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attachment_Answer_InstitutionIncomings_Destroy([DataSourceRequest]DataSourceRequest request, Attachment_Answer_InstitutionIncoming attachment)
        {
            if (ModelState.IsValid)
            {
                var attach = db.Attachment_Answer_InstitutionIncomings.Find(attachment.Attachment_Answer_InstitutionIncomingId);
                if(attach != null)
                {
                    db.Entry(attach).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DownloadInstitutionIncomingAttachment_Answer(int attachmentId)
        {
            var record = db.Attachment_Answer_InstitutionIncomings.Find(attachmentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
        public ActionResult AttachmentInstitutionIncomingAnswer_MultipleFile(int masterId, HttpPostedFileBase[] AnswerFile)
        {
            var answer = db.InstitutionIncomingAnswers.Find(masterId);
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

                        var entity = new Attachment_Answer_InstitutionIncoming
                        {
                            InstitutionIncomingAnswerId = masterId,
                            FilePath = filePath,
                            FileName = newFileName,
                            ShortPath = shortPath,
                            FileExtension = fileExtension
                        };

                        db.Attachment_Answer_InstitutionIncomings.Add(entity);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index", "DepartmentSentLetter");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
