﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class VehicleReceptionFormController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
       
        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers.ToList();
            ViewData["Employee"] = db.Person_Employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).ToList();
            ViewData["VehiclePartDamageType"] = db.VehiclePartDamageType.ToList();
            ViewData["VehicleParts"] = db.VehiclePart.ToList();
            ViewData["category"] = db.EquipmentIdentity.Select(s => new {
                Id = s.EquipmentIdentityID,
                Name = s.EquipmentCategoryName
            }).ToList();
            return View();
        }

        public ActionResult Print(string id)
        {
          
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.MVInspectionAndHandingOverCheckList, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            ViewBag.id = id;
            return View();
        }
        public ActionResult VehicleReceptionForm_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<VehicleReceptionForm> vehiclereceptionform = db.VehicleReceptionForm.Where(x=>x.SitesVehicleTransferID==null);
            DataSourceResult result = vehiclereceptionform.ToDataSourceResult(request, vehicleReceptionForm => new {
                VehicleReceptionFormId = vehicleReceptionForm.VehicleReceptionFormId,
                Source= vehicleReceptionForm.Source,
                CustomerId = vehicleReceptionForm.CustomerId,
                VehicleCategoryId = vehicleReceptionForm.VehicleCategoryId,
                RecipientId = vehicleReceptionForm.RecipientId,
                TechnicianAssignedId = vehicleReceptionForm.TechnicianAssignedId,
                PlateNo = vehicleReceptionForm.PlateNo,
                VehicleType = vehicleReceptionForm.VehicleType,
                VehicleModel = vehicleReceptionForm.VehicleModel,
                Mileage = vehicleReceptionForm.Mileage,
                Fuel = vehicleReceptionForm.Fuel,
                Remark = vehicleReceptionForm.Remark,
                DateCreated = vehicleReceptionForm.DateCreated,
                ReferenceNumber = vehicleReceptionForm.ReferenceNumber,
                LastModified = vehicleReceptionForm.LastModified,
                CreatedBy = vehicleReceptionForm.CreatedBy,
                ModifiedBy = vehicleReceptionForm.ModifiedBy,
                FromRequest=vehicleReceptionForm.FromRequest,
                IsSendForApproval = vehicleReceptionForm.IsSendForApproval,
                OnlineApprovedTime = vehicleReceptionForm.OnlineApprovedTime,
                SendForApprovalTime = vehicleReceptionForm.SendForApprovalTime,
                IsOnlineApproved = vehicleReceptionForm.IsOnlineApproved,
            });

            return Json(result);
        }

        public ActionResult VehicleReceptionForm_Create([DataSourceRequest]DataSourceRequest request, VehicleReceptionForm vehicleReceptionForm)
        {
            
            if (ModelState.IsValid)
            {
                var vehicle = db.EquipmentInformation.Where(q => q.LicensePlate == vehicleReceptionForm.PlateNo).FirstOrDefault();
                if(vehicle!=null)
                    {
                    vehicleReceptionForm.VehicleModel = vehicle.DeviceNumber ;
                    vehicleReceptionForm.VehicleType = vehicle.GroupName ;
                }
                if (vehicleReceptionForm.Source == Source.External && vehicleReceptionForm.CustomerId == null)
                    ModelState.AddModelError("CustomerNull", "Please select customer !");
                var entity = new VehicleReceptionForm
                {
                    VehicleReceptionFormId = vehicleReceptionForm.VehicleReceptionFormId,
                    Source = vehicleReceptionForm.Source,
                    CustomerId = vehicleReceptionForm.CustomerId,
                    VehicleCategoryId = vehicleReceptionForm.VehicleCategoryId,
                    Mileage = vehicleReceptionForm.Mileage,
                    PlateNo = vehicleReceptionForm.PlateNo,
                    VehicleType = vehicleReceptionForm.VehicleType,
                    Fuel = vehicleReceptionForm.Fuel,
                    TechnicianAssignedId = vehicleReceptionForm.TechnicianAssignedId,
                    VehicleModel = vehicleReceptionForm.VehicleModel,
                    Remark = vehicleReceptionForm.Remark,
                    DateCreated=DateTime.Now,
                    CreatedBy=User.Identity.Name,

                };


                db.VehicleReceptionForm.Add(entity);
                db.SaveChanges();

                var parts = db.VehiclePart.Where(x=>x.VehicleCategoryId== vehicleReceptionForm.VehicleCategoryId).ToList();
                IList<VehiclePartManagement> vehiclePartManagements = new List<VehiclePartManagement>();
                foreach(var part in parts)
                {
                    var vehiclePartManagement = new VehiclePartManagement();
                    vehiclePartManagement.VehiclePartId = part.VehiclePartId;
                    vehiclePartManagement.VehicleReceptionFormId = entity.VehicleReceptionFormId;
                    db.VehiclePartManagement.Add(vehiclePartManagement);
                }
                db.SaveChanges();
                vehicleReceptionForm.VehicleReceptionFormId = entity.VehicleReceptionFormId;
            }

            return Json(new[] { vehicleReceptionForm }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleReceptionForm_Update([DataSourceRequest]DataSourceRequest request, VehicleReceptionForm vehicleReceptionForm)
        {

            if (ModelState.IsValid)
            {
                var vehicle = db.EquipmentInformation.Where(q => q.LicensePlate == vehicleReceptionForm.PlateNo).FirstOrDefault();
                if (vehicle != null)
                {
                    vehicleReceptionForm.VehicleModel = vehicle.DeviceNumber;
                    vehicleReceptionForm.VehicleType = vehicle.GroupName;
                }
                if (vehicleReceptionForm.Source==Source.External && vehicleReceptionForm.CustomerId == null)
                    ModelState.AddModelError("CustomerNull", "Please select customer !");
                var entity = new VehicleReceptionForm
                {

                    VehicleReceptionFormId = vehicleReceptionForm.VehicleReceptionFormId,
                    Source = vehicleReceptionForm.Source,
                    CustomerId = vehicleReceptionForm.CustomerId,
                    VehicleCategoryId = vehicleReceptionForm.VehicleCategoryId,
                    Mileage = vehicleReceptionForm.Mileage,
                    PlateNo = vehicleReceptionForm.PlateNo,
                    VehicleType = vehicleReceptionForm.VehicleType,
                    Fuel = vehicleReceptionForm.Fuel,
                    TechnicianAssignedId = vehicleReceptionForm.TechnicianAssignedId,
                    VehicleModel = vehicleReceptionForm.VehicleModel,
                    Remark = vehicleReceptionForm.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name


                };
                db.VehicleReceptionForm.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { vehicleReceptionForm }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Admin_Read([DataSourceRequest] DataSourceRequest request, int Id)
        {
            var Admin = new List<VehicleReceptionApproval>();
            var Request = db.VehicleReceptionForm.Where(x => x.VehicleReceptionFormId == Id).FirstOrDefault();
            

                Admin =
                    db.VehicleReceptionApprovals
                        .Where(x => x.VehicleReceptionFormId == Id)
                        .ToList();
            

            if (!Request.IsOnlineApproved&&Request.IsSendForApproval&&!Admin.Any())
            {
                var adminApproval = new VehicleReceptionApproval();
                adminApproval.VehicleReceptionApprovalId = 0;
                adminApproval.VehicleReceptionFormId =Id;
                adminApproval.Status = ApprovalStatuses.Pending;
                adminApproval.Remark = string.Empty;
                adminApproval.CreatedBy = User.Identity.Name;
                adminApproval.DateCreated = DateTime.Now;
                db.VehicleReceptionApprovals.Add(adminApproval);
                db.SaveChanges();

            }
            return Json(Admin.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleReceptionForm_Destroy([DataSourceRequest]DataSourceRequest request, VehicleReceptionForm VehicleReceptionForm)
        {

            if (ModelState.IsValid)
            {
                var entity = (VehicleReceptionForm)this.GetObject(VehicleReceptionForm);

                db.VehicleReceptionForm.Attach(entity);
                db.VehicleReceptionForm.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { VehicleReceptionForm }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(VehicleReceptionForm vehicleReceptionForm)
        {
            var userName = User.Identity.Name;

            var entity = new VehicleReceptionForm
            {
                VehicleReceptionFormId = vehicleReceptionForm.VehicleReceptionFormId,
                CustomerId = vehicleReceptionForm.CustomerId,
                RecipientId = UserInformationHelper.GetEmployeeId(userName),
                TechnicianAssignedId = vehicleReceptionForm.TechnicianAssignedId,
                PlateNo = vehicleReceptionForm.PlateNo,
                VehicleType = vehicleReceptionForm.VehicleType,
                VehicleCategoryId = vehicleReceptionForm.VehicleCategoryId,
                VehicleModel = vehicleReceptionForm.VehicleModel,
                Mileage = vehicleReceptionForm.Mileage,
                Fuel = vehicleReceptionForm.Fuel,
                Remark = vehicleReceptionForm.Remark,
                DateCreated = vehicleReceptionForm.DateCreated,
                LastModified = vehicleReceptionForm.LastModified,
                CreatedBy = vehicleReceptionForm.CreatedBy,
                ModifiedBy = vehicleReceptionForm.ModifiedBy

            };
            return entity;
        }
        public JsonResult SendReceptionForApproval(int id)
        {
            var reception = db.VehicleReceptionForm.FirstOrDefault(x => x.VehicleReceptionFormId == id);
            var parts = db.VehiclePartManagement.Where(x => x.VehicleReceptionFormId == id).ToList();

            if (!parts.Any())
            {
                return Json(new
                {
                    Message = "Please fill in parts first.",
                    Status = "NOTOK"
                }, JsonRequestBehavior.AllowGet);
            }

            if (reception != null)
            {
                reception.IsSendForApproval = true;
                reception.SendForApprovalTime = DateTime.Now;

                db.Entry(reception).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new
                {
                    Message = "Request sent successfully.",
                    Status = "OK"
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Message = "Reception form not found.",
                Status = "NOTOK"
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult sendBackToRequest(string Id)
        {
            var JobRequest = db.VehicleReceptionForm.FirstOrDefault(x => x.VehicleReceptionFormId.ToString() == Id);
            if (JobRequest != null)
            {
                // JobRequest.Status = MaintenanceConst.WaitingAdminstrative;
                JobRequest.IssentToBackToJobRequest = true;
                var formattedData = new
                {
                    Message = "Successfully Sent",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            }
          
            return null;
        }
        public ActionResult ApproveJobRequest(int id)
        {
            var FetchApproval = db.VehicleReceptionApprovals.FirstOrDefault(x => x.VehicleReceptionApprovalId == id);
            var Reception = db.VehicleReceptionForm.FirstOrDefault(x => x.VehicleReceptionFormId == FetchApproval.VehicleReceptionFormId);
            var Request = db.JobRequest.FirstOrDefault(x => x.JobRequestId.ToString() == Reception.JobRequestId);

            Reception.IsOnlineApproved = true;
            Reception.OnlineApprovedBy = User.Identity.Name;
            Reception.OnlineApprovedTime = DateTime.Now;
            Reception.Status = ApprovalStatuses.Approve.ToString();
                db.VehicleReceptionForm.Attach(Reception);
                db.Entry(Request).State = EntityState.Modified;
                db.SaveChanges();
                FetchApproval.Status = ApprovalStatuses.Approve;
                FetchApproval.LastModified = DateTime.Now;
                FetchApproval.ModifiedBy =User.Identity.Name;
                db.Entry(FetchApproval).State = EntityState.Modified;
                db.SaveChanges();
            var FetchCause = db.JobCauses.Where(x => x.OrderId == Request.JobRequestId).ToList();
            var Count = 0;
            foreach(var cause in FetchCause)
            {
                if(cause.FromReceptionCount>0)
                {
                    Count++;
                }
            }
            if(Count==0)
            { 
            string Odometer = string.Empty;
            string EquipmentType = string.Empty;
            string AssetNum = string.Empty;
            string PlateNum = string.Empty;
            Guid EquipmentTypeId = Guid.Empty;

                if (Request != null/* && fleet != null*/)
                {

                    //if (model.Status == ApprovalStatuses.Approve)
                    //{
                    var JobOrder = new JobOrder();
                    var maintenanceCenter = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID == Request.MaintenanceCenterId);
                    var fleet = db.Fleets.FirstOrDefault(x => x.PlateNumber == Request.LicensePlate || x.SerialNo == Request.LicensePlate);
                    JobOrder.JobOrderID = Guid.NewGuid();
                    JobOrder.EquipmentInformationId = Request.EquipmentInformationId == null ? Guid.Empty : Request.EquipmentInformationId.Value;
                    //JobOrder.ProjectName = Request.ProjectName;
                    JobOrder.CurrentFuel = Request.FuelAmount;
                    JobOrder.LicensePlate = Request.LicensePlate;
                    JobOrder.FleetsID = fleet?.FleetsID ?? Guid.Empty;
                    JobOrder.EquipmentName = Request.EquipmentName;
                    JobOrder.OperatorName = Request.OperatorName;
                    JobOrder.AssetNum = AssetNum;
                    JobOrder.Referance = Request.Num;
                    JobOrder.IsPM = Request.IsPM;
                    JobOrder.Odometer = Request.Odometer;
                    JobOrder.EquipmentType = EquipmentType;
                    //JobOrder.EquipmentTypeId = EquipmentTypeId;
                    JobOrder.MaintenanceCenter = maintenanceCenter.Name; // just name
                    JobOrder.MaintenanceCenterId = Request.MaintenanceCenterId; // Added Field to track maintenanceCenter
                    JobOrder.PercentCompleted = string.Empty;
                    JobOrder.Odometer = Request.Odometer;
                    JobOrder.InspectorComments = string.Empty;
                    JobOrder.DateCompleted = DateTime.Now;
                    JobOrder.DateCreated = DateTime.Now;
                    JobOrder.LabourCost = 0;
                    JobOrder.SpareCost = 0;
                    JobOrder.IsExternal = Request.Source == JobRequestSource.ExternalVehicle ? true : false;
                    JobOrder.DateIssued = DateTime.Now;
                    JobOrder.Type = string.Empty;
                    JobOrder.EquipmentNo = PlateNum;
                    JobOrder.InspectedBy = Request.Requester;
                    JobOrder.Status = MaintenanceConst.Pending;
                    //JobOrder.Site = Request.ProjectName;
                    JobOrder.SendForApprovalTime = DateTime.Now;
                    JobOrder.OnlineApprovedTime = DateTime.Now;
                    JobOrder.IsSentToWorkShop = false;
                    JobOrder.IsSubmittedToServiceCenter = false;
                    JobOrder.CreatedBy = User.Identity.Name;
                    JobOrder.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    JobOrder.OrgStructureId = Request.OrgStructureId;
                    JobOrder.BranchId = Request.BranchId;
                    JobOrder.BusinessUnitId = Request.BusinessUnitId;
                    JobOrder.RequesterId = Request.RequesterId;
                    db.JobOrder.Add(JobOrder);
                    db.SaveChanges();

                    //Fleet.Status = MaintenanceConst.OnMaintenance;
                    //Fleet.CurentOdoMeter = Request.Odometer;

                    //_FleetService.Update(Fleet);

                    Request.IsSecondOnlineApproved = true;
                    Request.SecondSendForApprovalBy = User.Identity.Name;
                    Request.SecondOnlineApprovedTime = DateTime.Now;
                    Request.JobOrderStatus = JobOrderStatus.Progress;

                    Request.Status = ApprovalStatuses.Approve.ToString();

                    db.JobRequest.Attach(Request);
                    db.Entry(Request).State = EntityState.Modified;
                    db.SaveChanges();
                }  
            }
            else
            {
                var FetchRequest = db.JobRequest.FirstOrDefault(x => x.JobRequestId.ToString() == Reception.JobRequestId);

                FetchRequest.CreateJobOrder = true;
                db.Entry(FetchRequest).State = EntityState.Modified;
                db.SaveChanges();
            }
            var formattedData = new
            {
                Message = "Approved Successefuly.",
                Status = "Approved"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RejectJobRequest(int id)

        {

            var FetchApproval = db.VehicleReceptionApprovals.FirstOrDefault(x => x.VehicleReceptionApprovalId == id);
            var Request = db.VehicleReceptionForm.FirstOrDefault(x => x.VehicleReceptionFormId == FetchApproval.VehicleReceptionFormId);


           // Request.IsOnlineApproved = true;
          //Request.VoidBy = User.Identity.Name;
            Request.OnlineApprovedTime = DateTime.Now;
            Request.Status = ApprovalStatuses.Reject.ToString();

            db.VehicleReceptionForm.Attach(Request);
            db.Entry(Request).State = EntityState.Modified;
            db.SaveChanges();
            FetchApproval.Status = ApprovalStatuses.Reject;
            FetchApproval.LastModified = DateTime.Now;
            FetchApproval.ModifiedBy = User.Identity.Name;
            db.Entry(FetchApproval).State = EntityState.Modified;
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Vehicle Reception Request Rejected .",
                Status = "Approved"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
