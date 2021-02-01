using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.ProductionStores
{
    public class FinishedGoodsStoreReceiveController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Production/FinishedGoodsStoreReceive
        public ActionResult Index()
        {
            ViewData["InventoryStores"] = db.InventoryStores.Select(s => new { s.Name, s.StoreCode }).ToList();
            ViewData["ProductionPeriods"] = db.ProductionPeriods.Select(p => new { p.Name, p.ProductionPeriodId }).ToList();
            ViewData["SeedProcessingOrderItems"] = db.SeedProcessingOrderItems.Select(o => new { o.ProcessingCode, o.SeedProcessingOrderItemId}).ToList();
            ViewData["GumProcessingOrderItems"] = db.GumProcessingOrderItems.Select(o => new { o.GumProcessingOrderItemId, o.ProcessingCode}).ToList();
            ViewData["ItemCategories"] = db.ItemCategorys.Select(c => new { c.Name, c.ItemCategoryCode }).ToList();
            ViewData["InventoryItems"] = db.InventoryItems.Select(i => new { i.Name, i.ItemCode }).ToList();
            var taxRates = db.GLTaxRates;
            var taxWithConcatnatedValue = new List<GLTaxRate>();
            foreach (GLTaxRate taxRate in taxRates)
            {
                var taxName = new GLTaxRate
                {
                    GLTaxRateID = taxRate.GLTaxRateID,
                    TaxName = taxRate.TaxName + "( " + Math.Round(taxRate.Rate, 0) + "% )"
                };
                taxWithConcatnatedValue.Add(taxName);
            }
            ViewData["TaxTypes"] = taxWithConcatnatedValue;
            ViewData["Branchs"] = db.Branch.Select(b => new { b.Name, b.BranchId}).ToList();
            return View();
        }
    }
}