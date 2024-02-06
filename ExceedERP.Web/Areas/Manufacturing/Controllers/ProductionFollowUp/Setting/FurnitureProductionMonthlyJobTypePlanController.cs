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
            var jobTypes = db.FurnitureJobCategories.Where(q => !q.HaveParent).ToList();
            IList<FurnitureProductionMonthlyJobTypePlan> jobTypePlans = new List<FurnitureProductionMonthlyJobTypePlan>();
            foreach (var jobType in jobTypes)
            {
                FurnitureProductionMonthlyJobTypePlan jobTypePlan = new FurnitureProductionMonthlyJobTypePlan();
                var jobTypePlann = db.FurnitureProductionMonthlyJobTypePlans.Where(q => q.FurnitureProductionMonthlyPlanId == id
                                         && q.JobTypeId == jobType.FurnitureJobCategoryId).FirstOrDefault();
                int jobTypePlanId = 0;
                decimal jobTypeTotalPricePlan = 0;
                decimal jobTypeTotalProductPlan = 0;
                if (jobTypePlann!=null)
                {
                     jobTypePlanId = (int)(jobTypePlann?.FurnitureProductionMonthlyJobTypePlanId);

                     jobTypeTotalPricePlan = (decimal)jobTypePlann?.TotalPrice;
                     jobTypeTotalProductPlan = (decimal)jobTypePlann?.TotalProduct;
                    jobTypePlan.FurnitureProductionMonthlyJobTypePlanId = jobTypePlanId;
                }
                
                jobTypePlan.JobTypeId = jobType.FurnitureJobCategoryId;
                jobTypePlan.TotalProduct = jobTypeTotalProductPlan;
                jobTypePlan.TotalPrice = jobTypeTotalPricePlan;

                jobTypePlans.Add(jobTypePlan);

            }
            DataSourceResult result = jobTypePlans.ToDataSourceResult(request, ProductionMonthlyJobTypePlan => new {
                ProductionMonthlyJobTypePlanId = ProductionMonthlyJobTypePlan.FurnitureProductionMonthlyJobTypePlanId,
                ProductionMonthlyPlanId = ProductionMonthlyJobTypePlan.FurnitureProductionMonthlyPlanId,
                JobTypeId = ProductionMonthlyJobTypePlan.JobTypeId,
                TotalPrice = ProductionMonthlyJobTypePlan.TotalPrice,
                TotalProduct = ProductionMonthlyJobTypePlan.TotalProduct
              
            });

            return Json(result);
        }

        public ActionResult ProductionMonthlyJobTypePlans_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<FurnitureProductionMonthlyJobTypePlan> productionMonthlyJobTypePlans, int id)
        {
            var jobTypeIds = productionMonthlyJobTypePlans.ToList().Select(s => s.FurnitureProductionMonthlyJobTypePlanId);
            var updatedMonthlyJobTypePlan = productionMonthlyJobTypePlans.ToList().Sum(s => s.TotalPrice);
            var monthlyPlan = db.FurnitureProductionMonthlyPlans.Find(id);
            var monthlyJobTypePlan = db.FurnitureProductionMonthlyJobTypePlans.Where(q => q.FurnitureProductionMonthlyPlanId == id).ToList().Sum(s => s.TotalPrice);

           // return Json(jobTypeIds);
                if (monthlyPlan?.TotalPrice < (updatedMonthlyJobTypePlan))
            {
                ModelState.AddModelError("Error", "Summation of monthly job type plans must equals with the monthly paln!");
            }
            if (ModelState.IsValid)
            {
                foreach (var productionMonthlyJobTypePlan in productionMonthlyJobTypePlans)
                {
                    productionMonthlyJobTypePlan.FurnitureProductionMonthlyPlanId = id;
                    var entity = (FurnitureProductionMonthlyJobTypePlan)this.GetObject(productionMonthlyJobTypePlan);

                    db.FurnitureProductionMonthlyJobTypePlans.Add(entity);
                }
                db.SaveChanges();
            }

            return Json(new[] { productionMonthlyJobTypePlans }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]      
            public ActionResult ProductionMonthlyJobTypePlans_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<FurnitureProductionMonthlyJobTypePlan> productionMonthlyJobTypePlans, int id)
            {
                var jobTypeIds = productionMonthlyJobTypePlans.ToList().Select(s => s.FurnitureProductionMonthlyJobTypePlanId);
                var updatedMonthlyJobTypePlan = productionMonthlyJobTypePlans.ToList().Sum(s => s.TotalPrice);
                var monthlyPlan = db.FurnitureProductionMonthlyPlans.Find(id);
                var monthlyJobTypePlan = db.FurnitureProductionMonthlyJobTypePlans.Where(q=>q.FurnitureProductionMonthlyPlanId == id).Where(q => !jobTypeIds.Contains(q.FurnitureProductionMonthlyJobTypePlanId)).ToList().Sum(s => s.TotalPrice);
            if (monthlyPlan?.TotalPrice < (monthlyJobTypePlan + updatedMonthlyJobTypePlan))
                {

                    ModelState.AddModelError("Error", "Summation of monthly job type plans must equals with the monthly paln!");
                }
                if (ModelState.IsValid)
            {
                foreach (var productionMonthlyJobTypePlan in productionMonthlyJobTypePlans)
                {
                    productionMonthlyJobTypePlan.FurnitureProductionMonthlyPlanId = id;
                    var entity = (FurnitureProductionMonthlyJobTypePlan)this.GetObject(productionMonthlyJobTypePlan);
                    if(productionMonthlyJobTypePlan.FurnitureProductionMonthlyJobTypePlanId == 0)
                    {
                        db.FurnitureProductionMonthlyJobTypePlans.Add(entity);

                    }
                    else
                    {
                        db.FurnitureProductionMonthlyJobTypePlans.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                    }
                   
                }
                db.SaveChanges();
            }

            return Json(new[] { productionMonthlyJobTypePlans }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionMonthlyJobTypePlans_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureProductionMonthlyJobTypePlan ProductionMonthlyJobTypePlan)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureProductionMonthlyJobTypePlan)this.GetObject(ProductionMonthlyJobTypePlan);

                db.FurnitureProductionMonthlyJobTypePlans.Attach(entity);
                db.FurnitureProductionMonthlyJobTypePlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ProductionMonthlyJobTypePlan }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureProductionMonthlyJobTypePlan ProductionMonthlyJobTypePlan)
        {
            var entity = new FurnitureProductionMonthlyJobTypePlan
            {
                FurnitureProductionMonthlyJobTypePlanId = ProductionMonthlyJobTypePlan.FurnitureProductionMonthlyJobTypePlanId,
                FurnitureProductionMonthlyPlanId = ProductionMonthlyJobTypePlan.FurnitureProductionMonthlyPlanId,
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
