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
    public class QuarterlyPlanController : BaseController
    {
        private TradeDbContext db = new TradeDbContext();

        public ActionResult Index()
        {
            ViewData["BudgetYears"] = db.BudgetYears.ToList();
            
            ViewData["Periods"] = db.Period.ToList();
            return View();
        }
        [AuthorizeRoles(DailyMarketRoles.DMPlanApprover, DailyMarketRoles.DMPlanner)]
        public ActionResult QuarterlyPlans_Read([DataSourceRequest] DataSourceRequest request, int? id)
        {

            IQueryable<QuarterlyPlan> MarketPriceEntryPlans = db.QuarterlyPlans;
            if(id!=null)
            {
                MarketPriceEntryPlans = MarketPriceEntryPlans.Where(q => q.HalfYearlyPlanId == id);
            }

            
            DataSourceResult result = MarketPriceEntryPlans.ToDataSourceResult(request, QuarterlyPlan => new
            {
                QuarterlyPlanId = QuarterlyPlan.QuarterlyPlanId,
                HalfYearlyPlanId = QuarterlyPlan.HalfYearlyPlanId,
                StartDate = QuarterlyPlan.StartDate,
                StartDateEth = QuarterlyPlan.StartDateEth,
                EndDate = QuarterlyPlan.EndDate,
                EndDateEth = QuarterlyPlan.EndDateEth,
                Plan = QuarterlyPlan.Plan,
                Name = QuarterlyPlan.Name,
                NameAm = QuarterlyPlan.NameAm,
                Actual = QuarterlyPlan.Actual,
                Performance = QuarterlyPlan.Performance,
                IsApproved = db.YearlyPlans.Find(db.HalfYearlyPlans.Find(QuarterlyPlan.HalfYearlyPlanId).YearlyPlanId).IsApproved,
                IsTransferred = db.YearlyPlans.Find(db.HalfYearlyPlans.Find(QuarterlyPlan.HalfYearlyPlanId).YearlyPlanId).IsTransferred,
                ApprovedBy = QuarterlyPlan.ApprovedBy,
                User = QuarterlyPlan.User
            });

            return Json(result,JsonRequestBehavior.AllowGet);
        }

        public ActionResult QuarterlyPlans_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<QuarterlyPlan> quarterlyPlans)
        {
            if (quarterlyPlans != null && ModelState.IsValid)
            {
                foreach (var quarterlyPlan in quarterlyPlans)
                {
                    var halfYearPlan = db.HalfYearlyPlans.Find(quarterlyPlan.HalfYearlyPlanId);
                    var totQuarterPlans = db.QuarterlyPlans.Where(q => q.HalfYearlyPlanId == quarterlyPlan.HalfYearlyPlanId && q.QuarterlyPlanId != quarterlyPlan.QuarterlyPlanId).Sum(s => s.Plan);
                    if ((totQuarterPlans + quarterlyPlan.Plan) > halfYearPlan.Plan)
                    {
                        ModelState.AddModelError(Resource.AlertWindow_ErrorTitle, Resource.DailyMarket_QuarterYearPlanValidation);
                    }
                    if (ModelState.IsValid)
                    {
                        var entity = db.QuarterlyPlans.Find(quarterlyPlan.QuarterlyPlanId);
                        if (entity != null)
                        {
                            
                            entity.Plan = quarterlyPlan.Plan;
                            db.QuarterlyPlans.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                    #region Update log
                    UserHelper.OperationLog(Request.UserHostAddress, "Update", "Success", User.Identity.Name, "Updated LED Display Commodity category Setting with Id: " + quarterlyPlans.Select(s => s.HalfYearlyPlanId).ToString(), Core.Model.Global.GlobalEnum.TradeGroup.DailyMarket);
                    #endregion
                }


            }

            return Json(new[] { quarterlyPlans }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
