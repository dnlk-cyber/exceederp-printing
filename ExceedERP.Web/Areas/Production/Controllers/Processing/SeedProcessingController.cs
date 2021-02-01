using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.Processing
{
    public class SeedProcessingController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Production/SeedProcessing
        public ActionResult Index()
        {
            #region View Data
            ViewData["ProcessingPlants"] = db.ProcessingPlants.Select(p => new { p.Name, p.ProcessingPlantId}).ToList();

            ViewData["SeedProductionItems"] = db.SeedProductionOrderItems.Select(i => new { i.ProductionCode, i.SeedProductionOrderItemId }).ToList();

            ViewData["Activities"] = db.ProductionActivities.Select(a => new { a.Name, a.ProductionActivityId}).ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(c => new { c.Name, c.ItemCategoryCode }).ToList();

            ViewData["Items"] = db.InventoryItems.Select(i => new { i.Name, i.ItemCode }).ToList();

            ViewData["ProductionPeriods"] = db.ProductionPeriods.Select(p => new { p.Name, p.ProductionPeriodId }).ToList();

            //ViewData["InventoryStores"] = db.InventoryStores.Select(s => new { s.Name, s.StoreCode }).ToList();

            #endregion
            return View();
        }
        public ActionResult Inputs()
        {
            #region View Data
            ViewData["ProcessingPlants"] = db.ProcessingPlants.Select(p => new { p.Name, p.ProcessingPlantId }).ToList();

            ViewData["SeedProductionItems"] = db.SeedProductionOrderItems.Select(i => new { i.ProductionCode, i.SeedProductionOrderItemId }).ToList();

            ViewData["Activities"] = db.ProductionActivities.Select(a => new { a.Name, a.ProductionActivityId }).ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(c => new { c.Name, c.ItemCategoryCode }).ToList();

            ViewData["Items"] = db.InventoryItems.Select(i => new { i.Name, i.ItemCode }).ToList();

            ViewData["ProductionPeriods"] = db.ProductionPeriods.Select(p => new { p.Name, p.ProductionPeriodId }).ToList();

            //ViewData["InventoryStores"] = db.InventoryStores.Select(s => new { s.Name, s.StoreCode }).ToList();
            #endregion
            return View();
        }
        public ActionResult Outputs()
        {
            #region View Data
            ViewData["ProcessingPlants"] = db.ProcessingPlants.Select(p => new { p.Name, p.ProcessingPlantId }).ToList();

            ViewData["SeedProductionItems"] = db.SeedProductionOrderItems.Select(i => new { i.ProductionCode, i.SeedProductionOrderItemId }).ToList();

            ViewData["Activities"] = db.ProductionActivities.Select(a => new { a.Name, a.ProductionActivityId }).ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(c => new { c.Name, c.ItemCategoryCode }).ToList();

            ViewData["Items"] = db.InventoryItems.Select(i => new { i.Name, i.ItemCode }).ToList();

            ViewData["ProductionPeriods"] = db.ProductionPeriods.Select(p => new { p.Name, p.ProductionPeriodId }).ToList();

           // ViewData["InventoryStores"] = db.InventoryStores.Select(s => new { s.Name, s.StoreCode }).ToList();
            #endregion
            return View();
        }

        public decimal GetRawQuantityFromStore(string storeCode, string itemCode)
        {
            var qty = new decimal();
            var receiveItems = 
                db.ProductionStoreReceiveItems
                .Where(r => r.ProductionStoreReceive.StoreCode == storeCode && 
                            r.ItemCode == itemCode && r.ProductionStoreReceive.IsOnlineApproved
                            ).ToList();
            if (receiveItems.Any())
            {
                qty = receiveItems.Sum(r => r.Quantity);
            }
            return qty;
        }
    }
}