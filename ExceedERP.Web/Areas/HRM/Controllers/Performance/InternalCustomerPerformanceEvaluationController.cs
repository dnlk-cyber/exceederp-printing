﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    public class InternalCustomerPerformanceEvaluationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InternalCustomerPerformanceEvaluations_Read([DataSourceRequest]DataSourceRequest request,int pId)
        {
            IQueryable<InternalCustomerPerformanceEvaluation> internalcustomerperformanceevaluations = db.InternalCustomerPerformanceEvaluations.Where(x=>x.PerformanceRequestPeriodId == pId);
            DataSourceResult result = internalcustomerperformanceevaluations.ToDataSourceResult(request, internalCustomerPerformanceEvaluation => new {
                InternalCustomerPerformanceEvaluationId = internalCustomerPerformanceEvaluation.InternalCustomerPerformanceEvaluationId,
                NumberOfEmployee = internalCustomerPerformanceEvaluation.NumberOfEmployee,
                OrgStructureId = internalCustomerPerformanceEvaluation.OrgStructureId,
                PerformanceRequestPeriodId = internalCustomerPerformanceEvaluation.PerformanceRequestPeriodId,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalCustomerPerformanceEvaluations_Create([DataSourceRequest]DataSourceRequest request, InternalCustomerPerformanceEvaluation internalCustomerPerformanceEvaluation,int pId)
        {
            if (ModelState.IsValid)
            {
                var entity = new InternalCustomerPerformanceEvaluation
                {
                    NumberOfEmployee = internalCustomerPerformanceEvaluation.NumberOfEmployee,
                    OrgStructureId = internalCustomerPerformanceEvaluation.OrgStructureId,
                    PerformanceRequestPeriodId = pId,


                };

                db.InternalCustomerPerformanceEvaluations.Add(entity);
                db.SaveChanges();
                internalCustomerPerformanceEvaluation.InternalCustomerPerformanceEvaluationId = entity.InternalCustomerPerformanceEvaluationId;
            }

            return Json(new[] { internalCustomerPerformanceEvaluation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalCustomerPerformanceEvaluations_Update([DataSourceRequest]DataSourceRequest request, InternalCustomerPerformanceEvaluation internalCustomerPerformanceEvaluation)
        {
            if (ModelState.IsValid)
            {
                var entity = new InternalCustomerPerformanceEvaluation
                {
                    InternalCustomerPerformanceEvaluationId = internalCustomerPerformanceEvaluation.InternalCustomerPerformanceEvaluationId,
                    NumberOfEmployee = internalCustomerPerformanceEvaluation.NumberOfEmployee,
                    OrgStructureId = internalCustomerPerformanceEvaluation.OrgStructureId,
                    PerformanceRequestPeriodId = internalCustomerPerformanceEvaluation.PerformanceRequestPeriodId,

                };

                db.InternalCustomerPerformanceEvaluations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { internalCustomerPerformanceEvaluation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalCustomerPerformanceEvaluations_Destroy([DataSourceRequest]DataSourceRequest request, InternalCustomerPerformanceEvaluation internalCustomerPerformanceEvaluation)
        {
            if (ModelState.IsValid)
            {
                var entity = new InternalCustomerPerformanceEvaluation
                {
                    InternalCustomerPerformanceEvaluationId = internalCustomerPerformanceEvaluation.InternalCustomerPerformanceEvaluationId,
                    NumberOfEmployee = internalCustomerPerformanceEvaluation.NumberOfEmployee,
                    OrgStructureId = internalCustomerPerformanceEvaluation.OrgStructureId,
                    PerformanceRequestPeriodId = internalCustomerPerformanceEvaluation.PerformanceRequestPeriodId,

                };

                db.InternalCustomerPerformanceEvaluations.Attach(entity);
                db.InternalCustomerPerformanceEvaluations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { internalCustomerPerformanceEvaluation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
