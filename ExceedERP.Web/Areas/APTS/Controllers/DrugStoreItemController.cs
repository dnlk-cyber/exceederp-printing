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
    public class DrugStoreItemController : Controller
    {
        // GET: APTS/DrugStoreItem
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
        public ActionResult DrugStoreItem_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
       

            var stores = db.DrugStoreItems.Where(x=>x.DrugStoreId == id);
           
            DataSourceResult result = stores.ToDataSourceResult(request, store => new
            {
                DrugListsId = store.DrugListsId,
                DrugStoreId = store.DrugStoreId,
                DrugStoreItemId = store.DrugStoreItemId,
                DrugUnitId = store.DrugUnitId,
                ItemCode = store.ItemCode,
                Balance = store.Balance
            });

            return Json(result);
        }
        public ActionResult DrugStoreItem_Create([DataSourceRequest]DataSourceRequest request, DrugStoreItem store,int idTwo)
        {
            var storeCode = db.DrugStores.FirstOrDefault(x => x.DrugStoreId == idTwo);
          
            
            if (ModelState.IsValid)
            {
                var entity = new DrugStoreItem
                {
                    DrugListsId = store.DrugListsId,
                    DrugStoreId = idTwo,
                    DrugUnitId = store.DrugUnitId,
                    ItemCode = store.ItemCode,
                    IsInnerTransfered = false,
                    Balance = store.Balance
                };
                db.DrugStoreItems.Add(entity);
                db.SaveChanges();
                store.DrugStoreItemId = entity.DrugStoreItemId;
            }

            return Json(new[] { store }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugStoreItem_Update([DataSourceRequest]DataSourceRequest request, DrugStoreItem store)
        {

            if (ModelState.IsValid)
            {
                var entity = new DrugStoreItem
                {
                    DrugListsId = store.DrugListsId,
                    DrugStoreId = store.DrugStoreId,
                    DrugStoreItemId = store.DrugStoreItemId,
                    DrugUnitId = store.DrugUnitId,
                    ItemCode = store.ItemCode,
                    IsInnerTransfered = store.IsInnerTransfered,
                    Balance = store.Balance
                };

                db.DrugStoreItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { store }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugStoreItem_Destroy([DataSourceRequest]DataSourceRequest request, DrugStoreItem store)
        {
            if (ModelState.IsValid)
            {
                var entity = new DrugStoreItem
                {
                   DrugListsId = store.DrugListsId,
                   DrugStoreId = store.DrugStoreId,
                   DrugStoreItemId = store.DrugStoreItemId,
                   DrugUnitId = store.DrugUnitId,
                   ItemCode = store.ItemCode,
                   IsInnerTransfered = store.IsInnerTransfered,
                    Balance = store.Balance
                };

                db.DrugStoreItems.Attach(entity);
                db.DrugStoreItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { store }.ToDataSourceResult(request, ModelState));
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