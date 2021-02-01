using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.SellingPrice
{
    public class SeedSellingPriceController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Production/SeedSellingPrice
        public ActionResult Index()
        {
            ViewData["Crops"] = db.Crops.Select(c => new { c.Name, c.CropId}).ToList();
            ViewData["ProductionYears"] = db.ProductionYears.Select(y => new { y.Name, y.ProductionYearId}).ToList();

            ViewData["SeedProcessingOrderItems"] = db.SeedProcessingOrderItems.Select(o => new { o.ProcessingCode, o.SeedProcessingOrderItemId}).ToList();
            return View();
        }
    }
}