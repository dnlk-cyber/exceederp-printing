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

namespace ExceedERP.Reporting.Budget.DataSource
{
    [DataObject]
    public class BudgetAllocationUsage : List<BudgetAllocationAndUsageViewModel>
    {
        public ExceedDbContext db = new ExceedDbContext();
      public  BudgetGLJournalHelper BudgetGLJournalHelper = new BudgetGLJournalHelper();

        public List<GLPeriod> GetPreviousMonthOfYear(string Period)
        {
            List<GLPeriod> Periods = new List<GLPeriod>();

            var period = db.GLPeriods.Where(x => x.Name == Period).FirstOrDefault();
            if (period != null)
            {
                Periods = db.GLPeriods.Where(x => x.GLPeriodId < period.GLPeriodId).ToList();
            }

            return Periods;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GlFiscalYear> GetFiscalYearList()
        {


            db = new ExceedDbContext();
            var result = db.GLFiscalYears.ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GLChartOfAccountCostCenter> GetWorkUnit()
        {


            db = new ExceedDbContext();
            var result = db.GLChartOfAccountCostCenters.ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PlanCategory> GetPlanCategory()
        {


            db = new ExceedDbContext();
            var result = db.PlanCategorys.Distinct().ToList();
            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetCategory> GetBudgetCategory()
        {


            db = new ExceedDbContext();
            var result = db.BudgetCategorys.ToList();
            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GLPeriod> GetPeriodList()
        {

            var result = db.GLPeriods.ToList();
            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<Branch> BranchDDBind()
        {
            db = new ExceedDbContext();
            List<Branch> result = db.Branch.ToList();

            return result;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<BudgetPlan> PlanDDBind()
        {
            db = new ExceedDbContext();
            List<BudgetPlan> result = db.BudgetPlans.DistinctByField(x => x.AccountCode).ToList();

            return result;
        }
         [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetAllocationAndUsageViewModel> GetUsageList(string BudgetCategory, string Year, string Period, string AccCode, string WorkUnit, string Branch)
        {
            List<BudgetAllocationAndUsageViewModel> BudgetList = new List<BudgetAllocationAndUsageViewModel>();
            BudgetAllocationAndUsageViewModel BudgetViewModel;
            List<BudgetUsage> BudgetUsages = new List<BudgetUsage>();
            FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();
            List<BudgetAllocationAndUsageViewModel> BudgetReturn = new List<BudgetAllocationAndUsageViewModel>();
            var BudgetPlans = db.BudgetPlans.ToList();
            var BudgetPlanMonth = db.BudgetPlanMonthlys.ToList();

            string Code = " All Code ";
            #region SharedDataSource


            string AmharicName = "";
            string Name = "";
            string Address = "";
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

                if (!string.IsNullOrEmpty(AccCode))
                {
                    Code = AccCode;
                }

    
                decimal CurrentYearPlanTotal = 0;
                decimal NetYearly = 0;

                decimal CurrentYearActualTotal = 0;
                decimal PeriodActualNet = 0;
                decimal PeriodNet = 0;
                decimal PeriodNetPercentage = 0;
                decimal PeriodPlanNetPercentage = 0;
                string WorkUnitName = string.Empty;

                if (string.IsNullOrWhiteSpace(BudgetCategory) || string.IsNullOrWhiteSpace(Year))
                {
                    var model = new BudgetAllocationAndUsageViewModel();
                    model.AmharicName = AmharicName;
                    model.Name = Name;
                    model.LogoFileName = LogoFileName;
                    model.Address = Address;

                    BudgetList.Add(model);
                    return BudgetList;
                }


                if (string.IsNullOrWhiteSpace(BudgetCategory))
                {

                    var BudgetCategorys = db.BudgetCategorys.ToList();
                    if (BudgetCategorys.Any())
                    {
                        BudgetCategory = BudgetCategorys.FirstOrDefault().BudgetCategoryID.ToString();
                    }
                }


 

                if (string.IsNullOrWhiteSpace(Year))
                {
                    Year = fiscalYearHelper.GetCurrentFYearId().ToString();
                }






                int PreviousPlanCategory = 0;
                int CurrentPlanCategory = 0;


         

                if (!string.IsNullOrEmpty(Year))
                {
                    var ThisYearRef = db.GLFiscalYears.Where(x => x.Name == Year).FirstOrDefault();


                    var BudgetCategorys = db.BudgetCategorys.Where(x => x.Name == BudgetCategory).FirstOrDefault();

                    var CurrentYearPlanCategory = db.PlanCategorys.Where(x => x.GlFiscalYear.Name == Year && x.Name == BudgetCategory && x.IsOnlineApproved == true).FirstOrDefault();


                    if (CurrentYearPlanCategory != null)
                    {
                        CurrentPlanCategory = CurrentYearPlanCategory.PlanCategoryID;
                    }

                    if (string.IsNullOrEmpty(Period))
                    {

                        if (CurrentYearPlanCategory == null)
                        {
                            CurrentYearPlanCategory = new PlanCategory();
                            CurrentYearPlanCategory.PlanCategoryID = 0;

                        }

                        var JournalDetails = db.GLRecordJournalDetails.Where(x => x.GLRecordJournal.FiscalYear == Year && x.GLRecordJournal.IsOnlineApproved == true).Where(
                  x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.FromAccountCode) >= 0)
                  .Where(x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.ToAccountCode) <= 0);

                        var TotalBudgetPlan = db.BudgetPlans.Where(so => so.BudgetCategoryID == CurrentPlanCategory).ToList();

                        if (!string.IsNullOrEmpty(WorkUnit))
                        {
                            JournalDetails = JournalDetails.Where(x => x.CostCenterSegment == WorkUnit);
                            TotalBudgetPlan = TotalBudgetPlan.Where(x => x.GLChartOfAccountCostCenterId.ToString() == WorkUnit).ToList();
                        }


                        if (TotalBudgetPlan.Any())
                        {
                            CurrentYearPlanTotal = TotalBudgetPlan.Sum(x => x.TotalYearlyBudget);
                        }

                        if (JournalDetails.Any())
                        {
                            CurrentYearActualTotal = BudgetGLJournalHelper.GetNetJournalsBalance(JournalDetails);
                        }
                        NetYearly = CurrentYearPlanTotal - CurrentYearActualTotal;


                        if (JournalDetails.Any())
                        {
                            decimal CurrentPeriodPlan = 0;
                            decimal CurrentPeriodActual = 0;
                           

                            foreach (var JournalDetail in JournalDetails)
                            {



                                var thisMonthActual = JournalDetails.Where(x => x.GLRecordJournal.PeriodName == JournalDetail.GLRecordJournal.PeriodName && x.AccountCode == JournalDetail.AccountCode && x.CostCenterSegment == JournalDetail.CostCenterSegment).FirstOrDefault();

                                var thisMonthPlanRef = db.BudgetPlans.Where(x => x.BudgetCategoryID == CurrentPlanCategory && x.AccountCode == JournalDetail.AccountCode && x.GLChartOfAccountCostCenterId.ToString() == JournalDetail.CostCenterSegment).FirstOrDefault();

                                if (!string.IsNullOrWhiteSpace(Branch))
                                {
                               //     thisMonthPlanRef = thisMonthPlanRef.Where(x => x.BranchId.ToString() == Branch).FirstOrDefault();
                                }

                                if (thisMonthPlanRef != null)
                                {
                                    var thisMonthPlan = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == thisMonthPlanRef.BudgetPlanID && x.GLPeriod.Name == JournalDetail.GLRecordJournal.PeriodName).FirstOrDefault();

                                    if (thisMonthPlan != null)
                                    {
                                        CurrentPeriodPlan = thisMonthPlan.CurrentBudget;

                                    }
                                }


                                if (thisMonthActual != null)
                                {
                                    CurrentPeriodActual = BudgetGLJournalHelper.GetNetJournalBalance(thisMonthActual);

                                }



                                var WorkUnitObj = db.GLChartOfAccountCostCenters.Where(x => x.Values == JournalDetail.CostCenterSegment).FirstOrDefault();

                                if (WorkUnitObj != null)
                                {
                                    WorkUnitName = WorkUnitObj.Description;
                                }



                                PeriodNet = CurrentPeriodPlan - CurrentPeriodActual;

                                PeriodNetPercentage = (CurrentPeriodActual / BudgetGenericHelper.DivideByZero(CurrentPeriodPlan)) * 100;

                                BudgetViewModel = new BudgetAllocationAndUsageViewModel();

                                BudgetViewModel.AccountDesc = Code + " On " + Year + " --  Up To " + Period;
                                BudgetViewModel.AccountCode = JournalDetail.AccountCode;

                                BudgetViewModel.TotalYearBudget = CurrentYearPlanTotal;
                                BudgetViewModel.YearlyActuals = CurrentYearActualTotal;
                                BudgetViewModel.YTD = NetYearly;
                                BudgetViewModel.Unit = thisMonthPlanRef.BudgetCurrency.Name;
                                BudgetViewModel.MonthlyBudget = CurrentPeriodPlan;
                                BudgetViewModel.MonthlyActuals = CurrentPeriodActual;
                                BudgetViewModel.MonthlyVariance = PeriodNetPercentage;
                                BudgetViewModel.Month = JournalDetail.GLRecordJournal.PeriodName;
                                BudgetViewModel.WorkUnit = WorkUnitName;
                                BudgetViewModel.Index = JournalDetail.GLRecordJournal.PeriodName + WorkUnitName + JournalDetail.AccountCode;


                                BudgetViewModel.AmharicName = AmharicName;
                                BudgetViewModel.Name = Name;
                                BudgetViewModel.LogoFileName = LogoFileName;
                                BudgetViewModel.Address = Address;

                                BudgetList.Add(BudgetViewModel);


                            }
                            BudgetReturn = new List<BudgetAllocationAndUsageViewModel>();
                            decimal YearlyActual = BudgetList.Sum(x => x.MonthlyActuals);

                        }

                    }

                    else
                    {
                        if (CurrentYearPlanCategory == null)
                        {
                            CurrentYearPlanCategory = new PlanCategory();
                            CurrentYearPlanCategory.PlanCategoryID = 0;

                        }

                        var CurrentYearPeriods = ThisYearRef.GLPeriod.ToList();
                        var LastPeriod = CurrentYearPeriods.Where(x => x.Name == Period).FirstOrDefault();
                        int LastPeriodIndex = CurrentYearPeriods.IndexOf(LastPeriod);

                        var JournalDetails = db.GLRecordJournalDetails.Where(x => x.GLRecordJournal.FiscalYear == Year && x.GLRecordJournal.IsOnlineApproved == true).Where(
                  x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.FromAccountCode) >= 0)
                  .Where(x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.ToAccountCode) <= 0);

                        var TotalBudgetPlan = db.BudgetPlans.Where(so => so.BudgetCategoryID == CurrentPlanCategory).ToList();

                        if (!string.IsNullOrEmpty(WorkUnit))
                        {
                            JournalDetails = JournalDetails.Where(x => x.CostCenterSegment == WorkUnit);
                            TotalBudgetPlan = TotalBudgetPlan.Where(x => x.GLChartOfAccountCostCenterId.ToString() == WorkUnit).ToList();
                        }


                        if (TotalBudgetPlan.Any())
                        {
                            CurrentYearPlanTotal = TotalBudgetPlan.Sum(x => x.TotalYearlyBudget);
                        }

                        if (JournalDetails.Any())
                        {
                            CurrentYearActualTotal = BudgetGLJournalHelper.GetNetJournalsBalance(JournalDetails);
                        }
                        NetYearly = CurrentYearPlanTotal - CurrentYearActualTotal;


                        if (JournalDetails.Any())
                        {
                            decimal CurrentPeriodPlan = 0;
                            decimal CurrentPeriodActual = 0;


                            for (int i = 0; i < LastPeriodIndex; i++)
                            {
                                var CurrentIndexPeriod = CurrentYearPeriods[i];
                                if (CurrentIndexPeriod != null)
                                {
                                    foreach (var JournalDetail in JournalDetails.Where(x => x.GLRecordJournal.PeriodName == CurrentIndexPeriod.Name))
                                    {



                                        var thisMonthActual = JournalDetails.Where(x => x.GLRecordJournal.PeriodName == JournalDetail.GLRecordJournal.PeriodName && x.AccountCode == JournalDetail.AccountCode && x.CostCenterSegment == JournalDetail.CostCenterSegment).FirstOrDefault();

                                        var thisMonthPlanRef = db.BudgetPlans.Where(x => x.BudgetCategoryID == CurrentPlanCategory && x.AccountCode == JournalDetail.AccountCode && x.GLChartOfAccountCostCenterId.ToString() == JournalDetail.CostCenterSegment).FirstOrDefault();


                                        if (thisMonthPlanRef != null)
                                        {
                                            var thisMonthPlan = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == thisMonthPlanRef.BudgetPlanID && x.GLPeriod.Name == JournalDetail.GLRecordJournal.PeriodName).FirstOrDefault();

                                            if (thisMonthPlan != null)
                                            {
                                                CurrentPeriodPlan = thisMonthPlan.CurrentBudget;

                                            }
                                        }


                                        if (thisMonthActual != null)
                                        {
                                            CurrentPeriodActual = BudgetGLJournalHelper.GetNetJournalBalance(thisMonthActual);

                                        }



                                        var WorkUnitObj = db.GLChartOfAccountCostCenters.Where(x => x.Values == JournalDetail.CostCenterSegment).FirstOrDefault();

                                        if (WorkUnitObj != null)
                                        {
                                            WorkUnitName = WorkUnitObj.Description;
                                        }



                                        PeriodNet = CurrentPeriodPlan - CurrentPeriodActual;

                                        PeriodNetPercentage = (CurrentPeriodActual / BudgetGenericHelper.DivideByZero(CurrentPeriodPlan)) * 100;

                                        BudgetViewModel = new BudgetAllocationAndUsageViewModel();

                                        BudgetViewModel.AccountDesc = Code + " On " + Year + " --  Up To " + Period;
                                        BudgetViewModel.AccountCode = JournalDetail.AccountCode;

                                        BudgetViewModel.TotalYearBudget = CurrentYearPlanTotal;
                                        BudgetViewModel.YearlyActuals = CurrentYearActualTotal;
                                        BudgetViewModel.YTD = NetYearly;
                                        BudgetViewModel.Unit = thisMonthPlanRef.BudgetCurrency.Name;
                                        BudgetViewModel.MonthlyBudget = CurrentPeriodPlan;
                                        BudgetViewModel.MonthlyActuals = CurrentPeriodActual;
                                        BudgetViewModel.MonthlyVariance = PeriodNetPercentage;
                                        BudgetViewModel.Month = JournalDetail.GLRecordJournal.PeriodName;
                                        BudgetViewModel.WorkUnit = WorkUnitName;
                                        BudgetViewModel.Index = JournalDetail.GLRecordJournal.PeriodName + WorkUnitName + JournalDetail.AccountCode;


                                        BudgetViewModel.AmharicName = AmharicName;
                                        BudgetViewModel.Name = Name;
                                        BudgetViewModel.LogoFileName = LogoFileName;
                                        BudgetViewModel.Address = Address;

                                        BudgetList.Add(BudgetViewModel);


                                    }
                                }
                            }
                            

                        }
                        //BudgetReturn = new List<BudgetAllocationAndUsageViewModel>();


                        //if (!String.IsNullOrEmpty(Period))
                        //{
                        //    List<GLPeriod> periods = GetPreviousMonthOfYear(Period);
                        //    foreach (var p in periods)
                        //    {
                        //        foreach (var b in BudgetList)
                        //        {
                        //            if (b.Month == p.Name)
                        //            {

                        //                BudgetReturn.Add(b);
                        //            }

                        //        }
                        //    }
                        //    BudgetList = BudgetReturn;
                        //}

                    }


                }

                if (!string.IsNullOrEmpty(AccCode))
                {
                    BudgetList = BudgetList.Where(x => x.AccountCode == AccCode).ToList();
                }
                if (!BudgetList.Any())
                {




                    BudgetViewModel = new BudgetAllocationAndUsageViewModel();

                    BudgetViewModel.AccountDesc = Code + " On " + Year + " --  Up To " + Period;
                    BudgetViewModel.AccountCode =   string.Empty;

                    BudgetViewModel.TotalYearBudget = CurrentYearPlanTotal;
                    BudgetViewModel.YearlyActuals = CurrentYearActualTotal;
                    BudgetViewModel.YTD = NetYearly;
                    BudgetViewModel.Unit =  string.Empty;
                    BudgetViewModel.MonthlyBudget = 0;
                    BudgetViewModel.MonthlyActuals = 0;
                    BudgetViewModel.MonthlyVariance = PeriodNetPercentage;
                    BudgetViewModel.Month = string.Empty;
                    BudgetViewModel.WorkUnit = WorkUnitName;
                    BudgetViewModel.Index = string.Empty;


                    BudgetViewModel.AmharicName = AmharicName;
                    BudgetViewModel.Name = Name;
                    BudgetViewModel.LogoFileName = LogoFileName;
                    BudgetViewModel.Address = Address;


                    BudgetList.Add(BudgetViewModel);

                }
                return BudgetList.DistinctByField(x=>x.Index).ToList();

            }
            catch (Exception e)
            {
                if (!BudgetList.Any())
                {




                    BudgetViewModel = new BudgetAllocationAndUsageViewModel();

                    BudgetViewModel.AccountDesc = Code + " On " + Year + " --  Up To " + Period;
                    BudgetViewModel.AccountCode = string.Empty;
 


                    BudgetViewModel.AmharicName = AmharicName;
                    BudgetViewModel.Name = Name;
                    BudgetViewModel.LogoFileName = LogoFileName;
                    BudgetViewModel.Address = Address;


                    BudgetList.Add(BudgetViewModel);

                }
                return BudgetList;
            }

        }


    }
}



