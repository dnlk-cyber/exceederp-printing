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
using ExceedERP.Helpers.Planning;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [Authorize]
    public class PlanningDetailsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly PlanningApprovalHelper auth;

        private const string NotEditableMessage =
            "You can change figures only on your own work unit's plan, and only while it is a draft or has been returned for revision.";

        public PlanningDetailsController()
        {
            auth = new PlanningApprovalHelper(db);
        }

        public ActionResult Index()
        {
            return View();
        }
      
        public ActionResult PlanningDetails_Read([DataSourceRequest]DataSourceRequest request,int pid)
        {
            if (!CanViewPlan(pid))
                return Json(Enumerable.Empty<PlanningDetail>().ToDataSourceResult(request));

            IQueryable<PlanningDetail> planningdetails = db.PlanningDetails.Where(p=>p.BSCPlanId==pid&&p.ParentId==0&&p.GetIndicator.IsActive);
            DataSourceResult result = planningdetails.ToDataSourceResult(request, planningDetail => new {
                PlanningDetailId = planningDetail.PlanningDetailId,
                BSCPlanId = planningDetail.BSCPlanId,
                ParentId = planningDetail.ParentId,
                GlobalIndicatorId = planningDetail.GlobalIndicatorId,
                Unit = planningDetail.Unit,
                KPIType = planningDetail.KPIType,
                IndicatorId = planningDetail.IndicatorId,
                HasSubIndicator = planningDetail.HasSubIndicator,
                Plan = planningDetail.Plan,
                Baseline = planningDetail.Baseline,
                Target = planningDetail.Target
            });

            return Json(result);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult PlanningDetails_Create([DataSourceRequest]DataSourceRequest request, PlanningDetail planningDetail,int pid)
        {
            if (IsPlanLocked(pid))
                ModelState.AddModelError(string.Empty, NotEditableMessage);

            if (ModelState.IsValid)
            {
                planningDetail.BSCPlanId = pid;
                var entity = GetMappedObject(planningDetail);
                db.PlanningDetails.Add(entity);
                db.SaveChanges();
                planningDetail.PlanningDetailId = entity.PlanningDetailId;
            }

            return Json(new[] { planningDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult PlanningDetails_Update([DataSourceRequest]DataSourceRequest request, PlanningDetail planningDetail)
        {
            var entity = db.PlanningDetails.Find(planningDetail.PlanningDetailId);
            if (entity == null)
                ModelState.AddModelError(string.Empty, "The planning detail was not found.");
            else if (IsPlanLocked(entity.BSCPlanId))
                ModelState.AddModelError(string.Empty, NotEditableMessage);

            if (ModelState.IsValid)
            {
                entity.Plan = planningDetail.Plan;
                entity.Baseline = planningDetail.Baseline;
                entity.Target = planningDetail.Target;
                db.SaveChanges();
            }

            return Json(new[] { planningDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult PlanningDetails_Destroy([DataSourceRequest]DataSourceRequest request, PlanningDetail planningDetail)
        {
            var entity = db.PlanningDetails.Find(planningDetail.PlanningDetailId);
            if (entity == null)
                ModelState.AddModelError(string.Empty, "The planning detail was not found.");
            else if (IsPlanLocked(entity.BSCPlanId))
                ModelState.AddModelError(string.Empty, NotEditableMessage);

            //var monthlyDetails = db.MonthlyPlans.Where(x => x.PlanningDetailId == planningDetail.PlanningDetailId).ToList();
            //if (monthlyDetails.Any())
            //{
            //    ModelState.AddModelError("exists", "You Can't delete this! you have to delete child Data first");
            //}

            if (ModelState.IsValid)
            {
                db.PlanningDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { planningDetail }.ToDataSourceResult(request, ModelState));
        }
        private PlanningDetail GetMappedObject(PlanningDetail planningDetail)
        {
            var entity = new PlanningDetail
            {
                PlanningDetailId = planningDetail.PlanningDetailId,
                BSCPlanId = planningDetail.BSCPlanId,
                ParentId = planningDetail.ParentId,
                Unit = planningDetail.Unit,
                KPIType = planningDetail.KPIType,
                IndicatorId = planningDetail.IndicatorId,
                GlobalIndicatorId = planningDetail.GlobalIndicatorId,
                HasSubIndicator = planningDetail.HasSubIndicator,
                Plan = planningDetail.Plan,
                Baseline = planningDetail.Baseline,
                Target = planningDetail.Target
            };
            return entity;
        }

        private bool IsPlanLocked(int bscPlanId)
        {
            var plan = db.BSCPlans.Find(bscPlanId);
            var placement = auth.GetCurrentPlacement(UserInformationHelper.GetEmployeeId(User.Identity.Name));
            return !auth.CanEditFigures(plan, placement);
        }

        private bool CanViewPlan(int bscPlanId)
        {
            var plan = db.BSCPlans.Find(bscPlanId);
            if (plan != null && PlanningSelfCareHelper.CanViewAllWorkUnits(User))
                return true;

            var placement = auth.GetCurrentPlacement(UserInformationHelper.GetEmployeeId(User.Identity.Name));
            return plan != null && placement != null && auth.CanViewPlan(plan, placement);
        }
       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
