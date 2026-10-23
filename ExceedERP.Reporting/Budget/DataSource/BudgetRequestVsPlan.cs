using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Budget;
using ExceedERP.Reporting.Budget.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data.Entity;

namespace ExceedERP.Reporting.Budget.DataSource
{
    [DataObject]
    public class BudgetRequestVsPlan
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetRequestVsPlanViewModel> GetRequestUsageVsPlan(string Year, string period, string WorkUnit, string Branch)
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
            var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
            var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
            var Workunits = glChartOfAccountCostCenters.ToList();

            if (!string.IsNullOrWhiteSpace(Year))
            {
                var fiscalyearId = int.Parse(Year);
                BudgetRequests = BudgetRequests.Where(x => x.FiscalYearId == fiscalyearId).ToList();
            }
            //if (!string.IsNullOrWhiteSpace(FromDate))
            //{
            //    DateTime fromDate = DateTime.Parse(FromDate);
            //    BudgetRequests = BudgetRequests.Where(x => x.DateCreated >= fromDate).ToList();
            //}
            //if (!string.IsNullOrWhiteSpace(ToDate))
            //{
            //    DateTime toDate = DateTime.Parse(ToDate);
               
            //    BudgetRequests = BudgetRequests.Where(x => x.DateCreated <= toDate.Date).ToList();
            //}
            if (!string.IsNullOrWhiteSpace(WorkUnit))
            {
                BudgetRequests = BudgetRequests.Where(x => x.WorkUnit == WorkUnit).ToList();
                BudgetPlans = BudgetPlans.ToList();

            }
            if (!string.IsNullOrWhiteSpace(Branch))
            {
                BudgetRequests = BudgetRequests.Where(x => x.BranchId.ToString() == Branch).ToList();
                BudgetPlans = BudgetPlans.Where(x => x.BranchId.ToString() == Branch).ToList();
            }


            foreach (var budgetrequest in BudgetRequests)
            {
                //foreach (var request in budgetrequest.BudgetRequestLine)
                //{
                foreach (var requestDetail in budgetrequest.BudgetRequestDetails)
                {
                    decimal plan = 0;
                    decimal variance = 0;
                    decimal variancePercent = 0;
                    decimal utilazation = 0;
                    int planId = 0;

                    var PlanCategory = db.PlanCategorys.Where(x => x.PlanCategoryID == budgetrequest.PlanCategoryId).FirstOrDefault();

                    if (PlanCategory!= null)
                    {
                        planId = PlanCategory.PlanCategoryID;
                    }

                    BudgetPlans = BudgetPlans.Where(x => x.AccountCode == requestDetail.AccountCode && x.PlanCategoryId == planId).ToList();//BudgetRequestLine
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
                    //decimal totalBudgetPlan = 0;
                    
                    //foreach (var budgetPlan in BudgetPlans)
                    //{
                    //    foreach (var budgetPlanMonthlys in budgetPlan.BudgetPlanMonthly)
                    //    {
                    //        totalBudgetPlan = totalBudgetPlan + budgetPlanMonthlys.CurrentBudget;
                    //    }
                    //}
                    //plan = BudgetPlans.Sum(x => x.BudgetPlanMonthly.Sum(y => y.CurrentBudget));
                    plan = BudgetPlans.Sum(x => x.CurrentYearlyBudget);
                    variance = plan - budgetrequest.Amount;
                    variancePercent = (variance / plan)*100;
                    variancePercent = Math.Round(variancePercent, 2);
                    //variance = budgetrequest.Amount - plan; /*request.Amount - plan;*///BudgetRequestLine
                    //variancePercent = variance / 100;

                    var fiscalyear = FiscalYear.Where(x => x.GlFiscalYearId == budgetrequest.FiscalYearId).FirstOrDefault();
                    if (fiscalyear != null)
                    {
                        year = fiscalyear.Name;
                    }
                   utilazation = budgetGLJournalHelper.GetGLJournalUtilization(requestDetail.AccountCode,budgetrequest.FiscalYearId);//BudgetRequestLine

                    BudgetViewModel = new BudgetRequestVsPlanViewModel
                    {
                        Branch = PlanCategory.Name,
                        Year = budgetrequest.BudgetYear,
                        WorkUnit = WorkUnitName,
                        Actual = budgetrequest.Amount, //request.Amount,//BudgetRequestLine
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
