﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin,
                    PlanningRoles.Planning_User,
                    PlanningRoles.Planning_Risk_User,
                    HrmConstants.HRDepartmentHead,
                    HrmConstants.HREmployeeSelfService)]
    public class KpiProgressTrackersController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            var kpiIds = db.KpiProgressTrackers.Select(x => x.GlobalKPIId).Distinct();
            var SubkpiIds = db.SubKpiProgressTrackers.Select(x => x.GlobalSubKPIId).Distinct();
            ViewData["kpi"] = db.GlobalKPIs.Where(x => kpiIds.Contains(x.GlobalKPIId)).Select(x=>new { x.GlobalKPIId,x.KPIName}).AsQueryable();
            ViewData["subkpi"] = db.GlobalSubKPIs.Where(x => SubkpiIds.Contains(x.GlobalSubKPIId)).Select(x => new { x.GlobalSubKPIId, x.SubKPIName }).AsQueryable();
            return View();
        }
        
        public ActionResult KpiProgressTrackers_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<KpiProgressTracker> kpiProgressTrackers = db.KpiProgressTrackers;
            DataSourceResult result = kpiProgressTrackers.ToDataSourceResult(request, kpiProgressTracker => new {
                KpiProgressTrackerId = kpiProgressTracker.KpiProgressTrackerId,
                //GlobalSubKPIId = kpiProgressTracker.GlobalSubKPIId,
                GlobalKPIId = kpiProgressTracker.GlobalKPIId,
                //Saf = kpiProgressTracker.Saf,
                Weight = kpiProgressTracker.Weight,
                AnnualPlan = kpiProgressTracker.AnnualPlan,
                UpTo2ndQuarterPlan = kpiProgressTracker.UpTo2ndQuarterPlan,
                UpTo2ndQuarterPerformance = kpiProgressTracker.UpTo2ndQuarterPerformance,
                UpTo2ndQuarterPercent = kpiProgressTracker.UpTo2ndQuarterPercent,
                Evaluation = kpiProgressTracker.Evaluation,
                Remark = kpiProgressTracker.Remark,
                IsActive = kpiProgressTracker.IsActive,
                CreatedBy = kpiProgressTracker.CreatedBy,
                DateCreated = kpiProgressTracker.DateCreated,
                LastModified = kpiProgressTracker.LastModified,
                ModifiedBy = kpiProgressTracker.ModifiedBy,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult KpiProgressTrackers_Create([DataSourceRequest]DataSourceRequest request, KpiProgressTracker kpiProgressTracker)
        {
            if(db.KpiProgressTrackers.Any(x=>x.GlobalKPIId == kpiProgressTracker.GlobalKPIId))
            {
                ModelState.AddModelError("exist", "  Dublication not Allowed.");
                return Json(new[] { kpiProgressTracker }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
                kpiProgressTracker.CreatedBy = User.Identity.Name;
                kpiProgressTracker.DateCreated = DateTime.Now;
                kpiProgressTracker.IsActive = true;
                var entity = GetMappedObject(kpiProgressTracker);
                db.KpiProgressTrackers.Add(entity);
                db.SaveChanges();
                kpiProgressTracker.KpiProgressTrackerId = entity.KpiProgressTrackerId;
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "GlobalKPI ", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { kpiProgressTracker }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult KpiProgressTrackers_Update([DataSourceRequest]DataSourceRequest request, KpiProgressTracker kpiProgressTracker)
        {
            if (db.KpiProgressTrackers.Any(x => x.GlobalKPIId == kpiProgressTracker.GlobalKPIId
                                             &&x.KpiProgressTrackerId != kpiProgressTracker.KpiProgressTrackerId))
            {
                ModelState.AddModelError("exist", "  dublication not allowed.");
                return Json(new[] { kpiProgressTracker }.ToDataSourceResult(request, ModelState));

            }
            kpiProgressTracker.ModifiedBy = User.Identity.Name;
            kpiProgressTracker.LastModified = DateTime.Now;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(kpiProgressTracker);
                db.KpiProgressTrackers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "KpiProgressTrackers", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { kpiProgressTracker }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult KpiProgressTrackers_Destroy([DataSourceRequest]DataSourceRequest request, KpiProgressTracker kpiProgressTracker)
        {
            // Check if the GlobalIndicator is being used in any Indicator
            if (db.SubKpiProgressTrackers.Any(x => x.KpiProgressTrackerId == kpiProgressTracker.KpiProgressTrackerId))
            {
                ModelState.AddModelError("exist", "You cannot delete this item because it is used in KpI pregess tracker. Please delete it the progess  first.");
                return Json(new[] { kpiProgressTracker }.ToDataSourceResult(request, ModelState));

            }

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(kpiProgressTracker);
                #region delete log
                var delete = kpiProgressTracker.KpiProgressTrackerId.ToString();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "     Objective deleted  " + delete);
                #endregion
                db.KpiProgressTrackers.Attach(entity);
                db.KpiProgressTrackers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { kpiProgressTracker }.ToDataSourceResult(request, ModelState));
        }
        private KpiProgressTracker GetMappedObject(KpiProgressTracker kpiProgressTracker)
        {
            var entity = new KpiProgressTracker
            {
                KpiProgressTrackerId = kpiProgressTracker.KpiProgressTrackerId,
                //GlobalSubKPIId = kpiProgressTracker.GlobalSubKPIId,
                GlobalKPIId = kpiProgressTracker.GlobalKPIId,
                //Saf = kpiProgressTracker.Saf,
                Weight = kpiProgressTracker.Weight,
                AnnualPlan = kpiProgressTracker.AnnualPlan,
                UpTo2ndQuarterPlan = kpiProgressTracker.UpTo2ndQuarterPlan,
                UpTo2ndQuarterPerformance = kpiProgressTracker.UpTo2ndQuarterPerformance,
                UpTo2ndQuarterPercent = kpiProgressTracker.UpTo2ndQuarterPercent,
                Evaluation = kpiProgressTracker.Evaluation,
                Remark = kpiProgressTracker.Remark,
                IsActive = kpiProgressTracker.IsActive,
                CreatedBy = kpiProgressTracker.CreatedBy,
                DateCreated = kpiProgressTracker.DateCreated,
                LastModified = kpiProgressTracker.LastModified,
                ModifiedBy = kpiProgressTracker.ModifiedBy,

            };
            return entity;
        }

        public ActionResult EnableOrDisable(int id)
        {
            var indicator = db.GlobalIndicators.Find(id);

            if (indicator == null)
            {
                return Json(new
                {
                    Message = "This Indicator was not found.",
                    Status = "Fail"
                }, JsonRequestBehavior.AllowGet);
            }

            // Toggle IsActive status
            indicator.IsActive = !indicator.IsActive;
            db.Entry(indicator).State = EntityState.Modified;
            // Update Globalindicators based on the parent's new IsActive status
            db.SaveChanges();

            #region Operation Log
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            UserHelper.OperationLog(Request.UserHostAddress, "Indicator", "Success", User.Identity.Name,
                User.Identity.Name + "updated Indicator" + indicator.Indicator + "to IsActive" + indicator.IsActive, Modules.HRM
            );
            #endregion

            var responseMessage = indicator.IsActive
                ? "You successfully enabled this Global Indicator."
                : "You successfully disabled this Global Indicator.";

            return Json(new
            {
                Message = responseMessage,
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
