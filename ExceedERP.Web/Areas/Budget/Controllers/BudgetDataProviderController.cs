using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Web.Areas.Budget.Models;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.Common.ViewModel;

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

      
        public ActionResult BudgetGroupDDBind()
        {

            var workunit = db.PlanCategorys.Distinct().Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.PlanCategoryID.ToString()
            });

            return Json(workunit, JsonRequestBehavior.AllowGet);
        }
      
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
      
        public ActionResult PlanYearByGroup(string Group)
        {

            var planCategorys = db.PlanCategorys.Where(x => x.Name == Group).Select(s => new SelectListItem
            {
                Text = s.GlFiscalYear.Name,
                Value = s.PlanCategoryID.ToString()
            });

            return Json(planCategorys, JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult BudgetPlanMonthlyDDBind(string Budgetplan)
        {

            var budgetPlanMonthlys = db.BudgetPlanMonthlys.Select(s => new SelectListItem
            {
               // Text = s.AccountDesc,
                Value = s.BudgetPlanMonthlyID.ToString()
            });

            return Json(budgetPlanMonthlys, JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult UnitDDBind()
        {

            var budgetPlanMonthlys = db.BudgetPlanMonthlys.Select(s => new SelectListItem
            {
               // Text = s.AccountDesc,
                Value = s.BudgetPlanMonthlyID.ToString()
            });

            return Json(budgetPlanMonthlys, JsonRequestBehavior.AllowGet);
        }



        //
        //public ActionResult TaskPlanDDBind(int Category, int Year, string WorkUnit)
        //{

        //    var TaskPlans = db.TaskPlans.Where(p => p.GLChartOfAccountCostCenterId == WorkUnit && p.BudgetCategoryID == Category && p.GlFiscalYearId == Year).Select(s => new BindingItem
        //    {
        //        Text = s.Name,
        //        Value = s.TaskPlanId
        //    }).ToList();

        //    return Json(TaskPlans, JsonRequestBehavior.AllowGet);
        //}
      
        public ActionResult BudgetBankAccountMapDDBind()
        {

            var budgetBankAccountMaps = db.BudgetBankAccountMaps.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.BudgetBankAccountMapId.ToString()
            });

            return Json(budgetBankAccountMaps, JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult BranchDDBind()
        {
            var Branches = db.Branch.ToList();
            return Json(Branches, JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult CurrencyDDBind()
        {

            var workunit = db.FinSetupCurrencies.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.FinSetupCurrencyID.ToString()
            });

            return Json(workunit, JsonRequestBehavior.AllowGet);
        }
        //
        //public ActionResult TaskUnitDDBind()
        //{

        //    var workunit = db.TaskUnits.Select(s => new SelectListItem
        //    {
        //        Text = s.Name,
        //        Value = s.TaskUnitId.ToString()
        //    });

        //    return Json(workunit, JsonRequestBehavior.AllowGet);
        //}
        //
        //public ActionResult TaskCategoryDDBind()
        //{

        //    var taskCategorys = db.TaskCategorys.Select(s => new SelectListItem
        //    {
        //        Text = s.Name,
        //        Value = s.TaskCategoryId.ToString()
        //    });

        //    return Json(taskCategorys, JsonRequestBehavior.AllowGet);
        //}

      
        public ActionResult PeriodDDBind(string Year)
        {
           

            var fiscalYear = db.GLFiscalYears.Where(x => x.Name == Year).FirstOrDefault().GlFiscalYearId;
            var Periods = db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYear).OrderByDescending(x => x.DateFrom).Select(s => new { Name = s.Name }).ToList();
            return Json(Periods, JsonRequestBehavior.AllowGet);
        }
      
        public JsonResult YearDDBind()
        {
           
            var fiscalYears = db.GLFiscalYears.OrderByDescending(x=>x.DateFrom).Select(s => new
            {
                GlFiscalYearId = s.GlFiscalYearId,
                Name = s.Name
            }).ToList();

            return Json(fiscalYears, JsonRequestBehavior.AllowGet);
        }

      
        public ActionResult PlanDDBind()
        {
           
            var result = db.BudgetPlans.AsNoTracking().Distinct().ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

      
        public ActionResult PlanTransferDDBind(int budgetYear)
        {
           
            var result = db.BudgetPlans.Where(x => x.FiscalYearId != budgetYear).Distinct().ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult MonthDDBind()
        {
           

            List<SelectListItem> result = GetMonths();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

      
        public ActionResult AccCodeByGroupYear(int PlanCategoryID)
        {

        var plans = db.BudgetPlans
                .Where(x => x.PlanCategoryId == PlanCategoryID)
                .DistinctByField(X => X.AccountCode)
                .Select(s => new SelectListItem
                 {
                     Text = s.AccountCode,
                     Value = s.AccountCode
                 });
            
            return Json(plans, JsonRequestBehavior.AllowGet);
        }
        //
        //public ActionResult BudgetGLChartOfAccountCostCenterId(int PlanCategoryID, string AccCode)
        //{
        //    List<SelectListItem> workUnit = new List<SelectListItem>();
        //    var plans = db.TaskPlans.Where(x => x.BudgetCategoryID == PlanCategoryID).ToList();

        //    foreach (var plan in plans)
        //    {
        //        var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
        //        var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
        //        var strucure = glChartOfAccountCostCenters.Where(x => x.Values == plan.GLChartOfAccountCostCenterId.ToString()).FirstOrDefault();


        //        var item = new SelectListItem
        //        {
        //            Text = strucure.Description,
        //            Value = strucure.Values
        //        };
        //        workUnit.Add(item);
        //    }

        //    return Json(workUnit.Distinct().ToList(), JsonRequestBehavior.AllowGet);
        //}
        //
        //public ActionResult TaskToWorkUnit(int PlanCategoryID, string AccCode, string GLChartOfAccountCostCenterId)
        //{
        //    List<SelectListItem> workUnit = new List<SelectListItem>();
        //    var plans = db.TaskPlans.Where(x => x.BudgetCategoryID == PlanCategoryID && x.GLChartOfAccountCostCenterId.ToString() != GLChartOfAccountCostCenterId).ToList();

        //    foreach (var plan in plans)
        //    {
        //        var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
        //        var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
        //        var strucure = glChartOfAccountCostCenters.Where(x => x.Values == plan.GLChartOfAccountCostCenterId.ToString()).FirstOrDefault();


        //        var item = new SelectListItem
        //        {
        //            Text = strucure.Description,
        //            Value = strucure.Values.ToString()
        //        };
        //        workUnit.Add(item);
        //    }

        //    return Json(workUnit.Distinct().ToList(), JsonRequestBehavior.AllowGet);
        //}
        
      
        public ActionResult BudgetTransferFromAccCode(int PlanCategoryID, string budgetitem)
        {
            
            List<SelectListItem> workUnit = new List<SelectListItem>(); //??

            //Gets PlanCategoryID and WorkUnit-from "From Work Unit" this field cascades from the from workunit 
            var plans = db.BudgetPlans
                .Where(x => x.PlanCategoryId == PlanCategoryID && x.BudgetItem == budgetitem)
                .ToList();
            
            var AccountCodes = plans.DistinctByField(x => x.AccountCode).Select(s => new SelectListItem()
            {
                Text = s.AccountCode,
                Value = s.AccountCode
            }).ToList();
            
            return Json(AccountCodes, JsonRequestBehavior.AllowGet);
        }

      
        public ActionResult BudgetTransferToAccCode(int PlanCategoryID, string budgetitem, string AccCode)
        {
            List<SelectListItem> workUnit = new List<SelectListItem>();
            //filters by WorkUnit and PlanCategoryId 
            var plans = db.BudgetPlans
                .Where(x => x.PlanCategoryId == PlanCategoryID && x.BudgetItem == budgetitem)
                .ToList();
            //filters by AccCode
            var AccountCodes = plans
                .DistinctByField(x => x.AccountCode)
                .Where(x => x.AccountCode != AccCode).Select(s => new SelectListItem()
                {
                    Text = s.AccountCode,
                    Value = s.AccountCode
                }).ToList();

            return Json(AccountCodes, JsonRequestBehavior.AllowGet);
        }


        /* 
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
      
        public ActionResult BudgetFromWorkUnit(int PlanCategoryID)
        {
            List<SelectListItem> workUnit = new List<SelectListItem>();
            var plans = db.BudgetPlans.Where(x => x.PlanCategoryId == PlanCategoryID).ToList();

            foreach (var plan in plans)
            {
                var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
                //var strucure = glChartOfAccountCostCenters.Where(x => x.Values == plan.GLChartOfAccountCostCenterValue.ToString()).FirstOrDefault();


                //var item = new SelectListItem
                //{
                //    Text = strucure.Description,
                //    Value = strucure.Values.ToString()
                //};
                //workUnit.Add(item);
            }

            return Json(workUnit.Distinct().ToList(), JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult BudgetToWorkUnit(int PlanCategoryID, string GLChartOfAccountCostCenterId)
        {
            List<SelectListItem> workUnit = new List<SelectListItem>();
            var plans = db.BudgetPlans.Where(x => x.PlanCategoryId == PlanCategoryID).ToList();

            foreach (var plan in plans)
            {
                var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
                //var strucure = glChartOfAccountCostCenters.Where(x => x.Values == plan.GLChartOfAccountCostCenterValue.ToString()).FirstOrDefault();


                //var item = new SelectListItem
                //{
                //    Text = strucure.Description,
                //    Value = strucure.Values.ToString()
                //};
                //workUnit.Add(item);
            }

            return Json(workUnit.Distinct().ToList(), JsonRequestBehavior.AllowGet);
        }
        //
        //public ActionResult ApprovalDDBind()
        //{
        //    var approvals = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject>();
        //    var approval = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
        //    {
        //        Name = BudgetConstants.Approved,
        //        Id = 1,
        //    };
        //    approvals.Add(approval);

        //    var approval2 = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
        //    {
        //        Name = BudgetConstants.Pending,
        //        Id = 2,
        //    };
        //    approvals.Add(approval2);
        //    var approval3 = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
        //    {
        //        Name = BudgetConstants.Rejected,
        //        Id = 3,
        //    };
        //    approvals.Add(approval3);
        //    return Json(approvals, JsonRequestBehavior.AllowGet);
        //}
       
        public ActionResult WorkUnitDDBind()
        {

            AccountHelper accountHelper = new AccountHelper();
            var glSetupCostCenter = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
            if (glSetupCostCenter != null)
            {
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

            return null;
        }


      
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
      
        public ActionResult GetUnits()
        {
            List<SelectListItem> units = new List<SelectListItem>();
            var currency = db.FinSetupCurrencies.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Name.ToString()
            });


            return Json(currency, JsonRequestBehavior.AllowGet);

        }


      
        public ActionResult CategoryDDBind()
        {

            var workunit = db.BudgetCategorys.Select(s => new SelectorVm
            {
                Name = s.Name,
                Code = s.BudgetCategoryID.ToString()
            });

            return Json(workunit, JsonRequestBehavior.AllowGet);
        }

      
        public ActionResult PlanCategoryDDBind()
        {
            var planCategory = db.PlanCategorys.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.PlanCategoryID.ToString()
            });
            return Json(planCategory, JsonRequestBehavior.AllowGet);
        }

      
        public ActionResult CategoryTypeDDBind()
        {

            var CategoryType = db.BudgetTypes.Select(s => new SelectListItem()
            {
                Text = s.Name,
                Value = s.Name
            }).ToList();

            return Json(CategoryType, JsonRequestBehavior.AllowGet);
        }






        //test for budgettransfer
      
        public ActionResult AccCodeByCategory(/*string planCategoryID*/)
        {

            var plans = db.BudgetPlans
                    //.Where(x => x.BudgetCategoryID.ToString() ==planCategoryID)
                    .DistinctByField(X => X.AccountCode)
                    .Select(s => new SelectListItem
                    {
                        Text = s.AccountCode,
                        Value = s.AccountCode
                    });

            return Json(plans, JsonRequestBehavior.AllowGet);
        }

        #region Support Method

        public List<SelectListItem> GetMonths()
        {
            List<SelectListItem> months = new List<SelectListItem>();
            var Year = db.GLFiscalYears.Where(x => x.DateFrom <= DateTime.Now && x.DateTo >= DateTime.Now).FirstOrDefault();
            if (Year != null)
            {
                months = db.GLPeriods.Where(x => x.GlFiscalYearId == Year.GlFiscalYearId).OrderByDescending(x => x.DateFrom).DistinctByField(x => x.Name).Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.GLPeriodId.ToString()
                }).ToList();

                return months;
            }
            return new List<SelectListItem>();

        }

        #endregion


        ///new 2023...//
        ///

        public JsonResult GetBudgetableWorkUnits()
        {
            //will be customized based on how much they do down to bduget
            var data = db.MainCompanyStructures
                .Select(s => new { s.OrgStructureId, s.Name }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBudgetItems(string catId,string text)
        {
            List<SelectorVm> list = new List<SelectorVm>();
            if(!String.IsNullOrEmpty(catId))
            {
                int did = int.Parse(catId);
                var planCategory = db.PlanCategorys.Find(did);
                //var budgetCategory = db.PlanCategorys.Find(planCategory.BudgetCategoryID);

                var data = db.BudgetCategoryDetails.Where(x => x.BudgetCategoryId == planCategory.BudgetCategoryID).ToList();

                if (!String.IsNullOrEmpty(text))
                    data = data.Where(x => x.BudgetItem.ToUpper().Contains(text.ToUpper())).ToList();

                foreach (var d in data)
                {
                    SelectorVm r = new SelectorVm
                    {
                        Code = d.BudgetCategoryDetailID.ToString(),
                        Name = d.BudgetCode+"-"+d.BudgetItem
                    };

                    list.Add(r);
                }
            }
           
            return Json(list.Select(s => 
            new { s.Code, s.Name })
            .ToList(),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategoryBudgetItems(string catId, string text)
        {
            List<SelectorVm> list = new List<SelectorVm>();
            if (!String.IsNullOrEmpty(catId))
            {
                int did = int.Parse(catId);

                var data = db.BudgetCategoryDetails.Where(x => x.BudgetCategoryId == did).ToList();

                if (!String.IsNullOrEmpty(text))
                    data = data.Where(x => x.BudgetItem.ToUpper().Contains(text.ToUpper())).ToList();

                foreach (var d in data)
                {
                    SelectorVm r = new SelectorVm
                    {
                        Code = d.BudgetCategoryDetailID.ToString(),
                        Name = d.BudgetCode + "-" + d.BudgetItem
                    };

                    list.Add(r);
                }
            }

            return Json(list.Select(s =>
            new { s.Code, s.Name })
            .ToList(),
                JsonRequestBehavior.AllowGet);
        }
    }
}