using ExceedERP.Core.Domain.APTS;
using ExceedERP.Core.Domain.Common;
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
    public class DrugReceivalValidationController : Controller
    {
        //GET: APTS/DrugReceivalValidation
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DrugReceivalValidation_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<APTSValidation> drugReceivalModel19s = db.APTSValidations.Where(x => x.DrugReceivalModel19Id == id);
            DataSourceResult result = drugReceivalModel19s.ToDataSourceResult(request, drugReceival => new
            {
                DrugReceivalModel19Id = drugReceival.DrugReceivalModel19Id,
                LastModified = drugReceival.LastModified,
                ModifiedBy = drugReceival.ModifiedBy,
                Remark = drugReceival.Remark,
                Status = drugReceival.Status,
                CreatedBy = drugReceival.CreatedBy,
                DateCreated = drugReceival.DateCreated
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DrugReceivalValidation_Create([DataSourceRequest] DataSourceRequest request, APTSValidation aptsValidation, int id)
        {
            var record = db.DrugReceivalModel19s.Find(id);
            var items = db.DrugReceivalDetails.Where(x => x.DrugReceivalModel19Id == id).ToList();
            var pricesetting = db.AveragePriceSettings.FirstOrDefault();
            if (pricesetting == null)
                ModelState.AddModelError("price setting", "Average price setting is not defined.");
            #region Checking the grn

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                if (items.Count == 0)
                {
                    string message = "The Record Does not Contain Any Item";
                    ModelState.AddModelError(String.Empty, message);
                }
                if (aptsValidation.Status == ApprovalStatuses.Approve)
                {
                    //record.DrugReceivalModel19Id = id;
                    //record.Status = "Approved";
                    //record.IsOnlineApproved = true;
                    //record.OnlineApprovedBy = User.Identity.Name;
                    //record.OnlineApprovedTime = DateTime.Now;
                    //db.Entry(record).State = EntityState.Modified;
                    //db.SaveChanges();

                    foreach (var i in items)
                    {
                        var drugItem = db.DrugLists.FirstOrDefault(x => x.DrugListsId == i.DrugListsId);
                        if (drugItem != null)
                        {
                            var item = db.InventoryItems.FirstOrDefault(ic => ic.ItemCode == drugItem.ItemCode);
                            if (item == null)
                            {
                                string message = "Item " + drugItem.ItemCode + " is not defined in the system.";
                                ModelState.AddModelError("stockItem", message);
                            }
                        }

                    }
                }
                if (aptsValidation.Status == ApprovalStatuses.Reject)
                {
                    record.IsSendForApproval = false;
                    record.Status = "Rejected";
                }
            }
            else
            {
                ModelState.AddModelError("No Found", "GRN record not found");
            }


            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    //validate 
                    if (aptsValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.DrugReceivalModel19Id = id;
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;

                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();

                        var store = db.InventoryStores.Find(record.StoreCode);
                        foreach (var i in items)
                        {
                            if (i.QtyReceived > 0)
                            {

                                #region update stock avg price and total



                                //var drugItemLists = db.DrugReceivalDetails.Where(x => x.DrugReceivalModel19Id == id).ToList();
                                //foreach (var drug in drugItemLists)
                                //{
                                var drugReceival = db.DrugReceivalModel19s.FirstOrDefault(x => x.DrugReceivalModel19Id == i.DrugReceivalModel19Id);
                                if (drugReceival != null)
                                {
                                    var drugSt = db.DrugStores.FirstOrDefault(x => x.DrugStoreId == drugReceival.DrugStoreId);
                                    if (drugSt != null)
                                    {
                                        var receival = db.DrugStoreItems.FirstOrDefault(x => x.DrugListsId == i.DrugListsId && x.DrugUnitId == i.DrugUnitId && x.DrugStoreId == drugSt.DrugStoreId);
                                        if (receival != null)
                                        {

                                            receival.Balance = receival.Balance + i.QtyReceived;

                                            db.Entry(receival).State = EntityState.Modified;
                                            db.SaveChanges();


                                        }
                                        else
                                        {
                                            var drugList = db.DrugLists.FirstOrDefault(x => x.DrugListsId == i.DrugListsId);
                                            if (drugList != null)
                                            {
                                                var drugStore = new DrugStoreItem
                                                {
                                                    DrugListsId = i.DrugListsId,
                                                    ItemCode = drugList.ItemCode,
                                                    DrugStoreId = drugSt.DrugStoreId,
                                                    Balance = i.QtyReceived,
                                                    DrugUnitId = i.DrugUnitId,
                                                    IsInnerTransfered = false,
                                                };
                                                db.DrugStoreItems.Add(drugStore);
                                                db.SaveChanges();
                                            }


                                        }

                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", "No such Drug Store");
                                    }

                                    #region update Inventory Items
                                    var drugL = db.DrugLists.FirstOrDefault(x => x.DrugListsId == i.DrugListsId);
                                    if (drugL != null)
                                    {
                                        //var itemCatagory = db.ItemCategorys.FirstOrDefault(x => x.ItemCategoryCode == drugL.ItemCategoryCode );
                                        //if (itemCatagory != null)
                                        //{


                                        var inventoryItem = db.InventoryItems.FirstOrDefault(x => x.ItemCode == drugL.ItemCode);
                                        if (inventoryItem != null)
                                        {


                                            var storeItem = db.StoreItemAssignments.Where(x => x.ItemCode == inventoryItem.ItemCode).FirstOrDefault(x => x.StoreCode == store.StoreCode);

                                            if (storeItem != null)
                                            {
                                                if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                                                {
                                                    storeItem.AvgPrice = ((storeItem.Balance * storeItem.AvgPrice) +
                                                                          (i.QtyReceived * i.UnitCost))
                                                                         / (storeItem.Balance + i.QtyReceived);
                                                }

                                                storeItem.Balance = storeItem.Balance + i.QtyReceived;
                                                db.Entry(storeItem).State = EntityState.Modified;
                                                //db.SaveChanges();

                                                #region bin card line

                                                BinCardLine binCardLine = new BinCardLine
                                                {
                                                    StoreItemAssignment = storeItem,
                                                    ReferenceDoc = "Drug Receive_" + record.DrugReceivalModel19Id.ToString(),
                                                    Reference = "not available",//!String.IsNullOrEmpty(record.ReceiveRef) ? record.ReceiveRef : record.Reference,
                                                    Date = record.ReceivingDate,
                                                    ReceivedQTY = i.QtyReceived,
                                                    ReceivedUPrice = i.UnitCost,
                                                    ReceivedTPrice = i.QtyReceived * i.UnitCost,//i.Total,
                                                    BalanceQTY = storeItem.Balance,
                                                    SupplierName = record.Name,
                                                    //new    
                                                    StoreCode = record.StoreCode,
                                                    GLPeriodId = 0,//record.GLPeriodId,
                                                    TransactionType = BinCardTransactionTypes.Receive,
                                                    StatusType = BinCardTransactionStatus.Regular,
                                                };

                                                #region bin card average price

                                                if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                                {
                                                    binCardLine.BalanceUPrice = inventoryItem.AvgPrice;
                                                    binCardLine.BalanceTPrice = inventoryItem.AvgPrice * storeItem.Balance;
                                                }

                                                else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                                {
                                                    //binCardLine.BalanceUPrice = branchItem.AvgPrice;
                                                    //binCardLine.BalanceTPrice = branchItem.AvgPrice * storeItem.Balance;
                                                }
                                                else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                                                {
                                                    binCardLine.BalanceUPrice = storeItem.AvgPrice;
                                                    binCardLine.BalanceTPrice = storeItem.AvgPrice * storeItem.Balance;
                                                }


                                                #endregion

                                                db.BinCardLines.Add(binCardLine);
                                                // db.SaveChanges();

                                                #endregion
                                            }
                                            else
                                            {
                                                storeItem = new StoreItemAssignment
                                                {
                                                    StoreCode = record.StoreCode,
                                                    ItemCategoryCode = inventoryItem.ItemCategoryCode,
                                                    ItemCode = inventoryItem.ItemCode,
                                                    Balance = i.QtyReceived,
                                                    AvgPrice = i.UnitCost,
                                                };
                                                db.StoreItemAssignments.Add(storeItem);
                                                //db.SaveChanges();

                                                #region bin card line

                                                BinCardLine binCardLine = new BinCardLine
                                                {
                                                    StoreItemAssignment = storeItem,
                                                    ReferenceDoc = "Receive_" + record.DrugReceivalModel19Id.ToString(),
                                                    Reference = "not available",//!String.IsNullOrEmpty(record.ReceiveRef) ? record.ReceiveRef : record.Reference,
                                                    Date = record.ReceivingDate,
                                                    ReceivedQTY = i.QtyReceived,
                                                    ReceivedUPrice = i.UnitCost,
                                                    ReceivedTPrice = i.QtyReceived * i.UnitCost,//i.Total,
                                                    BalanceQTY = storeItem.Balance,
                                                    SupplierName = record.Name,

                                                    //new    
                                                    StoreCode = record.StoreCode,
                                                    GLPeriodId = 0,//record.GLPeriodId,
                                                    TransactionType = BinCardTransactionTypes.Receive,
                                                    StatusType = BinCardTransactionStatus.Regular,

                                                };

                                                #region bin card average price

                                                if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                                {
                                                    binCardLine.BalanceUPrice = inventoryItem.AvgPrice;
                                                    binCardLine.BalanceTPrice = inventoryItem.AvgPrice * storeItem.Balance;
                                                }

                                                else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                                {
                                                    //binCardLine.BalanceUPrice = branchItem.AvgPrice;
                                                    //binCardLine.BalanceTPrice = branchItem.AvgPrice * storeItem.Balance;
                                                }
                                                else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                                                {
                                                    binCardLine.BalanceUPrice = storeItem.AvgPrice;
                                                    binCardLine.BalanceTPrice = storeItem.AvgPrice * storeItem.Balance;
                                                }


                                                #endregion

                                                db.BinCardLines.Add(binCardLine);
                                                // db.SaveChanges();

                                                #endregion
                                            }

                                            inventoryItem.Balance = inventoryItem.Balance + i.QtyReceived;
                                            inventoryItem.AvgPrice = i.UnitCost;
                                            inventoryItem.TotalPrice = inventoryItem.Balance * i.UnitCost;

                                            db.Entry(inventoryItem).State = EntityState.Modified;
                                            db.SaveChanges();


                                        }

                                    }

                                    #endregion

                                }
                                //}


                                //////////////////////


                                #region Commented
                                //var inventoryItem = db.InventoryItems.Find(i.ItemCode);

                                //var branchItem =
                                //    db.BranchItemAssignments.Where(x => x.ItemCode == inventoryItem.ItemCode)
                                //        .FirstOrDefault(x => x.BranchId == store.BranchId);
                                //var storeItem =
                                //    db.StoreItemAssignments.Where(x => x.ItemCode == inventoryItem.ItemCode)
                                //        .FirstOrDefault(x => x.StoreCode == store.StoreCode);
                                //var priceIndex =
                                //    db.ItemPriceIndexes.Where(x => x.SourceType == SourceType.GRN)
                                //        .FirstOrDefault(x => x.ItemCode == i.ItemCode);

                                //if (inventoryItem != null)
                                //{
                                //    if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                //    {
                                //        inventoryItem.AvgPrice = ((inventoryItem.Balance * inventoryItem.AvgPrice) +
                                //                                  (i.Quantity * i.UnitPrice))
                                //                                 / (inventoryItem.Balance + i.Quantity);
                                //    }

                                //    inventoryItem.Balance = inventoryItem.Balance + i.Quantity;
                                //    db.Entry(inventoryItem).State = EntityState.Modified;
                                //    db.SaveChanges();

                                //    if (priceIndex != null)
                                //    {
                                //        priceIndex.PriceBeforeVAT = i.UnitPrice;
                                //        db.Entry(priceIndex).State = EntityState.Modified;
                                //        db.SaveChanges();
                                //    }

                                //    if (priceIndex == null)
                                //    {
                                //        var priceindex = new ItemPriceIndex
                                //        {
                                //            BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(record.StoreCode),
                                //            ItemCategoryCode = inventoryItem.ItemCategoryCode,
                                //            ItemCode = inventoryItem.ItemCode,
                                //            PriceBeforeVAT = i.UnitPrice,
                                //            SourceOfData = "GRN",
                                //            GLPeriodId = record.GLPeriodId,
                                //            DateOfInformation = i.GoodReceive.ReceiveDate,
                                //            SourceType = SourceType.GRN
                                //        };
                                //        db.ItemPriceIndexes.Add(priceindex);
                                //        db.SaveChanges();
                                //    }

                                //    if (branchItem != null)
                                //    {
                                //        if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                //        {
                                //            branchItem.AvgPrice = ((branchItem.Balance * branchItem.AvgPrice) +
                                //                                   (i.Quantity * i.UnitPrice))
                                //                                  / (branchItem.Balance + i.Quantity);
                                //        }

                                //        branchItem.Balance = branchItem.Balance + i.Quantity;
                                //        db.Entry(branchItem).State = EntityState.Modified;
                                //        db.SaveChanges();
                                //    }
                                //    else
                                //    {
                                //        branchItem = new BranchItemAssignment();
                                //        if (store != null) branchItem.BranchId = store.BranchId;
                                //        branchItem.ItemCategoryCode = inventoryItem.ItemCategoryCode;
                                //        branchItem.ItemCode = inventoryItem.ItemCode;
                                //        branchItem.Balance = i.Quantity;
                                //        branchItem.AvgPrice = i.UnitPrice;
                                //        db.BranchItemAssignments.Add(branchItem);
                                //        db.SaveChanges();
                                //    }

                                //    if (storeItem != null)
                                //    {
                                //        if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                                //        {
                                //            storeItem.AvgPrice = ((storeItem.Balance * storeItem.AvgPrice) +
                                //                                  (i.Quantity * i.UnitPrice))
                                //                                 / (storeItem.Balance + i.Quantity);
                                //        }

                                //        storeItem.Balance = storeItem.Balance + i.Quantity;
                                //        db.Entry(storeItem).State = EntityState.Modified;
                                //        db.SaveChanges();

                                //        #region bin card line

                                //        BinCardLine binCardLine = new BinCardLine
                                //        {
                                //            StoreItemAssignment = storeItem,
                                //            ReferenceDoc = "Receive_" + record.GoodReceiveId.ToString(),
                                //            Reference = !String.IsNullOrEmpty(record.ReceiveRef) ? record.ReceiveRef : record.Reference,
                                //            Date = record.ReceiveDate,
                                //            ReceivedQTY = i.Quantity,
                                //            ReceivedUPrice = i.UnitPrice,
                                //            ReceivedTPrice = i.Total,
                                //            BalanceQTY = storeItem.Balance,
                                //            SupplierName = record.SupplierName
                                //        };

                                //        #region bin card average price

                                //        if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                //        {
                                //            binCardLine.BalanceUPrice = inventoryItem.AvgPrice;
                                //            binCardLine.BalanceTPrice = inventoryItem.AvgPrice * storeItem.Balance;
                                //        }

                                //        else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                //        {
                                //            binCardLine.BalanceUPrice = branchItem.AvgPrice;
                                //            binCardLine.BalanceTPrice = branchItem.AvgPrice * storeItem.Balance;
                                //        }
                                //        else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                                //        {
                                //            binCardLine.BalanceUPrice = storeItem.AvgPrice;
                                //            binCardLine.BalanceTPrice = storeItem.AvgPrice * storeItem.Balance;
                                //        }


                                //        #endregion

                                //        db.BinCardLines.Add(binCardLine);
                                //        db.SaveChanges();

                                //        #endregion
                                //    }
                                //    else
                                //    {
                                //        storeItem = new StoreItemAssignment
                                //        {
                                //            StoreCode = record.StoreCode,
                                //            ItemCategoryCode = inventoryItem.ItemCategoryCode,
                                //            ItemCode = inventoryItem.ItemCode,
                                //            Balance = i.Quantity,
                                //            AvgPrice = i.UnitPrice,
                                //        };
                                //        db.StoreItemAssignments.Add(storeItem);
                                //        db.SaveChanges();

                                //        #region bin card line

                                //        BinCardLine binCardLine = new BinCardLine
                                //        {
                                //            StoreItemAssignment = storeItem,
                                //            ReferenceDoc = "Receive_" + record.GoodReceiveId.ToString(),
                                //            Reference = !String.IsNullOrEmpty(record.ReceiveRef) ? record.ReceiveRef : record.Reference,
                                //            Date = record.ReceiveDate,
                                //            ReceivedQTY = i.Quantity,
                                //            ReceivedUPrice = i.UnitPrice,
                                //            ReceivedTPrice = i.Total,
                                //            BalanceQTY = storeItem.Balance,
                                //            SupplierName = record.SupplierName

                                //        };

                                //        #region bin card average price

                                //        if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                //        {
                                //            binCardLine.BalanceUPrice = inventoryItem.AvgPrice;
                                //            binCardLine.BalanceTPrice = inventoryItem.AvgPrice * storeItem.Balance;
                                //        }

                                //        else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                //        {
                                //            binCardLine.BalanceUPrice = branchItem.AvgPrice;
                                //            binCardLine.BalanceTPrice = branchItem.AvgPrice * storeItem.Balance;
                                //        }
                                //        else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                                //        {
                                //            binCardLine.BalanceUPrice = storeItem.AvgPrice;
                                //            binCardLine.BalanceTPrice = storeItem.AvgPrice * storeItem.Balance;
                                //        }


                                //        #endregion

                                //        db.BinCardLines.Add(binCardLine);
                                //        db.SaveChanges();

                                //        #endregion
                                //    }
                                //}

                                #endregion
                                #endregion
                            }
                        }


                    }
                    if (aptsValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";

                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var entity = new APTSValidation
                    {
                        DrugReceivalModel19Id = id,
                        Status = aptsValidation.Status,
                        Remark = aptsValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = aptsValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aptsValidation.ModifiedBy
                    };

                    db.APTSValidations.Add(entity);
                    db.SaveChanges();
                    aptsValidation.APTSValidationId = entity.APTSValidationId;
                }

            }


            return Json(new[] { aptsValidation }.ToDataSourceResult(request, ModelState));
        }



        public JsonResult SendForApproval(int id)
        {
            var record = db.DrugReceivalModel19s.Find(id);
            var items = db.DrugReceivalDetails.Where(x => x.DrugReceivalModel19Id == id).ToList();
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && items.Count > 0)
            {
                record.Status = "Waiting Approval";// Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.DrugReceivalModel19s.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (items.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        #region Void
        //public JsonResult Void(int id)
        //{
        //    var record = db.DrugReceivalModel19s.Find(id);
        //    var items = db.DrugReceivalDetails.Where(x => x.DrugReceivalModel19Id == id).ToList();
        //    if (record != null && record.IsOnlineApproved && !record.IsVoid &&/* !record.IsOnlineTransferred*/  items.Count > 0)
        //    {
        //        bool valid = true;
        //        #region validate

        //        foreach (var i in items)
        //        {
        //            #region stock

        //            var storeItem = db.StoreItemAssignments.Where(x => x.ItemCode == i.ItemCode).FirstOrDefault(x => x.StoreCode == record.StoreCode);
        //            if (storeItem == null)
        //            {
        //                valid = false;
        //                string message = "Item: " + i.InventoryItem.Name + " is not available in store: " + record.StoreCode;
        //                ModelState.AddModelError("store", message);

        //                var formattedData2 = new
        //                {
        //                    Message = message,
        //                    Status = "OK"
        //                };
        //                return Json(formattedData2, JsonRequestBehavior.AllowGet);

        //            }
        //            if (storeItem.Balance - i.Quantity < 0)
        //            {
        //                valid = false;
        //                string message = "There is no balance in system to void for item " + i.ItemCode + "-" + i.InventoryItem.Name;
        //                var formattedData2 = new
        //                {
        //                    Message = message,
        //                    Status = "OK"
        //                };
        //                return Json(formattedData2, JsonRequestBehavior.AllowGet);
        //            }

        //            #endregion

        //        }
        //        #endregion

        //        if (valid)
        //        {
        //            record.Status = Enum.GetName(typeof(OperationStatuses), 10);
        //            record.IsVoid = true;
        //            record.VoidBy = User.Identity.Name;
        //            record.VoidTime = DateTime.Now;
        //            db.GoodReceives.Attach(record);
        //            db.Entry(record).State = EntityState.Modified;
        //            db.SaveChanges();

        //            #region update stock

        //            string storecode = record.StoreCode;

        //            foreach (GoodReceiveItem i in record.GoodReceiveItem)
        //            {
        //                string itemCode = i.ItemCode;

        //                InventoryItem stockItem = db.InventoryItems.Find(itemCode);
        //                int storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storecode);

        //                #region stock

        //                var branchItem =
        //                    db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId)
        //                        .FirstOrDefault(x => x.ItemCode == i.ItemCode);
        //                var storeItem =
        //                    db.StoreItemAssignments.FirstOrDefault(
        //                        x => x.StoreCode == storecode && x.ItemCode == itemCode);

        //                if (storeItem != null)
        //                {
        //                    storeItem.Balance = storeItem.Balance - i.Quantity;
        //                    db.Entry(storeItem).State = EntityState.Modified;
        //                    db.SaveChanges();

        //                    #region update bin card

        //                    #region bin card line

        //                    BinCardLine binCardLine = new BinCardLine
        //                    {
        //                        StoreItemAssignment = storeItem,
        //                        ReferenceDoc = "Void Receive " + record.GoodReceiveId,
        //                        Date = record.ReceiveDate,
        //                        IssuedQTY = i.Quantity,
        //                        IssuedUPrice = i.UnitPrice,
        //                        IssuedTPrice = i.Total,
        //                        BalanceQTY = storeItem.Balance,
        //                        BalanceUPrice = i.UnitPrice,
        //                        BalanceTPrice = i.UnitPrice * storeItem.Balance
        //                    };
        //                    db.BinCardLines.Add(binCardLine);
        //                    db.SaveChanges();

        //                    #endregion

        //                    #endregion

        //                }
        //                if (branchItem != null)
        //                {
        //                    branchItem.Balance = branchItem.Balance - i.Quantity;
        //                    db.Entry(branchItem).State = EntityState.Modified;
        //                    db.SaveChanges();

        //                }
        //                if (stockItem != null)
        //                {
        //                    stockItem.Balance = stockItem.Balance - i.Quantity;
        //                    db.Entry(stockItem).State = EntityState.Modified;
        //                    db.SaveChanges();
        //                }



        //                #endregion

        //            }


        //            #endregion

        //            //copy record
        //            CopyRecord(id);

        //            var formattedData = new
        //            {
        //                Message = "Void Successful.",
        //                Status = "OK"
        //            };
        //            return Json(formattedData, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    if (record == null)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "The record is not found.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }

        //    if (record.GoodReceiveItem.Count == 0)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "Please add line items first.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }
        //    if (record.IsVoid)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "The record is already void.You can not void again.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }
        //    if (record.IsOnlineTransferred)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "The record is transferred to GL.You can not void .",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }
        //    if (!record.IsOnlineApproved)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "The record is not approved hence you can modify by editing.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }


        //    return null;
        //}
        #endregion
    }
}