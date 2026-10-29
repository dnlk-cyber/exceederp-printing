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
using ExceedERP.Core.Domain.Planning;
using ExceedERP.Core.Domain.Planning.Project;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Planning.Controllers.Project
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin,
                   PlanningRoles.Planning_User,
                   HrmConstants.HRDepartmentHead,
                   HrmConstants.HREmployeeSelfService)]
    public class ProjectPlanSelfCaresController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["projects"] = db.ProjectInformation.Select(x => new { x.ProjectInformationId, ProjectName = x.Code + " - " + x.Name }).ToList();
            ViewData["projectObjective"] = db.ProjectObjectives.Select(x => new {  x.ProjectObjectiveId, Name = x.ProjectObjectiveName }).ToList();
            ViewData["projectKpIs"] = db.ProjectKPIs.Select(x => new {  x.ProjectKPIId, Name = x.KPIName }).ToList();
            ViewData["projectSubKpIs"] = db.ProjectSubKPIs.Select(x => new {  x.ProjectSubKPIId, Name = x.SubKPIName }).ToList();
            ViewData["FiscalYears"] = db.GLFiscalYears.Select(x => new { x.GlFiscalYearId, x.Name }).ToList();
            ViewData["periods"] = db.GLPeriods.ToList().Select(x => new { x.GLPeriodId, period = x.Name }).ToList();

            return View();
        }

        public ActionResult ProjectPlanSelfCares_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectPlan> projectplans = db.ProjectPlans;
            DataSourceResult result = projectplans.ToDataSourceResult(request, plan => new {
                ProjectPlanId = plan.ProjectPlanId,
                ProjectInformationId = plan.ProjectInformationId,
                ProjectObjectiveId = plan.ProjectObjectiveId,
                ProjectKPIId = plan.ProjectKPIId,
                ProjectSubKPIId = plan.ProjectSubKPIId,
                GlFiscalYearId = plan.GlFiscalYearId,
                Baseline = plan.Baseline,
                Target = plan.Target,
                ToDate = plan.ToDate,
                Plan = plan.Plan,
                Description = plan.Description
               
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectPlanSelfCares_Create([DataSourceRequest]DataSourceRequest request, ProjectPlan projectPlan)
        {
            var exists = db.ProjectPlans.Where(x => x.ProjectInformationId == projectPlan.ProjectInformationId && x.GlFiscalYearId == projectPlan.GlFiscalYearId && x.ProjectKPIId == projectPlan.ProjectKPIId && x.ProjectObjectiveId == projectPlan.ProjectObjectiveId && x.ProjectSubKPIId == projectPlan.ProjectSubKPIId).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "Duplication is not allowed ");
            }
            if (ModelState.IsValid)
            {
               
                var entity = GetMappedObject(projectPlan);
                db.ProjectPlans.Add(entity);
                db.SaveChanges();
                projectPlan.ProjectPlanId = entity.ProjectPlanId;
                PUtProjectMonthlyPlan(entity.ProjectPlanId, projectPlan.GlFiscalYearId);

            }
            return Json(new[] { projectPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectPlanSelfCares_Update([DataSourceRequest]DataSourceRequest request, ProjectPlan projectPlan)
        {
            var exists = db.ProjectPlans.Where(x => x.ProjectInformationId == projectPlan.ProjectInformationId && x.GlFiscalYearId == projectPlan.GlFiscalYearId && x.ProjectKPIId == projectPlan.ProjectKPIId && x.ProjectObjectiveId == projectPlan.ProjectObjectiveId && x.ProjectSubKPIId == projectPlan.ProjectSubKPIId&&x.ProjectPlanId != projectPlan.ProjectPlanId).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "Duplication is not allowed ");
            }
            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(projectPlan);
                db.ProjectPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectPlanSelfCares_Destroy([DataSourceRequest]DataSourceRequest request, ProjectPlan projectPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(projectPlan);
                db.ProjectPlans.Attach(entity);
                db.ProjectPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectPlan }.ToDataSourceResult(request, ModelState));
        }
        private  int  PUtProjectMonthlyPlan(int planId,int fiscalYearId)
        {
            var glPeriods = db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYearId).ToList();
           
            foreach(var period in glPeriods)
            {

                var entity = new ProjectMonthlyPlan
                {
                    ProjectPlanId = planId,
                    GLPeriodId = period.GLPeriodId,
                    Value = 0,
                };

                db.ProjectMonthlyPlans.Add(entity);
                db.SaveChanges();
            }
            return 0;
        }
        private ProjectPlan GetMappedObject(ProjectPlan projectPlan)
        {
            var entity = new ProjectPlan
            {
                ProjectPlanId = projectPlan.ProjectPlanId,
                ProjectInformationId = projectPlan.ProjectInformationId,
                ProjectObjectiveId = projectPlan.ProjectObjectiveId,
                ProjectKPIId = projectPlan.ProjectKPIId,
                ProjectSubKPIId = projectPlan.ProjectSubKPIId,
                GlFiscalYearId = projectPlan.GlFiscalYearId,
                Baseline = projectPlan.Baseline,
                Target = projectPlan.Target,
                ToDate = projectPlan.ToDate,
                Plan = projectPlan.Plan,
                Description = projectPlan.Description
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
