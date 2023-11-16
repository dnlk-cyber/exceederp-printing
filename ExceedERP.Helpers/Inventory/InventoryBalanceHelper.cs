using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Helpers.Inventory
{
    public class InventoryBalanceHelper
    {
        public static void RemoveDuplicateBinCardTransactions()
        {
            ExceedDbContext _db = new ExceedDbContext();
            #region find duplicate bin card records

            var duplicates = _db.BinCardLines
             .GroupBy(x => new { x.StoreItemAssignmentID, x.ReferenceDoc })
            .Where(g => g.Count() > 1)
            .Select(y => new { Element = y.Key, Counter = y.Count() })
            .ToList();
            foreach (var d in duplicates)
            {
                #region remove bin card line

                var duplicateledgerlines = _db.BinCardLines
                    .Where(x => x.StoreItemAssignmentID == d.Element.StoreItemAssignmentID)
                    .Where(x => x.ReferenceDoc == d.Element.ReferenceDoc)
                    .ToList();
                var minline = duplicateledgerlines.Min(x => x.BinCardLineID);
                var toberemoved = duplicateledgerlines.Where(x => x.BinCardLineID != minline).ToList();
                _db.BinCardLines.RemoveRange(toberemoved);
                _db.SaveChanges();

                //update balance dayly end
                //var jobUpdateStandard = BackgroundJob.Enqueue(() => _financeHelper.UpdateAllStandardBalancesMonthlyFastP(firstPeriodId, User.Identity.Name));
                #endregion
            }

            #endregion


        }

        public static string RecalculateTransactionBincardData(int storeAssignmentId, int bincardId,bool useNextYear=false)
        {
            ExceedDbContext db = new ExceedDbContext();
            string errorMessage = string.Empty;
            var storeitem = db.StoreItemAssignments.Find(storeAssignmentId);

            var startingLine = db.BinCardLines
                .Where(x => x.StoreItemAssignmentID == storeAssignmentId)
                .FirstOrDefault(x => x.BinCardLineID == bincardId);
            if (startingLine != null)
            {
                if (startingLine.ReferenceDoc.StartsWith("Count on date"))
                    useNextYear = true;// if it is count usually it is end of year ,no transaction to recalculate unless it goes to next year

                var currentYear = db.GLFiscalYears.FirstOrDefault(y => startingLine.Date >= y.DateFrom && startingLine.Date <= y.DateTo);
                if (currentYear != null)
                {
                    // for recalculate all we shall use the next year since it starts from previous year
                    if (useNextYear)
                    {
                        var fiscalYearHelper = new FiscalYearHelper();
                        int nextFiscalYearId = fiscalYearHelper.GetNextFYearId(currentYear.GlFiscalYearId);
                        if (nextFiscalYearId != 0)
                        {
                            var nextYear = db.GLFiscalYears.Find(nextFiscalYearId);
                            if (nextYear != null)
                                currentYear = nextYear;
                        }
                    }
                    var countBinCard = db.BinCardLines
                        .FirstOrDefault(b => b.StoreItemAssignmentID == storeAssignmentId &&
                            b.ReferenceDoc != null &&
                            b.ReferenceDoc.StartsWith("Count on date") &&
                            b.Date > startingLine.Date && b.Date <= currentYear.DateTo
                        );
                    var cardLines = db.BinCardLines
                        .Where(x => x.StoreItemAssignmentID == storeAssignmentId)
                        .AsQueryable();//added
                                       //.Where(x => x.Date >= startingLine.Date)
                                       //.OrderBy(x => x.Date)
                                       //.ThenBy(x => x.BinCardLineID)
                                       //.ToArray();

                    string sortOrder = InventoryConfigurationHelper.GetBinCardRSortOrder();

                    if (sortOrder == BincardSortOrderAttributes.DateOfTransaction.ToString())
                    {
                        cardLines = cardLines
                              .Where(x => x.Date >= startingLine.Date && x.Date <= currentYear.DateTo)
                              .OrderBy(x => x.Date)
                              .ThenBy(x => x.BinCardLineID)
                              .AsQueryable();

                        if (countBinCard != null)
                        {
                            cardLines = cardLines
                                .Where(b => b.Date <= countBinCard.Date && b.BinCardLineID != countBinCard.BinCardLineID)
                                .OrderBy(x => x.Date)
                                .ThenBy(x => x.BinCardLineID);
                        }
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


                                    cardLinesArray[i].BalanceQTY = startingLine.BalanceQTY;

                                    cardLinesArray[i].ReceivedTPrice = cardLinesArray[i].ReceivedQTY * cardLinesArray[i].ReceivedUPrice;
                                    cardLinesArray[i].BalanceUPrice = startingLine.BalanceUPrice;
                                    cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                                    #endregion
                                }

                                else if (cardLinesArray[i].ReferenceDoc.StartsWith("Return"))
                                {
                                    #region return                               
                                    cardLinesArray[i].BalanceQTY = startingLine.BalanceQTY;

                                    cardLinesArray[i].ReceivedTPrice = cardLinesArray[i].ReceivedQTY * cardLinesArray[i].ReceivedUPrice;
                                    cardLinesArray[i].BalanceUPrice = startingLine.BalanceUPrice;
                                    cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                                    #endregion
                                }

                                else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Issue") ||
                                            cardLinesArray[i].ReferenceDoc.StartsWith("Void SalesDonation"))
                                {
                                    #region Void Issue

                                    cardLinesArray[i].BalanceQTY = startingLine.BalanceQTY;

                                    cardLinesArray[i].ReceivedUPrice = Math.Abs(cardLinesArray[i].ReceivedUPrice);
                                    cardLinesArray[i].ReceivedTPrice = cardLinesArray[i].ReceivedQTY * cardLinesArray[i].ReceivedUPrice;
                                    cardLinesArray[i].BalanceUPrice = startingLine.BalanceUPrice;
                                    cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                                    #endregion
                                }

                                else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Shipment"))
                                {
                                    #region Void shipment
                                    cardLinesArray[i].BalanceQTY = startingLine.BalanceQTY;

                                    cardLinesArray[i].ReceivedUPrice = Math.Abs(cardLinesArray[i].ReceivedUPrice);
                                    cardLinesArray[i].ReceivedTPrice = cardLinesArray[i].ReceivedQTY * cardLinesArray[i].ReceivedUPrice;
                                    cardLinesArray[i].BalanceUPrice = startingLine.BalanceUPrice;
                                    cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                                    #endregion
                                }

                                else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Transfer Send"))
                                {
                                    #region Void Transfer Send
                                    cardLinesArray[i].BalanceQTY = startingLine.BalanceQTY;

                                    cardLinesArray[i].ReceivedTPrice = cardLinesArray[i].ReceivedQTY * cardLinesArray[i].ReceivedUPrice;
                                    cardLinesArray[i].BalanceUPrice = startingLine.BalanceUPrice;
                                    cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                                    #endregion
                                }

                                #endregion
                                #region dencrementing operation

                                if (cardLinesArray[i].ReferenceDoc.StartsWith("Issue") ||
                                    cardLinesArray[i].ReferenceDoc.StartsWith("Transfer Send") ||
                                    cardLinesArray[i].ReferenceDoc.StartsWith("SalesDonation") ||
                                    cardLinesArray[i].ReferenceDoc.StartsWith("Shipment"))
                                {
                                    #region issue

                                    cardLinesArray[i].IssuedUPrice = startingLine.BalanceUPrice;
                                    cardLinesArray[i].IssuedTPrice =
                                        cardLinesArray[i].IssuedUPrice * cardLinesArray[i].IssuedQTY;

                                    cardLinesArray[i].BalanceQTY = startingLine.BalanceQTY;
                                    cardLinesArray[i].IssuedTPrice = cardLinesArray[i].IssuedQTY * cardLinesArray[i].IssuedUPrice;
                                    cardLinesArray[i].BalanceUPrice = startingLine.BalanceUPrice;
                                    cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                                    #endregion
                                }
                                else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Receive"))
                                {
                                    //decimal totalpricebefore = 0;

                                    #region Void Receive



                                    cardLinesArray[i].BalanceQTY = startingLine.BalanceQTY;
                                    cardLinesArray[i].IssuedTPrice = cardLinesArray[i].IssuedQTY * cardLinesArray[i].IssuedUPrice;
                                    cardLinesArray[i].BalanceUPrice = startingLine.BalanceUPrice;
                                    cardLinesArray[i].BalanceTPrice = cardLinesArray[i].BalanceUPrice * cardLinesArray[i].BalanceQTY;

                                    #endregion
                                }

                                else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Transfer Receive"))
                                {
                                    decimal totalpricebefore = 0;

                                    #region Void Transfer Receive

                                    totalQty = startingLine.BalanceQTY;
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


                                    cardLinesArray[i].BalanceQTY = startingLine.BalanceQTY;
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

                                else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Issue") ||
                                           cardLinesArray[i].ReferenceDoc.StartsWith("Void SalesDonation"))
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

                                else if (cardLinesArray[i].ReferenceDoc.StartsWith("Void Shipment"))
                                {
                                    #region Void Shipment

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
                                    cardLinesArray[i].ReferenceDoc.StartsWith("Transfer Send") ||
                                    cardLinesArray[i].ReferenceDoc.StartsWith("SalesDonation") ||
                                    cardLinesArray[i].ReferenceDoc.StartsWith("Shipment"))
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
                                    // (cardLinesArray[i].IssuedQTY * cardLinesArray[i].IssuedUPrice);

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

                            #region updated related transaction
                            if (InventoryConfigurationHelper.RecalculateSourceTransactionOnBincardUpdate())
                            {
                                int updatedPeriodId = 0;
                                var card = cardLinesArray[i];
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

                                    string id = card.ReferenceDoc.Split(' ')[2];
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
                            #endregion
                        }

                        #region find and update count table with the most recent bincard balance
                        //NEW.. COUNT SHOULD NOT BE UPDATED EVERYTIME.
                        //if (sortOrder == BincardSortOrderAttributes.DateOfTransaction.ToString() && countBinCard != null)
                        //{
                        //    PropertyCountHelper
                        //        .UpdatePropertyCountItemPreviousBalanceFromBinCard
                        //        (countBinCard: countBinCard, latestBinCard: cardLines.ToArray()[cardLines.Count() - 1]);
                        //}
                        #endregion
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
                else
                {
                    errorMessage = "Fiscal year not found.";
                }
            }
            else
            {
                errorMessage = "Starting bincard line not found.";
            }

            return errorMessage;
        }
        public static void UpdateInventoryRecordWithBincardData(string assignmentid, string binid)
        {
            ExceedDbContext db = new ExceedDbContext();
            int updatedPeriodId = 0;
            var allowed = InventoryConfigurationHelper.RecalculateSourceTransactionOnBincardUpdate();

            if (assignmentid != null && binid != null && allowed)
            {
                int storeAssignmentId = int.Parse(assignmentid);
                int bincardId = int.Parse(binid);

                var storeitem = db.StoreItemAssignments.Find(storeAssignmentId);

                var cardLines = db.BinCardLines
                    .Where(x => x.StoreItemAssignmentID == storeAssignmentId)
                    .Where(x => x.BinCardLineID >= bincardId).ToArray();

                foreach (var card in cardLines)
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

                        string id = card.ReferenceDoc.Split(' ')[2];
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

        //public static void AutoRecalculateBincardLines(List<BinCardLine> binCardLinesList)
        public static void AutoRecalculateBincardLines(List<int> binCardLineIds)
        {
            if (InventoryConfigurationHelper.IsBinCardRecalculateAllowed())
            {
                var db = new ExceedDbContext();
                string sortOrder = InventoryConfigurationHelper.GetBinCardRSortOrder();
                var binCardLinesList = db.BinCardLines.Where(b => binCardLineIds.Contains(b.BinCardLineID)).ToList();
                foreach (var binCardLine in binCardLinesList)
                {
                    var currentYear = db.GLFiscalYears.FirstOrDefault(y => binCardLine.Date >= y.DateFrom && binCardLine.Date <= y.DateTo);
                    if (currentYear != null)
                    {
                        #region find the most recent record prior to this very one
                        var binCardLines =
                               db.BinCardLines
                               .Where(b => b.Date <= binCardLine.Date &&
                               b.BinCardLineID != binCardLine.BinCardLineID &&
                               b.StoreItemAssignmentID == binCardLine.StoreItemAssignmentID &&
                               b.Date >= currentYear.DateFrom && b.Date <= currentYear.DateTo &&
                               b.ReferenceDoc != null &&
                               !b.ReferenceDoc.StartsWith("Count on date")
                               ).ToList();
                        if (sortOrder == BincardSortOrderAttributes.DateOfTransaction.ToString())
                        {
                            binCardLines = binCardLines.OrderByDescending(d => d.Date).ThenByDescending(d => d.BinCardLineID).ToList();
                            if (binCardLines.Any())
                            {
                                var targetBinTransaction = binCardLines[0];
                                RecalculateTransactionBincardData(targetBinTransaction.StoreItemAssignmentID, targetBinTransaction.BinCardLineID);
                            }
                        }
                        #endregion
                    }
                }
            }
        }
    }
}
