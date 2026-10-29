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

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin,
                    PlanningRoles.Planning_User,
                    HrmConstants.HRDepartmentHead,
                    HrmConstants.HREmployeeSelfService)]
    public class GlobalIndicatorsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["departmentLists"] = db.MainCompanyStructures.Select(x => new
            {
                x.OrgStructureId,
                x.Name
            }).ToList();
            return View();
        }
        
        public ActionResult GlobalIndicators_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GlobalIndicator> globalIndicators = db.GlobalIndicators;
            DataSourceResult result = globalIndicators.ToDataSourceResult(request, globalIndicator => new {
                GlobalIndicatorId = globalIndicator.GlobalIndicatorId,
                OrgStructureId = globalIndicator.OrgStructureId,
                Indicator = globalIndicator.Indicator,
                IsActive = globalIndicator.IsActive,
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
                globalIndicator.IsActive = true;
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
            // Check if the GlobalIndicator is being used in any Indicator
            if (db.Indicators.Any(x => x.GlobalIndicatorId == globalIndicator.GlobalIndicatorId))
            {
                ModelState.AddModelError("exist", "You cannot delete this item because it is used in indicators. Please delete it from those indicators first.");
                
            }

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
                OrgStructureId = globalIndicator.OrgStructureId,
                Indicator = globalIndicator.Indicator,
                IsActive = globalIndicator.IsActive,
                Description = globalIndicator.Description,
                CreatedBy = globalIndicator.CreatedBy,
                DateCreated = globalIndicator.DateCreated,
                LastModified = globalIndicator.LastModified,
                ModifiedBy = globalIndicator.ModifiedBy,

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
