using System;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Helpers.Inventory
{
    public class InventoryBalanceHelper
    {

        public static void RecalculateTransactionBincardData(int storeAssignmentId, int bincardId)
        {
            ExceedDbContext db = new ExceedDbContext();          

            var storeitem = db.StoreItemAssignments.Find(storeAssignmentId);

            var startingLine = db.BinCardLines
                .Where(x => x.StoreItemAssignmentID == storeAssignmentId)
                .FirstOrDefault(x => x.BinCardLineID == bincardId);

            var cardLines = db.BinCardLines
                .Where(x => x.StoreItemAssignmentID == storeAssignmentId)
                .AsQueryable();//added
                //.Where(x => x.Date >= startingLine.Date)
                //.OrderBy(x => x.Date)
                //.ThenBy(x => x.BinCardLineID)
                //.ToArray();

            string sortOrder = InventoryConfigurationHelper.GetBinCardRSortOrder();

            if(sortOrder== BincardSortOrderAttributes.DateOfTransaction.ToString())
            {
                cardLines = cardLines
                      .Where(x => x.Date >= startingLine.Date)
                      .OrderBy(x => x.Date)
                      .ThenBy(x => x.BinCardLineID)
                      .AsQueryable();
            }
            else
            {
                cardLines = cardLines
                      .Where(x => x.BinCardLineID >= startingLine.BinCardLineID)
                      .OrderBy(x => x.BinCardLineID)
                      .AsQueryable();
            }

            var cardLinesArray = cardLines.ToArray();

            if (cardLinesArray.Length > 0)
            {
                for (int i = 0; i < cardLinesArray.Length; i++)
                {
                    decimal totalPrice = 0;
                    decimal totalQty = 0;

                    #region update the arary

                    if (i == 0 && startingLine != null)
                    {
                        #region incrementing operation

                        if (cardLinesArray[i].ReferenceDoc.StartsWith("Receive") ||
                            cardLinesArray[i].ReferenceDoc.StartsWith("Transfer Receive"))
                        {
                            #region grn Transfer Receive

                            totalQty = startingLine.BalanceQTY + cardLinesArray[i].ReceivedQTY;
                            cardLinesArray[i].BalanceQTY = totalQty;

                            totalPrice =
                                (startingLine.BalanceQTY * startingLine.BalanceUPrice)
                                + (cardLinesArray[i].ReceivedQTY * cardLinesArray[i].ReceivedUPrice);

                            cardLinesArray[i].ReceivedTPrice = cardLinesArray[i].ReceivedQTY * cardLinesArray[i].ReceivedUPrice;
                            cardLinesArray[i].BalanceUPrice = totalQty == 0 ? 0 : totalPrice / totalQty;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }

                        else if (cardLinesArray[i].ReferenceDoc.StartsWith("Return"))
                        {
                            #region return

                            totalQty = startingLine.BalanceQTY + cardLinesArray[i].ReceivedQTY;
                            cardLinesArray[i].BalanceQTY = totalQty;

                            cardLinesArray[i].ReceivedTPrice = cardLinesArray[i].ReceivedQTY * cardLinesArray[i].ReceivedUPrice;
                            cardLinesArray[i].BalanceUPrice = startingLine.BalanceUPrice;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }

                        else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Issue"))
                        {
                            #region Void Issue

                            totalQty = startingLine.BalanceQTY + cardLinesArray[i].ReceivedQTY;
                            cardLinesArray[i].BalanceQTY = totalQty;

                            cardLinesArray[i].ReceivedUPrice = Math.Abs(cardLinesArray[i].ReceivedUPrice);
                            cardLinesArray[i].ReceivedTPrice = cardLinesArray[i].ReceivedQTY * cardLinesArray[i].ReceivedUPrice;
                            cardLinesArray[i].BalanceUPrice = startingLine.BalanceUPrice;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }

                        else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Transfer Send"))
                        {
                            #region Void Transfer Send

                            totalQty = startingLine.BalanceQTY + cardLinesArray[i].ReceivedQTY;
                            cardLinesArray[i].BalanceQTY = totalQty;

                            cardLinesArray[i].ReceivedTPrice = cardLinesArray[i].ReceivedQTY * cardLinesArray[i].ReceivedUPrice;
                            cardLinesArray[i].BalanceUPrice = startingLine.BalanceUPrice;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }

                        #endregion
                        #region dencrementing operation

                        if (cardLinesArray[i].ReferenceDoc.StartsWith("Issue") ||
                            cardLinesArray[i].ReferenceDoc.StartsWith("Transfer Send"))
                        {
                            #region issue

                            cardLinesArray[i].IssuedUPrice = startingLine.BalanceUPrice;
                            cardLinesArray[i].IssuedTPrice =
                                cardLinesArray[i].IssuedUPrice * cardLinesArray[i].IssuedQTY;

                            totalQty = startingLine.BalanceQTY - cardLinesArray[i].IssuedQTY;

                            cardLinesArray[i].BalanceQTY = totalQty;
                            cardLinesArray[i].IssuedTPrice = cardLinesArray[i].IssuedQTY * cardLinesArray[i].IssuedUPrice;
                            cardLinesArray[i].BalanceUPrice = startingLine.BalanceUPrice;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }
                        else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Receive"))
                        {
                            //decimal totalpricebefore = 0;

                            #region Void Receive

                            totalQty = startingLine.BalanceQTY - cardLinesArray[i].IssuedQTY;
                            //get issued price from original record if possible
                            //totalpricebefore = cardLines[i].BalanceTPrice -
                            //                   (cardLines[i].IssuedQTY * cardLines[i].IssuedUPrice);

                            cardLinesArray[i].BalanceQTY = totalQty;
                            cardLinesArray[i].IssuedTPrice = cardLinesArray[i].IssuedQTY * cardLinesArray[i].IssuedUPrice;
                            cardLinesArray[i].BalanceUPrice = totalQty == 0 ? 0 : startingLine.BalanceUPrice;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }

                        else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Transfer Receive"))
                        {
                            decimal totalpricebefore = 0;

                            #region Void Transfer Receive

                            totalQty = startingLine.BalanceQTY - cardLinesArray[i].IssuedQTY;
                            //get issued price from original record if possible
                            totalpricebefore = cardLinesArray[i].BalanceTPrice -
                                               (cardLinesArray[i].IssuedQTY * cardLinesArray[i].IssuedUPrice);

                            cardLinesArray[i].BalanceQTY = totalQty;
                            cardLinesArray[i].IssuedTPrice = cardLinesArray[i].IssuedQTY * cardLinesArray[i].IssuedUPrice;
                            cardLinesArray[i].BalanceUPrice = totalQty == 0 ? 0 : totalpricebefore / totalQty;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }
                        else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Return"))
                        {
                            #region Void Return

                            totalQty = startingLine.BalanceQTY - cardLinesArray[i].IssuedQTY;

                            cardLinesArray[i].BalanceQTY = totalQty;
                            cardLinesArray[i].IssuedTPrice = cardLinesArray[i].IssuedQTY * cardLinesArray[i].IssuedUPrice;
                            cardLinesArray[i].BalanceUPrice = startingLine.BalanceUPrice;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }

                        #endregion
                    }
                    else
                    {
                        #region incrementing operation

                        if (cardLinesArray[i].ReferenceDoc.StartsWith("Receive") ||
                            cardLinesArray[i].ReferenceDoc.StartsWith("Transfer Receive"))
                        {
                            #region grn Transfer Receive

                            totalQty = cardLinesArray[i - 1].BalanceQTY + cardLinesArray[i].ReceivedQTY;
                            cardLinesArray[i].BalanceQTY = totalQty;

                            totalPrice =
                                (cardLinesArray[i - 1].BalanceQTY * cardLinesArray[i - 1].BalanceUPrice)
                                + (cardLinesArray[i].ReceivedQTY * cardLinesArray[i].ReceivedUPrice);

                            cardLinesArray[i].ReceivedTPrice = cardLinesArray[i].ReceivedQTY * cardLinesArray[i].ReceivedUPrice;
                            cardLinesArray[i].BalanceUPrice = totalQty == 0 ? 0 : totalPrice / totalQty;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }

                        else if (cardLinesArray[i].ReferenceDoc.StartsWith("Return"))
                        {
                            #region return

                            cardLinesArray[i].BalanceQTY = cardLinesArray[i - 1].BalanceQTY + cardLinesArray[i].ReceivedQTY;

                            cardLinesArray[i].ReceivedUPrice = cardLinesArray[i - 1].BalanceUPrice;

                            cardLinesArray[i].ReceivedTPrice = cardLinesArray[i].ReceivedQTY * cardLinesArray[i].ReceivedUPrice;
                            cardLinesArray[i].BalanceUPrice = cardLinesArray[i - 1].BalanceUPrice;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }

                        else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Issue"))
                        {
                            #region Void Issue

                            totalQty = cardLinesArray[i - 1].BalanceQTY + cardLinesArray[i].ReceivedQTY;
                            cardLinesArray[i].BalanceQTY = totalQty;

                            cardLinesArray[i].ReceivedUPrice = Math.Abs(cardLinesArray[i].ReceivedUPrice);
                            cardLinesArray[i].ReceivedTPrice = cardLinesArray[i].ReceivedQTY * cardLinesArray[i].ReceivedUPrice;
                            cardLinesArray[i].BalanceUPrice = cardLinesArray[i - 1].BalanceUPrice;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }

                        else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Transfer Send"))
                        {
                            #region Void Transfer Send

                            totalQty = cardLinesArray[i - 1].BalanceQTY + cardLinesArray[i].ReceivedQTY;
                            cardLinesArray[i].BalanceQTY = totalQty;

                            cardLinesArray[i].ReceivedTPrice = cardLinesArray[i].ReceivedQTY * cardLinesArray[i].ReceivedUPrice;
                            cardLinesArray[i].BalanceUPrice = cardLinesArray[i - 1].BalanceUPrice;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }

                        #endregion
                        #region dencrementing operation

                        if (cardLinesArray[i].ReferenceDoc.StartsWith("Issue") ||
                            cardLinesArray[i].ReferenceDoc.StartsWith("Transfer Send"))
                        {
                            #region issue

                            cardLinesArray[i].IssuedUPrice = cardLinesArray[i - 1].BalanceUPrice;
                            cardLinesArray[i].IssuedTPrice = cardLinesArray[i].IssuedUPrice * cardLinesArray[i].IssuedQTY;

                            totalQty = cardLinesArray[i - 1].BalanceQTY - cardLinesArray[i].IssuedQTY;

                            cardLinesArray[i].BalanceQTY = totalQty;
                            cardLinesArray[i].IssuedTPrice = cardLinesArray[i].IssuedQTY * cardLinesArray[i].IssuedUPrice;
                            cardLinesArray[i].BalanceUPrice = cardLinesArray[i - 1].BalanceUPrice;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }
                        else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Receive"))
                        {
                            //decimal totalpricebefore = 0;

                            #region Void Receive

                            totalQty = cardLinesArray[i - 1].BalanceQTY - cardLinesArray[i].IssuedQTY;
                            ////get issued price from original record if possible
                            //totalpricebefore = cardLines[i].BalanceTPrice -
                            //                   (cardLines[i].IssuedQTY * cardLines[i].IssuedUPrice);

                            cardLinesArray[i].IssuedUPrice = cardLinesArray[i - 1].BalanceUPrice;
                            cardLinesArray[i].IssuedTPrice = cardLinesArray[i].IssuedUPrice * cardLinesArray[i].IssuedQTY;

                            cardLinesArray[i].BalanceQTY = totalQty;
                            cardLinesArray[i].IssuedTPrice = cardLinesArray[i].IssuedQTY * cardLinesArray[i].IssuedUPrice;
                            cardLinesArray[i].BalanceUPrice = totalQty == 0 ? 0 : cardLinesArray[i - 1].BalanceUPrice;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }

                        else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Transfer Receive"))
                        {
                            decimal totalpricebefore = 0;

                            #region Void Transfer Receive

                            totalQty = cardLinesArray[i - 1].BalanceQTY - cardLinesArray[i].IssuedQTY;
                            //get issued price from original record if possible
                            //totalpricebefore = cardLinesArray[i].BalanceTPrice -
                            //                   (cardLinesArray[i].IssuedQTY * cardLinesArray[i].IssuedUPrice);

                            cardLinesArray[i].BalanceQTY = totalQty;
                            cardLinesArray[i].IssuedTPrice = cardLinesArray[i].IssuedQTY * cardLinesArray[i].IssuedUPrice;
                            cardLinesArray[i].BalanceUPrice = totalQty == 0 ? 0 : cardLinesArray[i - 1].BalanceUPrice;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }
                        else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Return"))
                        {
                            #region Void Return

                            totalQty = cardLinesArray[i - 1].BalanceQTY - cardLinesArray[i].IssuedQTY;

                            cardLinesArray[i].BalanceQTY = totalQty;
                            cardLinesArray[i].IssuedTPrice = cardLinesArray[i].IssuedQTY * cardLinesArray[i].IssuedUPrice;
                            cardLinesArray[i].BalanceUPrice = cardLinesArray[i - 1].BalanceUPrice;
                            cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                            #endregion
                        }

                        #endregion
                    }

                    #endregion

                    db.Entry(cardLinesArray[i]).State = EntityState.Modified;

                    if (i == cardLinesArray.Length - 1)
                    {
                        if (storeitem != null)
                        {
                            storeitem.AvgPrice = cardLinesArray[i].BalanceUPrice;
                            storeitem.Balance = cardLinesArray[i].BalanceQTY;
                            db.Entry(storeitem).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
            }
            else
            {
                if (storeitem != null && startingLine != null)
                {
                    storeitem.AvgPrice = startingLine.BalanceUPrice;
                    storeitem.Balance = startingLine.BalanceQTY;
                    db.Entry(storeitem).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        public static void UpdateInventoryRecordWithBincardData(string assignmentid, string binid)
        {
            ExceedDbContext db = new ExceedDbContext();
            int updatedPeriodId = 0;
            var allowed = InventoryConfigurationHelper.RecalculateSourceTransactionOnBincardUpdate();

            if (assignmentid != null && binid != null&& allowed)
            {
                int storeAssignmentId = int.Parse(assignmentid);
                int bincardId = int.Parse(binid);

                var storeitem = db.StoreItemAssignments.Find(storeAssignmentId);               

                var cardLines = db.BinCardLines
                    .Where(x => x.StoreItemAssignmentID == storeAssignmentId)
                    .Where(x => x.BinCardLineID >= bincardId).ToArray();

                foreach(var card in cardLines)
                {

                    #region incrementing operation

                    if (card.ReferenceDoc.StartsWith("Receive"))
                    {
                        #region grn 
                        string id = card.ReferenceDoc.Split('_')[1];
                        int recordid = int.Parse(id);

                        #region update record
                        var record = db.GoodReceives.Find(recordid);
                        if (record != null)
                        {
                            updatedPeriodId = InventoryHelper.GetInventoryPeriodFromDate(card.Date);

                            record.GLPeriodId = updatedPeriodId != 0 ? updatedPeriodId : record.GLPeriodId;
                            record.ReceiveDate = card.Date;                           
                            db.GoodReceives.Attach(record);
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        #endregion

                        var item = db.GoodReceiveItems
                            .Where(x => x.ItemCode == storeitem.ItemCode)
                            .Where(x => x.GoodReceiveId == recordid).FirstOrDefault();

                        var tax = db.GLTaxRates.Find(item.GLTaxRateID);

                        if (item != null)
                        {
                            item.UnitPrice = card.ReceivedUPrice;
                            item.Quantity = card.ReceivedQTY;
                            item.SubTotal = card.ReceivedQTY * card.ReceivedUPrice;
                            item.TaxAmount = card.ReceivedUPrice * card.ReceivedQTY * tax.CalculatedRate;
                            item.Total = item.TaxAmount + item.SubTotal;

                            db.GoodReceiveItems.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }                        

                        #endregion
                    }

                    else if (card.ReferenceDoc.StartsWith("Transfer Receive"))
                    {
                        #region Transfer Receive

                        string id = card.ReferenceDoc.Split('_')[1];
                        int recordid = int.Parse(id);
                        #region update record
                        var record = db.ItemTransferReceive.Find(recordid);
                        if (record != null)
                        {
                            updatedPeriodId = InventoryHelper.GetInventoryPeriodFromDate(card.Date);

                            record.GLPeriodId = updatedPeriodId != 0 ? updatedPeriodId : record.GLPeriodId;
                            record.ReceiveDate = card.Date;
                            db.ItemTransferReceive.Attach(record);
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        #endregion
                        var item = db.ItemTransferItemReceive
                            .Where(x => x.ItemCode == storeitem.ItemCode)
                            .Where(x => x.ItemTransferReceiveId == recordid).FirstOrDefault();                      

                        if (item != null)
                        {
                            item.UPrice = card.ReceivedUPrice;
                            item.ReceivedQuantity = card.ReceivedQTY;
                            item.TPrice = card.ReceivedUPrice * card.ReceivedQTY;

                            db.ItemTransferItemReceive.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        #endregion
                    }

                    if (card.ReferenceDoc.StartsWith("Return"))
                    {
                        #region return 
                        string id = card.ReferenceDoc.Split('_')[1];
                        int recordid = int.Parse(id);

                        #region update record
                        var record = db.StoreReturns.Find(recordid);
                        if (record != null)
                        {
                            updatedPeriodId = InventoryHelper.GetInventoryPeriodFromDate(card.Date);

                            record.GLPeriodId = updatedPeriodId != 0 ? updatedPeriodId : record.GLPeriodId;
                            record.ReturnDate = card.Date;
                            db.StoreReturns.Attach(record);
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        #endregion

                        var item = db.StoreReturnItemss
                            .Where(x => x.ItemCode == storeitem.ItemCode)
                            .Where(x => x.StoreReturnID == recordid).FirstOrDefault();

                      

                        if (item != null)
                        {
                            item.UnitPrice = card.ReceivedUPrice;
                            item.Quantity = card.ReceivedQTY;

                            db.StoreReturnItemss.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        #endregion
                    }



                    #endregion
                    #region dencrementing operation

                    else if (card.ReferenceDoc.StartsWith("Issue"))
                    {
                        #region issue

                        string id = card.ReferenceDoc.Split(' ')[1];
                        int recordid = int.Parse(id);

                        #region update record
                        var record = db.Issues.Find(recordid);
                        if (record != null)
                        {
                            updatedPeriodId = InventoryHelper.GetInventoryPeriodFromDate(card.Date);

                            record.GLPeriodId = updatedPeriodId != 0 ? updatedPeriodId : record.GLPeriodId;
                            record.IssueDate = card.Date;
                            db.Issues.Attach(record);
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        #endregion

                        var item = db.IssueItems
                            .Where(x => x.ItemCode == storeitem.ItemCode)
                            .Where(x => x.IssueID == recordid).FirstOrDefault();                       

                        if (item != null)
                        {
                            item.UnitPrice = card.IssuedUPrice;
                            item.IssuedQuantity = card.IssuedQTY;
                            item.Total = card.IssuedUPrice * card.IssuedQTY;

                            db.IssueItems.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        #endregion
                    }

                    else if (card.ReferenceDoc.StartsWith("Transfer Send"))
                    {
                        #region transfer send

                        string id = card.ReferenceDoc.Split(' ')[1];
                        int recordid = int.Parse(id);
                        #region update record
                        var record = db.ItemTransfers.Find(recordid);
                        if (record != null)
                        {
                            updatedPeriodId = InventoryHelper.GetInventoryPeriodFromDate(card.Date);

                            record.GLPeriodId = updatedPeriodId != 0 ? updatedPeriodId : record.GLPeriodId;
                            record.TransferDate = card.Date;
                            db.ItemTransfers.Attach(record);
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        #endregion
                        var item = db.ItemTransferItems
                            .Where(x => x.ItemCode == storeitem.ItemCode)
                            .Where(x => x.ItemTransferID == recordid).FirstOrDefault();

                        if (item != null)
                        {
                            item.UPrice = card.IssuedUPrice;
                            item.ShippedQuantity = card.IssuedQTY;
                            item.TPrice = card.IssuedUPrice * card.IssuedQTY;

                            db.ItemTransferItems.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        #endregion
                    }


                    #endregion

                }      


               

            }
        }
    }
}
