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
    [AuthorizeRoles(PlanningRoles.Planning_Admin,PlanningRoles.Planning_Risk_User)]
    public class RiskMitigationPlansController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RiskMitigationPlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RiskMitigationPlan> riskmitigationplans = db.RiskMitigationPlans;
            DataSourceResult result = riskmitigationplans.ToDataSourceResult(request, riskMitigationPlan => new {
                RiskMitigationPlanId = riskMitigationPlan.RiskMitigationPlanId,
                IdentifiedRisk = riskMitigationPlan.IdentifiedRisk,
                RiskLevel = riskMitigationPlan.RiskLevel,
                TreatmentMethod = riskMitigationPlan.TreatmentMethod,
                ActionToBeTaken = riskMitigationPlan.ActionToBeTaken,
                MainResponsibly = riskMitigationPlan.MainResponsibly,
                SupportResponsibly = riskMitigationPlan.SupportResponsibly,
                Opportunity = riskMitigationPlan.Opportunity,
                Time = riskMitigationPlan.Time,
                MonitoringMethod = riskMitigationPlan.MonitoringMethod,
                Evaluation = riskMitigationPlan.Evaluation,
                PlanDate = riskMitigationPlan.PlanDate,
                IsClosed = riskMitigationPlan.IsClosed,
                DateCreated = riskMitigationPlan .DateCreated,
                LastModified = riskMitigationPlan.LastModified,
                CreatedBy = riskMitigationPlan.CreatedBy,
                ModifiedBy = riskMitigationPlan.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RiskMitigationPlans_Create([DataSourceRequest]DataSourceRequest request, RiskMitigationPlan riskMitigationPlan)
        {
            if (ModelState.IsValid)
            {
                riskMitigationPlan.DateCreated = DateTime.Now.Date;
                riskMitigationPlan.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(riskMitigationPlan);

                db.RiskMitigationPlans.Add(entity);
                db.SaveChanges();
                riskMitigationPlan.RiskMitigationPlanId = entity.RiskMitigationPlanId;
            }

            return Json(new[] { riskMitigationPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RiskMitigationPlans_Update([DataSourceRequest]DataSourceRequest request, RiskMitigationPlan riskMitigationPlan)
        {
            if (ModelState.IsValid)
            {
                riskMitigationPlan.LastModified = DateTime.Now.Date;
                riskMitigationPlan.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(riskMitigationPlan);

                db.RiskMitigationPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { riskMitigationPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RiskMitigationPlans_Destroy([DataSourceRequest]DataSourceRequest request, RiskMitigationPlan riskMitigationPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(riskMitigationPlan);
                db.RiskMitigationPlans.Attach(entity);
                db.RiskMitigationPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { riskMitigationPlan }.ToDataSourceResult(request, ModelState));
        }
        private RiskMitigationPlan GetMappedObject(RiskMitigationPlan riskMitigationPlan)
        {
            var entity = new RiskMitigationPlan
            {
                RiskMitigationPlanId = riskMitigationPlan.RiskMitigationPlanId,
                IdentifiedRisk = riskMitigationPlan.IdentifiedRisk,
                RiskLevel = riskMitigationPlan.RiskLevel,
                TreatmentMethod = riskMitigationPlan.TreatmentMethod,
                ActionToBeTaken = riskMitigationPlan.ActionToBeTaken,
                MainResponsibly = riskMitigationPlan.MainResponsibly,
                SupportResponsibly = riskMitigationPlan.SupportResponsibly,
                Opportunity = riskMitigationPlan.Opportunity,
                Time = riskMitigationPlan.Time,
                MonitoringMethod = riskMitigationPlan.MonitoringMethod,
                Evaluation = riskMitigationPlan.Evaluation,
                PlanDate = riskMitigationPlan.PlanDate,
                IsClosed = riskMitigationPlan.IsClosed,
                DateCreated = riskMitigationPlan.DateCreated,
                LastModified = riskMitigationPlan.LastModified,
                CreatedBy = riskMitigationPlan.CreatedBy,
                ModifiedBy = riskMitigationPlan.ModifiedBy
            };
            return entity;
        }

       public JsonResult CloseRiskMitigation(int id)
        {
            var mitigation = db.RiskMitigationPlans.Find(id);
            if(mitigation != null)
            {
                if (mitigation.IsClosed)
                {
                    var formattedData1 = new
                    {
                        Message = "already closed you can't close again.",
                        Status = "OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    mitigation.IsClosed = true;
                    db.Entry(mitigation).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedData1 = new
                    {
                        Message = "Succesfully closed ",
                        Status = "OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                }
            }
            var formattedData = new
            {
                Message = "Record not found.",
                Status = "NOT-OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
