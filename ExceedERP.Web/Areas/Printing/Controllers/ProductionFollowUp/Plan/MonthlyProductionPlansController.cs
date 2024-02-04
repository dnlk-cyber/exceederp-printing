using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers;
using System.Web;
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Plan
{
    public class MonthlyProductionPlansController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYears"] = db.GLFiscalYears.ToList();
            var quarterPlans = db.QuarterlyProductionPlans.ToList();
            var quarters = quarterPlans.ToList();
            ViewData["Quarters"] = quarters.Select(s => new {
                QuarterlyProductionPlanId = s.QuarterlyProductionPlanId,
                Name = getName(s.QuarterlyProductionPlanId),
                NameAm = getName(s.QuarterlyProductionPlanId)
            }).ToList();

            return View();
        }
        private string getName(int quarterPlanId)
        {

            var quarter = db.QuarterlyProductionPlans.Find(quarterPlanId);
            if (quarter != null)
            {
                var halfYear = db.HalfYearlyProductionPlans.Find(quarter.HalfYearlyProductionPlanId);
                if (halfYear != null)
                {

                    var year = db.YearlyProductionPlans.Find(halfYear.YearlyProductionPlanId);
                    if (year != null)
                    {
                        var budgetYear = db.GLFiscalYears.Find(year.GlFiscalYearId);
                        if (budgetYear != null)
                        {
                            string cultureName = "";
                            HttpCookie cultureCookie = Request.Cookies["_culture"];
                            if (cultureCookie != null)
                            {
                                cultureName = cultureCookie.Value;
                            }
                            if (cultureName.IsCaseInsensitiveEqual("am") || cultureName.IsCaseInsensitiveEqual("or"))
                            {
                                return budgetYear.Name + "_" + quarter.NameAm + "___Plan:" + quarter.ProductionPlanQuantity;

                            }
                            else
                            {
                                return budgetYear.Name + "_" + quarter.Name + "___Plan:" + quarter.ProductionPlanQuantity;

                            }
                        }
                    }
                }
            }

            return "";

        }

        public ActionResult MonthlyProductionPlans_Read([DataSourceRequest] DataSourceRequest request, int? id)
        {

            IQueryable<MonthlyProductionPlan> monthlyPlans = db.MonthlyProductionPlans;
            if (id != null)
            {
                monthlyPlans = monthlyPlans.Where(q => q.QuarterlyProductionPlan.HalfYearlyProductionPlan.YearlyPlan.YearlyProductionPlanId == id);

            }

            DataSourceResult result = monthlyPlans.ToDataSourceResult(request, MonthlyPlan => new
            {
                MonthlyProductionPlanId = MonthlyPlan.MonthlyProductionPlanId,
                QuarterlyProductionPlanId = MonthlyPlan.QuarterlyProductionPlanId,
                StartDate = MonthlyPlan.StartDate,
                EndDate = MonthlyPlan.EndDate,
                StartDateEth = MonthlyPlan.StartDateEth,
                EndDateEth = MonthlyPlan.EndDateEth,
                WorkingHours = MonthlyPlan.WorkingHours,
                WorkingDays = MonthlyPlan.WorkingDays,
                ProductionPlanBirr = MonthlyPlan.ProductionPlanBirr,
                ProductionPlanQuantity = MonthlyPlan.ProductionPlanQuantity,
                Name = MonthlyPlan.Name,
                NameAm = MonthlyPlan.NameAm,

            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MonthlyProductionPlans_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<MonthlyProductionPlan> monthlyPlans)
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
                foreach (var monthlyPlan in monthlyGroupByQuarters)
                {
                    var monthIds = monthlyPlans.Select(s => s.MonthlyProductionPlanId);
                    var quarterPlan = db.QuarterlyProductionPlans.Find(monthlyPlan.QuarterlyProductionPlanId);
                    var plans = db.MonthlyProductionPlans.Where(q => q.QuarterlyProductionPlanId == monthlyPlan.QuarterlyProductionPlanId && !(monthIds.Contains(q.MonthlyProductionPlanId))).ToList();
                    var totMonthPlanQuas = plans.Sum(s => s.ProductionPlanQuantity);
                    var totMonthPlanBirrs = plans.Sum(s => s.ProductionPlanBirr);
                    //    var totMonthPlans = db.MonthlyPlans.Where(q => q.QuarterlyProductionPlanId == monthlyPlan.QuarterlyProductionPlanId && monthIds.Contains(q.MonthlyPlanId)).Sum(s => s.Plan);
                    if ((totMonthPlanQuas + monthlyPlan.ProductionPlanQuantity) > quarterPlan.ProductionPlanQuantity)
                    {
                        ModelState.AddModelError("Error", "Summation of monthly plans in quantity must equals quarterly plan!");
                    }
                    if ((totMonthPlanBirrs + monthlyPlan.ProductionPlanBirr) > quarterPlan.ProductionPlanBirr)
                    {
                        ModelState.AddModelError("Error", "Summation of monthly plans in Birr must equals quarterly plan!");
                    }
                }

                if (ModelState.IsValid)
                {
                    foreach (var monthlyPlan in monthlyPlans)
                    {


                        var entity = db.MonthlyProductionPlans.Find(monthlyPlan.MonthlyProductionPlanId);
                        if (entity != null)
                        {

                            entity.StartDate = monthlyPlan.StartDate;
                            entity.EndDate = monthlyPlan.EndDate;
                            entity.WorkingDays = monthlyPlan.WorkingDays;
                            entity.WorkingHours = monthlyPlan.WorkingHours;
                            entity.ProductionPlanBirr = monthlyPlan.ProductionPlanBirr;
                            entity.ProductionPlanQuantity = monthlyPlan.ProductionPlanQuantity;
                            db.MonthlyProductionPlans.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();

                }


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
