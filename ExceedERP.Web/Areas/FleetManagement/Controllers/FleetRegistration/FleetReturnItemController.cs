﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.RegistrationUser, FleetManagementRoles.FleetAdmin)]
    public class FleetReturnItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FleetReturnItems_Read([DataSourceRequest]DataSourceRequest request, string Id)
        {
            IQueryable<FleetReturnItem> FleetReturnItems = db.FleetReturnItems.Where(x => x.FleetReturnId.ToString() == Id);
            DataSourceResult result = FleetReturnItems.ToDataSourceResult(request);

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetReturnItems_Create([DataSourceRequest]DataSourceRequest request, FleetReturnItem FleetReturnItem, string Id)
        {
            if (ModelState.IsValid)
            {
                FleetReturnItem.FleetReturnId = int.Parse(Id);
                FleetReturnItem.DateCreated = DateTime.Now;
                FleetReturnItem.LastModified = FleetReturnItem.LastModified;
                FleetReturnItem.CreatedBy = User.Identity.Name;
                FleetReturnItem.ModifiedBy = FleetReturnItem.ModifiedBy;
            };

            db.FleetReturnItems.Add(FleetReturnItem);
            db.SaveChanges();
            FleetReturnItem.FleetReturnItemId = FleetReturnItem.FleetReturnItemId;
            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "Fleet return item registration", "Success", User.Identity.Name, $"created fleet return item {FleetReturnItem.FleetReturnItemId}", Modules.FleetManagement);
            #endregion


            return Json(new[] { FleetReturnItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetReturnItems_Update([DataSourceRequest]DataSourceRequest request, FleetReturnItem FleetReturnItem)
        {
            if (ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {

                    FleetReturnItem.LastModified = DateTime.Now;
                    FleetReturnItem.ModifiedBy = User.Identity.Name;
                };
                db.FleetReturnItems.Attach(FleetReturnItem);
                db.Entry(FleetReturnItem).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet return item registration", "Success", User.Identity.Name, $"updated fleet return item {FleetReturnItem.FleetReturnItemId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { FleetReturnItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetReturnItems_Destroy([DataSourceRequest]DataSourceRequest request, FleetReturnItem FleetReturnItem)
        {
            if (ModelState.IsValid)
            {


                db.FleetReturnItems.Attach(FleetReturnItem);
                db.FleetReturnItems.Remove(FleetReturnItem);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted fleet return item {FleetReturnItem.FleetReturnItemId }");
                #endregion
            }

            return Json(new[] { FleetReturnItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
