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
using ExceedERP.Core.Domain.Budget.Task;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.Budget.DataSource
{
    [DataObject]
    public class TaskPlan : List<WorkUnitReportViewModel>
    {
        public ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WorkUnitReportViewModel> GetTaskPlans(string BudgetCategory, string Year, string WorkUnit)
        {
            List<WorkUnitReportViewModel> BudgetList = new List<WorkUnitReportViewModel>();
            WorkUnitReportViewModel BudgetViewModel;
            List<BudgetUsage> BudgetUsages = new List<BudgetUsage>();


            decimal ThisYearMonthlyPlan = decimal.Zero;
            decimal ThisYearMonthlyActual = decimal.Zero;
             decimal CurrentYearActual = decimal.Zero;
           

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
            FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();

            if (string.IsNullOrWhiteSpace(BudgetCategory) || string.IsNullOrWhiteSpace(Year))
            {
                var model = new WorkUnitReportViewModel();
                model.AmharicName = AmharicName;
                model.Name = Name;
                model.LogoFileName = LogoFileName;
                model.Address = Address;

                BudgetList.Add(model);
                return BudgetList;
            }



          


            var CurrentYear = db.GLFiscalYears.Where(x => x.GlFiscalYearId.ToString() == Year).FirstOrDefault();
            var CurrentPeriod = db.GLPeriods.Where(x => x.DateFrom <= DateTime.Now && x.DateTo >= DateTime.Now && x.GlFiscalYearId == CurrentYear.GlFiscalYearId).FirstOrDefault();
            var BudgetCategorys = db.TaskCategorys.Where(x => x.TaskCategoryId.ToString() == BudgetCategory).FirstOrDefault();
            var PlanCategory = db.TaskPlans.Where(x => x.GlFiscalYearId == CurrentYear.GlFiscalYearId && x.BudgetCategoryID.ToString() == BudgetCategory && x.IsOnlineApproved == true).ToList();

            if (CurrentPeriod == null)
            {
                CurrentPeriod = new GLPeriod();
                CurrentPeriod.Name = string.Empty;
                CurrentPeriod.GlFiscalYearId = 0;
            }

            var TaskPerformances = db.TaskPerformances.Where(x => x.GlFiscalYearId == CurrentYear.GlFiscalYearId && x.BudgetCategoryID.ToString() == BudgetCategory && x.IsOnlineApproved == true);

            if (!string.IsNullOrEmpty(WorkUnit))
            {
                TaskPerformances = TaskPerformances.Where(x => x.GLChartOfAccountCostCenterId.ToString() == WorkUnit);
                PlanCategory = PlanCategory.Where(x => x.GLChartOfAccountCostCenterId.ToString() == WorkUnit).ToList();
            }
            List<WorkUnitReportViewModel> List = new List<WorkUnitReportViewModel>();

            foreach (var list in PlanCategory.DistinctByField(x => x.GLChartOfAccountCostCenterId))
            {

                var CostCenter = db.GLChartOfAccountCostCenters.Where(x => x.Values == list.GLChartOfAccountCostCenterId.ToString()).FirstOrDefault();
                var Currency = db.TaskUnits.Where(x => x.TaskUnitId == list.Unit).FirstOrDefault();
                var WorkUnitTotal = PlanCategory.Where(x => x.GLChartOfAccountCostCenterId == list.GLChartOfAccountCostCenterId).ToList();
                var ThisMonthPlans = db.TaskPlanMonthlys.Where(x => x.GLPeriodId == CurrentPeriod.GLPeriodId && x.BudgetDefineID == list.TaskPlanId).FirstOrDefault();
                var ThisMonthActuals = TaskPerformances.Where(x => x.GLPeriodId == CurrentPeriod.GLPeriodId && x.GLChartOfAccountCostCenterId == list.GLChartOfAccountCostCenterId).FirstOrDefault();
                var ThisYearActuals = TaskPerformances.Where(x => x.GLChartOfAccountCostCenterId == list.GLChartOfAccountCostCenterId).ToList();
                var ThisYearUntil = db.TaskPlanMonthlys.Where(x => x.BudgetDefineID == list.TaskPlanId  && x.GLPeriodId <= CurrentPeriod.GLPeriodId).ToList();
               
                if (ThisMonthActuals != null)
                {
                    ThisYearMonthlyActual = ThisMonthActuals.Amount;
                }

                if (ThisMonthPlans != null)
                {
                    ThisYearMonthlyPlan = ThisMonthPlans.MonthlyBudget;
                }
                CurrentYearActual = ThisYearActuals.Sum(x => x.Amount);

                var model = new WorkUnitReportViewModel();

                model.ThisYearBudget = WorkUnitTotal.Sum(x => x.Amount);
                model.ThisYearMonthlyPlan = ThisYearMonthlyPlan;
                model.ThisYearMonthlyActual = ThisYearMonthlyActual;
                if (ThisYearMonthlyActual == decimal.Zero)
                { ThisYearMonthlyActual = 1; }

                model.MonthlyDiffrencePecent = (ThisYearMonthlyPlan / ThisYearMonthlyActual) *100;
                model.CurrentYearPlan = ThisYearUntil.Sum(x => x.MonthlyBudget);
                model.CurrentYearActual = CurrentYearActual;
                if (CurrentYearActual == decimal.Zero)
                { CurrentYearActual = 1; }

                

                decimal YearDiffrence = (ThisYearUntil.Sum(x => x.MonthlyBudget) / CurrentYearActual) * 100;
                model.CurrentYearDifference = YearDiffrence;
                if (YearDiffrence == decimal.Zero)
                { YearDiffrence = 1; }

                
                model.GrandDifferencePercentage = (model.MonthlyDiffrencePecent / YearDiffrence) * 100;

                model.WorkUnit = CostCenter.Description;
                model.Unit = Currency.Name;
                model.AmharicName = AmharicName;
                model.Name = Name;
                model.LogoFileName = LogoFileName;
                model.Address = Address;

                List.Add(model);
            }

            return List;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TaskCategory> GetPlanCategory()
        {


            db = new ExceedDbContext();
            var result = db.TaskCategorys.ToList();
            return result;

        }
    }
}



