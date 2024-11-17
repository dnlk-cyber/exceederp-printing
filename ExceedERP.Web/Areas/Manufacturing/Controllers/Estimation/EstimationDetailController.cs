﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    public class EstimationDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EstimationDetails_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<EstimationDetail> estimationdetails = db.EstimationDetails.Where(x => x.EstimationSummaryId == id);
            DataSourceResult result = estimationdetails.ToDataSourceResult(request, estimationDetail => new {
                EstimationDetailId = estimationDetail.EstimationDetailId,
                JobTypeId = estimationDetail.JobTypeId,
                Quantity = estimationDetail.Quantity,
                MaterialCost = estimationDetail.MaterialCost,
                LabourCost = estimationDetail.LabourCost,
                OverHeadCost = estimationDetail.OverHeadCost,
                ManufacturingCost = estimationDetail.ManufacturingCost,
                AdministrativeCost = estimationDetail.AdministrativeCost,
                GrandTotalCost = estimationDetail.GrandTotalCost,
                EstimationSummaryId = estimationDetail.EstimationSummaryId
            });

            return Json(result);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
