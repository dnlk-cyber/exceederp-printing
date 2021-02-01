﻿using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Inventory.Controllers
{

    public class PropertyCountController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();


        [AuthorizeRoles(InventoryConstants.PropertyCount, InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyCountApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var branches = db.Branch;
            var stores = db.InventoryStores;

            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var fiscalYears = db.GLFiscalYears;
            var periods = db.GLPeriods;

            ViewData["Years"] = fiscalYears;
            ViewData["periods"] = periods;

            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();
            ViewData["Stores"] = stores;
            ViewData["branches"] = branches;
            ViewData["defaultCategory"] = branches.FirstOrDefault();
        }


        [AuthorizeRoles(InventoryConstants.PropertyCount, InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyCountApprover)]
        public ActionResult PropertyCounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<PropertyCount> propertycounts = db.PropertyCounts.Where(x => userStores.Contains(x.StoreCode));

            DataSourceResult result = propertycounts.ToDataSourceResult(request, propertyCount => new
            {
                PropertyCountId = propertyCount.PropertyCountId,
                CountDate = propertyCount.CountDate,
                BranchId = propertyCount.BranchId,
                StoreCode = propertyCount.StoreCode,
                IsMidOfPeriod = propertyCount.IsMidOfPeriod,
                GlFiscalYearId = propertyCount.GlFiscalYearId,
                GLPeriodId = propertyCount.GLPeriodId,
                StoreKeeperId = UserInformationHelper.GetUserFullNamebyId(propertyCount.StoreKeeperId),
                IsClosed = propertyCount.IsClosed,
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyCount)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PropertyCounts_Create([DataSourceRequest]DataSourceRequest request, PropertyCount propertyCount)
        {
            var exist = db.PropertyCounts.Where(x => x.BranchId == propertyCount.BranchId)
                .Where(x => x.StoreCode == propertyCount.StoreCode)
                .Where(x => x.CountDate ==propertyCount.CountDate.Date).AsQueryable();
            if (exist.Any())
                ModelState.AddModelError("exist", "Count for date " + propertyCount.CountDate + " and store " + propertyCount.StoreCode + " already exists");

            propertyCount.StoreKeeperId =ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreKeeper(propertyCount.StoreCode);

            if (ModelState.IsValid)
            {
                var entity = new PropertyCount
                {
                    CountDate =propertyCount.CountDate.Date,
                    BranchId = propertyCount.BranchId,
                    StoreCode = propertyCount.StoreCode,
                    GlFiscalYearId = propertyCount.GlFiscalYearId,
                    GLPeriodId = propertyCount.GLPeriodId,
                    StoreKeeperId = propertyCount.StoreKeeperId,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    OrgBranchName = propertyCount.BranchId.ToString()
                };

                db.PropertyCounts.Add(entity);
                db.SaveChanges();
                propertyCount.PropertyCountId = entity.PropertyCountId;
                GetItemsInStore(entity);
            }

            #region disable store

            var store = db.InventoryStores.Find(propertyCount.StoreCode);
            if (store != null)
            {
                store.Enable = false;
                db.Entry(store).State = EntityState.Modified;
                db.SaveChanges();
            }

            #endregion
            return Json(new[] { propertyCount }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyCount)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PropertyCounts_Update([DataSourceRequest]DataSourceRequest request, PropertyCount propertyCount)
        {
            var exist = db.PropertyCounts.Where(x => x.PropertyCountId != propertyCount.PropertyCountId)
                            .Where(x => x.BranchId == propertyCount.BranchId)
                            .Where(x => x.StoreCode == propertyCount.StoreCode)
                            .Where(x => x.CountDate == propertyCount.CountDate.Date).AsQueryable();
            if (exist.Any())
                ModelState.AddModelError("exist", "Count for date " + propertyCount.CountDate + " and store " + propertyCount.StoreCode + " already exists");

            propertyCount.StoreKeeperId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreKeeper(propertyCount.StoreCode);

            if (ModelState.IsValid)
            {
                var entity = new PropertyCount
                {
                    PropertyCountId = propertyCount.PropertyCountId,
                    CountDate = propertyCount.CountDate.Date,
                    BranchId = propertyCount.BranchId,
                    StoreCode = propertyCount.StoreCode,
                    GlFiscalYearId = propertyCount.GlFiscalYearId,
                    GLPeriodId = propertyCount.GLPeriodId,
                    StoreKeeperId = propertyCount.StoreKeeperId,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                };

                db.PropertyCounts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region disable store

                var store = db.InventoryStores.Find(propertyCount.StoreCode);
                if (store != null)
                {
                    store.Enable = false;
                    db.Entry(store).State = EntityState.Modified;
                    db.SaveChanges();
                }

                #endregion

            }

            return Json(new[] { propertyCount }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyCount)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PropertyCounts_Destroy([DataSourceRequest]DataSourceRequest request, PropertyCount propertyCount)
        {
            if (ModelState.IsValid)
            {
                var entity = new PropertyCount
                {
                    PropertyCountId = propertyCount.PropertyCountId,
                    CountDate = propertyCount.CountDate,
                    BranchId = propertyCount.BranchId,
                    StoreCode = propertyCount.StoreCode,

                };

                db.PropertyCounts.Attach(entity);
                db.PropertyCounts.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted property count number " + propertyCount.PropertyCountId + " " + propertyCount.Remark);
                #endregion
            }

            return Json(new[] { propertyCount }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult RefreshStoreItems(int id)
        {
            var countRecord = db.PropertyCounts.Find(id);
            var storeitems = db.StoreItemAssignments.Where(x => x.StoreCode == countRecord.StoreCode).ToList();
            var countItems = db.PropertyCountItems.Where(i => i.PropertyCountId == id).ToList();

            foreach (var item in storeitems)
            {
                var countItem = countItems.FirstOrDefault(x => x.ItemCode == item.ItemCode);
                if (countItem == null)
                {
                    var inventoryitem = db.InventoryItems.Find(item.ItemCode);
                    #region missing item

                    if (countRecord != null)
                    {
                        if (inventoryitem != null)
                        {
                            var propertyCountItem = new PropertyCountItem
                            {
                                ItemCategoryCode = item.ItemCategoryCode,
                                ItemName=inventoryitem.Name,
                                ItemCode = item.ItemCode,
                                ItemCodeReadOnly = item.ItemCode.Trim(),
                                PropertyCountId = id,
                                Count = 0,
                                PreviousBalance = item.Balance,
                                Price = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(item.ItemCode, countRecord.StoreCode),
                                Status = false,
                                Location=item.ItemLocation,
                                CreatedBy = "Update missing item and balance"
                            };

                            db.PropertyCountItems.Add(propertyCountItem);
                        }

                        db.SaveChanges();
                    }

                    #endregion
                }
                else
                {
                    if (!countItem.Updated)
                    {
                        #region update item item

                        countItem.PreviousBalance = item.Balance;
                        db.Entry(countItem).State = EntityState.Modified;
                        db.SaveChanges();

                        #endregion
                    }
                }
            }

            var formattedData1 = new
            {
                Message = "Completed.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }
        private void GetItemsInStore(PropertyCount propertyCount)
        {
            List<PropertyCountItem> list = new List<PropertyCountItem>();
            var itmeList = db.StoreItemAssignments.Where(x => x.StoreCode == propertyCount.StoreCode).ToList();
            foreach (var item in itmeList)
            {
                var inventoryitem = db.InventoryItems.Find(item.ItemCode);
                if (inventoryitem != null)
                {
                    var propertyCountItem = new PropertyCountItem
                    {
                        ItemCategoryCode = item.ItemCategoryCode,
                        ItemCode = item.ItemCode,
                        ItemCodeReadOnly = item.ItemCode.Trim(),
                        ItemName =inventoryitem.Name,
                        PropertyCountId = propertyCount.PropertyCountId,
                        Count = 0,
                        PreviousBalance = item.Balance,
                        Location=item.ItemLocation,
                        Price = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(item.ItemCode, propertyCount.StoreCode),
                        Status = false,
                        CreatedBy = "Loading Store items"
                    };

                    list.Add(propertyCountItem);
                    //db.PropertyCountItems.Add(propertyCountItem);
                }

                //db.SaveChanges();
            }
            db.PropertyCountItems.AddRange(list);
            db.SaveChanges();
        }

        private string GetStatus(int id)
        {
            var data = db.PropertyCountItems.Where(a => a.PropertyCountId.Equals(id)).Where(x => x.Status == false);
            if (data.Any())
            {
                return "There are items with status not updated.Please update them first";
            }

            var price = db.PropertyCountItems
                .Where(x => x.PropertyCountId==id)
                .Where(x=>x.Count>0)
                .Where(x => x.Price == 0);
            if (price.Any())
            {
                return "There are items with zero price.Please update them first";
            }
            return null;
        }

        public ActionResult PropertyCountPrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyCountApprover)]
        public JsonResult UpdateBalance(int id)
        {
            string errorMessage = GetStatus(id);
            if (!String.IsNullOrEmpty(errorMessage))
            {
                var formattedData0 = new
                {
                    Message = errorMessage,
                    Status = "NOT_OK"
                };
                return Json(formattedData0, JsonRequestBehavior.AllowGet);
            }
            var record = db.PropertyCounts.Find(id);
            var propertyCountItems = db.PropertyCountItems.Where(x => x.PropertyCountId == id).ToList();
            if (record != null)
            {
                foreach (var item in propertyCountItems)
                {
                    if (!item.Updated)
                    {
                        var storeItem = db.StoreItemAssignments.FirstOrDefault(x =>
                            x.StoreCode == record.StoreCode && x.ItemCode == item.ItemCode);
                        if (storeItem != null)
                        {
                            storeItem.Balance = item.Count;
                            storeItem.AvgPrice = item.Price;//added 
                            storeItem.ItemLocation = !String.IsNullOrWhiteSpace(item.Location)? item.Location:storeItem.ItemLocation;
                            item.Updated = true;
                            db.Entry(item).State = EntityState.Modified;
                            db.Entry(storeItem).State = EntityState.Modified;
                            db.SaveChanges();

                            #region bin card line
                            var binCardLine = new BinCardLine
                            {
                                StoreItemAssignment = storeItem,
                                ReferenceDoc = "Count on date " + record.CountDate.ToShortDateString(),
                                Reference = record.PropertyCountId.ToString(),
                                Date = record.CountDate,
                                BalanceQTY = storeItem.Balance,
                                BalanceUPrice = storeItem.AvgPrice,
                                BalanceTPrice = storeItem.AvgPrice * storeItem.Balance,
                                TimeStamp = DateTime.Now,
                                TransactionType= BinCardTransactionTypes.Count,
                                StatusType= BinCardTransactionStatus.Regular
                            };
                            db.BinCardLines.Add(binCardLine);
                            db.SaveChanges();
                            #endregion
                        }
                        else
                        {
                            #region create store item

                            storeItem = new StoreItemAssignment
                            {
                                StoreCode = record.StoreCode,
                                ItemCategoryCode = item.ItemCategoryCode,
                                ItemCode = item.ItemCode,
                                Balance = item.Count,
                                AvgPrice = item.Price,
                                ItemLocation = item.Location,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now
                            };
                            item.Updated = true;
                            db.StoreItemAssignments.Add(storeItem);
                            db.SaveChanges();

                            #endregion
                            #region bin card line


                            var binCardLine = new BinCardLine
                            {
                                StoreItemAssignment = storeItem,
                                ReferenceDoc = "Count on date " + record.CountDate.ToShortDateString(),
                                Reference = record.PropertyCountId.ToString(),
                                Date = record.CountDate,
                                BalanceQTY = storeItem.Balance,
                                BalanceUPrice = storeItem.AvgPrice,
                                BalanceTPrice = storeItem.AvgPrice * storeItem.Balance,
                                TimeStamp = DateTime.Now
                            };
                            db.BinCardLines.Add(binCardLine);
                            db.SaveChanges();



                            #endregion

                        }

                    }

                }
                var formattedData = new
                {
                    Message = "Updating items completed.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var formattedData2 = new
            {
                Message = "The count is not found.",
                Status = "NOT_OK"
            };
            return Json(formattedData2, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(InventoryConstants.PropertyCountApprover)]
        public JsonResult ReverseUpdate(int id)
        {
            var record = db.PropertyCounts.Find(id);
            var propertyCountItems = db.PropertyCountItems.Where(x => x.PropertyCountId == id).ToList();

            #region validate if all are updated

            foreach (var item in propertyCountItems)
            {
                if (!item.Updated)
                {
                    var formattedData = new
                    {
                        Message = "There are items not update, please update before reversing.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

            }

            #endregion

            foreach (var item in propertyCountItems)
            {
                var storeItem = db.StoreItemAssignments.FirstOrDefault(x => x.StoreCode == record.StoreCode && x.ItemCode == item.ItemCode);
                if (storeItem != null)
                {
                    #region reverse store balance
                    storeItem.Balance = item.PreviousBalance;
                    item.Updated = false;
                    db.Entry(item).State = EntityState.Modified;
                    db.Entry(storeItem).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion

                }

            }
            if (record != null)
            {
                var referenceDoc = "Count on date " + record.CountDate.ToShortDateString();
                var reference = record.PropertyCountId.ToString();
                #region remove bin card
                var bincardLines = db.BinCardLines
                    .Where(x => x.ReferenceDoc == referenceDoc && x.Reference == reference).ToList();
                db.BinCardLines.RemoveRange(bincardLines);
                db.SaveChanges();

                #endregion
            }
            var formattedData1 = new
            {
                Message = "Reversed Successfully.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeRoles(InventoryConstants.PropertyCountApprover)]
        public JsonResult CloseCount(int id)
        {
            
            var record = db.PropertyCounts.Find(id);
            
            if (record != null)
            {
                var propertyCountItems = db.PropertyCountItems.Where(x => x.PropertyCountId == id).ToList();
                #region validate if all are updated

                foreach (var item in propertyCountItems)
                {
                    if (!item.Updated)
                    {
                        var formattedData = new
                        {
                            Message = "There are items not update, please update before closing.\n " + item.ItemCode + "-" + item.InventoryItem.Name,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }
                #endregion

                record.IsClosed = true;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
            }
           
           
            var formattedData1 = new
            {
                Message = "Count is closed Sucessfully.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeRoles(InventoryConstants.PropertyCount)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(InventoryConstants.PropertyCount)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
