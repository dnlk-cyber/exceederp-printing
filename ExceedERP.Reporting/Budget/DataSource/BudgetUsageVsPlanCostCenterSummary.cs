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
using ExceedERP.Helpers.Budget;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;

namespace ExceedERP.Reporting.Budget.DataSource
{
    [DataObject]
    public class BudgetUsageVsPlanCostCenterSummary : List<WorkUnitReportViewModel>
    {
        public ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WorkUnitReportViewModel> GetUsageList(string Type, string BudgetCategory, string Year, string WorkUnit, string Branch)
        {
            List<WorkUnitReportViewModel> BudgetList = new List<WorkUnitReportViewModel>();
              List<BudgetUsage> BudgetUsages = new List<BudgetUsage>();
            List<WorkUnitReportViewModel> List = new List<WorkUnitReportViewModel>();
            FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();


    decimal ThisYearMonthlyPlan = decimal.Zero;
            decimal ThisYearMonthlyActual = decimal.Zero;
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

            try
            {
            

                if (string.IsNullOrWhiteSpace(BudgetCategory))
                {

                    var BudgetCategoryes = db.BudgetCategorys.ToList();
                    if (BudgetCategoryes.Any())
                    {
                        BudgetCategory = BudgetCategoryes.FirstOrDefault().BudgetCategoryID.ToString();
                    }
                }




                if (string.IsNullOrWhiteSpace(Year))
                {
                    Year = fiscalYearHelper.GetCurrentFYearId().ToString();
                }

                decimal CurrentYearActual = decimal.Zero;

                var CurrentYear = db.GLFiscalYears.Where(x => x.GlFiscalYearId.ToString() == Year).FirstOrDefault();
                  var CurrentPeriod = db.GLPeriods.Where(x => x.GlFiscalYearId == CurrentYear.GlFiscalYearId).FirstOrDefault();
                var BudgetCategorys = db.BudgetCategorys.Where(x => x.Name == BudgetCategory).FirstOrDefault();
                var PlanCategory = db.PlanCategorys.Where(x => x.GlFiscalYearId == CurrentYear.GlFiscalYearId && x.Name == BudgetCategory && x.IsOnlineApproved == true).FirstOrDefault();

                if (CurrentPeriod == null)
                {
                    CurrentPeriod = new GLPeriod();
                    CurrentPeriod.Name = string.Empty;
                    CurrentPeriod.GlFiscalYearId = 0;
                }

                var JournalDetails = db.GLRecordJournalDetails.Where(x => x.GLRecordJournal.FiscalYear == CurrentYear.Name && x.GLRecordJournal.IsOnlineApproved == true).Where(
                     x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.FromAccountCode) >= 0)
                     .Where(x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.ToAccountCode) <= 0);

                var BudgetPlans = db.BudgetPlans.Where(x=> x.PlanCategoryId ==  PlanCategory.PlanCategoryID).ToList();

                if (!string.IsNullOrEmpty(WorkUnit))
                {
                    BudgetPlans = BudgetPlans.ToList();
                    JournalDetails = JournalDetails.Where(x => x.CostCenterSegment == WorkUnit);
                }
                if (!string.IsNullOrWhiteSpace(Branch))
                {
                    BudgetPlans = BudgetPlans.Where(x => x.BranchId.ToString() == Branch).ToList();
                }
                foreach (var list in BudgetPlans)
                {
                    var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                    var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
                   // var CostCenter = glChartOfAccountCostCenters.Where(x => x.Values == list.GLChartOfAccountCostCenterValue.ToString()).FirstOrDefault();
                    var Currency = db.FinSetupCurrencies.Where(x => x.FinSetupCurrencyID == list.Unit).FirstOrDefault();
                    var WorkUnitTotal = db.BudgetPlans.Where(x=>x.PlanCategoryId == PlanCategory.PlanCategoryID ).ToList();
                    var ThisMonthPlans = db.BudgetPlanMonthlys.Where(x => x.GLPeriodId == CurrentPeriod.GLPeriodId && x.BudgetDefineID == list.BudgetPlanID).FirstOrDefault();
                    var ThisMonthActuals = JournalDetails.Where(x => x.GLRecordJournal.PeriodName == CurrentPeriod.Name ).FirstOrDefault();
                    var ThisYearActuals = JournalDetails.ToList();
                    var ThisYearUntil = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == list.BudgetPlanID && x.GLPeriodId <= CurrentPeriod.GLPeriodId).ToList();

                    if (ThisMonthActuals != null)
                    {
                        ThisYearMonthlyActual = ThisMonthActuals.Credit - ThisMonthActuals.Debit;
                    }

                    if (ThisMonthPlans != null)
                    {
                        ThisYearMonthlyPlan = ThisMonthPlans.MonthlyBudget;
                    }
                    foreach (var budget in ThisYearActuals)
                    {

                        if (budget.AccountType == GlAccountTypes.Asset || budget.AccountType == GlAccountTypes.Expense)
                        {
                            CurrentYearActual = CurrentYearActual + (budget.Credit - budget.Debit);
                        }
                        if (budget.AccountType == GlAccountTypes.Liability || budget.AccountType == GlAccountTypes.Capital || budget.AccountType == GlAccountTypes.Revenue)
                        {
                            CurrentYearActual = CurrentYearActual + (budget.Debit - budget.Credit);
                        }

                    }

                    var model = new WorkUnitReportViewModel
                    {
                        ThisYearBudget = WorkUnitTotal.Sum(x => x.TotalYearlyBudget),
                        ThisYearMonthlyPlan = ThisYearMonthlyPlan,
                        ThisYearMonthlyActual = ThisYearMonthlyActual
                    };
                    if (ThisYearMonthlyActual == decimal.Zero)
                    {
                        ThisYearMonthlyActual = 1;
                    }
                    model.MonthlyDiffrencePecent = (ThisYearMonthlyPlan / ThisYearMonthlyActual) * 100;
                    model.CurrentYearPlan = ThisYearUntil.Sum(x => x.MonthlyBudget);
                    model.CurrentYearActual = CurrentYearActual;
                    if (CurrentYearActual == decimal.Zero)
                    {
                        CurrentYearActual = 1;
                    }


                    decimal YearDiffrence = (ThisYearUntil.Sum(x => x.MonthlyBudget) / CurrentYearActual) * 100;
                    model.CurrentYearDifference = YearDiffrence;
                    if (YearDiffrence == decimal.Zero)
                    { YearDiffrence = 1; }

                    model.GrandDifferencePercentage = (model.MonthlyDiffrencePecent / YearDiffrence) * 100;


                   // model.WorkUnit = CostCenter.Description;
                    //model.Unit = Currency.Name;
                    //model.AmharicName = AmharicName;
                    //model.Name = Name;
                    //model.LogoFileName = LogoFileName;
                    //model.Address = Address;

                    //List.Add(model);
                }

            }
            catch(Exception e)
            {
                var model = new WorkUnitReportViewModel();

            
                model.WorkUnit = e.Message;
                model.Unit = "";
                //model.AmharicName = e.StackTrace;
                //model.Name = Name;
                //model.LogoFileName = LogoFileName;
                //model.Address = Address;

                //List.Add(model);
                //return List;
            }
            return List;
        }

    }
}



