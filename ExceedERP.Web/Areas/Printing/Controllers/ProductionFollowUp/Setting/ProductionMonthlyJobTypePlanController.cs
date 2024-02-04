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
    public class ProductionMonthlyJobTypePlanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYears"] = db.GLFiscalYears.ToList();
            return View();
        }

      
        public ActionResult ProductionMonthlyJobTypePlans_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            var jobTypes = db.JobCategories.Where(q => !q.HaveParent).ToList();
            IList<ProductionMonthlyJobTypePlan> jobTypePlans = new List<ProductionMonthlyJobTypePlan>();
            foreach (var jobType in jobTypes)
            {
                ProductionMonthlyJobTypePlan jobTypePlan = new ProductionMonthlyJobTypePlan();
                var jobTypePlann = db.ProductionMonthlyJobTypePlans.Where(q => q.ProductionMonthlyPlanId == id
                                         && q.JobTypeId == jobType.JobCategoryId).FirstOrDefault();
                int jobTypePlanId = 0;
                decimal jobTypeTotalPricePlan = 0;
                decimal jobTypeTotalProductPlan = 0;
                if (jobTypePlann!=null)
                {
                     jobTypePlanId = (int)(jobTypePlann?.ProductionMonthlyJobTypePlanId);

                     jobTypeTotalPricePlan = (decimal)jobTypePlann?.TotalPrice;
                     jobTypeTotalProductPlan = (decimal)jobTypePlann?.TotalProduct;
                    jobTypePlan.ProductionMonthlyJobTypePlanId = jobTypePlanId;
                }
                
                jobTypePlan.JobTypeId = jobType.JobCategoryId;
                jobTypePlan.TotalProduct = jobTypeTotalProductPlan;
                jobTypePlan.TotalPrice = jobTypeTotalPricePlan;

                jobTypePlans.Add(jobTypePlan);

            }
            DataSourceResult result = jobTypePlans.ToDataSourceResult(request, ProductionMonthlyJobTypePlan => new {
                ProductionMonthlyJobTypePlanId = ProductionMonthlyJobTypePlan.ProductionMonthlyJobTypePlanId,
                ProductionMonthlyPlanId = ProductionMonthlyJobTypePlan.ProductionMonthlyPlanId,
                JobTypeId = ProductionMonthlyJobTypePlan.JobTypeId,
                TotalPrice = ProductionMonthlyJobTypePlan.TotalPrice,
                TotalProduct = ProductionMonthlyJobTypePlan.TotalProduct
              
            });

            return Json(result);
        }

        public ActionResult ProductionMonthlyJobTypePlans_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProductionMonthlyJobTypePlan> productionMonthlyJobTypePlans, int id)
        {
            var jobTypeIds = productionMonthlyJobTypePlans.ToList().Select(s => s.ProductionMonthlyJobTypePlanId);
            var updatedMonthlyJobTypePlan = productionMonthlyJobTypePlans.ToList().Sum(s => s.TotalPrice);
            var monthlyPlan = db.ProductionMonthlyPlans.Find(id);
            var monthlyJobTypePlan = db.ProductionMonthlyJobTypePlans.Where(q => q.ProductionMonthlyPlanId==id).ToList().Sum(s => s.TotalPrice);

           // return Json(jobTypeIds);
                if (monthlyPlan?.TotalPrice < (updatedMonthlyJobTypePlan))
            {
                ModelState.AddModelError("Error", "Summation of monthly job type plans must equals with the monthly paln!");
            }
            if (ModelState.IsValid)
            {
                foreach (var productionMonthlyJobTypePlan in productionMonthlyJobTypePlans)
                {
                    productionMonthlyJobTypePlan.ProductionMonthlyPlanId = id;
                    var entity = (ProductionMonthlyJobTypePlan)this.GetObject(productionMonthlyJobTypePlan);

                    db.ProductionMonthlyJobTypePlans.Add(entity);
                }
                db.SaveChanges();
            }

            return Json(new[] { productionMonthlyJobTypePlans }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]      
            public ActionResult ProductionMonthlyJobTypePlans_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProductionMonthlyJobTypePlan> productionMonthlyJobTypePlans, int id)
            {
                var jobTypeIds = productionMonthlyJobTypePlans.ToList().Select(s => s.ProductionMonthlyJobTypePlanId);
                var updatedMonthlyJobTypePlan = productionMonthlyJobTypePlans.ToList().Sum(s => s.TotalPrice);
                var monthlyPlan = db.ProductionMonthlyPlans.Find(id);
                var monthlyJobTypePlan = db.ProductionMonthlyJobTypePlans.Where(q=>q.ProductionMonthlyPlanId==id).Where(q => !jobTypeIds.Contains(q.ProductionMonthlyJobTypePlanId)).ToList().Sum(s => s.TotalPrice);
            if (monthlyPlan?.TotalPrice < (monthlyJobTypePlan + updatedMonthlyJobTypePlan))
                {

                    ModelState.AddModelError("Error", "Summation of monthly job type plans must equals with the monthly paln!");
                }
                if (ModelState.IsValid)
            {
                foreach (var productionMonthlyJobTypePlan in productionMonthlyJobTypePlans)
                {
                    productionMonthlyJobTypePlan.ProductionMonthlyPlanId = id;
                    var entity = (ProductionMonthlyJobTypePlan)this.GetObject(productionMonthlyJobTypePlan);
                    if(productionMonthlyJobTypePlan.ProductionMonthlyJobTypePlanId==0)
                    {
                        db.ProductionMonthlyJobTypePlans.Add(entity);

                    }
                    else
                    {
                        db.ProductionMonthlyJobTypePlans.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                    }
                   
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
