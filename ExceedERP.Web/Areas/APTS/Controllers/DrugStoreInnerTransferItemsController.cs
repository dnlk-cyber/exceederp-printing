using ExceedERP.Core.Domain.APTS;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.APTS.Controllers
{
    public class DrugStoreInnerTransferItemsController : Controller
    {
        // GET: APTS/DrugStoreInnerTransferItems
        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["ItemCatagory"] = db.ItemCategorys.ToList();
            ViewData["Store"] = db.InventoryStores.ToList();
            ViewData["Unit"] = db.UoMs.ToList();
            ViewData["Items"] = db.InventoryItems.ToList();
        }
        public ActionResult DrugStoreInnerTransferItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {


            var stores = db.DrugToTransferTemporaryLists.Where(x => x.DrugStoreTransferId == id && !x.IsNew);

            DataSourceResult result = stores.ToDataSourceResult(request, drugToTransferTemporaryLists => new
            {
                DrugToTransferTemporaryListsId = drugToTransferTemporaryLists.DrugToTransferTemporaryListsId,
                DrugListsId = drugToTransferTemporaryLists.DrugListsId,
                DrugStoreTransferId = drugToTransferTemporaryLists.DrugStoreTransferId,
                DrugUnitId = drugToTransferTemporaryLists.DrugUnitId,
                Quantity = drugToTransferTemporaryLists.Quantity,
            });

            return Json(result);
        }
        public ActionResult DrugStoreInnerTransferItems_Create([DataSourceRequest]DataSourceRequest request, DrugToTransferTemporaryLists drugToTransferTemporaryLists, int id)
        {
          
            if (ModelState.IsValid)
            {
                var entity = new DrugToTransferTemporaryLists
                {
                    DrugListsId = drugToTransferTemporaryLists.DrugListsId,
                    DrugStoreTransferId = id,
                    DrugUnitId = drugToTransferTemporaryLists.DrugUnitId,
                    Quantity = drugToTransferTemporaryLists.Quantity,
                };
                db.DrugToTransferTemporaryLists.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { drugToTransferTemporaryLists }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugStoreInnerTransferItems_Update([DataSourceRequest]DataSourceRequest request, DrugToTransferTemporaryLists drugToTransferTemporaryLists)
        {

            if (ModelState.IsValid)
            {
                var entity = new DrugToTransferTemporaryLists
                {
                    DrugToTransferTemporaryListsId = drugToTransferTemporaryLists.DrugToTransferTemporaryListsId,
                    DrugListsId = drugToTransferTemporaryLists.DrugListsId,
                    DrugStoreTransferId = drugToTransferTemporaryLists.DrugStoreTransferId,
                    DrugUnitId = drugToTransferTemporaryLists.DrugUnitId,
                    Quantity = drugToTransferTemporaryLists.Quantity,
                };

                db.DrugToTransferTemporaryLists.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { drugToTransferTemporaryLists }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugStoreInnerTransferItems_Destroy([DataSourceRequest]DataSourceRequest request, DrugToTransferTemporaryLists drugToTransferTemporaryLists)
        {
            if (ModelState.IsValid)
            {
                var entity = new DrugToTransferTemporaryLists
                {
                    DrugToTransferTemporaryListsId = drugToTransferTemporaryLists.DrugToTransferTemporaryListsId,
                    DrugListsId = drugToTransferTemporaryLists.DrugListsId,
                    DrugStoreTransferId = drugToTransferTemporaryLists.DrugStoreTransferId,
                    DrugUnitId = drugToTransferTemporaryLists.DrugUnitId,
                    Quantity = drugToTransferTemporaryLists.Quantity,
                };

                db.DrugToTransferTemporaryLists.Attach(entity);
                db.DrugToTransferTemporaryLists.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { drugToTransferTemporaryLists }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetDrugList()
        {
            var drugList = db.DrugLists.ToList();
            return Json(drugList.Select(p => new
            {
                DrugListsId = p.DrugListsId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}