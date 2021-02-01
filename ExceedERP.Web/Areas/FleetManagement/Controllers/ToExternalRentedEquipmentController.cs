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

                var RentToOther = db.RentOtherSuppliers.Where(x => x.RentOtherSuppliersID.ToString() == Id).FirstOrDefault();
                if (RentToOther != null)
                {
                    var Fleet = db.Fleets.Where(x => x.FleetsID == entity.FleetId).FirstOrDefault();
                    if (Fleet != null)
                    {
                        Fleet.Location = RentToOther.NameBankBranch;


                        db.Fleets.Attach(Fleet);
                        db.Entry(Fleet).State = EntityState.Modified;
                        db.SaveChanges();
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
                var RentToOther = db.RentOtherSuppliers.Where(x => x.RentOtherSuppliersID  == entity.RentId).FirstOrDefault();
                if (RentToOther != null)
                {
                    var Fleet = db.Fleets.Where(x => x.FleetsID == entity.FleetId).FirstOrDefault();
                    if (Fleet != null)
                    {
                        Fleet.Location = RentToOther.NameBankBranch;


                        db.Fleets.Attach(Fleet);
                        db.Entry(Fleet).State = EntityState.Modified;
                        db.SaveChanges();
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
