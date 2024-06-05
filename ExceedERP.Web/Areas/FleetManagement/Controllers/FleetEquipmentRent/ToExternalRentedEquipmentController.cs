﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.DataAccess.Context;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class ToExternalRentedEquipmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExternalRentedEquipments_Read([DataSourceRequest]DataSourceRequest request,string Id)
        {
            IQueryable<ExternalRentedEquipment> externalrentedequipments = db.ExternalRentedEquipments.Where(x => x.RentId.ToString() == Id);
            DataSourceResult result = externalrentedequipments.ToDataSourceResult(request, externalRentedEquipment => new {
                ExternalRentedEquipmentId = externalRentedEquipment.ExternalRentedEquipmentId,
                CurrentOdoMeter = externalRentedEquipment.CurrentOdoMeter,
                EquipmentIdentityID = externalRentedEquipment.EquipmentIdentityID,
                EquipmentTypeID = externalRentedEquipment.EquipmentTypeID,
                AssetCode = externalRentedEquipment.Fleets.PlateNumber,
                FleetId = externalRentedEquipment.FleetId,
                EquipmentName = externalRentedEquipment.Fleets.UnitNum + " - " + externalRentedEquipment.Fleets.SerialNo,
                EquipmentDetail = externalRentedEquipment.EquipmentDetail,
                OperatorName = externalRentedEquipment.OperatorName,
                RentedRate = externalRentedEquipment.RentedRate,
                RentStatus = externalRentedEquipment.RentStatus,
                Status = externalRentedEquipment.Status,
                Remark = externalRentedEquipment.Remark,
                IsVoid = externalRentedEquipment.IsVoid,
                VoidBy = externalRentedEquipment.VoidBy,
                VoidTime = externalRentedEquipment.VoidTime,
                IsOnlineApproved = externalRentedEquipment.IsOnlineApproved,
                OnlineApprovedBy = externalRentedEquipment.OnlineApprovedBy,
                OnlineApprovedTime = externalRentedEquipment.OnlineApprovedTime,
                IsOnlineTransferred = externalRentedEquipment.IsOnlineTransferred,
                IsOnlineTransferredBy = externalRentedEquipment.IsOnlineTransferredBy,
                OnlineTransferredTime = externalRentedEquipment.OnlineTransferredTime,
                IsSendForApproval = externalRentedEquipment.IsSendForApproval,
                OrgBranchName = externalRentedEquipment.OrgBranchName,
                SendForApprovalBy = externalRentedEquipment.SendForApprovalBy,
                SendForApprovalTime = externalRentedEquipment.SendForApprovalTime,
                DateCreated = externalRentedEquipment.DateCreated,
                LastModified = externalRentedEquipment.LastModified,
                CreatedBy = externalRentedEquipment.CreatedBy,
                ModifiedBy = externalRentedEquipment.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalRentedEquipments_Create([DataSourceRequest]DataSourceRequest request, ExternalRentedEquipment externalRentedEquipment, string Id)
        {
            ModelState.Remove("RentId");
            ModelState.Remove("ExternalRentedEquipmentId");

                    IFleetService _FleetService = new FleetService();
                    var Equipment = _FleetService.GetById(externalRentedEquipment.FleetId);

            if (ModelState.IsValid && Equipment != null)
            {
                var entity = new ExternalRentedEquipment
                {
                    ExternalRentedEquipmentId = Guid.NewGuid(),
                    RentId = Guid.Parse(Id),
                    FleetId = externalRentedEquipment.FleetId,
                    EquipmentTypeID = externalRentedEquipment.EquipmentTypeID,
                    EquipmentIdentityID = externalRentedEquipment.EquipmentIdentityID,
                    CurrentOdoMeter = Equipment.CurentOdoMeter.ToString(),
                    AssetCode = Equipment.UnitNum+" - "+Equipment.PlateNumber,
                    EquipmentDetail = externalRentedEquipment.EquipmentDetail,
                    OperatorName = Equipment.OperatorName,
                    RentedRate = Equipment.RentalRate,
                    RentStatus = externalRentedEquipment.RentStatus,
                    Status = externalRentedEquipment.Status,
                    Remark = externalRentedEquipment.Remark,
                    IsVoid = externalRentedEquipment.IsVoid,
                    VoidBy = externalRentedEquipment.VoidBy,
                    VoidTime = externalRentedEquipment.VoidTime,
                    IsOnlineApproved = externalRentedEquipment.IsOnlineApproved,
                    OnlineApprovedBy = externalRentedEquipment.OnlineApprovedBy,
                    OnlineApprovedTime = externalRentedEquipment.OnlineApprovedTime,
                    IsOnlineTransferred = externalRentedEquipment.IsOnlineTransferred,
                    IsOnlineTransferredBy = externalRentedEquipment.IsOnlineTransferredBy,
                    OnlineTransferredTime = externalRentedEquipment.OnlineTransferredTime,
                    IsSendForApproval = externalRentedEquipment.IsSendForApproval,
                    OrgBranchName = externalRentedEquipment.OrgBranchName,
                    SendForApprovalBy = externalRentedEquipment.SendForApprovalBy,
                    SendForApprovalTime = externalRentedEquipment.SendForApprovalTime,
                    DateCreated = externalRentedEquipment.DateCreated,
                    LastModified = externalRentedEquipment.LastModified,
                    CreatedBy = externalRentedEquipment.CreatedBy,
                    ModifiedBy = externalRentedEquipment.ModifiedBy
                };

                db.ExternalRentedEquipments.Add(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "To external rented equipment registration", "Success", User.Identity.Name, $"created external rented equipments reg {entity.ExternalRentedEquipmentId}", Modules.FleetManagement);
                #endregion

                var RentToOther = db.RentToExternals.Where(x => x.RentToExternalID.ToString() == Id).FirstOrDefault();
                if (RentToOther != null)
                {
                    var Fleet = db.Fleets.Where(x => x.FleetsID == entity.FleetId).FirstOrDefault();
                    if (Fleet != null)
                    {
                        Fleet.Location = RentToOther.NameBankBranch;
                        db.Fleets.Attach(Fleet);
                        db.Entry(Fleet).State = EntityState.Modified;
                        db.SaveChanges();
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "Rent to other", "Success", User.Identity.Name, $"created external rented equipments reg {Fleet.FleetsID}", Modules.FleetManagement);
                        #endregion
                    }
                }
                externalRentedEquipment.ExternalRentedEquipmentId = entity.ExternalRentedEquipmentId;
            }

            return Json(new[] { externalRentedEquipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalRentedEquipments_Update([DataSourceRequest]DataSourceRequest request, ExternalRentedEquipment externalRentedEquipment)
        {
             if (ModelState.IsValid)
            {
                var entity = new ExternalRentedEquipment
                {
                    ExternalRentedEquipmentId = externalRentedEquipment.ExternalRentedEquipmentId,
                    CurrentOdoMeter = externalRentedEquipment.CurrentOdoMeter,
                    AssetCode = externalRentedEquipment.AssetCode,
                    EquipmentDetail = externalRentedEquipment.EquipmentDetail,
                    OperatorName = externalRentedEquipment.OperatorName,
                    FleetId = externalRentedEquipment.FleetId,
                    EquipmentTypeID = externalRentedEquipment.EquipmentTypeID,
                    EquipmentIdentityID = externalRentedEquipment.EquipmentIdentityID,
                    RentedRate = externalRentedEquipment.RentedRate,
                    RentStatus = externalRentedEquipment.RentStatus,
                    Status = externalRentedEquipment.Status,
                    Remark = externalRentedEquipment.Remark,
                    IsVoid = externalRentedEquipment.IsVoid,
                    VoidBy = externalRentedEquipment.VoidBy,
                    VoidTime = externalRentedEquipment.VoidTime,
                    IsOnlineApproved = externalRentedEquipment.IsOnlineApproved,
                    OnlineApprovedBy = externalRentedEquipment.OnlineApprovedBy,
                    OnlineApprovedTime = externalRentedEquipment.OnlineApprovedTime,
                    IsOnlineTransferred = externalRentedEquipment.IsOnlineTransferred,
                    IsOnlineTransferredBy = externalRentedEquipment.IsOnlineTransferredBy,
                    OnlineTransferredTime = externalRentedEquipment.OnlineTransferredTime,
                    IsSendForApproval = externalRentedEquipment.IsSendForApproval,
                    OrgBranchName = externalRentedEquipment.OrgBranchName,
                    SendForApprovalBy = externalRentedEquipment.SendForApprovalBy,
                    SendForApprovalTime = externalRentedEquipment.SendForApprovalTime,
                    DateCreated = externalRentedEquipment.DateCreated,
                    LastModified = externalRentedEquipment.LastModified,
                    CreatedBy = externalRentedEquipment.CreatedBy,
                    ModifiedBy = externalRentedEquipment.ModifiedBy
                };

                db.ExternalRentedEquipments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "External rented equipments", "Success", User.Identity.Name, $"updated external rented equipment reg {entity.ExternalRentedEquipmentId} ", Modules.FleetManagement);
                #endregion
                var RentToOther = db.RentToExternals.Where(x => x.RentToExternalID == entity.RentId).FirstOrDefault();
                if (RentToOther != null)
                {
                    var Fleet = db.Fleets.Where(x => x.FleetsID == entity.FleetId).FirstOrDefault();
                    if (Fleet != null)
                    {
                        Fleet.Location = RentToOther.NameBankBranch;


                        db.Fleets.Attach(Fleet);
                        db.Entry(Fleet).State = EntityState.Modified;
                        db.SaveChanges();
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "Rent to others", "Success", User.Identity.Name, $"updated external rented equipment reg {Fleet.FleetsID} ", Modules.FleetManagement);
                        #endregion
                    }
                }
            }

            return Json(new[] { externalRentedEquipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalRentedEquipments_Destroy([DataSourceRequest]DataSourceRequest request, ExternalRentedEquipment externalRentedEquipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ExternalRentedEquipment
                {
                    ExternalRentedEquipmentId = externalRentedEquipment.ExternalRentedEquipmentId,
                    CurrentOdoMeter = externalRentedEquipment.CurrentOdoMeter,
                    AssetCode = externalRentedEquipment.AssetCode,
                    FleetId = externalRentedEquipment.FleetId,
                    EquipmentTypeID = externalRentedEquipment.EquipmentTypeID,
                    EquipmentIdentityID = externalRentedEquipment.EquipmentIdentityID,
                    EquipmentDetail = externalRentedEquipment.EquipmentDetail,
                    OperatorName = externalRentedEquipment.OperatorName,
                    RentedRate = externalRentedEquipment.RentedRate,
                    RentStatus = externalRentedEquipment.RentStatus,
                    Status = externalRentedEquipment.Status,
                    Remark = externalRentedEquipment.Remark,
                    IsVoid = externalRentedEquipment.IsVoid,
                    VoidBy = externalRentedEquipment.VoidBy,
                    VoidTime = externalRentedEquipment.VoidTime,
                    IsOnlineApproved = externalRentedEquipment.IsOnlineApproved,
                    OnlineApprovedBy = externalRentedEquipment.OnlineApprovedBy,
                    OnlineApprovedTime = externalRentedEquipment.OnlineApprovedTime,
                    IsOnlineTransferred = externalRentedEquipment.IsOnlineTransferred,
                    IsOnlineTransferredBy = externalRentedEquipment.IsOnlineTransferredBy,
                    OnlineTransferredTime = externalRentedEquipment.OnlineTransferredTime,
                    IsSendForApproval = externalRentedEquipment.IsSendForApproval,
                    OrgBranchName = externalRentedEquipment.OrgBranchName,
                    SendForApprovalBy = externalRentedEquipment.SendForApprovalBy,
                    SendForApprovalTime = externalRentedEquipment.SendForApprovalTime,
                    DateCreated = externalRentedEquipment.DateCreated,
                    LastModified = externalRentedEquipment.LastModified,
                    CreatedBy = externalRentedEquipment.CreatedBy,
                    ModifiedBy = externalRentedEquipment.ModifiedBy
                };

                db.ExternalRentedEquipments.Attach(entity);
                db.ExternalRentedEquipments.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted external rented equipment {entity.ExternalRentedEquipmentId }");
                #endregion

                var Fleet = db.Fleets.Where(x => x.FleetsID == entity.FleetId).FirstOrDefault();
                if (Fleet != null)
                {
                    Fleet.Location = "HO";


                    db.Fleets.Attach(Fleet);
                    db.Entry(Fleet).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { externalRentedEquipment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
