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
    public class JobMaterialReturnController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobMaterialReturns_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IList<JobMaterialReturn> jobmaterialreturns = new List<JobMaterialReturn>();
            var jobCost = db.JobCosts.Find(id);
            if (jobCost != null)
            {

                var items = db.IssueItems.Where(q => q.JobNo == jobCost.JobId.ToString());
                foreach (var item in items)
                {
                    JobMaterialReturn jobMaterialReturn = new JobMaterialReturn();

                    jobMaterialReturn.JobMaterialReturnId = item.IssueItemID;
                    jobMaterialReturn.JobCostId = id;
                    jobMaterialReturn.ItemCode = item.ItemCode;
                    jobMaterialReturn.ItemCategoryCode = item.ItemCategoryCode;
                    jobMaterialReturn.Date = DateTime.Now;
                    jobMaterialReturn.Reference = item.JobNo;
                    jobMaterialReturn.Quantity = item.IssuedQuantity;
                    jobMaterialReturn.UnitPrice = item.UnitPrice;
                    jobMaterialReturn.Cost = item.Total;
                    jobMaterialReturn.SubTotal = item.UnitPrice * item.IssuedQuantity;
                    jobMaterialReturn.TaxAmount = item.Total - (item.UnitPrice * item.IssuedQuantity);
                    jobMaterialReturn.ItemDescription = jobCost.Description;
                    jobMaterialReturn.DateCreated = item.DateCreated;
                    jobMaterialReturn.LastModified = item.LastModified;
                    jobMaterialReturn.CreatedBy = item.CreatedBy;
                    jobMaterialReturn.ModifiedBy = item.ModifiedBy;
                    jobmaterialreturns.Add(jobMaterialReturn);
                }

            }
            DataSourceResult result = jobmaterialreturns.ToDataSourceResult(request, jobMaterialReturn => new {
                JobMaterialReturnId = jobMaterialReturn.JobMaterialReturnId,
                JobCostId = jobMaterialReturn.JobCostId,
                ItemCategoryCode = jobMaterialReturn.ItemCategoryCode,
                ItemCode = jobMaterialReturn.ItemCode,
                Date = jobMaterialReturn.Date,
                Reference = jobMaterialReturn.Reference,
                Quantity = jobMaterialReturn.Quantity,
                UnitPrice = jobMaterialReturn.UnitPrice,
                TaxAmount = jobMaterialReturn.TaxAmount,
                SubTotal = jobMaterialReturn.SubTotal,
                Cost = jobMaterialReturn.Cost,
                ItemDescription = jobMaterialReturn.ItemDescription,
                DateCreated = jobMaterialReturn.DateCreated,
                LastModified = jobMaterialReturn.LastModified,
                CreatedBy = jobMaterialReturn.CreatedBy,
                ModifiedBy = jobMaterialReturn.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult JobMaterialReturns_Create([DataSourceRequest]DataSourceRequest request, JobMaterialReturn jobMaterialReturn, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (JobMaterialReturn)this.GetObject(jobMaterialReturn, true, id);
                db.JobMaterialReturns.Add(entity);
                db.SaveChanges();
                jobMaterialReturn.JobMaterialReturnId = entity.JobMaterialReturnId;
            }

            return Json(new[] { jobMaterialReturn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobMaterialReturns_Update([DataSourceRequest]DataSourceRequest request, JobMaterialReturn jobMaterialReturn)
        {
            if (ModelState.IsValid)
            {
                var entity = (JobMaterialReturn)this.GetObject(jobMaterialReturn, false);

                db.JobMaterialReturns.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobMaterialReturn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobMaterialReturns_Destroy([DataSourceRequest]DataSourceRequest request, JobMaterialReturn jobMaterialReturn)
        {

            if (ModelState.IsValid)
            {
                var entity = (JobMaterialReturn)this.GetObject(jobMaterialReturn, false);

                db.JobMaterialReturns.Attach(entity);
                db.JobMaterialReturns.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobMaterialReturn }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(JobMaterialReturn jobMaterialReturn, bool status, int id = 0)
        {
            if (status)
            {
                jobMaterialReturn.JobCostId = id;
            }
            var entity = new JobMaterialReturn
            {
                JobMaterialReturnId = jobMaterialReturn.JobMaterialReturnId,
                JobCostId = jobMaterialReturn.JobCostId,
                ItemCategoryCode = jobMaterialReturn.ItemCategoryCode,
                ItemCode = jobMaterialReturn.ItemCode,
                Date = jobMaterialReturn.Date,
                Reference = jobMaterialReturn.Reference,
                Quantity = jobMaterialReturn.Quantity,
                UnitPrice = jobMaterialReturn.UnitPrice,
                TaxAmount = jobMaterialReturn.TaxAmount,
                SubTotal = jobMaterialReturn.SubTotal,
                Cost = jobMaterialReturn.Cost,
                ItemDescription = jobMaterialReturn.ItemDescription,
                DateCreated = jobMaterialReturn.DateCreated,
                LastModified = jobMaterialReturn.LastModified,
                CreatedBy = jobMaterialReturn.CreatedBy,
                ModifiedBy = jobMaterialReturn.ModifiedBy
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