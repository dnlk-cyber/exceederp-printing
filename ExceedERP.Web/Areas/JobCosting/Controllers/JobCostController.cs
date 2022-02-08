﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.JobCosting;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.JobCosting.Controllers
{
    //[AuthorizeRoles(JobCostingRoles.JobCostingJobCostUser)]
    public class JobCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYears"] = db.GLFiscalYears.ToList();
            ViewData["CostCenters"] = db.GLChartOfAccountCostCenters.ToList();
            ViewData["Periods"] = db.GLPeriods.ToList();
            ViewData["Customers"] = db.OrganizationCustomers.Select(s => new {
                 s.OrganizationCustomerID,
               s.TradeName
            }).ToList();
            ViewData["JobOrders"] = db.Jobs.ToList();
            ViewData["JobCategories"] = db.JobCategories.ToList();
            var itemCategorys = db.ItemCategorys.Select(s => new { Code = s.ItemCategoryCode, NAme = s.Name }).ToList();
            ViewData["ItemCategorys"] = itemCategorys;
            var items = db.InventoryItems.Select(s => new { Code = s.ItemCode, Name=s.Name }).ToList();
            ViewData["Items"] = items;
            return View();
        }
        public JsonResult GetAllCostCenters()
        {
            var data = db.GLChartOfAccountCostCenters;
            return Json(data.Select(s => new { s.Values }), JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult JobCosts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<JobCost> jobcosts = db.JobCosts;
            DataSourceResult result = jobcosts.ToDataSourceResult(request, jobCost => new {
                JobCostId = jobCost.JobCostId,
                JobId = jobCost.JobId,
                JobCategoryId = jobCost.JobCategoryId,
                CustomerId = jobCost.CustomerId,
                JobReceivedDate = jobCost.JobReceivedDate,
                Date = jobCost.Date,
                ReciptNo = jobCost.ReciptNo,
                Quantity = jobCost.Quantity,
                SellingPrice = jobCost.SellingPrice,
                OrderSize = jobCost.OrderSize,
                Void = jobCost.Void,
                Status = jobCost.Status,
                Description = jobCost.Description,
                DateCreated = jobCost.DateCreated,
                LastModified = jobCost.LastModified,
                CreatedBy = jobCost.CreatedBy,
                ModifiedBy = jobCost.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult JobCosts_Create([DataSourceRequest]DataSourceRequest request, JobCost jobCost)
        {
            if (ModelState.IsValid)
            {
                var entity = (JobCost)this.GetObject(jobCost);
                db.JobCosts.Add(entity);
                db.SaveChanges();
                jobCost.JobCostId = entity.JobCostId;
                return Json(new[] { entity }.ToDataSourceResult(request, ModelState));

            }

            return Json(new[] { jobCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobCosts_Update([DataSourceRequest]DataSourceRequest request, JobCost jobCost)
        {
            if (ModelState.IsValid)
            {
                var entity = (JobCost)this.GetObject(jobCost);

                db.JobCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobCosts_Destroy([DataSourceRequest]DataSourceRequest request, JobCost jobCost)
        {

            if (ModelState.IsValid)
            {
                var entity = (JobCost)this.GetObject(jobCost);

                db.JobCosts.Attach(entity);
                db.JobCosts.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobCost }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(JobCost jobCost)
        {
            var jobOrder = db.Jobs.Find(jobCost.JobId);
            bool status = false;
            if (jobOrder != null)
            {
                 status = true;

            }
            var entity = new JobCost
            {
                JobCostId = jobCost.JobCostId,
                JobId = jobCost.JobId,
                JobCategoryId = status?jobOrder.JobTypeId:jobCost.JobCategoryId,
                CustomerId = jobOrder.CustomerId,
                JobReceivedDate = jobCost.JobReceivedDate,
                Date = jobCost.Date,
                ReciptNo = jobCost.ReciptNo,
                Quantity = jobCost.Quantity,
                SellingPrice = jobCost.SellingPrice,
                OrderSize = jobCost.OrderSize,
                Void = jobCost.Void,
                Status = jobCost.Status,
                Description = jobCost.Description,
                DateCreated = jobCost.DateCreated,
                LastModified = jobCost.LastModified,
                CreatedBy = jobCost.CreatedBy,
                ModifiedBy = jobCost.ModifiedBy

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