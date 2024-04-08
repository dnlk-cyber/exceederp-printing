using System;
using System.Collections.Generic;
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
    public class ArchiveAttachmentsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ArchiveAttachments_Read([DataSourceRequest]DataSourceRequest request, int id, ArchiveCategory ac, LetterType lt)
        {
            var archiveattachements = db.ArchiveAttachements.Where(x => x.InstitutionIncomingLetterId == id && x.ArchiveCategory == ac && x.LetterType == lt).ToList();
            DataSourceResult result = archiveattachements.ToDataSourceResult(request, archiveAttachement => new
            {
                ArchiveAttachementId = archiveAttachement.ArchiveAttachementId,
                InstitutionIncomingLetterId = archiveAttachement.InstitutionIncomingLetterId,
                ArchiveCategory = archiveAttachement.ArchiveCategory,
                LetterType = archiveAttachement.LetterType,
                FilePath = archiveAttachement.FilePath,
                FileName = archiveAttachement.FileName,
                ShortPath = archiveAttachement.ShortPath,
                FileExtension = archiveAttachement.FileExtension,
                Description = archiveAttachement.Description
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArchiveAttachments_Create([DataSourceRequest]DataSourceRequest request, ArchiveAttachement archiveAttachement, int id, ArchiveCategory ac, LetterType lt)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArchiveAttachement
                {
                    InstitutionIncomingLetterId = id,
                    ArchiveCategory = ac,
                    LetterType = lt,
                    FilePath = archiveAttachement.FilePath,
                    FileName = archiveAttachement.FileName,
                    Description = archiveAttachement.Description
                };

                db.ArchiveAttachements.Add(entity);
                db.SaveChanges();
                archiveAttachement.ArchiveAttachementId = entity.ArchiveAttachementId;
            }

            return Json(new[] { archiveAttachement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArchiveAttachments_Update([DataSourceRequest]DataSourceRequest request, ArchiveAttachement archiveAttachement)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArchiveAttachement
                {
                    ArchiveAttachementId = archiveAttachement.ArchiveAttachementId,
                    InstitutionIncomingLetterId = archiveAttachement.InstitutionIncomingLetterId,
                    ArchiveCategory = archiveAttachement.ArchiveCategory,
                    LetterType = archiveAttachement.LetterType,
                    FilePath = archiveAttachement.FilePath,
                    FileName = archiveAttachement.FileName,
                    Description = archiveAttachement.Description
                };

                db.ArchiveAttachements.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { archiveAttachement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArchiveAttachments_Destroy([DataSourceRequest]DataSourceRequest request, ArchiveAttachement archiveAttachement)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArchiveAttachement
                {
                    ArchiveAttachementId = archiveAttachement.ArchiveAttachementId,
                    InstitutionIncomingLetterId = archiveAttachement.InstitutionIncomingLetterId,
                    ArchiveCategory = archiveAttachement.ArchiveCategory,
                    LetterType = archiveAttachement.LetterType,
                    FilePath = archiveAttachement.FilePath,
                    FileName = archiveAttachement.FileName,
                    Description = archiveAttachement.Description
                };

                db.ArchiveAttachements.Attach(entity);
                db.ArchiveAttachements.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { archiveAttachement }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult UploadIncomingLetters(int masterId, ArchiveCategory ac, LetterType lt, HttpPostedFileBase file, string description)
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

                var entity = new ArchiveAttachement
                {
                    InstitutionIncomingLetterId = masterId,
                    FilePath = filePath,
                    FileName = newFileName,
                    Description = description,
                    ArchiveCategory = ac,
                    LetterType = lt,
                    ShortPath = shortPath,
                    FileExtension = fileExtension
                };

                db.ArchiveAttachements.Add(entity);
                db.SaveChanges();
            }

            return Json("Successfull");
        }
        public JsonResult UploadIncomingLetters_MultipleFile(int masterId, ArchiveCategory ac, LetterType lt, HttpPostedFileBase[] letter, string description)
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

                    var entity = new ArchiveAttachement
                    {
                        InstitutionIncomingLetterId = masterId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = description,
                        ArchiveCategory = ac,
                        LetterType = lt,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.ArchiveAttachements.Add(entity);
                    db.SaveChanges();
                }
            }           

            return Json("Successfull");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
