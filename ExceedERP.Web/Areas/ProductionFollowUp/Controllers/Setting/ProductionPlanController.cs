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
    public class ProductionPlanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYears"] = db.GLFiscalYears.ToList();
            return View();
        }

      
        public ActionResult ProductionPlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductionPlan> ProductionPlans = db.ProductionPlans;
            DataSourceResult result = ProductionPlans.ToDataSourceResult(request, ProductionPlan => new {
                ProductionPlanId = ProductionPlan.ProductionPlanId,
                GlFiscalYearId = ProductionPlan.GlFiscalYearId,
                TotalPrice = ProductionPlan.TotalPrice,
                TotalProduct = ProductionPlan.TotalProduct
              
            });

            return Json(result);
        }

        public ActionResult ProductionPlans_Create([DataSourceRequest]DataSourceRequest request, ProductionPlan ProductionPlan)
        {
            var plan = db.ProductionPlans.Where(q => q.GlFiscalYearId == ProductionPlan.GlFiscalYearId).FirstOrDefault();
            if(plan!=null)
            {
                ModelState.AddModelError("Error","Fiscal Year Plan Already Existed!");
            }
            if (ModelState.IsValid)
            {
                var entity = (ProductionPlan)this.GetObject(ProductionPlan);
                db.ProductionPlans.Add(entity);
                db.SaveChanges();
                ProductionPlan.ProductionPlanId = entity.ProductionPlanId;
            }

            return Json(new[] { ProductionPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionPlans_Update([DataSourceRequest]DataSourceRequest request, ProductionPlan ProductionPlan)
        {
            var plan = db.ProductionPlans.Where(q => q.GlFiscalYearId == ProductionPlan.GlFiscalYearId
            &&q.ProductionPlanId!= ProductionPlan.ProductionPlanId
            ).FirstOrDefault();
            if (plan != null)
            {
                ModelState.AddModelError("Error", "Fiscal Year Plan Already Existed!");
            }
            if (ModelState.IsValid)
            {
                var entity = (ProductionPlan)this.GetObject(ProductionPlan);

                db.ProductionPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ProductionPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionPlans_Destroy([DataSourceRequest]DataSourceRequest request, ProductionPlan ProductionPlan)
        {

            if (ModelState.IsValid)
            {
                var entity = (ProductionPlan)this.GetObject(ProductionPlan);

                db.ProductionPlans.Attach(entity);
                db.ProductionPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ProductionPlan }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(ProductionPlan ProductionPlan)
        {
            var entity = new ProductionPlan
            {
                ProductionPlanId = ProductionPlan.ProductionPlanId,
                GlFiscalYearId = ProductionPlan.GlFiscalYearId,
                TotalPrice = ProductionPlan.TotalPrice,
                TotalProduct = ProductionPlan.TotalProduct

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
