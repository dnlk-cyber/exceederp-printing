using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Reporting.ViewModel;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Helpers.Budget;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using System.Web.Mvc;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Migrations;

namespace ExceedERP.Reporting.Budget.DataSource
{
    [DataObject]
    public class BudgetAllocationUsage : List<BudgetAllocationAndUsageViewModel>
    {
        public ExceedDbContext db = new ExceedDbContext();

        public BudgetGLJournalHelper BudgetGLJournalHelper = new BudgetGLJournalHelper();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GLChartOfAccountSegment> GetWorkUnit()
        {
            var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
            var glChartOfAccountCostCenters =
                db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
            return glChartOfAccountCostCenters.ToList();

        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetBudgetCategory(string name)
        {
            return db.BudgetCategorys.Where(x => x.BudgetType.Equals(name)).Select(s => new SelectListItem()
            {
                Text = s.Name,
                Value = s.BudgetCategoryID.ToString()
            }).ToList();


        }



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> BranchDDBind()
        {
            return db.Branch.Select(s => new SelectListItem()
            {
                Text = s.Name,
                Value = s.BranchId.ToString()
            }).ToList();
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetPlan> PlanDDBind()
        {
            List<BudgetPlan> result = db.BudgetPlans.DistinctByField(x => x.AccountCode).ToList();

            return result;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetAllocationAndUsageViewModel> GetUsageList(string BudgetType, string BugetCategory, string Year, string WorkUnit, string Branch, string BusinessUnit, string from, string to, bool draft)
        {
            List<BudgetAllocationAndUsageViewModel> BudgetList = new List<BudgetAllocationAndUsageViewModel>();

            decimal monthlyBudget = 0;
            decimal monthlyActual = 0;
            decimal monthlyDiff = 0;

            var budgetItems = db.BudgetCategoryDetails.AsNoTracking().ToList();



            if (!string.IsNullOrWhiteSpace(BugetCategory) && !string.IsNullOrWhiteSpace(Year))
            {
                int budgetCatId = int.Parse(BugetCategory);
                int yearId = int.Parse(Year);

                var currYearPeriods = db.GLPeriods.Where(x => x.GlFiscalYearId == yearId).OrderBy(x => x.DateFrom).Select(x => new { x.Name, x.GLPeriodId, x.Index, x.DateFrom, x.DateTo }).AsQueryable();

                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
                {
                    int fromPeriodD = int.Parse(from);
                    int toPeriodD = int.Parse(to);

                    var fromP = db.GLPeriods
                        .Select(x => new { x.DateFrom, x.DateTo, x.GLPeriodId })//added
                        .FirstOrDefault(x => x.GLPeriodId == fromPeriodD);

                    var toP = db.GLPeriods
                        .Select(x => new { x.DateFrom, x.DateTo, x.GLPeriodId })//added
                        .FirstOrDefault(x => x.GLPeriodId == toPeriodD);


                    currYearPeriods = currYearPeriods
                                                .Where(x => x.DateFrom >= fromP.DateFrom)
                                                  .Where(x => x.DateTo <= toP.DateTo)

                                                    .AsQueryable();
                }

                //var currYearPlancatIds = db.PlanCategorys.Where(x => x.GlFiscalYearId == yearId && x.BudgetCategoryID == budgetCatId && x.IsOnlineApproved == true && x.IsVoid == false).Select(x => x.PlanCategoryID).ToList();
                var currYearPlancatIds = db.PlanCategorys
                                    .Where(x => x.GlFiscalYearId == yearId
                                                && x.IsDepartmentOnlineApproved
                                                && x.BudgetCategoryID == budgetCatId
                                                && (draft || x.IsOnlineApproved)
                                                && !x.IsVoid)
                                    .Select(x => x.PlanCategoryID)
                                    .ToList();
                var CurrentBudgetPlans = db.BudgetPlans.Where(x => currYearPlancatIds.Contains(x.PlanCategoryId)).ToList();



                foreach (var p in currYearPeriods)
                {
                    if (!string.IsNullOrWhiteSpace(WorkUnit))
                    {
                        CurrentBudgetPlans = CurrentBudgetPlans.Where(x => x.CostCenter == WorkUnit).ToList();
                    }

                    if (!string.IsNullOrWhiteSpace(Branch))
                    {
                        int branchId = int.Parse(Branch);
                        CurrentBudgetPlans = CurrentBudgetPlans.Where(x => x.BranchId == branchId).ToList();
                    }

                    if (!string.IsNullOrWhiteSpace(BusinessUnit))
                    {
                        int bizUnitId = int.Parse(BusinessUnit);
                        CurrentBudgetPlans = CurrentBudgetPlans.Where(x => x.BusinessUnitId == bizUnitId).ToList();
                    }

                    foreach (var CurrentBudgetPlan in CurrentBudgetPlans)
                    {
                        monthlyBudget = 0;
                        monthlyActual = 0;
                        
                        var currMonthlyPlan = db.BudgetPlanMonthlys.AsNoTracking().Where(x => x.BudgetDefineID == CurrentBudgetPlan.BudgetPlanID && x.GLPeriodId == p.GLPeriodId).FirstOrDefault();
                        var JournalDetails = db.GLRecordJournalDetails.AsNoTracking().Where(x => x.AccountCode == CurrentBudgetPlan.AccountCode && x.GLRecordJournal.IsOnlineApproved && !x.GLRecordJournal.isVoid);

                        var currMonthlyActual = JournalDetails.Where(x => x.GLRecordJournal.PeriodName == p.Name).ToList();

                        if (currMonthlyPlan != null)
                        {

                            monthlyBudget = currMonthlyPlan.CurrentBudget;

                        }
                        if (currMonthlyActual.Any())
                        {
                            monthlyActual = BudgetGLJournalHelper.GetNetJournalsBalance(currMonthlyActual);

                        }

                        monthlyDiff = monthlyBudget - monthlyActual;
                        int budgetItemId = int.Parse(CurrentBudgetPlan.BudgetItem);
                        var record = new BudgetAllocationAndUsageViewModel
                        {
                            Index = p.Index,
                            Month = p.Name,
                            AccountCode = CurrentBudgetPlan.AccountCode,// + " "+ CurrentBudgetPlan.AccountDesc,
                            Unit = "",
                            // WorkUnit = db.MainCompanyStructures.Find(CurrentBudgetPlan.OrgStructureId)?.Name,
                            BudgetCode = CurrentBudgetPlan.BudgetCode + " " + budgetItems.FirstOrDefault(x => x.BudgetCategoryDetailID == budgetItemId)?.BudgetItem,

                            MonthlyActuals = monthlyActual,
                            MonthlyBudget = monthlyBudget,
                            MonthlyVariance = monthlyDiff,
                        };


                        BudgetList.Add(record);

                    }


                }

            }



            return BudgetList;
        }


    }


}




