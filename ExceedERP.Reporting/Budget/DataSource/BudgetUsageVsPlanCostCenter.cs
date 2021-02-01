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
using ExceedERP.Reporting.SharedDataSources;
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
        public List<BudgetAllocationAndUsageAgainstPreviousYearViewModel> GetUsageList(string BudgetCategory, string Year, string WorkUnit, string Branch)
        {
            List<BudgetAllocationAndUsageAgainstPreviousYearViewModel> BudgetList = new List<BudgetAllocationAndUsageAgainstPreviousYearViewModel>();
            BudgetAllocationAndUsageAgainstPreviousYearViewModel BudgetViewModel;
            List<BudgetUsage> BudgetUsages = new List<BudgetUsage>();
            FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();
            AccountHelper accountHelper = new AccountHelper();
            decimal thisYearBudget = 0;
            decimal thisYearActualBudget = 0;

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


            if (string.IsNullOrWhiteSpace(BudgetCategory))
            {

                var BudgetCategorys = db.BudgetCategorys.ToList();
                if (BudgetCategorys.Any())
                {
                    BudgetCategory = BudgetCategorys.FirstOrDefault().Name;
                }
            }




            if (string.IsNullOrWhiteSpace(Year))
            {
                Year = fiscalYearHelper.GetCurrentFYearId().ToString();
            }



            int ThisCategory = 0;
            var ThisYearRef = db.GLFiscalYears.Where(x => x.Name == Year).FirstOrDefault();



            var PlanCategory = db.PlanCategorys.Where(x => x.GlFiscalYear.Name == Year && x.Name == BudgetCategory && x.IsOnlineApproved == true).FirstOrDefault();

            if (PlanCategory != null)
            {
                ThisCategory = PlanCategory.PlanCategoryID;
            }


            if (!string.IsNullOrEmpty(Year))
            {
                List<BudgetPlan> thisyearBudgetList = new List<BudgetPlan>();
                if (!string.IsNullOrEmpty(WorkUnit))
                {
                    string WU = WorkUnit;
                    thisyearBudgetList = db.BudgetPlans.Where(x => x.BudgetCategoryID == ThisCategory && x.GLChartOfAccountCostCenterId.ToString() == WU).ToList();
                }
                else
                {
                    thisyearBudgetList = db.BudgetPlans.Where(x => x.BudgetCategoryID == ThisCategory).ToList();

                }
                var BudgetCategorys = db.BudgetCategorys.Where(x => x.Name == BudgetCategory).FirstOrDefault();



                var JournalDetails = db.GLRecordJournalDetails.Where(x => x.GLRecordJournal.FiscalYear == ThisYearRef.Name && x.GLRecordJournal.IsOnlineApproved == true).Where(
          x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.FromAccountCode) >= 0)
          .Where(x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.ToAccountCode) <= 0);

                foreach (var b in thisyearBudgetList)
                {


                    List<BudgetUsage> Usages = new List<BudgetUsage>();
                    var thisyearActualobj = new List<GLRecordJournalDetail>();
                    string costtoSegment = string.Empty;

                    if (!accountHelper.IsDynamicCoA())
                    {
                        costtoSegment = b.AccountCode.Split('-')[2];
                      
                        var WorkUnitObj = db.GLChartOfAccountCostCenters.Where(x => x.Values == b.GLChartOfAccountCostCenterId.ToString()).FirstOrDefault();
                        if (WorkUnitObj != null)
                        {
                            WorkUnitName = WorkUnitObj.Description;
                        }
                    }
                    else
                    {
                        costtoSegment =
                           b.AccountCode.Split('-')[new DynamicAccountHelper().GetCostCenterAccountSequenceIndex()];

                        var glSetupCostCenter =
          db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                        var GlChartOfAccountSegments = db.GlChartOfAccountSegments
   .Where(x => x.GLSegmentSetupId == glSetupCostCenter.GLSegmentSetupId)
   .FirstOrDefault(x => x.Values == costtoSegment);

                        if (GlChartOfAccountSegments != null)
                        {
                            WorkUnitName = GlChartOfAccountSegments.Description;
                        }
                    }


                    if (WorkUnitName == b.WorkUnitName)
                    {
                        thisyearActualobj = JournalDetails.Where(x => x.AccountCode == b.AccountCode && x.CostCenterSegment == costtoSegment).ToList();
                    }

 
                        thisYearBudget = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == b.BudgetPlanID).Sum(x => x.CurrentBudget);
             
                    if (thisyearActualobj != null)
                    {
                        foreach (var budget in thisyearActualobj)
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






                    BudgetViewModel = new BudgetAllocationAndUsageAgainstPreviousYearViewModel();
                    BudgetViewModel.AccountDesc = b.AccountDesc;
                    BudgetViewModel.AccountCode = b.AccountCode;
                    BudgetViewModel.FiscalYear = Year;
                    BudgetViewModel.BudgetCategory = BudgetCategory;
                    BudgetViewModel.Unit = b.BudgetCurrency.Name;
                    BudgetViewModel.WorkUnit = WorkUnitName;
                    BudgetViewModel.ThisYearActual = thisYearActualBudget;
                    BudgetViewModel.ThisYearBudget = thisYearBudget;

                    BudgetViewModel.BudgetDifferencePercentage = BudgethisDiffPerc;

                    BudgetViewModel.AmharicName = AmharicName;
                    BudgetViewModel.Name = Name;
                    BudgetViewModel.LogoFileName = LogoFileName;
                    BudgetViewModel.Address = Address;


                    BudgetList.Add(BudgetViewModel);

                }
            }

            return BudgetList;
        }

    }
}



