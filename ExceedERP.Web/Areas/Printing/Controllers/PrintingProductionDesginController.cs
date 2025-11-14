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
    public class PrintingProductionDesginController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Printing/PrintingProductionDesgin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadProduction([DataSourceRequest]DataSourceRequest request, int id)
        {
            var entity = db.PrintingJobOrderProductionJobDesigns.Where(x => x.ReferenceId == id);
           
            return Json(entity.ToDataSourceResult(request));
        }

        public ActionResult DownloadCompanyDocument(int CompanyDocumentId)
        {
            var record = db.PrintingJobOrderProductionJobDesigns.Find(CompanyDocumentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }


        public JsonResult UploadCompanyFile(int id, HttpPostedFileBase[] CompanyFile, string Description)
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

                    var entity = new PrintingJobOrderProductionJobDesign
                    {
                        ReferenceId = id,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = Description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.PrintingJobOrderProductionJobDesigns.Add(entity);
                    db.SaveChanges();
                }
            }

            return Json("Successfull");
        }

        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, PrintingJobOrderProductionJobDesign model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.PrintingJobOrderProductionJobDesigns.FirstOrDefault(x => x.PrintingJobOrderProductionJobDesignId == model.PrintingJobOrderProductionJobDesignId);
                db.PrintingJobOrderProductionJobDesigns.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}