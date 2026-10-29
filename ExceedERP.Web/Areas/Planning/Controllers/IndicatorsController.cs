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

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [Authorize]
    public class IndicatorsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult Indicators_Read([DataSourceRequest]DataSourceRequest request, int? oId, bool activeOnly = false, int? old = null)
        {
            // Nested grids can issue an initial request before the client-template
            // value has been resolved. Do not let MVC fail binding with a 500.
            var objectiveId = oId ?? old;
            if (!objectiveId.HasValue)
            {
                return Json(Enumerable.Empty<Indicator>().ToDataSourceResult(request));
            }

            IQueryable<Indicator> indicators = db.Indicators.Where(x=>x.ObjectiveId == objectiveId.Value && x.ParentId==0);
            if (activeOnly)
            {
                indicators = indicators.Where(x => x.IsActive);
            }

            DataSourceResult result = indicators.ToDataSourceResult(request, indicator => new {
                IndicatorId = indicator.IndicatorId,
                ParentId = indicator.ParentId,
                HasSubIndicator = indicator.HasSubIndicator,
                IsActive = indicator.IsActive,
                Name = indicator.Name,
                Weight = indicator.Weight,
                Description = indicator.Description,
                Unit = indicator.Unit,
                KPIType = indicator.KPIType,
                GlobalIndicatorId = indicator.GlobalIndicatorId,
                ObjectiveId = indicator.ObjectiveId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Indicators_Create([DataSourceRequest]DataSourceRequest request, Indicator indicator,int oId)
        {
            var globalIndicator = db.GlobalIndicators.Find(indicator.GlobalIndicatorId);
            if (globalIndicator != null)
            {
                indicator.Name = globalIndicator.Indicator;
            }
            // New indicators must be active, otherwise the activeOnly grid read hides them
            indicator.IsActive = true;
            var normalizedUnit = (indicator.Unit ?? string.Empty).Trim();
            var exist = db.Indicators.Any(x => x.GlobalIndicatorId == indicator.GlobalIndicatorId
                                            && x.ParentId == indicator.ParentId
                                            && x.ObjectiveId == oId
                                            && x.KPIType == indicator.KPIType
                                            && (x.Unit ?? string.Empty).Trim() == normalizedUnit);
            if (exist)
            {
                ModelState.AddModelError("", "The Indicator already Exists");
            }
            if (ModelState.IsValid)
            {
                indicator.ObjectiveId = oId;
                var entity = GetMappedObject(indicator);
                db.Indicators.Add(entity);
                db.SaveChanges();
                indicator.IndicatorId = entity.IndicatorId;
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Indicator ", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);

                #endregion

            }

            return Json(new[] { indicator }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Indicators_Update([DataSourceRequest]DataSourceRequest request, Indicator indicator)
        {
            var globalIndicator = db.GlobalIndicators.Find(indicator.GlobalIndicatorId);
            if (globalIndicator != null)
            {
                indicator.Name = globalIndicator.Indicator;
            }
            // Preserve the indicator's own IsActive; it is managed via EnableOrDisable
            var currentIsActive = db.Indicators.Where(x => x.IndicatorId == indicator.IndicatorId)
                                               .Select(x => x.IsActive)
                                               .FirstOrDefault();
            indicator.IsActive = currentIsActive;
            var normalizedUnit = (indicator.Unit ?? string.Empty).Trim();
            var exist = db.Indicators.Any(x => x.GlobalIndicatorId == indicator.GlobalIndicatorId
                                            && x.ObjectiveId == indicator.ObjectiveId
                                            && x.ParentId == indicator.ParentId
                                            && x.KPIType == indicator.KPIType
                                            && (x.Unit ?? string.Empty).Trim() == normalizedUnit
                                            && x.IndicatorId != indicator.IndicatorId);
            if (exist)
            {
                ModelState.AddModelError("", "The Indicator already Exists");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(indicator);
                db.Indicators.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Indicator", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { indicator }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Indicators_Destroy([DataSourceRequest]DataSourceRequest request, Indicator indicator)
        {
            if (db.Indicators.Any(x => x.ParentId == indicator.IndicatorId))
            {
                ModelState.AddModelError("children", "You cannot delete this indicator until its sub-indicators are deleted.");
                return Json(new[] { indicator }.ToDataSourceResult(request, ModelState));
            }

            if (db.PlanningDetails.Any(x => x.GlobalIndicatorId == indicator.GlobalIndicatorId))
            {
                ModelState.AddModelError("exist", "You cannot delete this indicator as it is used in a plan. Please delete it from the plan first.");
                return Json(new[] { indicator }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(indicator);
                #region delete log
                var delete = indicator.Name;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "     Objective deleted  " + delete);
                #endregion

                db.Indicators.Attach(entity);
                db.Indicators.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { indicator }.ToDataSourceResult(request, ModelState));
        }
        private Indicator GetMappedObject(Indicator indicator)
        {
            var entity = new Indicator
            {
                IndicatorId = indicator.IndicatorId,
                GlobalIndicatorId = indicator.GlobalIndicatorId,
                ParentId = indicator.ParentId,
                HasSubIndicator = indicator.HasSubIndicator,
                IsActive = indicator.IsActive,
                Name = indicator.Name,
                Weight = indicator.Weight,
                Description = indicator.Description,
                Unit = indicator.Unit,
                KPIType = indicator.KPIType,
                ObjectiveId = indicator.ObjectiveId
            };
            return entity;
        }
        public ActionResult EnableOrDisable(int id)
        {
            var indicator = db.Indicators.Find(id);

            if (indicator == null)
            {
                return Json(new
                {
                    Message = "This Indicator was not found.",
                    Status = "FAIL"
                }, JsonRequestBehavior.AllowGet);
            }

            // Toggle IsActive status
            indicator.IsActive = !indicator.IsActive;
            db.Entry(indicator).State = EntityState.Modified;
            // Update subindicators based on the parent's new IsActive status
            var subindicators = db.Indicators.Where(x => x.ParentId == indicator.IndicatorId).ToList();
            foreach (var subindicator in subindicators)
            {
                subindicator.IsActive = indicator.IsActive;
                db.Entry(subindicator).State = EntityState.Modified;
            }

            db.SaveChanges();

            #region Operation Log
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            UserHelper.OperationLog(Request.UserHostAddress,"Indicator","Success", User.Identity.Name,
                User.Identity.Name+ "updated Indicator" +indicator.Name+ "to IsActive"+indicator.IsActive, Modules.HRM
            );
            #endregion

            var responseMessage = indicator.IsActive
                ? "You successfully enabled this indicator."
                : "You successfully disabled this indicator.";

            return Json(new
            {
                Message = responseMessage,
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult EnableOrDisable(int id)
        //{
        //    var indicator = db.Indicators.Find(id);

        //    if (indicator != null && indicator.IsActive)
        //    {

        //        indicator.IsActive = false;

        //        db.Entry(indicator).State = EntityState.Modified;
        //        var subindicators = db.Indicators.Where(x => x.ParentId == indicator.IndicatorId).ToList();
        //        foreach (var subindicator in subindicators)
        //        {
        //            subindicator.IsActive = false;

        //            db.Entry(subindicator).State = EntityState.Modified;

        //        }
        //        db.SaveChanges();

        //        #region operation log

        //        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

        //        UserHelper.OperationLog(Request.UserHostAddress, "Indicator ", "Success", User.Identity.Name, User.Identity.Name + " Add new cIndicator " + indicator.Name + "IsActive" + indicator.IsActive, Modules.HRM);


        //        #endregion
        //        var formattedData = new
        //        {
        //            Message = "You successfully Disabled this Work unit.",
        //            Status = "OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }
        //    if (indicator != null && !indicator.IsActive)
        //    {

        //        indicator.IsActive = true;

        //        db.Entry(indicator).State = EntityState.Modified;
        //        var subindicators = db.Indicators.Where(x => x.ParentId == indicator.IndicatorId).ToList();
        //        foreach (var subindicator in subindicators)
        //        {
        //            subindicator.IsActive = true;

        //            db.Entry(subindicator).State = EntityState.Modified;

        //        }
        //        db.SaveChanges();
        //        db.SaveChanges();
        //        #region operation log

        //        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

        //        UserHelper.OperationLog(Request.UserHostAddress, "Indicator ", "Success", User.Identity.Name, User.Identity.Name + " Add new cIndicator " + indicator.Name + "IsActive" + indicator.IsActive, Modules.HRM);


        //        #endregion

        //        var formattedData = new
        //        {
        //            Message = "You successfully enabled this indicator.",
        //            Status = "OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }
        //    var formattedData1 = new
        //    {
        //        Message = "this Indicator Not Found .",
        //        Status = "FAIL"
        //    };
        //    return Json(formattedData1, JsonRequestBehavior.AllowGet);

        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
