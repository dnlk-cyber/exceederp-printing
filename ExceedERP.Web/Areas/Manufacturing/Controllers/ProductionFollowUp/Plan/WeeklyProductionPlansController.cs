using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Plan

{
    public class FurnitureWeeklyProductionPlansController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        //[AuthorizeRoles(DailyMarketRoles.DMPriceEntry, DailyMarketRoles.DMPlanApprover, DailyMarketRoles.DMPlanner)]
        public ActionResult Index()
        {
            ViewData["BudgetYears"] = db.GLFiscalYears.ToList();
            ViewData["JobTypes"] = db.FurnitureJobCategories.ToList();

            ViewData["Periods"] = db.GLPeriods.ToList();
            var quarters = db.FurnitureQuarterlyProductionPlans.ToList();

            ViewData["Quarters"] = quarters.Select(s => new {
                QuarterlyPlanId = s.FurnitureQuarterlyProductionPlanId,

                Name = s.Name,
                NameAm = s.NameAm
            }).ToList();
            var months = db.FurnitureMonthlyProductionPlans.ToList();
            ViewData["MonthlyProductionPlans"] = months.Select(s => new {
                MonthlyProductionPlanId = s.FurnitureMonthlyProductionPlanId,
                Name = getName(s.FurnitureMonthlyProductionPlanId),
                NameAm = getName(s.FurnitureMonthlyProductionPlanId)
            }).ToList();
            return View();
        }

        private string getName(int monthlyPlanId)
        {
            var month = db.FurnitureMonthlyProductionPlans.Find(monthlyPlanId);
            if (month != null)
            {
                var quarter = db.FurnitureQuarterlyProductionPlans.Find(month.QuarterlyProductionPlanId);
                if (quarter != null)
                {
                    var halfYear = db.FurnitureHalfYearlyProductionPlans.Find(quarter.FurnitureHalfYearlyProductionPlanId);
                    if (halfYear != null)
                    {

                        var year = db.FurnitureYearlyProductionPlans.Find(halfYear.YearlyProductionPlanId);
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
                                    return budgetYear.Name + "_" + halfYear.NameAm + "_" + quarter.NameAm + "_" + month.NameAm;

                                }
                                else
                                {
                                    return budgetYear.Name + "_" + halfYear.Name + "_" + quarter.Name + "_" + month.Name;

                                }
                            }
                        }
                    }
                }

            }
            return "";

        }

        public ActionResult WeeklyProductionPlans_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            IQueryable<FurnitureWeeklyProductionPlan> WeeklyProductionPlans = db.FurnitureWeeklyProductionPlans.Where(q => q.FurnitureMonthlyProductionPlanId == id);




            DataSourceResult result = WeeklyProductionPlans.ToDataSourceResult(request, weeklyPlan => new
            {
                MonthlyProductionPlanId = weeklyPlan.FurnitureMonthlyProductionPlanId,
                WeeklyProductionPlanId = weeklyPlan.FurnitureWeeklyProductionPlanId,
                StartDate = weeklyPlan.StartDate,
                EndDate = weeklyPlan.EndDate,
                EndDateEth = XAPI.EthiopicDateTime.GetEthiopicDate(weeklyPlan.EndDate.Value.Day, weeklyPlan.EndDate.Value.Month, weeklyPlan.EndDate.Value.Year),
                StartDateEth = XAPI.EthiopicDateTime.GetEthiopicDate(weeklyPlan.StartDate.Value.Day, weeklyPlan.StartDate.Value.Month, weeklyPlan.StartDate.Value.Year),
                ProductionPlanBirr = weeklyPlan.ProductionPlanBirr,
                ProductionPlanQuantity = weeklyPlan.ProductionPlanQuantity,
                Name = weeklyPlan.Name,
                NameAm = weeklyPlan.NameAm
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }




        public ActionResult WeeklyProductionPlans_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<FurnitureWeeklyProductionPlan> weeklyPlans, int id)
        {

            if (weeklyPlans != null && ModelState.IsValid)
            {
                var monthlyPlan = db.FurnitureMonthlyProductionPlans.Find(id);
                var weekIds = weeklyPlans.ToList().Select(s => s.FurnitureWeeklyProductionPlanId);
                var weeklyPlansNotUpdateds = db.FurnitureWeeklyProductionPlans.Where(q => !weekIds.Contains(q.FurnitureWeeklyProductionPlanId) && q.FurnitureMonthlyProductionPlanId == id).ToList();
                var weeklyPlansNotUpdatedQuas = weeklyPlansNotUpdateds.Sum(s => s.ProductionPlanQuantity);
                var weeklyPlansNotUpdatedBirrs = weeklyPlansNotUpdateds.Sum(s => s.ProductionPlanBirr);
                if ((weeklyPlansNotUpdatedQuas + weeklyPlans.Sum(s => s.ProductionPlanQuantity) > monthlyPlan.ProductionPlanQuantity))
                {
                    ModelState.AddModelError("Error", "Summation of weekly plans in quantity must equals monthly plan!");
                }
                if ((weeklyPlansNotUpdatedBirrs + weeklyPlans.Sum(s => s.ProductionPlanBirr) > monthlyPlan.ProductionPlanBirr))
                {
                    ModelState.AddModelError("Error", "Summation of weekly plans in birr must equals monthly plan!");
                }
                foreach (var weeklyPlan in weeklyPlans)
                {

                    if (weeklyPlan.StartDate < monthlyPlan.StartDate || weeklyPlan.StartDate > monthlyPlan.EndDate)
                    {
                        ModelState.AddModelError("key", "Entered start date between the month period!");

                    }
                    if (weeklyPlan.EndDate > monthlyPlan.EndDate || weeklyPlan.EndDate < monthlyPlan.StartDate)
                    {
                        ModelState.AddModelError("key", "Entered start date between the month period!");

                    }

                    if (ModelState.IsValid)
                    {
                        var entity = db.FurnitureWeeklyProductionPlans.Find(weeklyPlan.FurnitureWeeklyProductionPlanId);
                        if (entity != null)
                        {

                            entity.NameAm = weeklyPlan.NameAm;
                            entity.Name = weeklyPlan.Name;
                            entity.ProductionPlanQuantity = weeklyPlan.ProductionPlanQuantity;
                            entity.ProductionPlanBirr = weeklyPlan.ProductionPlanBirr;
                            entity.StartDate = weeklyPlan.StartDate;
                            entity.StartDateEth = weeklyPlan.StartDateEth;
                            entity.EndDate = weeklyPlan.EndDate;
                            entity.EndDateEth = weeklyPlan.EndDateEth;
                            db.FurnitureWeeklyProductionPlans.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;

                        }
                    }
                }
                db.SaveChanges();




            }

            return Json(new[] { weeklyPlans }.ToDataSourceResult(request, ModelState));





        }




        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
