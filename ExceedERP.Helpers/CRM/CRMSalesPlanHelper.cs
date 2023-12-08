using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using System.Data.Entity;
using System.Linq;

namespace ExceedERP.Helpers.CRM
{
    public  class CRMSalesPlanHelper
    {
        private ExceedDbContext db = new ExceedDbContext();

        public void UpdateBranchPlanOnCompanyPlanUpdate(AnnualSalesPlan salesPlan)
        {
            var branchAnnualPlans = db.BranchAnnualSalesPlans.Where(b => b.AnnualSalesPlanId == salesPlan.AnnualSalesPlanId).ToList();
            if (branchAnnualPlans.Any())
            {
                foreach (var branchPlan in branchAnnualPlans)
                {
                    var ratio = branchPlan.Ratio;
                    var qtyOfBranch = ratio / 100 * salesPlan.TotalAnnualPlanQuantity;
                    var valueOfBranchWithoutProfit = qtyOfBranch * salesPlan.AveragePrice;
                    var profitOfBranch = valueOfBranchWithoutProfit * salesPlan.Profit / 100;

                    branchPlan.Quantity = qtyOfBranch;
                    branchPlan.Value = valueOfBranchWithoutProfit + profitOfBranch;

                    db.BranchAnnualSalesPlans.Attach(branchPlan);
                    db.Entry(branchPlan).State = EntityState.Modified;
                    db.SaveChanges();

                    UpdateStoreAnnualPlansOnBranchAnnualUpdate(branchPlan);
                }
            }
        }

        public void UpdateStoreAnnualPlansOnBranchAnnualUpdate(BranchAnnualSalesPlan branchAnnualPlan)
        {
            var storeAnnualPlans = db.StoreAnnualSalesPlans.Where(p => p.BranchAnnualSalesPlanId == branchAnnualPlan.BranchAnnualSalesPlanId).ToList();
            foreach (var storePlan in storeAnnualPlans)
            {
                storePlan.AnnualQuantity = branchAnnualPlan.Quantity * storePlan.Ratio / 100;
                storePlan.AnnualValue = branchAnnualPlan.Value * storePlan.Ratio / 100;

                db.StoreAnnualSalesPlans.Attach(storePlan);
                db.Entry(storePlan).State = EntityState.Modified;
                db.SaveChanges();

                UpdateStoreMonthlyPlanOnStoreAnnualPlanUpdate(storePlan);
            }
        }
        public void UpdateStoreMonthlyPlanOnStoreAnnualPlanUpdate(StoreAnnualSalesPlan storeAnnualPlan)
        {
            var storeMonthlyPlans = db.StoreMonthlySalesPlans.Where(p => p.StoreAnnualSalesPlanId == storeAnnualPlan.StoreAnnualSalesPlanId).ToList();
            foreach (var storeMonthlyPlan in storeMonthlyPlans)
            {
                storeMonthlyPlan.Quantity = storeAnnualPlan.AnnualQuantity / 12;
                storeMonthlyPlan.Value = storeAnnualPlan.AnnualValue / 12;

                db.StoreMonthlySalesPlans.Attach(storeMonthlyPlan);
                db.Entry(storeMonthlyPlan).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
