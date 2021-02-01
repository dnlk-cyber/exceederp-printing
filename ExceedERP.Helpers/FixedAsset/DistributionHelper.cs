using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FixedAsset.Depreciation;
using ExceedERP.Core.Domain.FixedAsset.Receives;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.HR;
using ExceedERP.Helpers.Inventory;

namespace ExceedERP.Helpers.FixedAsset
{
    public  class FixedAssetDistributionHelper
    {
        private AccountHelper _accountHelper;

        // fixed asset
        public  void UpdateGoodReceiveDistribution(FixedAssetReceive record)
        {
            ExceedDbContext db = new ExceedDbContext();
            var distributions = db.FixedAssetReceiveDistributions.Where(x => x.FixedAssetReceiveId == record.FixedAssetReceiveId);
            db.FixedAssetReceiveDistributions.RemoveRange(distributions);
            db.SaveChanges();

            if (record.ReceiveType.Type == GoodReceiveType.Other)
            {
                DonationGoodReceiveDistribution(record);
            }
            else
            {
                PurchaseGoodReceiveDistribution(record);
            }
        }
        public  void PurchaseGoodReceiveDistribution(FixedAssetReceive record)
        {
            ExceedDbContext db = new ExceedDbContext();
            var supplier = db.ProcurementSuppliers.FirstOrDefault(x => x.Name == record.SupplierName);

            if (record.FixedAssetReceiveItem != null)
            {
                #region debit stock

                FixedAssetReceiveDistribution distribution;
                foreach (var line in record.FixedAssetReceiveItem)
                {
                    if (line.SubTotal != 0)
                    {
                        var fixedAssetCategory = db.FixedAssetCategories.Find(line.FixedAssetCategoryID);
                        if (fixedAssetCategory != null)
                        {
                            string acc = "";
                            var depreciationRule = db.DepretiationRules.FirstOrDefault(x => x.FixedAssetGroupID == fixedAssetCategory.FixedAssetGroupID);
                            if (depreciationRule != null)
                                acc = depreciationRule.AccountCode;
                            //string[] account = acc.Split('-');
                            //string acccountSeG = account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                            //string locationSeG = account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                            //string costCenterSeG =account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                            //string subAccountSeG = HrHelper.GetBranchSubAccount(record.BranchId);
                            //string newacc = AccountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,
                            //    costCenterSeG, subAccountSeG);

                            distribution = new FixedAssetReceiveDistribution
                            {
                                AccountCode = acc,
                                Debit = line.Total,
                                Credit = 0,
                                FixedAssetReceiveId = line.FixedAssetReceiveId,
                            };
                            db.FixedAssetReceiveDistributions.Add(distribution);
                            db.SaveChanges();
                        }

                    }
                }

                #endregion

                #region debit tax

                foreach (var line in record.FixedAssetReceiveItem)
                {
                    if (line.TaxAmount != 0)
                    {
                        var tax = db.GLTaxRates.Find(line.GLTaxRateID);
                        if (tax != null)
                        {
                            string acc = tax.ApAccountCode;
                            //string[] account = acc.Split('-');
                            //string acccountSeG = account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                            //string locationSeG = account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                            //string costCenterSeG =account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                            //string subAccountSeG = HrHelper.GetBranchSubAccount(record.BranchId);
                            //string newacc = AccountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,
                            //    costCenterSeG, subAccountSeG);

                            distribution = new FixedAssetReceiveDistribution
                            {
                                AccountCode = acc,
                                Debit = line.TaxAmount,
                                Credit = 0,
                                FixedAssetReceiveId = line.FixedAssetReceiveId,
                            };
                            db.FixedAssetReceiveDistributions.Add(distribution);
                            db.SaveChanges();
                        }

                    }
                }

                #endregion

                if (supplier != null && supplier.IsCredit)
                {
                    #region Credit supplier

                    if (record.FixedAssetReceiveItem.Sum(x => x.Total) != 0)
                    {
                        distribution = new FixedAssetReceiveDistribution
                        {
                            AccountCode = supplier.AccountCode,
                            Debit = 0,
                            Credit = record.FixedAssetReceiveItem.Sum(x => x.Total),
                            FixedAssetReceiveId = record.FixedAssetReceiveId
                        };

                        db.FixedAssetReceiveDistributions.Add(distribution);
                        db.SaveChanges();
                    }

                    #endregion
                }
                else 
                {
                    #region Credit common supplier

                    var commonSupplier = db.ApSetupCommonSuppliers.FirstOrDefault();
                    if (record.FixedAssetReceiveItem.Sum(x => x.Total) != 0)
                    {
                        if (commonSupplier != null)
                        {
                            distribution = new FixedAssetReceiveDistribution
                            {
                                AccountCode = commonSupplier.AccountCode,
                                Debit = 0,
                                Credit = record.FixedAssetReceiveItem.Sum(x => x.Total),
                                FixedAssetReceiveId = record.FixedAssetReceiveId
                            };

                            db.FixedAssetReceiveDistributions.Add(distribution);
                            db.SaveChanges();
                        }
                    }

                    #endregion
                }
            }

        }
        public  void DonationGoodReceiveDistribution(FixedAssetReceive record)
        {
            ExceedDbContext db = new ExceedDbContext();
            var revenueAccount = db.RevenueAccounts.FirstOrDefault(x => x.ApplyToInventory);

            if (record.FixedAssetReceiveItem!=null)
            {
                #region debit stock

                FixedAssetReceiveDistribution distribution;
                foreach (var line in record.FixedAssetReceiveItem)
                {
                    if (line.SubTotal != 0)
                    {
                        var fixedAssetCategory = db.FixedAssetCategories.Find(line.FixedAssetCategoryID);
                        if (fixedAssetCategory != null)
                        {
                            string acc = "";
                            var depreciationRule = db.DepretiationRules.FirstOrDefault(x=>x.FixedAssetGroupID==fixedAssetCategory.FixedAssetGroupID);
                            if (depreciationRule != null)
                                acc = depreciationRule.AccountCode;

                            //string[] account = acc.Split('-');
                            //string acccountSeG = account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                            //string locationSeG = account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                            //string costCenterSeG =account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                            //string subAccountSeG = HrHelper.GetBranchSubAccount(record.BranchId);
                            //string newacc = AccountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,
                            //    costCenterSeG, subAccountSeG);

                            distribution = new FixedAssetReceiveDistribution
                            {
                                AccountCode = acc,
                                Debit = line.Total,
                                Credit = 0,
                                FixedAssetReceiveId = line.FixedAssetReceiveId,
                            };
                            db.FixedAssetReceiveDistributions.Add(distribution);
                            db.SaveChanges();
                        }

                    }
                }

                #endregion

                #region credit tax payable

                foreach (var line in record.FixedAssetReceiveItem)
                {
                    if (line.TaxAmount != 0)
                    {
                        var tax = db.GLTaxRates.Find(line.GLTaxRateID);
                        if (tax != null)
                        {
                            string acc = tax.ApAccountCode;
                            //string[] account = acc.Split('-');
                            //string acccountSeG = account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                            //string locationSeG = account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                            //string costCenterSeG =account[AccountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                            //string subAccountSeG = HrHelper.GetBranchSubAccount(record.BranchId);
                            //string newacc = AccountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,
                            //    costCenterSeG, subAccountSeG);

                            distribution = new FixedAssetReceiveDistribution
                            {
                                AccountCode = acc,
                                Debit = 0,
                                Credit = line.TaxAmount,
                                FixedAssetReceiveId = line.FixedAssetReceiveId,
                            };
                            db.FixedAssetReceiveDistributions.Add(distribution);
                            db.SaveChanges();
                        }

                    }
                }

                #endregion

                #region Credit revenue

                if (record.FixedAssetReceiveItem.Sum(x => x.SubTotal) != 0)
                {
                    if (revenueAccount != null)
                    {
                        distribution = new FixedAssetReceiveDistribution
                        {
                            AccountCode = revenueAccount.InventoryAccountCode,
                            Debit = 0,
                            Credit = record.FixedAssetReceiveItem.Sum(x => x.SubTotal),
                            FixedAssetReceiveId = record.FixedAssetReceiveId
                        };

                        db.FixedAssetReceiveDistributions.Add(distribution);
                        db.SaveChanges();
                    }
                }

                #endregion
            }

        }

        //depreciation distribution helper
        public  void UpdateMonthlyDepreciationDistribution(int id)
        {
          ExceedDbContext db = new ExceedDbContext();

         List<MonthlyDepreciationDistribution> distributions = new List<MonthlyDepreciationDistribution>();
         _accountHelper = new AccountHelper();

            var monthlyDepreciation = db.MonthlyDepreciations.Find(id);
            if (monthlyDepreciation != null)
            {
                #region clear existing distribution
                var record = db.MonthlyDepreciationDistributions.Where(x => x.MonthlyDepreciationId == id).ToList();
                db.MonthlyDepreciationDistributions.RemoveRange(record);
                db.SaveChanges();
                #endregion
                var lines = db.MonthlyDepreciationItems.Where(x => x.MonthlyDepreciationId == id).AsNoTracking().ToList();
                foreach (var item in lines)
                {
                    var depreciationRule = db.DepretiationRules.FirstOrDefault(x => x.FixedAssetGroupID == item.AssetRegistration.FixedAssetCategory.FixedAssetGroupID);
                    
                    #region debit depreciation

                   
                    if (item.DepreciationAmount > 0 && depreciationRule!=null)
                    {
                        string accountCode = depreciationRule.AccountCode;
                        var invoiceDistribution = distributions.FirstOrDefault(x => x.AccountCode == accountCode);
                        if (invoiceDistribution != null)
                        {
                            invoiceDistribution.AccountCode = accountCode;
                            invoiceDistribution.AccountDesc = _accountHelper.GetAccountDesc(accountCode);
                            invoiceDistribution.Debit += item.DepreciationAmount;
                            invoiceDistribution.Credit = 0;
                            invoiceDistribution.MonthlyDepreciationId = item.MonthlyDepreciationId;
                        }
                        else
                        {
                            MonthlyDepreciationDistribution defaultDistribution = new MonthlyDepreciationDistribution
                            {
                                AccountCode = accountCode,
                                AccountDesc = _accountHelper.GetAccountDesc(accountCode),
                                Debit = item.DepreciationAmount,
                                Credit = 0,
                                MonthlyDepreciationId = item.MonthlyDepreciationId
                            };

                            distributions.Add(defaultDistribution);
                        }


                    }


                    #endregion
                    #region credit accumulated depreciation


                    if (item.DepreciationAmount > 0 && depreciationRule != null)
                    {
                        string accountCode = depreciationRule.AccumulatedAccount;
                        var invoiceDistribution = distributions.FirstOrDefault(x => x.AccountCode == accountCode);
                        if (invoiceDistribution != null)
                        {
                            invoiceDistribution.AccountCode = accountCode;
                            invoiceDistribution.AccountDesc = _accountHelper.GetAccountDesc(accountCode);
                            invoiceDistribution.Debit = 0;
                            invoiceDistribution.Credit += item.DepreciationAmount;
                            invoiceDistribution.MonthlyDepreciationId = item.MonthlyDepreciationId;
                        }
                        else
                        {
                            MonthlyDepreciationDistribution defaultDistribution = new MonthlyDepreciationDistribution
                            {
                                AccountCode = accountCode,
                                AccountDesc = _accountHelper.GetAccountDesc(accountCode),
                                Debit =0,
                                Credit = item.DepreciationAmount,
                                MonthlyDepreciationId = item.MonthlyDepreciationId
                            };

                            distributions.Add(defaultDistribution);
                        }


                    }


                    #endregion
                }

                //persist
                foreach (var distribution in distributions)
                {
                    db.MonthlyDepreciationDistributions.Add(distribution);
                    db.SaveChanges();
                }
            }

        }
        public void UpdateAnnualDepreciationDistribution(int id)
        {
            ExceedDbContext db = new ExceedDbContext();

            List<AnnualDepreciationDistribution> distributions = new List<AnnualDepreciationDistribution>();
            _accountHelper = new AccountHelper();

            var annualDepreciation = db.AnnualDepreciations.Find(id);
            if (annualDepreciation != null)
            {
                #region clear existing distribution
                var record = db.AnnualDepreciationDistributions.Where(x => x.AnnualDepreciationId == id).ToList();
                db.AnnualDepreciationDistributions.RemoveRange(record);
                db.SaveChanges();
                #endregion
                var lines = db.AnnualDepreciationItems.Where(x => x.AnnualDepreciationId == id).AsNoTracking().ToList();
                foreach (var item in lines)
                {
                    var depreciationRule = db.DepretiationRules.FirstOrDefault(x => x.FixedAssetGroupID == item.AssetRegistration.FixedAssetCategory.FixedAssetGroupID);

                    #region debit depreciation


                    if (item.DepreciationAmount > 0 && depreciationRule != null)
                    {
                        string accountCode = depreciationRule.AccountCode;
                        var invoiceDistribution = distributions.FirstOrDefault(x => x.AccountCode == accountCode);
                        if (invoiceDistribution != null)
                        {
                            invoiceDistribution.AccountCode = accountCode;
                            invoiceDistribution.AccountDesc = _accountHelper.GetAccountDesc(accountCode);
                            invoiceDistribution.Debit += item.DepreciationAmount;
                            invoiceDistribution.Credit = 0;
                            invoiceDistribution.AnnualDepreciationId = item.AnnualDepreciationId;
                        }
                        else
                        {
                            AnnualDepreciationDistribution defaultDistribution = new AnnualDepreciationDistribution
                            {
                                AccountCode = accountCode,
                                AccountDesc = _accountHelper.GetAccountDesc(accountCode),
                                Debit = item.DepreciationAmount,
                                Credit = 0,
                                AnnualDepreciationId = item.AnnualDepreciationId
                            };

                            distributions.Add(defaultDistribution);
                        }


                    }


                    #endregion
                    #region credit accumulated depreciation


                    if (item.DepreciationAmount > 0 && depreciationRule != null)
                    {
                        string accountCode = depreciationRule.AccumulatedAccount;
                        var invoiceDistribution = distributions.FirstOrDefault(x => x.AccountCode == accountCode);
                        if (invoiceDistribution != null)
                        {
                            invoiceDistribution.AccountCode = accountCode;
                            invoiceDistribution.AccountDesc = _accountHelper.GetAccountDesc(accountCode);
                            invoiceDistribution.Debit = 0;
                            invoiceDistribution.Credit += item.DepreciationAmount;
                            invoiceDistribution.AnnualDepreciationId = item.AnnualDepreciationId;
                        }
                        else
                        {
                            AnnualDepreciationDistribution defaultDistribution = new AnnualDepreciationDistribution
                            {
                                AccountCode = accountCode,
                                AccountDesc = _accountHelper.GetAccountDesc(accountCode),
                                Debit = 0,
                                Credit = item.DepreciationAmount,
                                AnnualDepreciationId = item.AnnualDepreciationId
                            };

                            distributions.Add(defaultDistribution);
                        }


                    }


                    #endregion
                }

                //persist
                foreach (var distribution in distributions)
                {
                    db.AnnualDepreciationDistributions.Add(distribution);
                    db.SaveChanges();
                }
            }

        }
    }
}
