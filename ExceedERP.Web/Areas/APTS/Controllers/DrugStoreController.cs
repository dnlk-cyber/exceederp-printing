using ExceedERP.Core.Domain.APTS;
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
    public class DrugStoreController : Controller
    {
        // GET: APTS/DrugStore
        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["Branch"] = db.Branch.ToList();
            ViewData["DrugStore"] = db.InventoryStores.ToList(); //db.DrugStores.ToList();
            ViewData["Store"] = db.InventoryStores.ToList();
            ViewData["Unit"] = db.DrugUnits.ToList();
            ViewData["DrugList"] = db.DrugLists.ToList();
            ViewData["Items"] = db.InventoryItems.ToList();
            ViewData["ItemCatagory"] = db.ItemCategorys.ToList();
        }
        public ActionResult DrugStore_Read([DataSourceRequest]DataSourceRequest request)
        {
            var drugStore = db.DrugStores.ToList();
            DataSourceResult result = drugStore.ToDataSourceResult(request, store => new
            {
                DrugStoreId = store.DrugStoreId,
                BranchId = store.BranchId,
                StoreCode = store.StoreCode,
                Name = store.Name,
                Remark = store.Remark
            });

            return Json(result);
        }
        public ActionResult DrugStore_Create([DataSourceRequest]DataSourceRequest request, DrugStore store)
        {
            //var checkDrug = db.DrugStores.FirstOrDefault(x => x.DrugListsId == drugStore.DrugListsId);
            //if (checkDrug != null)
            //{
            //    ModelState.AddModelError("", "This Drug is already registered ");
            //}
            if (store != null)
            {
                //var code = store.StoreCode.Split('-')[0];
                //var name = store.StoreCode.Split('-')[1];



                if (ModelState.IsValid)
                {
                    var entity = new DrugStore
                    {
                        DrugStoreId = store.DrugStoreId,
                        BranchId = store.BranchId,
                        StoreCode = store.StoreCode,//code,
                        Name = db.InventoryStores.FirstOrDefault(x=>x.StoreCode == store.StoreCode)?.Name,//store.Name,//name,
                        Remark = store.Remark
                    };
                    db.DrugStores.Add(entity);
                    db.SaveChanges();
                    store.DrugStoreId = entity.DrugStoreId;
                }
            }
            return Json(new[] { store }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugStore_Update([DataSourceRequest]DataSourceRequest request, DrugStore store)
        {

            if (ModelState.IsValid)
            {
                //if (store != null)
                //{
                    //var code = store.StoreCode.Split('-')[0];
                    //var name = store.StoreCode.Split('-')[1];

                    var entity = new DrugStore
                    {
                        DrugStoreId = store.DrugStoreId,
                        BranchId = store.BranchId,
                        StoreCode = store.StoreCode,//code,
                        Name = db.InventoryStores.FirstOrDefault(x => x.StoreCode == store.StoreCode)?.Name,//store.Name,//name,
                        Remark = store.Remark

                    };

                    db.DrugStores.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                //}
            }

            return Json(new[] { store }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugStore_Destroy([DataSourceRequest]DataSourceRequest request, DrugStore store)
        {
            if (ModelState.IsValid)
            {
                var entity = new DrugStore
                {
                    DrugStoreId = store.DrugStoreId,
                    BranchId = store.BranchId,
                    StoreCode = store.StoreCode,
                    Name = store.Name,
                    Remark = store.Remark

                };

                db.DrugStores.Attach(entity);
                db.DrugStores.Remove(entity);
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