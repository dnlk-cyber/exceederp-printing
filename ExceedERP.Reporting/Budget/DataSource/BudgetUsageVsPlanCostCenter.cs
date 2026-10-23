using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Reporting.ViewModel;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using System.Drawing;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Finance;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;

namespace ExceedERP.Reporting.Budget.DataSource
{
    [DataObject]
    public class BudgetUsageVsPlanCostCenter : List<BudgetAllocationAndUsageAgainstPreviousYearViewModel>
    {
        public ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetAllocationAndUsageAgainstPreviousYearViewModel> GetUsageList(string Type, string BudgetCategory, string Year, string WorkUnit, string Branch)
        {
            List<BudgetAllocationAndUsageAgainstPreviousYearViewModel> BudgetList = new List<BudgetAllocationAndUsageAgainstPreviousYearViewModel>();
            BudgetAllocationAndUsageAgainstPreviousYearViewModel BudgetViewModel;
            List<BudgetUsage> BudgetUsages = new List<BudgetUsage>();
            FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();
            AccountHelper accountHelper = new AccountHelper();


            decimal BudgethisDiffPerc = 0;
            string AccCode = string.Empty;
            string thisyear = Year;


            #region SharedDataSource
            string AmharicName = "";
            string Name = "";
            string Address = "";
            string WorkUnitName = "";
            Image LogoFileName = null;
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {

                        if (Address == null)
                        {
                            Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }

            }

            #endregion


            if (!string.IsNullOrWhiteSpace(BudgetCategory) && !string.IsNullOrWhiteSpace(Year))
            {
                int budgetCatId = int.Parse(BudgetCategory);
                int yearId = int.Parse(Year);

                var currentYear = db.GLFiscalYears.Find(yearId);
                var currYearPlancatIds = db.PlanCategorys
                    .AsNoTracking()
                                     .Where(x => x.GlFiscalYearId == yearId
                                                 && x.IsDepartmentOnlineApproved
                                                 && x.BudgetCategoryID == budgetCatId
                                                 && (x.IsOnlineApproved)
                                                 && !x.IsVoid)
                                     .Select(x => x.PlanCategoryID)
                                     .ToList();

                //var categoryObj = db.BudgetCategorys.Where(x => x.BudgetCategoryID == budgetCatId).FirstOrDefault();

                var CurrentBudgetPlans = db.BudgetPlans.AsNoTracking().Where(x => currYearPlancatIds.Contains(x.PlanCategoryId)).ToList();


                if (!string.IsNullOrEmpty(WorkUnit))
                {
                    CurrentBudgetPlans = CurrentBudgetPlans.Where(x => x.CostCenter == WorkUnit).ToList();
                }

                foreach (var b in CurrentBudgetPlans)
                {
                    //List<BudgetUsage> Usages = new List<BudgetUsage>();
                    decimal thisYearBudget = 0;
                    decimal thisYearActualBudget = 0;

                    var JournalDetails = db.GLRecordJournalDetails.AsNoTracking().Where(x => x.AccountCode == b.AccountCode && x.GLRecordJournal.IsOnlineApproved && !x.GLRecordJournal.isVoid);
                    var monthly = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == b.BudgetPlanID);

                    if (monthly.Any())
                    {
                        thisYearBudget = monthly.Sum(x => x.CurrentBudget);
                    }


                    if (JournalDetails != null)
                    {
                        foreach (var budget in JournalDetails)
                        {

                            if (budget.AccountType == GlAccountTypes.Asset || budget.AccountType == GlAccountTypes.Expense)
                            {
                                thisYearActualBudget = thisYearActualBudget + (budget.Credit - budget.Debit);
                            }
                            if (budget.AccountType == GlAccountTypes.Liability || budget.AccountType == GlAccountTypes.Capital || budget.AccountType == GlAccountTypes.Revenue)
                            {
                                thisYearActualBudget = thisYearActualBudget + (budget.Debit - budget.Credit);
                            }

                        }
                    }

                    if (thisYearBudget > 0)
                    {
                        BudgethisDiffPerc = (thisYearActualBudget / thisYearBudget) * 100;
                    }
                    else
                    {
                        BudgethisDiffPerc = 0;
                    }


                    BudgetViewModel = new BudgetAllocationAndUsageAgainstPreviousYearViewModel
                    {
                        AccountCode = b.AccountCode,

                        Unit = "",
                        WorkUnit = WorkUnitName,
                        ThisYearActual = thisYearActualBudget,
                        ThisYearBudget = thisYearBudget,

                        BudgetDifferencePercentage = BudgethisDiffPerc
                    };



                    BudgetList.Add(BudgetViewModel);

                }
            }

            return BudgetList;
        }

    }
}



