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
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation.Setting;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.JobCosting

{
    //[AuthorizeRoles(JobCostingRoles.JobCostingJobCostUser)]
    public class FurnitureJobLaborCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobLaborCosts_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureJobLaborCost> joblaborcosts = db.FurnitureJobLaborCosts.Where(q=>q.FurnitureJobCostId == id);
            DataSourceResult result = joblaborcosts.ToDataSourceResult(request, jobLaborCost => new {
                JobLaborCostId = jobLaborCost.FurnitureJobLaborCostId,
                JobCostId = jobLaborCost.FurnitureJobCostId,
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

        public ActionResult JobLaborCosts_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobLaborCost jobLaborCost, int id)
        {

            if (ModelState.IsValid)
            {
                var dlrs = db.FurnitureDailyLaborRates.Where(q => q.CostCenter == jobLaborCost.CostCenter && q.GLPeriodId == jobLaborCost.GLPeriodId).ToList();
                var dlrPer = dlrs.Where(q => q.EmployeeType== FurnitureEmployementType.Permanent).FirstOrDefault();
                var dlrCon = dlrs.Where(q => q.EmployeeType== FurnitureEmployementType.Contrat).FirstOrDefault();
                var yearlyOHRate = db.FurnitureYearlyOverHeadRates.Where(q => q.Values == jobLaborCost.CostCenter && q.GlFiscalYearId == jobLaborCost.GlFiscalYearId).FirstOrDefault();
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
                var entity = (FurnitureJobLaborCost)this.GetObject(jobLaborCost, true, id);
                db.FurnitureJobLaborCosts.Add(entity);
                db.SaveChanges();
                jobLaborCost.FurnitureJobLaborCostId = entity.FurnitureJobLaborCostId;
            }

            return Json(new[] { jobLaborCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobLaborCosts_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobLaborCost jobLaborCost)
        {
            if (ModelState.IsValid)
            {
                var dlrs = db.FurnitureDailyLaborRates.Where(q => q.CostCenter == jobLaborCost.CostCenter && q.GLPeriodId == jobLaborCost.GLPeriodId).ToList();
                var dlrPer = dlrs.Where(q => q.EmployeeType == FurnitureEmployementType.Permanent).FirstOrDefault();
                var dlrCon = dlrs.Where(q => q.EmployeeType == FurnitureEmployementType.Contrat).FirstOrDefault();
                var yearlyOHRate = db.FurnitureYearlyOverHeadRates.Where(q => q.Values == jobLaborCost.CostCenter && q.GlFiscalYearId == jobLaborCost.GlFiscalYearId).FirstOrDefault();
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
                var entity = (FurnitureJobLaborCost)this.GetObject(jobLaborCost, false);

                db.FurnitureJobLaborCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobLaborCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobLaborCosts_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobLaborCost jobLaborCost)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureJobLaborCost)this.GetObject(jobLaborCost, false);

                db.FurnitureJobLaborCosts.Attach(entity);
                db.FurnitureJobLaborCosts.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobLaborCost }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureJobLaborCost jobLaborCost, bool status, int id = 0)
        {
            if (status)
            {
                jobLaborCost.FurnitureJobCostId = id;
            }
            var entity = new FurnitureJobLaborCost
            {
                FurnitureJobLaborCostId = jobLaborCost.FurnitureJobLaborCostId,
                FurnitureJobCostId = jobLaborCost.FurnitureJobCostId,
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

