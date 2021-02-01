using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using System.Data.Entity;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.ResourceApprover, FleetManagementRoles.ResourceDistributor, FleetManagementRoles.ResourceRequestApprover)]
    public class AssignedEquipmentController : Controller
    {
        IAssignedEquipmentService _AssignedEquipmentService;
        IFleetService _FleetService;
        ExceedDbContext db = new ExceedDbContext();
        public AssignedEquipmentController()
        {
            _AssignedEquipmentService = new AssignedEquipmentService();
            _FleetService = new FleetService();
        }
        [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceApprover, FleetManagementRoles.ResourceDistributor, FleetManagementRoles.ResourceRequestApprover)]
        public JsonResult AssignedEquipment_Read([DataSourceRequest] DataSourceRequest request)
        {
            var AssignedEquipment = _AssignedEquipmentService.GetAll().Select(a => new AssignedEquipment
            {
                RequestNumber = a.RequestNumber,
                EquipmentName = a.EquipmentName,
                EquipmentID = a.EquipmentID,
                Quantity = a.Quantity,
                AssignedEquipmentID = a.AssignedEquipmentID,
                EquipmentStatus = a.EquipmentStatus,

                AssignedDate = a.AssignedDate
            });

            return Json(AssignedEquipment.ToDataSourceResult(request));
        }
        [AuthorizeRoles(FleetManagementRoles.ResourceDistributor, FleetManagementRoles.ResourceRequestApprover)]

        public JsonResult RequestEquipment_Create(string createRequestNumber, string createType, [DataSourceRequest] DataSourceRequest request, RequestedEquipment RequestedEquipment)
        {
            ModelState.Remove("RequestedEquipmentId");
            ModelState.Remove("RequestNumber");
            ModelState.Remove("EquipmentID");
            IEquipmentIdentityService IdentityService = new EquipmentIdentityService();
            var Identity = IdentityService.GetAll().Where(x => x.EquipmentIdentityID.ToString() == RequestedEquipment.Type).FirstOrDefault().EquipmentCategoryName;
            if (RequestedEquipment.StartDate > RequestedEquipment.EndDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("IncorrectDate", "  Start Date Is Above End Date !!! ");
            }
            else
            {
                RequestedEquipment.RequestedEquipmentId = Guid.NewGuid();
                RequestedEquipment.EquipmentID = Guid.Empty;
                RequestedEquipment.StartDate = RequestedEquipment.StartDate;
                RequestedEquipment.EndDate = RequestedEquipment.EndDate;
                RequestedEquipment.RequestNumber = Guid.Parse(createRequestNumber);
                RequestedEquipment.Remark = MaintenanceConst.R;
                RequestedEquipment.Type = Identity;
                RequestedEquipment.EquipmentName = RequestedEquipment.EquipmentName;
                RequestedEquipment.SubEquipmentName = RequestedEquipment.SubEquipmentName;
                RequestedEquipment.EquipmentStatus = RequestedEquipment.EquipmentStatus;
                RequestedEquipment.Quantity = RequestedEquipment.Quantity;
                RequestedEquipment.ManufacturedYear = RequestedEquipment.ManufacturedYear;
                RequestedEquipment.IsAssigned = false;
                RequestedEquipment.DateCreated = DateTime.Now;
                RequestedEquipment.CreatedBy = User.Identity.Name;

                if (RequestedEquipment != null)
                {
                    db.RequestedEquipments.Add(RequestedEquipment);
                    db.SaveChanges();
                }
            }

            return Json(new[] { RequestedEquipment }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult AssignedEquipment_Create(string createRequestNumber, string createType, string Id, [DataSourceRequest] DataSourceRequest request, AssignedEquipment AssignedEquipment)
        {
            ModelState.Remove("AssignedEquipmentID");
            ModelState.Remove("RequestNumber");
            ModelState.Remove("EquipmentID");
            ModelState.Remove("StartDate");
            ModelState.Remove("EndDate");
            ModelState.Remove("Type");

            _AssignedEquipmentService = new AssignedEquipmentService();
            IFleetService _fleetService = new FleetService();
            IOperatorService _operatorService = new OperatorService();
            IRentRequestService _rentRequestService = new RentRequestService();
            IVehicleDemandService _vehicleDemandService = new VehicleDemandService();
            IVehicleDistributionService _vehicleDistributionService = new VehicleDistributionService();

            DateTime ExpiredDate = DateTime.Now;
            DateTime StartDate = DateTime.Now;
            string operatorName = "";


            Fleets fleet = _fleetService.GetAll().Where(x => x.PlateNumber == AssignedEquipment.EquipmentName).FirstOrDefault();
            Operator Equipmentoperator = _operatorService.GetAll().Where(x => x.FleetsID == fleet.FleetsID).FirstOrDefault();
            VehicleDistribution Distribution = _vehicleDistributionService.GetAll().Where(x => x.VehicleDistributionID.ToString() == Id).FirstOrDefault();


            fleet.Location = Distribution.FromBranchName;
            _fleetService.Update(fleet);


            operatorName = fleet.OperatorName;

            List<AssignedEquipment> AssignedEquipments = _AssignedEquipmentService.GetAll().Where(f => f.EquipmentID == fleet.FleetsID && f.RequestNumber.ToString() == createRequestNumber).ToList();

            if (AssignedEquipments.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", "  Equipment is Already Requested !!! ");
            }


            else
            {
                var RequestDetail = db.RequestedEquipments.Where(
                       RequestedEquipment => RequestedEquipment.RequestedEquipmentId.ToString() == AssignedEquipment.CategoryRef).FirstOrDefault();

                var AssignedCount = db.AssignedEquipment.Where(x => x.RequestNumber.ToString() == AssignedEquipment.CategoryRef).ToList();
                if (AssignedCount.Count > RequestDetail.Quantity)
                {
                    ModelState.AddModelError("AssignedEnough", " Already Assigned Requested Amount !");

                }
                else
                {
                    AssignedEquipment.AssignedEquipmentID = Guid.NewGuid();
                    AssignedEquipment.EquipmentID = Guid.Empty;
                    AssignedEquipment.EquipmentID = fleet.FleetsID;
                    AssignedEquipment.EquipmentName = fleet.PlateNumber + " -- " + fleet.EquipmentName + " -- " + operatorName;// fleet.PlateNumber + " -- " + operatorName;
                    AssignedEquipment.RequestedDate = DateTime.Now;
                    AssignedEquipment.RequestNumber = Guid.Parse(createRequestNumber);
                    AssignedEquipment.Type = createType;
                    AssignedEquipment.Remark = AssignedEquipment.EquipmentName;
                    AssignedEquipment.StartDate = StartDate;
                    AssignedEquipment.EndDate = ExpiredDate;
                    AssignedEquipment.Type = MaintenanceConst.R;
                    AssignedEquipment.CategoryRef = RequestDetail.Type + " - " + RequestDetail.EquipmentName;
                    AssignedEquipment.EquipmentName = AssignedEquipment.EquipmentName;
                  //  AssignedEquipment.SubEquipmentName = RequestDetail.SubEquipmentName;
                    AssignedEquipment.Quantity = AssignedEquipment.Quantity;
                    AssignedEquipment.IsAssigned = true;

                    AssignedEquipment.DateCreated = DateTime.Now;
                    AssignedEquipment.CreatedBy = User.Identity.Name;

                    if (AssignedEquipment != null)
                    {
                        _AssignedEquipmentService.Insert(AssignedEquipment);
                        fleet.Status = MaintenanceConst.ONDUTY;
                        _fleetService.Update(fleet);

                    }
                }
            }

            return Json(new[] { AssignedEquipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AssignedEquipment_Update([DataSourceRequest] DataSourceRequest request, RequestedEquipment RequestedEquipment)
        {

            if (RequestedEquipment != null)
            {
                IEquipmentIdentityService IdentityService = new EquipmentIdentityService();
           
                var Identity = IdentityService.GetAll().Where(x => x.EquipmentIdentityID.ToString() == RequestedEquipment.Type).FirstOrDefault().EquipmentCategoryName;

                RequestedEquipment.Type = Identity;
                RequestedEquipment.EquipmentName = RequestedEquipment.EquipmentName;
                RequestedEquipment.SubEquipmentName = RequestedEquipment.SubEquipmentName;
                RequestedEquipment.Quantity = RequestedEquipment.Quantity;
                RequestedEquipment.LastModified = DateTime.Now;
                RequestedEquipment.ModifiedBy = User.Identity.Name;

                db.RequestedEquipments.Attach(RequestedEquipment);
                db.Entry(RequestedEquipment).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { RequestedEquipment }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AssignedEquipment_Destroy([DataSourceRequest] DataSourceRequest request, RequestedEquipment RequestedEquipments)
        {
            IFleetService _fleetService = new FleetService();

            var RequestedEquipment = db.RequestedEquipments.Where(x => x.RequestedEquipmentId == RequestedEquipments.RequestedEquipmentId).FirstOrDefault();


            if (RequestedEquipment != null)
            {

                db.RequestedEquipments.Attach(RequestedEquipment);
                db.RequestedEquipments.Remove(RequestedEquipment);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Requested Equipment number " + RequestedEquipments.RequestedEquipmentId);

            }

            return Json(new[] { RequestedEquipment }.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Assignment_Destroy([DataSourceRequest] DataSourceRequest request, AssignedEquipment AssignedEquipment)
        {
            IFleetService _fleetService = new FleetService();
            IAssignedEquipmentService _AssignedEquipmentService = new AssignedEquipmentService();
            var Assigned = _AssignedEquipmentService.GetById(AssignedEquipment.AssignedEquipmentID);


            Fleets fleet = _fleetService.GetAll().Where(x => x.FleetsID == Assigned.EquipmentID).FirstOrDefault();

            fleet.Status = MaintenanceConst.ACTIVE;
            _fleetService.Update(fleet);

            if (Assigned != null)
            {
                _AssignedEquipmentService.Delete(Assigned);

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Requested Equipment number " + Assigned.AssignedEquipmentID);

            }

            return Json(new[] { AssignedEquipment }.ToDataSourceResult(request));
        }




        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AcceptEquipment([DataSourceRequest] DataSourceRequest request, AssignedEquipment assignedEquipment)
        {
            _FleetService = new FleetService();
            var fleet = _FleetService.GetById(assignedEquipment.EquipmentID);
            var AssignedEquipment = _AssignedEquipmentService.GetById(assignedEquipment.AssignedEquipmentID);

            if (AssignedEquipment != null)
            {
                AssignedEquipment.IsAssigned = false;
                AssignedEquipment.OnlineTransferredTime = DateTime.Now;
                AssignedEquipment.ModifiedBy = User.Identity.Name;
                AssignedEquipment.IsOnlineTransferred = true;
                _AssignedEquipmentService.Update(AssignedEquipment);

                fleet.Status = MaintenanceConst.ACTIVE;
                fleet.Location = MaintenanceConst.UnAssigned;
                _FleetService.Update(fleet);
            }

            return Json(new[] { AssignedEquipment }.ToDataSourceResult(request));
        }


        public ActionResult AssignmentDDBind(string readRequestNumber)
        {

            _AssignedEquipmentService = new AssignedEquipmentService();

            return Json(db.RequestedEquipments.Where(RequestedEquipment => RequestedEquipment.RequestNumber.ToString() == readRequestNumber && RequestedEquipment.Remark == MaintenanceConst.R).Select(a => new { RequestedEquipmentId = a.RequestedEquipmentId, RequestNumber = a.RequestNumber, SubEquipmentName = a.SubEquipmentName, EquipmentID = a.EquipmentID, ManufacturedYear = a.ManufacturedYear, AssignedDate = a.AssignedDate, Type = a.Type, EquipmentName = a.EquipmentName + " -- " + a.Quantity, Quantity = a.Quantity, StartDate = a.StartDate, EndDate = a.EndDate, CategoryRef = a.CategoryRef }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult RequestedEquipment(string readRequestNumber, string readType, [DataSourceRequest] DataSourceRequest request)
        {


            return Json(db.RequestedEquipments.Where(RequestedEquipment => RequestedEquipment.RequestNumber.ToString() == readRequestNumber && RequestedEquipment.IsAssigned == false).Select(a => new { RequestedEquipmentId = a.RequestedEquipmentId, RequestNumber = a.RequestNumber, SubEquipmentName = a.SubEquipmentName, EquipmentID = a.EquipmentID, ManufacturedYear = a.ManufacturedYear, AssignedDate = a.AssignedDate, Type = a.Type, EquipmentName = a.EquipmentName, RequestDesc = a.Type + " -- " + a.EquipmentName + " -- " + a.Quantity + " -- Year " + a.ManufacturedYear, Quantity = a.Quantity, StartDate = a.StartDate, EndDate = a.EndDate, EquipmentStatus = a.EquipmentStatus, CategoryRef = a.CategoryRef }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult RequestedViewEquipment(string readRequestNumber, string readType, [DataSourceRequest] DataSourceRequest request)
        {

            var Request = db.RequestedEquipments.Where(RequestedEquipment => RequestedEquipment.RequestNumber.ToString() == readRequestNumber && RequestedEquipment.IsAssigned == false).Select(a => new { RequestedEquipmentId = a.RequestedEquipmentId, RequestNumber = a.RequestNumber, SubEquipmentName =a.SubEquipmentName, ManufacturedYear = a.ManufacturedYear, EquipmentID = a.EquipmentID, AssignedDate = a.AssignedDate, Type = a.Type, EquipmentName = a.EquipmentName, RequestDesc = a.Type + " -- " + a.EquipmentName + " -- " + a.Quantity + " -- Year " + a.ManufacturedYear, Quantity = a.Quantity, StartDate = a.StartDate, EndDate = a.EndDate, EquipmentStatus = a.EquipmentStatus, CategoryRef = a.CategoryRef }).ToList();

            return Json(Request.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AllEquipment(string readRequestNumber, string readType, [DataSourceRequest] DataSourceRequest request)
        {
            _AssignedEquipmentService = new AssignedEquipmentService();

            return Json(_AssignedEquipmentService.GetAll().Where(AssignedEquipment => AssignedEquipment.RequestNumber == Guid.Parse(readRequestNumber.ToString()) && AssignedEquipment.Remark != MaintenanceConst.R).Select(a => new AssignedEquipment { AssignedEquipmentID = a.AssignedEquipmentID, RequestNumber = a.RequestNumber, EquipmentID = a.EquipmentID, AssignedDate = a.AssignedDate, RequestDesc = a.Type + " -- " + a.EquipmentName + " -- " + a.Quantity, EquipmentName = a.EquipmentName, Type = a.Type, StartDate = a.StartDate, EndDate = a.EndDate, EquipmentStatus = a.EquipmentStatus, CategoryRef = a.CategoryRef, IsAssigned = a.IsAssigned }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ArrivedEquipment(string readRequestNumber, string readType, [DataSourceRequest] DataSourceRequest request)
        {
            _AssignedEquipmentService = new AssignedEquipmentService();

            return Json(_AssignedEquipmentService.GetAll().Where(AssignedEquipment => AssignedEquipment.RequestNumber == Guid.Parse(readRequestNumber.ToString()) && AssignedEquipment.Remark != MaintenanceConst.R).Select(a => new AssignedEquipment { AssignedEquipmentID = a.AssignedEquipmentID, RequestNumber = a.RequestNumber, EquipmentID = a.EquipmentID, AssignedDate = a.AssignedDate, RequestDesc = a.Type + " -- " + a.EquipmentName + " -- " + a.Quantity, EquipmentName = a.EquipmentName, Type = a.Type, StartDate = a.StartDate, EndDate = a.EndDate, EquipmentStatus = a.EquipmentStatus, CategoryRef = a.CategoryRef, IsAssigned = a.IsAssigned }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AssignedConfirmedEquipment(string rentRequest, string type, [DataSourceRequest] DataSourceRequest request)
        {
            _AssignedEquipmentService = new AssignedEquipmentService();

            return Json(_AssignedEquipmentService.GetAll().Where(AssignedEquipment => AssignedEquipment.RequestNumber == Guid.Parse(rentRequest.ToString()) && AssignedEquipment.IsAssigned == true).Select(a => new AssignedEquipment { AssignedEquipmentID = a.AssignedEquipmentID, RequestNumber = a.RequestNumber, EquipmentID = a.EquipmentID, AssignedDate = a.AssignedDate, Type = a.Type, EquipmentName = a.EquipmentName, Quantity = a.Quantity, StartDate = a.StartDate, EndDate = a.EndDate, EquipmentStatus = a.EquipmentStatus, CategoryRef = a.CategoryRef }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public JsonResult AssignEquipment([DataSourceRequest] DataSourceRequest request, AssignedEquipment AssignedEquipment)
        {

            if (AssignedEquipment != null)
            {
                if (AssignedEquipment.IsAssigned)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("ExistingName", "  Equipment is Already Assigned !!! ");
                }
                else
                {
                    IFleetService _fleetService = new FleetService();

                    Fleets fleet = _fleetService.GetAll().Where(x => x.PlateNumber == AssignedEquipment.EquipmentName).FirstOrDefault();
                    IOperatorService _operatorService = new OperatorService();
                    Operator Equipmentoperator =
                    _operatorService.GetAll().Where(x => x.FleetsID == fleet.FleetsID).FirstOrDefault();
                    string operatorName = "";
                    if (Equipmentoperator == null)
                    {
                        operatorName = " Operator Not Assigned ";

                    }
                    else
                    {
                        operatorName = Equipmentoperator.FullName;
                    }
                    AssignedEquipment.EquipmentName = fleet.PlateNumber + " -- " + operatorName;
                    AssignedEquipment.LastModified = DateTime.Now;
                    AssignedEquipment.ModifiedBy = User.Identity.Name;
                    AssignedEquipment.OnlineApprovedBy = User.Identity.Name;
                    AssignedEquipment.OnlineApprovedTime = DateTime.Now;
                    AssignedEquipment.IsAssigned = true;

                    _AssignedEquipmentService.Update(AssignedEquipment);
                }
            }

            return Json(new[] { AssignedEquipment }.ToDataSourceResult(request, ModelState));
        }

        public bool CanAssigned(Guid RequestID, string Type)
        {
            if (Type == "RENT")
            {
                IRentRequestService _rentRequestService = new RentRequestService();
                RentRequest vehicleDistribution =
                    _rentRequestService.GetAll().Where(x => x.RentRequestID == RequestID).FirstOrDefault();
                var fleets =
                    _AssignedEquipmentService.GetAll().Where(e => e.RequestNumber == RequestID).ToList();

                if (fleets.Count > vehicleDistribution.RequestQuantity ||
                    fleets.Count == vehicleDistribution.RequestQuantity)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (Type == "NORMAL")
            {
                IVehicleDemandService vehicleDemandService = new VehicleDemandService();
                VehicleDemand vehicleDemand =
                    vehicleDemandService.GetAll().Where(x => x.VehicleDemandId == RequestID).FirstOrDefault();
                var fleets =
                    _AssignedEquipmentService.GetAll().Where(e => e.RequestNumber == RequestID).ToList();

                if (fleets.Count > vehicleDemand.RequestQuantity ||
                    fleets.Count == vehicleDemand.RequestQuantity)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }


    }
}