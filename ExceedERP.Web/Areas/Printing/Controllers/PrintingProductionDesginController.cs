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

        public ActionResult DownloadCompanyDocument(int CompanyDocumentId)
        {
            var record = db.PrintingJobOrderProductionJobDesigns.Find(CompanyDocumentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
    }
}