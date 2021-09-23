using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.HR;
using System.Collections.Generic;
using System.Linq;

namespace ExceedERP.Helpers.Inventory
{
    public  class StaticDistributionHelper
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        private readonly AccountHelper _accountHelper = new AccountHelper();
        // inventory
        public  void UpdateGoodReceiveDistribution(GoodReceive record)
        {
            var distributions = _db.GoodReceiveDistributions.Where(x => x.GoodReceiveId == record.GoodReceiveId)
                .ToList();
            _db.GoodReceiveDistributions.RemoveRange(distributions);
            _db.SaveChanges();

            if (record.ReceiveType.Type == GoodReceiveType.Other)
            {
                DonationGoodReceiveDistribution(record);
            }
            else
            {
                PurchaseGoodReceiveDistribution(record);
            }
        }
        public  void PurchaseGoodReceiveDistribution(GoodReceive record)
        {
            List<GoodReceiveDistribution> distributions = new List<GoodReceiveDistribution>();

            var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == record.SupplierId);

            if (record.GoodReceiveItem != null)
            {
                #region debit stock

                
                foreach (var line in record.GoodReceiveItem)
                {
                    if (line.SubTotal != 0)
                    {
                        var item = _db.InventoryItems.FirstOrDefault(it => it.ItemCode == line.ItemCode);
                        if (item != null)
                        {
                            AccountHelper accountHelper = new AccountHelper();
                            string acc = item.ItemCategory.AccountCode;
                            string[] account = acc.Split('-');
                            string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                            string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                            string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                            string subAccountSeG = HrHelper.GetBranchSubAccount(record.BranchId);
                            string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,costCenterSeG, subAccountSeG);

                            var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);
                            if (receiveDistribution != null)
                            {
                                receiveDistribution.Debit += line.SubTotal;
                                receiveDistribution.Credit = 0;
                                receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                            }
                            else
                            {

                                var defaultDistribution = new GoodReceiveDistribution()
                                {
                                    AccountCode = newacc,
                                    AccountDesc = accountHelper.GetAccountDesc(newacc),
                                    Debit = line.SubTotal,
                                    Credit = 0,
                                    GoodReceiveId = line.GoodReceiveId,
                                    GoodReceiveItemIDTrackBack = line.GoodReceiveItemID
                                };
                               distributions.Add(defaultDistribution);
                            }
                        }

                    }
                }

                #endregion

                #region debit tax

                foreach (var line in record.GoodReceiveItem)
                {
                    if (line.TaxAmount != 0)
                    {
                        var tax = _db.GLTaxRates.Find(line.GLTaxRateID);
                        if (tax != null)
                        {
                            string acc = tax.ApAccountCode;
                            //string[] account = acc.Split('-');
                            //string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                            //string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                            //string costCenterSeG =account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                            //string subAccountSeG = HrHelper.GetBranchSubAccount(record.BranchId);
                            //string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,
                            //    costCenterSeG, subAccountSeG);

                            var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == acc);
                            if (receiveDistribution != null)
                            {
                                receiveDistribution.Debit += line.TaxAmount;
                                receiveDistribution.Credit = 0;
                                receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                            }
                            else
                            {

                                var defaultDistribution = new GoodReceiveDistribution()
                                {
                                    AccountCode = acc,
                                    AccountDesc = _accountHelper.GetAccountDesc(acc),
                                    Debit = line.SubTotal,
                                    Credit = 0,
                                    GoodReceiveId = line.GoodReceiveId,
                                    GoodReceiveItemIDTrackBack = line.GoodReceiveItemID
                                };
                                distributions.Add(defaultDistribution);
                            }

                           
                        }

                    }
                }

                #endregion

                if (supplier != null && supplier.IsCredit)
                {
                    #region Credit supplier

                    if (record.GoodReceiveItem.Sum(x => x.Total) != 0)
                    {
                        var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == supplier.AccountCode);
                        if (receiveDistribution != null)
                        {
                            receiveDistribution.Debit = 0;
                            receiveDistribution.Credit += record.GoodReceiveItem.Sum(x => x.Total);
                            receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                        }
                        else
                        {

                            var defaultDistribution = new GoodReceiveDistribution()
                            {
                                AccountCode = supplier.AccountCode,
                                AccountDesc = _accountHelper.GetAccountDesc(supplier.AccountCode),
                                Debit = 0,
                                Credit = record.GoodReceiveItem.Sum(x => x.Total),
                                GoodReceiveId = record.GoodReceiveId
                            };
                            distributions.Add(defaultDistribution);
                        }
                        
                    }

                    #endregion
                }
                else 
                {
                    #region Credit common supplier

                    var commonSupplier = _db.ApSetupCommonSuppliers.FirstOrDefault();
                    if (record.GoodReceiveItem.Sum(x => x.Total) != 0)
                    {
                        if (commonSupplier != null)
                        {
                            var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == commonSupplier.AccountCode);
                            if (receiveDistribution != null)
                            {
                                receiveDistribution.Debit = 0;
                                receiveDistribution.Credit += record.GoodReceiveItem.Sum(x => x.Total);
                                receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                            }
                            else
                            {

                                var defaultDistribution = new GoodReceiveDistribution()
                                {
                                    AccountCode = commonSupplier.AccountCode,
                                    AccountDesc = _accountHelper.GetAccountDesc(commonSupplier.AccountCode),
                                    Debit = 0,
                                    Credit = record.GoodReceiveItem.Sum(x => x.Total),
                                    GoodReceiveId = record.GoodReceiveId
                                };
                                distributions.Add(defaultDistribution);
                            }

                           
                        }
                    }

                    #endregion
                }
            }

            foreach (var distribution in distributions)
            {
                _db.GoodReceiveDistributions.Add(distribution);
                _db.SaveChanges();
            }

        }      
        public  void DonationGoodReceiveDistribution(GoodReceive record)
        {
            var revenueAccount = _db.RevenueAccounts.FirstOrDefault(x => x.ApplyToInventory);

            if (record.GoodReceiveItem!=null)
            {
                #region debit stock

               
                foreach (var line in record.GoodReceiveItem)
                {
                    if (line.SubTotal != 0)
                    {
                        var item = _db.InventoryItems.FirstOrDefault(it => it.ItemCode == line.ItemCode);
                        if (item != null)
                        {
                            AccountHelper accountHelper = new AccountHelper();
                            string acc = item.ItemCategory.AccountCode;
                            string[] account = acc.Split('-');
                            string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                            string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                            string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                            string subAccountSeG = HrHelper.GetBranchSubAccount(record.BranchId);
                            string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,
                                costCenterSeG, subAccountSeG);

                            GoodReceiveDistribution distribution = new GoodReceiveDistribution
                            {
                                AccountCode = newacc,
                                AccountDesc = accountHelper.GetAccountDesc(newacc),
                                Debit = line.SubTotal,
                                Credit = 0,
                                GoodReceiveId = line.GoodReceiveId,
                                GoodReceiveItemIDTrackBack = line.GoodReceiveItemID
                            };
                            _db.GoodReceiveDistributions.Add(distribution);
                            _db.SaveChanges();
                        }

                    }
                }

                #endregion

                #region credit tax payable

                foreach (var line in record.GoodReceiveItem)
                {
                    if (line.TaxAmount != 0)
                    {
                        var tax = _db.GLTaxRates.Find(line.GLTaxRateID);
                        if (tax != null)
                        {
                            string acc = tax.ApAccountCode;
                            //string[] account = acc.Split('-');
                            //string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                            //string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                            //string costCenterSeG =account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                            //string subAccountSeG = HrHelper.GetBranchSubAccount(record.BranchId);
                            //string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,
                            //    costCenterSeG, subAccountSeG);

                            GoodReceiveDistribution distribution = new GoodReceiveDistribution
                            {
                                AccountCode = acc,
                                AccountDesc = _accountHelper.GetAccountDesc(acc),
                                Debit = 0,
                                Credit = line.TaxAmount,
                                GoodReceiveId = line.GoodReceiveId,
                                GoodReceiveItemIDTrackBack = line.GoodReceiveItemID
                            };
                            _db.GoodReceiveDistributions.Add(distribution);
                            _db.SaveChanges();
                        }

                    }
                }

                #endregion

                #region Credit revenue

                if (record.GoodReceiveItem.Sum(x => x.SubTotal) != 0)
                {
                    if (revenueAccount != null)
                    {
                        GoodReceiveDistribution distribution = new GoodReceiveDistribution
                        {
                            AccountCode = revenueAccount.InventoryAccountCode,
                            AccountDesc = _accountHelper.GetAccountDesc(revenueAccount.InventoryAccountCode),
                            Debit = 0,
                            Credit = record.GoodReceiveItem.Sum(x => x.SubTotal),
                            GoodReceiveId = record.GoodReceiveId
                        };

                        _db.GoodReceiveDistributions.Add(distribution);
                        _db.SaveChanges();
                    }
                }

                #endregion
            }

        }
        public  void UpdateIssueDistribution(Issue record)
        {
            Dictionary<string, string> checkAccount = new Dictionary<string, string>();
            List<IssueDistribution> distributions = new List<IssueDistribution>();

            var issueDistributions = _db.IssueDistributions.Where(x => x.IssueID == record.IssueID).ToList();
            _db.IssueDistributions.RemoveRange(issueDistributions);
            _db.SaveChanges();

            int branch = 0;

            var storereq = _db.StoreRequisitions.FirstOrDefault(x => x.StoreRequisitionID == record.StoreRequisitionID);
            if (storereq != null)
            {
                if (storereq.IsForOther)
                {
                    if (storereq.ForBranchId != null) branch = (int) storereq.ForBranchId;
                    else
                    {
                        branch = storereq.BranchId;
                    }
                }
                else
                {
                    branch = storereq.BranchId;
                }
            }

            if (record.IssueItems != null)
            {
                #region debit expense

                
                foreach (var line in record.IssueItems)
                {
                    var item = _db.InventoryItems.FirstOrDefault(it => it.ItemCode == line.ItemCode);
                    if (item != null)
                    {
                        var itemCategory = _db.ItemCategorys.Find(item.ItemCategoryCode);

                        if (itemCategory != null)
                        {
                           
                            string acc = itemCategory.IssueAccountCode;
                            string[] account = acc.Split('-');
                            string acccountSeG = account[_accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                            string locationSeG = account[_accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                            string costCenterSeG = HrHelper.GetWorkUnitCostCenter(record.OrgStructureId,branch);
                            string subAccountSeG = HrHelper.GetBranchSubAccount(branch);
                            string newacc = _accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,costCenterSeG, subAccountSeG);

                            if (line.Total != 0 && checkAccount.ContainsKey(newacc))
                            {
                                IssueDistribution iDistribution =distributions.FirstOrDefault(x => x.AccountCode == newacc);

                                if (iDistribution != null)
                                {
                                    iDistribution.AccountCode = newacc;
                                    iDistribution.AccountDesc = _accountHelper.GetAccountDesc(newacc);
                                    iDistribution.Debit += line.Total;
                                    iDistribution.Credit = 0;
                                    iDistribution.IssueID = line.IssueID;
                                    iDistribution.IssueItemIDTrackBack = line.IssueItemID;
                                }
                            }
                            else if (line.Total != 0 && !checkAccount.ContainsKey(newacc))
                            {
                                IssueDistribution distribution = new IssueDistribution
                                {
                                    AccountCode = newacc,
                                    AccountDesc = _accountHelper.GetAccountDesc(newacc),
                                    Debit = line.Total,
                                    Credit = 0,
                                    IssueID = line.IssueID,
                                    IssueItemIDTrackBack = line.IssueItemID
                                };
                                distributions.Add(distribution);
                                checkAccount.Add(newacc, line.ItemCode);
                            }
                        }
                    }
                }
                foreach (IssueDistribution issueDistribution in distributions)
                {
                    _db.IssueDistributions.Add(issueDistribution);
                    _db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion

                #region Credit stock

                if (record.IssueItems.Sum(x => x.Total) != 0)
                {
                    foreach (var items in record.IssueItems)
                    {
                        var inventory = _db.InventoryItems.Find(items.ItemCode);
                        if (inventory != null)
                        {
                            AccountHelper accountHelper = new AccountHelper();
                            string accCode = inventory.ItemCategory.AccountCode;
                            string[] account = accCode.Split('-');
                            string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                            string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                            string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                            string subAccountSeG = HrHelper.GetBranchSubAccount(branch);
                            string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);

                            if (items.Total != 0 && checkAccount.ContainsKey(newacc))
                            {
                                var iDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

                                if (iDistribution != null)
                                {
                                    iDistribution.AccountCode = newacc;
                                    iDistribution.AccountDesc = accountHelper.GetAccountDesc(newacc);
                                    iDistribution.Debit = 0;
                                    iDistribution.Credit += items.Total;
                                    iDistribution.IssueID = items.IssueID;
                                    iDistribution.IssueItemIDTrackBack = items.IssueItemID;
                                }
                            }
                            else if (items.Total != 0 && !checkAccount.ContainsKey(newacc))
                            {
                                IssueDistribution distribution = new IssueDistribution
                                {
                                    AccountCode = newacc,
                                    AccountDesc = accountHelper.GetAccountDesc(newacc),
                                    Debit = 0,
                                    Credit = items.Total,
                                    IssueID = items.IssueID,
                                    IssueItemIDTrackBack = items.IssueItemID
                                };
                                distributions.Add(distribution);
                                checkAccount.Add(newacc, items.ItemCode);
                            }
                        }
                    }
                    foreach (var issueDistribution in distributions)
                    {
                        _db.IssueDistributions.Add(issueDistribution);
                        _db.SaveChanges();
                    }
                    distributions.Clear();
                    checkAccount.Clear();

                }

                #endregion
            }

        }
        public  void UpdateStoreReturnDistribution(StoreReturn record)
        {
           
            Dictionary<string, string> checkAccount = new Dictionary<string, string>();
            List<StoreReturnDistribution> distributions = new List<StoreReturnDistribution>();

            var returnDistributions = _db.StoreReturnDistributions.Where(x => x.StoreReturnID == record.StoreReturnID).ToList();
            _db.StoreReturnDistributions.RemoveRange(returnDistributions);
            _db.SaveChanges();

            int branch = 0;
            
            if (record != null)
            {
                if (record.IsForOther)
                {
                    if (record.ForBranchId != null) branch = (int)record.ForBranchId;
                    else
                    {
                        branch = InventoryHelper.GetStoreBranch(record.StoreCode); 
                    }
                }
                else
                {
                    branch = InventoryHelper.GetStoreBranch(record.StoreCode);
                }
            }

            if (record != null && record.StoreReturnItems != null)
            {
                #region credit expense

                foreach (var line in record.StoreReturnItems)
                {
                    var item = _db.InventoryItems.FirstOrDefault(it => it.ItemCode == line.ItemCode);
                    if (item != null)
                    {
                        var itemCategory = _db.ItemCategorys.Find(item.ItemCategoryCode);

                        if (itemCategory != null)
                        {
                            AccountHelper accountHelper = new AccountHelper();
                            string acc = itemCategory.IssueAccountCode;
                            string[] account = acc.Split('-');
                            string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                            string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                            string costCenterSeG = HrHelper.GetWorkUnitCostCenter(record.OrgStructureId,branch);
                            string subAccountSeG = HrHelper.GetBranchSubAccount(branch);
                            string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);

                            if (line.Quantity * line.UnitPrice != 0 && checkAccount.ContainsKey(newacc))
                            {
                                StoreReturnDistribution iDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

                                if (iDistribution != null)
                                {
                                    iDistribution.AccountCode = newacc;
                                    iDistribution.AccountDesc = accountHelper.GetAccountDesc(newacc);
                                    iDistribution.Debit = 0;
                                    iDistribution.Credit += (line.Quantity * line.UnitPrice);
                                    iDistribution.StoreReturnID = line.StoreReturnID;
                                }
                            }
                            else if (line.Quantity * line.UnitPrice != 0 && !checkAccount.ContainsKey(newacc))
                            {
                                StoreReturnDistribution distribution = new StoreReturnDistribution
                                {
                                    AccountCode = newacc,
                                    AccountDesc = accountHelper.GetAccountDesc(newacc),
                                    Debit = 0,
                                    Credit = (line.Quantity * line.UnitPrice),
                                    StoreReturnID = line.StoreReturnID,
                                };
                                distributions.Add(distribution);
                                checkAccount.Add(newacc, line.ItemCode);
                            }
                        }
                    }
                }
                foreach (StoreReturnDistribution returnDistribution in distributions)
                {
                    _db.StoreReturnDistributions.Add(returnDistribution);
                    _db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion

                #region debit stock

                if (record.StoreReturnItems.Sum(x => x.Quantity*x.UnitPrice) != 0)
                {
                    foreach (var items in record.StoreReturnItems)
                    {
                        var inventory = _db.InventoryItems.Find(items.ItemCode);
                        if (inventory != null)
                        {
                            string accCode = inventory.ItemCategory.AccountCode;

                            if (items.Quantity * items.UnitPrice != 0 && checkAccount.ContainsKey(accCode))
                            {
                                var iDistribution = distributions.FirstOrDefault(x => x.AccountCode == accCode);

                                if (iDistribution != null)
                                {
                                    iDistribution.AccountCode = accCode;
                                    iDistribution.AccountDesc = _accountHelper.GetAccountDesc(accCode);
                                    iDistribution.Debit += (items.Quantity * items.UnitPrice);
                                    iDistribution.Credit = 0;
                                    iDistribution.StoreReturnID = items.StoreReturnID;
                                }
                            }
                            else if (items.Quantity * items.UnitPrice != 0 && !checkAccount.ContainsKey(accCode))
                            {
                                StoreReturnDistribution distribution = new StoreReturnDistribution
                                {
                                    AccountCode = accCode,
                                    AccountDesc = _accountHelper.GetAccountDesc(accCode),
                                    Debit = (items.Quantity * items.UnitPrice),
                                    Credit = 0,
                                    StoreReturnID = items.StoreReturnID
                                };
                                distributions.Add(distribution);
                                checkAccount.Add(accCode, items.ItemCode);
                            }
                        }
                    }
                    foreach (var returnDistribution in distributions)
                    {
                        _db.StoreReturnDistributions.Add(returnDistribution);
                        _db.SaveChanges();
                    }
                    distributions.Clear();
                    checkAccount.Clear();

                }

                #endregion
            }

        }
        public  void UpdateTransferSendDistribution(ItemTransfer record)
        {
           
            Dictionary<string, string> checkAccount = new Dictionary<string, string>();
            List<ItemTransferDistribution> distributions = new List<ItemTransferDistribution>();

            var records = _db.ItemTransferDistributions.Where(x => x.ItemTransferID == record.ItemTransferID).ToList();
            _db.ItemTransferDistributions.RemoveRange(records);
            _db.SaveChanges();

            //int receiverBranch = InventoryHelper.GetStoreBranch(record.ToStore);
            int senderBranch = InventoryHelper.GetStoreBranch(record.FromStore);

            if (record.ItemTransferItem != null)
            {
                #region debit clearing account

                var clearingAccount = _db.StockClearingAccounts.FirstOrDefault(x => x.Enable);
                if (clearingAccount != null)
                {
                    AccountHelper accountHelper = new AccountHelper();
                    string acc = clearingAccount.AccountCode;
                    string[] account = acc.Split('-');
                    string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                    string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                    string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                    string subAccountSeG = HrHelper.GetBranchSubAccount(senderBranch);
                    string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG,
                        subAccountSeG);

                    decimal clearingAmount = 0;
                    foreach (var items in record.ItemTransferItem)
                    {
                        clearingAmount = clearingAmount + (items.ShippedQuantity* items.UPrice);
                    }

                    if (clearingAmount != 0)
                    {
                        ItemTransferDistribution iDistribution = new ItemTransferDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = accountHelper.GetAccountDesc(newacc),
                            Debit = clearingAmount,
                            Credit = 0,
                            ItemTransferID = record.ItemTransferID
                        };

                        distributions.Add(iDistribution);
                        checkAccount.Add(newacc, "Clearing Account");
                    }


                }
                foreach (ItemTransferDistribution transferDistribution in distributions)
                {
                    _db.ItemTransferDistributions.Add(transferDistribution);
                    _db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion

                #region Credit sender stock

                if (record.ItemTransferItem.Sum(x => x.TPrice) != 0)
                {
                    foreach (var items in record.ItemTransferItem)
                    {
                        var inventory = _db.InventoryItems.Find(items.ItemCode);
                        var senderBranchId = InventoryHelper.GetStoreBranch(record.FromStore);
                        if (inventory != null && senderBranchId != 0)
                        {
                            AccountHelper accountHelper = new AccountHelper();
                            string accCode = inventory.ItemCategory.AccountCode;
                            string[] account = accCode.Split('-');
                            string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                            string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                            string costCenterSeG =account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                            string subAccountSeG = HrHelper.GetBranchSubAccount(senderBranchId);
                            string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,costCenterSeG, subAccountSeG);

                            if (items.TPrice != 0 && checkAccount.ContainsKey(newacc))
                            {
                                var iDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

                                if (iDistribution != null)
                                {
                                    iDistribution.AccountCode = newacc;
                                    iDistribution.AccountDesc = accountHelper.GetAccountDesc(newacc);
                                    iDistribution.Debit = 0;
                                    iDistribution.Credit += (items.ShippedQuantity * items.UPrice);
                                    iDistribution.ItemTransferID = items.ItemTransferID;
                                }
                            }
                            else if (items.TPrice != 0 && !checkAccount.ContainsKey(newacc))
                            {
                                ItemTransferDistribution distribution = new ItemTransferDistribution
                                {
                                    AccountCode = newacc,
                                    AccountDesc = accountHelper.GetAccountDesc(newacc),
                                    Debit = 0,
                                    Credit = (items.ShippedQuantity * items.UPrice),
                                    ItemTransferID = items.ItemTransferID,
                                };
                                distributions.Add(distribution);
                                checkAccount.Add(newacc, items.ItemCode);
                            }
                        }
                    }
                    foreach (var transferDistribution in distributions)
                    {
                        _db.ItemTransferDistributions.Add(transferDistribution);
                        _db.SaveChanges();
                    }
                    distributions.Clear();
                    checkAccount.Clear();
                }

                #endregion
            }
        }
        public  void UpdateTransferReceiveDistribution(ItemTransferReceive record)
        {
           
            Dictionary<string, string> checkAccount = new Dictionary<string, string>();
            List<ItemTransferReceiveDistribution> distributions = new List<ItemTransferReceiveDistribution>();

            var records = _db.ItemTransferReceiveDistribution.Where(x => x.ItemTransferReceiveId == record.ItemTransferReceiveId).ToList();
            _db.ItemTransferReceiveDistribution.RemoveRange(records);
            _db.SaveChanges();

            //int receiverBranch = InventoryHelper.GetStoreBranch(record.ToStore);
            int senderBranch = InventoryHelper.GetStoreBranch(record.FromStore);

            if (record.ItemTransferItemReceive != null)
            {
                #region debit receiver stock

                if (record.ItemTransferItemReceive.Sum(x => x.TPrice) != 0)
                {
                    foreach (var items in record.ItemTransferItemReceive)
                    {
                        var inventory = _db.InventoryItems.Find(items.ItemCode);
                        var receiverBranchId = InventoryHelper.GetStoreBranch(record.ToStore);
                        if (inventory != null && receiverBranchId != 0)
                        {
                            AccountHelper accountHelper = new AccountHelper();
                            string accCode = inventory.ItemCategory.AccountCode;
                            string[] account = accCode.Split('-');
                            string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                            string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                            string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                            string subAccountSeG = HrHelper.GetBranchSubAccount(receiverBranchId);
                            string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,
                                costCenterSeG, subAccountSeG);

                            if (items.TPrice != 0 && checkAccount.ContainsKey(newacc))
                            {
                                var iDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

                                if (iDistribution != null)
                                {
                                    iDistribution.AccountCode = newacc;
                                    iDistribution.AccountDesc = accountHelper.GetAccountDesc(newacc);
                                    iDistribution.Debit += (items.ReceivedQuantity * items.UPrice);
                                    iDistribution.Credit = 0;
                                    iDistribution.ItemTransferReceiveId = items.ItemTransferReceiveId;
                                }
                            }
                            else if (items.TPrice != 0 && !checkAccount.ContainsKey(newacc))
                            {
                                ItemTransferReceiveDistribution distribution = new ItemTransferReceiveDistribution
                                {
                                    AccountCode = newacc,
                                    AccountDesc = accountHelper.GetAccountDesc(newacc),
                                    Debit = (items.ReceivedQuantity * items.UPrice),
                                    Credit = 0,
                                    ItemTransferReceiveId = items.ItemTransferReceiveId,
                                };
                                distributions.Add(distribution);
                                checkAccount.Add(newacc, items.ItemCode);
                            }
                        }
                    }
                    foreach (var transferDistribution in distributions)
                    {
                        _db.ItemTransferReceiveDistribution.Add(transferDistribution);
                        _db.SaveChanges();
                    }
                    distributions.Clear();
                    checkAccount.Clear();
                }

                #endregion

                #region credit clearing account

                var clearingAccount = _db.StockClearingAccounts.FirstOrDefault(x => x.Enable);
                if (clearingAccount != null)
                {
                    AccountHelper accountHelper = new AccountHelper();
                    string acc = clearingAccount.AccountCode;
                    string[] account = acc.Split('-');
                    string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                    string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                    string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                    string subAccountSeG = HrHelper.GetBranchSubAccount(senderBranch);
                    string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG,
                        subAccountSeG);

                    decimal clearingAmount = 0;
                    foreach (var items in record.ItemTransferItemReceive)
                    {
                        clearingAmount = clearingAmount + (items.ReceivedQuantity * items.UPrice);
                    }

                    if (clearingAmount != 0)
                    {
                        ItemTransferReceiveDistribution iDistribution=new ItemTransferReceiveDistribution();
                        iDistribution.AccountCode = newacc;
                        iDistribution.AccountDesc = accountHelper.GetAccountDesc(newacc);
                        iDistribution.Debit = 0;
                        iDistribution.Credit = clearingAmount;
                        iDistribution.ItemTransferReceiveId = record.ItemTransferReceiveId;
                        distributions.Add(iDistribution);
                        checkAccount.Add(newacc, "Clearing Account");
                    }


                }
                foreach (ItemTransferReceiveDistribution transferDistribution in distributions)
                {
                    _db.ItemTransferReceiveDistribution.Add(transferDistribution);
                    _db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion
                
            }
        }
        public void UpdateGoodSellingDistribution(GoodSellingAndLending record)
        {
            //throw new NotImplementedException();
        }

        //generate distribution data
        public  void GenerateIssueDistribution(int inventoryperiod,int start)
        {
            var issues = _db.Issues.Where(x => x.IsOnlineApproved && !x.IsOnlineTransferred && !x.IsVoid && x.GLPeriodId == inventoryperiod && x.IssueID >= start)
                .OrderBy(x=>x.IssueID).ToList();
            foreach (var issue in issues)
            {
                UpdateIssueDistribution(issue);
            }
        }
        public  void GenerateGoodReceiveDistribution(int inventoryperiod, int start)
        {
            var receives = _db.GoodReceives.Where(x => x.IsOnlineApproved && !x.IsOnlineTransferred && !x.IsVoid && x.GLPeriodId == inventoryperiod && x.GoodReceiveId >= start)
                .OrderBy(x => x.GoodReceiveId).ToList();
            foreach (var receive in receives)
            {
                UpdateGoodReceiveDistribution(receive);
            }
        }
        public  void GenerateStoreReturnDistribution(int inventoryperiod, int start)
        {
            var storeReturns = _db.StoreReturns.Where(x => x.IsOnlineApproved && !x.IsOnlineTransferred && !x.IsVoid && x.GLPeriodId == inventoryperiod && x.StoreReturnID >= start)
                .OrderBy(x => x.StoreReturnID).ToList();
            foreach (var storeReturn in storeReturns)
            {
                UpdateStoreReturnDistribution(storeReturn);
            }
        }
        public  void GenerateTransferSendDistribution(int inventoryperiod, int start)
        {
            var itemTransfers = _db.ItemTransfers.Where(x => x.IsOnlineApproved && !x.IsOnlineTransferred && !x.IsVoid && x.GLPeriodId == inventoryperiod && x.ItemTransferID >= start)
                .OrderBy(x => x.ItemTransferID).ToList();
            foreach (var itemTransfer in itemTransfers)
            {
                UpdateTransferSendDistribution(itemTransfer);
            }
        }
        public  void GenerateTransferReceiveDistribution(int inventoryperiod, int start)
        {
            var transferReceives = _db.ItemTransferReceive.Where(x => x.IsOnlineApproved && !x.IsOnlineTransferred && !x.IsVoid && x.GLPeriodId == inventoryperiod && x.ItemTransferReceiveId >= start)
                .OrderBy(x => x.ItemTransferReceiveId).ToList();
            foreach (var itemTransfer in transferReceives)
            {
                UpdateTransferReceiveDistribution(itemTransfer);
            }
        }
    }
}
