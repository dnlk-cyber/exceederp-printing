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

namespace ExceedERP.Reporting.Budget.DataSource
{
    [DataObject]
    public class BudgetUsagePerMonth : List<BudgetAllocationAndUsageAgainstPreviousYearMonthlyViewModel>
    {
        public ExceedDbContext db = new ExceedDbContext();




        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetAllocationAndUsageAgainstPreviousYearMonthlyViewModel> GetUsageList(string BudgetType, string BugetCategory, string Year, string WorkUnit, string Branch)
        {
            BudgetGLJournalHelper BudgetGLJournalHelper = new BudgetGLJournalHelper();
            List<BudgetAllocationAndUsageAgainstPreviousYearMonthlyViewModel> BudgetList = new List<BudgetAllocationAndUsageAgainstPreviousYearMonthlyViewModel>();
            BudgetAllocationAndUsageAgainstPreviousYearMonthlyViewModel BudgetViewModel;
            List<BudgetPlanMonthly> BudgetPlanMonthlys = new List<BudgetPlanMonthly>();
            BudgetPlan BudgetPlanObj = new BudgetPlan();
            string AmharicName = "";
            string Name = "";
            string Address = "";
            Image LogoFileName = null;
            try
            {
                var BudgetPlans = db.BudgetPlans.ToList();
                var BudgetPlanMonth = db.BudgetPlanMonthlys.ToList();
                var budgetUsages = db.BudgetUsages.ToList();
                if (!string.IsNullOrWhiteSpace(Branch))
                {
                    BudgetPlans = BudgetPlans.Where(x => x.BranchId.ToString() == Branch).ToList();
                }

                if(string.IsNullOrWhiteSpace(Year))
                {

                    FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();
                    Year = fiscalYearHelper.GetCurrentFYearId().ToString();
                }

                #region SharedDataSource

           
         
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



                string AccountCode = "";
                decimal PreviousPlanGrowth = 0;
                decimal CurrentPlanGrowth = 0;
                decimal PreviousActual = 0;
                decimal CurrentActual = 0;
                decimal ActualNet = 0;
                decimal YearlyPlanNet = 0;
                decimal BudgetActualDiffPerc = 0;
                decimal YearlyPlanNetPercentage = 0;
                int PreviousCategory = 0;
                int CurrentCategory = 0;
                string WorkUnitName = string.Empty;



                var ThisYearRef = db.GLFiscalYears.Where(x => x.Name == Year).FirstOrDefault();
                int IdRef = ThisYearRef.GlFiscalYearId - 1;
                var PrevYearRef = db.GLFiscalYears.Where(x => x.GlFiscalYearId == IdRef).FirstOrDefault();

                if (PrevYearRef == null)
                {
                    PrevYearRef = new GlFiscalYear();
                    PrevYearRef.Name = string.Empty;
                    PrevYearRef.GlFiscalYearId = 0;
                }


                var CurrentPlanCategory = db.PlanCategorys.Where(x => x.GlFiscalYear.Name == Year && x.Name == BugetCategory && x.IsOnlineApproved == true).FirstOrDefault();

                var PreviousPlanCategory = db.PlanCategorys.Where(x => x.GlFiscalYear.Name == PrevYearRef.Name && x.Name == BugetCategory && x.IsOnlineApproved == true).FirstOrDefault();
                if (PreviousPlanCategory != null)
                {
                    PreviousCategory = PreviousPlanCategory.PlanCategoryID;
                }


                if (CurrentPlanCategory != null)
                {
                    CurrentCategory = CurrentPlanCategory.PlanCategoryID;
                }
             
                if (!string.IsNullOrEmpty(Year))
                {
                    int thisBudgetMonthRef = 0;
                    int prevBudgetMonthRef = 0;
                    List<BudgetPlan> PreviousBudgetPlans = new List<BudgetPlan>();
                    List<BudgetPlan> CurrentBudgetPlans = new List<BudgetPlan>();

                    if (!string.IsNullOrEmpty(WorkUnit))
                    {
                        PreviousBudgetPlans = db.BudgetPlans.AsNoTracking().Where(x => x.BudgetCategoryID == PreviousCategory && x.GLChartOfAccountCostCenterId.ToString() == WorkUnit).ToList();
                        CurrentBudgetPlans = db.BudgetPlans.AsNoTracking().Where(x => x.BudgetCategoryID == CurrentCategory && x.GLChartOfAccountCostCenterId.ToString() == WorkUnit).ToList();


                    }
                    else
                    {
                        PreviousBudgetPlans = db.BudgetPlans.AsNoTracking().Where(x => x.BudgetCategoryID == PreviousCategory).ToList();
                        CurrentBudgetPlans = db.BudgetPlans.AsNoTracking().Where(x => x.BudgetCategoryID == CurrentCategory).ToList();


                    }
                    if (PreviousBudgetPlans.Any())
                    {
                        PreviousPlanGrowth = PreviousBudgetPlans.Sum(x => x.TotalYearlyBudget);
                        prevBudgetMonthRef = PreviousBudgetPlans.FirstOrDefault().BudgetPlanID;
                    }
                    if (CurrentBudgetPlans.Any())
                    {
                        CurrentPlanGrowth = CurrentBudgetPlans.Sum(x => x.TotalYearlyBudget);
                        thisBudgetMonthRef = CurrentBudgetPlans.FirstOrDefault().BudgetPlanID;

                    }
                    YearlyPlanNet = CurrentPlanGrowth - PreviousPlanGrowth;


                    decimal CurrentPlanGrowths = CurrentPlanGrowth;
                    if (CurrentPlanGrowths > 0)
                    {
                        YearlyPlanNetPercentage = (YearlyPlanNet / CurrentPlanGrowths);
                    }

                    if (true)
                    {
                        if (!string.IsNullOrWhiteSpace(Branch))
                        {
                            CurrentBudgetPlans = CurrentBudgetPlans.Where(x => x.BranchId.ToString() == Branch).ToList();
                        }
                        foreach (var CurrentBudgetPlan in CurrentBudgetPlans)
                        {
                            var PreviousBudgetPlan = PreviousBudgetPlans.Where(x => x.AccountCode == CurrentBudgetPlan.AccountCode && x.GLChartOfAccountCostCenterId == CurrentBudgetPlan.GLChartOfAccountCostCenterId).FirstOrDefault();

                            var CurrentPeriodPlan = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == CurrentBudgetPlan.BudgetPlanID).ToList();
                            var PreviousPeriodPlans = new List<BudgetPlanMonthly>();
                            if (PreviousBudgetPlan != null)
                            {
                                PreviousPeriodPlans = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == PreviousBudgetPlan.BudgetPlanID).ToList();
                            }

                            foreach (var budgetPlanMonthlys in CurrentPeriodPlan)
                            {

                                var PreviousPeriodPlan = PreviousPeriodPlans.Where(x => x.BudgetPlan.AccountCode == budgetPlanMonthlys.BudgetPlan.AccountCode && x.BudgetPlan.GLChartOfAccountCostCenterId == budgetPlanMonthlys.BudgetPlan.GLChartOfAccountCostCenterId && x.GLPeriodId == budgetPlanMonthlys.GLPeriodId).FirstOrDefault();
                                var CurrentPeriodActual = db.GLRecordJournalDetails.Where(x => x.AccountCode == budgetPlanMonthlys.AccountCode && x.GLRecordJournal.FiscalYear == ThisYearRef.Name && x.GLRecordJournal.PeriodName == budgetPlanMonthlys.GLPeriod.Name && x.CostCenterSegment == CurrentBudgetPlan.GLChartOfAccountCostCenterId.ToString() && x.GLRecordJournal.IsOnlineApproved == true).FirstOrDefault();
                                var PreviousPeriodActual = db.GLRecordJournalDetails.Where(x => x.AccountCode == budgetPlanMonthlys.AccountCode && x.GLRecordJournal.FiscalYear == PrevYearRef.Name && x.GLRecordJournal.PeriodName == budgetPlanMonthlys.GLPeriod.Name && x.CostCenterSegment == CurrentBudgetPlan.GLChartOfAccountCostCenterId.ToString() && x.GLRecordJournal.IsOnlineApproved == true).FirstOrDefault();

                                var WorkUnitObj = db.GLChartOfAccountCostCenters.Where(x => x.Values == CurrentBudgetPlan.GLChartOfAccountCostCenterId.ToString()).FirstOrDefault();

                                if (WorkUnitObj != null)
                                {
                                    WorkUnitName = WorkUnitObj.Description;
                                }
                                decimal PreviousPlan = 0;
                                decimal CurrentPlan = 0;
                                decimal monthlyDiffPercent = 0;


                                CurrentPlan = budgetPlanMonthlys.CurrentBudget;

                                if (PreviousPeriodPlan != null)
                                {
                                    PreviousPlan = PreviousPeriodPlan.CurrentBudget;
                                }

                                var PlanNet = CurrentPlan - PreviousPlan;

                                if (CurrentPlan > 0)
                                {
                                    monthlyDiffPercent = (PlanNet / CurrentPlan);
                                }
                                if (CurrentPeriodActual != null)
                                {
                                    CurrentActual = BudgetGLJournalHelper.GetNetJournalBalance(CurrentPeriodActual);
                                }
                                if (PreviousPeriodActual != null)
                                {
                                    PreviousActual = BudgetGLJournalHelper.GetNetJournalBalance(PreviousPeriodActual);
                                }
                                ActualNet = CurrentActual - PreviousActual;
                                if (CurrentActual > 0)
                                {
                                    BudgetActualDiffPerc = (ActualNet / CurrentActual);
                                }

                                BudgetViewModel = new BudgetAllocationAndUsageAgainstPreviousYearMonthlyViewModel();
                                BudgetViewModel.AccountDesc = Year + " Against " + PrevYearRef.Name;
                                BudgetViewModel.AccountCode = CurrentBudgetPlan.AccountCode;
                                BudgetViewModel.Unit = CurrentBudgetPlan.BudgetCurrency.Name;
                                BudgetViewModel.PreviousYearBudget = PreviousPlanGrowth;
                                BudgetViewModel.PreviousYearActual = PreviousActual;
                                BudgetViewModel.ThisYearActual = CurrentActual;
                                BudgetViewModel.ThisYearBudget = CurrentPlanGrowth;
                                BudgetViewModel.PreviousMonthlyBudget = PreviousPlan;
                                BudgetViewModel.ThisYearMonthlyBudget = CurrentPlan;
                                BudgetViewModel.MonthlyDiffrence = PlanNet;
                                BudgetViewModel.MonthlyDiffrencePecent = monthlyDiffPercent;
                                BudgetViewModel.ActualDifference = ActualNet;
                                BudgetViewModel.BudgetDifference = YearlyPlanNet;
                                BudgetViewModel.BudgetMonth = budgetPlanMonthlys.GLPeriod.Name + " -- " + WorkUnitName;
                                BudgetViewModel.BudgetDifferencePercentage = YearlyPlanNetPercentage;
                                BudgetViewModel.ActualDifferencePercentage = BudgetActualDiffPerc;

                                BudgetViewModel.AmharicName = AmharicName;
                                BudgetViewModel.Name = Name;
                                BudgetViewModel.LogoFileName = LogoFileName;
                                BudgetViewModel.Address = Address;

                                BudgetList.Add(BudgetViewModel);


                                CurrentActual = 0;
                                PreviousActual = 0;
                                BudgetActualDiffPerc = 0;

                            }
                        }
                        BudgetList.GroupBy(x => x.BudgetMonth);

                        return BudgetList;
                    }

                    return null;
                }
                return null;
            }

            catch (Exception e)
            {
                BudgetViewModel = new BudgetAllocationAndUsageAgainstPreviousYearMonthlyViewModel();
      
                BudgetViewModel.AmharicName = AmharicName;
                BudgetViewModel.Name = Name;
                BudgetViewModel.LogoFileName = LogoFileName;
                BudgetViewModel.Address = Address;

                BudgetList.Add(BudgetViewModel);

                return BudgetList;
            }
        }
    }

}




