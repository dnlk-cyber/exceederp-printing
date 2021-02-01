using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.Inventory.ViewModel;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Reporting.Inventory.DataSource
{
    [DataObject]
    public class StockBalanceOds : List<StockBalanceOds>
    {

        ExceedDbContext db = new ExceedDbContext();

     
        [DataObjectMethod(DataObjectMethodType.Select)]
        public IOrderedEnumerable<StockBalanceMultipleVm> GetStockBalance(string store, DateTime? start, DateTime? end, string category,string subcategory, string item, bool includeVoid, bool sorted)
        {
         List<StockBalanceMultipleVm> _stockBalances = new List<StockBalanceMultipleVm>();
         List<StockBalanceMultipleVm> list = new List<StockBalanceMultipleVm>();

            DateTime startDate = (DateTime)start;
            DateTime endDate = (DateTime)end;
           

            //if (String.IsNullOrEmpty(store))
            //{
            //    return _stockBalances.OrderBy(x => x.AccountCode); 
            //}

            var storeItemAssignments = db.StoreItemAssignments
                                        .Where(i => i.StoreCode == store)
                                        //.Select(x=>new {x.InventoryItem,x.StoreItemAssignmentID, x.ItemCode,x.ItemCategoryCode,x.BinCardLines,x.BinCardLineSecondaries})
                                        .AsQueryable();

            if (!String.IsNullOrEmpty(category))
            {
                storeItemAssignments = storeItemAssignments.Where(x => x.ItemCategoryCode == category).AsQueryable();
            }

            if (!String.IsNullOrEmpty(subcategory))
            {
                storeItemAssignments = storeItemAssignments.Where(x => x.InventoryItem.SubCategoryCode == subcategory).AsQueryable();
            }

            if (!String.IsNullOrEmpty(item))
            {
                storeItemAssignments = storeItemAssignments.Where(x => x.ItemCode == item).AsQueryable();
            }

            //only those who has transaction
            var storeItems = storeItemAssignments.Where(x=>x.BinCardLines.Any()).ToList();

            //get previous year end date and for recalculation
            FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();
            int prevYearid = fiscalYearHelper.GetPreviousFYearId(fiscalYearHelper.GetCurrentFYearId());
            var prevYear = db.GLFiscalYears.Find(prevYearid);

            string sortOrder = InventoryConfigurationHelper.GetBinCardRSortOrder();

            foreach (var assignment in storeItems)
            {
                if (InventoryConfigurationHelper.IsBinCardRecalculateAllowed())
                {
                    //#region recalculate bincard
                    //var count_startingLine = db.BinCardLines
                    //.Where(x => x.StoreItemAssignmentID == assignment.StoreItemAssignmentID)
                    //.Where(x => x.Date == prevYear.DateTo)
                    //.Where(x => x.ReferenceDoc.StartsWith("Count") || x.TransactionType == BinCardTransactionTypes.Count)
                    //.FirstOrDefault();

                    //if (count_startingLine != null)
                    //    InventoryBalanceHelper.RecalculateTransactionBincardData(assignment.StoreItemAssignmentID, count_startingLine.BinCardLineID);
                    //else
                    //{
                    //    var non_count_startingLine = db.BinCardLines
                    //   .Where(x => x.StoreItemAssignmentID == assignment.StoreItemAssignmentID)
                    //   .Where(x => x.Date == prevYear.DateTo)                      
                    //   .FirstOrDefault();
                    //    if (non_count_startingLine != null)
                    //        InventoryBalanceHelper.RecalculateTransactionBincardData(assignment.StoreItemAssignmentID, non_count_startingLine.BinCardLineID);
                    //}
                    //#endregion
                }

                var stockBalanceMultipleVm = new StockBalanceMultipleVm
                {
                    ItemName = assignment.InventoryItem.Name,
                    AccountCode = assignment.InventoryItem.ItemCode,
                    Unit = assignment.InventoryItem.UoMName,
                    PartNo = assignment.InventoryItem.PartNo,
                    StockBalanceBinCards = new List<StockBalanceBinCard>()
                };

                var itemBinCardLines = assignment.BinCardLines.Where(b => b.Date >= startDate && b.Date <= endDate)
                       .ToList();
                //new
                //var itemBinCardLines = db.BinCardLines
                //    .Where(x=>x.StoreItemAssignmentID==assignment.StoreItemAssignmentID)
                //    .Where(x=>x.Date >= startDate)
                //    .Where(x=>x.Date <= endDate)
                //    .OrderBy(x=>x.BinCardLineID)//new
                //    .ToList();

                if (!includeVoid)
                {
                    #region filter void and related

                    var nonvoids =
                        itemBinCardLines.Where(x => x.ReferenceDoc.ToUpper().Contains("void".ToUpper()) == false)
                            .ToList();
                    var voids =
                        itemBinCardLines.Where(x => x.ReferenceDoc.ToUpper().Contains("void".ToUpper())).ToList();

                    List<string> voidId = new List<string>();
                    foreach (var v in voids)
                    {
                        if (v.ReferenceDoc.Contains("Void Receive"))
                        {
                            string relatedrecord = "Receive_" + v.ReferenceDoc.Substring(13);
                            voidId.Add(relatedrecord);
                        }
                        else if (v.ReferenceDoc.Contains("Void Issue"))
                        {
                            string relatedrecord = "Issue " + v.ReferenceDoc.Substring(11);
                            voidId.Add(relatedrecord);
                        }
                        else if (v.ReferenceDoc.Contains("Void Return_"))
                        {
                            string relatedrecord = "Return_" + v.ReferenceDoc.Substring(12);
                            voidId.Add(relatedrecord);
                        }
                        else if (v.ReferenceDoc.Contains("Void Transfer Send "))
                        {
                            string relatedrecord = "Transfer Send " + v.ReferenceDoc.Substring(19);
                            voidId.Add(relatedrecord);
                        }

                        else if (v.ReferenceDoc.Contains("Void Transfer Receive "))
                        {
                            string relatedrecord = "Transfer Receive_" + v.ReferenceDoc.Substring(22);
                            voidId.Add(relatedrecord);
                        }
                    }

                    nonvoids = nonvoids.Where(x => voidId.Contains(x.ReferenceDoc) == false).ToList();
                    itemBinCardLines = nonvoids;

                    #endregion
                }

                #region BBF

                var bbfStock = new StockBalanceBinCard();

                if (sortOrder == BincardSortOrderAttributes.DateOfTransaction.ToString())
                {
                    #region sort by date
                    var BBF = assignment.BinCardLines.Where(x => x.Date < startDate).AsQueryable();
                    //from all not from within range                   
                    if (BBF.Any())
                    {
                        DateTime maxofMin = BBF.Max(x => x.Date);
                        var temp = assignment.BinCardLines.FirstOrDefault(x => x.Date == maxofMin);
                        if (temp != null)
                        {
                            int bbfId = temp.BinCardLineID;
                            var bbfBinCard = assignment.BinCardLines.FirstOrDefault(x => x.BinCardLineID == bbfId);

                            if (bbfBinCard != null)
                            {
                                bbfStock.Id = bbfBinCard.BinCardLineID;
                                bbfStock.StoreCode = bbfBinCard.StoreCode;
                                bbfStock.Date = bbfBinCard.Date;
                                bbfStock.ReferenceDoc = "(BBF) " + bbfBinCard.ReferenceDoc;
                                bbfStock.BalanceQTY = bbfBinCard.BalanceQTY;
                                bbfStock.IssuedQTY = bbfBinCard.IssuedQTY;
                                bbfStock.BalanceTPrice = bbfBinCard.BalanceTPrice;
                                bbfStock.BalanceUPrice = bbfBinCard.BalanceUPrice;
                                bbfStock.IssuedTPrice = bbfBinCard.IssuedTPrice;
                                bbfStock.IssuedOrReceivedUPrice = bbfBinCard.ReceivedQTY == 0
                                    ? bbfBinCard.IssuedUPrice
                                    : bbfBinCard.ReceivedUPrice;
                                bbfStock.ReceivedUPrice = bbfBinCard.ReceivedUPrice;
                                bbfStock.IssuedUPrice = bbfBinCard.IssuedUPrice;
                                bbfStock.AveragePrice = bbfBinCard.BalanceUPrice;
                                bbfStock.BalanceTPrice = bbfBinCard.BalanceTPrice;
                                bbfStock.OriginalRef = bbfBinCard.Reference;
                            }
                        }
                    }
                    else
                    {
                        bbfStock.Id = 0;
                        bbfStock.Date = startDate.AddDays(-1);//for sort to be at top
                        bbfStock.ReferenceDoc = "No BBF";
                        bbfStock.BalanceQTY = 0;
                        bbfStock.IssuedQTY = 0;
                        bbfStock.BalanceTPrice = 0;
                        bbfStock.BalanceUPrice = 0;
                        bbfStock.IssuedTPrice = 0;
                        bbfStock.ReceivedUPrice = 0;
                        bbfStock.IssuedUPrice = 0;
                        bbfStock.AveragePrice = 0;
                        bbfStock.BalanceTPrice = 0;
                        bbfStock.OriginalRef = "";
                    }

                    stockBalanceMultipleVm.StockBalanceBinCards.Add(bbfStock);
                    #endregion
                }
                else
                {
                    #region sort by id
                    var BBF = assignment.BinCardLines.Where(x => x.Date < startDate).AsQueryable();
                    //from all not from within range                   
                    if (BBF.Any())
                    {
                        int maxofMin = BBF.Max(x => x.BinCardLineID);
                        var bbfBinCard = assignment.BinCardLines.FirstOrDefault(x => x.BinCardLineID == maxofMin);
                        if (bbfBinCard != null)
                        {
                            bbfStock.Id = bbfBinCard.BinCardLineID;
                            bbfStock.StoreCode = bbfBinCard.StoreCode;
                            bbfStock.Date = bbfBinCard.Date;
                            bbfStock.ReferenceDoc = "(BBF) " + bbfBinCard.ReferenceDoc;
                            bbfStock.BalanceQTY = bbfBinCard.BalanceQTY;
                            bbfStock.IssuedQTY = bbfBinCard.IssuedQTY;
                            bbfStock.BalanceTPrice = bbfBinCard.BalanceTPrice;
                            bbfStock.BalanceUPrice = bbfBinCard.BalanceUPrice;
                            bbfStock.IssuedTPrice = bbfBinCard.IssuedTPrice;
                            bbfStock.IssuedOrReceivedUPrice = bbfBinCard.ReceivedQTY == 0
                                ? bbfBinCard.IssuedUPrice
                                : bbfBinCard.ReceivedUPrice;
                            bbfStock.ReceivedUPrice = bbfBinCard.ReceivedUPrice;
                            bbfStock.IssuedUPrice = bbfBinCard.IssuedUPrice;
                            bbfStock.AveragePrice = bbfBinCard.BalanceUPrice;
                            bbfStock.BalanceTPrice = bbfBinCard.BalanceTPrice;
                            bbfStock.OriginalRef = bbfBinCard.Reference;
                        }
                    }
                    else
                    {
                        bbfStock.Id = 0;
                        bbfStock.Date = startDate.AddDays(-1);//for sort to be at top
                        bbfStock.ReferenceDoc = "No BBF";
                        bbfStock.BalanceQTY = 0;
                        bbfStock.IssuedQTY = 0;
                        bbfStock.BalanceTPrice = 0;
                        bbfStock.BalanceUPrice = 0;
                        bbfStock.IssuedTPrice = 0;
                        bbfStock.ReceivedUPrice = 0;
                        bbfStock.IssuedUPrice = 0;
                        bbfStock.AveragePrice = 0;
                        bbfStock.BalanceTPrice = 0;
                        bbfStock.OriginalRef = "";
                    }

                    stockBalanceMultipleVm.StockBalanceBinCards.Add(bbfStock);
                    #endregion
                }




                #endregion
                #region current transaction

                if (itemBinCardLines.Any())
                {

                    var newBinCardLines = itemBinCardLines
                        .Where(x => x.Date >= startDate)
                        //.OrderBy(x=>x.Date)
                        .AsQueryable();                  

                    if (sortOrder == BincardSortOrderAttributes.DateOfTransaction.ToString())
                    {
                        newBinCardLines = newBinCardLines
                                            .OrderBy(x => x.Date)
                                            .ThenBy(x => x.BinCardLineID)
                                           .AsQueryable();
                    }
                    else
                    {
                        newBinCardLines = newBinCardLines
                                            .OrderBy(x => x.BinCardLineID)
                                           .AsQueryable();
                    }

                    var sortedBinCardLines = newBinCardLines.ToList();

                    foreach (var balance in sortedBinCardLines)
                    {


                        int workunit = 0;
                        try
                        {
                            workunit = int.Parse(balance.IssuedTo);

                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine(e);
                            //throw;
                        }

                        var stocBalance = new StockBalanceBinCard
                        {
                            Id = balance.BinCardLineID,
                            StoreCode = balance.StoreCode,
                            Date = balance.Date,
                            ReferenceDoc = balance.ReferenceDoc,
                            IssuedQTY = balance.IssuedQTY,
                            IssuedUPrice = balance.IssuedUPrice,
                            IssuedTPrice = balance.IssuedTPrice,

                            ReceivedQTY = balance.ReceivedQTY,
                            ReceivedUPrice = balance.ReceivedUPrice,
                            ReceivedTPrice = balance.ReceivedTPrice,
                            IssuedOrReceivedUPrice =
                                balance.ReceivedQTY == 0 ? balance.IssuedUPrice : balance.ReceivedUPrice,
                            BalanceQTY = balance.BalanceQTY,
                            BalanceUPrice = balance.BalanceUPrice,
                            BalanceTPrice = balance.BalanceTPrice,
                            AveragePrice = balance.BalanceUPrice,
                            OriginalRef = balance.Reference,
                            SupplierName = balance.SupplierName,
                            IssuedTo = balance.IssuedTo != null && balance.IssuedTo != "NA"
                                ? InventoryPrintHelper.WorkUnitName(workunit)
                                : ""

                        };

                        stockBalanceMultipleVm.StockBalanceBinCards.Add(stocBalance);
                    }
                }

                #endregion

               

                _stockBalances.Add(stockBalanceMultipleVm);
            }

            #region filter zero count and no transaction

           
            foreach (var balance in _stockBalances)
            {
                bool canAdd = true;
                if (balance.StockBalanceBinCards.Count == 1)
                {
                    var countRecord = balance.StockBalanceBinCards
                        .Where(x=>x.BalanceQTY==0)
                        .FirstOrDefault(x => x.ReferenceDoc.StartsWith("(BBF) Count"));

                    if (countRecord!=null)
                        canAdd = false;
                }
                if (balance.StockBalanceBinCards.Count == 2)
                {
                    var countRecord = balance.StockBalanceBinCards
                        .Where(x => x.BalanceQTY == 0)
                        .FirstOrDefault(x => x.ReferenceDoc.StartsWith("Count"));

                    var bbfRecord = balance.StockBalanceBinCards
                        .FirstOrDefault(x => x.ReferenceDoc.StartsWith("(BBF)"));

                    if (countRecord != null && bbfRecord != null)
                        canAdd = false;
                }
                if (canAdd)
                    list.Add(balance);
            }
            #endregion 

            return list.OrderBy(x=>x.AccountCode);
        }

    }
}
