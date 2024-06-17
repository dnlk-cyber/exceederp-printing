﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin)]
    public class RiskFollowUpsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult RiskFollowUps_Read([DataSourceRequest]DataSourceRequest request,int rId)
        {
            IQueryable<RiskFollowUp> riskFellowUps = db.RiskFollowUps.Where(x=>x.RiskMitigationPlanId==rId);
            DataSourceResult result = riskFellowUps.ToDataSourceResult(request, riskFellowUp => new {
                RiskFollowUpId = riskFellowUp.RiskFollowUpId,
                RiskMitigationPlanId = riskFellowUp.RiskMitigationPlanId,
                PlanDate = riskFellowUp.PlanDate,
                Description = riskFellowUp.Description,
                TreatmentMethod = riskFellowUp.TreatmentMethod,
                PlannedActionToBeTaken = riskFellowUp.PlannedActionToBeTaken,
                ActualActionTaken = riskFellowUp.ActualActionTaken,
                CurrentStatus = riskFellowUp.CurrentStatus
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RiskFollowUps_Create([DataSourceRequest]DataSourceRequest request, RiskFollowUp riskFollowUp,int rId)
        {
            if (ModelState.IsValid)
            {
                riskFollowUp.RiskMitigationPlanId = rId;
                var entity = GetMappedObject(riskFollowUp);

                db.RiskFollowUps.Add(entity);
                db.SaveChanges();
                riskFollowUp.RiskFollowUpId = entity.RiskFollowUpId;
            }

            return Json(new[] { riskFollowUp }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RiskFollowUps_Update([DataSourceRequest]DataSourceRequest request, RiskFollowUp riskFellowUp)
        {
            if (ModelState.IsValid)
            {
               
                var entity = GetMappedObject(riskFellowUp);
                db.RiskFollowUps.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { riskFellowUp }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RiskFollowUps_Destroy([DataSourceRequest]DataSourceRequest request, RiskFollowUp riskFellowUp)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(riskFellowUp);
                db.RiskFollowUps.Attach(entity);
                db.RiskFollowUps.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { riskFellowUp }.ToDataSourceResult(request, ModelState));
        }
        private RiskFollowUp GetMappedObject(RiskFollowUp riskFellowUp)
        {
            var entity = new RiskFollowUp
            {
                RiskFollowUpId = riskFellowUp.RiskFollowUpId,
                RiskMitigationPlanId = riskFellowUp.RiskMitigationPlanId,
                PlanDate = riskFellowUp.PlanDate,
                Description = riskFellowUp.Description,
                TreatmentMethod = riskFellowUp.TreatmentMethod,
                PlannedActionToBeTaken = riskFellowUp.PlannedActionToBeTaken,
                ActualActionTaken = riskFellowUp.ActualActionTaken,
                CurrentStatus = riskFellowUp.CurrentStatus

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
