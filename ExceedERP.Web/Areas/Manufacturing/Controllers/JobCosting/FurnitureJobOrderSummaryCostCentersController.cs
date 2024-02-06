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
using ExceedERP.Core.Domain.Manufacturing.JobCosting;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.JobCosting
{
    public class FurnitureJobOrderSummaryCostCentersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["JobOrder"] = db.Jobs.ToList();
            return View();
        }

        public ActionResult JobOrderSummaryCostCenters_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureJobOrderSummaryCostCenter> jobordersummarycostcenters = db.FurnitureJobOrderSummaryCostCenters;
            DataSourceResult result = jobordersummarycostcenters.ToDataSourceResult(request, jobOrderSummaryCostCenter => new {
                JobOrderSummaryCostCenterId = jobOrderSummaryCostCenter.FurnitureJobOrderSummaryCostCenterId,
                JobId = jobOrderSummaryCostCenter.JobId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderSummaryCostCenters_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderSummaryCostCenter jobOrderSummaryCostCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderSummaryCostCenter
                {
                    JobId = jobOrderSummaryCostCenter.JobId
                };

                db.FurnitureJobOrderSummaryCostCenters.Add(entity);
                db.SaveChanges();
                jobOrderSummaryCostCenter.FurnitureJobOrderSummaryCostCenterId = entity.FurnitureJobOrderSummaryCostCenterId;
            }

            return Json(new[] { jobOrderSummaryCostCenter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderSummaryCostCenters_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderSummaryCostCenter jobOrderSummaryCostCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderSummaryCostCenter
                {
                    FurnitureJobOrderSummaryCostCenterId = jobOrderSummaryCostCenter.FurnitureJobOrderSummaryCostCenterId,
                    JobId = jobOrderSummaryCostCenter.JobId
                };

                db.FurnitureJobOrderSummaryCostCenters.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobOrderSummaryCostCenter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderSummaryCostCenters_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderSummaryCostCenter jobOrderSummaryCostCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderSummaryCostCenter
                {
                    FurnitureJobOrderSummaryCostCenterId = jobOrderSummaryCostCenter.FurnitureJobOrderSummaryCostCenterId,
                    JobId = jobOrderSummaryCostCenter.JobId
                };

                db.FurnitureJobOrderSummaryCostCenters.Attach(entity);
                db.FurnitureJobOrderSummaryCostCenters.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobOrderSummaryCostCenter }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
