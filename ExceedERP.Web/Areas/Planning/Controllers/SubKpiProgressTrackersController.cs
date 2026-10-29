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
    public class SubKpiProgressTrackersController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
       
        
        public ActionResult SubKpiProgressTrackers_Read([DataSourceRequest]DataSourceRequest request,int kpiId)
        {
            IQueryable<SubKpiProgressTracker> kpiProgressTrackers = db.SubKpiProgressTrackers.Where(x=>x.KpiProgressTrackerId== kpiId).AsQueryable();
            DataSourceResult result = kpiProgressTrackers.ToDataSourceResult(request, kpiProgressTracker => new {
                SubKpiProgressTrackerId = kpiProgressTracker.SubKpiProgressTrackerId,
                KpiProgressTrackerId = kpiProgressTracker.KpiProgressTrackerId,
                GlobalSubKPIId = kpiProgressTracker.GlobalSubKPIId,
                Indicators = kpiProgressTracker.Indicators,
                Saf = kpiProgressTracker.Saf,
                Weight = kpiProgressTracker.Weight,
                AnnualPlan = kpiProgressTracker.AnnualPlan,
                UpTo2ndQuarterPlan = kpiProgressTracker.UpTo2ndQuarterPlan,
                UpTo2ndQuarterPerformance = kpiProgressTracker.UpTo2ndQuarterPerformance,
                UpTo2ndQuarterPercent = kpiProgressTracker.UpTo2ndQuarterPercent,
                Evaluation = kpiProgressTracker.Evaluation,
                Remark = kpiProgressTracker.Remark,
                CreatedBy = kpiProgressTracker.CreatedBy,
                DateCreated = kpiProgressTracker.DateCreated,
                LastModified = kpiProgressTracker.LastModified,
                ModifiedBy = kpiProgressTracker.ModifiedBy,


            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubKpiProgressTrackers_Create([DataSourceRequest]DataSourceRequest request, SubKpiProgressTracker subKpiProgressTracker,int kpiId)
        {
            subKpiProgressTracker.KpiProgressTrackerId = kpiId;
            var globalkpi = db.KpiProgressTrackers.Find(kpiId);
            if (globalkpi == null)
            {

                ModelState.AddModelError("exist", "  KPI not Found.");
                return Json(new[] { subKpiProgressTracker }.ToDataSourceResult(request, ModelState));

            }
            if (globalkpi.Weight == null)
            {
                globalkpi.Weight = 0;
            }
            if (globalkpi.Evaluation == null)
            {
                globalkpi.Evaluation = 0;
            }
            if (db.SubKpiProgressTrackers.Any(x=>x.Indicators == subKpiProgressTracker.Indicators
                                            &&x.GlobalSubKPIId== subKpiProgressTracker.GlobalSubKPIId
                                            &&x.KpiProgressTrackerId== kpiId))
            {
                ModelState.AddModelError("exist", "  Dublication not Allowed.");
                return Json(new[] { subKpiProgressTracker }.ToDataSourceResult(request, ModelState));

            }
          
           
            //subKpiProgressTracker.UpTo2ndQuarterPercent = subKpiProgressTracker.UpTo2ndQuarterPerformance > subKpiProgressTracker.UpTo2ndQuarterPlan ? 100 : subKpiProgressTracker.UpTo2ndQuarterPlan != 0 ? subKpiProgressTracker.UpTo2ndQuarterPerformance * 100 / subKpiProgressTracker.UpTo2ndQuarterPlan : 0;
            subKpiProgressTracker.UpTo2ndQuarterPercent = subKpiProgressTracker.UpTo2ndQuarterPerformance > subKpiProgressTracker.UpTo2ndQuarterPlan
                                       ? 100 : subKpiProgressTracker.UpTo2ndQuarterPlan != 0 ?
                                       subKpiProgressTracker.UpTo2ndQuarterPerformance * 100 / subKpiProgressTracker.UpTo2ndQuarterPlan : 0;
            subKpiProgressTracker.Evaluation = (subKpiProgressTracker.Weight * subKpiProgressTracker.UpTo2ndQuarterPercent) / 100;

            globalkpi.Weight += subKpiProgressTracker.Weight;
            globalkpi.Evaluation += subKpiProgressTracker.Evaluation;
            if (ModelState.IsValid)
            {
                subKpiProgressTracker.CreatedBy = User.Identity.Name;
                subKpiProgressTracker.DateCreated = DateTime.Now;
                var entity = GetMappedObject(subKpiProgressTracker);
                db.SubKpiProgressTrackers.Add(entity);
                db.Entry(globalkpi).State = EntityState.Modified;
                db.SaveChanges();
                subKpiProgressTracker.SubKpiProgressTrackerId = entity.SubKpiProgressTrackerId;
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "sub KPI Progress Tracker ", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { subKpiProgressTracker }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubKpiProgressTrackers_Update([DataSourceRequest]DataSourceRequest request, SubKpiProgressTracker subKpiProgressTracker)
        {
            var subKpis = db.SubKpiProgressTrackers.Where(x =>  x.KpiProgressTrackerId == subKpiProgressTracker.KpiProgressTrackerId
                                             && x.SubKpiProgressTrackerId != subKpiProgressTracker.SubKpiProgressTrackerId);
            if (subKpis.Any(x=> x.Indicators == subKpiProgressTracker.Indicators
                                &&x.GlobalSubKPIId == subKpiProgressTracker.GlobalSubKPIId))
            {
                ModelState.AddModelError("exist", "  Dublication not Allowed.");
                return Json(new[] { subKpiProgressTracker }.ToDataSourceResult(request, ModelState));

            }
            var globalkpi = db.KpiProgressTrackers.Find(subKpiProgressTracker.KpiProgressTrackerId);
            if (globalkpi == null)
            {

                ModelState.AddModelError("exist", "  KPI not Found.");
                return Json(new[] { subKpiProgressTracker }.ToDataSourceResult(request, ModelState));

            }
            //kpiProgressTracker.UpTo2ndQuarterPercent = kpiProgressTracker.UpTo2ndQuarterPerformance > kpiProgressTracker.UpTo2ndQuarterPlan ? 100 : kpiProgressTracker.UpTo2ndQuarterPlan != 0 ? kpiProgressTracker.UpTo2ndQuarterPerformance * 100 / kpiProgressTracker.UpTo2ndQuarterPlan : 0;
            subKpiProgressTracker.UpTo2ndQuarterPercent = subKpiProgressTracker.UpTo2ndQuarterPerformance > subKpiProgressTracker.UpTo2ndQuarterPlan
                                       ? 100 : subKpiProgressTracker.UpTo2ndQuarterPlan != 0 ?
                                       subKpiProgressTracker.UpTo2ndQuarterPerformance * 100 / subKpiProgressTracker.UpTo2ndQuarterPlan : 0;
            subKpiProgressTracker.Evaluation = (subKpiProgressTracker.Weight * subKpiProgressTracker.UpTo2ndQuarterPercent) / 100;
            globalkpi.Weight = subKpiProgressTracker.Weight+ (subKpis.Any()?subKpis.Sum(x=>x.Weight):0);
            globalkpi.Evaluation = subKpiProgressTracker.Evaluation + (subKpis.Any() ? subKpis.Sum(x => x.Evaluation) : 0);
            subKpiProgressTracker.ModifiedBy = User.Identity.Name;
            subKpiProgressTracker.LastModified = DateTime.Now;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(subKpiProgressTracker);
                db.SubKpiProgressTrackers.Attach(entity);
                db.Entry(globalkpi).State = EntityState.Modified;
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Sub Kpi Progress Trackers", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { subKpiProgressTracker }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubKpiProgressTrackers_Destroy([DataSourceRequest]DataSourceRequest request, SubKpiProgressTracker kpiProgressTracker)
        {
            

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(kpiProgressTracker);
                #region delete log
                var delete = kpiProgressTracker.KpiProgressTrackerId.ToString();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "     Objective deleted  " + delete);
                #endregion
                db.SubKpiProgressTrackers.Attach(entity);
                db.SubKpiProgressTrackers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { kpiProgressTracker }.ToDataSourceResult(request, ModelState));
        }
        private SubKpiProgressTracker GetMappedObject(SubKpiProgressTracker kpiProgressTracker)
        {
            var entity = new SubKpiProgressTracker
            {
                SubKpiProgressTrackerId = kpiProgressTracker.SubKpiProgressTrackerId,
                KpiProgressTrackerId = kpiProgressTracker.KpiProgressTrackerId,
                GlobalSubKPIId = kpiProgressTracker.GlobalSubKPIId,
                Indicators = kpiProgressTracker.Indicators,
                Saf = kpiProgressTracker.Saf,
                Weight = kpiProgressTracker.Weight,
                AnnualPlan = kpiProgressTracker.AnnualPlan,
                UpTo2ndQuarterPlan = kpiProgressTracker.UpTo2ndQuarterPlan,
                UpTo2ndQuarterPerformance = kpiProgressTracker.UpTo2ndQuarterPerformance,
                UpTo2ndQuarterPercent = kpiProgressTracker.UpTo2ndQuarterPercent,
                Evaluation = kpiProgressTracker.Evaluation,
                Remark = kpiProgressTracker.Remark,
                CreatedBy = kpiProgressTracker.CreatedBy,
                DateCreated = kpiProgressTracker.DateCreated,
                LastModified = kpiProgressTracker.LastModified,
                ModifiedBy = kpiProgressTracker.ModifiedBy,

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
