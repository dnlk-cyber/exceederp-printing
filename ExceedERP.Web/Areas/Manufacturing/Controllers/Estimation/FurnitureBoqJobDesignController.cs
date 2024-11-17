using ExceedERP.Core.Domain.Manufacturing;
using ExceedERP.Core.Domain.Manufacturing.Production;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    public class FurnitureBoqJobDesignController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Manufacturing/FurnitureBoqJobDesign
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureBoqJobDesign> furnitureBoqDoc = db.FurnitureBoqJobDesigns.Where(x => x.ReferenceId == id);
            DataSourceResult result = furnitureBoqDoc.ToDataSourceResult(request, model => new FurnitureBoqJobDesign
            {
                FurnitureBoqJobDesignId = model.FurnitureBoqJobDesignId,
                Description = model.Description,
                FileExtension = model.FileExtension,
                FileName = model.FileName,
                FilePath = model.FilePath,
                ReferenceId = model.ReferenceId,
                ShortPath = model.ShortPath
            });

            return Json(result);
        }
        public ActionResult ReadJob([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureJobDesign> furnitureBoqDoc = db.FurnitureJobDesigns.Where(x => x.FurnitureJobOrderProductionId == id);
            DataSourceResult result = furnitureBoqDoc.ToDataSourceResult(request, model => new FurnitureJobDesign
            {
                FurnitureJobDesignId = model.FurnitureJobDesignId,
                Description = model.Description,
                FileExtension = model.FileExtension,
                FileName = model.FileName,
                FilePath = model.FilePath,
                ShortPath = model.ShortPath,
                FurnitureJobOrderProductionId = model.FurnitureJobOrderProductionId
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

                    var entity = new FurnitureBoqJobDesign
                    {
                        ReferenceId = furnitureBoqRequestId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = Description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.FurnitureBoqJobDesigns.Add(entity);
                    db.SaveChanges();
                }
            }

            return Json("Successfull");
        }
        public ActionResult DownloadCompanyDocument(int CompanyDocumentId)
        {
            var record = db.FurnitureJobDesigns.Find(CompanyDocumentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, FurnitureBoqJobDesign model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.FurnitureBoqJobDesigns.FirstOrDefault(x => x.FurnitureBoqJobDesignId == model.FurnitureBoqJobDesignId);
                db.FurnitureBoqJobDesigns.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}