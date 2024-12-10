using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingMaterialCostProductionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Printing/PrintingMaterialCostProduction
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintingMaterialCost_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var entity = db.PrintingProductionMaterialCosts.Where(x => x.PrintingJobOrderProductionId == id);
            return Json(entity.ToDataSourceResult(request));
        }

        public ActionResult PrintingLaborCost_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var entity = db.PrintingProductionLaborCosts.Where(x => x.PrintingJobOrderProductionId == id);
            return Json(entity.ToDataSourceResult(request));
        }
    }
}