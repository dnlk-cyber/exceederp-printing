using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.ProductionOrders
{
    public class ProductionOupputsController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["FarmingPlots"] = db.Plots.Select(p => new { p.PlotId, p.PlotCode }).ToList();

            var crops = db.Crops.ToList();
            ViewData["CropTypes"] = db.Crops.Select(c => new { c.CropId, c.Name });

            ViewData["NaturalGumNames"] = db.NaturalGums.Select(g => new { g.Name, g.NaturalGumId }).ToList();

            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name }).ToList();

            ViewData["ProductionSites"] = db.ProductionSites.Select(s => new { s.Name, s.ProductionSiteId }).ToList();

            ViewData["Activities"] = db.ProductionActivities.Select(a => new { a.ProductionActivityId, a.Name }).ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(c => new { c.ItemCategoryCode, c.Name }).ToList();

            ViewData["Items"] = db.InventoryItems.Select(i => new { i.Name, i.ItemCode }).ToList();

            ViewData["ProductionPeriods"] = db.ProductionPeriods.Select(p => new { p.ProductionPeriodId, p.Name }).ToList();

            ViewData["ProductionYears"] = db.ProductionYears.Select(y => new { y.ProductionYearId, y.Name }).ToList();

            return View();
        }
    }
}