using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingJobOrderDesginController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Printing/PrintingJobOrderDesgin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PrintingJobOrderJobDesign> furnitureBoqDoc = db.PrintingJobOrderJobDesigns.Where(x => x.ReferenceId == id);
            DataSourceResult result = furnitureBoqDoc.ToDataSourceResult(request, model => new PrintingJobOrderJobDesign
            {
                PrintingJobOrderJobDesignId = model.PrintingJobOrderJobDesignId,
                Description = model.Description,
                FileExtension = model.FileExtension,
                FileName = model.FileName,
                FilePath = model.FilePath,
                ReferenceId = model.ReferenceId,
                ShortPath = model.ShortPath
            });

            return Json(result);
        }
        public ActionResult ReadProduction([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PrintingJobOrderProductionJobDesign> furnitureBoqDoc = db.PrintingJobOrderProductionJobDesigns.Where(x => x.ReferenceId == id);
            DataSourceResult result = furnitureBoqDoc.ToDataSourceResult(request, model => new PrintingJobOrderProductionJobDesign
            {
                PrintingJobOrderProductionJobDesignId = model.PrintingJobOrderProductionJobDesignId,
                Description = model.Description,
                FileExtension = model.FileExtension,
                FileName = model.FileName,
                FilePath = model.FilePath,
                ReferenceId = model.ReferenceId,
                ShortPath = model.ShortPath
            });

            return Json(result);
        }

        public JsonResult UploadCompanyFile(int furnitureBoqRequestId, HttpPostedFileBase[] CompanyFile, string Description)
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
                    newFileName = newFileName + name + fileExtension;
                    string filePath = path + newFileName;

                    compFile.SaveAs(filePath);

                    var entity = new PrintingJobOrderJobDesign
                    {
                        ReferenceId = furnitureBoqRequestId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = Description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.PrintingJobOrderJobDesigns.Add(entity);
                    db.SaveChanges();
                }
            }

            return Json("Successfull");
        }

        public ActionResult DownloadCompanyDocument(int CompanyDocumentId)
        {
            var record = db.PrintingJobOrderJobDesigns.Find(CompanyDocumentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, PrintingJobOrderJobDesign model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.PrintingJobOrderJobDesigns.FirstOrDefault(x => x.PrintingJobOrderJobDesignId == model.PrintingJobOrderJobDesignId);
                db.PrintingJobOrderJobDesigns.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}