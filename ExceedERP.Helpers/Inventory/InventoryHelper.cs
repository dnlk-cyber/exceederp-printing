using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Helpers.Construction;

namespace ExceedERP.Helpers.Inventory
{
    public  class InventoryHelper
    {
        public static List<string> GetStoreByBusinessUnit(int businessunitid)
        {
            ExceedDbContext db = new ExceedDbContext();           
            return db.InventoryStores.Where(x => x.BusinessUnitId == businessunitid).Select(x=>x.StoreCode).ToList();
        }
        public static string GetStoreKeeper(string storecode)
        {
            ExceedDbContext db = new ExceedDbContext();
            string skeeper = "";
            var record = db.StoreKeeperAssignments.FirstOrDefault(x => x.StoreCode == storecode);
            if (record != null) skeeper = record.StoreKeeper;
            return skeeper;
        }

        public static int GetInventoryPeriodFromDate(DateTime date)
        {
            ExceedDbContext db = new ExceedDbContext();
            int periodid = 0;
            var period = db.InventoryPeriods
                .Where(x => x.DateFrom <= date)
                .Where(x => x.DateTo >= date)
                .FirstOrDefault();

            if (period != null)
                periodid = period.InventoryPeriodId;

            return periodid;
        }
        public static int GetDocumentCopy(PrintCopyAttributes docType)
        {
            ExceedDbContext db = new ExceedDbContext();
            int numberOfCopy = 5;
            var copyNumberSetting = db.Configurations
                .Where(x => x.Category == "Inventory")
                .Where(x => x.Reference == "PRINT_COPY_QTY")
                .Where(x => x.Attribute ==docType.ToString()).FirstOrDefault();
            if (copyNumberSetting != null)
            {
                try
                {
                    numberOfCopy = int.Parse(copyNumberSetting.CurrentValue);
                }
                catch (Exception e)
                {
                    //log
                }
            }
            return numberOfCopy;
        }
        public static string GetDocumentCopyList(PrintCopyAttributes docType)
        {
            ExceedDbContext db = new ExceedDbContext();
            string memberListComposed = "";
            List<string> copyList =new List<string>();

            var copyMemberSetting = db.Configurations
                .Where(x => x.Category == "Inventory")
                .Where(x => x.Reference == "PRINT_COPY_MEMBER")
                .Where(x => x.Attribute == docType.ToString())
                .Where(x => x.CurrentValue != "")
                 .Where(x => x.CurrentValue != null).ToArray();

            for (int i = 0; i < copyMemberSetting.Length; i++)
            {
                if (!string.IsNullOrEmpty(copyMemberSetting[i].CurrentValue))
                    memberListComposed = $"{memberListComposed}{i + 1}) {copyMemberSetting[i].CurrentValue} ";
                    
            }
            return memberListComposed;
        }

        public static int GetStoreBranch(string storecode)
        {
            ExceedDbContext db = new ExceedDbContext();
            int branchid = 0;
            var record = db.InventoryStores.FirstOrDefault(x => x.StoreCode == storecode);
            if (record != null) branchid = record.BranchId;
            return branchid;
        }
        public static int GetStoreBusinessUnit(string storecode)
        {
            ExceedDbContext db = new ExceedDbContext();
            int bizUnitId = 0;
            var record = db.InventoryStores.FirstOrDefault(x => x.StoreCode == storecode);
            if (record != null) bizUnitId = record.BusinessUnitId;
            return bizUnitId;
        }

        public static string GetItemUom(string itemcode)
        {
            ExceedDbContext db = new ExceedDbContext();
            string itemUom = "";
            var record = db.InventoryItems.FirstOrDefault(x => x.ItemCode == itemcode);
            if (record != null) itemUom = record.UoMName;
            return itemUom;
        }
        public static decimal GetStoreItemBalance(string itemcode, string storecode)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal balance = 0;
            var record = db.StoreItemAssignments.Where(x => x.StoreCode == storecode).AsNoTracking().FirstOrDefault(x => x.ItemCode == itemcode);
            if (record != null) balance = record.Balance;
            return balance;
        }
        public static decimal GetBranchItemAveragePrice(int branchid, string itemcode)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal averagePrice = 0;
            var record = db.BranchItemAssignments.Where(x => x.BranchId == branchid).FirstOrDefault(x => x.ItemCode == itemcode);
            if (record != null) averagePrice = record.AvgPrice;
            return averagePrice;
        }
        public static decimal GetItemAveragePrice(string itemcode, string storecode)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal avgPrice = 0;


            #region get average price
            var inventoryItem = db.InventoryItems.Find(itemcode);

            var pricesetting = db.AveragePriceSettings.FirstOrDefault();
            if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
            {
                if (inventoryItem != null)
                    avgPrice = inventoryItem.AvgPrice;
            }
            else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
            {
                int storeBranch = GetStoreBranch(storecode);
                if (inventoryItem != null)
                {
                    var branchPrice = GetBranchItemAveragePrice(storeBranch, itemcode);

                    avgPrice = branchPrice;
                }
            }
            else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
            {
                var storeItemAssignment = db.StoreItemAssignments.Where(x => x.StoreCode == storecode).FirstOrDefault(x => x.ItemCode == itemcode);
                if (storeItemAssignment != null)
                {
                    var storePrice = storeItemAssignment.AvgPrice;
                    avgPrice = storePrice;
                }
            }

            #endregion


            return avgPrice;
        }

        public static void DefineBranchItems(int id)
        {
            ExceedDbContext db = new ExceedDbContext();
            var branchs = db.Branch.Where(x => x.BranchId == id).ToList();
            var stores = db.InventoryStores.ToList();
            foreach (var branch in branchs)
            {
                var branchStores = stores.Where(x => x.BranchId == branch.BranchId).Select(x => x.StoreCode).ToList();
                var branchStoreItems = db.StoreItemAssignments.Where(x => branchStores.Contains(x.StoreCode)).ToList();
                foreach (var item in branchStoreItems)
                {
                    db = new ExceedDbContext();
                    var item1 = item;
                    var exist = db.BranchItemAssignments.Where(x=>x.BranchId==id).Where(x => x.ItemCode == item1.ItemCode).ToList();
                    if (!exist.Any())
                    {
                        #region define branch item

                        var branchItem = new BranchItemAssignment
                        {
                            BranchId = branch.BranchId,
                            ItemCategoryCode = item.ItemCategoryCode,
                            ItemCode = item.ItemCode,
                            Balance = 0,
                            AvgPrice = 0
                        };
                        db.BranchItemAssignments.Add(branchItem);
                        db.SaveChanges();
                        #endregion
                    }
                }
            }

        }
        public static void UpdateAllBranchItems(int id)
        {
            ExceedDbContext db = new ExceedDbContext();
            DefineBranchItems(id);
            var branch = db.Branch.Find(id);
            var branchItems = db.BranchItemAssignments.ToList();
            var storeitems = db.StoreItemAssignments.ToList();
            if (branch != null)
            {
                var brachStores = db.InventoryStores.Where(x => x.BranchId == branch.BranchId).Select(x => x.StoreCode).ToList();
                var branchStoreItems = storeitems.Where(x => brachStores.Contains(x.StoreCode)).ToList();
                var selectedbranchItems = branchItems.Where(x => x.BranchId == branch.BranchId).ToList();
                foreach (var branchItem in selectedbranchItems)
                {
                    db = new ExceedDbContext();
                    decimal avg = 0;
                    decimal price = 0;
                    decimal qty = 0;

                    var bitem = branchItem;
                    var itemInstances = branchStoreItems.Where(x => x.ItemCode == bitem.ItemCode).ToList();

                    qty = itemInstances.Sum(x => x.Balance);

                    foreach (var item in itemInstances)
                    {
                        price = price + item.Balance * item.AvgPrice;
                    }
                    if (qty != 0)
                    {
                        avg = price / qty;
                    }

                    branchItem.Balance = qty;
                    branchItem.AvgPrice = avg;
                    db.Entry(branchItem).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

        }
        public static void UpdateAllInventoryItems()
        {
            ExceedDbContext db = new ExceedDbContext();
            var inventoryItems = db.InventoryItems.ToList();
            var storeitems = db.StoreItemAssignments.ToList();
            foreach (var item in inventoryItems)
            {
                db = new ExceedDbContext();
                item.Balance = storeitems.Where(x => x.ItemCode == item.ItemCode).Sum(x => x.Balance);
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }

        }

        public static void CreateBeginning(string storecode)
        {
            ExceedDbContext db = new ExceedDbContext();
            var storeitems = db.StoreItemAssignments.Where(x => x.StoreCode == storecode).AsNoTracking().AsQueryable();
            foreach (var item in storeitems)
            {
                db = new ExceedDbContext();
                var exist = db.BinCardLines.Where(x => x.StoreItemAssignmentID == item.StoreItemAssignmentID).ToList();
                if (!exist.Any())
                {
                    #region bin card line

                    BinCardLine binCardLine = new BinCardLine
                    {
                        StoreItemAssignment = item,
                        ReferenceDoc = "Beginning",
                        Date = new DateTime(2018, 07, 01),
                        BalanceQTY = item.Balance,
                        BalanceUPrice = item.AvgPrice,
                        StoreCode = storecode,
                        TransactionType = BinCardTransactionTypes.NA,
                        StatusType = BinCardTransactionStatus.NA,
                    };
                    binCardLine.BalanceTPrice = item.AvgPrice * binCardLine.BalanceQTY;
                    db.BinCardLines.Add(binCardLine);
                    db.SaveChanges();

                    #endregion
                }
            }
        }

        ///construction ///

        private static ConstructionInputs inputs;
        public static string GetItemName(string code)
        {
            var db = new ExceedDbContext();
            var material = new InventoryItem();
            var temp = db.InventoryItems.FirstOrDefault(x => x.ItemCode == code);
            if (temp != null)
            {
                material = temp;
            }
            return material.Name;

        }
        public static decimal GetUnitPrice(string itemCode)
        {
            var db = new ExceedDbContext();
            var price = new decimal();
            var currentItem = db.MaterialPriceIndexIndices.FirstOrDefault(material => material.ItemCode == itemCode);
            if (currentItem != null)
            {
                price = currentItem.ApprovedCost;
            }
            return price;
        }
        public static string GetUnitOfMeasurment(string itemCode)
        {
            var db = new ExceedDbContext();
            var items = db.InventoryItems.FirstOrDefault(x => x.ItemCode == itemCode);
            if (items != null)
            {
                return items.UoMName;
            }
            return "";
        }
        public static string GetItemUnitOfMeasurnment(string code)
        {
            var db = new ExceedDbContext();
            var material = new InventoryItem();
            var temp = db.InventoryItems.FirstOrDefault(x => x.ItemCode == code);
            if (temp != null)
            {
                material = temp;
            }
            return material.UoMName;

        }
        public static string GetEquipmentName(Guid fleetsId)
        {
            var db = new ExceedDbContext();
            //var equ = new InternalExternalEquipmentViewModel();
            //var items = GetInternalExternalEquipmentViewModel();
            var equ = new Fleets();
            var temp = db.Fleets.FirstOrDefault(e => e.FleetsID == fleetsId);
            if (temp != null)
            {
                equ = temp;
            }
            return equ.EquipmentName;
            //ViewData["Equipment"] = items.Select(p => new { AssetNumber = p.AssetNumber, Name = p.Name });
            //return Json(items.Select(p => new { AssetNumber = p.AssetNumber, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        private static List<InternalExternalEquipmentViewModel> GetInternalExternalEquipmentViewModel()
        {
            var db = new ExceedDbContext();
            List<InternalExternalEquipmentViewModel> views = new List<InternalExternalEquipmentViewModel>();
            InternalExternalEquipmentViewModel view = new InternalExternalEquipmentViewModel();
            var internalList = db.Fleets.Where(x => x.Status == "Active").AsQueryable();
            //var externalList = db.RentToExternals.AsQueryable();

            foreach (var i in internalList)
            {
                view = new InternalExternalEquipmentViewModel();
                view.AssetNumber = i.FleetsID;
                view.Name = i.EquipmentName;
                view.Model = i.Model;
                view.Year = i.ManufacturerYear;
                view.Type = "Internal";
                view.EquipmentTypeID = i.EquipmentTypeID;
                view.PurchaseCost = i.PurchasingCost;
                views.Add(view);
            }
            //foreach (var i in externalList)
            //{
            //    view = new InternalExternalEquipmentViewModel();
            //    //view.AssetNumber = i.RentOtherSuppliersID;
            //    view.Name = i.Description;
            //    view.Type = "External";
            //    //view.EquipmentTypeID = i.EquipmentTypeID;
            //    // view.Year = i.ManufacturedYear;
            //    views.Add(view);
            //}
            var list = views.ToList();
            return list;
        }


        // 
        public static decimal GetAverageUnitPrice(int branchid, string itemCode)
        {
            var db = new ExceedDbContext();
            //var currentProjet = new ProjectInformation();
            var avaragePrice = new decimal();
            //var temp = db.ProjectInformation.Find(projectId);
            //if (temp != null)
            //{
            //    currentProjet = temp;
            //}
            // get the amount of a given item received for this project in the given period
            var receivedGoods = db.GoodReceiveItems.Where(item => item.GoodReceive.IsOnlineApproved &&
                                                           !item.GoodReceive.IsVoid &&
                                                           item.ItemCode == itemCode &&
                                                           item.GoodReceive.BranchId == branchid
                                                        ).ToList();
            if (receivedGoods.Any())
            {
                avaragePrice = receivedGoods.Average(g => g.UnitPrice);
            }
            return avaragePrice;
        }
        public static decimal GetDeliveredQuantityTillPrevMonth(int branchId, int periodId, string itemCode)
        {
            var db = new ExceedDbContext();
            //var currentProjet = new ProjectInformation();
            var receivedQuantity = new decimal();
            //var temp = db.ProjectInformation.Find(projectId);
            //if (temp != null)
            //{
            //    currentProjet = temp;
            //}
            var currentPeriod = db.InventoryPeriods.FirstOrDefault(p => p.InventoryPeriodId == periodId);
            var receivedGoods = new List<GoodReceiveItem>();
            var transferedInGoods = new List<ItemTransferItemReceive>();
            if (currentPeriod != null)
            {
                receivedGoods = db.GoodReceiveItems.Where(item => item.ItemCode == itemCode &&
                                                          item.GoodReceive.IsOnlineApproved &&
                                                          !item.GoodReceive.IsVoid &&
                                                          item.GoodReceive.BranchId == branchId &&
                                                          item.GoodReceive.ReceiveDate < currentPeriod.DateFrom
                                                       ).ToList();
                // find transfered quantity
                transferedInGoods = db.ItemTransferItemReceive.Where(item => item.ItemCode == itemCode &&
                                                         item.ItemTransferReceive.IsOnlineApproved &&
                                                         !item.ItemTransferReceive.IsVoid &&
                                                         item.ItemTransferReceive.BranchId == branchId &&
                                                         item.ItemTransferReceive.ReceiveDate < currentPeriod.DateFrom
                                                      ).ToList();
            }
            if (receivedGoods.Any())
            {
                receivedQuantity = receivedGoods.Sum(g => g.Quantity);
            }
            if (transferedInGoods.Any())
            {
                receivedQuantity = receivedQuantity + transferedInGoods.Sum(itm => itm.ReceivedQuantity);
            }

            return receivedQuantity;
        }
        public static decimal GetConsumedQuanityTillPrevMonth(int branchId, int periodId, string itemCode)
        {
            var db = new ExceedDbContext();
            //var currentProjet = new ProjectInformation();
            var issuedQuantity = new decimal();
            var returneQuantity = new decimal();
            var transferedOutQuantity = new decimal();
            //var temp = db.ProjectInformation.Find(projectId);
            //if (temp != null)
            //{
            //    currentProjet = temp;
            //}
            var currentPeriod = db.InventoryPeriods.FirstOrDefault(p => p.InventoryPeriodId == periodId);
            if (currentPeriod != null)
            {
                var issuedItems = db.IssueItems.Where(item => item.Issue.BranchId == branchId &&
                                                    item.Issue.IssueDate < currentPeriod.DateFrom &&
                                                    item.ItemCode == itemCode &&
                                                    item.Issue.IsOnlineApproved &&
                                                    !item.Issue.IsVoid
                                                    ).ToList();

                var transferedOutItems = db.ItemTransferItems.Where(item => item.ItemTransfer.BranchId == branchId &&
                                                            item.ItemTransfer.ReceiveDate < currentPeriod.DateFrom &&
                                                            item.ItemCode == itemCode &&
                                                            item.ItemTransfer.IsOnlineApproved &&
                                                            !item.ItemTransfer.IsVoid
                                                            ).ToList();

                var returnedItems = db.StoreReturnItemss.Where(item => item.StoreReturn.ForBranchId == branchId &&
                                                        item.StoreReturn.ReturnDate < currentPeriod.DateFrom &&
                                                        item.ItemCode == itemCode &&
                                                        item.StoreReturn.IsOnlineApproved &&
                                                        !item.StoreReturn.IsVoid
                                                        ).ToList();
                if (issuedItems.Any())
                {
                    issuedQuantity = issuedItems.Sum(i => i.IssuedQuantity);
                }
                if (returnedItems.Any())
                {
                    returneQuantity = returnedItems.Sum(i => i.Quantity);
                }
                if (transferedOutItems.Any())
                {
                    transferedOutQuantity = transferedOutItems.Sum(i => i.ReceivedQuantity);
                }


            }
            return issuedQuantity - returneQuantity - transferedOutQuantity;
        }
        // material quantity and cost for the current month
        public static decimal GetDeliveredQuantityForThisMonth(int branchId, int periodId, string itemCode)
        {
            var db = new ExceedDbContext();
            //var currentProjet = new ProjectInformation();
            var receivedQuantity = new decimal();
            var transferedInQuantity = new decimal();
            //var temp = db.ProjectInformation.Find(projectId);
            //if (temp != null)
            //{
            //    currentProjet = temp;
            //}
            var currentPeriod = db.InventoryPeriods.FirstOrDefault(p => p.InventoryPeriodId == periodId);
            var receivedGoods = new List<GoodReceiveItem>();
            var transferedInItems = new List<ItemTransferItemReceive>();
            if (currentPeriod != null)
            {
                receivedGoods = db.GoodReceiveItems.Where(item => item.ItemCode == itemCode &&
                                                          item.GoodReceive.IsOnlineApproved &&
                                                          !item.GoodReceive.IsVoid &&
                                                          item.GoodReceive.BranchId == branchId &&
                                                          item.GoodReceive.GLPeriodId == currentPeriod.InventoryPeriodId
                                                       ).ToList();

                transferedInItems = db.ItemTransferItemReceive.Where(item => item.ItemCode == itemCode &&
                                                          item.ItemTransferReceive.IsOnlineApproved &&
                                                          !item.ItemTransferReceive.IsVoid &&
                                                          item.ItemTransferReceive.BranchId == branchId &&
                                                          item.ItemTransferReceive.GLPeriodId == currentPeriod.InventoryPeriodId
                                                       ).ToList();
            }
            if (receivedGoods.Any())
            {
                receivedQuantity = receivedGoods.Sum(g => g.Quantity);
            }
            if (transferedInItems.Any())
            {
                transferedInQuantity = transferedInItems.Sum(i => i.ReceivedQuantity);
            }
            return receivedQuantity + transferedInQuantity;
        }
        public static decimal GetConsumedQuanityForThisMonth(int branchId, int periodId, string itemCode)
        {
            var db = new ExceedDbContext();
            //var currentProjet = new ProjectInformation();
            var issuedQuantity = new decimal();
            var transferedOutQuantity = new decimal();
            var returnedQuantity = new decimal();
            //var temp = db.ProjectInformation.Find(projectId);
            //if (temp != null)
            //{
            //    currentProjet = temp;
            //}
            var currentPeriod = db.InventoryPeriods.FirstOrDefault(p => p.InventoryPeriodId == periodId);
            if (currentPeriod != null)
            {
                // issue items
                var issuedItems = db.IssueItems.Where(item => item.Issue.BranchId == branchId &&
                                                item.Issue.GLPeriodId == currentPeriod.InventoryPeriodId &&
                                                item.Issue.IsOnlineApproved &&
                                                !item.Issue.IsVoid &&
                                                item.ItemCode == itemCode
                                                ).ToList();
                if (issuedItems.Any())
                {
                    issuedQuantity = issuedItems.Sum(i => i.IssuedQuantity);
                }
                // find the returned quantity
                var returnedItems = db.StoreReturnItemss.Where(item => item.StoreReturn.ForBranchId == branchId &&
                                                        item.StoreReturn.GLPeriodId == currentPeriod.InventoryPeriodId &&
                                                        item.ItemCode == itemCode &&
                                                        item.StoreReturn.IsOnlineApproved &&
                                                        !item.StoreReturn.IsVoid
                                                        ).ToList();
                returnedQuantity = returnedItems.Sum(itm => itm.Quantity);

                // tranfered out 
                var transferedOutItems = db.ItemTransferItems.Where(item => item.ItemTransfer.BranchId == branchId &&
                                                                item.ItemTransfer.GLPeriodId == currentPeriod.InventoryPeriodId &&
                                                                item.ItemTransfer.IsOnlineApproved &&
                                                                !item.ItemTransfer.IsVoid &&
                                                                item.ItemCode == itemCode
                                                                ).ToList();
                transferedOutQuantity = transferedOutItems.Sum(i => i.ReceivedQuantity);

            }
            return issuedQuantity - transferedOutQuantity - returnedQuantity;
        }
        public static decimal GetTransferedInForThisMonth(int branchId, int periodId, string itemCode)
        {
            var db = new ExceedDbContext();
            var transferedInQuantity = new decimal();

            var currentPeriod = db.InventoryPeriods.FirstOrDefault(p => p.InventoryPeriodId == periodId);
            var transferedInItems = new List<ItemTransferItemReceive>();

            if (currentPeriod != null)
            {
                transferedInItems = db.ItemTransferItemReceive.Where(item => item.ItemCode == itemCode &&
                                                         item.ItemTransferReceive.IsOnlineApproved &&
                                                         !item.ItemTransferReceive.IsVoid &&
                                                         item.ItemTransferReceive.BranchId == branchId &&
                                                         item.ItemTransferReceive.GLPeriodId == currentPeriod.InventoryPeriodId
                                                      ).ToList();
                if (transferedInItems.Any())
                {
                    transferedInQuantity = transferedInItems.Sum(i => i.ReceivedQuantity);
                }
            }

            return transferedInQuantity;
        }
        public static decimal GetTransferedOoutForThisMonth(int branchId, int periodId, string itemCode)
        {
            var db = new ExceedDbContext();
            var transferedOutQuantity = new decimal();

            var currentPeriod = db.InventoryPeriods.FirstOrDefault(p => p.InventoryPeriodId == periodId);
            var transferedInItems = new List<ItemTransferItemReceive>();

            if (currentPeriod != null)
            {
                var transferedOutItems = db.ItemTransferItems.Where(item => item.ItemTransfer.BranchId == branchId &&
                                                                item.ItemTransfer.GLPeriodId == currentPeriod.InventoryPeriodId &&
                                                                item.ItemTransfer.IsOnlineApproved &&
                                                                !item.ItemTransfer.IsVoid &&
                                                                item.ItemCode == itemCode
                                                                ).ToList();
                transferedOutQuantity = transferedOutItems.Sum(i => i.ReceivedQuantity);
            }

            return transferedOutQuantity;
        }
    }
}
