﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class CannibalizationDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CannibalizationDetails_Read([DataSourceRequest]DataSourceRequest request, Guid Id)
        {
            IQueryable<CannibalizationDetail> cannibalizationdetails = db.CannibalizationDetails.Where(x=>x.CannibalizationId == Id);
            DataSourceResult result = cannibalizationdetails.ToDataSourceResult(request, cannibalizationDetail => new {
                CannibalizationDetailId = cannibalizationDetail.CannibalizationDetailId,
                Model = cannibalizationDetail.Model,
                PartType = cannibalizationDetail.PartType,
                UnitofMeasurement = cannibalizationDetail.UnitofMeasurement,
                PartName = cannibalizationDetail.PartName,
                WHPRNo = cannibalizationDetail.WHPRNo,
                Quantity = cannibalizationDetail.Quantity,
                Status = cannibalizationDetail.Status,
                TakenBy = cannibalizationDetail.TakenBy,
                Description = cannibalizationDetail.Description,
                Remark = cannibalizationDetail.Remark,
                IsVoid = cannibalizationDetail.IsVoid,
                VoidBy = cannibalizationDetail.VoidBy,
                VoidTime = cannibalizationDetail.VoidTime,
                IsOnlineApproved = cannibalizationDetail.IsOnlineApproved,
                OnlineApprovedBy = cannibalizationDetail.OnlineApprovedBy,
                OnlineApprovedTime = cannibalizationDetail.OnlineApprovedTime,
                IsOnlineTransferred = cannibalizationDetail.IsOnlineTransferred,
                IsOnlineTransferredBy = cannibalizationDetail.IsOnlineTransferredBy,
                OnlineTransferredTime = cannibalizationDetail.OnlineTransferredTime,
                IsSendForApproval = cannibalizationDetail.IsSendForApproval,
                OrgBranchName = cannibalizationDetail.OrgBranchName,
                SendForApprovalBy = cannibalizationDetail.SendForApprovalBy,
                SendForApprovalTime = cannibalizationDetail.SendForApprovalTime,
                DateCreated = cannibalizationDetail.DateCreated,
                LastModified = cannibalizationDetail.LastModified,
                CreatedBy = cannibalizationDetail.CreatedBy,
                ModifiedBy = cannibalizationDetail.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CannibalizationDetails_Create( [DataSourceRequest]DataSourceRequest request, CannibalizationDetail cannibalizationDetail, Guid Id)
        {
            ModelState.Remove("CannibalizationDetailId");
            ModelState.Remove("CannibalizationId");
            ModelState.Remove("UnitofMeasurement");
            if (ModelState.IsValid)
            {
                var entity = new CannibalizationDetail
                {
                    CannibalizationDetailId = Guid.NewGuid(),
                    CannibalizationId = Id,
                    Model = cannibalizationDetail.Model,
                    UnitofMeasurement = cannibalizationDetail.UnitofMeasurement,
                    PartName = cannibalizationDetail.PartName,
                    PartType = cannibalizationDetail.PartType,
                    WHPRNo = cannibalizationDetail.WHPRNo,
                    Quantity = cannibalizationDetail.Quantity,
                    Status = cannibalizationDetail.Status,
                    TakenBy = cannibalizationDetail.TakenBy,
                    Description = cannibalizationDetail.Description,
                    Remark = cannibalizationDetail.Remark,
                    IsVoid = cannibalizationDetail.IsVoid,
                    VoidBy = cannibalizationDetail.VoidBy,
                    VoidTime = cannibalizationDetail.VoidTime,
                    IsOnlineApproved = cannibalizationDetail.IsOnlineApproved,
                    OnlineApprovedBy = cannibalizationDetail.OnlineApprovedBy,
                    OnlineApprovedTime = cannibalizationDetail.OnlineApprovedTime,
                    IsOnlineTransferred = cannibalizationDetail.IsOnlineTransferred,
                    IsOnlineTransferredBy = cannibalizationDetail.IsOnlineTransferredBy,
                    OnlineTransferredTime = cannibalizationDetail.OnlineTransferredTime,
                    IsSendForApproval = cannibalizationDetail.IsSendForApproval,
                    OrgBranchName = cannibalizationDetail.OrgBranchName,
                    SendForApprovalBy = cannibalizationDetail.SendForApprovalBy,
                    SendForApprovalTime = cannibalizationDetail.SendForApprovalTime,
                    DateCreated = cannibalizationDetail.DateCreated,
                    LastModified = cannibalizationDetail.LastModified,
                    CreatedBy = cannibalizationDetail.CreatedBy,
                    ModifiedBy = cannibalizationDetail.ModifiedBy
                };

                db.CannibalizationDetails.Add(entity);
                db.SaveChanges();
                cannibalizationDetail.CannibalizationDetailId = entity.CannibalizationDetailId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Canibalization detail registration", "Success", User.Identity.Name, $"created canibalization detail {entity.CannibalizationDetailId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { cannibalizationDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CannibalizationDetails_Update([DataSourceRequest]DataSourceRequest request, CannibalizationDetail cannibalizationDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new CannibalizationDetail
                {
                    CannibalizationDetailId = cannibalizationDetail.CannibalizationDetailId,
                    Model = cannibalizationDetail.Model,
                    UnitofMeasurement = cannibalizationDetail.UnitofMeasurement,
                    PartName = cannibalizationDetail.PartName,
                    WHPRNo = cannibalizationDetail.WHPRNo,
                    Quantity = cannibalizationDetail.Quantity,
                    Status = cannibalizationDetail.Status,
                    TakenBy = cannibalizationDetail.TakenBy,
                    Description = cannibalizationDetail.Description,
                    Remark = cannibalizationDetail.Remark,
                    IsVoid = cannibalizationDetail.IsVoid,
                    VoidBy = cannibalizationDetail.VoidBy,
                    VoidTime = cannibalizationDetail.VoidTime,
                    IsOnlineApproved = cannibalizationDetail.IsOnlineApproved,
                    OnlineApprovedBy = cannibalizationDetail.OnlineApprovedBy,
                    OnlineApprovedTime = cannibalizationDetail.OnlineApprovedTime,
                    IsOnlineTransferred = cannibalizationDetail.IsOnlineTransferred,
                    IsOnlineTransferredBy = cannibalizationDetail.IsOnlineTransferredBy,
                    OnlineTransferredTime = cannibalizationDetail.OnlineTransferredTime,
                    IsSendForApproval = cannibalizationDetail.IsSendForApproval,
                    OrgBranchName = cannibalizationDetail.OrgBranchName,
                    SendForApprovalBy = cannibalizationDetail.SendForApprovalBy,
                    SendForApprovalTime = cannibalizationDetail.SendForApprovalTime,
                    DateCreated = cannibalizationDetail.DateCreated,
                    LastModified = cannibalizationDetail.LastModified,
                    CreatedBy = cannibalizationDetail.CreatedBy,
                    ModifiedBy = cannibalizationDetail.ModifiedBy
                };

                db.CannibalizationDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Canibalization detail registration", "Success", User.Identity.Name, $"updated canibalization detail {entity.CannibalizationDetailId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { cannibalizationDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CannibalizationDetails_Destroy([DataSourceRequest]DataSourceRequest request, CannibalizationDetail cannibalizationDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new CannibalizationDetail
                {
                    CannibalizationDetailId = cannibalizationDetail.CannibalizationDetailId,
                    Model = cannibalizationDetail.Model,
                    UnitofMeasurement = cannibalizationDetail.UnitofMeasurement,
                    PartName = cannibalizationDetail.PartName,
                    WHPRNo = cannibalizationDetail.WHPRNo,
                    Quantity = cannibalizationDetail.Quantity,
                    Status = cannibalizationDetail.Status,
                    TakenBy = cannibalizationDetail.TakenBy,
                    Description = cannibalizationDetail.Description,
                    Remark = cannibalizationDetail.Remark,
                    IsVoid = cannibalizationDetail.IsVoid,
                    VoidBy = cannibalizationDetail.VoidBy,
                    VoidTime = cannibalizationDetail.VoidTime,
                    IsOnlineApproved = cannibalizationDetail.IsOnlineApproved,
                    OnlineApprovedBy = cannibalizationDetail.OnlineApprovedBy,
                    OnlineApprovedTime = cannibalizationDetail.OnlineApprovedTime,
                    IsOnlineTransferred = cannibalizationDetail.IsOnlineTransferred,
                    IsOnlineTransferredBy = cannibalizationDetail.IsOnlineTransferredBy,
                    OnlineTransferredTime = cannibalizationDetail.OnlineTransferredTime,
                    IsSendForApproval = cannibalizationDetail.IsSendForApproval,
                    OrgBranchName = cannibalizationDetail.OrgBranchName,
                    SendForApprovalBy = cannibalizationDetail.SendForApprovalBy,
                    SendForApprovalTime = cannibalizationDetail.SendForApprovalTime,
                    DateCreated = cannibalizationDetail.DateCreated,
                    LastModified = cannibalizationDetail.LastModified,
                    CreatedBy = cannibalizationDetail.CreatedBy,
                    ModifiedBy = cannibalizationDetail.ModifiedBy
                };

                db.CannibalizationDetails.Attach(entity);
                db.CannibalizationDetails.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted cannibalization detail {entity.CannibalizationDetailId }");
                #endregion
            }

            return Json(new[] { cannibalizationDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
