using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Settings
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
    public class ItemMergeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

       

        public ActionResult Index()
        {
            PopulateCategories();
            return View();
        }
        private void PopulateCategories()
        {
            var dataContext = new ExceedDbContext();
            var categories = dataContext.ItemCategorys;

            ViewData["categories"] = categories;
            //ViewData["defaultCategory"] = categories.FirstOrDefault();
        }

        public ActionResult Items_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InventoryItem> items = db.InventoryItems;
            DataSourceResult result = items.ToDataSourceResult(request, item => new
            {
                ItemCode = item.ItemCode,
                Name = item.Name,
                Type = item.Type,
                ItemCategoryCode = item.ItemCategoryCode,
                SubCategoryCode = item.SubCategoryCode,
                Balance = item.Balance,
                Enable = item.Enable,
                AvgPrice = item.AvgPrice,
                TotalPrice = item.TotalPrice,
                UoMName = item.UoMName,
                PartNo = item.PartNo,
                ItemIndex = item.ItemIndex,
                Prefix = item.Prefix,
            });

            return Json(result);
        }

        public class ItemVM
        {
            public string ItemCode { get; set; }
            public string Name { get; set; }
        }   
        public JsonResult Merge(List<ItemVM> selected, List<ItemVM> destination)
        {
            List<string> affectedStores = new List<string>();
            var destiObject = destination.FirstOrDefault();
            string destiItemCode = destiObject.ItemCode;
            var item = db.InventoryItems.Find(destiObject.ItemCode);
            var storeItemDest = db.StoreItemAssignments.Where(x => x.ItemCode == destiItemCode).Select(x => x.StoreCode).Distinct().ToList();
            if (storeItemDest.Any())
                affectedStores.AddRange(storeItemDest);

            foreach(var sel in selected)
            {
                var storeItemSel= db.StoreItemAssignments.Where(x => x.ItemCode == sel.ItemCode).Select(x => x.StoreCode).Distinct().ToList();
                if (storeItemSel.Any())
                    affectedStores.AddRange(storeItemSel);
            }

            var distinctStores = affectedStores.Distinct().ToList();
            var distinctSelectItems = selected.Where(x=>x.ItemCode!=destiItemCode).Select(x=>x.ItemCode).Distinct().ToList();
            #region merge data
            foreach (var store in distinctStores)
            {

                var destiStoreItem = db.StoreItemAssignments
                    .Where(x => x.ItemCode == destiItemCode)
                     .Where(x => x.StoreCode == store).FirstOrDefault();
                if(destiStoreItem!=null)
                {
                    #region update
                    decimal balanceTotransfer = 0;
                    // added for average price
                    decimal totalOfAvgPriceMulByBalance = 0;

                    foreach (var sel in distinctSelectItems)
                    {
                        var selectStoreItem = db.StoreItemAssignments
                            .Where(x => x.ItemCode == sel)
                            .Where(x => x.StoreCode == store).FirstOrDefault();

                        if (selectStoreItem != null)
                        {
                            balanceTotransfer = balanceTotransfer + selectStoreItem.Balance;
                            // added for average price
                            totalOfAvgPriceMulByBalance = totalOfAvgPriceMulByBalance + (selectStoreItem.Balance * selectStoreItem.AvgPrice);

                            selectStoreItem.Balance = 0;
                            selectStoreItem.AvgPrice = 0;
                            db.StoreItemAssignments.Attach(selectStoreItem);
                            db.Entry(selectStoreItem).State = EntityState.Modified;
                        }
                    }
                    decimal totalAmountExisting = destiStoreItem.Balance * destiStoreItem.AvgPrice;
                    destiStoreItem.Balance = destiStoreItem.Balance + balanceTotransfer;

                    // added for average price
                    if(destiStoreItem.Balance != 0)
                    {
                        destiStoreItem.AvgPrice = (totalOfAvgPriceMulByBalance + totalAmountExisting) / destiStoreItem.Balance;
                    }


                    db.StoreItemAssignments.Attach(destiStoreItem);
                    db.Entry(destiStoreItem).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion
                }
                else
                {
                    #region create store item
                    decimal balanceTotransfer = 0;
                    // added for average price
                    decimal totalOfAvgPriceMulByBalance = 0;

                    foreach (var sel in distinctSelectItems)
                    {
                        var selectStoreItem = db.StoreItemAssignments.Where(x => x.ItemCode == sel).Where(x => x.StoreCode == store).FirstOrDefault();
                        if (selectStoreItem != null)
                        {
                            balanceTotransfer = balanceTotransfer + selectStoreItem.Balance;
                            // added for average price
                            totalOfAvgPriceMulByBalance = totalOfAvgPriceMulByBalance + (selectStoreItem.Balance * selectStoreItem.AvgPrice);

                            selectStoreItem.Balance = 0;
                            selectStoreItem.AvgPrice = 0;
                            db.StoreItemAssignments.Attach(selectStoreItem);
                            db.Entry(selectStoreItem).State = EntityState.Modified;
                        }
                    }

                    var entity = new StoreItemAssignment
                    {
                        StoreCode = store,
                        ItemCategoryCode = item.ItemCategoryCode,
                        ItemCode = destiItemCode,
                        ItemName = item.Name,
                        Balance = balanceTotransfer,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };
                    // added for average price
                    if (entity.Balance != 0)
                    {
                        entity.AvgPrice = totalOfAvgPriceMulByBalance / entity.Balance;
                    }

                    db.StoreItemAssignments.Add(entity);
                    db.SaveChanges();
                    #endregion
                }
            }
            #endregion
            #region disable items
            foreach (var sel in distinctSelectItems)
            {
                var itemSel = db.InventoryItems.Find(sel);
                itemSel.Enable = false;
                db.InventoryItems.Attach(itemSel);
                db.Entry(itemSel).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Item merge", "Success", User.Identity.Name, "merged item  " + itemSel.ItemCode + " to " + item.ItemCode, Modules.Inventory);
                #endregion
            }
            #endregion

            var formattedData = new
            {
                Message = "Operation complete.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
