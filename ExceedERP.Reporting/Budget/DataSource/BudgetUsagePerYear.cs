using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Reporting.ViewModel;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Budget;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.SharedDataSources;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Reporting.Budget.DataSource
{
    [DataObject]
    public class BudgetUsagePerYear : List<BudgetAllocationAndUsageAgainstPreviousYearViewModel>
    {
        public ExceedDbContext db = new ExceedDbContext();
        BudgetGLJournalHelper BudgetGLJournalHelper = new BudgetGLJournalHelper();
        FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> CategoryTypeDDBind()
        {

            var CategoryType = db.BudgetTypes.Select(s => new SelectListItem()
            {
                Text = s.Name,
                Value = s.Name
            }).ToList();

            return CategoryType;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetAllocationAndUsageAgainstPreviousYearViewModel> GetUsageList(string Type, string BudgetCategory, string Year, string WorkUnit, string Branch, string BusinessUnit, bool draft)
        {
            List<BudgetAllocationAndUsageAgainstPreviousYearViewModel> BudgetList = new List<BudgetAllocationAndUsageAgainstPreviousYearViewModel>();

            decimal PreviousYearBudgetPlan = 0;
            decimal CurrentYearBudgetPlan = 0;
            decimal PreviousActual = 0;
            decimal CurrentActual = 0;
            decimal ActualNet = 0;
            decimal PlanNet = 0;
            decimal ActualPercentage = 0;
            decimal PlanPercentage = 0;
            //string AccCode = string.Empty;
            // string thisyear = Year;



            if (!string.IsNullOrWhiteSpace(BudgetCategory) && !string.IsNullOrWhiteSpace(Year))
            {
                int budgetCatId = int.Parse(BudgetCategory);

                int yearId = int.Parse(Year);
                int prevYearId = fiscalYearHelper.GetPreviousFYearId(yearId);
                var currYear = db.GLFiscalYears.Find(yearId);
                var previousYear = db.GLFiscalYears.Find(prevYearId);

                var budgetItems = db.BudgetCategoryDetails.AsNoTracking().ToList();

                //var currYearPlancatIds = db.PlanCategorys.Where(x => x.GlFiscalYearId == yearId && x.BudgetCategoryID == budgetCatId && x.IsOnlineApproved == true && x.IsVoid==false).Select(x => x.PlanCategoryID).ToList();
                //var prevYearPlancatIds = db.PlanCategorys.Where(x => x.GlFiscalYearId == prevYearId && x.BudgetCategoryID == budgetCatId && x.IsOnlineApproved == true && x.IsVoid == false).Select(x => x.PlanCategoryID).ToList();
                var currYearPlancatIds = db.PlanCategorys
                                            .Where(x => x.GlFiscalYearId == yearId
                                                        && x.BudgetCategoryID == budgetCatId
                                                        && x.IsDepartmentOnlineApproved
                                                        && (draft || x.IsOnlineApproved)
                                                        && !x.IsVoid)
                                            .Select(x => x.PlanCategoryID)
                                            .ToList();

                var prevYearPlancatIds = db.PlanCategorys
                                            .Where(x => x.GlFiscalYearId == prevYearId
                                                        && x.BudgetCategoryID == budgetCatId
                                                        && x.IsDepartmentOnlineApproved
                                                        && (draft || x.IsOnlineApproved)
                                                        && !x.IsVoid)
                                            .Select(x => x.PlanCategoryID)
                                            .ToList();
                var CurrentBudgetPlans = db.BudgetPlans.Where(x => currYearPlancatIds.Contains(x.PlanCategoryId)).ToList();

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

                    var PreviousBudgetPlans = db.BudgetPlans
                        .Where(x => prevYearPlancatIds.Contains(x.PlanCategoryId) && x.AccountCode == CurrentBudgetPlan.AccountCode)
                        .ToList();

                    var JournalDetails = db.GLRecordJournalDetails.Where(x => x.AccountCode == CurrentBudgetPlan.AccountCode && x.GLRecordJournal.IsOnlineApproved == true && x.GLRecordJournal.isVoid == false);


                    var PreviousYearlyActual =
                        JournalDetails.Where(x => x.GLRecordJournal.FiscalYear == previousYear.Name).ToList();

                    var CurrentYearlyActual =
                    JournalDetails.Where(x => x.GLRecordJournal.FiscalYear == currYear.Name).ToList();


                    if (PreviousBudgetPlans.Any())
                    {
                        PreviousYearBudgetPlan = PreviousBudgetPlans.Sum(x => x.TotalYearlyBudget);
                    }
                    if (PreviousYearlyActual.Any())
                    {
                        PreviousActual = PreviousYearlyActual.Sum(x => x.Credit);
                    }

                    if (CurrentBudgetPlan != null)
                    {

                        CurrentYearBudgetPlan = CurrentBudgetPlan.TotalYearlyBudget;

                    }
                    if (CurrentYearlyActual.Any())
                    {
                        CurrentActual = BudgetGLJournalHelper.GetNetJournalsBalance(CurrentYearlyActual);

                    }

                    if (PreviousYearlyActual.Any())
                    {
                        PreviousActual = BudgetGLJournalHelper.GetNetJournalsBalance(PreviousYearlyActual);

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

                    var BudgetViewModel = new BudgetAllocationAndUsageAgainstPreviousYearViewModel
                    {
                        AccountCode = CurrentBudgetPlan.AccountCode,//+ " " + CurrentBudgetPlan.AccountDesc,
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


                    BudgetList.Add(BudgetViewModel);

                }


            }
            return BudgetList;
        }

    }
}



