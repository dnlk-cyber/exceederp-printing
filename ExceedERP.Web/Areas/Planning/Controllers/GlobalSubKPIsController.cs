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
    public class GlobalSubKPIsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult GlobalSubKPIs_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<GlobalSubKPI> subKPIs = db.GlobalSubKPIs.Where(x=>x.GlobalKPIId==id);
            DataSourceResult result = subKPIs.ToDataSourceResult(request, subKPI => new {
                GlobalSubKPIId = subKPI.GlobalSubKPIId,
                GlobalKPIId = subKPI.GlobalKPIId,
                SubKPIName = subKPI.SubKPIName,
                CreatedBy = subKPI.CreatedBy,
                DateCreated = subKPI.DateCreated,
                LastModified = subKPI.LastModified,
                ModifiedBy = subKPI.ModifiedBy,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlobalSubKPIs_Create([DataSourceRequest]DataSourceRequest request, GlobalSubKPI subKPI, int id)
        {
            subKPI.SubKPIName = subKPI.SubKPIName?.ToLower()                  // lowercase
                                      .Trim()                      // trim spaces
                                      .Replace("\r", "")           // remove carriage return
                                      .Replace("\n", "")           // remove new line
                                      .Replace("#", "");           // remove #
            if (db.GlobalSubKPIs
                .Any(x=>x.SubKPIName.ToLower().Trim() == subKPI.SubKPIName.ToLower().Trim()
                        &&x.GlobalKPIId==id))
            {
                ModelState.AddModelError("exist", "  This Sub-KPI Name is already Registed.");
                return Json(new[] { subKPI }.ToDataSourceResult(request, ModelState));

            }
            subKPI.GlobalKPIId = id;
            subKPI.CreatedBy = User.Identity.Name;
            subKPI.DateCreated = DateTime.Now;
            if (ModelState.IsValid)
            {
               
                var entity = GetMappedObject(subKPI);
                db.GlobalSubKPIs.Add(entity);
                db.SaveChanges();
                subKPI.GlobalKPIId = entity.GlobalKPIId;
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "GlobalSub-KPI ", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { subKPI }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlobalSubKPIs_Update([DataSourceRequest]DataSourceRequest request, GlobalSubKPI subKPI)
        {
            subKPI.SubKPIName = subKPI.SubKPIName?.ToLower()                  // lowercase
                                     .Trim()                      // trim spaces
                                     .Replace("\r", "")           // remove carriage return
                                     .Replace("\n", "")           // remove new line
                                     .Replace("#", "");           // remove #
            if (db.GlobalSubKPIs
                .Any(x => x.SubKPIName.ToLower().Trim() == subKPI.SubKPIName.ToLower().Trim()
                 &&x.GlobalSubKPIId !=subKPI.GlobalSubKPIId
                 &&x.GlobalKPIId ==subKPI.GlobalKPIId))
            {
                ModelState.AddModelError("exist", "  This Sub-KPI Name is already registered.");
                return Json(new[] { subKPI }.ToDataSourceResult(request, ModelState));

            }
            subKPI.ModifiedBy = User.Identity.Name;
            subKPI.LastModified = DateTime.Now;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(subKPI);
                db.GlobalSubKPIs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "globalSubKPI", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { subKPI }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlobalSubKPIs_Destroy([DataSourceRequest]DataSourceRequest request, GlobalSubKPI subKPI)
        {
            // Check if the GlobalIndicator is being used in any Indicator
            if (db.SubKpiProgressTrackers.Any(x => x.GlobalSubKPIId == subKPI.GlobalSubKPIId))
            {
                ModelState.AddModelError("exist", "You cannot delete this item because it is used in KpI pregess tracker. Please delete it the progess  first.");
                return Json(new[] { subKPI }.ToDataSourceResult(request, ModelState));

            }

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(subKPI);
                #region delete log
                var delete = subKPI.SubKPIName;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "     Objective deleted  " + delete);
                #endregion
                db.GlobalSubKPIs.Attach(entity);
                db.GlobalSubKPIs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { subKPI }.ToDataSourceResult(request, ModelState));
        }
        private GlobalSubKPI GetMappedObject(GlobalSubKPI subKPI)
        {
            var entity = new GlobalSubKPI
            {
                GlobalSubKPIId = subKPI.GlobalSubKPIId,
                GlobalKPIId = subKPI.GlobalKPIId,
                SubKPIName = subKPI.SubKPIName,
                CreatedBy = subKPI.CreatedBy,
                DateCreated = subKPI.DateCreated,
                LastModified = subKPI.LastModified,
                ModifiedBy = subKPI.ModifiedBy,

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
                    Message = "This KPI was not found.",
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
            UserHelper.OperationLog(Request.UserHostAddress, "Global KPI", "Success", User.Identity.Name,
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
