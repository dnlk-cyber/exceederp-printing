using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Inventory;

namespace ExceedERP.Web.Controllers.Cmd
{
    public partial class CmdController : Controller
    {
        
        // GET: CMD

        public ActionResult BinBalance(int start,int end)
        {
            var newBincardLines =db.BinCardLines
                     .Where(x => x.BinCardLineID <= start)
                     .Where(x => x.BinCardLineID >= end)
                    .ToList();
            List<int> items = newBincardLines.Select(x => x.StoreItemAssignmentID).Distinct().ToList();

            return View("Success");
        }
        public ActionResult ReturnBin(string store)
        {
            var records = db.StoreReturns.Where(x => x.IsOnlineApproved).AsQueryable();
            if(!String.IsNullOrEmpty(store))
                records= records.Where(x=>x.StoreCode==store).AsQueryable();
            foreach (var record in records)
            {
                string reff = "Return_" + record.StoreReturnID;
                foreach (var item in record.StoreReturnItems)
                {
                    #region bincard record

                    var storeItem = db.StoreItemAssignments.Where(x => x.StoreCode == record.StoreCode)
                        .FirstOrDefault(it => it.ItemCode == item.ItemCode);
                    if (storeItem != null)
                    {

                        var exist =
                            db.BinCardLines.Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID)
                                .Where(x => x.ReferenceDoc == reff)
                                .ToList();
                        if (!exist.Any())
                        {
                            #region bin card line

                            BinCardLine binCardLine = new BinCardLine
                            {
                                StoreItemAssignment = storeItem,
                                ReferenceDoc = reff,
                                Date = record.ReturnDate,
                                ReceivedQTY = item.Quantity,
                                ReceivedUPrice = item.UnitPrice,
                                ReceivedTPrice = item.UnitPrice * item.Quantity,
                                StoreCode = record.StoreCode,
                                GLPeriodId = record.GLPeriodId,
                                TransactionType = BinCardTransactionTypes.Return,
                                StatusType = BinCardTransactionStatus.Regular,
                            };

                            db.BinCardLines.Add(binCardLine);
                            db.SaveChanges();

                            #endregion
                        }
                    }


                    #endregion
                }

                if (record.IsVoid)
                {
                    string reffVoid = "Void Return_" + record.StoreReturnID;

                    #region void

                    foreach (var i in record.StoreReturnItems)
                    {
                        #region void bin card record

                        StoreItemAssignment storeItem =
                            db.StoreItemAssignments.FirstOrDefault(
                                x => x.StoreCode == record.StoreCode && x.ItemCode == i.ItemCode);
                        if (storeItem != null)
                        {
                            var exist1 = db.BinCardLines.Where(x => x.ReferenceDoc == reffVoid)
                                .Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID).ToList();
                            if (!exist1.Any())
                            {
                                #region bin card line

                                BinCardLine binCardLine = new BinCardLine
                                {
                                    StoreItemAssignment = storeItem,
                                    ReferenceDoc = "Void Return_" + record.StoreReturnID,
                                    Date = record.ReturnDate,
                                    IssuedQTY = i.Quantity,
                                    IssuedUPrice = i.UnitPrice,
                                    IssuedTPrice = i.UnitPrice * i.Quantity,
                                    StoreCode = record.StoreCode,
                                    GLPeriodId = record.GLPeriodId,
                                    TransactionType = BinCardTransactionTypes.Return,
                                    StatusType = BinCardTransactionStatus.Void,
                                };

                                db.BinCardLines.Add(binCardLine);
                                db.SaveChanges();

                                #endregion
                            }
                        }


                        #endregion
                    }

                    #endregion

                }
            }

            return View("Success");
        }
        public ActionResult TransferSend(string store)
        {
            var records = db.ItemTransfers.Where(x => x.IsOnlineApproved).AsQueryable();
            if (!String.IsNullOrEmpty(store))
                records = records.Where(x => x.FromStore == store).AsQueryable();
            foreach (var record in records)
            {
                string reff = "Transfer Send " + record.ItemTransferID;
                foreach (var item in record.ItemTransferItem)
                {
                    #region record bincard

                    var storeItem = db.StoreItemAssignments.Where(x => x.StoreCode == record.FromStore)
                        .FirstOrDefault(it => it.ItemCode == item.ItemCode);
                    if (storeItem != null)
                    {

                        var exist =
                            db.BinCardLines.Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID)
                                .Where(x => x.ReferenceDoc == reff)
                                .ToList();
                        if (!exist.Any())
                        {
                            #region bin card line

                            BinCardLine binCardLine = new BinCardLine
                            {
                                StoreItemAssignment = storeItem,
                                ReferenceDoc = reff,
                                Date = record.TransferDate,
                                IssuedQTY = item.ShippedQuantity,
                                IssuedUPrice = item.UPrice,
                                IssuedTPrice = item.TPrice,
                                StoreCode = record.ToStore,
                                GLPeriodId = record.GLPeriodId,
                                TransactionType = BinCardTransactionTypes.TransferSend,
                                StatusType = BinCardTransactionStatus.Regular,
                            };

                            db.BinCardLines.Add(binCardLine);
                            db.SaveChanges();

                            #endregion
                        }
                    }


                    #endregion
                }

                if (record.IsVoid)
                {
                    string reffVoid = "Void Transfer Send " + record.ItemTransferID;
                    #region update record void bincard

                    foreach (ItemTransferItem i in record.ItemTransferItem)
                    {
                        string itemCode = i.ItemCode;
                        #region stock

                        StoreItemAssignment storeItem = db.StoreItemAssignments.FirstOrDefault(x => x.StoreCode == record.FromStore && x.ItemCode == itemCode);

                        if (storeItem != null)
                        {
                            var exist1 = db.BinCardLines.Where(x => x.ReferenceDoc == reffVoid)
                                .Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID).ToList();
                            if (!exist1.Any())
                            {
                                #region update bin card

                                #region bin card line

                                BinCardLine binCardLine = new BinCardLine
                                {
                                    StoreItemAssignment = storeItem,
                                    ReferenceDoc = "Void Transfer Send " + record.ItemTransferID,
                                    Date = record.TransferDate,
                                    ReceivedQTY = i.ShippedQuantity,
                                    ReceivedUPrice = i.UPrice,
                                    ReceivedTPrice = i.TPrice,
                                    StoreCode = record.ToStore,
                                    GLPeriodId = record.GLPeriodId,
                                    TransactionType = BinCardTransactionTypes.TransferSend,
                                    StatusType = BinCardTransactionStatus.Void,
                                };
                                db.BinCardLines.Add(binCardLine);
                                db.SaveChanges();

                                #endregion

                                #endregion
                            }

                        }


                        #endregion
                    }


                    #endregion
                }

            }


            return View("Success");
        }
        public ActionResult TransferReceive(string store)
        {
            var records = db.ItemTransferReceive.Where(x => x.IsOnlineApproved).AsQueryable();
            if (!String.IsNullOrEmpty(store))
                records = records.Where(x => x.ToStore == store).AsQueryable();

            foreach (var record in records)
            {
                string reff = "Transfer Receive_" + record.ItemTransferID; //to be changed after running once

                #region update bin card

                foreach (var item in record.ItemTransferItemReceive)
                {
                    #region Store Item Balance

                    var storeItem = db.StoreItemAssignments.Where(x => x.StoreCode == record.ToStore)
                        .FirstOrDefault(it => it.ItemCode == item.ItemCode);
                    if (storeItem != null)
                    {
                        var exist =
                            db.BinCardLines.Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID)
                                .Where(x => x.ReferenceDoc == reff)
                                .ToList();

                        if (!exist.Any())
                        {
                            #region bin card line

                            BinCardLine binCardLine = new BinCardLine
                            {
                                StoreItemAssignment = storeItem,
                                ReferenceDoc = reff,
                                Date = record.ReceiveDate,
                                ReceivedQTY = item.ReceivedQuantity,
                                ReceivedUPrice = item.UPrice,
                                ReceivedTPrice = item.TPrice,
                                StoreCode = record.FromStore,
                                GLPeriodId = record.GLPeriodId,
                                TransactionType = BinCardTransactionTypes.TransferReceive,
                                StatusType = BinCardTransactionStatus.Regular,
                            };

                            db.BinCardLines.Add(binCardLine);
                            db.SaveChanges();

                            #endregion
                        }
                    }


                    #endregion
                }

                #endregion

                if (record.IsVoid)
                {
                    string reffVoid = "Void Transfer Receive " + record.ItemTransferID;

                    foreach (var i in record.ItemTransferItemReceive)
                    {
                        string itemCode = i.ItemCode;

                        #region void bincard 

                        StoreItemAssignment storeItem =
                            db.StoreItemAssignments.FirstOrDefault(
                                x => x.StoreCode == record.ToStore && x.ItemCode == itemCode);

                        if (storeItem != null)
                        {
                            var exist1 = db.BinCardLines.Where(x => x.ReferenceDoc == reffVoid)
                                .Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID).ToList();
                            if (!exist1.Any())
                            {
                                #region update bin card

                                #region bin card line

                                BinCardLine binCardLine = new BinCardLine
                                {
                                    StoreItemAssignment = storeItem,
                                    ReferenceDoc = "Void Transfer Receive " + record.ItemTransferID,
                                    Date = record.ReceiveDate,
                                    IssuedQTY = i.ReceivedQuantity,
                                    IssuedUPrice = i.UPrice,
                                    IssuedTPrice = i.TPrice,
                                    StoreCode = record.FromStore,
                                    GLPeriodId = record.GLPeriodId,
                                    TransactionType = BinCardTransactionTypes.TransferReceive,
                                    StatusType = BinCardTransactionStatus.Void,
                                };
                                db.BinCardLines.Add(binCardLine);
                                db.SaveChanges();

                                #endregion

                                #endregion
                            }

                        }

                        #endregion
                    }
                }
            }


            return View("Success");
        }
        public ActionResult Issue(string store)
        {
            var records = db.Issues.Where(x => x.IsOnlineApproved).AsQueryable();
            if (!String.IsNullOrEmpty(store))
                records = records.Where(x => x.StoreCode == store).AsQueryable();

            foreach (var record in records)
            {
                string reff = "Issue " + record.IssueID.ToString();
                string storecode = record.StoreCode;

                #region  update bin card

                foreach (IssueItem i in record.IssueItems)
                {
                    string itemCode = i.ItemCode;

                    var storeItem =
                        db.StoreItemAssignments.FirstOrDefault(
                            x => x.StoreCode == storecode && x.ItemCode == itemCode);

                    if (storeItem != null)
                    {
                        var exist = db.BinCardLines.Where(x => x.ReferenceDoc == reff)
                            .Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID).ToList();
                        if (!exist.Any())
                        {
                            #region update bin card

                            #region bin card line

                            BinCardLine binCardLine = new BinCardLine
                            {
                                StoreItemAssignment = storeItem,
                                ReferenceDoc = "Issue " + record.IssueID.ToString(),
                                Date = record.IssueDate,
                                IssuedQTY = i.IssuedQuantity,
                                IssuedUPrice = i.UnitPrice,
                                IssuedTPrice = i.Total,
                                StoreCode = record.StoreCode,
                                GLPeriodId = record.GLPeriodId,
                                TransactionType = BinCardTransactionTypes.Issue,
                                StatusType = BinCardTransactionStatus.Regular,
                            };
                            db.BinCardLines.Add(binCardLine);
                            db.SaveChanges();

                            #endregion

                            #endregion
                        }

                    }
                }


                #endregion


                if (record.IsVoid)
                {
                    #region  record void bincard

                    string reffVoid = "Void Issue " + record.IssueID;


                    foreach (IssueItem i in record.IssueItems)
                    {
                        string itemCode = i.ItemCode;


                        #region  record void bincard

                        var storeItem = db.StoreItemAssignments.FirstOrDefault(x =>
                            x.StoreCode == storecode && x.ItemCode == itemCode);

                        if (storeItem != null)
                        {
                            var exist1 = db.BinCardLines.Where(x => x.ReferenceDoc == reffVoid).Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID).ToList();
                            if (!exist1.Any())
                            {
                                #region update bin card

                                #region bin card line

                                BinCardLine binCardLine = new BinCardLine
                                {
                                    StoreItemAssignment = storeItem,
                                    ReferenceDoc = "Void Issue " + record.IssueID,
                                    Date = record.IssueDate,
                                    ReceivedQTY = i.IssuedQuantity,
                                    ReceivedUPrice = i.UnitPrice,
                                    ReceivedTPrice = i.Total,
                                    StoreCode = record.StoreCode,
                                    GLPeriodId = record.GLPeriodId,
                                    TransactionType = BinCardTransactionTypes.Issue,
                                    StatusType = BinCardTransactionStatus.Void,
                                };
                                db.BinCardLines.Add(binCardLine);
                                db.SaveChanges();

                                #endregion

                                #endregion
                            }

                        }

                        #endregion

                    }


                    #endregion
                }
            }



            return View("Success");
        }
        public ActionResult Grn(string store)
        {
            var records = db.GoodReceives.Where(x => x.IsOnlineApproved).AsQueryable();
            if (!String.IsNullOrEmpty(store))
                records = records.Where(x => x.StoreCode == store).AsQueryable();
            foreach (var record in records)
            {
                string reff = "Receive_" + record.GoodReceiveId.ToString();

                #region record bin card

                
                foreach (var i in record.GoodReceiveItem)
                {
                    var storeItem =
                        db.StoreItemAssignments.Where(x => x.ItemCode == i.ItemCode)
                            .FirstOrDefault(x => x.StoreCode == record.StoreCode);


                    if (storeItem != null)
                    {
                        var exist = db.BinCardLines.Where(x => x.ReferenceDoc == reff).Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID).ToList();
                        if (!exist.Any())
                        {
                            #region bin card line

                            BinCardLine binCardLine = new BinCardLine
                            {
                                StoreItemAssignment = storeItem,
                                ReferenceDoc = "Receive_" + record.GoodReceiveId.ToString(),
                                Date = record.ReceiveDate,
                                ReceivedQTY = i.Quantity,
                                ReceivedUPrice = i.UnitPrice,
                                ReceivedTPrice = i.Total,
                                StoreCode = record.StoreCode,
                                GLPeriodId = record.GLPeriodId,
                                TransactionType = BinCardTransactionTypes.Receive,
                                StatusType = BinCardTransactionStatus.Regular,
                            };

                            db.BinCardLines.Add(binCardLine);
                            db.SaveChanges();

                            #endregion
                        }
                    }
                }

                #endregion


                if (record.IsVoid)
                {
                    string reffVoid = "Void Receive " + record.GoodReceiveId;

                    #region record void bincard

                    string storecode = record.StoreCode;

                    foreach (var i in record.GoodReceiveItem)
                    {
                        string itemCode = i.ItemCode;

                        #region update bin card

                        var storeItem =
                            db.StoreItemAssignments.FirstOrDefault(
                                x => x.StoreCode == storecode && x.ItemCode == itemCode);

                        if (storeItem != null)
                        {
                            var exist1 = db.BinCardLines.Where(x => x.ReferenceDoc == reffVoid).Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID).ToList();
                            if (!exist1.Any())
                            {
                                #region update bin card

                                #region bin card line

                                BinCardLine binCardLine = new BinCardLine
                                {
                                    StoreItemAssignment = storeItem,
                                    ReferenceDoc = "Void Receive " + record.GoodReceiveId,
                                    Date = record.ReceiveDate,
                                    IssuedQTY = i.Quantity,
                                    IssuedUPrice = i.UnitPrice,
                                    IssuedTPrice = i.Total,
                                    StoreCode = record.StoreCode,
                                    GLPeriodId = record.GLPeriodId,
                                    TransactionType = BinCardTransactionTypes.Receive,
                                    StatusType = BinCardTransactionStatus.Void,
                                };
                                db.BinCardLines.Add(binCardLine);
                                db.SaveChanges();

                                #endregion

                                #endregion
                            }

                        }



                        #endregion

                    }


                    #endregion

                }
            }
            return View("Success");
        }

        public ActionResult SplitItemCode()
        {
            var items = db.InventoryItems.ToList();
            foreach (var item in items)
            {

                try
                {
                    string[] itemcode = item.ItemCode.Split('-');

                    if (itemcode.Length == 3)
                    {
                        item.ItemIndex = int.Parse(itemcode[2]);
                        item.Prefix = itemcode[0]+ "-"+itemcode[1];
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                    //throw;
                }

            }
            

            return View("Success");
        }
    }
}