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
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;
using System.IO;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User)]
    public class PlanSelfCaresController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var objectives = db.Objectives.Select(O => new
            {
                O.ObjectiveId,
                O.Name,
                O.Description

            }).ToList();
            var indicators = db.Indicators.Select(I => new
            {
                I.IndicatorId,
                I.Name,
                I.Description,
                I.KPIType
            }).ToList();
            var glFiscalYears = db.GLFiscalYears.Select(g => new
            {
                g.GlFiscalYearId,
                g.Name,

            }).ToList();
            ViewData["Objectives"] = objectives;
            ViewData["Indicators"] = indicators;
            ViewData["GlFiscalYears"] = glFiscalYears;
            ViewData["GlobalIndicators"] = db.GlobalIndicators.Select(x => new { x.GlobalIndicatorId, x.Indicator });

            ViewData["periods"] = db.GLPeriods.ToList().Select(x => new { x.GLPeriodId, period = x.Name }).ToList();
            ViewData["departmentLists"] = db.MainCompanyStructures.ToList();

            return View();
        }

        public ActionResult PlanSelfCares_Read([DataSourceRequest]DataSourceRequest request)
        {
            //var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //var workUnitId = EmployeeHelper.GetEmployeeWorkUnitId(employeeId);
            //var planIds = db.BSCPlans.Where(x => x.OrgStructureId == workUnitId).Select(x => x.PlanId);
            IQueryable<Plan> planningfiscalyears = db.Plans/*.Where(x=>planIds.Contains(x.PlanId))*/;
            DataSourceResult result = planningfiscalyears.ToDataSourceResult(request, planningFiscalYear => new {
                PlanId = planningFiscalYear.PlanId,
                GlFiscalYearId = planningFiscalYear.GlFiscalYearId,
                IsPersonal = planningFiscalYear.IsPersonal,
                IsDepartnental = planningFiscalYear.IsDepartnental,
                DateCreated = planningFiscalYear.DateCreated,
                CreatedBy = planningFiscalYear.CreatedBy,
                LastModified = planningFiscalYear.LastModified,
                ModifiedBy = planningFiscalYear.ModifiedBy,
                Description = planningFiscalYear.Description
            });

            return Json(result);
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlanSelfCares_Create([DataSourceRequest]DataSourceRequest request, Plan plan)
        {
            var exists = db.Plans.Any(s => s.GlFiscalYearId ==plan.GlFiscalYearId);
            if (exists)
            {
                ModelState.AddModelError("exists", "This is already Registered ");
            }
            if (ModelState.IsValid)
            {
                plan.DateCreated = DateTime.Now.Date;
                plan.CreatedBy = User.Identity.Name;
                plan.IsPersonal = true;
                var entity = GetMappedObject(plan);
                db.Plans.Add(entity);
                db.SaveChanges();
                plan.PlanId = entity.PlanId;
                #region operation log
                var fiscalYear = db.GLFiscalYears.Find(plan.GlFiscalYearId);
                if (fiscalYear != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Plan", "Success", User.Identity.Name, fiscalYear.Name + " is Created By  " + User.Identity.Name, Modules.Planning);

                }

                #endregion
            }

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlanSelfCares_Update([DataSourceRequest]DataSourceRequest request, Plan plan)
        {
            var exists = db.Plans.Any(s => s.GlFiscalYearId == plan.GlFiscalYearId && s.PlanId != plan.PlanId);
            if (exists)
            {
                ModelState.AddModelError("exists", "This is already registered ");
            }
            if (ModelState.IsValid)
            {
                plan.LastModified = DateTime.Now.Date;
                plan.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(plan);
                db.Plans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                var fiscalYear = db.GLFiscalYears.Find(plan.GlFiscalYearId);
                if (fiscalYear != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Plan", "Success", User.Identity.Name, fiscalYear.Name + " is Updated By  " + User.Identity.Name, Modules.Planning);

                }
                #endregion
            }

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlanSelfCares_Destroy([DataSourceRequest]DataSourceRequest request, Plan plan)
        {
            var bscPlans = db.BSCPlans.Where(x => x.PlanId == plan.PlanId).ToList();
            if (bscPlans.Any())
            {
                ModelState.AddModelError("exists", "You Can't delete this ! you have to delete child Data first");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(plan);
                #region delete log
                var yearDelete = db.GLFiscalYears.Find(plan.GlFiscalYearId);
                var delete = yearDelete.Name;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "    Fiscal year " + delete);
                #endregion
                db.Plans.Attach(entity);
                db.Plans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }
        private Plan GetMappedObject(Plan plan)
        {
            var entity = new Plan
            {
                PlanId = plan.PlanId,
                GlFiscalYearId = plan.GlFiscalYearId,
                IsPersonal = plan.IsPersonal,
                DateCreated = plan.DateCreated,
                CreatedBy = plan.CreatedBy,
                LastModified = plan.LastModified,
                ModifiedBy = plan.ModifiedBy,
                Description = plan.Description
            };
            return entity;
        }

        public ActionResult UploadDocument(int subIndicatorId, HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + subIndicatorId.ToString() + "-" + AttachmentType.DOCUMENT), subIndicatorId.ToString() + "-" + txtValue + "-" + fileName);

                    var mainPath = Path.Combine(Server.MapPath("~/Content/Images/" + subIndicatorId.ToString() + "-" + AttachmentType.DOCUMENT));

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {
                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = subIndicatorId.ToString(),
                            Description = subIndicatorId.ToString() + "-" + txtValue + "-" + fileName,
                            FileName = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.DOCUMENT,
                            Index = 0,
                            URL = path,
                            Remark = subIndicatorId.ToString()
                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "The file is existed";

                }
                return RedirectToAction("Index", "PlanSelfCares");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
        public ActionResult DownloadDoument(string filename, string attachId)
        {
            int.TryParse(attachId, out int attachmentId);
            var record = db.Attachments.Find(attachmentId);

            return File(record.URL, "application/force-download", Path.GetFileName(record.URL));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
