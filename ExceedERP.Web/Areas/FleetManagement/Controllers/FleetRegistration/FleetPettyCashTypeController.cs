﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class FleetPettyCashTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FleetPettyCashTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FleetPettyCashType> fleetpettycashtypes = db.FleetPettyCashTypes;
            DataSourceResult result = fleetpettycashtypes.ToDataSourceResult(request, fleetPettyCashType => new {
                FleetPettyCashTypeId = fleetPettyCashType.FleetPettyCashTypeId,
                Name = fleetPettyCashType.Name,
                Description = fleetPettyCashType.Description,
                OrgBranchName = fleetPettyCashType.OrgBranchName,
                DateCreated = fleetPettyCashType.DateCreated,
                LastModified = fleetPettyCashType.LastModified,
                CreatedBy = fleetPettyCashType.CreatedBy,
                ModifiedBy = fleetPettyCashType.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetPettyCashTypes_Create([DataSourceRequest]DataSourceRequest request, FleetPettyCashType fleetPettyCashType)
        {
            if (ModelState.IsValid)
            {
                var entity = new FleetPettyCashType
                {
                    Name = fleetPettyCashType.Name,
                    Description = fleetPettyCashType.Description,
                    OrgBranchName = fleetPettyCashType.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = fleetPettyCashType.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = fleetPettyCashType.ModifiedBy
                };

                db.FleetPettyCashTypes.Add(entity);
                db.SaveChanges();
                fleetPettyCashType.FleetPettyCashTypeId = entity.FleetPettyCashTypeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet petty cash type registration", "Success", User.Identity.Name, $"created petty cash type {entity.FleetPettyCashTypeId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { fleetPettyCashType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetPettyCashTypes_Update([DataSourceRequest]DataSourceRequest request, FleetPettyCashType fleetPettyCashType)
        {
            if (ModelState.IsValid)
            {
                var entity = new FleetPettyCashType
                {
                    FleetPettyCashTypeId = fleetPettyCashType.FleetPettyCashTypeId,
                    Name = fleetPettyCashType.Name,
                    Description = fleetPettyCashType.Description,
                    OrgBranchName = fleetPettyCashType.OrgBranchName,
                    DateCreated = fleetPettyCashType.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = fleetPettyCashType.ModifiedBy
                };

                db.FleetPettyCashTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet petty cash types registration", "Success", User.Identity.Name, $"updated petty cash types {entity.FleetPettyCashTypeId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { fleetPettyCashType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetPettyCashTypes_Destroy([DataSourceRequest]DataSourceRequest request, FleetPettyCashType fleetPettyCashType)
        {
            if (ModelState.IsValid)
            {
                var entity = new FleetPettyCashType
                {
                    FleetPettyCashTypeId = fleetPettyCashType.FleetPettyCashTypeId,
                    Name = fleetPettyCashType.Name,
                    Description = fleetPettyCashType.Description,
                    OrgBranchName = fleetPettyCashType.OrgBranchName,
                    DateCreated = fleetPettyCashType.DateCreated,
                    LastModified = fleetPettyCashType.LastModified,
                    CreatedBy = fleetPettyCashType.CreatedBy,
                    ModifiedBy = fleetPettyCashType.ModifiedBy
                };

                db.FleetPettyCashTypes.Attach(entity);
                db.FleetPettyCashTypes.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted petty cash type {entity.FleetPettyCashTypeId}");
                #endregion
            }

            return Json(new[] { fleetPettyCashType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
