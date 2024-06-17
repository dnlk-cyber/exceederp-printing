﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Planning.Project;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Planning.Controllers.Project
{
    public class ProjectActualSelfcaresController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            ViewData["projects"] = db.ProjectInformation.Select(x => new { x.ProjectInformationId, ProjectName = x.Code + " - " + x.Name }).ToList();
            ViewData["projectObjective"] = db.ProjectObjectives.Select(x => new { x.ProjectObjectiveId, Name = x.ProjectObjectiveName }).ToList();
            ViewData["projectKpIs"] = db.ProjectKPIs.Select(x => new { x.ProjectKPIId, Name = x.KPIName }).ToList();
            ViewData["projectSubKpIs"] = db.ProjectSubKPIs.Select(x => new { x.ProjectSubKPIId, Name = x.SubKPIName }).ToList();
            ViewData["periods"] = db.GLPeriods.ToList().Select(x => new { x.GLPeriodId,  period = x.Name }).ToList();
            ViewData["FiscalYears"] = db.GLFiscalYears.Select(x => new { x.GlFiscalYearId, x.Name }).ToList();

            return View();
        }

        public ActionResult ProjectActualSelfcares_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectActual> projectactuals = db.ProjectActuals;
            DataSourceResult result = projectactuals.ToDataSourceResult(request, projectActual => new {
                ProjectActualId = projectActual.ProjectActualId,
                ProjectInformationId = projectActual.ProjectInformationId,
                ProjectObjectiveId = projectActual.ProjectObjectiveId,
                ProjectKPIId = projectActual.ProjectKPIId,
                ProjectSubKPIId = projectActual.ProjectSubKPIId,
                GlFiscalYearId = projectActual.GlFiscalYearId,
                Description = projectActual.Description
               
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectActualSelfcares_Create([DataSourceRequest]DataSourceRequest request, ProjectActual projectActual)
        {
            var exists = db.ProjectActuals.Where(x => x.ProjectInformationId == projectActual.ProjectInformationId && x.GlFiscalYearId == projectActual.GlFiscalYearId && x.ProjectKPIId == projectActual.ProjectKPIId && x.ProjectObjectiveId == projectActual.ProjectObjectiveId && x.ProjectSubKPIId == projectActual.ProjectSubKPIId).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "Duplication is not allowed ");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(projectActual);
                db.ProjectActuals.Add(entity);
                db.SaveChanges();
                projectActual.ProjectActualId = entity.ProjectActualId;
                PUtProjectMonthlyActual(entity.ProjectActualId, projectActual.ProjectObjectiveId, projectActual.ProjectKPIId, projectActual.ProjectSubKPIId, projectActual.ProjectInformationId, projectActual.GlFiscalYearId);
            }

            return Json(new[] { projectActual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectActualSelfcares_Update([DataSourceRequest]DataSourceRequest request, ProjectActual projectActual)
        {
            var exists = db.ProjectActuals.Where(x => x.ProjectInformationId == projectActual.ProjectInformationId && x.GlFiscalYearId == projectActual.GlFiscalYearId && x.ProjectKPIId == projectActual.ProjectKPIId && x.ProjectObjectiveId == projectActual.ProjectObjectiveId && x.ProjectSubKPIId == projectActual.ProjectSubKPIId&&x.ProjectActualId!=projectActual.ProjectActualId).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "Duplication is not allowed ");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(projectActual);
                db.ProjectActuals.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectActual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectActualSelfcares_Destroy([DataSourceRequest]DataSourceRequest request, ProjectActual projectActual)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(projectActual);
                db.ProjectActuals.Attach(entity);
                db.ProjectActuals.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectActual }.ToDataSourceResult(request, ModelState));
        }
        private ProjectActual GetMappedObject(ProjectActual projectActual)
        {
            var entity = new ProjectActual
            {
                ProjectActualId = projectActual.ProjectActualId,
                ProjectInformationId = projectActual.ProjectInformationId,
                ProjectObjectiveId = projectActual.ProjectObjectiveId,
                ProjectKPIId = projectActual.ProjectKPIId,
                ProjectSubKPIId = projectActual.ProjectSubKPIId,
                GlFiscalYearId = projectActual.GlFiscalYearId,
                Description = projectActual.Description

               
            };
            return entity;
        }
        private int PUtProjectMonthlyActual (int actualId, int objectiveId,int kpi,int subKpi,int projectId,int fiscalYearId)
        {
            var glPeriods = db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYearId).ToList();
            var planId = db.ProjectPlans.Where(x => x.ProjectInformationId == projectId && x.ProjectObjectiveId == objectiveId && x.ProjectKPIId == kpi && x.ProjectSubKPIId == subKpi && x.GlFiscalYearId == fiscalYearId).Select(x => x.ProjectPlanId);
            foreach (var period in glPeriods)
            {

                var planvalue = db.ProjectMonthlyPlans.Where(x=>planId.Contains(x.ProjectPlanId)&&x.GLPeriodId==period.GLPeriodId).FirstOrDefault();
                if(planvalue != null)
                {
                    var entity = new ProjectMonthlyActual
                    {
                        ProjectActualId = actualId,
                        GLPeriodId = period.GLPeriodId,
                        PlanValue = planvalue.Value,
                        ActualValue = 0,
                    };
                    db.ProjectMonthlyActuals.Add(entity);

                }
                else
                {
                    var entity = new ProjectMonthlyActual
                    {
                        ProjectActualId = actualId,
                        GLPeriodId = period.GLPeriodId,
                        PlanValue = 0,
                        ActualValue = 0,
                    };
                    db.ProjectMonthlyActuals.Add(entity);

                }
                db.SaveChanges();
            }

            return 0;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
