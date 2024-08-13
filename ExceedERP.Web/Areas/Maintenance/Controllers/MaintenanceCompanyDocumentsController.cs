using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using System.IO;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class MaintenanceCompanyDocumentsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MaintenanceCompanyDocuments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var companydocuments = db.MaintenanceCompanyDocuments.Where(x => x.MaintenanceCompanyDocumentTypeId == id).ToList();

            DataSourceResult result = companydocuments.ToDataSourceResult(request, companyDocument => new
            {
                MaintenanceCompanyDocumentId = companyDocument.MaintenanceCompanyDocumentId,
                MaintenanceCompanyDocumentTypeId = companyDocument.MaintenanceCompanyDocumentTypeId,
                FilePath = companyDocument.FilePath,
                FileName = companyDocument.FileName,
                ShortPath = companyDocument.ShortPath,
                FileExtension = companyDocument.FileExtension,
                Description = companyDocument.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaintenanceCompanyDocuments_Destroy([DataSourceRequest]DataSourceRequest request, MaintenanceCompanyDocument companyDocument)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaintenanceCompanyDocument
                {
                    MaintenanceCompanyDocumentId = companyDocument.MaintenanceCompanyDocumentId,
                    MaintenanceCompanyDocumentTypeId = companyDocument.MaintenanceCompanyDocumentTypeId,
                    FilePath = companyDocument.FilePath,
                    FileName = companyDocument.FileName,
                    Description = companyDocument.Description
                };

                db.MaintenanceCompanyDocuments.Attach(entity);
                db.MaintenanceCompanyDocuments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { companyDocument }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DownloadCompanyDocument(int CompanyDocumentId)
        {
            var record = db.MaintenanceCompanyDocuments.Find(CompanyDocumentId);

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

                    var entity = new MaintenanceCompanyDocument
                    {
                        MaintenanceCompanyDocumentTypeId = CompanyDocumentTypeId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = Description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.MaintenanceCompanyDocuments.Add(entity);
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
