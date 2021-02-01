using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.CostTracking
{
    public class CostTrackingController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Production/CostTracking
        public ActionResult Index()
        {
            var periods = db.ProductionPeriods.ToList();
            ViewData["ProductionPeriods"] = periods;

            var crops = db.Crops.ToList();
            ViewData["Crops"] = crops;

            return View();
        }
    }
}