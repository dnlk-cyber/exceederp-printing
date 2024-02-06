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
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    //[AuthorizeRoles(PrintingEstimationRoles.PrintingEstimationFormUser, PrintingEstimationRoles.PrintingEstimationAdmin)]
    public class FurnitureStepController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAllFurnitureStepValues(int? FurnitureStep)
        {
            var FurnitureSteps = db.FurnitureSteps.AsEnumerable();

            if (FurnitureStep != null)
            {
                FurnitureSteps = FurnitureSteps.Where(p =>(int)p.FurnitureSteps == FurnitureStep);
            }

            return Json(FurnitureSteps.Select(p => new { FurnitureSteps = p.FurnitureSteps, FurnitureStepId = p.FurnitureStepId, Name = p.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult FurnitureSteps_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureStep> printingsteps = db.FurnitureSteps;
            DataSourceResult result = printingsteps.ToDataSourceResult(request, printingStep => new {
                FurnitureStepId = printingStep.FurnitureStepId,
                FurnitureSteps = printingStep.FurnitureSteps,
                Name = printingStep.Name,
                Description = printingStep.Description
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingSteps_Create([DataSourceRequest]DataSourceRequest request, FurnitureStep printingStep)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureStep
                {
                    FurnitureStepId = printingStep.FurnitureStepId,
                    FurnitureSteps = printingStep.FurnitureSteps,
                    Name = printingStep.Name,
                    Description = printingStep.Description
                };

                db.FurnitureSteps.Add(entity);
                db.SaveChanges();
                printingStep.FurnitureStepId = entity.FurnitureStepId;
            }

            return Json(new[] { printingStep }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingSteps_Update([DataSourceRequest]DataSourceRequest request, FurnitureStep printingStep)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureStep
                {
                    FurnitureStepId = printingStep.FurnitureStepId,
                    FurnitureSteps = printingStep.FurnitureSteps,
                    Name = printingStep.Name,
                    Description = printingStep.Description
                };

                db.FurnitureSteps.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printingStep }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingSteps_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureStep printingStep)
        {

            if (ModelState.IsValid)
            {
                var entity = new FurnitureStep
                {
                    FurnitureStepId = printingStep.FurnitureStepId,
                    FurnitureSteps = printingStep.FurnitureSteps,
                    Name = printingStep.Name,
                    Description = printingStep.Description

                };

                db.FurnitureSteps.Attach(entity);
                db.FurnitureSteps.Remove(entity);
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
