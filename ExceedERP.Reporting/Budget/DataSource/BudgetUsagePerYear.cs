using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
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
        public List<BudgetAllocationAndUsageAgainstPreviousYearViewModel> GetUsageList(string Type, string BudgetCategory, string Year, string WorkUnit, string Branch)
        {
            List<BudgetAllocationAndUsageAgainstPreviousYearViewModel> BudgetList = new List<BudgetAllocationAndUsageAgainstPreviousYearViewModel>();
            BudgetAllocationAndUsageAgainstPreviousYearViewModel BudgetViewModel;
            List<BudgetUsage> BudgetUsages = new List<BudgetUsage>();
            BudgetGLJournalHelper BudgetGLJournalHelper = new BudgetGLJournalHelper();
            FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();
            var BudgetPlans = db.BudgetPlans.ToList();
            var BudgetPlanMonth = db.BudgetPlanMonthlys.ToList();

            string AccountCode = "";
            decimal PreviousYearlyBudgetPlan = 0;
            decimal CurrentYearBudgetPlan = 0;
            decimal PreviousActual = 0;
            decimal CurrentActual = 0;
            decimal ActualNet = 0;
            decimal PlanNet = 0;
            decimal ActualPercentage = 0;
            decimal PlanPercentage = 0;
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

            if (string.IsNullOrWhiteSpace(BudgetCategory) || string.IsNullOrWhiteSpace(Year))
            {
                var model = new BudgetAllocationAndUsageAgainstPreviousYearViewModel();
                model.AmharicName = AmharicName;
                model.Name = Name;
                model.LogoFileName = LogoFileName;
                model.Address = Address;

                BudgetList.Add(model);
                return BudgetList;
            }





           




            int PrevCategory = 0;
            int ThisCategory = 0;
            var ThisYearRef = db.GLFiscalYears.Where(x => x.Name == Year).FirstOrDefault();
            int IdRef = ThisYearRef.GlFiscalYearId - 1;
            var PrevYearRef = db.GLFiscalYears.Where(x => x.GlFiscalYearId == IdRef).FirstOrDefault();

            if (PrevYearRef == null)
            {
                PrevYearRef = new GlFiscalYear
                {
                    Name = string.Empty,
                    GlFiscalYearId = 0
                };
            }

            var CurrentCategory = db.PlanCategorys.Where(x => x.GlFiscalYear.Name == Year && x.Name == BudgetCategory && x.IsOnlineApproved == true).FirstOrDefault();

            var PrevPlanCategory = db.PlanCategorys.Where(x => x.GlFiscalYear.Name == PrevYearRef.Name && x.Name == BudgetCategory && x.IsOnlineApproved == true).FirstOrDefault();

            if (PrevPlanCategory != null)
            {
                PrevCategory = PrevPlanCategory.PlanCategoryID;
            }
            if (CurrentCategory != null)
            {
                ThisCategory = CurrentCategory.PlanCategoryID;
            }

            if (!string.IsNullOrEmpty(Year))
            {
                List<BudgetPlan> CurrentBudgetPlans = new List<BudgetPlan>();
                if (!string.IsNullOrEmpty(WorkUnit))
                {
                    string WU = WorkUnit;
                    CurrentBudgetPlans = db.BudgetPlans.Where(x => x.BudgetCategoryID == ThisCategory && x.GLChartOfAccountCostCenterId.ToString() == WU).ToList();
                }
                else
                {
                    CurrentBudgetPlans = db.BudgetPlans.Where(x => x.BudgetCategoryID == ThisCategory).ToList();

                }
                if (!string.IsNullOrWhiteSpace(Branch))
                {
                    CurrentBudgetPlans = CurrentBudgetPlans.Where(x => x.BranchId.ToString() == Branch).ToList();
                }
                foreach (var CurrentBudgetPlan in CurrentBudgetPlans)
                {

                    var PreviousBudgetPlans = db.BudgetPlans.Where(x => x.BudgetCategoryID == PrevCategory && x.AccountCode == CurrentBudgetPlan.AccountCode && x.GLChartOfAccountCostCenterId == CurrentBudgetPlan.GLChartOfAccountCostCenterId).FirstOrDefault();

                    List<BudgetUsage> Usages = new List<BudgetUsage>();
                    var BudgetCategorys = db.BudgetCategorys.Where(x => x.Name == BudgetCategory).FirstOrDefault();



                    var JournalDetails = db.GLRecordJournalDetails.Where(
              x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.FromAccountCode) >= 0)
              .Where(x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.ToAccountCode) <= 0);


                    var PreviousYearlyActual =
                        JournalDetails.Where(x => x.GLRecordJournal.FiscalYear == PrevYearRef.Name && x.AccountCode == CurrentBudgetPlan.AccountCode && x.CostCenterSegment == CurrentBudgetPlan.GLChartOfAccountCostCenterId.ToString() && x.GLRecordJournal.IsOnlineApproved == true).ToList();

                    var CurrentYearlyActual =
                    JournalDetails.Where(x => x.GLRecordJournal.FiscalYear == Year && x.AccountCode == CurrentBudgetPlan.AccountCode && x.CostCenterSegment == CurrentBudgetPlan.GLChartOfAccountCostCenterId.ToString() && x.GLRecordJournal.IsOnlineApproved == true).ToList();//.ForEach(entry => {




                    var WorkUnitObj = db.GLChartOfAccountCostCenters.Where(x => x.Values == CurrentBudgetPlan.GLChartOfAccountCostCenterId.ToString()).FirstOrDefault();

                    if (WorkUnitObj != null)
                    {
                        WorkUnitName = WorkUnitObj.Description;
                    }

                    if (PreviousBudgetPlans != null)
                    {
                        PreviousYearlyBudgetPlan = PreviousBudgetPlans.TotalYearlyBudget;
                        AccountCode = PreviousBudgetPlans.AccountCode;
                    }
                    if (PreviousYearlyActual != null)
                    {
                        PreviousActual = PreviousYearlyActual.Sum(x => x.Credit);
                    }

                    if (CurrentBudgetPlan != null)
                    {


                        CurrentYearBudgetPlan = CurrentBudgetPlan.TotalYearlyBudget;

                    }
                    if (CurrentYearlyActual != null)
                    {
                        CurrentActual = BudgetGLJournalHelper.GetNetJournalsBalance(CurrentYearlyActual);

                    }

                    if (PreviousYearlyActual != null)
                    {
                        PreviousActual = BudgetGLJournalHelper.GetNetJournalsBalance(PreviousYearlyActual);

                    }


                    ActualNet = CurrentActual - PreviousActual;
                    PlanNet = CurrentYearBudgetPlan - PreviousYearlyBudgetPlan;



                    if (CurrentActual > 0)
                    {
                        ActualPercentage = (CurrentActual / BudgetGenericHelper.DivideByZero(PreviousActual));
                    }
                    else
                    {
                        ActualPercentage = 0;
                    }
                    if (CurrentYearBudgetPlan > 0)
                    {
                        PlanPercentage = (PreviousYearlyBudgetPlan / BudgetGenericHelper.DivideByZero(CurrentYearBudgetPlan));
                    }







                    BudgetViewModel = new BudgetAllocationAndUsageAgainstPreviousYearViewModel
                    {
                        AccountDesc = PrevYearRef.Name + " Against " + Year,
                        AccountCode = CurrentBudgetPlan.AccountDesc,
                        FiscalYear = Year,
                        BudgetCategory = BudgetCategory,
                        Unit = CurrentBudgetPlan.BudgetCurrency.Name,
                        WorkUnit = WorkUnitName,
                        PreviousYearBudget = PreviousYearlyBudgetPlan,
                        PreviousYearActual = PreviousActual,
                        ThisYearActual = CurrentActual,
                        ThisYearBudget = CurrentYearBudgetPlan,
                        ActualDifference = ActualNet,
                        BudgetDifference = PlanNet,
                        BudgetDifferencePercentage = PlanPercentage,
                        ActualDifferencePercentage = ActualPercentage,

                        AmharicName = AmharicName,
                        Name = Name,
                        LogoFileName = LogoFileName,
                        Address = Address
                    };


                    BudgetList.Add(BudgetViewModel);

                }
            }
            if (!BudgetList.Any())
            {




                BudgetViewModel = new BudgetAllocationAndUsageAgainstPreviousYearViewModel
                {
                    AccountDesc = string.Empty,
                    AccountCode = string.Empty,
                    FiscalYear = Year,
                    BudgetCategory = BudgetCategory,
                    Unit = string.Empty,
                    WorkUnit = WorkUnitName,
                    PreviousYearBudget = PreviousYearlyBudgetPlan,
                    PreviousYearActual = PreviousActual,
                    ThisYearActual = CurrentActual,
                    ThisYearBudget = CurrentYearBudgetPlan,
                    ActualDifference = ActualNet,
                    BudgetDifference = PlanNet,
                    BudgetDifferencePercentage = PlanPercentage,
                    ActualDifferencePercentage = ActualPercentage,

                    AmharicName = AmharicName,
                    Name = Name,
                    LogoFileName = LogoFileName,
                    Address = Address
                };


                BudgetList.Add(BudgetViewModel);

            }
            return BudgetList;
        }

    }
}



