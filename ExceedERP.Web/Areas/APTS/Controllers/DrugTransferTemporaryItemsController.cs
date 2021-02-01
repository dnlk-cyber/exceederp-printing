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
    public class DrugTransferTemporaryItemsController : Controller
    {
        // GET: APTS/DrugTransferTemporaryItems
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
        public ActionResult DrugTransferTemporaryItems_Read([DataSourceRequest]DataSourceRequest request, int storeTransferId)
        {
            var stores = db.DrugToTransferTemporaryLists.Where(x => x.DrugStoreTransferId == storeTransferId && x.IsNew);

            DataSourceResult result = stores.ToDataSourceResult(request, drugToTransferTemporaryLists => new
            {
                DrugListsId = drugToTransferTemporaryLists.DrugListsId,
                DrugStoreTransferId = drugToTransferTemporaryLists.DrugStoreTransferId,
                DrugUnitId = drugToTransferTemporaryLists.DrugUnitId,
                IsNew = drugToTransferTemporaryLists.IsNew,
                HaveNoBalance = drugToTransferTemporaryLists.HaveNoBalance,
                Quantity = drugToTransferTemporaryLists.Quantity
            });

            return Json(result);
        }
        public ActionResult DrugTransferTemporaryItems_Create([DataSourceRequest]DataSourceRequest request, DrugToTransferTemporaryLists drugToTransferTemporaryLists, int storeTransferId)
        {

            if (ModelState.IsValid)
            {
                var entity = new DrugToTransferTemporaryLists
                {
                    DrugListsId = drugToTransferTemporaryLists.DrugListsId,
                    DrugStoreTransferId = storeTransferId,
                    IsNew = true,
                    DrugUnitId = drugToTransferTemporaryLists.DrugUnitId,
                    Quantity = drugToTransferTemporaryLists.Quantity
                };
                db.DrugToTransferTemporaryLists.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { drugToTransferTemporaryLists }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugTransferTemporaryItems_Update([DataSourceRequest]DataSourceRequest request, DrugToTransferTemporaryLists drugToTransferTemporaryLists)
        {

            if (ModelState.IsValid)
            {
                var entity = new DrugToTransferTemporaryLists
                {
                    DrugListsId = drugToTransferTemporaryLists.DrugListsId,
                    DrugStoreTransferId = drugToTransferTemporaryLists.DrugStoreTransferId,
                    DrugUnitId = drugToTransferTemporaryLists.DrugUnitId,
                    IsNew = drugToTransferTemporaryLists.IsNew,
                    Quantity = drugToTransferTemporaryLists.Quantity
                };

                db.DrugToTransferTemporaryLists.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { drugToTransferTemporaryLists }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugTransferTemporaryItems_Destroy([DataSourceRequest]DataSourceRequest request, DrugToTransferTemporaryLists drugToTransferTemporaryLists)
        {
            if (ModelState.IsValid)
            {
                var entity = new DrugToTransferTemporaryLists
                {
                    DrugListsId = drugToTransferTemporaryLists.DrugListsId,
                    DrugStoreTransferId = drugToTransferTemporaryLists.DrugStoreTransferId,
                    DrugUnitId = drugToTransferTemporaryLists.DrugUnitId,
                    IsNew = drugToTransferTemporaryLists.IsNew,
                    Quantity = drugToTransferTemporaryLists.Quantity
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
        public JsonResult TransferDrugToOtherStore(int DrugStoreTransferId)
        {
            bool CheckMixed = false;
            bool CheckNo = false;

            var store = db.DrugStoreTransfers.Find(DrugStoreTransferId);
            if (store != null)
            {
                var storeTemporaryItems = db.DrugToTransferTemporaryLists.Where(x => x.DrugStoreTransferId == DrugStoreTransferId && x.IsNew).ToList();
                if (storeTemporaryItems.Any())
                {
                    foreach (var item in storeTemporaryItems)
                    {
                        db = new ExceedDbContext();

                        var entity = new DrugStoreItem
                        {
                            DrugListsId = item.DrugListsId,
                            DrugUnitId = item.DrugUnitId,
                            DrugStoreId = store.ToDrugStoreId,
                            Balance = item.Quantity,
                            IsInnerTransfered = true,

                        };

                        db.DrugStoreItems.Add(entity);

                        item.IsNew = false;
                        item.HaveNoBalance = false;
                        db.Entry(item).State = EntityState.Modified;

                        var fromDrugStore = db.DrugStoreItems.FirstOrDefault(x=>x.DrugStoreId == store.FromDrugStoreId && x.DrugListsId == item.DrugListsId && x.DrugUnitId == item.DrugUnitId);
                        if(fromDrugStore != null)
                        {
                            if(fromDrugStore.Balance < item.Quantity)
                            {
                                CheckNo = true;                                
                                continue;
                            }
                            fromDrugStore.Balance = fromDrugStore.Balance - item.Quantity;                            
                        }
                        else
                        {
                            continue;
                        }
                        CheckMixed = true;

                        db.SaveChanges();
                    }
                    var changeStatusForColors = db.DrugToTransferTemporaryLists.Where(x=>x.IsNew).ToList();
                    foreach(var change in changeStatusForColors)
                    {
                        change.HaveNoBalance = true;

                        db.Entry(change).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if(CheckMixed && CheckNo)
                    {
                        var formatedData2 = new
                        {
                            Message = "Some drugs have no balance some are transfered successfully",
                            Status = "OK"
                        };
                        return Json(formatedData2, JsonRequestBehavior.AllowGet);
                    }
                    if (!CheckMixed && CheckNo)
                    {
                        var formatedData2 = new
                        {
                            Message = "The drug you trying to transfer have no balance",
                            Status = "OK"
                        };
                        return Json(formatedData2, JsonRequestBehavior.AllowGet);
                    }
                    var formatedData = new
                    {
                        Message = "Drug Transfer Successfull",
                        Status = "OK"
                    };
                    return Json(formatedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formatedData = new
                    {
                        Message = "Please add drugs first to transfer",
                        Status = "FAIL"
                    };
                    return Json(formatedData, JsonRequestBehavior.AllowGet);
                }
            }
            var formatedData1 = new
            {
                Message = "Please select the row first",
                Status = "FAIL"
            };
            return Json(formatedData1, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}