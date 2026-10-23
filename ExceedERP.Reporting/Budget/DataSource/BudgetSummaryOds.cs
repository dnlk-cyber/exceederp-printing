using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Reporting.ViewModel;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Budget;
using ExceedERP.Helpers.Finance;
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
    public class BudgetSummaryOds : List<BudgetTypeVM>
    {
        public ExceedDbContext db = new ExceedDbContext();

        public BudgetGLJournalHelper BudgetGLJournalHelper = new BudgetGLJournalHelper();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetTypeVM> GetBudgetSummary(string Year, string Branch, string BusinessUnit, bool draft)
        {
            if (string.IsNullOrWhiteSpace(Year))
            {
                return new List<BudgetTypeVM>();
            }

            int yearId = int.Parse(Year);
            var currYear = db.GLFiscalYears.Find(yearId);
            //var currYearPlanCategory = db.PlanCategorys.Where(x => x.GlFiscalYearId == yearId && x.IsOnlineApproved == true && x.IsVoid == false).Select(x => new { x.PlanCategoryID, x.BudgetCategoryID, x.BudgetType }).ToList();
            var currYearPlanCategory = db.PlanCategorys
                .Where(x => x.GlFiscalYearId == yearId
                            && x.IsDepartmentOnlineApproved
                            && (draft || x.IsOnlineApproved)
                            && !x.IsVoid)
                .Select(x => new
                {
                    x.PlanCategoryID,
                    x.BudgetCategoryID,
                    x.BudgetType
                })
                .ToList();
            var currYearCateIds = currYearPlanCategory.Select(x => x.BudgetCategoryID).ToList();
            var currYearPlancatIds = currYearPlanCategory.Select(x => x.PlanCategoryID).ToList();
            var currYearBudgetTypes = currYearPlanCategory.Select(x => x.BudgetType).ToList();
            var currentBudgetPlans = db.BudgetPlans.Where(x => currYearPlancatIds.Contains(x.PlanCategoryId)).AsQueryable();

            if (!string.IsNullOrWhiteSpace(Branch))
            {
                int branchId = int.Parse(Branch);
                currentBudgetPlans = currentBudgetPlans.Where(x => x.BranchId == branchId);
            }

            if (!string.IsNullOrWhiteSpace(BusinessUnit))
            {
                int bizUnitId = int.Parse(BusinessUnit);
                currentBudgetPlans = currentBudgetPlans.Where(x => x.BusinessUnitId == bizUnitId);
            }

            var plan = currentBudgetPlans.ToList();
            var currYearBudgetItemIds = currentBudgetPlans.Select(x => x.BudgetItem).ToList();


            var budgetTypes = db.BudgetTypes.Where(x => currYearBudgetTypes.Contains(x.Name)).ToList();
            var budgetCategories = db.BudgetCategorys.Where(x => currYearCateIds.Contains(x.BudgetCategoryID)).ToList();
            var budgetCategoryDetails = db.BudgetCategoryDetails.Where(x => currYearBudgetItemIds.Contains(x.BudgetCategoryDetailID.ToString())).ToList();

            var JournalDetails = db.GLRecordJournalDetails.Where(x => x.GLRecordJournal.FiscalYear == currYear.Name && x.GLRecordJournal.IsOnlineApproved && !x.GLRecordJournal.isVoid);

            var list = budgetTypes
                .Select(bt =>
                {
                    var categories = budgetCategories
                        .Where(bc => bc.BudgetType == bt.Name)
                        .Select(bc =>
                        {
                            int planCategoryId = currYearPlanCategory.FirstOrDefault(x => x.BudgetCategoryID == bc.BudgetCategoryID)?.PlanCategoryID ?? 0;


                            var items = budgetCategoryDetails
                                    .Where(bcd => bcd.BudgetCategoryId == bc.BudgetCategoryID)
                                    .Select(bcd =>
                                    {
                                        var matchingPlans = plan.Where(p =>
                                            p.PlanCategoryId == planCategoryId &&
                                            p.BudgetItem == bcd.BudgetCategoryDetailID.ToString()
                                        ).ToList();

                                        // Get distinct account codes from the matching plans
                                        var accountCodes = matchingPlans.Select(p => p.AccountCode).Distinct().ToList();

                                        var relevantJournalEntries = JournalDetails
                                            .Where(j => accountCodes.Contains(j.AccountCode))
                                            .ToList();

                                        return new BudgetItemVM
                                        {
                                            BudgetCode = bcd.BudgetCode,
                                            BudgetItem = bcd.BudgetItem,
                                            Budget = matchingPlans.Sum(p => p.TotalYearlyBudget),
                                            Actual = BudgetGLJournalHelper.GetNetJournalsBalance(relevantJournalEntries)
                                        };
                                    })
                                    .ToList();

                            return new BudgetCategoryVM
                            {
                                Name = bc.Name,
                                BudgetTotal = items.Sum(x => x.Budget),
                                ActualTotal = items.Sum(x => x.Actual),
                                Items = items
                            };
                        })
                        .ToList();

                    return new BudgetTypeVM
                    {
                        Name = bt.Name,
                        BudgetTotal = categories.Sum(c => c.BudgetTotal),
                        ActualTotal = categories.Sum(c => c.ActualTotal),
                        Categories = categories
                    };
                })
                .ToList();

            return list;
        }

    }
}
