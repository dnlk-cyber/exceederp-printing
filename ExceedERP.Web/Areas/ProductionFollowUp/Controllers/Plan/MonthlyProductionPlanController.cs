using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using ExceedERP.Core.Domain.ProductionFollowUp;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers;

namespace ExceedERP.Areas.DailyMarket.Controllers.Setting
{
    public class MonthlyProductionPlanController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["BudgetYears"] = db.GLFiscalYears.ToList();
            ViewData["Periods"] = db.GLPeriods.ToList();
            var quarterPlans = db.QuarterlyProductionPlans.ToList();
            ViewData["QuarterlyProductionPlans"] = quarterPlans;
            var quarters = quarterPlans.ToList();
            ViewData["Quarters"] = quarters.Select(s => new {
                QuarterlyProductionPlanId = s.QuarterlyProductionPlanId,

                //Name = getName(s.QuarterlyProductionProductionPlanId),
                //NameAm = getName(s.QuarterlyProductionProductionPlanId)
            }).ToList();

            return View();
        }
        //private string getName(int quarterPlanId)
        //{
            
        //        var quarter = db.QuarterlyProductionPlans.Find(quarterPlanId);
        //        if (quarter != null)
        //        {
        //            var halfYear = db.HalfYearlyProductionPlans.Find(quarter.HalfYearlyProductionPlanId);
        //            if (halfYear != null)
        //            {

        //                var year = db.YearlyProductionPlans.Find(halfYear.YearlyProductionPlanId);
        //                if (year != null)
        //                {
        //                    var budgetYear = db.G.Find(year.BudgetYearId);
        //                    if (budgetYear != null)
        //                    {
        //                        string cultureName = "";
        //                        HttpCookie cultureCookie = Request.Cookies["_culture"];
        //                        if (cultureCookie != null)
        //                        {
        //                            cultureName = cultureCookie.Value;
        //                        }
        //                        if (cultureName.IsCaseInsensitiveEqual("am") || cultureName.IsCaseInsensitiveEqual("or"))
        //                        {
        //                            return budgetYear.Year + "_" + quarter.NameAm + "___" + Resource.Plan + ":" + quarter.Plan;

        //                        }
        //                        else
        //                        {
        //                            return budgetYear.Year + "_" + quarter.Name + "___" + Resource.Plan + ":" + quarter.Plan;

        //                        }
        //                    }
        //                }
        //            }
        //        }

        //    return "";

        //}

        public ActionResult MonthlyPlans_Read([DataSourceRequest] DataSourceRequest request, int? id)
        {

            IQueryable<MonthlyProductionPlan> monthlyPlans = db.MonthlyProductionPlans;
            if (id != null)
            {
                monthlyPlans = monthlyPlans.Where(q =>q.QuarterlyProductionPlan.HalfYearlyProductionPlan.YearlyPlan.YearlyProductionPlanId==id);

            }
           
            DataSourceResult result = monthlyPlans.ToDataSourceResult(request, MonthlyPlan => new
            {
                MonthlyProductionPlanId = MonthlyPlan.MonthlyProductionPlanId,
                QuarterlyProductionPlanId = MonthlyPlan.QuarterlyProductionPlanId,
                StartDate = MonthlyPlan.StartDate,
                EndDate = MonthlyPlan.EndDate,
                StartDateEth = MonthlyPlan.StartDateEth,
                EndDateEth = MonthlyPlan.EndDateEth,
                ProductionPlanBirr = MonthlyPlan.ProductionPlanBirr,
                ProductionPlanQuantity = MonthlyPlan.ProductionPlanQuantity,
                Name = MonthlyPlan.Name,
                NameAm = MonthlyPlan.NameAm,
              
            });

            return Json(result,JsonRequestBehavior.AllowGet);
        }
        public ActionResult MonthlyPlans_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<MonthlyProductionPlan> monthlyPlans)
        {
            if (monthlyPlans != null && ModelState.IsValid)
               
            {
                var monthlyGroupByQuarters = monthlyPlans.GroupBy(g => g.QuarterlyProductionPlanId)
                   .Select(s => new
                   {
                       QuarterlyProductionPlanId = s.Key,
                       ProductionPlanQuantity = s.Sum(su => su.ProductionPlanQuantity),
                       ProductionPlanBirr = s.Sum(su => su.ProductionPlanBirr)


                   });
                //foreach(var monthlyPlan in monthlyGroupByQuarters)
                //{
                //    var monthIds = monthlyPlans.Select(s=>s.MonthlyPlanId);
                //    var quarterPlan = db.QuarterlyProductionPlans.Find(monthlyPlan.QuarterlyProductionPlanId);
                //    var totMonthPlans = db.MonthlyPlans.Where(q => q.QuarterlyProductionPlanId == monthlyPlan.QuarterlyProductionPlanId && !(monthIds.Contains(q.MonthlyPlanId))).ToList().Sum(s => s.Plan);
                ////    var totMonthPlans = db.MonthlyPlans.Where(q => q.QuarterlyProductionPlanId == monthlyPlan.QuarterlyProductionPlanId && monthIds.Contains(q.MonthlyPlanId)).Sum(s => s.Plan);
                //    if ((totMonthPlans + monthlyPlan.Plan) > quarterPlan.Plan)
                //    {
                //        ModelState.AddModelError("Error", "Summation of month plans must equals quarter year plan!");
                //    }
                //}
                
                //if (ModelState.IsValid)
                //{
                //    foreach (var monthlyPlan in monthlyPlans)
                //{
                    
                   
                //        var entity = db.MonthlyPlans.Find(monthlyPlan.MonthlyPlanId);
                //        if (entity != null)
                //        {
                         
                //            entity.Plan = monthlyPlan.Plan;
                //            db.MonthlyPlans.Attach(entity);
                //            db.Entry(entity).State = EntityState.Modified;
                //        }
                //    }
                //    db.SaveChanges();
                //    #region Update log
                //    UserHelper.OperationLog(Request.UserHostAddress, "Update", "Success", User.Identity.Name, "Updated LED Display Commodity category Setting with Id: " + monthlyPlans.Select(s => s.MonthlyPlanId).ToString(), Core.Domain.Global.GlobalEnum.ExceedGroup.DailyMarket);
                //    #endregion
                //}


            }

            return Json(new[] { monthlyPlans }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
