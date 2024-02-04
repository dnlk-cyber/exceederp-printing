
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Plan
{
    public class QuarterlyProductionPlansController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: ProductionFollowUp/QuarterlyProductionPlans
        public ActionResult Index()
        {
            ViewData["BudgetYears"] = db.GLFiscalYears.ToList();

            ViewData["Periods"] = db.GLPeriods.ToList();
            return View();
        }
        public ActionResult QuarterlyProductionPlans_Read([DataSourceRequest] DataSourceRequest request, int? id)
        {

            IQueryable<QuarterlyProductionPlan> MarketPriceEntryPlans = db.QuarterlyProductionPlans;
            if (id != null)
            {
                MarketPriceEntryPlans = MarketPriceEntryPlans.Where(q => q.HalfYearlyProductionPlanId == id);
            }


            DataSourceResult result = MarketPriceEntryPlans.ToDataSourceResult(request, QuarterlyProductionPlan => new
            {
                QuarterlyProductionPlanId = QuarterlyProductionPlan.QuarterlyProductionPlanId,
                HalfYearlyProductionPlanId = QuarterlyProductionPlan.HalfYearlyProductionPlanId,
                StartDate = QuarterlyProductionPlan.StartDate,
                StartDateEth = QuarterlyProductionPlan.StartDateEth,
                EndDate = QuarterlyProductionPlan.EndDate,
                EndDateEth = QuarterlyProductionPlan.EndDateEth,
                ProductionPlanQuantity = QuarterlyProductionPlan.ProductionPlanQuantity,
                ProductionPlanBirr = QuarterlyProductionPlan.ProductionPlanBirr,
                Name = QuarterlyProductionPlan.Name,
                NameAm = QuarterlyProductionPlan.NameAm,

            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QuarterlyProductionPlans_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<QuarterlyProductionPlan> quarterlyPlans, int id)
        {
            if (quarterlyPlans != null && ModelState.IsValid)
            {
                var halfYearlyPlan = db.HalfYearlyProductionPlans.Find(id);
                var quarterYearIds = quarterlyPlans.Select(s => s.QuarterlyProductionPlanId);
                var qaurterYearProductionPlanNotUpdatedRecs = db.QuarterlyProductionPlans.Where(q => q.HalfYearlyProductionPlanId == id && !quarterYearIds.Contains(q.QuarterlyProductionPlanId)).ToList();
                var totQuarterYearPlanUpdateds = quarterlyPlans.Sum(s => s.ProductionPlanBirr);
                var totQuarterYearPlanNotUpdateds = qaurterYearProductionPlanNotUpdatedRecs.Sum(s => s.ProductionPlanBirr);
                if ((totQuarterYearPlanUpdateds + totQuarterYearPlanNotUpdateds) > halfYearlyPlan.ProductionPlanBirr)
                {
                    ModelState.AddModelError("Error", "Summation of quarter plans in birr must equal with half yearly plan");
                }
                totQuarterYearPlanUpdateds = quarterlyPlans.Sum(s => s.ProductionPlanQuantity);
                totQuarterYearPlanNotUpdateds = qaurterYearProductionPlanNotUpdatedRecs.Sum(s => s.ProductionPlanQuantity);
                if ((totQuarterYearPlanUpdateds + totQuarterYearPlanNotUpdateds) > halfYearlyPlan.ProductionPlanQuantity)
                {
                    ModelState.AddModelError("Error", "Summation of quarter plans in quantity must equal with half yearly plan");
                }
                if (ModelState.IsValid)
                {
                    foreach (var quarterlyPlan in quarterlyPlans)
                    {
                        var entity = db.QuarterlyProductionPlans.Find(quarterlyPlan.QuarterlyProductionPlanId);
                        if (entity != null)
                        {

                            entity.ProductionPlanBirr = quarterlyPlan.ProductionPlanBirr;
                            entity.ProductionPlanQuantity = quarterlyPlan.ProductionPlanQuantity;
                            db.QuarterlyProductionPlans.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();

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