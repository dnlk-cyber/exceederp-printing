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
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Plan
{
    // [AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpAdmin)]
    public class ProductionJobTypePlansController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYears"] = db.GLFiscalYears.ToList();
            ViewData["JobTypes"] = db.JobCategories.ToList();

            return View();
        }


        public ActionResult ProductionJobTypePlans_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var jobTypes = db.JobCategories.Where(q => !q.HaveParent).ToList();
            IList<ProductionJobTypePlan> jobTypePlans = new List<ProductionJobTypePlan>();
            foreach (var jobType in jobTypes)
            {
                ProductionJobTypePlan jobTypePlan = new ProductionJobTypePlan();
                var jobTypePlann = db.ProductionJobTypePlans.Where(q => q.WeeklyProductionPlanId == id
                                         && q.JobTypeId == jobType.JobCategoryId).FirstOrDefault();
                int jobTypePlanId = 0;
                decimal jobTypeTotalPricePlan = 0;
                decimal jobTypeTotalProductPlan = 0;
                if (jobTypePlann != null)
                {
                    jobTypePlanId = (int)(jobTypePlann?.ProductionJobTypePlanId);

                    jobTypeTotalPricePlan = (decimal)jobTypePlann?.ProductionPlanBirr;
                    jobTypeTotalProductPlan = (decimal)jobTypePlann?.ProductionPlanQuantity;
                    jobTypePlan.ProductionJobTypePlanId = jobTypePlanId;
                }

                jobTypePlan.JobTypeId = jobType.JobCategoryId;
                jobTypePlan.ProductionPlanQuantity = jobTypeTotalProductPlan;
                jobTypePlan.ProductionPlanBirr = jobTypeTotalPricePlan;

                jobTypePlans.Add(jobTypePlan);

            }
            DataSourceResult result = jobTypePlans.ToDataSourceResult(request, ProductionJobTypePlan => new {
                ProductionJobTypePlanId = ProductionJobTypePlan.ProductionJobTypePlanId,
                WeeklyProductionPlanId = ProductionJobTypePlan.WeeklyProductionPlanId,
                JobTypeId = ProductionJobTypePlan.JobTypeId,
                ProductionPlanBirr = ProductionJobTypePlan.ProductionPlanBirr,
                ProductionPlanQuantity = ProductionJobTypePlan.ProductionPlanQuantity

            });

            return Json(result);
        }

        public ActionResult ProductionJobTypePlans_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProductionJobTypePlan> productionJobTypePlans, int id)
        {
            var weeklyPlan = db.WeeklyProductionPlans.Find(id);
            var jopTypePlanIds = productionJobTypePlans.Select(s => s.ProductionJobTypePlanId);
            var jopTypeProductionPlanNotUpdatedRecs = db.ProductionJobTypePlans.Where(q => q.WeeklyProductionPlanId == id && !jopTypePlanIds.Contains(q.ProductionJobTypePlanId)).ToList();
            var totJobTypePlanUpdateds = productionJobTypePlans.Sum(s => s.ProductionPlanBirr);
            var totJobTypePlanNotUpdateds = jopTypeProductionPlanNotUpdatedRecs.Sum(s => s.ProductionPlanBirr);
            if ((totJobTypePlanUpdateds + totJobTypePlanNotUpdateds) > weeklyPlan.ProductionPlanBirr)
            {
                ModelState.AddModelError("Error", "Summation of quarter plans in birr must equal with half yearl plan");
            }
            totJobTypePlanUpdateds = productionJobTypePlans.Sum(s => s.ProductionPlanQuantity);
            totJobTypePlanNotUpdateds = jopTypeProductionPlanNotUpdatedRecs.Sum(s => s.ProductionPlanQuantity);
            if ((totJobTypePlanUpdateds + totJobTypePlanNotUpdateds) > weeklyPlan.ProductionPlanQuantity)
            {
                ModelState.AddModelError("Error", "Summation of quarter plans in quantity must equal with half yearl plan");
            }
            if (ModelState.IsValid)
            {
                foreach (var productionMonthlyJobTypePlan in productionJobTypePlans)
                {
                    productionMonthlyJobTypePlan.WeeklyProductionPlanId = id;
                    var entity = (ProductionJobTypePlan)this.GetObject(productionMonthlyJobTypePlan);

                    db.ProductionJobTypePlans.Add(entity);
                }
                db.SaveChanges();
            }

            return Json(new[] { productionJobTypePlans }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionJobTypePlans_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProductionJobTypePlan> productionJobTypePlans, int id)
        {
            var weeklyPlan = db.WeeklyProductionPlans.Find(id);

            var jobTypeIds = productionJobTypePlans.ToList().Select(s => s.ProductionJobTypePlanId);
            var updatedMonthlyJobTypePlanQuas = productionJobTypePlans.ToList().Sum(s => s.ProductionPlanQuantity);
            var updatedMonthlyJobTypePlanBirrs = productionJobTypePlans.ToList().Sum(s => s.ProductionPlanBirr);
            var jobTypePlansNotUpdateds = db.ProductionJobTypePlans.Where(q => !jobTypeIds.Contains(q.ProductionJobTypePlanId) && q.WeeklyProductionPlanId == id).ToList();
            var jobTypePlansNotUpdatedQuas = jobTypePlansNotUpdateds.Sum(s => s.ProductionPlanQuantity);
            var jobTypePlansNotUpdatedBirrs = jobTypePlansNotUpdateds.Sum(s => s.ProductionPlanBirr);
            if ((jobTypePlansNotUpdatedQuas + updatedMonthlyJobTypePlanQuas) > weeklyPlan.ProductionPlanQuantity)
            {
                ModelState.AddModelError("Error", "Summation of monthly job type plans in birr must equals with the monthly paln!");
            }
            if ((updatedMonthlyJobTypePlanBirrs + jobTypePlansNotUpdatedBirrs > weeklyPlan.ProductionPlanBirr))
            {
                ModelState.AddModelError("Error", "Summation of monthly job type plans in quantity must equals with the monthly paln!");
            }
            if (ModelState.IsValid)
            {
                foreach (var productionMonthlyJobTypePlan in productionJobTypePlans)
                {
                    productionMonthlyJobTypePlan.WeeklyProductionPlanId = id;
                    var entity = (ProductionJobTypePlan)this.GetObject(productionMonthlyJobTypePlan);
                    if (productionMonthlyJobTypePlan.ProductionJobTypePlanId == 0)
                    {
                        db.ProductionJobTypePlans.Add(entity);

                    }
                    else
                    {
                        db.ProductionJobTypePlans.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                    }

                }
                db.SaveChanges();
            }

            return Json(new[] { productionJobTypePlans }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionJobTypePlans_Destroy([DataSourceRequest]DataSourceRequest request, ProductionJobTypePlan ProductionJobTypePlan)
        {

            if (ModelState.IsValid)
            {
                var entity = (ProductionJobTypePlan)this.GetObject(ProductionJobTypePlan);

                db.ProductionJobTypePlans.Attach(entity);
                db.ProductionJobTypePlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ProductionJobTypePlan }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(ProductionJobTypePlan ProductionJobTypePlan)
        {
            var entity = new ProductionJobTypePlan
            {
                ProductionJobTypePlanId = ProductionJobTypePlan.ProductionJobTypePlanId,
                WeeklyProductionPlanId = ProductionJobTypePlan.WeeklyProductionPlanId,
                JobTypeId = ProductionJobTypePlan.JobTypeId,
                ProductionPlanBirr = ProductionJobTypePlan.ProductionPlanBirr,
                ProductionPlanQuantity = ProductionJobTypePlan.ProductionPlanQuantity

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
