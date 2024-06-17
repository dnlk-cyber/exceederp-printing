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
using ExceedERP.Core.Domain.Planning.Project;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Common.HRM;
using System.IO;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.Planning.Controllers.Project
{
    public class ProjectPlansController : Controller
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

        public ActionResult ProjectPlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectPlan> projectplans = db.ProjectPlans;
            DataSourceResult result = projectplans.ToDataSourceResult(request, projectPlan => new {
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
               
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectPlans_Create([DataSourceRequest]DataSourceRequest request, ProjectPlan projectPlan)
        {
            var exists = db.ProjectPlans.Where(x => x.ProjectInformationId == projectPlan.ProjectInformationId
                                                    && x.GlFiscalYearId == projectPlan.GlFiscalYearId
                                                    && x.ProjectObjectiveId == projectPlan.ProjectObjectiveId
                                                    && x.ProjectKPIId == projectPlan.ProjectKPIId 
                                                    && x.ProjectSubKPIId == projectPlan.ProjectSubKPIId).Any();
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
                #region operation log
                var fiscaYears = db.GLFiscalYears.Find(projectPlan.GlFiscalYearId);
                if(fiscaYears != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Project Actual", "Success", User.Identity.Name, fiscaYears.Name + " is Created By  " + User.Identity.Name, Modules.Planning);

                }

                #endregion

            }
            return Json(new[] { projectPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectPlans_Update([DataSourceRequest]DataSourceRequest request, ProjectPlan projectPlan)
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
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Project Actual", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion
            }

            return Json(new[] { projectPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectPlans_Destroy([DataSourceRequest]DataSourceRequest request, ProjectPlan projectPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(projectPlan);
                db.ProjectPlans.Attach(entity);
                db.ProjectPlans.Remove(entity);
                db.SaveChanges();
                #region delete log
                var yearDelete = db.GLFiscalYears.Find(projectPlan.GlFiscalYearId);
                var delete = yearDelete.Name;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "    Fiscal year " + delete);
                #endregion

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
        public ActionResult UploadProjectPlanDocument(int projectPlanId, HttpPostedFileBase file)
        {
            if (file != null)
            {
                string path = HttpContext.Server.MapPath("~/Content/Files/Planning/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var newFileName = Path.GetFileName(file.FileName);
                //var logoFileName = file.FileName;
                var filePath = path + newFileName;
                file.SaveAs(filePath);
                var entity = new Attachment
                {

                    Reference = projectPlanId.ToString(),
                    Description = projectPlanId.ToString() + "-" + "-" + newFileName,
                    FileName = newFileName,
                    FileType = file.ContentType,
                    ReferenceType = AttachmentType.DOCUMENT,
                    Index = 0,
                    //URL = path,
                    URL = filePath,
                    Remark = projectPlanId.ToString()
                };
                db.Attachments.Add(entity);

                db.SaveChanges();
                ViewBag.Message = "Upload successful";
            }
            return RedirectToAction("Index", "ProjectPlans");
        }

        public ActionResult DownloadDoument(string id)
        {
            int.TryParse(id, out int attachmentId);

            var record = db.Attachments.Find(attachmentId);

            return File(record.URL, "application/force-download", Path.GetFileName(record.URL));
        }
        //public ActionResult DownloadDoument(string attachId)
        //{
        //    int.TryParse(attachId, out int attachmentId);
        //    var record = db.Attachments.Find(attachmentId);

        //    return File(record.URL, "application/force-download", Path.GetFileName(record.URL));
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
