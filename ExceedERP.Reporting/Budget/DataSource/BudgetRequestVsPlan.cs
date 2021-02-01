using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Budget;
using ExceedERP.Reporting.Budget.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Budget.DataSource
{
    [DataObject]
    public class BudgetRequestVsPlan
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetRequestVsPlanViewModel> GetRequestUsageVsPlan(string Year, string FromDate, string ToDate, string WorkUnit, string Branch)
        {
            List<BudgetRequestVsPlanViewModel> list = new List<BudgetRequestVsPlanViewModel>();
            BudgetRequestVsPlanViewModel BudgetViewModel;
            BudgetGLJournalHelper budgetGLJournalHelper = new BudgetGLJournalHelper();
            var BudgetPlans = db.BudgetPlans.ToList();
            var BudgetPlanMonth = db.BudgetPlanMonthlys.ToList();
            var BudgetRequests = db.BudgetRequests.Where(x => x.IsSecondOnlineApproved).ToList();
            var FiscalYear = db.GLFiscalYears.ToList();
            var Branches = db.Branch.ToList();
            var BudgetCategorys = db.BudgetCategorys.ToList();
            var Workunits = db.GLChartOfAccountCostCenters.ToList();

            if (!string.IsNullOrWhiteSpace(Year))
            {
                var fiscalyear = int.Parse(Year);
                BudgetRequests = BudgetRequests.Where(x => x.FiscalYearId == fiscalyear).ToList();
            }
            if (!string.IsNullOrWhiteSpace(FromDate))
            {
                DateTime date = DateTime.Parse(FromDate);
                BudgetRequests = BudgetRequests.Where(x => x.DateCreated >= date).ToList();
            }
            if (!string.IsNullOrWhiteSpace(ToDate))
            {
                DateTime date = DateTime.Parse(ToDate);
                BudgetRequests = BudgetRequests.Where(x => x.DateCreated <= date).ToList();
            }
            if (!string.IsNullOrWhiteSpace(WorkUnit))
            {
                BudgetRequests = BudgetRequests.Where(x => x.WorkUnit == WorkUnit).ToList();
                BudgetPlans = BudgetPlans.Where(x => x.GLChartOfAccountCostCenterId.ToString() == WorkUnit).ToList();

            }
            if (!string.IsNullOrWhiteSpace(Branch))
            {
                BudgetRequests = BudgetRequests.Where(x => x.BranchId.ToString() == Branch).ToList();
                BudgetPlans = BudgetPlans.Where(x => x.BranchId.ToString() == Branch).ToList();
            }


            foreach (var budgetrequest in BudgetRequests)
            {
                foreach (var request in budgetrequest.BudgetRequestLine)
                {
                    decimal plan = decimal.Zero;
                    decimal variance = decimal.Zero;
                    decimal variancePercent = decimal.Zero;
                    decimal utilazation = decimal.Zero;
                    int planId = 0;
                    var PlanCategory = db.PlanCategorys.Where(x => x.Name == budgetrequest.PlanCategoryId).FirstOrDefault();
                    if (PlanCategory!= null)
                    {
                        planId = PlanCategory.PlanCategoryID;
                    }
                    BudgetPlans = BudgetPlans.Where(x => x.AccountCode == request.AccountCode && x.BudgetCategoryID == planId).ToList();
                    string year = string.Empty;
                    string transferbranch = string.Empty;
                 
                    var requestBranch = Branches.Where(x => x.BranchId  == budgetrequest.BranchId).FirstOrDefault();

                    string WorkUnitName = string.Empty;

                    var requestWorkUnit = Workunits.Where(x => x.Values == budgetrequest.WorkUnit).FirstOrDefault();
                    if (requestWorkUnit != null)
                    {
                        WorkUnitName = requestWorkUnit.Description;
                    }

                    if (requestBranch != null)
                    {
                        transferbranch = requestBranch.Name;
                    }

                    plan = BudgetPlans.Sum(x => x.BudgetPlanMonthly.Sum(y => y.CurrentBudget));
                    variance = request.Amount - plan;
                    variancePercent = variance / 100;

                    var fiscalyear = FiscalYear.Where(x => x.GlFiscalYearId == budgetrequest.FiscalYearId).FirstOrDefault();
                    if (fiscalyear != null)
                    {
                        year = fiscalyear.Name;
                    }
                   utilazation = budgetGLJournalHelper.GetGLJournalUtilization(request.AccountCode,budgetrequest.FiscalYearId);
                 
                    BudgetViewModel = new BudgetRequestVsPlanViewModel
                    {
                        Branch = budgetrequest.PlanCategoryId,
                        Year = budgetrequest.BudgetYear,
                        WorkUnit = WorkUnitName,
                        Actual = request.Amount,
                        Plan = plan,
                        Variance = variance,
                        VariancePercent = variancePercent,
                        Utilaization = utilazation
                    };

                    list.Add(BudgetViewModel);
                }
            }



            return list;
        }

    }
}
