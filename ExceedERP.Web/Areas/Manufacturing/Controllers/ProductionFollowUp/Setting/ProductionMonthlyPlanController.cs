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
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Setting

{
    // [AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpAdmin)]
    public class FurnitureProductionMonthlyPlanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYears"] = db.GLFiscalYears.ToList();
            return View();
        }

      
        public ActionResult ProductionMonthlyPlans_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureProductionMonthlyPlan> ProductionMonthlyPlans = db.FurnitureProductionMonthlyPlans.Where(q=>q.FurnitureProductionYearlyPlanId == id);
            DataSourceResult result = ProductionMonthlyPlans.ToDataSourceResult(request, ProductionMonthlyPlan => new {
                ProductionMonthlyPlanId = ProductionMonthlyPlan.FurnitureProductionMonthlyPlanId,
                ProductionYearlyPlanId = ProductionMonthlyPlan.FurnitureProductionYearlyPlanId,
                GLPeriodId = ProductionMonthlyPlan.GLPeriodId,
                TotalPrice = ProductionMonthlyPlan.TotalPrice,
                TotalProduct = ProductionMonthlyPlan.TotalProduct
              
            });

            return Json(result);
        }

        public ActionResult ProductionMonthlyPlans_Create([DataSourceRequest]DataSourceRequest request, FurnitureProductionMonthlyPlan ProductionMonthlyPlan)
        {
            //var plan = db.ProductionMonthlyPlans.Where(q => q.G == ProductionMonthlyPlan.GlFiscalYearId).FirstOrDefault();
            //if(plan!=null)
            //{
            //    ModelState.AddModelError("Error","Fiscal Year Plan Already Existed!");
            //}
            if (ModelState.IsValid)
            {
                var entity = (FurnitureProductionMonthlyPlan)this.GetObject(ProductionMonthlyPlan);
                db.FurnitureProductionMonthlyPlans.Add(entity);
                db.SaveChanges();
                ProductionMonthlyPlan.FurnitureProductionMonthlyPlanId = entity.FurnitureProductionMonthlyPlanId;
            }

            return Json(new[] { ProductionMonthlyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionMonthlyPlans_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<FurnitureProductionMonthlyPlan> productionMonthlyPlans,int id)
        {
            var monthlIds = productionMonthlyPlans.ToList().Select(s => s.FurnitureProductionMonthlyPlanId);
            var updatedMOnthlyPlan = productionMonthlyPlans.ToList().Sum(s => s.TotalPrice);
            var yearlyPlan = db.FurnitureProductionYearlyPlans.Find(id);
            var monthlyPlan = db.FurnitureProductionMonthlyPlans.Where(q => !monthlIds.Contains(q.FurnitureProductionMonthlyPlanId)).ToList().Sum(s => s.TotalPrice);
            if(yearlyPlan?.TotalPrice< (monthlyPlan+ updatedMOnthlyPlan))
            {
                ModelState.AddModelError("Error","Summation of monthly plans must equals withthe yearly paln!");
            }
            if (ModelState.IsValid)
            {
                foreach (var productionMonthlyPlan in productionMonthlyPlans)
                {
                    var entity = (FurnitureProductionMonthlyPlan)this.GetObject(productionMonthlyPlan);

                    db.FurnitureProductionMonthlyPlans.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                }
                db.SaveChanges();

            }

            return Json(new[] {productionMonthlyPlans }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionMonthlyPlans_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureProductionMonthlyPlan ProductionMonthlyPlan)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureProductionMonthlyPlan)this.GetObject(ProductionMonthlyPlan);

                db.FurnitureProductionMonthlyPlans.Attach(entity);
                db.FurnitureProductionMonthlyPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ProductionMonthlyPlan }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureProductionMonthlyPlan ProductionMonthlyPlan)
        {
            var entity = new FurnitureProductionMonthlyPlan
            {
                FurnitureProductionMonthlyPlanId = ProductionMonthlyPlan.FurnitureProductionMonthlyPlanId,
                FurnitureProductionYearlyPlanId = ProductionMonthlyPlan.FurnitureProductionYearlyPlanId,
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
