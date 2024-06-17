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
    public class SubIndicatorsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult SubIndicators_Read([DataSourceRequest]DataSourceRequest request,int objecId, int parentId )
        {
            IQueryable<Indicator> subIndicators = db.Indicators.Where(x=>x.ObjectiveId == objecId && x.ParentId==parentId);
            DataSourceResult result = subIndicators.ToDataSourceResult(request, indicator => new {
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
        public ActionResult SubIndicators_Create([DataSourceRequest]DataSourceRequest request, Indicator subIndicator, int objecId,int partId)
        {
            var globalIndicator = db.GlobalIndicators.Find(subIndicator.GlobalIndicatorId);
            if(globalIndicator != null)
            {
                subIndicator.Name = globalIndicator.Indicator;
            }
            var exist = db.Indicators.Where(x => x.GlobalIndicatorId == subIndicator.GlobalIndicatorId
                                                && x.ObjectiveId == objecId
                                                && x.KPIType == subIndicator.KPIType
                                                && x.ParentId==partId).Any();
            if (exist)
            {
                ModelState.AddModelError("", "The Sub-Indicator already Exists");
            }
            if (ModelState.IsValid)
            {
                subIndicator.ObjectiveId = objecId;
                subIndicator.ParentId = partId;
                var entity = GetMappedObject(subIndicator);
                db.Indicators.Add(entity);
                db.SaveChanges();
                subIndicator.IndicatorId = entity.IndicatorId;
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Indicator ", "Success", User.Identity.Name, subIndicator.Name+"  is Created By  " + User.Identity.Name, Modules.Planning);

                #endregion

            }

            return Json(new[] { subIndicator }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubIndicators_Update([DataSourceRequest]DataSourceRequest request, Indicator subIndicator)
        {
            var globalIndicator = db.GlobalIndicators.Find(subIndicator.GlobalIndicatorId);
            if (globalIndicator != null)
            {
                subIndicator.Name = globalIndicator.Indicator;
            }
            var exist = db.Indicators.Where(x => x.GlobalIndicatorId == subIndicator.GlobalIndicatorId 
                                            && x.ObjectiveId == subIndicator.ObjectiveId
                                            && x.ParentId == subIndicator.ParentId     
                                            && x.KPIType == subIndicator.KPIType
                                            && x.IndicatorId !=subIndicator.IndicatorId).Any();
            if (exist)
            {
                ModelState.AddModelError("", "The Sub-Indicator already Exists");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(subIndicator);
                db.Indicators.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Indicator", "Success", User.Identity.Name,subIndicator.Name+ " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { subIndicator }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubIndicators_Destroy([DataSourceRequest]DataSourceRequest request, Indicator subIndicator)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(subIndicator);
                #region delete log
                var delete = subIndicator.Name;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "     Objective deleted  " + delete);
                #endregion

                db.Indicators.Attach(entity);
                db.Indicators.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { subIndicator }.ToDataSourceResult(request, ModelState));
        }
        private Indicator GetMappedObject(Indicator indicator)
        {
            var entity = new Indicator
            {
                IndicatorId = indicator.IndicatorId,
                ParentId = indicator.ParentId,
                HasSubIndicator = indicator.HasSubIndicator,
                Name = indicator.Name,
                Description = indicator.Description,
                Unit = indicator.Unit,
                KPIType = indicator.KPIType,
                GlobalIndicatorId = indicator.GlobalIndicatorId,
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
