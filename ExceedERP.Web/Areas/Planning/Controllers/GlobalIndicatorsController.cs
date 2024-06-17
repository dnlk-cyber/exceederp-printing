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
    public class GlobalIndicatorsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult GlobalIndicators_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GlobalIndicator> globalIndicators = db.GlobalIndicators;
            DataSourceResult result = globalIndicators.ToDataSourceResult(request, globalIndicator => new {
                GlobalIndicatorId = globalIndicator.GlobalIndicatorId,
                Indicator = globalIndicator.Indicator,
                Description = globalIndicator.Description,
                CreatedBy = globalIndicator.CreatedBy,
                DateCreated = globalIndicator.DateCreated,
                LastModified = globalIndicator.LastModified,
                ModifiedBy = globalIndicator.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlobalIndicators_Create([DataSourceRequest]DataSourceRequest request, GlobalIndicator globalIndicator)
        {
            if (ModelState.IsValid)
            {
                globalIndicator.CreatedBy = User.Identity.Name;
                globalIndicator.DateCreated = DateTime.Now;
                var entity = GetMappedObject(globalIndicator);
                db.GlobalIndicators.Add(entity);
                db.SaveChanges();
                globalIndicator.GlobalIndicatorId = entity.GlobalIndicatorId;
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Indicator ", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { globalIndicator }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlobalIndicators_Update([DataSourceRequest]DataSourceRequest request, GlobalIndicator globalIndicator)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(globalIndicator);
                db.GlobalIndicators.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Indicator", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { globalIndicator }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlobalIndicators_Destroy([DataSourceRequest]DataSourceRequest request, GlobalIndicator globalIndicator)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(globalIndicator);
                #region delete log
                var delete = globalIndicator.Indicator;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "     Objective deleted  " + delete);
                #endregion
                db.GlobalIndicators.Attach(entity);
                db.GlobalIndicators.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { globalIndicator }.ToDataSourceResult(request, ModelState));
        }
        private GlobalIndicator GetMappedObject(GlobalIndicator globalIndicator)
        {
            var entity = new GlobalIndicator
            {
                GlobalIndicatorId = globalIndicator.GlobalIndicatorId,
                Indicator = globalIndicator.Indicator,
                Description = globalIndicator.Description,
                CreatedBy = globalIndicator.CreatedBy,
                DateCreated = globalIndicator.DateCreated,
                LastModified = globalIndicator.LastModified,
                ModifiedBy = globalIndicator.ModifiedBy,

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
