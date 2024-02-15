using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Setting

{
    // [AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpAdmin)]
    public class FurnitureProductionYearlyPlanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYears"] = db.GLFiscalYears.ToList();
            ViewData["GlPeriods"] = db.GLPeriods.ToList();
            ViewData["JobTypes"] = db.FurnitureJobCategories.ToList();
            return View();
        }

      
        public ActionResult ProductionYearlyPlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureProductionYearlyPlan> ProductionYearlyPlans = db.FurnitureProductionYearlyPlans;
            DataSourceResult result = ProductionYearlyPlans.ToDataSourceResult(request, ProductionYearlyPlan => new {
                ProductionYearlyPlanId = ProductionYearlyPlan.FurnitureProductionYearlyPlanId,
                GlFiscalYearId = ProductionYearlyPlan.GlFiscalYearId,
                TotalPrice = ProductionYearlyPlan.TotalPrice,
                TotalProduct = ProductionYearlyPlan.TotalProduct
              
            });

            return Json(result);
        }

        public ActionResult ProductionYearlyPlans_Create([DataSourceRequest]DataSourceRequest request, FurnitureProductionYearlyPlan ProductionYearlyPlan)
        {
            var plan = db.ProductionYearlyPlans.Where(q => q.GlFiscalYearId == ProductionYearlyPlan.GlFiscalYearId).FirstOrDefault();
            if(plan!=null)
            {
                ModelState.AddModelError("Error","Fiscal Year Plan Already Existed!");
            }
            if (ModelState.IsValid)
            {
                var entity = (FurnitureProductionYearlyPlan)this.GetObject(ProductionYearlyPlan);
                db.FurnitureProductionYearlyPlans.Add(entity);
                db.SaveChanges();
                ProductionYearlyPlan.FurnitureProductionYearlyPlanId = entity.FurnitureProductionYearlyPlanId;
                var periods = db.GLPeriods.Where(q => q.GlFiscalYearId == ProductionYearlyPlan.GlFiscalYearId).ToList();
                CreatMonthlyProductionPlans(periods, ProductionYearlyPlan.FurnitureProductionYearlyPlanId);
                db.SaveChanges();

            }

            return Json(new[] { ProductionYearlyPlan }.ToDataSourceResult(request, ModelState));
        }

        private void CreatMonthlyProductionPlans(List<GLPeriod> periods, int yearPlanId)
        {

             foreach(var period in periods)
            {
                var entity = new FurnitureProductionMonthlyPlan
                {
                    FurnitureProductionYearlyPlanId = yearPlanId,
                    GLPeriodId = period.GLPeriodId,
                    TotalPrice = 0,
                    TotalProduct = 0

                };
                db.FurnitureProductionMonthlyPlans.Add(entity);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionYearlyPlans_Update([DataSourceRequest]DataSourceRequest request, FurnitureProductionYearlyPlan ProductionYearlyPlan)
        {
            var plan = db.FurnitureProductionYearlyPlans.Where(q => q.GlFiscalYearId == ProductionYearlyPlan.GlFiscalYearId
            &&q.FurnitureProductionYearlyPlanId != ProductionYearlyPlan.FurnitureProductionYearlyPlanId
            ).FirstOrDefault();
            if (plan != null)
            {
                ModelState.AddModelError("Error", "Fiscal Year Plan Already Existed!");
            }
            if (ModelState.IsValid)
            {
                var entity = (FurnitureProductionYearlyPlan)this.GetObject(ProductionYearlyPlan);

                db.FurnitureProductionYearlyPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ProductionYearlyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionYearlyPlans_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureProductionYearlyPlan ProductionYearlyPlan)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureProductionYearlyPlan)this.GetObject(ProductionYearlyPlan);

                db.FurnitureProductionYearlyPlans.Attach(entity);
                db.FurnitureProductionYearlyPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ProductionYearlyPlan }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureProductionYearlyPlan ProductionYearlyPlan)
        {
            var entity = new FurnitureProductionYearlyPlan
            {
                FurnitureProductionYearlyPlanId = ProductionYearlyPlan.FurnitureProductionYearlyPlanId,
                GlFiscalYearId = ProductionYearlyPlan.GlFiscalYearId,
                TotalPrice = ProductionYearlyPlan.TotalPrice,
                TotalProduct = ProductionYearlyPlan.TotalProduct

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
