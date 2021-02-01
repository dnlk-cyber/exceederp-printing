using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.Processing
{
    public class SeedProcessingInputsController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Production/SeedProcessing
        public ActionResult Index()
        {
            ViewData["ProcessingPlants"] = db.ProcessingPlants.ToList();

            ViewData["SeedProductionItems"] = db.SeedProductionOrderItems.ToList();

            ViewData["Activities"] = db.ProductionActivities.ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.ToList();

            ViewData["Items"] = db.InventoryItems.ToList();

            ViewData["ProductionPeriods"] = db.ProductionPeriods.ToList();

            ViewData["InventoryStores"] = db.InventoryStores.ToList();
            return View();
        }
    }
}