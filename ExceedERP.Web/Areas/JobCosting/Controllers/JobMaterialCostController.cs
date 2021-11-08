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

namespace ExceedERP.Web.Areas.JobCosting.Common
{
    public class JobMaterialCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobMaterialCosts_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<JobMaterialCost> jobmaterialcosts = db.JobMaterialCosts.Where(q => q.JobCostId == id); ;
            DataSourceResult result = jobmaterialcosts.ToDataSourceResult(request, jobMaterialCost => new {
                JobMaterialCostId = jobMaterialCost.JobMaterialCostId,
                JobCostId = jobMaterialCost.JobCostId,
                CostCenterId = jobMaterialCost.CostCenterId,
                GlFiscalYearId = jobMaterialCost.GlFiscalYearId,
                GLPeriodId = jobMaterialCost.GLPeriodId,
                Date = jobMaterialCost.Date,
                Reference = jobMaterialCost.Reference,
                Quantity = jobMaterialCost.Quantity,
                UnitPrice = jobMaterialCost.UnitPrice,
                Cost = jobMaterialCost.Cost,
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
                CostCenterId = jobMaterialCost.CostCenterId,
                GlFiscalYearId = jobMaterialCost.GlFiscalYearId,
                GLPeriodId = jobMaterialCost.GLPeriodId,
                Date = jobMaterialCost.Date,
                Reference = jobMaterialCost.Reference,
                Quantity = jobMaterialCost.Quantity,
                UnitPrice = jobMaterialCost.UnitPrice,
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

