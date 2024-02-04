using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Localization;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers;
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Plan
{
    public class HalfYearlyProductionPlansController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["BudgetYears"] = db.GLFiscalYears.ToList();
            ViewData["Periods"] = db.GLPeriods.ToList();
            return View();
        }
        public ActionResult HalfYearlyProductionPlans_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            IQueryable<HalfYearlyProductionPlan> halfYearlyProductionPlans = db.HalfYearlyProductionPlans.Where(q => q.YearlyProductionPlanId == id);

            DataSourceResult result = halfYearlyProductionPlans.ToDataSourceResult(request, HalfYearlyProductionPlan => new
            {
                HalfYearlyProductionPlanId = HalfYearlyProductionPlan.HalfYearlyProductionPlanId,
                YearlyProductionPlanId = HalfYearlyProductionPlan.YearlyProductionPlanId,
                StartDate = HalfYearlyProductionPlan.StartDate,
                StartDateEth = HalfYearlyProductionPlan.StartDateEth,
                EndDateEth = HalfYearlyProductionPlan.EndDateEth,
                EndDate = HalfYearlyProductionPlan.EndDate,
                ProductionPlanBirr = HalfYearlyProductionPlan.ProductionPlanBirr,
                ProductionPlanQuantity = HalfYearlyProductionPlan.ProductionPlanQuantity,
                Name = HalfYearlyProductionPlan.Name,
                NameAm = HalfYearlyProductionPlan.NameAm,

            });

            return Json(result);
        }



        public ActionResult HalfYearlyProductionPlans_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<HalfYearlyProductionPlan> halfYearlyProductionPlans, int id)
        {
            if (halfYearlyProductionPlans != null && ModelState.IsValid)
            {
                var yearlyPlan = db.YearlyProductionPlans.Find(id);
                var halfYearIds = halfYearlyProductionPlans.Select(s => s.HalfYearlyProductionPlanId);
                var halfYearProductionPlanNotUpdatedRecs = db.HalfYearlyProductionPlans.Where(q => q.YearlyProductionPlanId == id && !halfYearIds.Contains(q.HalfYearlyProductionPlanId)).ToList();
                var totHalfYearPlanUpdateds = halfYearlyProductionPlans.Sum(s => s.ProductionPlanBirr);
                var totHalfYearPlanNotUpdateds = halfYearProductionPlanNotUpdatedRecs.Sum(s => s.ProductionPlanBirr);
                if ((totHalfYearPlanUpdateds + totHalfYearPlanNotUpdateds) > yearlyPlan.ProductionPlanBirr)
                {
                    ModelState.AddModelError("Error", "Summation of half year plan in birr must equal with yearly plan");
                }
                totHalfYearPlanUpdateds = halfYearlyProductionPlans.Sum(s => s.ProductionPlanQuantity);
                totHalfYearPlanNotUpdateds = halfYearProductionPlanNotUpdatedRecs.Sum(s => s.ProductionPlanQuantity);
                if ((totHalfYearPlanUpdateds + totHalfYearPlanNotUpdateds) > yearlyPlan.ProductionPlanQuantity)
                {
                    ModelState.AddModelError("Error", "Summation of half year plan in quantity must equal with yearly plan");
                }
                if (ModelState.IsValid)
                {
                    foreach (var halfYearlyProductionPlan in halfYearlyProductionPlans)
                    {

                        var entity = db.HalfYearlyProductionPlans.Find(halfYearlyProductionPlan.HalfYearlyProductionPlanId);
                        if (entity != null)
                        {
                            entity.HalfYearlyProductionPlanId = halfYearlyProductionPlan.HalfYearlyProductionPlanId;

                            entity.ProductionPlanBirr = halfYearlyProductionPlan.ProductionPlanBirr;
                            entity.ProductionPlanQuantity = halfYearlyProductionPlan.ProductionPlanQuantity;
                            db.HalfYearlyProductionPlans.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();

                }


            }

            return Json(new[] { halfYearlyProductionPlans }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
