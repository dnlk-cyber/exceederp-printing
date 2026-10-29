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
    [Authorize]
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

                var fiscalYear = db.GLFiscalYears.Find(projectActual.GlFiscalYearId);
                UserHelper.OperationLog(Request.UserHostAddress, "Project Actual", "Success", User.Identity.Name, (fiscalYear == null ? "Fiscal year" : fiscalYear.Name) + " is Created By  " + User.Identity.Name, Modules.Planning);
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
                    UserHelper.OperationLog(Request.UserHostAddress, "Project Actual", "Success", User.Identity.Name, fiscalyear.Name + " is Updated By  " + User.Identity.Name, Modules.Planning);

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

        public ActionResult PlanAttachments_Read([DataSourceRequest] DataSourceRequest request, int projectActualId)
        {
            var projectPlanId = FindProjectPlanId(projectActualId);
            var attachments = projectPlanId.HasValue
                ? db.Attachments.Where(x => x.Reference == projectPlanId.Value.ToString() && x.ReferenceType == AttachmentType.DOCUMENT)
                : Enumerable.Empty<Attachment>().AsQueryable();

            return Json(attachments.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult PlanAttachment_Create([DataSourceRequest] DataSourceRequest request, int projectActualId, HttpPostedFileBase file, string txtValue)
        {
            var projectPlanId = FindProjectPlanId(projectActualId);
            if (!projectPlanId.HasValue)
            {
                ModelState.AddModelError("File", "No matching project plan was found.");
            }
            else if (file == null || file.ContentLength == 0)
            {
                ModelState.AddModelError("File", "Select a file to upload.");
            }
            else if (ModelState.IsValid)
            {
                var directory = Server.MapPath("~/Content/Files/Planning/");
                Directory.CreateDirectory(directory);

                var fileName = Path.GetFileName(file.FileName);
                var storedFileName = Guid.NewGuid().ToString("N") + "_" + fileName;
                var filePath = Path.Combine(directory, storedFileName);
                file.SaveAs(filePath);

                db.Attachments.Add(new Attachment
                {
                    Reference = projectPlanId.Value.ToString(),
                    Description = string.IsNullOrWhiteSpace(txtValue) ? fileName : txtValue,
                    FileName = fileName,
                    FileType = file.ContentType,
                    ReferenceType = AttachmentType.DOCUMENT,
                    Index = 0,
                    URL = filePath,
                    Remark = projectActualId.ToString()
                });
                db.SaveChanges();
            }

            return Json(new object[0].ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult PlanAttachment_Destroy([DataSourceRequest] DataSourceRequest request, Attachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = db.Attachments.Find(attachment.AttachmentId);
                if (entity != null)
                {
                    if (System.IO.File.Exists(entity.URL))
                    {
                        System.IO.File.Delete(entity.URL);
                    }

                    db.Attachments.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        private int? FindProjectPlanId(int projectActualId)
        {
            var actual = db.ProjectActuals.Find(projectActualId);
            if (actual == null)
            {
                return null;
            }

            return db.ProjectPlans
                .Where(x => x.ProjectInformationId == actual.ProjectInformationId
                    && x.ProjectObjectiveId == actual.ProjectObjectiveId
                    && x.ProjectKPIId == actual.ProjectKPIId
                    && x.ProjectSubKPIId == actual.ProjectSubKPIId
                    && x.GlFiscalYearId == actual.GlFiscalYearId)
                .Select(x => (int?)x.ProjectPlanId)
                .FirstOrDefault();
        }

        public ActionResult DownloadDoument(string AttachmentId)
        {
            int.TryParse(AttachmentId, out int attachmentId);

            var record = db.Attachments.Find(attachmentId);

            return File(record.URL, "application/force-download", Path.GetFileName(record.URL));
        }
        public JsonResult RefreshToAddNewAddedItems(int id)
        {
            var actualRecord = db.ProjectActuals.Find(id);
            if (actualRecord == null)
            {
                return Json(new
                {
                    Message = "The record was not found.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            var monthlyActuals = db.ProjectMonthlyActuals
                .Where(x => x.ProjectActualId == actualRecord.ProjectActualId)
                .ToList();

            var modified = false;

            foreach (var actual in monthlyActuals)
            {
                var plan = db.ProjectMonthlyPlans.FirstOrDefault(p =>
                    p.GLPeriodId == actual.GLPeriodId &&
                    p.GetPlan.ProjectInformationId == actual.Actual.ProjectInformationId &&
                    p.GetPlan.GlFiscalYearId == actual.Actual.GlFiscalYearId &&
                    p.GetPlan.ProjectKPIId == actual.Actual.ProjectKPIId &&
                    p.GetPlan.ProjectSubKPIId == actual.Actual.ProjectSubKPIId);

                if (plan != null && actual.PlanValue != plan.Value && plan.Value != 0)
                {
                    actual.PlanValue = plan.Value;
                    db.Entry(actual).State = EntityState.Modified;
                    modified = true;
                }
            }

            if (modified)
            {
                db.SaveChanges(); // Save once after all modifications
                return Json(new
                {
                    Message = "Plan value(s) updated successfully.",
                    Status = "OK"
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Message = "No changes were made.",
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
