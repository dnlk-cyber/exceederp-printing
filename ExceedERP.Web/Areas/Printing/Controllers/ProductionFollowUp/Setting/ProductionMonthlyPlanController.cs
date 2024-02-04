﻿using System;
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
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Setting
{
   // [AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpAdmin)]
    public class ProductionMonthlyPlanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYears"] = db.GLFiscalYears.ToList();
            return View();
        }

      
        public ActionResult ProductionMonthlyPlans_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProductionMonthlyPlan> ProductionMonthlyPlans = db.ProductionMonthlyPlans.Where(q=>q.ProductionYearlyPlanId==id);
            DataSourceResult result = ProductionMonthlyPlans.ToDataSourceResult(request, ProductionMonthlyPlan => new {
                ProductionMonthlyPlanId = ProductionMonthlyPlan.ProductionMonthlyPlanId,
                ProductionYearlyPlanId = ProductionMonthlyPlan.ProductionYearlyPlanId,
                GLPeriodId = ProductionMonthlyPlan.GLPeriodId,
                TotalPrice = ProductionMonthlyPlan.TotalPrice,
                TotalProduct = ProductionMonthlyPlan.TotalProduct
              
            });

            return Json(result);
        }

        public ActionResult ProductionMonthlyPlans_Create([DataSourceRequest]DataSourceRequest request, ProductionMonthlyPlan ProductionMonthlyPlan)
        {
            //var plan = db.ProductionMonthlyPlans.Where(q => q.G == ProductionMonthlyPlan.GlFiscalYearId).FirstOrDefault();
            //if(plan!=null)
            //{
            //    ModelState.AddModelError("Error","Fiscal Year Plan Already Existed!");
            //}
            if (ModelState.IsValid)
            {
                var entity = (ProductionMonthlyPlan)this.GetObject(ProductionMonthlyPlan);
                db.ProductionMonthlyPlans.Add(entity);
                db.SaveChanges();
                ProductionMonthlyPlan.ProductionMonthlyPlanId = entity.ProductionMonthlyPlanId;
            }

            return Json(new[] { ProductionMonthlyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionMonthlyPlans_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProductionMonthlyPlan> productionMonthlyPlans,int id)
        {
            var monthlIds = productionMonthlyPlans.ToList().Select(s => s.ProductionMonthlyPlanId);
            var updatedMOnthlyPlan = productionMonthlyPlans.ToList().Sum(s => s.TotalPrice);
            var yearlyPlan = db.ProductionYearlyPlans.Find(id);
            var monthlyPlan = db.ProductionMonthlyPlans.Where(q => !monthlIds.Contains(q.ProductionMonthlyPlanId)).ToList().Sum(s => s.TotalPrice);
            if(yearlyPlan?.TotalPrice< (monthlyPlan+ updatedMOnthlyPlan))
            {
                ModelState.AddModelError("Error","Summation of monthly plans must equals withthe yearly paln!");
            }
            if (ModelState.IsValid)
            {
                foreach (var productionMonthlyPlan in productionMonthlyPlans)
                {
                    var entity = (ProductionMonthlyPlan)this.GetObject(productionMonthlyPlan);

                    db.ProductionMonthlyPlans.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                }
                db.SaveChanges();

            }

            return Json(new[] {productionMonthlyPlans }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionMonthlyPlans_Destroy([DataSourceRequest]DataSourceRequest request, ProductionMonthlyPlan ProductionMonthlyPlan)
        {

            if (ModelState.IsValid)
            {
                var entity = (ProductionMonthlyPlan)this.GetObject(ProductionMonthlyPlan);

                db.ProductionMonthlyPlans.Attach(entity);
                db.ProductionMonthlyPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ProductionMonthlyPlan }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(ProductionMonthlyPlan ProductionMonthlyPlan)
        {
            var entity = new ProductionMonthlyPlan
            {
                ProductionMonthlyPlanId = ProductionMonthlyPlan.ProductionMonthlyPlanId,
                ProductionYearlyPlanId = ProductionMonthlyPlan.ProductionYearlyPlanId,
                GLPeriodId = ProductionMonthlyPlan.GLPeriodId,
                TotalPrice = ProductionMonthlyPlan.TotalPrice,
                TotalProduct = ProductionMonthlyPlan.TotalProduct

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
