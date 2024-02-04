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
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.printing.PrintingEstimation;
using ExceedERP.Web.Areas.Printing.Models;

namespace ExceedERP.Web.Areas.Printing.Controllers.Estimation
{
    //[AuthorizeRoles(PrintingEstimationRoles.PrintingEstimationFormUser, PrintingEstimationRoles.PrintingEstimationAdmin)]
    public class LaborCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LaborCosts_Read([DataSourceRequest]DataSourceRequest request, int? estimationFormId)
        {
            IQueryable<LaborCost> laborcosts = db.LaborCosts.Where(q=>q.EstimationFormId== estimationFormId).OrderByDescending(a => a.LaborCostId);
            DataSourceResult result = laborcosts.ToDataSourceResult(request, laborCost => new {
                LaborCostId = laborCost.LaborCostId,
                EstimationFormId = laborCost.EstimationFormId,
                PrintingStepId = laborCost.PrintingStepId,
                PrintingSteps = laborCost.PrintingSteps,
                WorkingTime = laborCost.WorkingTime,
                CostPerHour = laborCost.CostPerHour,
                TotalCost = laborCost.TotalCost,
                DateCreated = laborCost.DateCreated,
                LastModified = laborCost.LastModified
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LaborCosts_Create([DataSourceRequest]DataSourceRequest request, LaborCost laborCost,  int parentId)
        {
            if (ModelState.IsValid)
            {
               
                var entity = new LaborCost
                {
                    EstimationFormId = parentId,
                    PrintingSteps = laborCost.PrintingSteps,
                    PrintingStepId = laborCost.PrintingStepId,
                    WorkingTime = laborCost.WorkingTime,
                    CostPerHour = laborCost.CostPerHour,
                    TotalCost = (laborCost.CostPerHour) * (laborCost.WorkingTime)
            };

                db.LaborCosts.Add(entity);
                db.SaveChanges();
                laborCost.LaborCostId = entity.LaborCostId;
                 var estimation = new UpdateOverallCost();
                   estimation.update(parentId);

            }
        

            return Json(new[] { laborCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LaborCosts_Update([DataSourceRequest]DataSourceRequest request, LaborCost laborCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new LaborCost
                {
                    LaborCostId = laborCost.LaborCostId,
                    PrintingStepId = laborCost.PrintingStepId,
                    EstimationFormId = laborCost.EstimationFormId,
                    PrintingSteps = laborCost.PrintingSteps,
                    WorkingTime = laborCost.WorkingTime,
                    CostPerHour = laborCost.CostPerHour,
                    TotalCost = (laborCost.CostPerHour) * (laborCost.WorkingTime)
                };

                db.LaborCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                var estimation = new UpdateOverallCost();
                estimation.update(laborCost.EstimationFormId);
            }

            return Json(new[] { laborCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LaborCosts_Destroy([DataSourceRequest]DataSourceRequest request, LaborCost laborCost)
        {

            if (ModelState.IsValid)
            {
                var entity = new LaborCost
                {
                    LaborCostId = laborCost.LaborCostId,
                    EstimationFormId = laborCost.EstimationFormId,
                    PrintingStepId = laborCost.PrintingStepId,
                    PrintingSteps = laborCost.PrintingSteps,
                    WorkingTime = laborCost.WorkingTime,
                    CostPerHour = laborCost.CostPerHour,
                    TotalCost = laborCost.TotalCost

                };

                db.LaborCosts.Attach(entity);
                db.LaborCosts.Remove(entity);
                db.SaveChanges();
                var estimation = new UpdateOverallCost();
                estimation.update(laborCost.EstimationFormId);
            }

            return Json(new[] { laborCost }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
