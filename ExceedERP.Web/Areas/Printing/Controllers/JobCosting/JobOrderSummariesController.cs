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
    public class JobOrderSummariesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["JobOrder"] = db.Jobs.ToList();
            return View();
        }

        public ActionResult JobOrderSummarys_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<JobOrderSummary> jobordersummarys = db.JobOrderSummarys;
            DataSourceResult result = jobordersummarys.ToDataSourceResult(request, jobOrderSummary => new {
                JobOrderSummaryId = jobOrderSummary.JobOrderSummaryId,
                JobId = jobOrderSummary.JobId,
                Product = jobOrderSummary.Product,
                Quantity = jobOrderSummary.Quantity,
                Page = jobOrderSummary.Page,
                SellingPrice = jobOrderSummary.SellingPrice,
                DirectMaterial = jobOrderSummary.DirectMaterial,
                DirectLabor = jobOrderSummary.DirectLabor,
                ManufacturingOverHead = jobOrderSummary.ManufacturingOverHead,
                SellingAndDistribution = jobOrderSummary.SellingAndDistribution,
                AdminAndGeneralExpense = jobOrderSummary.AdminAndGeneralExpense,
                TotalCost = jobOrderSummary.TotalCost,
                Percentage = jobOrderSummary.Percentage,
                ProfitOrLoss = jobOrderSummary.ProfitOrLoss
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderSummarys_Create([DataSourceRequest]DataSourceRequest request, JobOrderSummary jobOrderSummary)
        {
            if (ModelState.IsValid)
            {
                jobOrderSummary.TotalCost = jobOrderSummary.AdminAndGeneralExpense + jobOrderSummary.SellingAndDistribution + jobOrderSummary.ManufacturingOverHead + jobOrderSummary.DirectLabor + jobOrderSummary.DirectMaterial;
                jobOrderSummary.ProfitOrLoss = jobOrderSummary.SellingPrice - jobOrderSummary.TotalCost;
                jobOrderSummary.Percentage = Convert.ToInt32(jobOrderSummary.ProfitOrLoss / jobOrderSummary.SellingPrice);

                var entity = new JobOrderSummary
                {
                    JobId = jobOrderSummary.JobId,
                    Product = jobOrderSummary.Product,
                    Quantity = jobOrderSummary.Quantity,
                    Page = jobOrderSummary.Page,
                    SellingPrice = jobOrderSummary.SellingPrice,
                    DirectMaterial = jobOrderSummary.DirectMaterial,
                    DirectLabor = jobOrderSummary.DirectLabor,
                    ManufacturingOverHead = jobOrderSummary.ManufacturingOverHead,
                    SellingAndDistribution = jobOrderSummary.SellingAndDistribution,
                    AdminAndGeneralExpense = jobOrderSummary.AdminAndGeneralExpense,
                    TotalCost = jobOrderSummary.TotalCost,
                    Percentage = jobOrderSummary.Percentage,
                    ProfitOrLoss = jobOrderSummary.ProfitOrLoss
                };

                db.JobOrderSummarys.Add(entity);
                db.SaveChanges();
                jobOrderSummary.JobOrderSummaryId = entity.JobOrderSummaryId;
            }

            return Json(new[] { jobOrderSummary }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderSummarys_Update([DataSourceRequest]DataSourceRequest request, JobOrderSummary jobOrderSummary)
        {
            if (ModelState.IsValid)
            {
                jobOrderSummary.TotalCost = jobOrderSummary.AdminAndGeneralExpense + jobOrderSummary.SellingAndDistribution + jobOrderSummary.ManufacturingOverHead + jobOrderSummary.DirectLabor + jobOrderSummary.DirectMaterial;
                jobOrderSummary.ProfitOrLoss = jobOrderSummary.SellingPrice - jobOrderSummary.TotalCost;
                jobOrderSummary.Percentage = Convert.ToInt32(jobOrderSummary.ProfitOrLoss / jobOrderSummary.SellingPrice);

                var entity = new JobOrderSummary
                {
                    JobOrderSummaryId = jobOrderSummary.JobOrderSummaryId,
                    JobId = jobOrderSummary.JobId,
                    Product = jobOrderSummary.Product,
                    Quantity = jobOrderSummary.Quantity,
                    Page = jobOrderSummary.Page,
                    SellingPrice = jobOrderSummary.SellingPrice,
                    DirectMaterial = jobOrderSummary.DirectMaterial,
                    DirectLabor = jobOrderSummary.DirectLabor,
                    ManufacturingOverHead = jobOrderSummary.ManufacturingOverHead,
                    SellingAndDistribution = jobOrderSummary.SellingAndDistribution,
                    AdminAndGeneralExpense = jobOrderSummary.AdminAndGeneralExpense,
                    TotalCost = jobOrderSummary.TotalCost,
                    Percentage = jobOrderSummary.Percentage,
                    ProfitOrLoss = jobOrderSummary.ProfitOrLoss
                };

                db.JobOrderSummarys.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobOrderSummary }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderSummarys_Destroy([DataSourceRequest]DataSourceRequest request, JobOrderSummary jobOrderSummary)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobOrderSummary
                {
                    JobOrderSummaryId = jobOrderSummary.JobOrderSummaryId,
                    JobId = jobOrderSummary.JobId,
                    Product = jobOrderSummary.Product,
                    Quantity = jobOrderSummary.Quantity,
                    Page = jobOrderSummary.Page,
                    SellingPrice = jobOrderSummary.SellingPrice,
                    DirectMaterial = jobOrderSummary.DirectMaterial,
                    DirectLabor = jobOrderSummary.DirectLabor,
                    ManufacturingOverHead = jobOrderSummary.ManufacturingOverHead,
                    SellingAndDistribution = jobOrderSummary.SellingAndDistribution,
                    AdminAndGeneralExpense = jobOrderSummary.AdminAndGeneralExpense,
                    TotalCost = jobOrderSummary.TotalCost,
                    Percentage = jobOrderSummary.Percentage,
                    ProfitOrLoss = jobOrderSummary.ProfitOrLoss
                };

                db.JobOrderSummarys.Attach(entity);
                db.JobOrderSummarys.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobOrderSummary }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
