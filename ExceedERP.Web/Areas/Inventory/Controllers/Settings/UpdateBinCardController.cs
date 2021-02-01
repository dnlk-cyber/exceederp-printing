using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Settings
{
    enum StoreTransactionType
    {
        Grn,
        Issue,
        Return,
        Send,
        Receive
    }
    class TransactionViewModel
    {
        public int Id { get; set; }
        public StoreTransactionType Type { get; set; }
        public DateTime ApproveTime { get; set; }
    }
     [Authorize(Roles = "Admin")]
    public class UpdateBinCardController : Controller
    {
        // GET: Inventory/UpdateBinCard
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateCategories();
            return View();
        }

        private void PopulateCategories()
        {

            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var stores = db.InventoryStores;
            ViewData["Stores"] = stores;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();
        }
        public ActionResult StoreItemAssignments_Read([DataSourceRequest]DataSourceRequest request, string storecode)
        {
            IQueryable<StoreItemAssignment> storeItemassignments = db.StoreItemAssignments.Where(x => x.StoreCode == storecode);
            DataSourceResult result = storeItemassignments.ToDataSourceResult(request, storeItemAssignment => new
            {
                StoreItemAssignmentID = storeItemAssignment.StoreItemAssignmentID,
                StoreCode = storeItemAssignment.StoreCode,
                ItemCategoryCode = storeItemAssignment.ItemCategoryCode,
                ItemCode = storeItemAssignment.ItemCode,
                ReOrderPoint = storeItemAssignment.ReOrderPoint,
                MinPoint = storeItemAssignment.MinPoint,
                MaxPoint = storeItemAssignment.MaxPoint,
                Balance = storeItemAssignment.Balance,
                AvgPrice = storeItemAssignment.AvgPrice,
                ItemName = storeItemAssignment.InventoryItem.Name
            });

            return Json(result);
        }

        public ActionResult RunUpdate(int id)
        {
            List<TransactionViewModel> mList = new List<TransactionViewModel>();
            var storeitem = db.StoreItemAssignments.Find(id);
            var pricesetting = db.AveragePriceSettings.FirstOrDefault();
           

            if (storeitem != null)
            {
                UserHelper.OperationLog(Request.UserHostAddress, "Update Bin Card", "Success", User.Identity.Name,
                      "Updated item: " + storeitem.ItemCode + " in store : " + storeitem.StoreCode,Core.Domain.Global.Modules.Inventory);


                var store = db.InventoryStores.Find(storeitem.StoreCode);
                var inventoryItem = db.InventoryItems.Find(storeitem.ItemCode);
                var bincard = db.BinCardLines.Where(x => x.StoreItemAssignmentID == id).ToList();
                db.BinCardLines.RemoveRange(bincard);
                db.SaveChanges();
                #region beginning
                BinCardLine begbinCardLine = new BinCardLine
                {
                    StoreItemAssignment = storeitem,
                    ReferenceDoc = "Beginning",
                    Date = new DateTime(2017, 4, 30),
                    BalanceQTY = storeitem.Balance,
                    BalanceUPrice = storeitem.AvgPrice
                };
                begbinCardLine.BalanceTPrice = storeitem.AvgPrice * begbinCardLine.BalanceQTY;
                db.BinCardLines.Add(begbinCardLine);
                db.SaveChanges();
                #endregion
                #region receive
                var goodsreceives = db.GoodReceiveItems.Where(x => x.GoodReceive.IsOnlineApproved
                    && x.GoodReceive.StoreCode == storeitem.StoreCode
                    && x.ItemCode == storeitem.ItemCode).ToList();

                foreach (var i in goodsreceives)
                {
                    if (i.Quantity > 0)
                    {
                        #region update stock avg price and total


                        var branchItem = db.BranchItemAssignments.Where(x => x.ItemCode == inventoryItem.ItemCode).FirstOrDefault(x => x.BranchId == store.BranchId);
                        var storeItem = db.StoreItemAssignments.Where(x => x.ItemCode == inventoryItem.ItemCode).FirstOrDefault(x => x.StoreCode == store.StoreCode);


                        if (inventoryItem != null)
                        {
                            inventoryItem.AvgPrice = ((inventoryItem.Balance * inventoryItem.AvgPrice) +
                                                      (i.Quantity * i.UnitPrice))
                                                     / (inventoryItem.Balance + i.Quantity);
                            inventoryItem.Balance = inventoryItem.Balance + i.Quantity;
                            db.Entry(inventoryItem).State = EntityState.Modified;
                            db.SaveChanges();

                            if (branchItem != null)
                            {
                                branchItem.AvgPrice = ((branchItem.Balance * branchItem.AvgPrice) +
                                                       (i.Quantity * i.UnitPrice))
                                                      / (branchItem.Balance + i.Quantity);
                                branchItem.Balance = branchItem.Balance + i.Quantity;
                                db.Entry(branchItem).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                branchItem = new BranchItemAssignment();
                                if (store != null) branchItem.BranchId = store.BranchId;
                                branchItem.ItemCategoryCode = inventoryItem.ItemCategoryCode;
                                branchItem.ItemCode = inventoryItem.ItemCode;
                                branchItem.Balance = i.Quantity;
                                branchItem.AvgPrice = i.UnitPrice;
                                db.BranchItemAssignments.Add(branchItem);
                                db.SaveChanges();
                            }

                            if (storeItem != null)
                            {
                                storeItem.AvgPrice = ((storeItem.Balance * storeItem.AvgPrice) +
                                                      (i.Quantity * i.UnitPrice))
                                                     / (storeItem.Balance + i.Quantity);
                                storeItem.Balance = storeItem.Balance + i.Quantity;
                                db.Entry(storeItem).State = EntityState.Modified;
                                db.SaveChanges();

                                #region bin card line

                                BinCardLine binCardLine = new BinCardLine
                                {
                                    StoreItemAssignment = storeItem,
                                    ReferenceDoc = "Receive_" + i.GoodReceive.GoodReceiveId.ToString(),
                                    Reference = i.GoodReceive.ReceiveRef,
                                    Date = i.GoodReceive.ReceiveDate,
                                    ReceivedQTY = i.Quantity,
                                    ReceivedUPrice = i.UnitPrice,
                                    ReceivedTPrice = i.Total,
                                    BalanceQTY = storeItem.Balance,
                                };

                                #region bin card average price

                                if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                {
                                    binCardLine.BalanceUPrice = inventoryItem.AvgPrice;
                                    binCardLine.BalanceTPrice = inventoryItem.AvgPrice * storeItem.Balance;
                                }

                                else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                {
                                    binCardLine.BalanceUPrice = branchItem.AvgPrice;
                                    binCardLine.BalanceTPrice = branchItem.AvgPrice * storeItem.Balance;
                                }
                                else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                                {
                                    binCardLine.BalanceUPrice = storeItem.AvgPrice;
                                    binCardLine.BalanceTPrice = storeItem.AvgPrice * storeItem.Balance;
                                }


                                #endregion

                                db.BinCardLines.Add(binCardLine);
                                db.SaveChanges();

                                #endregion
                            }
                            else
                            {
                                storeItem = new StoreItemAssignment
                                {
                                    StoreCode = i.GoodReceive.StoreCode,
                                    ItemCategoryCode = inventoryItem.ItemCategoryCode,
                                    ItemCode = inventoryItem.ItemCode,
                                    Balance = i.Quantity,
                                    AvgPrice = i.UnitPrice,
                                };
                                db.StoreItemAssignments.Add(storeItem);
                                db.SaveChanges();

                                #region bin card line

                                BinCardLine binCardLine = new BinCardLine
                                {
                                    StoreItemAssignment = storeItem,
                                    ReferenceDoc = "Receive_" + i.GoodReceive.GoodReceiveId.ToString(),
                                    Reference = i.GoodReceive.ReceiveRef,
                                    Date = i.GoodReceive.ReceiveDate,
                                    ReceivedQTY = i.Quantity,
                                    ReceivedUPrice = i.UnitPrice,
                                    ReceivedTPrice = i.Total,
                                    BalanceQTY = storeItem.Balance,
                                };

                                #region bin card average price

                                if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                {
                                    binCardLine.BalanceUPrice = inventoryItem.AvgPrice;
                                    binCardLine.BalanceTPrice = inventoryItem.AvgPrice * storeItem.Balance;
                                }

                                else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                {
                                    binCardLine.BalanceUPrice = branchItem.AvgPrice;
                                    binCardLine.BalanceTPrice = branchItem.AvgPrice * storeItem.Balance;
                                }
                                else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                                {
                                    binCardLine.BalanceUPrice = storeItem.AvgPrice;
                                    binCardLine.BalanceTPrice = storeItem.AvgPrice * storeItem.Balance;
                                }


                                #endregion

                                db.BinCardLines.Add(binCardLine);
                                db.SaveChanges();

                                #endregion
                            }
                        }

                        #endregion

                        if (i.GoodReceive.IsVoid)
                        {
                            #region update stock
                            if (storeItem != null)
                            {
                                storeItem.Balance = storeItem.Balance - i.Quantity;
                                db.Entry(storeItem).State = EntityState.Modified;
                                db.SaveChanges();
                                #region update bin card

                                #region bin card line

                                BinCardLine binCardLine = new BinCardLine();
                                binCardLine.StoreItemAssignment = storeItem;
                                binCardLine.ReferenceDoc = "Void Receive " + i.GoodReceive.GoodReceiveId;
                                binCardLine.Date = i.GoodReceive.ReceiveDate;
                                binCardLine.IssuedQTY = i.Quantity;
                                binCardLine.IssuedUPrice = i.UnitPrice;
                                binCardLine.IssuedTPrice = i.Total;
                                binCardLine.BalanceQTY = storeItem.Balance;
                                binCardLine.BalanceUPrice = i.UnitPrice;
                                binCardLine.BalanceTPrice = i.UnitPrice * storeItem.Balance;
                                db.BinCardLines.Add(binCardLine);
                                db.SaveChanges();

                                #endregion

                                #endregion

                            }
                            if (branchItem != null)
                            {
                                branchItem.Balance = branchItem.Balance - i.Quantity;
                                db.Entry(branchItem).State = EntityState.Modified;
                                db.SaveChanges();

                            }
                            if (inventoryItem != null)
                            {
                                inventoryItem.Balance = inventoryItem.Balance - i.Quantity;
                                db.Entry(inventoryItem).State = EntityState.Modified;
                                db.SaveChanges();
                            }



                            #endregion
                        }
                    }


                }
                #endregion
                #region transfer receive
                var transferItems = db.ItemTransferItems.Where(x => x.ItemTransfer.IsOnlineApproved
                    && x.ItemTransfer.ToStore == storeitem.StoreCode
                    && x.ItemCode == storeitem.ItemCode).ToList();

                foreach (var i in transferItems)
                {
                    if (i.ReceivedQuantity > 0)
                    {
                        var frombranch = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(i.ItemTransfer.FromStore);
                        var tobranch = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(i.ItemTransfer.ToStore);

                        if (pricesetting != null)
                        {
                            var avgpricesetting = pricesetting.Level;

                            #region update to branch item

                            var branchItemAssignment =
                                db.BranchItemAssignments.Where(si => si.BranchId == tobranch)
                                    .FirstOrDefault(si => si.ItemCode == i.ItemCode);

                            if (branchItemAssignment == null)
                            {
                                BranchItemAssignment newItemAssignment = new BranchItemAssignment
                                {
                                    BranchId = tobranch,
                                    ItemCode = i.ItemCode,
                                    ItemCategoryCode = i.ItemCategoryCode,
                                    Balance = i.ReceivedQuantity,
                                    AvgPrice = i.UPrice,
                                };
                                db.BranchItemAssignments.Add(newItemAssignment);
                                db.SaveChanges();

                            }
                            else
                            {
                                if (frombranch != tobranch && avgpricesetting == AveragePriceLvel.PerBranch)
                                {
                                    branchItemAssignment.AvgPrice = ((branchItemAssignment.Balance *
                                                                      branchItemAssignment.AvgPrice) +
                                                                     i.ReceivedQuantity *
                                                                     i.UPrice)
                                                                    /
                                                                    (branchItemAssignment.Balance +
                                                                     i.ReceivedQuantity);
                                }
                                branchItemAssignment.Balance = branchItemAssignment.Balance +
                                                               i.ReceivedQuantity;
                                db.BranchItemAssignments.Attach(branchItemAssignment);
                                db.Entry(branchItemAssignment).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            #endregion

                            #region update to store

                            var iteminStore = storeitem;
                            if (i.ItemTransfer.FromStore != i.ItemTransfer.ToStore &&
                                avgpricesetting == AveragePriceLvel.PerStore)
                            {
                                iteminStore.AvgPrice = ((iteminStore.Balance * iteminStore.AvgPrice) +
                                                        i.ReceivedQuantity *
                                                        i.UPrice)
                                                       /
                                                       (iteminStore.Balance +
                                                        i.ReceivedQuantity);
                            }
                            iteminStore.Balance = iteminStore.Balance + i.ReceivedQuantity;
                            db.StoreItemAssignments.Attach(iteminStore);
                            db.Entry(iteminStore).State = EntityState.Modified;
                            db.SaveChanges();

                            #region bin card line

                            BinCardLine binCardLine = new BinCardLine
                            {
                                StoreItemAssignment = iteminStore,
                                ReferenceDoc = "Transfer Receive_" + i.ItemTransfer.ItemTransferID.ToString(),
                                Reference = i.ItemTransfer.Remark,
                                Date = i.ItemTransfer.TransferDate,
                                ReceivedQTY = i.ReceivedQuantity,
                                ReceivedUPrice = i.UPrice,
                                ReceivedTPrice = i.TPrice,
                                BalanceQTY = iteminStore.Balance,
                                BalanceUPrice = iteminStore.AvgPrice,
                                BalanceTPrice = iteminStore.AvgPrice * iteminStore.Balance
                            };
                            db.BinCardLines.Add(binCardLine);
                            db.SaveChanges();

                            #endregion

                            #endregion

                            if (i.ItemTransfer.IsVoid && i.ItemTransfer.Status == "ReceiverVoid")
                            {
                                int storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(i.ItemTransfer.ToStore);
                                #region stock

                                BranchItemAssignment branchItem =
                                    db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId)
                                        .FirstOrDefault(x => x.ItemCode == i.ItemCode);


                                storeitem.Balance = storeitem.Balance - i.ReceivedQuantity;
                                db.Entry(storeitem).State = EntityState.Modified;
                                db.SaveChanges();

                                #region update bin card

                                #region bin card line

                                BinCardLine transferReceivebinCardLine = new BinCardLine();
                                transferReceivebinCardLine.StoreItemAssignment = storeitem;
                                transferReceivebinCardLine.ReferenceDoc = "Void Transfer Receive " + i.ItemTransfer.ItemTransferID;
                                transferReceivebinCardLine.Date = i.ItemTransfer.TransferDate;
                                transferReceivebinCardLine.IssuedQTY = i.ReceivedQuantity;
                                transferReceivebinCardLine.IssuedUPrice = i.UPrice;
                                transferReceivebinCardLine.IssuedTPrice = i.TPrice;
                                transferReceivebinCardLine.BalanceQTY = storeitem.Balance;
                                transferReceivebinCardLine.BalanceUPrice = i.UPrice;
                                transferReceivebinCardLine.BalanceTPrice = i.UPrice * storeitem.Balance;
                                db.BinCardLines.Add(transferReceivebinCardLine);
                                db.SaveChanges();

                                #endregion

                                #endregion
                                if (branchItem != null)
                                {
                                    branchItem.Balance = branchItem.Balance - i.ShippedQuantity;
                                    db.Entry(branchItem).State = EntityState.Modified;
                                    db.SaveChanges();

                                }
                                if (inventoryItem != null)
                                {
                                    inventoryItem.Balance = inventoryItem.Balance - i.ShippedQuantity;
                                    db.Entry(inventoryItem).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                                #endregion
                            }
                        }
                    }


                }
                #endregion
                #region return
                var returnItems = db.StoreReturnItemss.Where(x => x.StoreReturn.IsOnlineApproved
                    && x.StoreReturn.StoreCode == storeitem.StoreCode
                    && x.ItemCode == storeitem.ItemCode).ToList();

                foreach (var i in returnItems)
                {
                    if (i.Quantity > 0)
                    {
                        decimal avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(i.ItemCode, storeitem.StoreCode);
                        if (avgPrice == 0)
                            avgPrice = i.UnitPrice;

                        #region Store Item Balance
                        storeitem.Balance = storeitem.Balance + i.Quantity;
                        db.StoreItemAssignments.Attach(storeitem);
                        db.Entry(storeitem).State = EntityState.Modified;
                        db.SaveChanges();

                        #region bin card line

                        BinCardLine returnbinCardLine = new BinCardLine
                        {
                            StoreItemAssignment = storeitem,
                            ReferenceDoc = "Return_" + i.StoreReturn.StoreReturnID.ToString(),
                            Reference = i.StoreReturn.Remark,
                            Date = i.StoreReturn.ReturnDate,
                            ReceivedQTY = i.Quantity,
                            ReceivedUPrice = avgPrice,
                            ReceivedTPrice = avgPrice * i.Quantity,
                            BalanceQTY = storeitem.Balance,
                            BalanceUPrice = avgPrice,
                            BalanceTPrice = avgPrice * storeitem.Balance
                        };
                        //price if manual
                        db.BinCardLines.Add(returnbinCardLine);
                        db.SaveChanges();

                        #endregion

                        #endregion

                        #region  branch Balance

                        var storeBranch = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(i.StoreReturn.StoreCode);
                        var branchItemAssignment = db.BranchItemAssignments.Where(x => x.BranchId == storeBranch)
                            .FirstOrDefault(x => x.ItemCode == i.ItemCode);
                        if (branchItemAssignment != null)
                        {
                            branchItemAssignment.Balance = branchItemAssignment.Balance + i.Quantity;
                            db.Entry(branchItemAssignment).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            var branchItem = new BranchItemAssignment();
                            if (store != null) branchItem.BranchId = store.BranchId;
                            branchItem.ItemCategoryCode = i.ItemCategoryCode;
                            branchItem.ItemCode = i.ItemCode;
                            branchItem.Balance = i.Quantity;
                            branchItem.AvgPrice = i.UnitPrice;
                            db.BranchItemAssignments.Add(branchItem);
                            db.SaveChanges();
                        }

                        #endregion

                        #region  Item Balance
                        if (inventoryItem != null)
                        {
                            inventoryItem.Balance += i.Quantity;
                            db.Entry(inventoryItem).State = EntityState.Modified;
                        }

                        #endregion

                        if (i.StoreReturn.IsVoid)
                        {
                            #region Store Item Balance

                            storeitem.Balance = storeitem.Balance - i.Quantity;
                            db.StoreItemAssignments.Attach(storeitem);
                            db.Entry(storeitem).State = EntityState.Modified;
                            db.SaveChanges();

                            #region bin card line

                            BinCardLine binCardLine = new BinCardLine
                            {
                                StoreItemAssignment = storeitem,
                                ReferenceDoc = "Void Return_" + i.StoreReturn.StoreReturnID,
                                Date = i.StoreReturn.ReturnDate,
                                IssuedQTY = i.Quantity,
                                IssuedUPrice = i.UnitPrice,
                                IssuedTPrice = i.UnitPrice * i.Quantity,
                                BalanceQTY = storeitem.Balance,
                                BalanceUPrice = i.UnitPrice,
                                BalanceTPrice = i.UnitPrice * storeitem.Balance,
                            };

                            db.BinCardLines.Add(binCardLine);
                            db.SaveChanges();

                            #endregion


                            #endregion

                            #region  branch Balance

                            storeBranch = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(i.StoreReturn.StoreCode);
                            branchItemAssignment = db.BranchItemAssignments.Where(x => x.BranchId == storeBranch).FirstOrDefault(x => x.ItemCode == i.ItemCode);
                            if (branchItemAssignment != null)
                            {
                                branchItemAssignment.Balance = branchItemAssignment.Balance - i.Quantity;
                                db.Entry(branchItemAssignment).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            #endregion

                            #region  Item Balance
                            if (inventoryItem != null)
                            {
                                inventoryItem.Balance = inventoryItem.Balance - i.Quantity;
                                db.Entry(inventoryItem).State = EntityState.Modified;
                            }

                            #endregion
                        }
                    }


                }
                #endregion
                #region issue
                var issueItems = db.IssueItems.Where(x => x.Issue.IsOnlineApproved
                    && x.Issue.StoreCode == storeitem.StoreCode
                    && x.ItemCode == storeitem.ItemCode).ToList();

                foreach (var i in issueItems)
                {
                    if (i.IssuedQuantity > 0)
                    {
                        int storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storeitem.StoreCode);

                        #region stock

                        BranchItemAssignment branchItem = db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId).FirstOrDefault(x => x.ItemCode == i.ItemCode);

                        if (branchItem != null)
                        {
                            if (i.IssuedQuantity > storeitem.Balance)
                            {

                                storeitem.Balance = 0;
                                branchItem.Balance = branchItem.Balance - i.IssuedQuantity;
                                inventoryItem.Balance = inventoryItem.Balance - i.IssuedQuantity;
                            }
                            else if (i.IssuedQuantity <= storeitem.Balance)
                            {
                                storeitem.Balance = storeitem.Balance - i.IssuedQuantity;
                                branchItem.Balance = branchItem.Balance - i.IssuedQuantity;
                                inventoryItem.Balance = inventoryItem.Balance - i.IssuedQuantity;
                            }


                            db.Entry(storeitem).State = EntityState.Modified;
                            db.SaveChanges();

                            db.Entry(branchItem).State = EntityState.Modified;
                            db.SaveChanges();

                            db.Entry(inventoryItem).State = EntityState.Modified;
                            db.SaveChanges();

                            #region update bin card

                            #region bin card line

                            BinCardLine binCardLine = new BinCardLine();
                            binCardLine.StoreItemAssignment = storeitem;
                            binCardLine.ReferenceDoc = "Issue " + i.Issue.IssueID.ToString();
                            binCardLine.Reference = i.Issue.Remark;
                            binCardLine.Date = i.Issue.IssueDate;
                            binCardLine.IssuedQTY = i.IssuedQuantity;
                            binCardLine.IssuedUPrice = i.UnitPrice;
                            binCardLine.IssuedTPrice = i.Total;
                            binCardLine.BalanceQTY = storeitem.Balance;
                            binCardLine.BalanceUPrice = i.UnitPrice;
                            binCardLine.BalanceTPrice = i.UnitPrice * storeitem.Balance;
                            db.BinCardLines.Add(binCardLine);
                            db.SaveChanges();

                            #endregion

                            #endregion


                        }

                        #endregion

                        if (i.Issue.IsVoid)
                        {

                            storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storeitem.StoreCode);
                            #region stock
                            branchItem = db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId).FirstOrDefault(x => x.ItemCode == i.ItemCode);

                            storeitem.Balance = storeitem.Balance + i.IssuedQuantity;
                            db.Entry(storeitem).State = EntityState.Modified;
                            db.SaveChanges();
                            #region update bin card

                            #region bin card line
                            BinCardLine binCardLine = new BinCardLine();
                            binCardLine.StoreItemAssignment = storeitem;
                            binCardLine.ReferenceDoc = "Void Issue " + i.Issue.IssueID;
                            binCardLine.Date = i.Issue.IssueDate;
                            binCardLine.ReceivedQTY = i.IssuedQuantity;
                            binCardLine.ReceivedUPrice = i.UnitPrice;
                            binCardLine.ReceivedTPrice = i.Total;
                            binCardLine.BalanceQTY = storeitem.Balance;
                            binCardLine.BalanceUPrice = i.UnitPrice;
                            binCardLine.BalanceTPrice = i.UnitPrice * storeitem.Balance;
                            db.BinCardLines.Add(binCardLine);
                            db.SaveChanges();

                            #endregion

                            #endregion
                            if (branchItem != null)
                            {
                                branchItem.Balance = branchItem.Balance + i.IssuedQuantity;
                                db.Entry(branchItem).State = EntityState.Modified;
                                db.SaveChanges();

                            }
                            if (inventoryItem != null)
                            {
                                inventoryItem.Balance = inventoryItem.Balance + i.IssuedQuantity;
                                db.Entry(inventoryItem).State = EntityState.Modified;
                                db.SaveChanges();
                            }



                            #endregion
                        }
                    }


                }
                #endregion
                #region transfer send
                var itemTransferItems = db.ItemTransferItems.Where(x => x.ItemTransfer.IsOnlineApproved
                    && x.ItemTransfer.FromStore == storeitem.StoreCode
                    && x.ItemCode == storeitem.ItemCode).ToList();

                foreach (var i in itemTransferItems)
                {
                    if (i.ShippedQuantity > 0)
                    {
                        storeitem.Balance = storeitem.Balance - i.ShippedQuantity;
                        db.StoreItemAssignments.Attach(storeitem);
                        db.Entry(storeitem).State = EntityState.Modified;
                        db.SaveChanges();

                        #region update bin card

                        #region bin card line

                        BinCardLine binCardLine = new BinCardLine
                        {
                            StoreItemAssignment = storeitem,
                            ReferenceDoc = "Transfer Send " + i.ItemTransfer.ItemTransferID,
                            Reference = i.ItemTransfer.DocumentRef,
                            Date = i.ItemTransfer.TransferDate,
                            IssuedQTY = i.ShippedQuantity,
                            IssuedUPrice = i.UPrice,
                            IssuedTPrice = i.TPrice,
                            BalanceQTY = storeitem.Balance,
                            BalanceUPrice = i.UPrice,
                            BalanceTPrice = i.UPrice * storeitem.Balance
                        };

                        db.BinCardLines.Add(binCardLine);
                        db.SaveChanges();

                        #endregion

                        #endregion

                        if (i.ItemTransfer.IsVoid && i.ItemTransfer.Status == "SenderVoid")
                        {
                            int storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(i.ItemTransfer.FromStore);
                            #region stock
                            BranchItemAssignment branchItem = db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId).FirstOrDefault(x => x.ItemCode == i.ItemCode);

                            storeitem.Balance = storeitem.Balance + i.ShippedQuantity;
                            db.Entry(storeitem).State = EntityState.Modified;
                            db.SaveChanges();
                            #region update bin card

                            #region bin card line
                            BinCardLine sendbinCardLine = new BinCardLine();
                            sendbinCardLine.StoreItemAssignment = storeitem;
                            sendbinCardLine.ReferenceDoc = "Void Transfer Send " + i.ItemTransfer.ItemTransferID;
                            sendbinCardLine.Date = i.ItemTransfer.TransferDate;
                            sendbinCardLine.ReceivedQTY = i.ShippedQuantity;
                            sendbinCardLine.ReceivedUPrice = i.UPrice;
                            sendbinCardLine.ReceivedTPrice = i.TPrice;
                            sendbinCardLine.BalanceQTY = storeitem.Balance;
                            sendbinCardLine.BalanceUPrice = i.UPrice;
                            sendbinCardLine.BalanceTPrice = i.UPrice * storeitem.Balance;
                            db.BinCardLines.Add(sendbinCardLine);
                            db.SaveChanges();

                            #endregion

                            #endregion
                            if (branchItem != null)
                            {
                                branchItem.Balance = branchItem.Balance + i.ShippedQuantity;
                                db.Entry(branchItem).State = EntityState.Modified;
                                db.SaveChanges();

                            }
                            if (inventoryItem != null)
                            {
                                inventoryItem.Balance = inventoryItem.Balance + i.ShippedQuantity;
                                db.Entry(inventoryItem).State = EntityState.Modified;
                                db.SaveChanges();
                            }



                            #endregion
                        }
                    }

                }
                #endregion
            }




            return View("Success");
        }
    }
}