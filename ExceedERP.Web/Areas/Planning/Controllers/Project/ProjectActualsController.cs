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
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using System.IO;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.Planning.Controllers.Project
{
    public class ProjectActualsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            var fiscalYears = from f in db.GLFiscalYears join
                             p in db.ProjectPlans  on f.GlFiscalYearId equals p.GlFiscalYearId
                              select new
                              {
                                  f.GlFiscalYearId,
                                  f.Name,

                              };
            ViewData["projects"] = db.ProjectInformation.Select(x => new { x.ProjectInformationId, ProjectName = x.Code + " - " + x.Name }).ToList();
            ViewData["projectObjective"] = db.ProjectObjectives.Select(x => new { x.ProjectObjectiveId, Name = x.ProjectObjectiveName }).ToList();
            ViewData["projectKpIs"] = db.ProjectKPIs.Select(x => new { x.ProjectKPIId, Name = x.KPIName }).ToList();
            ViewData["projectSubKpIs"] = db.ProjectSubKPIs.Select(x => new { x.ProjectSubKPIId, Name = x.SubKPIName }).ToList();
            ViewData["periods"] = db.GLPeriods.ToList().Select(x => new { x.GLPeriodId,  period = x.Name }).ToList();
            ViewData["FiscalYears"] = fiscalYears.DistinctByField(x=>x.GlFiscalYearId).ToList();

            return View();
        }
        private int GetProjectPlanId(int projectId,int objId,int kpiId,int subKpiId,int fiscalyearId)
        {
            var projectPlan = db.ProjectPlans.Where(x => x.ProjectInformationId == projectId
                                                       && x.ProjectObjectiveId == objId
                                                       && x.ProjectKPIId == kpiId
                                                       && x.ProjectSubKPIId == subKpiId
                                                       && x.GlFiscalYearId == fiscalyearId).FirstOrDefault();
            return  projectPlan != null? projectPlan.ProjectPlanId : 0;
              
        }
        public ActionResult ProjectActuals_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectActual> projectactuals = db.ProjectActuals;
            DataSourceResult result = projectactuals.ToDataSourceResult(request, projectActual => new {
                ProjectActualId = projectActual.ProjectActualId,
                ProjectInformationId = projectActual.ProjectInformationId,
                ProjectObjectiveId = projectActual.ProjectObjectiveId,
                ProjectKPIId = projectActual.ProjectKPIId,
                ProjectSubKPIId = projectActual.ProjectSubKPIId,
                GlFiscalYearId = projectActual.GlFiscalYearId,
                Description = projectActual.Description,
                ProjectPlanId = GetProjectPlanId(projectActual.ProjectInformationId,
                                                         projectActual.ProjectObjectiveId,
                                                         projectActual.ProjectKPIId,
                                                         projectActual.ProjectSubKPIId,
                                                         projectActual.GlFiscalYearId)

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectActuals_Create([DataSourceRequest]DataSourceRequest request, ProjectActual projectActual)
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
                PUtProjectMonthlyActual(entity.ProjectActualId,projectActual.ProjectObjectiveId, projectActual.ProjectKPIId, projectActual.ProjectSubKPIId, projectActual.ProjectInformationId, projectActual.GlFiscalYearId);
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Project Actual", "Success", User.Identity.Name, entity.FiscalYear.Name +" is Created By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { projectActual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectActuals_Update([DataSourceRequest]DataSourceRequest request, ProjectActual projectActual)
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
                #region operation log
                var fiscalyear = db.GLFiscalYears.Find(projectActual.GlFiscalYearId);
                if(fiscalyear != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Project Actual", "Success", User.Identity.Name, entity.FiscalYear.Name + " is Updated By  " + User.Identity.Name, Modules.Planning);

                }
                #endregion
            }

            return Json(new[] { projectActual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectActuals_Destroy([DataSourceRequest]DataSourceRequest request, ProjectActual projectActual)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(projectActual);
                #region delete log
                var yearDelete = db.GLFiscalYears.Find(projectActual.GlFiscalYearId);
                var delete = yearDelete.Name;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "    Fiscal year " + delete);
                #endregion

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
        public ActionResult UploadProjectActualDocument(int projectActualId, HttpPostedFileBase file,string txtValue)
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

                    Reference = projectActualId.ToString(),
                    Description = projectActualId.ToString() + "-" + "-" + newFileName+"-"+ txtValue,
                    FileName = newFileName,
                    FileType = file.ContentType,
                    ReferenceType = AttachmentType.DOCUMENT,
                    Index = 0,
                    //URL = path,
                    URL = filePath,
                    Remark = projectActualId.ToString()
                };
                db.Attachments.Add(entity);

                db.SaveChanges();
                ViewBag.Message = "Upload successful";
            }
            return RedirectToAction("Index", "ProjectActuals");
        }

        public ActionResult DownloadDoument(string AttachmentId)
        {
            int.TryParse(AttachmentId, out int attachmentId);

            var record = db.Attachments.Find(attachmentId);

            return File(record.URL, "application/force-download", Path.GetFileName(record.URL));
        }
        public JsonResult RefreshToAddNewAddedItems(int id)
        {
            int count = 0;
            var record = db.ProjectActuals.Find(id);
            if (record != null)
            {
                #region validate item Addition

                var actuals = db.ProjectMonthlyActuals.Where(x => x.ProjectActualId == record.ProjectActualId).ToList();
                foreach (var actual in actuals)
                {
                    var plan = db.ProjectMonthlyPlans.Where(x => x.GLPeriodId == actual.GLPeriodId 
                                                                    && x.GetPlan.ProjectInformationId == actual.Actual.ProjectInformationId
                                                                    && x.GetPlan.GlFiscalYearId == actual.Actual.GlFiscalYearId
                                                                    && x.GetPlan.ProjectKPIId == actual.Actual.ProjectKPIId
                                                                    && x.GetPlan.ProjectSubKPIId == actual.Actual.ProjectSubKPIId
                                                                    ).FirstOrDefault();

                    if (plan != null&&actual.PlanValue != plan.Value&& plan.Value !=0)
                    {
                        actual.PlanValue = plan.Value;
                        db.Entry(actual).State = EntityState.Modified;
                        db.SaveChanges();
                        count++;
                    }

                }
                if (count > 0)
                {
                    var formattedData = new
                    {
                        Message = "plan value has updated  Successfully .",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = " no change .",
                        Status = "OKe"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion
            }
            var formattedData12 = new
            {
                Message = "The record is not found.",
                Status = "Not_OKe"
            };
            return Json(formattedData12, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
