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
using ExceedERP.Core.Domain.printing.PrintingEstimation.Setting;

namespace ExceedERP.Web.Areas.Printing.Controllers.JobCosting

{
    //[AuthorizeRoles(JobCostingRoles.JobCostingJobCostUser)]
    public class JobLaborCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobLaborCosts_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<JobLaborCost> joblaborcosts = db.JobLaborCosts.Where(q=>q.JobCostId == id);
            DataSourceResult result = joblaborcosts.ToDataSourceResult(request, jobLaborCost => new {
                JobLaborCostId = jobLaborCost.JobLaborCostId,
                JobCostId = jobLaborCost.JobCostId,
                CostCenter = jobLaborCost.CostCenter,
                GlFiscalYearId = jobLaborCost.GlFiscalYearId,
                GLPeriodId = jobLaborCost.GLPeriodId,
                Reference = jobLaborCost.Reference,
                PermanetNormalHour = jobLaborCost.PermanetNormalHour,
                PermanetLaborCost = jobLaborCost.PermanetLaborCost,
                PermanetOverHeadCost = jobLaborCost.PermanetOverHeadCost,
                PermanetOTHour = jobLaborCost.PermanetOTHour,
                ContractNormalHour = jobLaborCost.ContractNormalHour,
                ContractLaborCost = jobLaborCost.ContractLaborCost,
                ContractOverHeadCost = jobLaborCost.ContractOverHeadCost,
                ContractOTHour = jobLaborCost.ContractOTHour,
                Beginning = jobLaborCost.Beginning,
                BeginningHour = jobLaborCost.BeginningHour,
                DateCreated = jobLaborCost.DateCreated,
                LastModified = jobLaborCost.LastModified,
                CreatedBy = jobLaborCost.CreatedBy,
                ModifiedBy = jobLaborCost.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult JobLaborCosts_Create([DataSourceRequest]DataSourceRequest request, JobLaborCost jobLaborCost, int id)
        {

            if (ModelState.IsValid)
            {
                var dlrs = db.DailyLaborRates.Where(q => q.CostCenter == jobLaborCost.CostCenter && q.GLPeriodId == jobLaborCost.GLPeriodId).ToList();
                var dlrPer = dlrs.Where(q => q.EmployeeType==EmployementType.Permanent).FirstOrDefault();
                var dlrCon = dlrs.Where(q => q.EmployeeType==EmployementType.Contrat).FirstOrDefault();
                var yearlyOHRate = db.YearlyOverHeadRates.Where(q => q.Values == jobLaborCost.CostCenter && q.GlFiscalYearId == jobLaborCost.GlFiscalYearId).FirstOrDefault();
                if(dlrPer!=null)
                {
                    jobLaborCost.PermanetLaborCost = (decimal)Math.Round(jobLaborCost.PermanetNormalHour *(decimal) dlrPer.DLR,3);
                }
                if (dlrCon != null)
                {
                    jobLaborCost.ContractLaborCost = (decimal)Math.Round(jobLaborCost.ContractNormalHour * (decimal)dlrCon.DLR, 3);
                }
                if(yearlyOHRate != null)
                {
                    jobLaborCost.PermanetOverHeadCost = Math.Round(jobLaborCost.PermanetNormalHour * yearlyOHRate.Rate,3);
                    jobLaborCost.ContractOverHeadCost = Math.Round(jobLaborCost.ContractNormalHour * yearlyOHRate.Rate,3);
                }
                var entity = (JobLaborCost)this.GetObject(jobLaborCost, true, id);
                db.JobLaborCosts.Add(entity);
                db.SaveChanges();
                jobLaborCost.JobLaborCostId = entity.JobLaborCostId;
            }

            return Json(new[] { jobLaborCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobLaborCosts_Update([DataSourceRequest]DataSourceRequest request, JobLaborCost jobLaborCost)
        {
            if (ModelState.IsValid)
            {
                var dlrs = db.DailyLaborRates.Where(q => q.CostCenter == jobLaborCost.CostCenter && q.GLPeriodId == jobLaborCost.GLPeriodId).ToList();
                var dlrPer = dlrs.Where(q => q.EmployeeType ==EmployementType.Permanent).FirstOrDefault();
                var dlrCon = dlrs.Where(q => q.EmployeeType ==EmployementType.Contrat).FirstOrDefault();
                var yearlyOHRate = db.YearlyOverHeadRates.Where(q => q.Values == jobLaborCost.CostCenter && q.GlFiscalYearId == jobLaborCost.GlFiscalYearId).FirstOrDefault();
                if (dlrPer != null)
                {
                    jobLaborCost.PermanetLaborCost = (decimal)Math.Round(jobLaborCost.PermanetNormalHour * (decimal)dlrPer.DLR, 3);
                }
                if (dlrCon != null)
                {
                    jobLaborCost.ContractLaborCost = (decimal)Math.Round(jobLaborCost.ContractNormalHour * (decimal)dlrCon.DLR, 3);
                }
                if (yearlyOHRate != null)
                {
                    jobLaborCost.PermanetOverHeadCost = Math.Round(jobLaborCost.PermanetNormalHour * yearlyOHRate.Rate, 3);
                    jobLaborCost.ContractOverHeadCost = Math.Round(jobLaborCost.ContractNormalHour * yearlyOHRate.Rate, 3);
                }
                var entity = (JobLaborCost)this.GetObject(jobLaborCost, false);

                db.JobLaborCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobLaborCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobLaborCosts_Destroy([DataSourceRequest]DataSourceRequest request, JobLaborCost jobLaborCost)
        {

            if (ModelState.IsValid)
            {
                var entity = (JobLaborCost)this.GetObject(jobLaborCost, false);

                db.JobLaborCosts.Attach(entity);
                db.JobLaborCosts.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobLaborCost }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(JobLaborCost jobLaborCost, bool status, int id = 0)
        {
            if (status)
            {
                jobLaborCost.JobCostId = id;
            }
            var entity = new JobLaborCost
            {
                JobLaborCostId = jobLaborCost.JobLaborCostId,
                JobCostId = jobLaborCost.JobCostId,
                CostCenter = jobLaborCost.CostCenter,
                GlFiscalYearId = jobLaborCost.GlFiscalYearId,
                GLPeriodId = jobLaborCost.GLPeriodId,
                Reference = jobLaborCost.Reference,
                PermanetNormalHour = jobLaborCost.PermanetNormalHour,
                PermanetLaborCost = jobLaborCost.PermanetLaborCost,
                PermanetOverHeadCost = jobLaborCost.PermanetOverHeadCost,
                PermanetOTHour = jobLaborCost.PermanetOTHour,
                ContractOTHour = jobLaborCost.ContractOTHour,
                ContractNormalHour = jobLaborCost.ContractNormalHour,
                ContractLaborCost = jobLaborCost.ContractLaborCost,
                ContractOverHeadCost = jobLaborCost.ContractOverHeadCost,
                Beginning = jobLaborCost.Beginning,
                BeginningHour = jobLaborCost.BeginningHour,
                DateCreated = jobLaborCost.DateCreated,
                LastModified = jobLaborCost.LastModified,
                CreatedBy = jobLaborCost.CreatedBy,
                ModifiedBy = jobLaborCost.ModifiedBy
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

