using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRLeaveUser)]
    public class LeaveEntitlementHistoryController : BaseController
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

       

        #region Read
        public ActionResult LeaveEntitlementHistory_Read([DataSourceRequest] DataSourceRequest request,int empId)
        {
            IQueryable<LeaveEntitlementHistory> histories = db.LeaveEntitlementHistories.Where(x=>x.EmployeeId==empId);

            DataSourceResult result = histories.ToDataSourceResult(request, h => new
            {
                h.LeaveEntitlementHistoryId,
                h.GlFiscalYearId,
                h.EmployeeId,
                h.Entitlement,
                h.Balance,
                h.DateCreated,
                h.CreatedBy,
                h.LastModified
            });

            return Json(result);
        }
        #endregion

        #region Create
        [HttpPost]
        public ActionResult LeaveEntitlementHistory_Create([DataSourceRequest] DataSourceRequest request, LeaveEntitlementHistory model,int empId)
        {
            bool exists = db.LeaveEntitlementHistories.Any(x =>
                x.EmployeeId == model.EmployeeId &&
                x.GlFiscalYearId == model.GlFiscalYearId);

            if (exists)
            {
                ModelState.AddModelError("", "Record already exists for this employee and fiscal year.");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            model.CreatedBy = User.Identity.Name;
            model.DateCreated = DateTime.Now;
            model.EmployeeId = empId;
            if (ModelState.IsValid)
            {
                var entity = Map(model);
                db.LeaveEntitlementHistories.Add(entity);
                db.SaveChanges();

                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Leave Entitlement History",
                    "Create",
                    User.Identity.Name,
                    "Leave Entitlement History created",
                    Modules.HRM);
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Update
        [HttpPost]
        public ActionResult LeaveEntitlementHistory_Update([DataSourceRequest] DataSourceRequest request,LeaveEntitlementHistory model)
        {
            bool exists = db.LeaveEntitlementHistories.Any(x =>
                x.EmployeeId == model.EmployeeId &&
                x.GlFiscalYearId == model.GlFiscalYearId &&
                x.LeaveEntitlementHistoryId != model.LeaveEntitlementHistoryId);

            if (exists)
            {
                ModelState.AddModelError("", "Duplicate record detected.");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            model.ModifiedBy = User.Identity.Name;
            model.LastModified = DateTime.Now;
            if (ModelState.IsValid)
            {
                var entity = Map(model);
                db.LeaveEntitlementHistories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Leave Entitlement History",
                    "Update",
                    User.Identity.Name,
                    "Leave Entitlement History updated",
                    Modules.HRM);
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult LeaveEntitlementHistory_Destroy( [DataSourceRequest] DataSourceRequest request,LeaveEntitlementHistory model)
        {
            if (ModelState.IsValid)
            {
                var entity = Map(model);
                db.LeaveEntitlementHistories.Attach(entity);
                db.LeaveEntitlementHistories.Remove(entity);
                db.SaveChanges();

                UserHelper.DeleteLog(
                    Request.UserHostAddress,
                    "Leave Entitlement History",
                    "Delete",
                    User.Identity.Name,
                    $"Deleted Leave Entitlement History for EmployeeId {model.EmployeeId}");
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Mapping
        private LeaveEntitlementHistory Map(LeaveEntitlementHistory model)
        {
            return new LeaveEntitlementHistory
            {
                LeaveEntitlementHistoryId = model.LeaveEntitlementHistoryId,
                GlFiscalYearId = model.GlFiscalYearId,
                EmployeeId = model.EmployeeId,
                Entitlement = model.Entitlement,
                Balance = model.Balance,
                DateCreated = model.DateCreated,
                CreatedBy = model.CreatedBy,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy
            };
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
