using ExceedERP.Core.Domain.Manufacturing.Setting;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class StandardJobDesignController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Manufacturing/StandardJobDesign
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurniturStandardJobDesign> furnitureBoqDoc = db.FurniturStandardJobDesigns.Where(x => x.ReferenceId == id);
            DataSourceResult result = furnitureBoqDoc.ToDataSourceResult(request, model => new FurniturStandardJobDesign
            {
                FurniturStandardJobDesignId = model.FurniturStandardJobDesignId,
                Description = model.Description,
                FileExtension = model.FileExtension,
                FileName = model.FileName,
                FilePath = model.FilePath,
                ReferenceId = model.ReferenceId,
                ShortPath = model.ShortPath
            });

            return Json(result);
        }

        public JsonResult UploadCompanyFile(int furnitureStandardJobTypeId, HttpPostedFileBase[] CompanyFile, string Description)
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

                    var entity = new FurniturStandardJobDesign
                    {
                        ReferenceId = furnitureStandardJobTypeId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = Description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.FurniturStandardJobDesigns.Add(entity);
                    db.SaveChanges();
                }
            }

            return Json("Successfull");
        }
        public ActionResult DownloadCompanyDocument(int CompanyDocumentId)
        {
            var record = db.FurniturStandardJobDesigns.Find(CompanyDocumentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, FurniturStandardJobDesign model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.FurniturStandardJobDesigns.FirstOrDefault(x => x.FurniturStandardJobDesignId == model.FurniturStandardJobDesignId);
                db.FurniturStandardJobDesigns.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}