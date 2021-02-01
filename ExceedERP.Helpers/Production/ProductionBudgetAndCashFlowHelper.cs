using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Production
{
    public static class ProductionBudgetAndCashFlowHelper
    {
        public static void UpdateProjectedMonthlyCashflow(int productionPeriodId, int branchId)
        {
            var db = new ExceedDbContext();

            var cashFlowRecord = 
                db.ProjectedMonthlyCashFlowItems
                .FirstOrDefault(cf => cf.ProductionPeriodId == productionPeriodId && cf.ProjectedMonthlyCashFlow.BranchId == branchId);
            if(cashFlowRecord != null)
            {
                var cleanedProductSaleIncome = new decimal();
                var customCleaningAndStorageIncome = new decimal();
                var byProductSellingIncome = new decimal();

                var rawProductPurchaseExpense = new decimal();


                var monthlyCleanedProductSales = 
                    db.CleanedProductSaleItemPerMonth
                    .Where(itm => itm.ProductionPeriodId == productionPeriodId && itm.CleanedProductSaleItem.CleanedProductSale.BranchId == branchId).ToList();

                var monthlyCustomCleaningIncomes = 
                    db.CustomCleaningAndStorageItemPerMonth
                    .Where(itm => itm.ProductionPeriodId == productionPeriodId && itm.CustomCleaningAndStorageItem.CustomCleaningAndStorage.BranchId == branchId).ToList();

                var monthlyRawProductPurchases = 
                    db.RawProductPurchaseItemPerMonth
                    .Where(itm => itm.ProductionPeriodId == productionPeriodId && itm.RawProductPurchaseItem.RawProductPurchase.BranchId == branchId).ToList();

                if (monthlyCleanedProductSales.Any())
                {
                    cleanedProductSaleIncome = monthlyCleanedProductSales.Sum(s => s.Amount);
                }
                if (monthlyCustomCleaningIncomes.Any())
                {
                    customCleaningAndStorageIncome = monthlyCustomCleaningIncomes.Sum(i => i.Amount);
                }
                if (monthlyRawProductPurchases.Any())
                {
                    rawProductPurchaseExpense = monthlyRawProductPurchases.Sum(i => i.Amount);
                }

                cashFlowRecord.CleanProductSales = cleanedProductSaleIncome;
                cashFlowRecord.CustomCleaningIncome = customCleaningAndStorageIncome;
                cashFlowRecord.TotalIncome = cashFlowRecord.CleanProductSales + cashFlowRecord.CustomCleaningIncome + cashFlowRecord.ByproductSales;

                cashFlowRecord.RawProductPurchase = rawProductPurchaseExpense;
                cashFlowRecord.TotalExpenditure = cashFlowRecord.RawProductPurchase +
                                            cashFlowRecord.OtherDirectCosts +
                                            cashFlowRecord.ProcessingAndStorage +
                                            cashFlowRecord.Maintenance +
                                            cashFlowRecord.Administration +
                                            cashFlowRecord.HeadOfficeAllocation;

                cashFlowRecord.TotalCashExpenditure = cashFlowRecord.TotalExpenditure - cashFlowRecord.Depreciation;

                cashFlowRecord.NetCashIncome = cashFlowRecord.TotalIncome - cashFlowRecord.TotalCashExpenditure;
                cashFlowRecord.CumulativeCashIncome = cashFlowRecord.NetCashIncome;

                cashFlowRecord.RevisedNetCashIncome = cashFlowRecord.NetCashIncome + cashFlowRecord.AdditionalIncome;
                cashFlowRecord.RevisedCumulativeCashIncome = cashFlowRecord.RevisedNetCashIncome;

                var previousMonthlsRecords =
                    db.ProjectedMonthlyCashFlowItems
                    .Where(i => i.ProjectedMonthlyCashFlowId == cashFlowRecord.ProjectedMonthlyCashFlowId &&
                    i.ProductionPeriod.DateTo < cashFlowRecord.ProductionPeriod.DateFrom
                    ).ToList();
                if (previousMonthlsRecords.Any())
                {
                    cashFlowRecord.CumulativeCashIncome += previousMonthlsRecords.Sum(i => i.NetCashIncome);
                }

                if (previousMonthlsRecords.Any())
                {
                    cashFlowRecord.RevisedCumulativeCashIncome += previousMonthlsRecords.Sum(i => i.RevisedNetCashIncome);
                }

                db.ProjectedMonthlyCashFlowItems.Attach(cashFlowRecord);
                db.Entry(cashFlowRecord).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
