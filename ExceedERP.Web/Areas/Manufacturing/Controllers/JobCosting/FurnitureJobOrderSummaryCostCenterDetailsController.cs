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
    public class FurnitureJobOrderSummaryCostCenterDetailsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobOrderSummaryCostCenterDetails_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureJobOrderSummaryCostCenterDetail> jobordersummarycostcenterdetails = db.FurnitureJobOrderSummaryCostCenterDetails.Where(x=>x.FurnitureJobOrderSummaryCostCenterId == id);
            DataSourceResult result = jobordersummarycostcenterdetails.ToDataSourceResult(request, jobOrderSummaryCostCenterDetail => new {
                FurnitureJobOrderSummaryCostCenterDetailId = jobOrderSummaryCostCenterDetail.FurnitureJobOrderSummaryCostCenterDetailId,
                FurnitureJobOrderSummaryCostCenterId = jobOrderSummaryCostCenterDetail.FurnitureJobOrderSummaryCostCenterId,
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
        public ActionResult JobOrderSummaryCostCenterDetails_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderSummaryCostCenterDetail jobOrderSummaryCostCenterDetail, int id)
        {
            if (ModelState.IsValid)
            {
                jobOrderSummaryCostCenterDetail.DirectLaborAmount = jobOrderSummaryCostCenterDetail.DirectLaborTotalHour * jobOrderSummaryCostCenterDetail.Rate;
                jobOrderSummaryCostCenterDetail.OverHeadAmount = jobOrderSummaryCostCenterDetail.DirectMachineTotalHour * jobOrderSummaryCostCenterDetail.OverHeadRate;
                jobOrderSummaryCostCenterDetail.TotalCostOfTheCenter = jobOrderSummaryCostCenterDetail.DirectLaborAmount + jobOrderSummaryCostCenterDetail.OverHeadAmount;
                var entity = new FurnitureJobOrderSummaryCostCenterDetail
                {
                    FurnitureJobOrderSummaryCostCenterId = id,
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

                db.FurnitureJobOrderSummaryCostCenterDetails.Add(entity);
                db.SaveChanges();
                jobOrderSummaryCostCenterDetail.FurnitureJobOrderSummaryCostCenterDetailId = entity.FurnitureJobOrderSummaryCostCenterDetailId;
            }

            return Json(new[] { jobOrderSummaryCostCenterDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderSummaryCostCenterDetails_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderSummaryCostCenterDetail jobOrderSummaryCostCenterDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderSummaryCostCenterDetail
                {
                    FurnitureJobOrderSummaryCostCenterDetailId = jobOrderSummaryCostCenterDetail.FurnitureJobOrderSummaryCostCenterDetailId,
                    FurnitureJobOrderSummaryCostCenterId = jobOrderSummaryCostCenterDetail.FurnitureJobOrderSummaryCostCenterId,
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

                db.FurnitureJobOrderSummaryCostCenterDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobOrderSummaryCostCenterDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderSummaryCostCenterDetails_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderSummaryCostCenterDetail jobOrderSummaryCostCenterDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderSummaryCostCenterDetail
                {
                    FurnitureJobOrderSummaryCostCenterDetailId = jobOrderSummaryCostCenterDetail.FurnitureJobOrderSummaryCostCenterDetailId,
                    FurnitureJobOrderSummaryCostCenterId = jobOrderSummaryCostCenterDetail.FurnitureJobOrderSummaryCostCenterId,
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

                db.FurnitureJobOrderSummaryCostCenterDetails.Attach(entity);
                db.FurnitureJobOrderSummaryCostCenterDetails.Remove(entity);
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
