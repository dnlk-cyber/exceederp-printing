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
using TradeMarketSystem.Controllers;
using TradeMarketSystem.Core.Model.Common;
using TradeMarketSystem.Core.Localization;
using TradeMarketSystem.Core.Model.Daily_Market;
using TradeMarketSystem.DataAccess.Context;
using TradeMarketSystem.Helper.Common;
using TradeMarketSystem.Helpers;

namespace TradeMarketSystem.Areas.DailyMarket.Controllers.Setting
{
    public class MonthlyPlanController : BaseController
    {
        private TradeDbContext db = new TradeDbContext();

        public ActionResult Index()
        {
            ViewData["BudgetYears"] = db.BudgetYears.ToList();
            ViewData["Periods"] = db.Period.ToList();
            ViewData["QuarterlyPlans"] = db.QuarterlyPlans.ToList();
            var quarters = db.QuarterlyPlans.ToList();
            ViewData["Quarters"] = quarters.Select(s => new {
                QuarterlyPlanId = s.QuarterlyPlanId,

                Name = getName(s.QuarterlyPlanId),
                NameAm = getName(s.QuarterlyPlanId)
            }).ToList();

            return View();
        }
        private string getName(int quarterPlanId)
        {
            
                var quarter = db.QuarterlyPlans.Find(quarterPlanId);
                if (quarter != null)
                {
                    var halfYear = db.HalfYearlyPlans.Find(quarter.HalfYearlyPlanId);
                    if (halfYear != null)
                    {

                        var year = db.YearlyPlans.Find(halfYear.YearlyPlanId);
                        if (year != null)
                        {
                            var budgetYear = db.BudgetYears.Find(year.BudgetYearId);
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
                                    return budgetYear.Year + "_" + quarter.NameAm + "___" + Resource.Plan + ":" + quarter.Plan;

                                }
                                else
                                {
                                    return budgetYear.Year + "_" + quarter.Name + "___" + Resource.Plan + ":" + quarter.Plan;

                                }
                            }
                        }
                    }
                }

            return "";

        }

        [AuthorizeRoles(DailyMarketRoles.DMPlanApprover, DailyMarketRoles.DMPriceEntry, DailyMarketRoles.DMPlanner)]
        public ActionResult MonthlyPlans_Read([DataSourceRequest] DataSourceRequest request, int? id)
        {
            var assignedWeredas = User.Identity.GetMappedWeredas().Select(s => s.WeredaId);

            IQueryable<MonthlyPlan> MarketPriceEntryPlans = db.MonthlyPlans.Where(q => assignedWeredas.Contains((int)q.QuarterlyPlan.HalfYearlyPlan.YearlyPlan.WeredaId));
            if (id != null)
            {
                var halfYearPlanIds = db.HalfYearlyPlans.Where(q => q.YearlyPlanId == id).ToList().Select(s=>s.HalfYearlyPlanId);
                var quarterPlanIds = db.QuarterlyPlans.Where(q => halfYearPlanIds.Contains(q.HalfYearlyPlanId)).ToList().Select(s=>s.QuarterlyPlanId);
                MarketPriceEntryPlans = db.MonthlyPlans.Where(q => quarterPlanIds.Contains(q.QuarterlyPlanId));

            }
            string username = User.Identity.Name;
            var notifications = db.Notifications.Where(x => x.SentTo == username && x.NotificationType == NotifType.DailyMarketPlan.ToString() && !x.IsRead).ToList();
            foreach (var notif in notifications)
            {
                notif.IsRead = true;

                db.Entry(notif).State = EntityState.Modified;
                db.SaveChanges();
            }
            DataSourceResult result = MarketPriceEntryPlans.ToDataSourceResult(request, MonthlyPlan => new
            {
                MonthlyPlanId = MonthlyPlan.MonthlyPlanId,
                QuarterlyPlanId = MonthlyPlan.QuarterlyPlanId,
                StartDate = MonthlyPlan.StartDate,
                EndDate = MonthlyPlan.EndDate,
                StartDateEth = MonthlyPlan.StartDateEth,
                EndDateEth = MonthlyPlan.EndDateEth,
                Plan = MonthlyPlan.Plan,
                Name = MonthlyPlan.Name,
                NameAm = MonthlyPlan.NameAm,
                Actual = MonthlyPlan.Actual,
                Performance = MonthlyPlan.Performance,
                IsTransferred = MonthlyPlan.IsTransferred,
                IsWeeklyApproved = getWeeklyApproalStatus(0, MonthlyPlan.MonthlyPlanId),
                IsWeeklyTransferred = getWeeklyApproalStatus(1, MonthlyPlan.MonthlyPlanId),
                IsWeeklyRejected = getWeeklyApproalStatus(2, MonthlyPlan.MonthlyPlanId),
                IsApproved = getPlanApprovalStatus(MonthlyPlan.QuarterlyPlanId),
                User = MonthlyPlan.User
            });

            return Json(result,JsonRequestBehavior.AllowGet);
        }


        public ActionResult MonthlyPlanUsers_Read([DataSourceRequest] DataSourceRequest request, int? id)
        {
            var assignedWeredas = User.Identity.GetMappedWeredas().Select(s => s.WeredaId);

            IQueryable<MonthlyPlan> MarketPriceEntryPlans = db.MonthlyPlans.Where(q=> assignedWeredas.Contains((int)q.QuarterlyPlan.HalfYearlyPlan.YearlyPlan.WeredaId));
            var yearlyPlanIds = db.YearlyPlans.Where(q => q.IsApproved).Select(q => q.YearlyPlanId);
            var halfYearIds = db.HalfYearlyPlans.Where(q => yearlyPlanIds.Contains(q.YearlyPlanId)).Select(q => q.HalfYearlyPlanId);
            var quareterPlanIds = db.QuarterlyPlans.Where(q => halfYearIds.Contains(q.HalfYearlyPlanId)).Select(q => q.QuarterlyPlanId);

            MarketPriceEntryPlans = MarketPriceEntryPlans.Where(q => quareterPlanIds.Contains(q.QuarterlyPlanId));
            DataSourceResult result = MarketPriceEntryPlans.ToDataSourceResult(request, MonthlyPlan => new
            {
                MonthlyPlanId = MonthlyPlan.MonthlyPlanId,
                QuarterlyPlanId = MonthlyPlan.QuarterlyPlanId,
                StartDate = MonthlyPlan.StartDate,
                EndDate = MonthlyPlan.EndDate,
                StartDateEth = MonthlyPlan.StartDateEth,
                EndDateEth = MonthlyPlan.EndDateEth,
                Plan = MonthlyPlan.Plan,
                Name = MonthlyPlan.Name,
                NameAm = MonthlyPlan.NameAm,
                Actual = MonthlyPlan.Actual,
                Performance = MonthlyPlan.Performance,
                IsTransferred = MonthlyPlan.IsTransferred,
                IsWeeklyApproved = getWeeklyApproalStatus(0, MonthlyPlan.MonthlyPlanId),
                IsWeeklyTransferred = getWeeklyApproalStatus(1, MonthlyPlan.MonthlyPlanId),
                IsWeeklyRejected = getWeeklyApproalStatus(2, MonthlyPlan.MonthlyPlanId),
                IsApproved = getPlanApprovalStatus(MonthlyPlan.QuarterlyPlanId),
                User = MonthlyPlan.User
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
        private bool getPlanApprovalStatus(int quarterId)
        {
            var quarter = db.QuarterlyPlans.Find(quarterId);
            if(quarter!=null)
            {
                var halfYear = db.HalfYearlyPlans.Find(quarter.HalfYearlyPlanId);
                if (halfYear != null)
                {
                    var IsApproved = (bool)db.YearlyPlans.Find(halfYear.YearlyPlanId)?.IsApproved;
                    return IsApproved;

                }
            }

            return false;
                }

        private bool getWeeklyApproalStatus(int stat, int monthId)
        {
            bool appStatus = false;
            if (stat==0)
            {
                var Status = db.WeeklyPlanValidations.Where(q => q.User == User.Identity.Name&&q.MonthlyPlanId==monthId).FirstOrDefault();
                if(Status != null)
                {
                    appStatus = Status.IsWeeklyApproved;
                }

            }
            if (stat == 1)
            {
                var Status = db.WeeklyPlanValidations.Where(q => q.User == User.Identity.Name && q.MonthlyPlanId == monthId).FirstOrDefault();
                if (Status != null)
                {
                    appStatus = Status.IsWeeklyTransferred;
                }
            }
            if (stat == 2)
            {
                var Status = db.WeeklyPlanValidations.Where(q => q.User == User.Identity.Name && q.MonthlyPlanId == monthId).FirstOrDefault();
                if (Status != null)
                {
                    appStatus = Status.IsWeeklyRejected;
                }
            }

            return appStatus;
        }

        public ActionResult MonthlyPlans_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<MonthlyPlan> monthlyPlans)
        {
            if (monthlyPlans != null && ModelState.IsValid)
               
            {
                var monthlyGroupByQuarters = monthlyPlans.GroupBy(g => g.QuarterlyPlanId)
                   .Select(s => new
                   {
                       QuarterlyPlanId = s.Key,
                       Plan = s.Sum(su => su.Plan)


                   });
                foreach(var monthlyPlan in monthlyGroupByQuarters)
                {
                    var monthIds = monthlyPlans.Select(s=>s.MonthlyPlanId);
                    var quarterPlan = db.QuarterlyPlans.Find(monthlyPlan.QuarterlyPlanId);
                    var totMonthPlans = db.MonthlyPlans.Where(q => q.QuarterlyPlanId == monthlyPlan.QuarterlyPlanId && !(monthIds.Contains(q.MonthlyPlanId))).ToList().Sum(s => s.Plan);
                //    var totMonthPlans = db.MonthlyPlans.Where(q => q.QuarterlyPlanId == monthlyPlan.QuarterlyPlanId && monthIds.Contains(q.MonthlyPlanId)).Sum(s => s.Plan);
                    if ((totMonthPlans + monthlyPlan.Plan) > quarterPlan.Plan)
                    {
                        ModelState.AddModelError("Error", "Summation of month plans must equals quarter year plan!");
                    }
                }
                
                if (ModelState.IsValid)
                {
                    foreach (var monthlyPlan in monthlyPlans)
                {
                    
                   
                        var entity = db.MonthlyPlans.Find(monthlyPlan.MonthlyPlanId);
                        if (entity != null)
                        {
                         
                            entity.Plan = monthlyPlan.Plan;
                            db.MonthlyPlans.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                    #region Update log
                    UserHelper.OperationLog(Request.UserHostAddress, "Update", "Success", User.Identity.Name, "Updated LED Display Commodity category Setting with Id: " + monthlyPlans.Select(s => s.MonthlyPlanId).ToString(), Core.Model.Global.GlobalEnum.TradeGroup.DailyMarket);
                    #endregion
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
