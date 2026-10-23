using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Budget.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ExceedERP.Reporting.Budget.DataSource
{

    [DataObject]
    public class BudgetPlanODS
    {
        ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetPlanViewModel> GetBudgetPlan(string budgetYearId, string budgetCategory, bool draft)
        {

            List<BudgetPlanViewModel> list = new List<BudgetPlanViewModel>();


            if (!string.IsNullOrEmpty(budgetYearId))
            {

                int fyearId = int.Parse(budgetYearId);

                //var pcategories = 
                //    db.PlanCategorys
                //    .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                //    .Where(x => x.GlFiscalYearId == fyearId).ToList();

                var pcategories = db.PlanCategorys
                    .Where(x => x.IsDepartmentOnlineApproved)
                    .Where(x => (draft || x.IsOnlineApproved) && !x.IsVoid)
                    .Where(x => x.GlFiscalYearId == fyearId)
                    .ToList();


                if (!string.IsNullOrEmpty(budgetCategory))
                {
                    int catId = int.Parse(budgetCategory);

                    pcategories = pcategories
                           .Where(x => x.BudgetCategoryID == catId).ToList();

                }

                foreach (var item in pcategories)
                {

                    var plans = db.BudgetPlans.Where(x => x.PlanCategoryId == item.PlanCategoryID).ToList();

                    foreach (var budgetPlan in plans)
                    {
                        int bItemId = int.Parse(budgetPlan.BudgetItem);
                        var entity = new BudgetPlanViewModel
                        {

                            BudgetCategory = item.Name,
                            AccountCode = budgetPlan.AccountCode,
                            AccountDesc = budgetPlan.AccountDesc,
                            BudgetItem = db.BudgetCategoryDetails.Find(bItemId)?.BudgetItem,
                            BusinessUnit = db.BusinessUnits.Find(budgetPlan.BusinessUnitId)?.Name,
                            Branch = db.Branch.Find(budgetPlan.BranchId)?.Name,
                            CostCenter = budgetPlan.CostCenter,
                            CurrentBudget = budgetPlan.CurrentYearlyBudget,
                            ThisYearBudget = budgetPlan.TotalYearlyBudget,

                        };
                        list.Add(entity);
                    }


                }

            }
            return list;

        }
    }
}
