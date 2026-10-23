using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
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
using ExceedERP.Core.Domain.Finance.GL.Dynamic;

namespace ExceedERP.Reporting.Budget.DataSource
{
    [DataObject]
    public class BudgetUsagePerMonth : List<BudgetAllocationAndUsageAgainstPreviousYearMonthlyViewModel>
    {
        public ExceedDbContext db = new ExceedDbContext();
        FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();
        BudgetGLJournalHelper BudgetGLJournalHelper = new BudgetGLJournalHelper();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetAllocationAndUsageAgainstPreviousYearMonthlyViewModel> GetUsageList(string BudgetType, string BugetCategory, string Year, string WorkUnit, string Branch, string BusinessUnit,bool draft)
        {
            List<BudgetAllocationAndUsageAgainstPreviousYearMonthlyViewModel> BudgetList = new List<BudgetAllocationAndUsageAgainstPreviousYearMonthlyViewModel>();

            decimal PreviousYearBudgetPlan = 0;
            decimal CurrentYearBudgetPlan = 0;
            decimal PreviousActual = 0;
            decimal CurrentActual = 0;
            decimal ActualNet = 0;
            decimal PlanNet = 0;
            decimal ActualPercentage = 0;
            decimal PlanPercentage = 0;
          


            if (!string.IsNullOrWhiteSpace(BugetCategory) && !string.IsNullOrWhiteSpace(Year))
            {
                int budgetCatId = int.Parse(BugetCategory);
                int yearId = int.Parse(Year);
                int prevYearId = new FiscalYearHelper().GetPreviousFYearId(yearId);
                var currYear = db.GLFiscalYears.Find(yearId);
                var previousYear = db.GLFiscalYears.Find(prevYearId);

                var currYearPeriods=db.GLPeriods.Where(x => x.GlFiscalYearId == yearId).OrderBy(x=>x.DateFrom).Select(x=> new { x.Name,x.GLPeriodId,x.Index}).ToList();
                var budgetItems = db.BudgetCategoryDetails.AsNoTracking().ToList();

               // var currYearPlancatIds = db.PlanCategorys.Where(x => x.GlFiscalYearId == yearId && x.BudgetCategoryID == budgetCatId && x.IsOnlineApproved  && x.IsVoid == false).Select(x => x.PlanCategoryID).ToList();
                var currYearPlancatIds = db.PlanCategorys
                                        .Where(x => x.GlFiscalYearId == yearId
                                                    && x.BudgetCategoryID == budgetCatId
                                                    && x.IsDepartmentOnlineApproved
                                                    && (draft || x.IsOnlineApproved)
                                                    && x.IsVoid == false)
                                        .Select(x => x.PlanCategoryID)
                                        .ToList();
                //  var prevYearPlancatIds = db.PlanCategorys.Where(x => x.GlFiscalYearId == prevYearId && x.BudgetCategoryID == budgetCatId && x.IsOnlineApproved == true && x.IsVoid == false).Select(x => x.PlanCategoryID).ToList();

                var CurrentBudgetPlans = db.BudgetPlans.Where(x => currYearPlancatIds.Contains(x.PlanCategoryId)).ToList();



                foreach (var p in currYearPeriods)
                {
                    var record = new BudgetAllocationAndUsageAgainstPreviousYearMonthlyViewModel
                    {
                        Month = p.Name,
                        List = new List<BudgetAllocationAndUsageAgainstPreviousYearViewModel>()
                    };

                    int prevPeriodId = fiscalYearHelper.GetParallelPeriodInYear(p.GLPeriodId, prevYearId);
                    var prevPeriod = db.GLPeriods.Find(prevPeriodId);

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
                        var currMonthlyPlan = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == CurrentBudgetPlan.BudgetPlanID && x.GLPeriodId == p.GLPeriodId).FirstOrDefault();
                        var prevMonthlyPlan = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == CurrentBudgetPlan.BudgetPlanID && x.GLPeriodId == prevPeriodId).FirstOrDefault();
                                              

                        var JournalDetails = db.GLRecordJournalDetails.Where(x => x.AccountCode == CurrentBudgetPlan.AccountCode && x.GLRecordJournal.IsOnlineApproved == true && x.GLRecordJournal.isVoid == false);

                        var currMonthlyActual = JournalDetails.Where(x => x.GLRecordJournal.PeriodName == p.Name).ToList();
                        var previousMonthlyActual = JournalDetails.Where(x => x.GLRecordJournal.FiscalYear == prevPeriod.Name).ToList();                       


                        if (prevMonthlyPlan!=null)
                        {
                            PreviousYearBudgetPlan = prevMonthlyPlan.CurrentBudget;
                        }
                        if (previousMonthlyActual.Any())
                        {
                            PreviousActual = previousMonthlyActual.Sum(x => x.Credit);
                        }

                        if (currMonthlyPlan != null)
                        {

                            CurrentYearBudgetPlan = currMonthlyPlan.CurrentBudget;

                        }
                        if (currMonthlyActual.Any())
                        {
                            CurrentActual = BudgetGLJournalHelper.GetNetJournalsBalance(currMonthlyActual);

                        }

                        if (previousMonthlyActual.Any())
                        {
                            PreviousActual = BudgetGLJournalHelper.GetNetJournalsBalance(previousMonthlyActual);

                        }

                        ActualNet = CurrentActual - PreviousActual;
                        PlanNet = CurrentYearBudgetPlan - PreviousYearBudgetPlan;



                        if (ActualNet > 0)
                        {
                            ActualPercentage = (ActualNet / BudgetGenericHelper.DivideByZero(PreviousActual));
                        }
                        else
                        {
                            ActualPercentage = 0;
                        }
                        if (PlanNet > 0)
                        {
                            PlanPercentage = (PlanNet / BudgetGenericHelper.DivideByZero(PreviousYearBudgetPlan));
                        }

                        int budgetItemId = int.Parse(CurrentBudgetPlan.BudgetItem);

                        var detail = new BudgetAllocationAndUsageAgainstPreviousYearViewModel
                        {
                            AccountCode = CurrentBudgetPlan.AccountCode,// + " "+ CurrentBudgetPlan.AccountDesc,
                            Unit = "",
                            // WorkUnit = db.MainCompanyStructures.Find(CurrentBudgetPlan.OrgStructureId)?.Name,
                            BudgetCode = CurrentBudgetPlan.BudgetCode + " " + budgetItems.FirstOrDefault(x => x.BudgetCategoryDetailID == budgetItemId)?.BudgetItem,
                            PreviousYearBudget = PreviousYearBudgetPlan,
                            PreviousYearActual = PreviousActual,
                            ThisYearActual = CurrentActual,
                            ThisYearBudget = CurrentYearBudgetPlan,
                            ActualDifference = ActualNet,
                            BudgetDifference = PlanNet,
                            BudgetDifferencePercentage = PlanPercentage,
                            ActualDifferencePercentage = ActualPercentage,
                        };


                        record.List.Add(detail);

                    }

                    BudgetList.Add(record);
                }                             

            }

            return BudgetList;
        }
    }

}




