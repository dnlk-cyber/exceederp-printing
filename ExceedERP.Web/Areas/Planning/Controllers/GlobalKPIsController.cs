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
                    HrmConstants.HRDepartmentHead,
                    HrmConstants.HREmployeeSelfService)]
    public class GlobalKPIsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult GlobalKPIs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GlobalKPI> globalKPIs = db.GlobalKPIs;
            DataSourceResult result = globalKPIs.ToDataSourceResult(request, globalKPI => new {
                GlobalKPIId = globalKPI.GlobalKPIId,
                KPIName = globalKPI.KPIName,
                IsActive = globalKPI.IsActive,
                CreatedBy = globalKPI.CreatedBy,
                DateCreated = globalKPI.DateCreated,
                LastModified = globalKPI.LastModified,
                ModifiedBy = globalKPI.ModifiedBy,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlobalKPIs_Create([DataSourceRequest]DataSourceRequest request, GlobalKPI globalKPI)
        {
                globalKPI.KPIName =globalKPI.KPIName?.ToLower()                  // lowercase
                                        .Trim()                      // trim spaces
                                        .Replace("\r", "")           // remove carriage return
                                        .Replace("\n", "")           // remove new line
                                        .Replace("#", "");           // remove #
            if (db.GlobalKPIs.Any(x=>x.KPIName.ToLower().Trim() == globalKPI.KPIName.ToLower().Trim()))
            {
                ModelState.AddModelError("exist", "  This KPI Name is found.");
                return Json(new[] { globalKPI }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
                globalKPI.CreatedBy = User.Identity.Name;
                globalKPI.DateCreated = DateTime.Now;
                globalKPI.IsActive = true;
                var entity = GetMappedObject(globalKPI);
                db.GlobalKPIs.Add(entity);
                db.SaveChanges();
                globalKPI.GlobalKPIId = entity.GlobalKPIId;
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "GlobalKPI ", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { globalKPI }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlobalKPIs_Update([DataSourceRequest]DataSourceRequest request, GlobalKPI globalKPI)
        {
            globalKPI.KPIName = globalKPI.KPIName?.ToLower()                  // lowercase
                                      .Trim()                      // trim spaces
                                      .Replace("\r", "")           // remove carriage return
                                      .Replace("\n", "")           // remove new line
                                      .Replace("#", "");           // remove #
            if (db.GlobalKPIs
                .Any(x => x.KPIName.ToLower().Trim() == globalKPI.KPIName.ToLower().Trim()
                 &&x.GlobalKPIId !=globalKPI.GlobalKPIId))
            {
                ModelState.AddModelError("exist", "  This KPI Name is already registered.");
                return Json(new[] { globalKPI }.ToDataSourceResult(request, ModelState));

            }
            globalKPI.ModifiedBy = User.Identity.Name;
            globalKPI.LastModified = DateTime.Now;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(globalKPI);
                db.GlobalKPIs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "globalKPI", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { globalKPI }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlobalKPIs_Destroy([DataSourceRequest]DataSourceRequest request, GlobalKPI globalKPI)
        {
            // Check if the GlobalIndicator is being used in any Indicator
            if (db.KpiProgressTrackers.Any(x => x.GlobalKPIId == globalKPI.GlobalKPIId))
            {
                ModelState.AddModelError("exist", "You cannot delete this item because it is used in KpI pregess tracker. Please delete it the progess  first.");
                return Json(new[] { globalKPI }.ToDataSourceResult(request, ModelState));

            }

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(globalKPI);
                #region delete log
                var delete = globalKPI.KPIName;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "     Objective deleted  " + delete);
                #endregion
                db.GlobalKPIs.Attach(entity);
                db.GlobalKPIs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { globalKPI }.ToDataSourceResult(request, ModelState));
        }
        private GlobalKPI GetMappedObject(GlobalKPI globalKPI)
        {
            var entity = new GlobalKPI
            {
                GlobalKPIId = globalKPI.GlobalKPIId,
                KPIName = globalKPI.KPIName,
                IsActive = globalKPI.IsActive,
                CreatedBy = globalKPI.CreatedBy,
                DateCreated = globalKPI.DateCreated,
                LastModified = globalKPI.LastModified,
                ModifiedBy = globalKPI.ModifiedBy,

            };
            return entity;
        }

        public ActionResult EnableOrDisable(int id)
        {
            var indicator = db.GlobalKPIs.Find(id);

            if (indicator == null)
            {
                return Json(new
                {
                    Message = "This GlobalKPIs was not found.",
                    Status = "FAIL"
                }, JsonRequestBehavior.AllowGet);
            }

            // Toggle IsActive status
            indicator.IsActive = !indicator.IsActive;
            db.Entry(indicator).State = EntityState.Modified;
            // Update Globalindicators based on the parent's new IsActive status
            db.SaveChanges();

            #region Operation Log
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            UserHelper.OperationLog(Request.UserHostAddress, "GlobalKPI", "Success", User.Identity.Name,
                User.Identity.Name + "updated KPI" + indicator.KPIName + "to IsActive" + indicator.IsActive, Modules.HRM
            );
            #endregion

            var responseMessage = indicator.IsActive
                ? "You successfully enabled this Global KPI."
                : "You successfully disabled this Global KPI.";

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
