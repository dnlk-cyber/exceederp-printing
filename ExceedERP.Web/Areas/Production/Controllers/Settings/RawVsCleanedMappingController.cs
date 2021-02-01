using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.Settings
{
    public class RawVsCleanedMappingController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Production/RawAndCleanedProductMapping
        public ActionResult Index()
        {
            ViewData["ItemCategorys"] = db.ItemCategorys.Select(c => new { c.ItemCategoryCode, c.Name }).ToList();
            ViewData["Items"] = db.InventoryItems.Select(i => new { i.Name, i.ItemCode }).ToList();
            return View();
        }
    }
}