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

namespace ExceedERP.Web.Areas.Printing.Controllers.Estimation
{
    //[AuthorizeRoles(PrintingEstimationRoles.PrintingEstimationFormUser, PrintingEstimationRoles.PrintingEstimationAdmin)]
    public class PrintingStepController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAllPrintingStepValues(int? PrintingStep)
        {
            var printingSteps = db.PrintingSteps.AsEnumerable();

            if (PrintingStep != null)
            {
                printingSteps = printingSteps.Where(p =>(int)p.PrintingSteps == PrintingStep);
            }

            return Json(printingSteps.Select(p => new { PrintingSteps = p.PrintingSteps, PrintingStepId = p.PrintingStepId, Name = p.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintingSteps_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingStep> printingsteps = db.PrintingSteps;
            DataSourceResult result = printingsteps.ToDataSourceResult(request, printingStep => new {
                PrintingStepId = printingStep.PrintingStepId,
                PrintingSteps = printingStep.PrintingSteps,
                Name = printingStep.Name,
                Description = printingStep.Description
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingSteps_Create([DataSourceRequest]DataSourceRequest request, PrintingStep printingStep)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingStep
                {
                    PrintingStepId = printingStep.PrintingStepId,
                    PrintingSteps = printingStep.PrintingSteps,
                    Name = printingStep.Name,
                    Description = printingStep.Description
                };

                db.PrintingSteps.Add(entity);
                db.SaveChanges();
                printingStep.PrintingStepId = entity.PrintingStepId;
            }

            return Json(new[] { printingStep }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingSteps_Update([DataSourceRequest]DataSourceRequest request, PrintingStep printingStep)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingStep
                {
                    PrintingStepId = printingStep.PrintingStepId,
                    PrintingSteps = printingStep.PrintingSteps,
                    Name = printingStep.Name,
                    Description = printingStep.Description
                };

                db.PrintingSteps.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printingStep }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingSteps_Destroy([DataSourceRequest]DataSourceRequest request, PrintingStep printingStep)
        {

            if (ModelState.IsValid)
            {
                var entity = new PrintingStep
                {
                    PrintingStepId = printingStep.PrintingStepId,
                    PrintingSteps = printingStep.PrintingSteps,
                    Name = printingStep.Name,
                    Description = printingStep.Description

                };

                db.PrintingSteps.Attach(entity);
                db.PrintingSteps.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { printingStep }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
