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
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Setting
{
   // [AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpAdmin)]
    public class ProductionYearlyPlanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYears"] = db.GLFiscalYears.ToList();
            ViewData["GlPeriods"] = db.GLPeriods.ToList();
            ViewData["JobTypes"] = db.JobCategories.ToList();
            return View();
        }

      
        public ActionResult ProductionYearlyPlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductionYearlyPlan> ProductionYearlyPlans = db.ProductionYearlyPlans;
            DataSourceResult result = ProductionYearlyPlans.ToDataSourceResult(request, ProductionYearlyPlan => new {
                ProductionYearlyPlanId = ProductionYearlyPlan.ProductionYearlyPlanId,
                GlFiscalYearId = ProductionYearlyPlan.GlFiscalYearId,
                TotalPrice = ProductionYearlyPlan.TotalPrice,
                TotalProduct = ProductionYearlyPlan.TotalProduct
              
            });

            return Json(result);
        }

        public ActionResult ProductionYearlyPlans_Create([DataSourceRequest]DataSourceRequest request, ProductionYearlyPlan ProductionYearlyPlan)
        {
            var plan = db.ProductionYearlyPlans.Where(q => q.GlFiscalYearId == ProductionYearlyPlan.GlFiscalYearId).FirstOrDefault();
            if(plan!=null)
            {
                ModelState.AddModelError("Error","Fiscal Year Plan Already Existed!");
            }
            if (ModelState.IsValid)
            {
                var entity = (ProductionYearlyPlan)this.GetObject(ProductionYearlyPlan);
                db.ProductionYearlyPlans.Add(entity);
                db.SaveChanges();
                ProductionYearlyPlan.ProductionYearlyPlanId = entity.ProductionYearlyPlanId;
                var periods = db.GLPeriods.Where(q => q.GlFiscalYearId == ProductionYearlyPlan.GlFiscalYearId).ToList();
                CreatMonthlyProductionPlans(periods, ProductionYearlyPlan.ProductionYearlyPlanId);
                db.SaveChanges();

            }

            return Json(new[] { ProductionYearlyPlan }.ToDataSourceResult(request, ModelState));
        }

        private void CreatMonthlyProductionPlans(List<GLPeriod> periods, int yearPlanId)
        {

             foreach(var period in periods)
            {
                var entity = new ProductionMonthlyPlan
                {
                    ProductionYearlyPlanId = yearPlanId,
                    GLPeriodId = period.GLPeriodId,
                    TotalPrice = 0,
                    TotalProduct = 0

                };
                db.ProductionMonthlyPlans.Add(entity);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionYearlyPlans_Update([DataSourceRequest]DataSourceRequest request, ProductionYearlyPlan ProductionYearlyPlan)
        {
            var plan = db.ProductionYearlyPlans.Where(q => q.GlFiscalYearId == ProductionYearlyPlan.GlFiscalYearId
            &&q.ProductionYearlyPlanId!= ProductionYearlyPlan.ProductionYearlyPlanId
            ).FirstOrDefault();
            if (plan != null)
            {
                ModelState.AddModelError("Error", "Fiscal Year Plan Already Existed!");
            }
            if (ModelState.IsValid)
            {
                var entity = (ProductionYearlyPlan)this.GetObject(ProductionYearlyPlan);

                db.ProductionYearlyPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ProductionYearlyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionYearlyPlans_Destroy([DataSourceRequest]DataSourceRequest request, ProductionYearlyPlan ProductionYearlyPlan)
        {

            if (ModelState.IsValid)
            {
                var entity = (ProductionYearlyPlan)this.GetObject(ProductionYearlyPlan);

                db.ProductionYearlyPlans.Attach(entity);
                db.ProductionYearlyPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ProductionYearlyPlan }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(ProductionYearlyPlan ProductionYearlyPlan)
        {
            var entity = new ProductionYearlyPlan
            {
                ProductionYearlyPlanId = ProductionYearlyPlan.ProductionYearlyPlanId,
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
