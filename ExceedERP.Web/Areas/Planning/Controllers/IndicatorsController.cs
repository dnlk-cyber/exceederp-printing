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
    [AuthorizeRoles(PlanningRoles.Planning_Admin)]
    public class IndicatorsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult Indicators_Read([DataSourceRequest]DataSourceRequest request,int oId)
        {
            IQueryable<Indicator> indicators = db.Indicators.Where(x=>x.ObjectiveId == oId && x.ParentId==0);
            DataSourceResult result = indicators.ToDataSourceResult(request, indicator => new {
                IndicatorId = indicator.IndicatorId,
                ParentId = indicator.ParentId,
                HasSubIndicator = indicator.HasSubIndicator,
                Name = indicator.Name,
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
            var exist = db.Indicators.Where(x => x.GlobalIndicatorId == indicator.GlobalIndicatorId 
                                            && x.ParentId == indicator.ParentId 
                                            && x.ObjectiveId == oId
                                            &&x.KPIType==indicator.KPIType).Any();
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
            var exist = db.Indicators.Where(x => x.GlobalIndicatorId == indicator.GlobalIndicatorId
                                            && x.ObjectiveId == indicator.ObjectiveId
                                            && x.ParentId == indicator.ParentId
                                            && x.KPIType == indicator.KPIType
                                            && x.IndicatorId != indicator.IndicatorId).Any();
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
                Name = indicator.Name,
                Description = indicator.Description,
                Unit = indicator.Unit,
                KPIType = indicator.KPIType,
                ObjectiveId = indicator.ObjectiveId
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
