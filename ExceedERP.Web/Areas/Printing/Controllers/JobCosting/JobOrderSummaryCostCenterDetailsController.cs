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
    public class JobOrderSummaryCostCenterDetailsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobOrderSummaryCostCenterDetails_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<JobOrderSummaryCostCenterDetail> jobordersummarycostcenterdetails = db.JobOrderSummaryCostCenterDetails.Where(x=>x.JobOrderSummaryCostCenterId == id);
            DataSourceResult result = jobordersummarycostcenterdetails.ToDataSourceResult(request, jobOrderSummaryCostCenterDetail => new {
                JobOrderSummaryCostCenterDetailId = jobOrderSummaryCostCenterDetail.JobOrderSummaryCostCenterDetailId,
                JobOrderSummaryCostCenterId = jobOrderSummaryCostCenterDetail.JobOrderSummaryCostCenterId,
                CostCenter = jobOrderSummaryCostCenterDetail.CostCenter,
                Date = jobOrderSummaryCostCenterDetail.Date,
                DirectLaborTotalHour = jobOrderSummaryCostCenterDetail.DirectLaborTotalHour,
                Rate = jobOrderSummaryCostCenterDetail.Rate,
                DirectLaborAmount = jobOrderSummaryCostCenterDetail.DirectLaborAmount,
                DirectMachineTotalHour = jobOrderSummaryCostCenterDetail.DirectMachineTotalHour,
                OverHeadRate = jobOrderSummaryCostCenterDetail.OverHeadRate,
                OverHeadAmount = jobOrderSummaryCostCenterDetail.OverHeadAmount,
                TotalCostOfTheCenter = jobOrderSummaryCostCenterDetail.TotalCostOfTheCenter
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderSummaryCostCenterDetails_Create([DataSourceRequest]DataSourceRequest request, JobOrderSummaryCostCenterDetail jobOrderSummaryCostCenterDetail, int id)
        {
            if (ModelState.IsValid)
            {
                jobOrderSummaryCostCenterDetail.DirectLaborAmount = jobOrderSummaryCostCenterDetail.DirectLaborTotalHour * jobOrderSummaryCostCenterDetail.Rate;
                jobOrderSummaryCostCenterDetail.OverHeadAmount = jobOrderSummaryCostCenterDetail.DirectMachineTotalHour * jobOrderSummaryCostCenterDetail.OverHeadRate;
                jobOrderSummaryCostCenterDetail.TotalCostOfTheCenter = jobOrderSummaryCostCenterDetail.DirectLaborAmount + jobOrderSummaryCostCenterDetail.OverHeadAmount;
                var entity = new JobOrderSummaryCostCenterDetail
                {
                    JobOrderSummaryCostCenterId = id,
                    CostCenter = jobOrderSummaryCostCenterDetail.CostCenter,
                    Date = jobOrderSummaryCostCenterDetail.Date,
                    DirectLaborTotalHour = jobOrderSummaryCostCenterDetail.DirectLaborTotalHour,
                    Rate = jobOrderSummaryCostCenterDetail.Rate,
                    DirectLaborAmount = jobOrderSummaryCostCenterDetail.DirectLaborAmount,
                    DirectMachineTotalHour = jobOrderSummaryCostCenterDetail.DirectMachineTotalHour,
                    OverHeadRate = jobOrderSummaryCostCenterDetail.OverHeadRate,
                    OverHeadAmount = jobOrderSummaryCostCenterDetail.OverHeadAmount,
                    TotalCostOfTheCenter = jobOrderSummaryCostCenterDetail.TotalCostOfTheCenter
                };

                db.JobOrderSummaryCostCenterDetails.Add(entity);
                db.SaveChanges();
                jobOrderSummaryCostCenterDetail.JobOrderSummaryCostCenterDetailId = entity.JobOrderSummaryCostCenterDetailId;
            }

            return Json(new[] { jobOrderSummaryCostCenterDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderSummaryCostCenterDetails_Update([DataSourceRequest]DataSourceRequest request, JobOrderSummaryCostCenterDetail jobOrderSummaryCostCenterDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobOrderSummaryCostCenterDetail
                {
                    JobOrderSummaryCostCenterDetailId = jobOrderSummaryCostCenterDetail.JobOrderSummaryCostCenterDetailId,
                    JobOrderSummaryCostCenterId = jobOrderSummaryCostCenterDetail.JobOrderSummaryCostCenterId,
                    CostCenter = jobOrderSummaryCostCenterDetail.CostCenter,
                    Date = jobOrderSummaryCostCenterDetail.Date,
                    DirectLaborTotalHour = jobOrderSummaryCostCenterDetail.DirectLaborTotalHour,
                    Rate = jobOrderSummaryCostCenterDetail.Rate,
                    DirectLaborAmount = jobOrderSummaryCostCenterDetail.DirectLaborAmount,
                    DirectMachineTotalHour = jobOrderSummaryCostCenterDetail.DirectMachineTotalHour,
                    OverHeadRate = jobOrderSummaryCostCenterDetail.OverHeadRate,
                    OverHeadAmount = jobOrderSummaryCostCenterDetail.OverHeadAmount,
                    TotalCostOfTheCenter = jobOrderSummaryCostCenterDetail.TotalCostOfTheCenter
                };

                db.JobOrderSummaryCostCenterDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobOrderSummaryCostCenterDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderSummaryCostCenterDetails_Destroy([DataSourceRequest]DataSourceRequest request, JobOrderSummaryCostCenterDetail jobOrderSummaryCostCenterDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobOrderSummaryCostCenterDetail
                {
                    JobOrderSummaryCostCenterDetailId = jobOrderSummaryCostCenterDetail.JobOrderSummaryCostCenterDetailId,
                    JobOrderSummaryCostCenterId = jobOrderSummaryCostCenterDetail.JobOrderSummaryCostCenterId,
                    CostCenter = jobOrderSummaryCostCenterDetail.CostCenter,
                    Date = jobOrderSummaryCostCenterDetail.Date,
                    DirectLaborTotalHour = jobOrderSummaryCostCenterDetail.DirectLaborTotalHour,
                    Rate = jobOrderSummaryCostCenterDetail.Rate,
                    DirectLaborAmount = jobOrderSummaryCostCenterDetail.DirectLaborAmount,
                    DirectMachineTotalHour = jobOrderSummaryCostCenterDetail.DirectMachineTotalHour,
                    OverHeadRate = jobOrderSummaryCostCenterDetail.OverHeadRate,
                    OverHeadAmount = jobOrderSummaryCostCenterDetail.OverHeadAmount,
                    TotalCostOfTheCenter = jobOrderSummaryCostCenterDetail.TotalCostOfTheCenter
                };

                db.JobOrderSummaryCostCenterDetails.Attach(entity);
                db.JobOrderSummaryCostCenterDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobOrderSummaryCostCenterDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
