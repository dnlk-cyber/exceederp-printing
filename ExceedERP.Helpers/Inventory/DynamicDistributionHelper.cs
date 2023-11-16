using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.HR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExceedERP.Helpers.Inventory
{
    public  class DynamicDistributionHelper
    {
        private  readonly ExceedDbContext _db = new ExceedDbContext();
        private DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();
        // inventory
        public  void UpdateGoodReceiveDistribution(GoodReceive record)
        {
            var distributions = _db.GoodReceiveDistributions.Where(x => x.GoodReceiveId == record.GoodReceiveId).ToList();
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
        //new changed for invoice
        public void PurchaseGoodReceiveDistribution(GoodReceive record)
        {
            if (!FinanceConfigurationHelper.IsGrnDistributionDisabled())
            {
                List<GoodReceiveDistribution> distributions = new List<GoodReceiveDistribution>();
                var receiveItems = _db.GoodReceiveItems.Where(x => x.GoodReceiveId == record.GoodReceiveId).ToList();
                var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == record.SupplierId);

                if (receiveItems.Any())
                {
                    if (FinanceConfigurationHelper.ApplyClearingAccountInGRN())
                    {
                        #region debit stock


                        foreach (var line in receiveItems)
                        {
                            if (line.SubTotal != 0)
                            {
                                var item = _db.InventoryItems.FirstOrDefault(it => it.ItemCode == line.ItemCode);
                                if (item != null)
                                {
                                    string acc = item.ItemCategory.AccountCode;
                                    if (!string.IsNullOrWhiteSpace(item.SubCategoryCode))
                                    {
                                        var subcategory = _db.ItemCategorys.Where(x => x.ItemCategoryCode == item.SubCategoryCode).FirstOrDefault();
                                        if (subcategory != null)
                                            acc = subcategory.AccountCode;
                                    }
                                    int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                                    string locationAccountSeG = HrHelper.GetBranchLocationAccount(record.BranchId);
                                    string newacc = _dynamicAccount.ReplaceSegment(acc, locationindex, locationAccountSeG);
                                                                       
                                    var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);
                                    if (receiveDistribution != null)
                                    {
                                        receiveDistribution.Debit += line.SubTotal;// + line.TaxAmount;
                                        receiveDistribution.Credit = 0;
                                        receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                                    }
                                    else
                                    {

                                        var defaultDistribution = new GoodReceiveDistribution()
                                        {
                                            AccountCode = newacc,
                                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                            Debit = line.SubTotal,// + line.TaxAmount,
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

                        #region debit tax // commented

                        //foreach (var line in receiveItems)
                        //{
                        //    if (line.TaxAmount != 0)
                        //    {
                        //        var tax = _db.GLTaxRates.Find(line.GLTaxRateID);
                        //        if (tax != null)
                        //        {
                        //            string acc = tax.ApAccountCode;

                        //            int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        //            int branchId = InventoryHelper.GetStoreBranch(record.StoreCode);
                        //            string locationAccountSeG = HrHelper.GetBranchLocationAccount(branchId);
                        //            string newacc = _dynamicAccount.ReplaceSegment(acc, locationindex, locationAccountSeG);

                        //            var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);
                        //            if (receiveDistribution != null)
                        //            {
                        //                receiveDistribution.Debit += line.TaxAmount;
                        //                receiveDistribution.Credit = 0;
                        //                receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                        //            }
                        //            else
                        //            {

                        //                var defaultDistribution = new GoodReceiveDistribution()
                        //                {
                        //                    AccountCode = newacc,
                        //                    AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                        //                    Debit = line.TaxAmount,
                        //                    Credit = 0,
                        //                    GoodReceiveId = line.GoodReceiveId,
                        //                    GoodReceiveItemIDTrackBack = line.GoodReceiveItemID
                        //                };
                        //                distributions.Add(defaultDistribution);
                        //            }


                        //        }

                        //    }
                        //}

                        #endregion

                        #region credit GR/IR clearing account
                        var clearingAccount = _db.StockClearingAccounts.Where(x => x.Type == ClearingAccountTypes.Inventory_Or_Good_Receiving).Where(x => x.Enable).FirstOrDefault();

                        if (receiveItems.Sum(x => x.Total) != 0 && clearingAccount!=null)
                        {
                            int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                            int branchId = InventoryHelper.GetStoreBranch(record.StoreCode);
                            string locationAccountSeG = HrHelper.GetBranchLocationAccount(branchId);
                            string newacc = _dynamicAccount.ReplaceSegment(clearingAccount.AccountCode, locationindex, locationAccountSeG);

                            var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);
                            if (receiveDistribution != null)
                            {
                                receiveDistribution.Debit = 0;
                                receiveDistribution.Credit += receiveItems.Sum(x => x.SubTotal);
                                receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                            }
                            else
                            {

                                var defaultDistribution = new GoodReceiveDistribution()
                                {
                                    AccountCode = newacc,
                                    AccountDesc = _dynamicAccount.GetAccountDesc(clearingAccount.AccountCode),
                                    Debit = 0,
                                    Credit = receiveItems.Sum(x => x.Total),
                                    GoodReceiveId = record.GoodReceiveId
                                };
                                distributions.Add(defaultDistribution);
                            }


                        }

                        #endregion
                    }
                    else
                    {
                        #region debit stock


                        foreach (var line in receiveItems)
                        {
                            if (line.SubTotal != 0)
                            {
                                var item = _db.InventoryItems.FirstOrDefault(it => it.ItemCode == line.ItemCode);
                                if (item != null)
                                {
                                    string acc = item.ItemCategory.AccountCode;
                                    if (!string.IsNullOrWhiteSpace(item.SubCategoryCode))
                                    {
                                        var subcategory = _db.ItemCategorys.Where(x => x.ItemCategoryCode == item.SubCategoryCode).FirstOrDefault();
                                        if (subcategory != null)
                                            acc = subcategory.AccountCode;
                                    }
                                    int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                                    int branchId = InventoryHelper.GetStoreBranch(record.StoreCode);
                                    string locationAccountSeG = HrHelper.GetBranchLocationAccount(branchId);
                                    string newacc = _dynamicAccount.ReplaceSegment(acc, locationindex, locationAccountSeG);

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
                                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
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

                        //foreach (var line in receiveItems)
                        //{
                        //    if (line.TaxAmount != 0)
                        //    {
                        //        var tax = _db.GLTaxRates.Find(line.GLTaxRateID);
                        //        if (tax != null)
                        //        {
                        //            string acc = tax.ApAccountCode;

                        //            int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        //            int branchId = InventoryHelper.GetStoreBranch(record.StoreCode);
                        //            string locationAccountSeG = HrHelper.GetBranchLocationAccount(branchId);
                        //            string newacc = _dynamicAccount.ReplaceSegment(acc, locationindex, locationAccountSeG);

                        //            var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);
                        //            if (receiveDistribution != null)
                        //            {
                        //                receiveDistribution.Debit += line.TaxAmount;
                        //                receiveDistribution.Credit = 0;
                        //                receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                        //            }
                        //            else
                        //            {

                        //                var defaultDistribution = new GoodReceiveDistribution()
                        //                {
                        //                    AccountCode = newacc,
                        //                    AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                        //                    Debit = line.TaxAmount,
                        //                    Credit = 0,
                        //                    GoodReceiveId = line.GoodReceiveId,
                        //                    GoodReceiveItemIDTrackBack = line.GoodReceiveItemID
                        //                };
                        //                distributions.Add(defaultDistribution);
                        //            }


                        //        }

                        //    }
                        //}

                        #endregion

                        if (supplier != null && supplier.IsCredit)
                        {
                            #region Credit supplier

                            if (record.GoodReceiveItem.Sum(x => x.Total) != 0)
                            {
                                int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                                int branchId = InventoryHelper.GetStoreBranch(record.StoreCode);
                                string locationAccountSeG = HrHelper.GetBranchLocationAccount(branchId);
                                string newacc = _dynamicAccount.ReplaceSegment(supplier.AccountCode, locationindex, locationAccountSeG);
                                var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);
                                if (receiveDistribution != null)
                                {
                                    receiveDistribution.Debit = 0;
                                    receiveDistribution.Credit += record.GoodReceiveItem.Sum(x => x.SubTotal); //total
                                    receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                                }
                                else
                                {

                                    var defaultDistribution = new GoodReceiveDistribution()
                                    {
                                        AccountCode = newacc,
                                        AccountDesc = _dynamicAccount.GetAccountDesc(supplier.AccountCode),
                                        Debit = 0,
                                        Credit = record.GoodReceiveItem.Sum(x => x.SubTotal), //total
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
                                    int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                                    int branchId = InventoryHelper.GetStoreBranch(record.StoreCode);
                                    string locationAccountSeG = HrHelper.GetBranchLocationAccount(branchId);
                                    string newacc = _dynamicAccount.ReplaceSegment(commonSupplier.AccountCode, locationindex, locationAccountSeG);

                                    var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);
                                    if (receiveDistribution != null)
                                    {
                                        receiveDistribution.Debit = 0;
                                        receiveDistribution.Credit += record.GoodReceiveItem.Sum(x => x.SubTotal);//total
                                        receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                                    }
                                    else
                                    {

                                        var defaultDistribution = new GoodReceiveDistribution()
                                        {
                                            AccountCode = newacc,
                                            AccountDesc = _dynamicAccount.GetAccountDesc(commonSupplier.AccountCode),
                                            Debit = 0,
                                            Credit = record.GoodReceiveItem.Sum(x => x.SubTotal),//total
                                            GoodReceiveId = record.GoodReceiveId
                                        };
                                        distributions.Add(defaultDistribution);
                                    }


                                }
                            }

                            #endregion
                        }
                    }
                }

                foreach (var d in distributions)
                {

                    var distribution = d;

                    #region replace business unit
                    if (GlobalHelper.IsMultipleBusinessUnit())
                    {
                        int storeBusinUnitId = InventoryHelper.GetStoreBusinessUnit(record.StoreCode);
                        if (storeBusinUnitId != 0)
                        {
                            int bizunitindex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                            if (bizunitindex != -1)
                            {
                                string sectorAccountSeG = HrHelper.GetBusinessUnitSectorAccount(storeBusinUnitId);
                                string newacc = _dynamicAccount.ReplaceSegment(distribution.AccountCode, bizunitindex, sectorAccountSeG);
                                distribution.AccountCode = newacc;
                                distribution.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            }
                        }
                    }
                    #endregion
                    _db.GoodReceiveDistributions.Add(distribution);
                    _db.SaveChanges();
                }
            }
        }

        //was active
        //public  void PurchaseGoodReceiveDistribution_1(GoodReceive record)
        //{
        //    List<GoodReceiveDistribution> distributions = new List<GoodReceiveDistribution>();
        //    var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == record.SupplierId);

        //    if (record.GoodReceiveItem != null)
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
        //                    if (!string.IsNullOrWhiteSpace(item.SubCategoryCode))
        //                    {
        //                        var subcategory = _db.ItemCategorys.Where(x => x.ItemCategoryCode == item.SubCategoryCode).FirstOrDefault();
        //                        if (subcategory != null)
        //                            acc = subcategory.AccountCode;
        //                    }
        //                    int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
        //                    string locationAccountSeG = HrHelper.GetBranchLocationAccount(record.BranchId);
        //                    string newacc = _dynamicAccount.ReplaceSegment(acc, locationindex, locationAccountSeG);

        //                    var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);
        //                    if (receiveDistribution != null)
        //                    {
        //                        receiveDistribution.Debit += line.SubTotal;
        //                        receiveDistribution.Credit = 0;
        //                        receiveDistribution.GoodReceiveId = record.GoodReceiveId;
        //                    }
        //                    else
        //                    {

        //                        var defaultDistribution = new GoodReceiveDistribution()
        //                        {
        //                            AccountCode = newacc,
        //                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
        //                            Debit = line.SubTotal,
        //                            Credit = 0,
        //                            GoodReceiveId = line.GoodReceiveId,
        //                            GoodReceiveItemIDTrackBack = line.GoodReceiveItemID
        //                        };
        //                        distributions.Add(defaultDistribution);
        //                    }
                           
        //                }

        //            }
        //        }

        //        #endregion

        //        #region debit tax

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

        //                    var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == acc);
        //                    if (receiveDistribution != null)
        //                    {
        //                        receiveDistribution.Debit += line.TaxAmount;
        //                        receiveDistribution.Credit = 0;
        //                        receiveDistribution.GoodReceiveId = record.GoodReceiveId;
        //                    }
        //                    else
        //                    {

        //                        var defaultDistribution = new GoodReceiveDistribution()
        //                        {
        //                            AccountCode = acc,
        //                            AccountDesc = _dynamicAccount.GetAccountDesc(acc),
        //                            Debit = line.TaxAmount,
        //                            Credit = 0,
        //                            GoodReceiveId = line.GoodReceiveId,
        //                            GoodReceiveItemIDTrackBack = line.GoodReceiveItemID
        //                        };
        //                        distributions.Add(defaultDistribution);
        //                    }

                            
        //                }

        //            }
        //        }

        //        #endregion

        //        if (supplier != null && supplier.IsCredit)
        //        {
        //            #region Credit supplier

        //            if (record.GoodReceiveItem.Sum(x => x.Total) != 0)
        //            {
        //                var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == supplier.AccountCode);
        //                if (receiveDistribution != null)
        //                {
        //                    receiveDistribution.Debit = 0;
        //                    receiveDistribution.Credit += record.GoodReceiveItem.Sum(x => x.Total);
        //                    receiveDistribution.GoodReceiveId = record.GoodReceiveId;
        //                }
        //                else
        //                {

        //                    var defaultDistribution = new GoodReceiveDistribution()
        //                    {
        //                        AccountCode = supplier.AccountCode,
        //                        AccountDesc = _dynamicAccount.GetAccountDesc(supplier.AccountCode),
        //                        Debit = 0,
        //                        Credit = record.GoodReceiveItem.Sum(x => x.Total),
        //                        GoodReceiveId = record.GoodReceiveId
        //                    };
        //                    distributions.Add(defaultDistribution);
        //                }

                        
        //            }

        //            #endregion
        //        }
        //        else 
        //        {
        //            #region Credit common supplier

        //            var commonSupplier = _db.ApSetupCommonSuppliers.FirstOrDefault();
        //            if (record.GoodReceiveItem.Sum(x => x.Total) != 0)
        //            {
        //                if (commonSupplier != null)
        //                {
        //                    var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == commonSupplier.AccountCode);
        //                    if (receiveDistribution != null)
        //                    {
        //                        receiveDistribution.Debit = 0;
        //                        receiveDistribution.Credit += record.GoodReceiveItem.Sum(x => x.Total);
        //                        receiveDistribution.GoodReceiveId = record.GoodReceiveId;
        //                    }
        //                    else
        //                    {

        //                        var defaultDistribution = new GoodReceiveDistribution()
        //                        {
        //                            AccountCode = commonSupplier.AccountCode,
        //                            AccountDesc = _dynamicAccount.GetAccountDesc(commonSupplier.AccountCode),
        //                            Debit = 0,
        //                            Credit = record.GoodReceiveItem.Sum(x => x.Total),
        //                            GoodReceiveId = record.GoodReceiveId
        //                        };
        //                        distributions.Add(defaultDistribution);
        //                    }

                            
        //                }
        //            }

        //            #endregion
        //        }
        //    }

        //    foreach (var d in distributions)
        //    {
        //        var distribution = d;

        //        #region replace business unit
        //        if (GlobalHelper.IsMultipleBusinessUnit())
        //        {
        //            int storeBusinUnitId = InventoryHelper.GetStoreBusinessUnit(record.StoreCode);
        //            if (storeBusinUnitId != 0)
        //            {
        //                int bizunitindex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
        //                string sectorAccountSeG = HrHelper.GetBusinessUnitSectorAccount(storeBusinUnitId);
        //                string newacc = _dynamicAccount.ReplaceSegment(distribution.AccountCode, bizunitindex, sectorAccountSeG);
        //                distribution.AccountCode = newacc;
        //                distribution.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
        //            }
        //        }
        //        #endregion
        //        _db.GoodReceiveDistributions.Add(distribution);
        //        _db.SaveChanges();
        //    }

        //}
              

        public  void DonationGoodReceiveDistribution(GoodReceive record)
        {
            if (!FinanceConfigurationHelper.IsGrnDistributionDisabled())
            {
                List<GoodReceiveDistribution> distributions = new List<GoodReceiveDistribution>();
                var receiveItems = _db.GoodReceiveItems.Where(x => x.GoodReceiveId == record.GoodReceiveId).ToList();

               // var revenueAccount = _db.RevenueAccounts.FirstOrDefault(x => x.ApplyToInventory);

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
                                string acc = item.ItemCategory.AccountCode;
                                if (!String.IsNullOrWhiteSpace(item.SubCategoryCode))
                                {
                                    var subcategory = _db.ItemCategorys.Where(x => x.ItemCategoryCode == item.SubCategoryCode).FirstOrDefault();
                                    if (subcategory != null)
                                        acc = subcategory.AccountCode;
                                }
                                int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                                int branchId = InventoryHelper.GetStoreBranch(record.StoreCode);
                                string locationAccountSeG = HrHelper.GetBranchLocationAccount(branchId);
                                string newacc = _dynamicAccount.ReplaceSegment(acc, locationindex, locationAccountSeG);

                                var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);
                                if (receiveDistribution != null)
                                {
                                    receiveDistribution.Debit += line.SubTotal;// + line.TaxAmount;//vat added here
                                    receiveDistribution.Credit = 0;
                                    receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                                }
                                else
                                {
                                    var defaultDistribution = new GoodReceiveDistribution
                                    {
                                        AccountCode = newacc,
                                        AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                        Debit = line.SubTotal ,//+ line.TaxAmount,//vat added here
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

                    #region debit tax payable //commented

                    //foreach (var line in record.GoodReceiveItem)
                    //{
                    //    if (line.TaxAmount != 0)
                    //    {
                    //        var tax = _db.GLTaxRates.Find(line.GLTaxRateID);
                    //        if (tax != null)
                    //        {
                    //            string acc = tax.ApAccountCode;

                    //            int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    //            int branchId = InventoryHelper.GetStoreBranch(record.StoreCode);
                    //            string locationAccountSeG = HrHelper.GetBranchLocationAccount(branchId);
                    //            string newacc = _dynamicAccount.ReplaceSegment(acc, locationindex, locationAccountSeG);

                    //            var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);
                    //            if (receiveDistribution != null)
                    //            {
                    //                receiveDistribution.Debit += line.TaxAmount;
                    //                receiveDistribution.Credit = 0;
                    //                receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                    //            }
                    //            else
                    //            {
                    //                var defaultDistribution = new GoodReceiveDistribution
                    //                {
                    //                    AccountCode = newacc,
                    //                    AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                    //                    Debit = line.TaxAmount,
                    //                    Credit = 0,
                    //                    GoodReceiveId = line.GoodReceiveId,
                    //                    GoodReceiveItemIDTrackBack = line.GoodReceiveItemID
                    //                };
                    //                distributions.Add(defaultDistribution);
                    //            }

                              
                    //        }

                    //    }
                    //}

                    #endregion

                    #region Credit revenue //commneted

                    //if (record.GoodReceiveItem.Sum(x => x.SubTotal) != 0)
                    //{
                    //    if (revenueAccount != null)
                    //    {
                    //        var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == revenueAccount.InventoryAccountCode);
                    //        if (receiveDistribution != null)
                    //        {
                    //            receiveDistribution.Debit = 0;
                    //            receiveDistribution.Credit += record.GoodReceiveItem.Sum(x => x.SubTotal) + record.GoodReceiveItem.Sum(x => x.TaxAmount);
                    //            receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                    //        }
                    //        else
                    //        {
                    //            var defaultDistribution = new GoodReceiveDistribution
                    //            {
                    //                AccountCode = revenueAccount.InventoryAccountCode,
                    //                AccountDesc = _dynamicAccount.GetAccountDesc(revenueAccount.InventoryAccountCode),
                    //                Debit = 0,
                    //                Credit = record.GoodReceiveItem.Sum(x => x.SubTotal) + record.GoodReceiveItem.Sum(x => x.TaxAmount),
                    //                GoodReceiveId = record.GoodReceiveId
                    //            };

                    //            distributions.Add(defaultDistribution);
                    //        }
                    //    }
                    //}

                    #endregion

                    #region credit GR/IR clearing account
                    var clearingAccount = _db.StockClearingAccounts.Where(x => x.Type == ClearingAccountTypes.Inventory_Or_Good_Receiving).Where(x => x.Enable).FirstOrDefault();

                    if (receiveItems.Sum(x => x.SubTotal) != 0 && clearingAccount!=null)
                    {
                        int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        int branchId = InventoryHelper.GetStoreBranch(record.StoreCode);
                        string locationAccountSeG = HrHelper.GetBranchLocationAccount(branchId);
                        string newacc = _dynamicAccount.ReplaceSegment(clearingAccount.AccountCode, locationindex, locationAccountSeG);

                        var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);
                        if (receiveDistribution != null)
                        {
                            receiveDistribution.Debit = 0;
                            receiveDistribution.Credit += receiveItems.Sum(x => x.SubTotal);
                            receiveDistribution.GoodReceiveId = record.GoodReceiveId;
                        }
                        else
                        {

                            var defaultDistribution = new GoodReceiveDistribution()
                            {
                                AccountCode = newacc,
                                AccountDesc = _dynamicAccount.GetAccountDesc(clearingAccount.AccountCode),
                                Debit = 0,
                                Credit = receiveItems.Sum(x => x.SubTotal),
                                GoodReceiveId = record.GoodReceiveId
                            };
                            distributions.Add(defaultDistribution);
                        }


                    }

                    #endregion
                }

                foreach (var d in distributions)
                {
                    var distribution = d;

                    #region replace business unit
                    if (GlobalHelper.IsMultipleBusinessUnit())
                    {
                        int storeBusinUnitId = InventoryHelper.GetStoreBusinessUnit(record.StoreCode);
                        if (storeBusinUnitId != 0)
                        {
                            int bizunitindex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                            string sectorAccountSeG = HrHelper.GetBusinessUnitSectorAccount(storeBusinUnitId);
                            string newacc = _dynamicAccount.ReplaceSegment(distribution.AccountCode, bizunitindex, sectorAccountSeG);
                            distribution.AccountCode = newacc;
                            distribution.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                        }
                    }
                    #endregion
                    _db.GoodReceiveDistributions.Add(distribution);
                    _db.SaveChanges();
                }
            }

        }
        public  void UpdateIssueDistribution(Issue record)
        {
            List<IssueDistribution> distributions = new List<IssueDistribution>();

            var issueDistributions = _db.IssueDistributions.Where(x => x.IssueID == record.IssueID).ToList();
            _db.IssueDistributions.RemoveRange(issueDistributions);
            _db.SaveChanges();

            int branch = InventoryHelper.GetStoreBranch(record.StoreCode);
           

            if (branch == 0)
            {
                var storereq = _db.StoreRequisitions.FirstOrDefault(x => x.StoreRequisitionID == record.StoreRequisitionID);
                if (storereq != null)
                {
                    if (storereq.IsForOther)
                    {
                        if (storereq.ForBranchId != null) branch = (int)storereq.ForBranchId;
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
                            if (!String.IsNullOrWhiteSpace(item.SubCategoryCode))
                            {
                                var subcategory = _db.ItemCategorys.Where(x => x.ItemCategoryCode == item.SubCategoryCode).FirstOrDefault();
                                if(subcategory!=null)
                                    acc = subcategory.IssueAccountCode;
                            }

                            string newacc = acc;                           
                            string replaceCostCenter = InventoryConfigurationHelper.GetIssueDistributionCostCenterDebitReplace();

                            if (replaceCostCenter == BooleanTypeAttributes.TRUE.ToString())
                            {
                                int costcenterindex = _dynamicAccount.GetCostCenterAccountSequenceIndex();
                                if (costcenterindex != -1)
                                {
                                    string costCenterSeG = HrHelper.GetWorkUnitCostCenter(record.OrgStructureId, branch);
                                    newacc = _dynamicAccount.ReplaceSegment(acc, costcenterindex, costCenterSeG);
                                }
                            }

                            string applySubAccount = InventoryConfigurationHelper.GetBranchSubAccountReplace();
                            if (applySubAccount == BooleanTypeAttributes.TRUE.ToString())
                            {
                                int subAccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                                if (subAccountindex != -1)
                                {
                                    int branchId = InventoryHelper.GetStoreBranch(record.StoreCode);
                                    string locationAccountSeG = HrHelper.GetBranchLocationAccount(branchId);
                                    newacc = _dynamicAccount.ReplaceSegment(newacc, subAccountindex, locationAccountSeG);
                                }
                            }
                            else
                            {
                                int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                                if (locationindex != -1)
                                {
                                    int branchId = InventoryHelper.GetStoreBranch(record.StoreCode);
                                    string locationAccountSeG = HrHelper.GetBranchLocationAccount(branchId);
                                    newacc = _dynamicAccount.ReplaceSegment(newacc, locationindex, locationAccountSeG);
                                }
                            }

                            if (line.Total != 0)
                            {
                                var issueDistribution =distributions.FirstOrDefault(x => x.AccountCode == newacc);

                                if (issueDistribution != null)
                                {
                                    issueDistribution.AccountCode = newacc;
                                    issueDistribution.Debit += line.Total;
                                    issueDistribution.Credit = 0;
                                    issueDistribution.IssueID = line.IssueID;
                                    issueDistribution.IssueItemIDTrackBack = line.IssueItemID;
                                }
                                else
                                {
                                    var distribution = new IssueDistribution
                                    {
                                        AccountCode = newacc,
                                        AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                        Debit = line.Total,
                                        Credit = 0,
                                        IssueID = line.IssueID,
                                        IssueItemIDTrackBack = line.IssueItemID
                                    };
                                    distributions.Add(distribution);
                                }
                            }
                           
                        }
                    }
                }
               

                #endregion

                #region Credit stock

                if (record.IssueItems.Sum(x => x.Total) != 0)
                {
                    foreach (var items in record.IssueItems)
                    {
                        var item = _db.InventoryItems.Find(items.ItemCode);
                        if (item != null)
                        {
                            string newacc = item.ItemCategory.AccountCode;
                            if (!string.IsNullOrWhiteSpace(item.SubCategoryCode))
                            {
                                var subcategory = _db.ItemCategorys.Where(x => x.ItemCategoryCode == item.SubCategoryCode).FirstOrDefault();
                                if(subcategory!=null)
                                    newacc = subcategory.AccountCode;
                            }

                            int locationindexindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                            if (locationindexindex != -1)
                            {
                                string branchSeG = HrHelper.GetBranchLocationAccount(branch);
                                newacc = _dynamicAccount.ReplaceSegment(newacc, locationindexindex, branchSeG);
                            }

                            if (items.Total != 0 )
                            {
                                var issueDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

                                if (issueDistribution != null)
                                {
                                    issueDistribution.AccountCode = newacc;
                                    issueDistribution.Debit = 0;
                                    issueDistribution.Credit += items.Total;
                                    issueDistribution.IssueID = items.IssueID;
                                    issueDistribution.IssueItemIDTrackBack = items.IssueItemID;
                                }
                                else
                                {
                                   var distribution = new IssueDistribution
                                    {
                                        AccountCode = newacc,
                                        AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                        Debit = 0,
                                        Credit = items.Total,
                                        IssueID = items.IssueID,
                                        IssueItemIDTrackBack = items.IssueItemID
                                    };
                                    distributions.Add(distribution);
                                }
                            }
                         
                        }
                    }
                   

                }

                #endregion
            }

            foreach (var d in distributions)
            {
                var distribution = d;

                #region replace business unit
                if (GlobalHelper.IsMultipleBusinessUnit())
                {
                    int storeBusinUnitId = InventoryHelper.GetStoreBusinessUnit(record.StoreCode);
                    if (storeBusinUnitId != 0)
                    {
                        int bizunitindex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                        string sectorAccountSeG = HrHelper.GetBusinessUnitSectorAccount(storeBusinUnitId);
                        string newacc = _dynamicAccount.ReplaceSegment(distribution.AccountCode, bizunitindex, sectorAccountSeG);
                        distribution.AccountCode = newacc;
                        distribution.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                    }
                }
                #endregion
                _db.IssueDistributions.Add(distribution);
                _db.SaveChanges();
            }

        }
        public  void UpdateStoreReturnDistribution(StoreReturn record)
        {
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
                            string acc = itemCategory.IssueAccountCode;
                            if (!string.IsNullOrWhiteSpace(item.SubCategoryCode))
                            {
                                var subcategory = _db.ItemCategorys.Where(x => x.ItemCategoryCode == item.SubCategoryCode).FirstOrDefault();
                                if (subcategory != null)
                                    acc = subcategory.IssueAccountCode;
                            }

                            string newacc = "";
                            int costcenterindex = _dynamicAccount.GetCostCenterAccountSequenceIndex();
                            string costCenterSeG = HrHelper.GetWorkUnitCostCenter(record.OrgStructureId,branch);
                            newacc = _dynamicAccount.ReplaceSegment(acc, costcenterindex, costCenterSeG);

                            int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                            string locationAccountSeG = HrHelper.GetBranchLocationAccount(branch);
                             newacc = _dynamicAccount.ReplaceSegment(acc, locationindex, locationAccountSeG);

                            if (line.Quantity * line.UnitPrice != 0)
                            {
                                var returnDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

                                if (returnDistribution != null)
                                {
                                    returnDistribution.AccountCode = newacc;
                                    returnDistribution.Debit = 0;
                                    returnDistribution.Credit += (line.Quantity * line.UnitPrice);
                                    returnDistribution.StoreReturnID = line.StoreReturnID;
                                }
                                else
                                {
                                   var distribution = new StoreReturnDistribution
                                    {
                                        AccountCode = newacc,
                                        AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                        Debit = 0,
                                        Credit = (line.Quantity * line.UnitPrice),
                                        StoreReturnID = line.StoreReturnID,
                                    };
                                    distributions.Add(distribution);
                                }
                            }
                           
                        }
                    }
                }
              

                #endregion

                #region debit stock

                if (record.StoreReturnItems.Sum(x => x.Quantity*x.UnitPrice) != 0)
                {
                    foreach (var items in record.StoreReturnItems)
                    {
                        var item = _db.InventoryItems.Find(items.ItemCode);
                        if (item != null)
                        {
                            string newacc = item.ItemCategory.AccountCode;
                            if (!string.IsNullOrWhiteSpace(item.SubCategoryCode))
                            {
                                var subcategory = _db.ItemCategorys.Where(x => x.ItemCategoryCode == item.SubCategoryCode).FirstOrDefault();
                                if (subcategory != null)
                                    newacc = subcategory.AccountCode;
                            }

                            if (items.Quantity * items.UnitPrice != 0 )
                            {
                                int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                                string locationAccountSeG = HrHelper.GetBranchLocationAccount(branch);
                                newacc = _dynamicAccount.ReplaceSegment(newacc, locationindex, locationAccountSeG);

                                var returnDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

                                if (returnDistribution != null)
                                {
                                    returnDistribution.AccountCode = newacc;
                                    returnDistribution.Debit += (items.Quantity * items.UnitPrice);
                                    returnDistribution.Credit = 0;
                                    returnDistribution.StoreReturnID = items.StoreReturnID;
                                }
                                else
                                {
                                   var distribution = new StoreReturnDistribution
                                    {
                                        AccountCode = newacc,
                                        AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                        Debit = (items.Quantity * items.UnitPrice),
                                        Credit = 0,
                                        StoreReturnID = items.StoreReturnID
                                    };
                                    distributions.Add(distribution);
                                }
                            }
                           
                        }
                    }
                   

                }

                #endregion
            }

            foreach (var d in distributions)
            {
                var distribution = d;

                #region replace business unit
                if (GlobalHelper.IsMultipleBusinessUnit())
                {
                    int storeBusinUnitId = InventoryHelper.GetStoreBusinessUnit(record.StoreCode);
                    if (storeBusinUnitId != 0)
                    {
                        int bizunitindex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                        string sectorAccountSeG = HrHelper.GetBusinessUnitSectorAccount(storeBusinUnitId);
                        string newacc = _dynamicAccount.ReplaceSegment(distribution.AccountCode, bizunitindex, sectorAccountSeG);
                        distribution.AccountCode = newacc;
                        distribution.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                    }
                }
                #endregion
                _db.StoreReturnDistributions.Add(distribution);
                _db.SaveChanges();
            }
        }
        public  void UpdateTransferSendDistribution(ItemTransfer record)
        {
            List<ItemTransferDistribution> distributions = new List<ItemTransferDistribution>();

            var records = _db.ItemTransferDistributions.Where(x => x.ItemTransferID == record.ItemTransferID).ToList();
            _db.ItemTransferDistributions.RemoveRange(records);
            _db.SaveChanges();

            int receiverBranchId = InventoryHelper.GetStoreBranch(record.ToStore);

            if (record.ItemTransferItem != null)
            {
                
                #region debit clearing account

                var clearingAccount = _db.StockClearingAccounts.Where(x => x.Type == ClearingAccountTypes.Store_Transfer).Where(x => x.Enable).FirstOrDefault();
                if (clearingAccount != null)
                {
                    string acc = clearingAccount.AccountCode;
                    int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    int branchId = InventoryHelper.GetStoreBranch(record.FromStore);
                    string locationAccountSeG = HrHelper.GetBranchLocationAccount(branchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, locationindex, locationAccountSeG);
                    #region replace business unit
                    if (GlobalHelper.IsMultipleBusinessUnit())
                    {
                        int storeBusinUnitId = InventoryHelper.GetStoreBusinessUnit(record.FromStore);
                        if (storeBusinUnitId != 0)
                        {
                            int bizunitindex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                            string sectorAccountSeG = HrHelper.GetBusinessUnitSectorAccount(storeBusinUnitId);
                            newacc = _dynamicAccount.ReplaceSegment(newacc, bizunitindex, sectorAccountSeG);
                        }
                    }
                    #endregion

                    decimal clearingAmount = 0;
                    foreach (var items in record.ItemTransferItem)
                    {
                        clearingAmount = clearingAmount + (items.ShippedQuantity* items.UPrice);
                    }

                    if (clearingAmount != 0)
                    {
                        var sendDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

                        if (sendDistribution != null)
                        {
                            sendDistribution.AccountCode = newacc;
                            sendDistribution.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            sendDistribution.Debit += clearingAmount;
                            sendDistribution.Credit =0;
                            sendDistribution.ItemTransferID = record.ItemTransferID;
                        }
                        else
                        {
                            var distribution = new ItemTransferDistribution
                            {
                                AccountCode = newacc,
                                AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                Debit = clearingAmount,
                                Credit = 0,
                                ItemTransferID = record.ItemTransferID
                            };
                            distributions.Add(distribution);
                        }
                    }


                }
           

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
                            string acc = inventory.ItemCategory.AccountCode;
                            int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                            string locationAccountSeG = HrHelper.GetBranchLocationAccount(senderBranchId);
                            string newacc = _dynamicAccount.ReplaceSegment(acc, locationindex, locationAccountSeG);
                            #region replace business unit
                            if (GlobalHelper.IsMultipleBusinessUnit())
                            {
                                int storeBusinUnitId = InventoryHelper.GetStoreBusinessUnit(record.FromStore);
                                if (storeBusinUnitId != 0)
                                {
                                    int bizunitindex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                                    string sectorAccountSeG = HrHelper.GetBusinessUnitSectorAccount(storeBusinUnitId);
                                    newacc = _dynamicAccount.ReplaceSegment(newacc, bizunitindex, sectorAccountSeG);
                                }
                            }
                            #endregion
                            if (items.TPrice != 0)
                            {
                                var sendDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

                                if (sendDistribution != null)
                                {
                                    sendDistribution.AccountCode = newacc;
                                    sendDistribution.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                                    sendDistribution.Debit = 0;
                                    sendDistribution.Credit += (items.ShippedQuantity * items.UPrice);
                                    sendDistribution.ItemTransferID = items.ItemTransferID;
                                }
                                else
                                {
                                    var distribution = new ItemTransferDistribution
                                    {
                                        AccountCode = newacc,
                                        AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                        Debit = 0,
                                        Credit = (items.ShippedQuantity * items.UPrice),
                                        ItemTransferID = items.ItemTransferID,
                                    };
                                    distributions.Add(distribution);

                                }
                            }
                         
                        }
                    }
                  
                }

                #endregion
            }

            foreach (var d in distributions)
            {
                var distribution = d;
                _db.ItemTransferDistributions.Add(distribution);
                _db.SaveChanges();
            }
        }
        public  void UpdateTransferReceiveDistribution(ItemTransferReceive record)
        {
            List<ItemTransferReceiveDistribution> distributions = new List<ItemTransferReceiveDistribution>();

            var records = _db.ItemTransferReceiveDistribution.Where(x => x.ItemTransferReceiveId == record.ItemTransferReceiveId).ToList();
            _db.ItemTransferReceiveDistribution.RemoveRange(records);
            _db.SaveChanges();

            var receiverBranchId = InventoryHelper.GetStoreBranch(record.ToStore);
            var senderBranchId = InventoryHelper.GetStoreBranch(record.FromStore);

            if (record.ItemTransferItemReceive != null)
            {
               
                #region debit receiver stock

                if (record.ItemTransferItemReceive.Sum(x => x.TPrice) != 0)
                {
                    foreach (var items in record.ItemTransferItemReceive)
                    {
                        var item = _db.InventoryItems.Find(items.ItemCode);
                        if (item != null && receiverBranchId != 0)
                        {
                            string acc = item.ItemCategory.AccountCode;
                            if (!string.IsNullOrWhiteSpace(item.SubCategoryCode))
                            {
                                var subcategory = _db.ItemCategorys.Where(x => x.ItemCategoryCode ==item.SubCategoryCode).FirstOrDefault();
                                if (subcategory != null)
                                    acc = subcategory.AccountCode;
                            }
                            int loacationAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                            string branchSeg = HrHelper.GetBranchLocationAccount(receiverBranchId);
                            string newacc = _dynamicAccount.ReplaceSegment(acc, loacationAccountIndex, branchSeg);

                            if (items.TPrice != 0)
                            {
                                var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

                                if (receiveDistribution != null)
                                {
                                    receiveDistribution.AccountCode = newacc;
                                    receiveDistribution.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                                    receiveDistribution.Debit += (items.ReceivedQuantity * items.UPrice);
                                    receiveDistribution.Credit = 0;
                                    receiveDistribution.ItemTransferReceiveId = items.ItemTransferReceiveId;
                                }
                                else
                                {
                                    var distribution = new ItemTransferReceiveDistribution
                                    {
                                        AccountCode = newacc,
                                        AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                        Debit = (items.ReceivedQuantity * items.UPrice),
                                        Credit = 0,
                                        ItemTransferReceiveId = items.ItemTransferReceiveId,
                                    };
                                    distributions.Add(distribution);
                                }
                            }
                           
                        }
                    }
                    
                }

                #endregion

                #region credit clearing account

                var clearingAccount = _db.StockClearingAccounts.Where(x => x.Type == ClearingAccountTypes.Store_Transfer).Where(x => x.Enable).FirstOrDefault();
                if (clearingAccount != null)
                {
                    string acc = clearingAccount.AccountCode;
                    int locationindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string locationAccountSeG = HrHelper.GetBranchLocationAccount(senderBranchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, locationindex, locationAccountSeG);

                    #region replace business unit
                    if (GlobalHelper.IsMultipleBusinessUnit())
                    {
                        int storeBusinUnitId = InventoryHelper.GetStoreBusinessUnit(record.FromStore);
                        if (storeBusinUnitId != 0)                       
                        {
                            int bizunitindex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                            string sectorAccountSeG = HrHelper.GetBusinessUnitSectorAccount(storeBusinUnitId);
                             newacc = _dynamicAccount.ReplaceSegment(newacc, bizunitindex, sectorAccountSeG);
                        }
                    }
                    #endregion
                    decimal clearingAmount = 0;
                    foreach (var items in record.ItemTransferItemReceive)
                    {
                        clearingAmount = clearingAmount + (items.ReceivedQuantity * items.UPrice);
                    }

                    if (clearingAmount != 0)
                    {
                        var receiveDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

                        if (receiveDistribution != null)
                        {
                            receiveDistribution.AccountCode = newacc;
                            receiveDistribution.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            receiveDistribution.Debit = 0;
                            receiveDistribution.Credit += clearingAmount;
                            receiveDistribution.ItemTransferReceiveId = record.ItemTransferReceiveId;
                        }
                        else
                        {
                            var distribution = new ItemTransferReceiveDistribution
                            {
                                AccountCode = newacc,
                                AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                Debit = 0,
                                Credit = clearingAmount,
                                ItemTransferReceiveId = record.ItemTransferReceiveId
                            };
                            distributions.Add(distribution);
                        }
                    }


                }
               

                #endregion
                
            }

            foreach (var d in distributions)
            {
                _db.ItemTransferReceiveDistribution.Add(d);
                _db.SaveChanges();
            }
        }
        public void UpdateGoodSellingDistribution(GoodSellingAndLending record)
        {
            //throw new NotImplementedException();
        }

        //new 

        public  void GenerateIssueDistribution(int inventoryperiod,int start)
        {
            var issues = _db.Issues.Where(x => x.IsOnlineApproved && !x.IsOnlineTransferred && !x.IsVoid && x.GLPeriodId == inventoryperiod && x.IssueID >= start)
                .ToList();
            foreach (var issue in issues)
            {
                UpdateIssueDistribution(issue);
            }
        }
        public  void GenerateGoodReceiveDistribution(int inventoryperiod, int start)
        {
            var receives = _db.GoodReceives.Where(x => x.IsOnlineApproved && !x.IsOnlineTransferred && !x.IsVoid && x.GLPeriodId == inventoryperiod && x.GoodReceiveId >= start)
                .ToList();
            foreach (var receive in receives)
            {
                UpdateGoodReceiveDistribution(receive);
            }
        }
        public  void GenerateStoreReturnDistribution(int inventoryperiod, int start)
        {
            var storeReturns = _db.StoreReturns.Where(x => x.IsOnlineApproved && !x.IsOnlineTransferred && !x.IsVoid && x.GLPeriodId == inventoryperiod && x.StoreReturnID >= start)
                .ToList();
            foreach (var storeReturn in storeReturns)
            {
                UpdateStoreReturnDistribution(storeReturn);
            }
        }
        public  void GenerateTransferSendDistribution(int inventoryperiod, int start)
        {
            var itemTransfers = _db.ItemTransfers.Where(x => x.IsOnlineApproved && !x.IsOnlineTransferred && !x.IsVoid && x.GLPeriodId == inventoryperiod && x.ItemTransferID >= start)
                .ToList();
            foreach (var itemTransfer in itemTransfers)
            {
                UpdateTransferSendDistribution(itemTransfer);
            }
        }
        public  void GenerateTransferReceiveDistribution(int inventoryperiod, int start)
        {
            var transferReceives = _db.ItemTransferReceive.Where(x => x.IsOnlineApproved && !x.IsOnlineTransferred && !x.IsVoid && x.GLPeriodId == inventoryperiod && x.ItemTransferReceiveId >= start)
                .ToList();
            foreach (var itemTransfer in transferReceives)
            {
                UpdateTransferReceiveDistribution(itemTransfer);
            }
        }
    }
}
