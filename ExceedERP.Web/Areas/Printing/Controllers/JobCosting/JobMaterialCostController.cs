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
using ExceedERP.Core.Domain.printing.JobCosting;

namespace ExceedERP.Web.Areas.Printing.Controllers.JobCosting

{
    //[AuthorizeRoles(JobCostingRoles.JobCostingJobCostUser)]
    public class JobMaterialCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobMaterialCosts_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IList<JobMaterialCost> jobmaterialcosts = new List<JobMaterialCost>();
            var jobCost = db.JobCosts.Find(id);
            if(jobCost != null)
            {
                var job = db.Jobs.Find(jobCost.JobId);
                if(job!=null)
                {
                    var items = db.IssueItems.Where(q => q.ProductionJobNo == job.JobNo).ToList();
                    foreach (var item in items)
                    {
                        JobMaterialCost jobMaterial = new JobMaterialCost();

                        jobMaterial.JobMaterialCostId = item.IssueItemID;
                        jobMaterial.JobCostId = id;
                        jobMaterial.ItemCode = item.ItemCode;
                        jobMaterial.ItemCategoryCode = item.ItemCategoryCode;
                        jobMaterial.Date = DateTime.Now;
                        jobMaterial.Reference = db.Issues.Find(item.IssueID)?.Reference;
                        jobMaterial.Quantity = item.IssuedQuantity;
                        jobMaterial.UnitPrice = item.UnitPrice;
                        jobMaterial.Cost = item.Total;
                        jobMaterial.SubTotal = item.UnitPrice * item.IssuedQuantity;
                        jobMaterial.TaxAmount = item.Total - (item.UnitPrice * item.IssuedQuantity);
                        // jobMaterial.ItemDescription = jobCost.Re;
                        jobMaterial.DateCreated = item.DateCreated;
                        jobMaterial.LastModified = item.LastModified;
                        jobMaterial.CreatedBy = item.CreatedBy;
                        jobMaterial.ModifiedBy = item.ModifiedBy;
                        jobmaterialcosts.Add(jobMaterial);
                    }
                }
               
                
            }
            //return Json(jobmaterialcosts);

            DataSourceResult result = jobmaterialcosts.ToDataSourceResult(request, jobMaterialCost => new {
                JobMaterialCostId = jobMaterialCost.JobMaterialCostId,
                JobCostId = jobMaterialCost.JobCostId,
                ItemCode = jobMaterialCost.ItemCode,
                ItemCategoryCode = jobMaterialCost.ItemCategoryCode,
                Date = jobMaterialCost.Date,
                Reference = jobMaterialCost.Reference,
                Quantity = jobMaterialCost.Quantity,
                UnitPrice = jobMaterialCost.UnitPrice,
                Cost = jobMaterialCost.Cost,
                SubTotal = jobMaterialCost.SubTotal,
                TaxAmount = jobMaterialCost.TaxAmount,
                ItemDescription = jobMaterialCost.ItemDescription,
                DateCreated = jobMaterialCost.DateCreated,
                LastModified = jobMaterialCost.LastModified,
                CreatedBy = jobMaterialCost.CreatedBy,
                ModifiedBy = jobMaterialCost.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult JobMaterialCosts_Create([DataSourceRequest]DataSourceRequest request, JobMaterialCost jobMaterialCost, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (JobMaterialCost)this.GetObject(jobMaterialCost, true, id);
                db.JobMaterialCosts.Add(entity);
                db.SaveChanges();
                jobMaterialCost.JobMaterialCostId = entity.JobMaterialCostId;
            }

            return Json(new[] { jobMaterialCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobMaterialCosts_Update([DataSourceRequest]DataSourceRequest request, JobMaterialCost jobMaterialCost)
        {
            if (ModelState.IsValid)
            {
                var entity = (JobMaterialCost)this.GetObject(jobMaterialCost, false);

                db.JobMaterialCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobMaterialCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobMaterialCosts_Destroy([DataSourceRequest]DataSourceRequest request, JobMaterialCost jobMaterialCost)
        {

            if (ModelState.IsValid)
            {
                var entity = (JobMaterialCost)this.GetObject(jobMaterialCost, false);

                db.JobMaterialCosts.Attach(entity);
                db.JobMaterialCosts.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobMaterialCost }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(JobMaterialCost jobMaterialCost, bool status, int id = 0)
        {
            if (status)
            {
                jobMaterialCost.JobCostId = id;
            }
            var entity = new JobMaterialCost
            {
                JobMaterialCostId = jobMaterialCost.JobMaterialCostId,
                JobCostId = jobMaterialCost.JobCostId,
                ItemCategoryCode = jobMaterialCost.ItemCategoryCode,
                ItemCode = jobMaterialCost.ItemCode,
                Date = jobMaterialCost.Date,
                Reference = jobMaterialCost.Reference,
                Quantity = jobMaterialCost.Quantity,
                UnitPrice = jobMaterialCost.UnitPrice,
                SubTotal = jobMaterialCost.SubTotal,
                TaxAmount = jobMaterialCost.TaxAmount,
                Cost = jobMaterialCost.Cost,
                ItemDescription = jobMaterialCost.ItemDescription,
                DateCreated = jobMaterialCost.DateCreated,
                LastModified = jobMaterialCost.LastModified,
                CreatedBy = jobMaterialCost.CreatedBy,
                ModifiedBy = jobMaterialCost.ModifiedBy
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

