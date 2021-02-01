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
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.RegistrationUser, FleetManagementRoles.FleetAdmin)]
    public class FleetPettyCashController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Fleets"] = db.Fleets.ToList();
            return View();
        }

        public ActionResult FleetPettyCashes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FleetPettyCash> fleetpettycashes = db.FleetPettyCashes.ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).AsQueryable();
            DataSourceResult result = fleetpettycashes.ToDataSourceResult(request, fleetPettyCash => new
            {
                FleetPettyCashId = fleetPettyCash.FleetPettyCashId,
                Amount = fleetPettyCash.Amount,
                Equipment = fleetPettyCash.Fleet.UnitNum + " - " + fleetPettyCash.Fleet.PlateNumber,
                Description = fleetPettyCash.Description,
                Type = fleetPettyCash.Type,
                Status = fleetPettyCash.Status,
                JobOrderId = fleetPettyCash.JobOrderId,
                Remark = fleetPettyCash.Remark,
                IsVoid = fleetPettyCash.IsVoid,
                FleetsID = fleetPettyCash.FleetsID,
                VoidBy = fleetPettyCash.VoidBy,
                VoidTime = fleetPettyCash.VoidTime,
                IsOnlineApproved = fleetPettyCash.IsOnlineApproved,
                OnlineApprovedBy = fleetPettyCash.OnlineApprovedBy,
                OnlineApprovedTime = fleetPettyCash.OnlineApprovedTime,
                IsOnlineTransferred = fleetPettyCash.IsOnlineTransferred,
                IsOnlineTransferredBy = fleetPettyCash.IsOnlineTransferredBy,
                OnlineTransferredTime = fleetPettyCash.OnlineTransferredTime,
                IsSendForApproval = fleetPettyCash.IsSendForApproval,
                OrgBranchName = fleetPettyCash.OrgBranchName,
                SendForApprovalBy = fleetPettyCash.SendForApprovalBy,
                SendForApprovalTime = fleetPettyCash.SendForApprovalTime,
                DateCreated = fleetPettyCash.DateCreated,
                LastModified = fleetPettyCash.LastModified,
                CreatedBy = fleetPettyCash.CreatedBy,
                ModifiedBy = fleetPettyCash.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetPettyCashes_Create([DataSourceRequest]DataSourceRequest request, FleetPettyCash fleetPettyCash)
        {
            ModelState.Remove("FleetPettyCashId");
            ModelState.Remove("Equipment");
            if (ModelState.IsValid)
            {
                var entity = new FleetPettyCash
                {
                    FleetPettyCashId = Guid.NewGuid(),
                    FleetsID = fleetPettyCash.FleetsID,
                    Amount = fleetPettyCash.Amount,
                    Description = fleetPettyCash.Description,
                    Status = fleetPettyCash.Status,
                    Type = fleetPettyCash.Type,
                    Remark = fleetPettyCash.Remark,
                    IsVoid = fleetPettyCash.IsVoid,
                    VoidBy = fleetPettyCash.VoidBy,
                    VoidTime = fleetPettyCash.VoidTime,
                    IsOnlineApproved = fleetPettyCash.IsOnlineApproved,
                    OnlineApprovedBy = fleetPettyCash.OnlineApprovedBy,
                    OnlineApprovedTime = fleetPettyCash.OnlineApprovedTime,
                    IsOnlineTransferred = fleetPettyCash.IsOnlineTransferred,
                    IsOnlineTransferredBy = fleetPettyCash.IsOnlineTransferredBy,
                    OnlineTransferredTime = fleetPettyCash.OnlineTransferredTime,
                    IsSendForApproval = fleetPettyCash.IsSendForApproval,
                      SendForApprovalBy = fleetPettyCash.SendForApprovalBy,
                    SendForApprovalTime = fleetPettyCash.SendForApprovalTime,
                    DateCreated = DateTime.Now,
                    LastModified = fleetPettyCash.LastModified,
                    CreatedBy = User.Identity.Name,
                       OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    ModifiedBy = fleetPettyCash.ModifiedBy
                };

                db.FleetPettyCashes.Add(entity);
                db.SaveChanges();
                fleetPettyCash.FleetPettyCashId = entity.FleetPettyCashId;
            }

            return Json(new[] { fleetPettyCash }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetPettyCashes_Update([DataSourceRequest]DataSourceRequest request, FleetPettyCash fleetPettyCash)
        {
            if (ModelState.IsValid)
            {
                var entity = new FleetPettyCash
                {
                    FleetPettyCashId = fleetPettyCash.FleetPettyCashId,
                    Amount = fleetPettyCash.Amount,
                    Description = fleetPettyCash.Description,
                    Status = fleetPettyCash.Status,
                    Type = fleetPettyCash.Type,
                    Remark = fleetPettyCash.Remark,
                    IsVoid = fleetPettyCash.IsVoid,
                    VoidBy = fleetPettyCash.VoidBy,
                    VoidTime = fleetPettyCash.VoidTime,
                    IsOnlineApproved = fleetPettyCash.IsOnlineApproved,
                    OnlineApprovedBy = fleetPettyCash.OnlineApprovedBy,
                    OnlineApprovedTime = fleetPettyCash.OnlineApprovedTime,
                    IsOnlineTransferred = fleetPettyCash.IsOnlineTransferred,
                    IsOnlineTransferredBy = fleetPettyCash.IsOnlineTransferredBy,
                    OnlineTransferredTime = fleetPettyCash.OnlineTransferredTime,
                    IsSendForApproval = fleetPettyCash.IsSendForApproval,
                      SendForApprovalBy = fleetPettyCash.SendForApprovalBy,
                    SendForApprovalTime = fleetPettyCash.SendForApprovalTime,
                    DateCreated = fleetPettyCash.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = fleetPettyCash.CreatedBy,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    ModifiedBy = User.Identity.Name
                };

                db.FleetPettyCashes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { fleetPettyCash }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetPettyCashes_Destroy([DataSourceRequest]DataSourceRequest request, FleetPettyCash fleetPettyCash)
        {
            if (ModelState.IsValid)
            {
                var entity = new FleetPettyCash
                {
                    FleetPettyCashId = fleetPettyCash.FleetPettyCashId,
                    Amount = fleetPettyCash.Amount,
                    Description = fleetPettyCash.Description,
                    Status = fleetPettyCash.Status,
                    Remark = fleetPettyCash.Remark,
                    IsVoid = fleetPettyCash.IsVoid,
                    VoidBy = fleetPettyCash.VoidBy,
                    VoidTime = fleetPettyCash.VoidTime,
                    IsOnlineApproved = fleetPettyCash.IsOnlineApproved,
                    OnlineApprovedBy = fleetPettyCash.OnlineApprovedBy,
                    OnlineApprovedTime = fleetPettyCash.OnlineApprovedTime,
                    IsOnlineTransferred = fleetPettyCash.IsOnlineTransferred,
                    IsOnlineTransferredBy = fleetPettyCash.IsOnlineTransferredBy,
                    OnlineTransferredTime = fleetPettyCash.OnlineTransferredTime,
                    IsSendForApproval = fleetPettyCash.IsSendForApproval,
                    OrgBranchName = fleetPettyCash.OrgBranchName,
                    SendForApprovalBy = fleetPettyCash.SendForApprovalBy,
                    SendForApprovalTime = fleetPettyCash.SendForApprovalTime,
                    DateCreated = fleetPettyCash.DateCreated,
                    LastModified = fleetPettyCash.LastModified,
                    CreatedBy = fleetPettyCash.CreatedBy,
                    ModifiedBy = fleetPettyCash.ModifiedBy
                };

                db.FleetPettyCashes.Attach(entity);
                db.FleetPettyCashes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { fleetPettyCash }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
