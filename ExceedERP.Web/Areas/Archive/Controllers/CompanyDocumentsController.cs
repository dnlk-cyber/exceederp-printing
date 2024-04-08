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
    public class CompanyDocumentsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CompanyDocuments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var companydocuments = db.CompanyDocuments.Where(x => x.CompanyDocumentTypeId == id).ToList();

            DataSourceResult result = companydocuments.ToDataSourceResult(request, companyDocument => new
            {
                CompanyDocumentId = companyDocument.CompanyDocumentId,
                CompanyDocumentTypeId = companyDocument.CompanyDocumentTypeId,
                FilePath = companyDocument.FilePath,
                FileName = companyDocument.FileName,
                ShortPath = companyDocument.ShortPath,
                FileExtension = companyDocument.FileExtension,
                Description = companyDocument.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyDocuments_Destroy([DataSourceRequest]DataSourceRequest request, CompanyDocument companyDocument)
        {
            if (ModelState.IsValid)
            {
                var entity = new CompanyDocument
                {
                    CompanyDocumentId = companyDocument.CompanyDocumentId,
                    CompanyDocumentTypeId = companyDocument.CompanyDocumentTypeId,
                    FilePath = companyDocument.FilePath,
                    FileName = companyDocument.FileName,
                    Description = companyDocument.Description
                };

                db.CompanyDocuments.Attach(entity);
                db.CompanyDocuments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { companyDocument }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DownloadCompanyDocument(int CompanyDocumentId)
        {
            var record = db.CompanyDocuments.Find(CompanyDocumentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
        public JsonResult UploadCompanyFile(int CompanyDocumentTypeId, HttpPostedFileBase[] CompanyFile, string Description)
        {
            for (int i = 0; i < CompanyFile.Count(); i++)
            {
                HttpPostedFileBase compFile = CompanyFile[i];
                if (compFile != null && compFile.ContentLength > 0)
                {
                    string path = HttpContext.Server.MapPath("~/Content/Files/Archive/");
                    string shortPath = "/Content/Files/Archive/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string newFileName = Path.GetFileNameWithoutExtension(compFile.FileName);// compFile.FileName;
                    var fileExtension = System.IO.Path.GetExtension(compFile.FileName);
                    string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                    newFileName = newFileName + name +fileExtension;
                    string filePath = path + newFileName;

                    compFile.SaveAs(filePath);

                    var entity = new CompanyDocument
                    {
                        CompanyDocumentTypeId = CompanyDocumentTypeId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = Description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.CompanyDocuments.Add(entity);
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
