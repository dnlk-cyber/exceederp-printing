using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.Inventory.ViewModel;

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
            int min = 0;

            //if (String.IsNullOrEmpty(store))
            //{
            //    return _stockBalances.OrderBy(x => x.AccountCode); 
            //}

            var storeItemAssignments = db.StoreItemAssignments.Where(i => i.StoreCode == store)
                //.Select(x=>new {x.InventoryItem,x.StoreItemAssignmentID, x.ItemCode,x.ItemCategoryCode,x.BinCardLines,x.BinCardLineSecondaries})
                .AsQueryable();

            if (!String.IsNullOrEmpty(item))
            {
                storeItemAssignments = storeItemAssignments.Where(x => x.ItemCode == item).AsQueryable();
            }
            if (!String.IsNullOrEmpty(category))
            {
                storeItemAssignments = storeItemAssignments.Where(x => x.ItemCategoryCode == category).AsQueryable();
            }

            if (!String.IsNullOrEmpty(subcategory))
            {
                storeItemAssignments = storeItemAssignments.Where(x => x.ItemCategoryCode == subcategory).AsQueryable();
            }

            //only those who has transaction
            var storeItems = storeItemAssignments.Where(x=>x.BinCardLines.Count>0).ToList();
            foreach (var assignment in storeItems)
            {
                var stockBalanceMultipleVm = new StockBalanceMultipleVm
                {
                    ItemName = assignment.InventoryItem.Name,
                    AccountCode = assignment.InventoryItem.ItemCode,
                    Unit = assignment.InventoryItem.UoMName,
                    PartNo = assignment.InventoryItem.PartNo,
                    StockBalanceBinCards = new List<StockBalanceBinCard>()
                };

                if (sorted)
                {
                    var itemBinCardLines = assignment.BinCardLineSecondaries.Where(b => b.Date >= startDate && b.Date <= endDate)
                       .ToList();
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

                    #region current transaction

                    if (itemBinCardLines.Any())
                    {
                        min = itemBinCardLines.Min(x => x.BinCardLineSecondaryId);
                        var itemNewBinCardLines = itemBinCardLines.Where(x => x.BinCardLineSecondaryId >= min).ToList();

                        foreach (var balance in itemNewBinCardLines)
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
                                Id = balance.BinCardLineSecondaryId,
                               
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
                                IssuedTo = balance.IssuedTo != null && balance.IssuedTo!="NA"
                                    ? InventoryPrintHelper.WorkUnitName(workunit)
                                    : ""

                            };

                            stockBalanceMultipleVm.StockBalanceBinCards.Add(stocBalance);
                        }
                    }

                    #endregion

                    #region BBF

                    var bbfStock = new StockBalanceBinCard();
                    if (min != 0)
                    {
                        //from all not from within range
                        var beforeMin = assignment.BinCardLineSecondaries.Where(x => x.BinCardLineSecondaryId < min).ToList();
                        if (beforeMin.Any())
                        {
                            int maxofMin = beforeMin.Max(x => x.BinCardLineSecondaryId);
                            var temp = assignment.BinCardLineSecondaries.FirstOrDefault(x => x.BinCardLineSecondaryId == maxofMin);
                            if (temp != null)
                            {
                                int bbfId = temp.BinCardLineSecondaryId;
                                var bbfBinCard = assignment.BinCardLineSecondaries.FirstOrDefault(x => x.BinCardLineSecondaryId == bbfId);

                                if (bbfBinCard != null)
                                {
                                    int workunit = 0;
                                    try
                                    {
                                        workunit = int.Parse(bbfBinCard.IssuedTo);

                                    }
                                    catch (Exception e)
                                    {
                                        //Console.WriteLine(e);
                                        //throw;
                                    }

                                    bbfStock.Id = bbfBinCard.BinCardLineSecondaryId;
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
                                    bbfStock.SupplierName = bbfBinCard.SupplierName;
                                    bbfStock.IssuedTo = bbfBinCard.IssuedTo != null && bbfBinCard.IssuedTo != "NA"
                                        ? InventoryPrintHelper.WorkUnitName(workunit)
                                        : "";
                                }
                            }
                        }
                        else
                        {
                            bbfStock.Id = 0;
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
                    }
                    else
                    {
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
                        stockBalanceMultipleVm.StockBalanceBinCards.Add(bbfStock);
                    }

                    #endregion

                    _stockBalances.Add(stockBalanceMultipleVm);
                }
                else 
                {

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

                    #region current transaction

                    if (itemBinCardLines.Any())
                    {
                        min = itemBinCardLines.Min(x => x.BinCardLineID);
                        var itemNewBinCardLines = itemBinCardLines.Where(x => x.BinCardLineID >= min).ToList();

                        foreach (var balance in itemNewBinCardLines)
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

                    #region BBF

                    var bbfStock = new StockBalanceBinCard();
                    if (min != 0)
                    {
                        //from all not from within range
                        var beforeMin = assignment.BinCardLines.Where(x => x.BinCardLineID < min).ToList();
                        if (beforeMin.Any())
                        {
                            int maxofMin = beforeMin.Max(x => x.BinCardLineID);
                            var temp = assignment.BinCardLines.FirstOrDefault(x => x.BinCardLineID == maxofMin);
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
                    }
                    else
                    {
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
                        stockBalanceMultipleVm.StockBalanceBinCards.Add(bbfStock);
                    }

                    #endregion

                    _stockBalances.Add(stockBalanceMultipleVm);
                }
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
