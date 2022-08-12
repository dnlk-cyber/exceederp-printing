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
using TradeMarketSystem.Core.Localization;
using TradeMarketSystem.Core.Model.Common;
using TradeMarketSystem.Core.Model.CustomerService;
using TradeMarketSystem.Core.Model.Daily_Market;
using TradeMarketSystem.DataAccess.Context;
using TradeMarketSystem.Helper.Common;
using TradeMarketSystem.Helpers;

namespace TradeMarketSystem.Areas.DailyMarket.Controllers.Setting
{
    public class HalfYearlyPlanController : BaseController
    {
        private TradeDbContext db = new TradeDbContext();

        public ActionResult Index()
        {
            ViewData["BudgetYears"] = db.BudgetYears.ToList();
            ViewData["Periods"] = db.Period.ToList();
            return View();
        }
        [AuthorizeRoles(DailyMarketRoles.DMPlanApprover, DailyMarketRoles.DMPlanner)]
        public ActionResult HalfYearlyPlans_Read([DataSourceRequest] DataSourceRequest request,int id)
        {
            IQueryable<HalfYearlyPlan> MarketPriceEntryPlans = db.HalfYearlyPlans.Where(q=>q.YearlyPlanId==id);

            var dailyPrices = db.DailyPriceEntries.ToList();
            DataSourceResult result = MarketPriceEntryPlans.ToDataSourceResult(request, HalfYearlyPlan => new
            {
                HalfYearlyPlanId = HalfYearlyPlan.HalfYearlyPlanId,
                YearlyPlanId = HalfYearlyPlan.YearlyPlanId,
                IsApproved = HalfYearlyPlan.IsApproved,
                ApprovedBy = HalfYearlyPlan.ApprovedBy,
                StartDate = HalfYearlyPlan.StartDate,
                StartDateEth = HalfYearlyPlan.StartDateEth,
                EndDateEth = HalfYearlyPlan.EndDateEth,
                EndDate = HalfYearlyPlan.EndDate,
                Plan = HalfYearlyPlan.Plan,
                Name = HalfYearlyPlan.Name,
                NameAm = HalfYearlyPlan.NameAm,
                Actual = HalfYearlyPlan.Actual,
                Performance = HalfYearlyPlan.Performance,
                User = HalfYearlyPlan.User
            });

            return Json(result);
        }



        public ActionResult HalfYearlyPlans_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<HalfYearlyPlan> halfYearlyPlans)
        {
            if (halfYearlyPlans != null && ModelState.IsValid)
            {
                foreach (var halfYearlyPlan in halfYearlyPlans)
                {
                    var yearlyPlan = db.YearlyPlans.Find(halfYearlyPlan.YearlyPlanId);
                    var totHalfYearPlans = db.HalfYearlyPlans.Where(q => q.YearlyPlanId == halfYearlyPlan.YearlyPlanId&&q.HalfYearlyPlanId!= halfYearlyPlan.HalfYearlyPlanId).Sum(s => s.Plan);
                    if((totHalfYearPlans+ halfYearlyPlan.Plan) > yearlyPlan.Plan)
                    {
                        ModelState.AddModelError("Error",Resource.DailyMarket_HalfYearValidation);
                    }
                    if (ModelState.IsValid)
                    {
                        var entity = db.HalfYearlyPlans.Find(halfYearlyPlan.HalfYearlyPlanId);
                        if (entity != null)
                        {
                            entity.HalfYearlyPlanId = halfYearlyPlan.HalfYearlyPlanId;
                           
                            entity.Plan = halfYearlyPlan.Plan;
                            db.HalfYearlyPlans.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                    #region Update log
                    UserHelper.OperationLog(Request.UserHostAddress, "Update", "Success", User.Identity.Name, "Updated LED Display Commodity category Setting with Id: " + halfYearlyPlans.Select(s=>s.HalfYearlyPlanId).ToString(), Core.Model.Global.GlobalEnum.TradeGroup.DailyMarket);
                    #endregion
                }


            }

            return Json(new[] { halfYearlyPlans }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
