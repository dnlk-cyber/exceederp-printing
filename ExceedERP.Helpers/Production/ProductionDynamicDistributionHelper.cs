using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.HR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExceedERP.Helpers.Production
{
    public  class ProductionDynamicDistributionHelper
    {
        private  readonly ExceedDbContext _db = new ExceedDbContext();
        private DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();
        // inventory
        //public  void UpdateGoodReceiveDistribution(GoodReceive record)
        //{
        //    var distributions = _db.GoodReceiveDistributions.Where(x => x.GoodReceiveId == record.GoodReceiveId).ToList();
        //    _db.GoodReceiveDistributions.RemoveRange(distributions);
        //    _db.SaveChanges();

        //    if (record.ReceiveType.Type == GoodReceiveType.Other)
        //    {
        //        DonationGoodReceiveDistribution(record);
        //    }
        //    else
        //    {
        //        PurchaseGoodReceiveDistribution(record);
        //    }
        //}
        public void UpdateRawGoodReceiveDistribution(ProductionStoreReceive record)
        {
            List<ProductionStoreReceiveDistribution> distributions = new List<ProductionStoreReceiveDistribution>();
            //var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == record.SupplierName);

            var existingDistributions = _db.ProductionStoreReceiveDistributions.Where(d => d.ProductionStoreReceiveId == record.ProductionStoreReceiveId).ToList();
            _db.ProductionStoreReceiveDistributions.RemoveRange(existingDistributions);
            _db.SaveChanges();

            if (record.ProductionStoreReceiveItems != null)
            {
                ProductionStoreReceiveDistribution distribution;
                #region debit stock
                foreach (var line in record.ProductionStoreReceiveItems)
                {
                    if (line.SubTotal != 0)
                    {
                        var item = _db.InventoryItems.FirstOrDefault(it => it.ItemCode == line.ItemCode);
                        if (item != null)
                        {
                            string acc = item.ItemCategory.AccountCode;
                            int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                            string locationAccountSeG = HrHelper.GetBranchLocationAccount(StoreHelper.GetStoreBranch(record.StoreCode));
                            string newacc = _dynamicAccount.ReplaceSegment(acc, locationindex, locationAccountSeG);

                            var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);
                            if (receiveDistribution != null)
                            {
                                receiveDistribution.Debit += line.SubTotal;
                                receiveDistribution.Credit = 0;
                                receiveDistribution.ProductionStoreReceiveId = record.ProductionStoreReceiveId;
                            }
                            else
                            {

                                var defaultDistribution = new ProductionStoreReceiveDistribution()
                                {
                                    AccountCode = newacc,
                                    AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                    Debit = line.SubTotal,
                                    Credit = 0,
                                    ProductionStoreReceiveId = line.ProductionStoreReceiveId,
                                    ReceiveItemIDTrackBack = line.ProductionStoreReceiveItemId
                                };
                                distributions.Add(defaultDistribution);
                            }

                        }

                    }
                }

                #endregion

                #region debit tax

                foreach (var line in record.ProductionStoreReceiveItems)
                {
                    if (line.TaxAmount != 0)
                    {
                        var tax = _db.GLTaxRates.Find(line.GLTaxRateID);
                        if (tax != null)
                        {
                            string acc = tax.ApAccountCode;

                            //int subaccountindex = dynamicAccount.GetSubAccountSequenceIndex();
                            //string subAccountSeG = HrHelper.GetBranchSubAccount(record.BranchId);
                            //string newacc = dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

                            var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == acc);
                            if (receiveDistribution != null)
                            {
                                receiveDistribution.Debit += line.TaxAmount;
                                receiveDistribution.Credit = 0;
                                receiveDistribution.ProductionStoreReceiveId = record.ProductionStoreReceiveId;
                            }
                            else
                            {

                                var defaultDistribution = new ProductionStoreReceiveDistribution()
                                {
                                    AccountCode = acc,
                                    AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                                    Debit = line.SubTotal,
                                    Credit = 0,
                                    ProductionStoreReceiveId = line.ProductionStoreReceiveId,
                                    ReceiveItemIDTrackBack = line.ProductionStoreReceiveItemId
                                };
                                distributions.Add(defaultDistribution);
                            }


                        }

                    }
                }

                #endregion

                #region credit side of the distribution empty
                //if (supplier != null && supplier.IsCredit)
                //{
                //    #region Credit supplier

                //    if (record.GoodReceiveItem.Sum(x => x.Total) != 0)
                //    {
                //        var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == supplier.AccountCode);
                //        if (receiveDistribution != null)
                //        {
                //            receiveDistribution.Debit = 0;
                //            receiveDistribution.Credit += record.GoodReceiveItem.Sum(x => x.Total);
                //            receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                //        }
                //        else
                //        {

                //            var defaultDistribution = new GoodReceiveDistribution()
                //            {
                //                AccountCode = supplier.AccountCode,
                //                AccountDesc = _dynamicAccount.GetAccountDesc(supplier.AccountCode),
                //                Debit = 0,
                //                Credit = record.GoodReceiveItem.Sum(x => x.Total),
                //                GoodReceiveId = record.GoodReceiveId
                //            };
                //            distributions.Add(defaultDistribution);
                //        }


                //    }

                //    #endregion
                //}
                //else
                //{
                //    #region Credit common supplier

                //    var commonSupplier = _db.ApSetupCommonSuppliers.FirstOrDefault();
                //    if (record.GoodReceiveItem.Sum(x => x.Total) != 0)
                //    {
                //        if (commonSupplier != null)
                //        {
                //            var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == commonSupplier.AccountCode);
                //            if (receiveDistribution != null)
                //            {
                //                receiveDistribution.Debit = 0;
                //                receiveDistribution.Credit += record.GoodReceiveItem.Sum(x => x.Total);
                //                receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                //            }
                //            else
                //            {

                //                var defaultDistribution = new GoodReceiveDistribution()
                //                {
                //                    AccountCode = commonSupplier.AccountCode,
                //                    AccountDesc = _dynamicAccount.GetAccountDesc(commonSupplier.AccountCode),
                //                    Debit = 0,
                //                    Credit = record.GoodReceiveItem.Sum(x => x.Total),
                //                    GoodReceiveId = record.GoodReceiveId
                //                };
                //                distributions.Add(defaultDistribution);
                //            }


                //        }
                //    }

                //    #endregion
                //} 
                #endregion
            }

            foreach (var distribution in distributions)
            {
                _db.ProductionStoreReceiveDistributions.Add(distribution);
                _db.SaveChanges();
            }

        }
        public void UpdateFinishedGoodReceiveDistribution(ProductionFinishedGoodsStoreReceive record)
        {
            Dictionary<string, string> checkAccount = new Dictionary<string, string>();
            List<ProductionFinishedGoodsStoreReceiveDistribution> distributions = new List<ProductionFinishedGoodsStoreReceiveDistribution>();
            var existingDistributions = _db.ProductionFinishedGoodsStoreReceiveDistributions.Where(d => d.ProductionFinishedGoodsStoreReceiveId == record.ProductionFinishedGoodsStoreReceiveId).ToList();
            _db.ProductionFinishedGoodsStoreReceiveDistributions.RemoveRange(existingDistributions);
            _db.SaveChanges();
            //var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == record.SupplierName);
            var lineItems = _db.ProductionFinishedGoodsStoreReceiveItems.Where(i => i.ProductionFinishedGoodsStoreReceiveId == record.ProductionFinishedGoodsStoreReceiveId).ToList();

            if (lineItems.Any())
            {
                ProductionFinishedGoodsStoreReceiveDistribution distribution;
                #region debit stock
                foreach (var line in lineItems)
                {
                    if (line.SubTotal != 0)
                    {
                        var item = _db.InventoryItems.FirstOrDefault(it => it.ItemCode == line.ItemCode);
                        if (item != null)
                        {
                            string acc = item.ItemCategory.AccountCode;
                            int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                            string locationAccountSeG = HrHelper.GetBranchLocationAccount(StoreHelper.GetStoreBranch(record.StoreCode));
                            string newacc = _dynamicAccount.ReplaceSegment(acc, locationindex, locationAccountSeG);

                            var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);
                            if (receiveDistribution != null)
                            {
                                receiveDistribution.Debit += line.SubTotal;
                                receiveDistribution.Credit = 0;
                                receiveDistribution.ProductionFinishedGoodsStoreReceiveId = record.ProductionFinishedGoodsStoreReceiveId;
                            }
                            else
                            {

                                var defaultDistribution = new ProductionFinishedGoodsStoreReceiveDistribution()
                                {
                                    AccountCode = newacc,
                                    AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                    Debit = line.SubTotal,
                                    Credit = 0,
                                    ProductionFinishedGoodsStoreReceiveId = line.ProductionFinishedGoodsStoreReceiveId,
                                    ReceiveItemIDTrackBack = line.ProductionFinishedGoodsStoreReceiveItemId
                                };
                                distributions.Add(defaultDistribution);
                            }

                        }

                    }
                }

                #endregion

                #region debit tax

                foreach (var line in lineItems)
                {
                    if (line.TaxAmount != 0)
                    {
                        var tax = _db.GLTaxRates.Find(line.GLTaxRateID);
                        if (tax != null)
                        {
                            string acc = tax.ApAccountCode;

                            //int subaccountindex = dynamicAccount.GetSubAccountSequenceIndex();
                            //string subAccountSeG = HrHelper.GetBranchSubAccount(record.BranchId);
                            //string newacc = dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

                            var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == acc);
                            if (receiveDistribution != null)
                            {
                                receiveDistribution.Debit += line.TaxAmount;
                                receiveDistribution.Credit = 0;
                                receiveDistribution.ProductionFinishedGoodsStoreReceiveId = record.ProductionFinishedGoodsStoreReceiveId;
                            }
                            else
                            {

                                var defaultDistribution = new ProductionFinishedGoodsStoreReceiveDistribution()
                                {
                                    AccountCode = acc,
                                    AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                                    Debit = line.SubTotal,
                                    Credit = 0,
                                    ProductionFinishedGoodsStoreReceiveId = line.ProductionFinishedGoodsStoreReceiveId,
                                    ReceiveItemIDTrackBack = line.ProductionFinishedGoodsStoreReceiveItemId
                                };
                                distributions.Add(defaultDistribution);
                            }


                        }

                    }
                }

                #endregion

                #region credit expense
                foreach (var line in lineItems)
                {
                    var item = _db.InventoryItems.FirstOrDefault(it => it.ItemCode == line.ItemCode);
                    if (item != null)
                    {
                        var itemCategory = _db.ItemCategorys.Find(item.ItemCategoryCode);

                        if (itemCategory != null)
                        {
                            string acc = itemCategory.AccountCode;

                            string newacc = "";
                            int costcenterindex = _dynamicAccount.GetCostCenterAccountSequenceIndex();
                            string costCenterSeG = ""; //HrHelper.GetWorkUnitCostCenter(record.OrgStructureId,branch);
                            newacc = _dynamicAccount.ReplaceSegment(acc, costcenterindex, costCenterSeG);

                            int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                            string branchAccountSeG = "";//HrHelper.GetBranchLocationAccount(record.BranchId);
                            newacc = _dynamicAccount.ReplaceSegment(newacc, locationindex, branchAccountSeG);

                            if (line.Total != 0 && checkAccount.ContainsKey(newacc))
                            {
                                ProductionFinishedGoodsStoreReceiveDistribution iDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

                                if (iDistribution != null)
                                {
                                    iDistribution.AccountCode = newacc;
                                    iDistribution.Debit += line.Total;
                                    iDistribution.Credit = 0;
                                    iDistribution.ProductionFinishedGoodsStoreReceiveId = line.ProductionFinishedGoodsStoreReceiveId;
                                    iDistribution.ReceiveItemIDTrackBack = line.ProductionFinishedGoodsStoreReceiveItemId;
                                }
                            }
                            else if (line.Total != 0 && !checkAccount.ContainsKey(newacc))
                            {
                                distribution = new ProductionFinishedGoodsStoreReceiveDistribution
                                {
                                    AccountCode = newacc,
                                    AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                    Debit = line.Total,
                                    Credit = 0,
                                    ProductionFinishedGoodsStoreReceiveId = line.ProductionFinishedGoodsStoreReceiveId,
                                    ReceiveItemIDTrackBack = line.ProductionFinishedGoodsStoreReceiveItemId
                                };
                                distributions.Add(distribution);
                                checkAccount.Add(newacc, line.ItemCode);
                            }
                        }
                    }
                }
                #endregion
            }

            foreach (var distribution in distributions)
            {
                _db.ProductionFinishedGoodsStoreReceiveDistributions.Add(distribution);
                _db.SaveChanges();
            }

        }
        //public  void DonationGoodReceiveDistribution(GoodReceive record)
        //{
        //    GoodReceiveDistribution distribution;
        //    var revenueAccount = _db.RevenueAccounts.FirstOrDefault(x => x.ApplyToInventory);

        //    if (record.GoodReceiveItem!=null)
        //    {
        //        #region debit stock

        //        foreach (var line in record.GoodReceiveItem)
        //        {
        //            if (line.SubTotal != 0)
        //            {
        //                var item = _db.InventoryItems.FirstOrDefault(it => it.ItemCode == line.ItemCode);
        //                if (item != null)
        //                {
        //                    string acc = item.ItemCategory.AccountCode;
        //                    int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
        //                    string subAccountSeG = HrHelper.GetBranchSubAccount(record.BranchId);
        //                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

        //                    distribution = new GoodReceiveDistribution
        //                    {
        //                        AccountCode = newacc,
        //                        AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
        //                        Debit = line.SubTotal,
        //                        Credit = 0,
        //                        GoodReceiveId = line.GoodReceiveId,
        //                        GoodReceiveItemIDTrackBack = line.GoodReceiveItemID
        //                    };
        //                    _db.GoodReceiveDistributions.Add(distribution);
        //                    _db.SaveChanges();
        //                }

        //            }
        //        }

        //        #endregion

        //        #region credit tax payable

        //        foreach (var line in record.GoodReceiveItem)
        //        {
        //            if (line.TaxAmount != 0)
        //            {
        //                var tax = _db.GLTaxRates.Find(line.GLTaxRateID);
        //                if (tax != null)
        //                {
        //                    string acc = tax.ApAccountCode;
        //                    //int subaccountindex = dynamicAccount.GetSubAccountSequenceIndex();
        //                    //string subAccountSeG = HrHelper.GetBranchSubAccount(record.BranchId);
        //                    //string newacc = dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

        //                    distribution = new GoodReceiveDistribution
        //                    {
        //                        AccountCode = acc,
        //                        AccountDesc = _dynamicAccount.GetAccountDesc(acc),
        //                        Debit = 0,
        //                        Credit = line.TaxAmount,
        //                        GoodReceiveId = line.GoodReceiveId,
        //                        GoodReceiveItemIDTrackBack = line.GoodReceiveItemID
        //                    };
        //                    _db.GoodReceiveDistributions.Add(distribution);
        //                    _db.SaveChanges();
        //                }

        //            }
        //        }

        //        #endregion

        //        #region Credit revenue

        //        if (record.GoodReceiveItem.Sum(x => x.SubTotal) != 0)
        //        {
        //            if (revenueAccount != null)
        //            {
        //                distribution = new GoodReceiveDistribution
        //                {
        //                    AccountCode = revenueAccount.InventoryAccountCode,
        //                    AccountDesc = _dynamicAccount.GetAccountDesc(revenueAccount.InventoryAccountCode),
        //                    Debit = 0,
        //                    Credit = record.GoodReceiveItem.Sum(x => x.SubTotal),
        //                    GoodReceiveId = record.GoodReceiveId
        //                };

        //                _db.GoodReceiveDistributions.Add(distribution);
        //                _db.SaveChanges();
        //            }
        //        }

        //        #endregion
        //    }

        //}
        public  void UpdateIssueDistribution(ProductionIssue record)
        {
            ProductionIssueDistribution distribution;
            Dictionary<string, string> checkAccount = new Dictionary<string, string>();
            List<ProductionIssueDistribution> distributions = new List<ProductionIssueDistribution>();

            var issueDistributions = _db.ProductionIssueDistributions.Where(x => x.ProductionIssueId == record.ProductionIssueId).ToList();
            _db.ProductionIssueDistributions.RemoveRange(issueDistributions);
            _db.SaveChanges();

            //int branch = 0;

            //var storereq = _db.StoreRequisitions.FirstOrDefault(x => x.StoreRequisitionID == record.StoreRequisitionID);
            //if (storereq != null)
            //{
            //    if (storereq.IsForOther)
            //    {
            //        if (storereq.ForBranchId != null) branch = (int) storereq.ForBranchId;
            //        else
            //        {
            //            branch = storereq.BranchId;
            //        }
            //    }
            //    else
            //    {
            //        branch = storereq.BranchId;
            //    }
            //}

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
                          
                            string newacc = "";
                            int costcenterindex = _dynamicAccount.GetCostCenterAccountSequenceIndex();
                            string costCenterSeG = ""; //HrHelper.GetWorkUnitCostCenter(record.OrgStructureId,branch);
                            newacc = _dynamicAccount.ReplaceSegment(acc, costcenterindex, costCenterSeG);

                            int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                            string branchAccountSeG = "";//HrHelper.GetBranchLocationAccount(record.BranchId);
                            newacc = _dynamicAccount.ReplaceSegment(newacc, locationindex, branchAccountSeG);

                            if (line.Total != 0 && checkAccount.ContainsKey(newacc))
                            {
                                ProductionIssueDistribution iDistribution =distributions.FirstOrDefault(x => x.AccountCode == newacc);

                                if (iDistribution != null)
                                {
                                    iDistribution.AccountCode = newacc;
                                    iDistribution.Debit += line.Total;
                                    iDistribution.Credit = 0;
                                    iDistribution.ProductionIssueId = line.ProductionIssueId;
                                    iDistribution.IssueItemIDTrackBack = line.ProductionIssueItemId;
                                }
                            }
                            else if (line.Total != 0 && !checkAccount.ContainsKey(newacc))
                            {
                                distribution = new ProductionIssueDistribution
                                {
                                    AccountCode = newacc,
                                    AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                    Debit = line.Total,
                                    Credit = 0,
                                    ProductionIssueId = line.ProductionIssueId,
                                    IssueItemIDTrackBack = line.ProductionIssueItemId
                                };
                                distributions.Add(distribution);
                                checkAccount.Add(newacc, line.ItemCode);
                            }
                        }
                    }
                }
                foreach (ProductionIssueDistribution issueDistribution in distributions)
                {
                    _db.ProductionIssueDistributions.Add(issueDistribution);
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
                            string accCode = inventory.ItemCategory.AccountCode;

                            if (items.Total != 0 && checkAccount.ContainsKey(accCode))
                            {
                                var iDistribution = distributions.FirstOrDefault(x => x.AccountCode == accCode);

                                if (iDistribution != null)
                                {
                                    iDistribution.AccountCode = accCode;
                                    iDistribution.Debit = 0;
                                    iDistribution.Credit += items.Total;
                                    iDistribution.ProductionIssueId = items.ProductionIssueId;
                                    iDistribution.IssueItemIDTrackBack = items.ProductionIssueItemId;
                                }
                            }
                            else if (items.Total != 0 && !checkAccount.ContainsKey(accCode))
                            {
                                distribution = new ProductionIssueDistribution
                                {
                                    AccountCode = accCode,
                                    AccountDesc = _dynamicAccount.GetAccountDesc(accCode),
                                    Debit = 0,
                                    Credit = items.Total,
                                    ProductionIssueId = items.ProductionIssueId,
                                    IssueItemIDTrackBack = items.ProductionIssueItemId
                                };
                                distributions.Add(distribution);
                                checkAccount.Add(accCode, items.ItemCode);
                            }
                        }
                    }
                    foreach (var issueDistribution in distributions)
                    {
                        _db.ProductionIssueDistributions.Add(issueDistribution);
                        _db.SaveChanges();
                    }
                    distributions.Clear();
                    checkAccount.Clear();

                }

                #endregion
            }

        }
        //public  void UpdateStoreReturnDistribution(StoreReturn record)
        //{
        //    StoreReturnDistribution distribution;
        //    Dictionary<string, string> checkAccount = new Dictionary<string, string>();
        //    List<StoreReturnDistribution> distributions = new List<StoreReturnDistribution>();

        //    var returnDistributions = _db.StoreReturnDistributions.Where(x => x.StoreReturnID == record.StoreReturnID).ToList();
        //    _db.StoreReturnDistributions.RemoveRange(returnDistributions);
        //    _db.SaveChanges();

        //    int branch = 0;
            
        //    if (record != null)
        //    {
        //        if (record.IsForOther)
        //        {
        //            if (record.ForBranchId != null) branch = (int)record.ForBranchId;
        //            else
        //            {
        //                branch = InventoryHelper.GetStoreBranch(record.StoreCode); 
        //            }
        //        }
        //        else
        //        {
        //            branch = InventoryHelper.GetStoreBranch(record.StoreCode);
        //        }
        //    }

        //    if (record != null && record.StoreReturnItems != null)
        //    {
        //        #region credit expense

        //        foreach (var line in record.StoreReturnItems)
        //        {
        //            var item = _db.InventoryItems.FirstOrDefault(it => it.ItemCode == line.ItemCode);
        //            if (item != null)
        //            {
        //                var itemCategory = _db.ItemCategorys.Find(item.ItemCategoryCode);

        //                if (itemCategory != null)
        //                {
        //                    string acc = itemCategory.IssueAccountCode;

        //                    string newacc = "";
        //                    int costcenterindex = _dynamicAccount.GetCostCenterAccountSequenceIndex();
        //                    string costCenterSeG = HrHelper.GetWorkUnitCostCenter(record.OrgStructureId,branch);
        //                    newacc = _dynamicAccount.ReplaceSegment(acc, costcenterindex, costCenterSeG);

        //                    int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
        //                    string subAccountSeG = HrHelper.GetBranchSubAccount(branch);
        //                    newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

        //                    if (line.Quantity * line.UnitPrice != 0 && checkAccount.ContainsKey(newacc))
        //                    {
        //                        StoreReturnDistribution iDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

        //                        if (iDistribution != null)
        //                        {
        //                            iDistribution.AccountCode = newacc;
        //                            iDistribution.Debit = 0;
        //                            iDistribution.Credit += (line.Quantity * line.UnitPrice);
        //                            iDistribution.StoreReturnID = line.StoreReturnID;
        //                        }
        //                    }
        //                    else if (line.Quantity * line.UnitPrice != 0 && !checkAccount.ContainsKey(newacc))
        //                    {
        //                        distribution = new StoreReturnDistribution
        //                        {
        //                            AccountCode = newacc,
        //                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
        //                            Debit = 0,
        //                            Credit = (line.Quantity * line.UnitPrice),
        //                            StoreReturnID = line.StoreReturnID,
        //                        };
        //                        distributions.Add(distribution);
        //                        checkAccount.Add(newacc, line.ItemCode);
        //                    }
        //                }
        //            }
        //        }
        //        foreach (StoreReturnDistribution returnDistribution in distributions)
        //        {
        //            _db.StoreReturnDistributions.Add(returnDistribution);
        //            _db.SaveChanges();
        //        }
        //        distributions.Clear();
        //        checkAccount.Clear();

        //        #endregion

        //        #region debit stock

        //        if (record.StoreReturnItems.Sum(x => x.Quantity*x.UnitPrice) != 0)
        //        {
        //            foreach (var items in record.StoreReturnItems)
        //            {
        //                var inventory = _db.InventoryItems.Find(items.ItemCode);
        //                if (inventory != null)
        //                {
        //                    string accCode = inventory.ItemCategory.AccountCode;

        //                    if (items.Quantity * items.UnitPrice != 0 && checkAccount.ContainsKey(accCode))
        //                    {
        //                        var iDistribution = distributions.FirstOrDefault(x => x.AccountCode == accCode);

        //                        if (iDistribution != null)
        //                        {
        //                            iDistribution.AccountCode = accCode;
        //                            iDistribution.Debit += (items.Quantity * items.UnitPrice);
        //                            iDistribution.Credit = 0;
        //                            iDistribution.StoreReturnID = items.StoreReturnID;
        //                        }
        //                    }
        //                    else if (items.Quantity * items.UnitPrice != 0 && !checkAccount.ContainsKey(accCode))
        //                    {
        //                        distribution = new StoreReturnDistribution
        //                        {
        //                            AccountCode = accCode,
        //                            AccountDesc = _dynamicAccount.GetAccountDesc(accCode),
        //                            Debit = (items.Quantity * items.UnitPrice),
        //                            Credit = 0,
        //                            StoreReturnID = items.StoreReturnID
        //                        };
        //                        distributions.Add(distribution);
        //                        checkAccount.Add(accCode, items.ItemCode);
        //                    }
        //                }
        //            }
        //            foreach (var returnDistribution in distributions)
        //            {
        //                _db.StoreReturnDistributions.Add(returnDistribution);
        //                _db.SaveChanges();
        //            }
        //            distributions.Clear();
        //            checkAccount.Clear();

        //        }

        //        #endregion
        //    }

        //}
        //public  void UpdateTransferSendDistribution(ItemTransfer record)
        //{
           
        //    Dictionary<string, string> checkAccount = new Dictionary<string, string>();
        //    List<ItemTransferDistribution> distributions = new List<ItemTransferDistribution>();

        //    var records = _db.ItemTransferDistributions.Where(x => x.ItemTransferID == record.ItemTransferID).ToList();
        //    _db.ItemTransferDistributions.RemoveRange(records);
        //    _db.SaveChanges();

        //    int receiverBranchId = InventoryHelper.GetStoreBranch(record.ToStore);

        //    if (record.ItemTransferItem != null)
        //    {
        //        ItemTransferDistribution distribution;
        //        #region debit clearing account

        //        var clearingAccount = _db.StockClearingAccounts.FirstOrDefault(x => x.Enable);
        //        if (clearingAccount != null)
        //        {
        //            string acc = clearingAccount.AccountCode;
        //            int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
        //            string subAccountSeG = HrHelper.GetBranchSubAccount(receiverBranchId);
        //            string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

        //            decimal clearingAmount = 0;
        //            foreach (var items in record.ItemTransferItem)
        //            {
        //                clearingAmount = clearingAmount + (items.ShippedQuantity* items.UPrice);
        //            }

        //            if (clearingAmount != 0)
        //            {
        //                ItemTransferDistribution iDistribution =new ItemTransferDistribution();

        //                iDistribution.AccountCode = newacc;
        //                iDistribution.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
        //                iDistribution.Debit = clearingAmount;
        //                iDistribution.Credit = 0;
        //                iDistribution.ItemTransferID = record.ItemTransferID;
        //                distributions.Add(iDistribution);
        //                checkAccount.Add(newacc, "Clearing Account");
        //            }


        //        }
        //        foreach (ItemTransferDistribution transferDistribution in distributions)
        //        {
        //            _db.ItemTransferDistributions.Add(transferDistribution);
        //            _db.SaveChanges();
        //        }
        //        distributions.Clear();
        //        checkAccount.Clear();

        //        #endregion

        //        #region Credit sender stock

        //        if (record.ItemTransferItem.Sum(x => x.TPrice) != 0)
        //        {
        //            foreach (var items in record.ItemTransferItem)
        //            {
        //                var inventory = _db.InventoryItems.Find(items.ItemCode);
        //                var senderBranchId = InventoryHelper.GetStoreBranch(record.FromStore);
        //                if (inventory != null && senderBranchId != 0)
        //                {
        //                    string acc = inventory.ItemCategory.AccountCode;
        //                    int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
        //                    string subAccountSeG = HrHelper.GetBranchSubAccount(senderBranchId);
        //                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

        //                    if (items.TPrice != 0 && checkAccount.ContainsKey(newacc))
        //                    {
        //                        var iDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

        //                        if (iDistribution != null)
        //                        {
        //                            iDistribution.AccountCode = newacc;
        //                            iDistribution.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
        //                            iDistribution.Debit = 0;
        //                            iDistribution.Credit += (items.ShippedQuantity * items.UPrice);
        //                            iDistribution.ItemTransferID = items.ItemTransferID;
        //                        }
        //                    }
        //                    else if (items.TPrice != 0 && !checkAccount.ContainsKey(newacc))
        //                    {
        //                        distribution = new ItemTransferDistribution
        //                        {
        //                            AccountCode = newacc,
        //                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
        //                            Debit = 0,
        //                            Credit = (items.ShippedQuantity * items.UPrice),
        //                            ItemTransferID = items.ItemTransferID,
        //                        };
        //                        distributions.Add(distribution);
        //                        checkAccount.Add(newacc, items.ItemCode);
        //                    }
        //                }
        //            }
        //            foreach (var transferDistribution in distributions)
        //            {
        //                _db.ItemTransferDistributions.Add(transferDistribution);
        //                _db.SaveChanges();
        //            }
        //            distributions.Clear();
        //            checkAccount.Clear();
        //        }

        //        #endregion
        //    }
        //}
        //public  void UpdateTransferReceiveDistribution(ItemTransferReceive record)
        //{
           
        //    Dictionary<string, string> checkAccount = new Dictionary<string, string>();
        //    List<ItemTransferReceiveDistribution> distributions = new List<ItemTransferReceiveDistribution>();

        //    var records = _db.ItemTransferReceiveDistribution.Where(x => x.ItemTransferReceiveId == record.ItemTransferReceiveId).ToList();
        //    _db.ItemTransferReceiveDistribution.RemoveRange(records);
        //    _db.SaveChanges();

        //    var receiverBranchId = InventoryHelper.GetStoreBranch(record.ToStore);

        //    if (record.ItemTransferItemReceive != null)
        //    {
        //        ItemTransferReceiveDistribution distribution;
        //        #region debit receiver stock

        //        if (record.ItemTransferItemReceive.Sum(x => x.TPrice) != 0)
        //        {
        //            foreach (var items in record.ItemTransferItemReceive)
        //            {
        //                var inventory = _db.InventoryItems.Find(items.ItemCode);
        //                if (inventory != null && receiverBranchId != 0)
        //                {
        //                    string acc = inventory.ItemCategory.AccountCode;
        //                    int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
        //                    string subAccountSeG = HrHelper.GetBranchSubAccount(receiverBranchId);
        //                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

        //                    if (items.TPrice != 0 && checkAccount.ContainsKey(newacc))
        //                    {
        //                        var iDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

        //                        if (iDistribution != null)
        //                        {
        //                            iDistribution.AccountCode = newacc;
        //                            iDistribution.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
        //                            iDistribution.Debit += (items.ReceivedQuantity * items.UPrice);
        //                            iDistribution.Credit = 0;
        //                            iDistribution.ItemTransferReceiveId = items.ItemTransferReceiveId;
        //                        }
        //                    }
        //                    else if (items.TPrice != 0 && !checkAccount.ContainsKey(newacc))
        //                    {
        //                        distribution = new ItemTransferReceiveDistribution
        //                        {
        //                            AccountCode = newacc,
        //                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
        //                            Debit = (items.ReceivedQuantity * items.UPrice),
        //                            Credit = 0,
        //                            ItemTransferReceiveId = items.ItemTransferReceiveId,
        //                        };
        //                        distributions.Add(distribution);
        //                        checkAccount.Add(newacc, items.ItemCode);
        //                    }
        //                }
        //            }
        //            foreach (var transferDistribution in distributions)
        //            {
        //                _db.ItemTransferReceiveDistribution.Add(transferDistribution);
        //                _db.SaveChanges();
        //            }
        //            distributions.Clear();
        //            checkAccount.Clear();
        //        }

        //        #endregion

        //        #region credit clearing account

        //        var clearingAccount = _db.StockClearingAccounts.FirstOrDefault(x => x.Enable);
        //        if (clearingAccount != null)
        //        {
        //            string acc = clearingAccount.AccountCode;
        //            int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
        //            string subAccountSeG = HrHelper.GetBranchSubAccount(receiverBranchId);
        //            string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

        //            decimal clearingAmount = 0;
        //            foreach (var items in record.ItemTransferItemReceive)
        //            {
        //                clearingAmount = clearingAmount + (items.ReceivedQuantity * items.UPrice);
        //            }

        //            if (clearingAmount != 0)
        //            {
        //                ItemTransferReceiveDistribution iDistribution=new ItemTransferReceiveDistribution();
        //                iDistribution.AccountCode = newacc;
        //                iDistribution.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
        //                iDistribution.Debit = 0;
        //                iDistribution.Credit = clearingAmount;
        //                iDistribution.ItemTransferReceiveId = record.ItemTransferReceiveId;
        //                distributions.Add(iDistribution);
        //                checkAccount.Add(newacc, "Clearing Account");
        //            }


        //        }
        //        foreach (ItemTransferReceiveDistribution transferDistribution in distributions)
        //        {
        //            _db.ItemTransferReceiveDistribution.Add(transferDistribution);
        //            _db.SaveChanges();
        //        }
        //        distributions.Clear();
        //        checkAccount.Clear();

        //        #endregion
                
        //    }
        //}

        //new 

        //public  void GenerateIssueDistribution(int inventoryperiod,int start)
        //{
        //    var issues = _db.Issues.Where(x => x.IsOnlineApproved && !x.IsOnlineTransferred && !x.IsVoid && x.GLPeriodId == inventoryperiod && x.IssueID >= start)
        //        .ToList();
        //    foreach (var issue in issues)
        //    {
        //        UpdateIssueDistribution(issue);
        //    }
        //}
        //public  void GenerateGoodReceiveDistribution(int inventoryperiod, int start)
        //{
        //    var receives = _db.GoodReceives.Where(x => x.IsOnlineApproved && !x.IsOnlineTransferred && !x.IsVoid && x.GLPeriodId == inventoryperiod && x.GoodReceiveId >= start)
        //        .ToList();
        //    foreach (var receive in receives)
        //    {
        //        UpdateGoodReceiveDistribution(receive);
        //    }
        //}
        //public  void GenerateStoreReturnDistribution(int inventoryperiod, int start)
        //{
        //    var storeReturns = _db.StoreReturns.Where(x => x.IsOnlineApproved && !x.IsOnlineTransferred && !x.IsVoid && x.GLPeriodId == inventoryperiod && x.StoreReturnID >= start)
        //        .ToList();
        //    foreach (var storeReturn in storeReturns)
        //    {
        //        UpdateStoreReturnDistribution(storeReturn);
        //    }
        //}
        //public  void GenerateTransferSendDistribution(int inventoryperiod, int start)
        //{
        //    var itemTransfers = _db.ItemTransfers.Where(x => x.IsOnlineApproved && !x.IsOnlineTransferred && !x.IsVoid && x.GLPeriodId == inventoryperiod && x.ItemTransferID >= start)
        //        .ToList();
        //    foreach (var itemTransfer in itemTransfers)
        //    {
        //        UpdateTransferSendDistribution(itemTransfer);
        //    }
        //}
        //public  void GenerateTransferReceiveDistribution(int inventoryperiod, int start)
        //{
        //    var transferReceives = _db.ItemTransferReceive.Where(x => x.IsOnlineApproved && !x.IsOnlineTransferred && !x.IsVoid && x.GLPeriodId == inventoryperiod && x.ItemTransferReceiveId >= start)
        //        .ToList();
        //    foreach (var itemTransfer in transferReceives)
        //    {
        //        UpdateTransferReceiveDistribution(itemTransfer);
        //    }
        //}
    }
}
