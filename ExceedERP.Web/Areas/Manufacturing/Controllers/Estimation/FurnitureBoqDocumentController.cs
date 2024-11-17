using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    public class FurnitureBoqDocumentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Manufacturing/FurnitureBoqDocument
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FurnitureBoqRequest_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureBoqDocument> furnitureBoqDoc = db.FurnitureBoqDocuments.Where(x => x.FurnitureBoqRequestId == id);
            DataSourceResult result = furnitureBoqDoc.ToDataSourceResult(request, model => new FurnitureBoqDocument
            {               
                FurnitureBoqRequestId = model.FurnitureBoqRequestId,
                Description = model.Description,
                FileExtension = model.FileExtension,
                FileName = model.FileName,
                FilePath = model.FilePath,
                FurnitureBoqDocumentId = model.FurnitureBoqDocumentId,
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

                    var entity = new FurnitureBoqDocument
                    {
                        FurnitureBoqRequestId = furnitureBoqRequestId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = Description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.FurnitureBoqDocuments.Add(entity);
                    db.SaveChanges();
                }
            }

            return Json("Successfull");
        }

        public ActionResult DownloadCompanyDocument(int CompanyDocumentId)
        {
            var record = db.FurnitureBoqDocuments.Find(CompanyDocumentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
        public ActionResult FurnitureBoqRequest_Delete([DataSourceRequest]DataSourceRequest request, FurnitureBoqDocument model)
        {
            if (ModelState.IsValid)
            {
                var boqRequest = db.FurnitureBoqDocuments.FirstOrDefault(x => x.FurnitureBoqRequestId == model.FurnitureBoqRequestId);
                db.FurnitureBoqDocuments.Remove(boqRequest);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}