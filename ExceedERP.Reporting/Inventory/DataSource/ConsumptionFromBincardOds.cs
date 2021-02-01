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
    public class ConsumptionFromBincardOds : List<StockBalanceOds>
    {

        ExceedDbContext db = new ExceedDbContext();

        private readonly List<StockBalanceMultipleVm> _stockBalances = new List<StockBalanceMultipleVm>();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public IOrderedEnumerable<StockBalanceMultipleVm> GetConsumption(string store, DateTime? start, DateTime? end, string category,string subcategory, string item, bool includeVoid)
        {
            DateTime startDate = (DateTime)start;
            DateTime endDate = (DateTime)end;
            int min = 0;

            if (String.IsNullOrEmpty(store))
            {
                return _stockBalances.OrderBy(x => x.AccountCode); 
            }

            var storeItemAssignments = db.StoreItemAssignments.Where(i => i.StoreCode == store).AsQueryable();

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
                
                var itemBinCardLines = assignment.BinCardLines
                    .Where(b => b.Date >= startDate && b.Date <= endDate)
                    .Where(x => x.ReferenceDoc.ToUpper().Contains("Issue".ToUpper()) || x.ReferenceDoc.ToUpper().Contains("Return".ToUpper())).ToList();

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
                        if(balance.ReferenceDoc.Contains("Return"))
                        {
                            balance.IssuedTPrice = 0-balance.IssuedTPrice;
                            balance.ReceivedQTY = 0-balance.ReceivedQTY;
                        }

                        var stocBalance = new StockBalanceBinCard
                        {
                            Id = balance.BinCardLineID,
                            Date = balance.Date,
                            ReferenceDoc = balance.ReferenceDoc,
                            IssuedQTY = balance.IssuedQTY,
                            IssuedUPrice = balance.IssuedUPrice,
                            IssuedTPrice = balance.IssuedTPrice,

                            ReceivedQTY = balance.ReceivedQTY,
                            ReceivedUPrice = balance.ReceivedUPrice,
                            ReceivedTPrice = balance.ReceivedTPrice,
                            IssuedOrReceivedQty= balance.ReceivedQTY == 0 ? balance.IssuedQTY : balance.ReceivedQTY,
                            IssuedOrReceivedUPrice = balance.ReceivedQTY==0? balance.IssuedUPrice: balance.ReceivedUPrice,
                            BalanceQTY = balance.BalanceQTY,
                            BalanceUPrice = balance.BalanceUPrice,
                            BalanceTPrice = balance.BalanceTPrice,
                            AveragePrice = balance.BalanceUPrice,
                            OriginalRef = balance.Reference,
                            SupplierName=balance.SupplierName,
                            IssuedTo= balance.IssuedTo!=null?InventoryPrintHelper.WorkUnitName(int.Parse(balance.IssuedTo)):""

                        };

                        stockBalanceMultipleVm.StockBalanceBinCards.Add(stocBalance);
                    }
                }

                #endregion

                stockBalanceMultipleVm.GrandTotal =
                    stockBalanceMultipleVm.StockBalanceBinCards.Sum(x => x.IssuedOrReceivedUPrice);
                _stockBalances.Add(stockBalanceMultipleVm);
            }
            
            return _stockBalances.OrderBy(x=>x.AccountCode);
        }

    }
}
