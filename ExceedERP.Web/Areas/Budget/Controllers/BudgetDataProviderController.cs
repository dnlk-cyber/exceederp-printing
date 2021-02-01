using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExceedERP.Helpers.Budget;
using System.Web.Mvc;
using ExceedERP.Web.Areas.Budget.Models;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    public class BudgetDataProviderController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        //
        // GET: /Budget/DataProvider/
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult BudgetGroupDDBind()
        {

            var workunit = db.PlanCategorys.Distinct().Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.PlanCategoryID.ToString()
            });

            return Json(workunit, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult BudgetGroupDistinctDDBind()
        {
            List<SelectListItem> BudgetGroup = new List<SelectListItem>();
            string BudgetCode = string.Empty;
            var Categories = db.PlanCategorys;

            foreach (var Category in Categories)
            {
                if (BudgetCode != Category.Name)
                {

                    BudgetGroup.Add(new SelectListItem { Text = Category.Name, Value = Category.PlanCategoryID.ToString() });
                    BudgetCode = Category.Name;
                }
            }
            return Json(BudgetGroup, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult PlanYearByGroup(string Group)
        {

            var planCategorys = db.PlanCategorys.Where(x => x.Name == Group).Select(s => new SelectListItem
            {
                Text = s.GlFiscalYear.Name,
                Value = s.PlanCategoryID.ToString()
            });

            return Json(planCategorys, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult BudgetPlanMonthlyDDBind(string Budgetplan)
        {

            var budgetPlanMonthlys = db.BudgetPlanMonthlys.Select(s => new SelectListItem
            {
                Text = s.AccountDesc,
                Value = s.BudgetPlanMonthlyID.ToString()
            });

            return Json(budgetPlanMonthlys, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult UnitDDBind()
        {

            var budgetPlanMonthlys = db.BudgetPlanMonthlys.Select(s => new SelectListItem
            {
                Text = s.AccountDesc,
                Value = s.BudgetPlanMonthlyID.ToString()
            });

            return Json(budgetPlanMonthlys, JsonRequestBehavior.AllowGet);
        }



        [AllowAnonymous]
        public ActionResult TaskPlanDDBind(int Category, int Year, string WorkUnit)
        {

            var TaskPlans = db.TaskPlans.Where(p => p.GLChartOfAccountCostCenterId == WorkUnit && p.BudgetCategoryID == Category && p.GlFiscalYearId == Year).Select(s => new BindingItem
            {
                Text = s.Name,
                Value = s.TaskPlanId
            }).ToList();

            return Json(TaskPlans, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult BudgetBankAccountMapDDBind()
        {

            var budgetBankAccountMaps = db.BudgetBankAccountMaps.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.BudgetBankAccountMapId.ToString()
            });

            return Json(budgetBankAccountMaps, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult BranchDDBind()
        {
            var Branches = db.Branch.ToList();
            return Json(Branches, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult CurrencyDDBind()
        {

            var workunit = db.BudgetCurrencies.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.BudgetCurrencyId.ToString()
            });

            return Json(workunit, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult TaskUnitDDBind()
        {

            var workunit = db.TaskUnits.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.TaskUnitId.ToString()
            });

            return Json(workunit, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult TaskCategoryDDBind()
        {

            var taskCategorys = db.TaskCategorys.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.TaskCategoryId.ToString()
            });

            return Json(taskCategorys, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult PeriodDDBind(string Year)
        {
            db = new ExceedDbContext();

            var fiscalYear = db.GLFiscalYears.Where(x => x.Name == Year).FirstOrDefault().GlFiscalYearId;
            var Periods = db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYear).Select(s => new { Name = s.Name }).ToList();
            return Json(Periods, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult YearDDBind()
        {
            db = new ExceedDbContext();
            var fiscalYears = db.GLFiscalYears.Select(s => new
            {
                GlFiscalYearId = s.GlFiscalYearId,
                Name = s.Name
            }).ToList();

            return Json(fiscalYears, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult PlanDDBind()
        {
            db = new ExceedDbContext();
            var result = db.BudgetPlans.AsNoTracking().Distinct().ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult PlanTransferDDBind(string budgetYear)
        {
            db = new ExceedDbContext();
            var result = db.BudgetPlans.Where(x => x.BudgetYear != budgetYear).Distinct().ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult MonthDDBind()
        {
            db = new ExceedDbContext();

            List<SelectListItem> result = GetMonths();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult AccCodeByGroupYear(int PlanCategoryID)
        {
            var plans = db.BudgetPlans.Where(x => x.BudgetCategoryID == PlanCategoryID).DistinctByField(X => X.AccountCode).Select(s => new SelectListItem
         {
             Text = s.AccountCode,
             Value = s.AccountCode
         });

            return Json(plans, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult BudgetGLChartOfAccountCostCenterId(int PlanCategoryID, string AccCode)
        {
            List<SelectListItem> workUnit = new List<SelectListItem>();
            var plans = db.TaskPlans.Where(x => x.BudgetCategoryID == PlanCategoryID).ToList();

            foreach (var plan in plans)
            {
                GLChartOfAccountCostCenter strucure = db.GLChartOfAccountCostCenters.Where(x => x.Values == plan.GLChartOfAccountCostCenterId.ToString()).FirstOrDefault();


                var item = new SelectListItem
                {
                    Text = strucure.Description,
                    Value = strucure.Values
                };
                workUnit.Add(item);
            }

            return Json(workUnit.Distinct().ToList(), JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult TaskToWorkUnit(int PlanCategoryID, string AccCode, string GLChartOfAccountCostCenterId)
        {
            List<SelectListItem> workUnit = new List<SelectListItem>();
            var plans = db.TaskPlans.Where(x => x.BudgetCategoryID == PlanCategoryID && x.GLChartOfAccountCostCenterId.ToString() != GLChartOfAccountCostCenterId).ToList();

            foreach (var plan in plans)
            {
                GLChartOfAccountCostCenter strucure = db.GLChartOfAccountCostCenters.Where(x => x.Values == plan.GLChartOfAccountCostCenterId.ToString()).FirstOrDefault();


                var item = new SelectListItem
                {
                    Text = strucure.Description,
                    Value = strucure.Values.ToString()
                };
                workUnit.Add(item);
            }

            return Json(workUnit.Distinct().ToList(), JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public ActionResult BudgetTransferFromAccCode(int PlanCategoryID, string WorkUnit)
        {
            List<SelectListItem> workUnit = new List<SelectListItem>();
            var plans = db.BudgetPlans.Where(x => x.BudgetCategoryID == PlanCategoryID && x.GLChartOfAccountCostCenterId.ToString() == WorkUnit).ToList();

            var AccountCodes = plans.DistinctByField(x => x.AccountCode).Select(s => new SelectListItem() { Text = s.AccountCode, Value = s.AccountCode }).ToList();

            return Json(AccountCodes, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult BudgetTransferToAccCode(int PlanCategoryID, string WorkUnit, string AccCode)
        {
            List<SelectListItem> workUnit = new List<SelectListItem>();
            var plans = db.BudgetPlans.Where(x => x.BudgetCategoryID == PlanCategoryID && x.GLChartOfAccountCostCenterId.ToString() == WorkUnit).ToList();

            var AccountCodes = plans.DistinctByField(x => x.AccountCode).Where(x => x.AccountCode != AccCode).Select(s => new SelectListItem() { Text = s.AccountCode, Value = s.AccountCode }).ToList();

            return Json(AccountCodes, JsonRequestBehavior.AllowGet);
        }






        /*   [AllowAnonymous]
           public ActionResult BudgetToWorkUnit(int PlanCategoryID, string AccCode, string GLChartOfAccountCostCenterId)
           {
               List<SelectListItem> workUnit = new List<SelectListItem>();
               var plans = db.TaskPlans.Where(x => x.BudgetCategoryID == PlanCategoryID && x.GLChartOfAccountCostCenterId.ToString() != GLChartOfAccountCostCenterId).ToList();

               foreach (var plan in plans)
               {
                   GLChartOfAccountCostCenter strucure = db.GLChartOfAccountCostCenters.Where(x => x.Values == plan.GLChartOfAccountCostCenterId.ToString()).FirstOrDefault();


                   var item = new SelectListItem
                   {
                       Text = strucure.Description,
                       Value = strucure.Values.ToString()
                   };
                   workUnit.Add(item);
               }

               return Json(workUnit.Distinct().ToList(), JsonRequestBehavior.AllowGet);
           }*/
        [AllowAnonymous]
        public ActionResult BudgetFromWorkUnit(int PlanCategoryID)
        {
            List<SelectListItem> workUnit = new List<SelectListItem>();
            var plans = db.BudgetPlans.Where(x => x.BudgetCategoryID == PlanCategoryID).ToList();

            foreach (var plan in plans)
            {
                GLChartOfAccountCostCenter strucure = db.GLChartOfAccountCostCenters.Where(x => x.Values == plan.GLChartOfAccountCostCenterId.ToString()).FirstOrDefault();


                var item = new SelectListItem
                {
                    Text = strucure.Description,
                    Value = strucure.Values.ToString()
                };
                workUnit.Add(item);
            }

            return Json(workUnit.Distinct().ToList(), JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult BudgetToWorkUnit(int PlanCategoryID, string GLChartOfAccountCostCenterId)
        {
            List<SelectListItem> workUnit = new List<SelectListItem>();
            var plans = db.BudgetPlans.Where(x => x.BudgetCategoryID == PlanCategoryID).ToList();

            foreach (var plan in plans)
            {
                GLChartOfAccountCostCenter strucure = db.GLChartOfAccountCostCenters.Where(x => x.Values == plan.GLChartOfAccountCostCenterId.ToString()).FirstOrDefault();


                var item = new SelectListItem
                {
                    Text = strucure.Description,
                    Value = strucure.Values.ToString()
                };
                workUnit.Add(item);
            }

            return Json(workUnit.Distinct().ToList(), JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult ApprovalDDBind()
        {
            var approvals = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject>();
            var approval = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
            {
                Name = BudgetConstants.Approved,
                Id = 1,
            };
            approvals.Add(approval);

            var approval2 = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
            {
                Name = BudgetConstants.Pending,
                Id = 2,
            };
            approvals.Add(approval2);
            var approval3 = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
            {
                Name = BudgetConstants.Rejected,
                Id = 3,
            };
            approvals.Add(approval3);
            return Json(approvals, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult WorkUnitDDBind()
        {

            AccountHelper accountHelper = new AccountHelper();


            if (accountHelper.IsDynamicCoA())
            {
                var glSetupCostCenter =
             db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                var currentCostCenter = db.GlChartOfAccountSegments
                    .Where(x => x.AllowBudgeting == true && x.Enable == true)
           .Where(x => x.GLSegmentSetupId == glSetupCostCenter.GLSegmentSetupId)
           .Select(s => new SelectListItem
            {
                Text = s.Description,
                Value = s.Values
            }).ToList();


                return Json(currentCostCenter, JsonRequestBehavior.AllowGet);


            }

            var workunit = db.GLChartOfAccountCostCenters.Where(x => x.AllowBudgeting == true && x.Enable == true).Select(s => new SelectListItem
            {
                Text = s.Description,
                Value = s.Values
            });

            return Json(workunit, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public List<BindingItem> GetMonths(int Id)
        {
            List<BindingItem> Months = new List<BindingItem>();

            Months = db.GLPeriods.Where(x => x.GlFiscalYear.GlFiscalYearId == Id).Select(s => new BindingItem
            {
                Text = s.Name,
                Value = s.GLPeriodId
            }).ToList();


            return Months;

        }
        [AllowAnonymous]
        public ActionResult GetUnits()
        {
            List<SelectListItem> units = new List<SelectListItem>();
            var currency = db.BudgetCurrencies.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Name.ToString()
            });


            return Json(currency, JsonRequestBehavior.AllowGet);

        }


        [AllowAnonymous]
        public ActionResult CategoryDDBind()
        {

            var workunit = db.BudgetCategorys.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.BudgetCategoryID.ToString()
            });

            return Json(workunit, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult CategoryTypeDDBind()
        {

            var CategoryType = db.BudgetTypes.Select(s => new SelectListItem()
            {
                Text = s.Name,
                Value = s.Name
            }).ToList();

            return Json(CategoryType, JsonRequestBehavior.AllowGet);
        }

        #region Support Method

        public List<SelectListItem> GetMonths()
        {
            List<SelectListItem> months = new List<SelectListItem>();
            var Year = db.GLFiscalYears.Where(x => x.DateFrom <= DateTime.Now && x.DateTo >= DateTime.Now).FirstOrDefault();
            if (Year != null)
            {
                months = db.GLPeriods.Where(x => x.GlFiscalYearId == Year.GlFiscalYearId).DistinctByField(x => x.Name).Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.GLPeriodId.ToString()
                }).ToList();

                return months;
            }
            return new List<SelectListItem>();

        }

        #endregion

    }
}