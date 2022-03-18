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
using ExceedERP.Core.Domain.ProductionFollowUp;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.ProductionFollowUp.Controllers.Settings
{
   // [AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpAdmin)]
    public class ProductionMonthlyJobTypePlanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYears"] = db.GLFiscalYears.ToList();
            return View();
        }

      
        public ActionResult ProductionMonthlyJobTypePlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductionMonthlyJobTypePlan> ProductionMonthlyJobTypePlans = db.ProductionMonthlyJobTypePlans;
            DataSourceResult result = ProductionMonthlyJobTypePlans.ToDataSourceResult(request, ProductionMonthlyJobTypePlan => new {
                ProductionMonthlyJobTypePlanId = ProductionMonthlyJobTypePlan.ProductionMonthlyJobTypePlanId,
                ProductionMonthlyPlanId = ProductionMonthlyJobTypePlan.ProductionMonthlyPlanId,
                JobTypeId = ProductionMonthlyJobTypePlan.JobTypeId,
                TotalPrice = ProductionMonthlyJobTypePlan.TotalPrice,
                TotalProduct = ProductionMonthlyJobTypePlan.TotalProduct
              
            });

            return Json(result);
        }

        public ActionResult ProductionMonthlyJobTypePlans_Create([DataSourceRequest]DataSourceRequest request, ProductionMonthlyJobTypePlan ProductionMonthlyJobTypePlan)
        {
            //var plan = db.ProductionMonthlyJobTypePlans.Where(q => q.G == ProductionMonthlyJobTypePlan.GlFiscalYearId).FirstOrDefault();
            //if(plan!=null)
            //{
            //    ModelState.AddModelError("Error","Fiscal Year Plan Already Existed!");
            //}
            if (ModelState.IsValid)
            {
                var entity = (ProductionMonthlyJobTypePlan)this.GetObject(ProductionMonthlyJobTypePlan);
                db.ProductionMonthlyJobTypePlans.Add(entity);
                db.SaveChanges();
                ProductionMonthlyJobTypePlan.ProductionMonthlyJobTypePlanId = entity.ProductionMonthlyJobTypePlanId;
            }

            return Json(new[] { ProductionMonthlyJobTypePlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]      
            public ActionResult ProductionMonthlyJobTypePlans_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProductionMonthlyJobTypePlan> productionMonthlyJobTypePlans)
            {
                var jobTypeIds = productionMonthlyJobTypePlans.ToList().Select(s => s.ProductionMonthlyJobTypePlanId);
                var updatedMonthlyJobTypePlan = productionMonthlyJobTypePlans.ToList().Sum(s => s.TotalPrice);
                var monthlyPlan = db.ProductionMonthlyPlans.Where(q => q.ProductionMonthlyPlanId == productionMonthlyJobTypePlans.FirstOrDefault().ProductionMonthlyPlanId).FirstOrDefault();
                var monthlyJobTypePlan = db.ProductionMonthlyPlans.Where(q => !jobTypeIds.Contains(q.ProductionMonthlyPlanId)).ToList().Sum(s => s.TotalPrice);
                if (monthlyPlan?.TotalPrice < (monthlyJobTypePlan + updatedMonthlyJobTypePlan))
                {
                    ModelState.AddModelError("Error", "Summation of monthly job type plans must equals with the monthly paln!");
                }
                if (ModelState.IsValid)
            {
                foreach (var productionMonthlyJobTypePlan in productionMonthlyJobTypePlans)
                {
                    var entity = (ProductionMonthlyJobTypePlan)this.GetObject(productionMonthlyJobTypePlan);

                    db.ProductionMonthlyJobTypePlans.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return Json(new[] { productionMonthlyJobTypePlans }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionMonthlyJobTypePlans_Destroy([DataSourceRequest]DataSourceRequest request, ProductionMonthlyJobTypePlan ProductionMonthlyJobTypePlan)
        {

            if (ModelState.IsValid)
            {
                var entity = (ProductionMonthlyJobTypePlan)this.GetObject(ProductionMonthlyJobTypePlan);

                db.ProductionMonthlyJobTypePlans.Attach(entity);
                db.ProductionMonthlyJobTypePlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ProductionMonthlyJobTypePlan }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(ProductionMonthlyJobTypePlan ProductionMonthlyJobTypePlan)
        {
            var entity = new ProductionMonthlyJobTypePlan
            {
                ProductionMonthlyJobTypePlanId = ProductionMonthlyJobTypePlan.ProductionMonthlyJobTypePlanId,
                ProductionMonthlyPlanId = ProductionMonthlyJobTypePlan.ProductionMonthlyPlanId,
                JobTypeId = ProductionMonthlyJobTypePlan.JobTypeId,
                TotalPrice = ProductionMonthlyJobTypePlan.TotalPrice,
                TotalProduct = ProductionMonthlyJobTypePlan.TotalProduct

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
