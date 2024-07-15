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
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead,
        HrmConstants.HRPerformanceUser, 
        HrmConstants.HRPerformanceAdmin, 
        HrmConstants.HREmployeeSelfService)]

    public class InternalCustomerPerformanceEvaluationsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult InternalCustomerResponses_Read([DataSourceRequest]DataSourceRequest request, int periodId)
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var orgstructure = db.Placements.FirstOrDefault(x => x.EmployeeId == id && x.isCurrent);
            int currentUserDepId = orgstructure != null ? orgstructure.OrgStructureId : 0;
            IQueryable<InternalCustomerPerformanceEvaluation> internalcustomerperformanceevaluations = db.InternalCustomerPerformanceEvaluations.Where(x => x.PerformanceRequestPeriodId == periodId && x.OrgStructureId == currentUserDepId);
            DataSourceResult result = internalcustomerperformanceevaluations.ToDataSourceResult(request, internalCustomerPerformanceEvaluation => new {
                InternalCustomerPerformanceEvaluationId = internalCustomerPerformanceEvaluation.InternalCustomerPerformanceEvaluationId,
                NumberOfEmployee = internalCustomerPerformanceEvaluation.NumberOfEmployee,
                OrgStructureId = internalCustomerPerformanceEvaluation.OrgStructureId,
                PerformanceRequestPeriodId = internalCustomerPerformanceEvaluation.PerformanceRequestPeriodId,

            });

            return Json(result);
        }
        public ActionResult InternalCustomerPerformanceEvaluations_Read([DataSourceRequest]DataSourceRequest request, int pId)
        {
            IQueryable<InternalCustomerPerformanceEvaluation> internalCustomerPerformances = db.InternalCustomerPerformanceEvaluations.Where(x=>x.PerformanceRequestPeriodId == pId);
            DataSourceResult result = internalCustomerPerformances.ToDataSourceResult(request, internalCustomerPerformance => new {

                InternalCustomerPerformanceEvaluationId = internalCustomerPerformance.InternalCustomerPerformanceEvaluationId,
                PerformanceRequestPeriodId = internalCustomerPerformance.PerformanceRequestPeriodId,
                NumberOfEmployee = internalCustomerPerformance.NumberOfEmployee,
                OrgStructureId = internalCustomerPerformance.OrgStructureId,
                
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalCustomerPerformanceEvaluations_Create([DataSourceRequest]DataSourceRequest request, InternalCustomerPerformanceEvaluation internalCustomerPerformance,int pId)
        {
            if (ModelState.IsValid)
            {
                internalCustomerPerformance.PerformanceRequestPeriodId = pId;
                var entity = GetMappedObject(internalCustomerPerformance);
                db.InternalCustomerPerformanceEvaluations.Add(entity);
                db.SaveChanges();
                internalCustomerPerformance.InternalCustomerPerformanceEvaluationId = entity.InternalCustomerPerformanceEvaluationId;
            }

            return Json(new[] { internalCustomerPerformance }.ToDataSourceResult(request, ModelState),JsonRequestBehavior.AllowGet);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalCustomerPerformanceEvaluations_Update([DataSourceRequest]DataSourceRequest request, InternalCustomerPerformanceEvaluation internalCustomerPerformance)
        {
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(internalCustomerPerformance);
                db.InternalCustomerPerformanceEvaluations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { internalCustomerPerformance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalCustomerPerformanceEvaluations_Destroy([DataSourceRequest]DataSourceRequest request, InternalCustomerPerformanceEvaluation internalCustomerPerformance)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(internalCustomerPerformance);
                db.InternalCustomerPerformanceEvaluations.Attach(entity);
                db.InternalCustomerPerformanceEvaluations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { internalCustomerPerformance }.ToDataSourceResult(request, ModelState));
        }
        private InternalCustomerPerformanceEvaluation GetMappedObject(InternalCustomerPerformanceEvaluation internalCustomerPerformance)
        {
            var entity = new InternalCustomerPerformanceEvaluation
            {
                InternalCustomerPerformanceEvaluationId = internalCustomerPerformance.InternalCustomerPerformanceEvaluationId,
                PerformanceRequestPeriodId = internalCustomerPerformance.PerformanceRequestPeriodId,
                NumberOfEmployee = internalCustomerPerformance.NumberOfEmployee,
                OrgStructureId = internalCustomerPerformance.OrgStructureId
                
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
