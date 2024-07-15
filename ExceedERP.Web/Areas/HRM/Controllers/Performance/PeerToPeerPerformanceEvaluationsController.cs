﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead,
        HrmConstants.HRPerformanceUser,
        HrmConstants.HRPerformanceAdmin,
        HrmConstants.HREmployeeSelfService)]
    public class PeerToPeerPerformanceEvaluationsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
       
        public ActionResult PeerToPeerPerformanceEvaluations_Read([DataSourceRequest]DataSourceRequest request,int pPId)
        {
            IQueryable<PeerToPeerPerformanceEvaluation> peerToPeerPerformances = db.PeerToPeerPerformanceEvaluations.Where(x=>x.PerformanceRequestPeriodId ==pPId);
            DataSourceResult result = peerToPeerPerformances.ToDataSourceResult(request, peerToPeerPerformance => new {
                PeerToPeerPerformanceEvaluationId = peerToPeerPerformance.PeerToPeerPerformanceEvaluationId,
                PerformanceRequestPeriodId = peerToPeerPerformance.PerformanceRequestPeriodId,
                EvaluatorId = peerToPeerPerformance.EvaluatorId,
                EmployeeId = peerToPeerPerformance.EmployeeId

            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PeerToPeerPerformanceEvaluations_Create([DataSourceRequest]DataSourceRequest request, PeerToPeerPerformanceEvaluation peerToPeerPerformance, int pPId)
        {
            if (peerToPeerPerformance.EmployeeId == peerToPeerPerformance.EvaluatorId)
            {
                ModelState.AddModelError("exists", "the evaluator and to be Evaluated can't be the some");

            }
            var employees = db.PeerToPeerPerformanceEvaluations.Where(x => (x.EmployeeId == peerToPeerPerformance.EmployeeId && x.EvaluatorId == peerToPeerPerformance.EvaluatorId)&&x.PerformanceRequestPeriodId==pPId).ToList();
            if (employees.Any())
            {
                ModelState.AddModelError("exists", "the employees are registered Already");
            }
            if (ModelState.IsValid)
            {
                peerToPeerPerformance.PerformanceRequestPeriodId = pPId;
                var entity = GetMappedObject(peerToPeerPerformance);
                db.PeerToPeerPerformanceEvaluations.Add(entity);
                db.SaveChanges();
                peerToPeerPerformance.PeerToPeerPerformanceEvaluationId = entity.PeerToPeerPerformanceEvaluationId;
            }

            return Json(new[] { peerToPeerPerformance }.ToDataSourceResult(request, ModelState),JsonRequestBehavior.AllowGet);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PeerToPeerPerformanceEvaluations_Update([DataSourceRequest]DataSourceRequest request, PeerToPeerPerformanceEvaluation peerToPeerPerformance)
        {
            if (peerToPeerPerformance.EmployeeId == peerToPeerPerformance.EvaluatorId)
            {
                ModelState.AddModelError("exists", "the evaluator and to be Evaluated can't be the same");

            }
            var employees = db.PeerToPeerPerformanceEvaluations.Where(x => (x.EmployeeId == peerToPeerPerformance.EmployeeId&& x.EvaluatorId == peerToPeerPerformance.EvaluatorId)
                                                                       && x.PerformanceRequestPeriodId == peerToPeerPerformance.PerformanceRequestPeriodId
                                                                       &&x.PeerToPeerPerformanceEvaluationId !=peerToPeerPerformance.PeerToPeerPerformanceEvaluationId).ToList();
            if (employees.Any())
            {
                ModelState.AddModelError("exists", "the employees are registered Already");
            }
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(peerToPeerPerformance);
                db.PeerToPeerPerformanceEvaluations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { peerToPeerPerformance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PeerToPeerPerformanceEvaluations_Destroy([DataSourceRequest]DataSourceRequest request, PeerToPeerPerformanceEvaluation peerToPeerPerformance)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(peerToPeerPerformance);
                db.PeerToPeerPerformanceEvaluations.Attach(entity);
                db.PeerToPeerPerformanceEvaluations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { peerToPeerPerformance }.ToDataSourceResult(request, ModelState));
        }
        private PeerToPeerPerformanceEvaluation GetMappedObject(PeerToPeerPerformanceEvaluation peerToPeerPerformance)
        {
            var entity = new PeerToPeerPerformanceEvaluation
            {
                PeerToPeerPerformanceEvaluationId = peerToPeerPerformance.PeerToPeerPerformanceEvaluationId,
                PerformanceRequestPeriodId = peerToPeerPerformance.PerformanceRequestPeriodId,
                EvaluatorId = peerToPeerPerformance.EvaluatorId,
                EmployeeId = peerToPeerPerformance.EmployeeId
               
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
