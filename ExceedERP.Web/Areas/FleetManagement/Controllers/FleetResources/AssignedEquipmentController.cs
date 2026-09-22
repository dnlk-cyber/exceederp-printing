using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
   // [AuthorizeRoles(FleetManagementRoles.ResourceApprover, FleetManagementRoles.ResourceDistributor, FleetManagementRoles.ResourceRequestApprover)]
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
       // [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceApprover, FleetManagementRoles.ResourceDistributor, FleetManagementRoles.ResourceRequestApprover)]
        public JsonResult AssignedEquipment_Read([DataSourceRequest] DataSourceRequest request)
        {
            var AssignedEquipment = db.AssignedEquipment.Select(a => new AssignedEquipment
            {
                RequestNumber = a.RequestNumber,
                EquipmentNameID = a.EquipmentNameID,
                FleetId = a.FleetId,
                Quantity = a.Quantity,
                AssignedEquipmentID = a.AssignedEquipmentID,
                EquipmentStatus = a.EquipmentStatus,

                AssignedDate = a.AssignedDate
            });

            return Json(AssignedEquipment.ToDataSourceResult(request));
        }
       // [AuthorizeRoles(FleetManagementRoles.ResourceDistributor, FleetManagementRoles.ResourceRequestApprover)]

        public JsonResult RequestEquipment_Create(string createRequestNumber, int num, string createType, [DataSourceRequest] DataSourceRequest request, RequestedEquipment RequestedEquipment)
        {
            ModelState.Remove("RequestedEquipmentId");
            ModelState.Remove("RequestNumber");
            ModelState.Remove("EquipmentID");
            IEquipmentIdentityService IdentityService = new EquipmentIdentityService();
            if (RequestedEquipment.StartDate > RequestedEquipment.EndDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("IncorrectDate", "  Start Date Is Above End Date !!! ");
            }
           
            else
            {
                RequestedEquipment.ForwardedFrom = User.Identity.Name;
                RequestedEquipment.ForwardedTo = User.Identity.Name;
                RequestedEquipment.IsFinalApproval = RequestedEquipment.IsFinalApproval;
                RequestedEquipment.CurrentApprovalStep = RequestedEquipment.CurrentApprovalStep;
                RequestedEquipment.IsDepartmentOnlineApproved = false;
                RequestedEquipment.DepartmentOnlineApprovedBy = RequestedEquipment.DepartmentOnlineApprovedBy;
                RequestedEquipment.DepartmentOnlineApprovedTime = RequestedEquipment.DepartmentOnlineApprovedTime;
                RequestedEquipment.IsDepartmentSendForApproval = RequestedEquipment.IsDepartmentSendForApproval;
                RequestedEquipment.DepartmentSendForApprovalBy = RequestedEquipment.DepartmentSendForApprovalBy;
                RequestedEquipment.DepartmentSendForApprovalTime = RequestedEquipment.DepartmentSendForApprovalTime; 
                RequestedEquipment.RequestedEquipmentId = Guid.NewGuid();
                RequestedEquipment.RequestNumber = Guid.Parse(createRequestNumber);
                RequestedEquipment.Remark = MaintenanceConst.R;
                RequestedEquipment.IsAssigned = false;
                decimal totalkm = (RequestedEquipment.AsphaltKm ?? 0)
                                + (RequestedEquipment.GravelKm ?? 0);
                RequestedEquipment.NumberHourPerKm = totalkm.ToString();
                RequestedEquipment.DateCreated = DateTime.Now;
                RequestedEquipment.CreatedBy = User.Identity.Name;
                RequestedEquipment.VehicleDemandId = num;



                if (RequestedEquipment != null)
                {
                    db.RequestedEquipments.Add(RequestedEquipment);
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Assigned equipment registration", "Success", User.Identity.Name, $"created assigned equipment {RequestedEquipment.RequestedEquipmentId}", Modules.FleetManagement);
                    #endregion
                }
            }

            return Json(new[] { RequestedEquipment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult AssignedEquipment_Create([DataSourceRequest] DataSourceRequest request, AssignedEquipment assignedEquipment, string Id,string createRequestNumber, int vDemandId)
        {
            ModelState.Remove("AssignedEquipmentID");
            ModelState.Remove("EquipmentNameId");
            ModelState.Remove("RequestNumber");
            ModelState.Remove("RequestedEquipmentId");
            ModelState.Remove("StartDate");
            ModelState.Remove("EndDate");
            if (ModelState.IsValid)
            {
                string operatorName = "";
                var fleet = db.Fleets.ToList().Where(x => x.FleetsID == assignedEquipment.FleetId).FirstOrDefault();
                int operatorId = db.Operator.FirstOrDefault(x => x.OperatorID == assignedEquipment.OperatorId).EmployeeId;
                if (operatorId != null)
                {
                    var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == operatorId);
                    operatorName = emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " + emp.LastNameEnglish;
                }
                var fleetId = assignedEquipment.FleetId.ToString();
                var AssignedEquipments = db.AssignedEquipment.ToList().Where(f => f.FleetId == assignedEquipment.FleetId && f.IsAssigned);

                if (AssignedEquipments.Any())
                {
                    ModelState.Clear();
                    ModelState.AddModelError("ExistingName", "  Equipment is Already Requested !!! ");
                }
                var FetchRequest = db.RequestedEquipments.FirstOrDefault(x => x.RequestedEquipmentId.ToString() == Id);

                var parent = db.VehicleDistribution.FirstOrDefault(x => x.RequestID == FetchRequest.RequestNumber);
                var additionalDistributions = db.VehicleDistribution.Where(x => x.ParentId == parent.VehicleDistributionID.ToString()).ToList();
                foreach(var additionalDis in additionalDistributions)
                {
                    var additionalRequest = db.RequestedEquipments.FirstOrDefault(x => x.RequestNumber == additionalDis.RequestID);
                    if (additionalRequest != null)
                    {
                        var assignedEquip = new AssignedEquipment();
                        assignedEquip.StartDate = additionalRequest.StartDate;
                        assignedEquip.EndDate = additionalRequest.EndDate;
                        assignedEquip.ArrivalPlace = additionalRequest.ArrivalPlace;
                        assignedEquip.OperatorFullName = operatorName;
                        assignedEquip.AssignedEquipmentID = Guid.NewGuid();
                        assignedEquip.RequestedEquipmentId = Guid.Parse(Id);
                        assignedEquip.EquipmentNameID = fleet.EquipmentNameID;
                        assignedEquip.ReturnDate = DateTime.Now;
                        assignedEquip.FleetId = fleet.FleetsID;
                        assignedEquip.RequestedDate = DateTime.Now;
                        assignedEquip.OperatorId = assignedEquipment.OperatorId;
                        assignedEquip.RequestNumber = additionalRequest.RequestedEquipmentId;
                        assignedEquip.NetOdometer = 0;
                        decimal totalkm = assignedEquipment.AsphaltKm + assignedEquipment.GravelKm;
                        assignedEquip.NumberHourPerKm = totalkm.ToString();
                        //  AssignedEquipment.SubEquipmentName = RequestDetail.SubEquipmentName;
                        //assignedEquipment.Quantity = AssignedEquipment.Quantity;
                        //assignedEquipment.IsAssigned = true;
                        assignedEquip.DateCreated = DateTime.Now;
                        assignedEquip.CreatedBy = User.Identity.Name;
                        assignedEquip.VehicleDemandId = additionalRequest.VehicleDemandId;
                        if (assignedEquipment != null)
                        {

                            db.AssignedEquipment.Add(assignedEquip);
                            db.SaveChanges();
                        }
                    }

                    }
                assignedEquipment.StartDate = FetchRequest.StartDate;
                assignedEquipment.EndDate = FetchRequest.EndDate;
                assignedEquipment.ArrivalPlace = FetchRequest.ArrivalPlace;
                
                assignedEquipment.OperatorFullName = operatorName;
                if (fleet!=null)
                {
                    var equipmentoperator = db.Operator.ToList().Where(x => x.FleetsID == fleet.FleetsID).FirstOrDefault();
                    var distribution = db.VehicleDistribution.ToList().Where(x => x.VehicleDistributionID.ToString() == Id).FirstOrDefault();
                    fleet.Location = distribution?.FromBranchName;

                    operatorName = fleet.OperatorName;
                    

                   
                        assignedEquipment.AssignedEquipmentID = Guid.NewGuid();
                        assignedEquipment.RequestedEquipmentId = Guid.Parse(Id);
                        assignedEquipment.FleetId = fleet.FleetsID;
                        assignedEquipment.RequestedDate = DateTime.Now;
                        assignedEquipment.RequestNumber = Guid.Parse(createRequestNumber);
                        assignedEquipment.NetOdometer = 0;
                        assignedEquipment.ReturnDate = DateTime.Now;
                        decimal totalkm = assignedEquipment.AsphaltKm + assignedEquipment.GravelKm;
                        assignedEquipment.NumberHourPerKm = totalkm.ToString();
                        assignedEquipment.DateCreated = DateTime.Now;
                        assignedEquipment.CreatedBy = User.Identity.Name;
                        assignedEquipment.EquipmentNameID = fleet.EquipmentNameID;
                        assignedEquipment.VehicleDemandId = vDemandId;
                        if (assignedEquipment != null)
                        {
                            
                            db.AssignedEquipment.Add(assignedEquipment);
                            if(fleet.AssetId!=null)
                            {
                                var AssetRegistration = db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId == fleet.AssetId);
                                if (AssetRegistration != null && fleet != null)
                                {
                                    AssetRegistration.Location = fleet.Location;
                                    db.Entry(AssetRegistration).State = EntityState.Modified;
                                }
                            }
                            
                            fleet.Status = MaintenanceConst.ONDUTY;
                            //db.Fleets.Attach(fleet);
                            db.Entry(fleet).State = EntityState.Modified;
                            db.SaveChanges();
                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "Assigned equipment registration", "Success", User.Identity.Name, $"updated assigned equipment {fleet.FleetsID}", Modules.FleetManagement);
                            #endregion


                        }
                    
               
                }
           }

            return Json(new[] {assignedEquipment}.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AssignedEquipment_Update([DataSourceRequest] DataSourceRequest request, RequestedEquipment RequestedEquipment)
        {

            if (RequestedEquipment != null)
            {
                IEquipmentIdentityService IdentityService = new EquipmentIdentityService();


                RequestedEquipment.ForwardedFrom = User.Identity.Name;
                RequestedEquipment.ForwardedTo = User.Identity.Name;
                RequestedEquipment.IsFinalApproval = RequestedEquipment.IsFinalApproval;
                RequestedEquipment.CurrentApprovalStep = RequestedEquipment.CurrentApprovalStep;
                RequestedEquipment.IsDepartmentOnlineApproved = RequestedEquipment.IsDepartmentOnlineApproved;
                RequestedEquipment.DepartmentOnlineApprovedBy = RequestedEquipment.DepartmentOnlineApprovedBy;
                RequestedEquipment.DepartmentOnlineApprovedTime = RequestedEquipment.DepartmentOnlineApprovedTime;
                RequestedEquipment.IsDepartmentSendForApproval = RequestedEquipment.IsDepartmentSendForApproval;
                RequestedEquipment.DepartmentSendForApprovalBy = RequestedEquipment.DepartmentSendForApprovalBy;
                RequestedEquipment.DepartmentSendForApprovalTime = RequestedEquipment.DepartmentSendForApprovalTime;
                RequestedEquipment.IsOnlineApproved = RequestedEquipment.IsOnlineApproved;
                RequestedEquipment.OnlineApprovedBy = RequestedEquipment.OnlineApprovedBy;
                RequestedEquipment.OnlineApprovedTime = RequestedEquipment.OnlineApprovedTime;
                RequestedEquipment.IsVoid = RequestedEquipment.IsVoid;
                RequestedEquipment.VoidBy = RequestedEquipment.VoidBy;
                RequestedEquipment.VoidTime = RequestedEquipment.VoidTime;
                RequestedEquipment.Quantity = RequestedEquipment.Quantity;
                RequestedEquipment.LastModified = DateTime.Now;
                RequestedEquipment.ModifiedBy = User.Identity.Name;
                RequestedEquipment.ModifiedBy = User.Identity.Name;
                decimal totalkm = (RequestedEquipment.AsphaltKm ?? 0)
                                + (RequestedEquipment.GravelKm ?? 0);
                RequestedEquipment.NumberHourPerKm = totalkm.ToString();
                db.RequestedEquipments.Attach(RequestedEquipment);
                db.Entry(RequestedEquipment).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Equipment assignment", "Success", User.Identity.Name, $"updated assigned equipment {RequestedEquipment.RequestedEquipmentId}", Modules.FleetManagement);
                #endregion
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
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted assigned equipment " + RequestedEquipments.RequestedEquipmentId);

            }

            return Json(new[] { RequestedEquipment }.ToDataSourceResult(request));
        }
        public JsonResult AssignedEquipments_Destroy([DataSourceRequest] DataSourceRequest request, AssignedEquipment assignedEquipments)
        {
            IFleetService _fleetService = new FleetService();

            var assEquipment = db.AssignedEquipment.Where(x => x.AssignedEquipmentID == assignedEquipments.AssignedEquipmentID).FirstOrDefault();


            if (assEquipment != null)
            {

                //db.RequestedEquipments.Attach(RequestedEquipment);
                db.AssignedEquipment.Remove(assEquipment);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted assigned equipment " + assignedEquipments.AssignedEquipmentID);

            }

            return Json(new[] { assignedEquipments }.ToDataSourceResult(request,ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Assignment_Destroy([DataSourceRequest] DataSourceRequest request, AssignedEquipment AssignedEquipment)
        {
            IFleetService _fleetService = new FleetService();
            IAssignedEquipmentService _AssignedEquipmentService = new AssignedEquipmentService();
            var Assigned = _AssignedEquipmentService.GetById(AssignedEquipment.AssignedEquipmentID);


            Fleets fleet = _fleetService.GetAll().Where(x => x.FleetsID == Assigned.FleetId).FirstOrDefault();

            fleet.Status = MaintenanceConst.ACTIVE;
            _fleetService.Update(fleet);

            if (Assigned != null)
            {
                _AssignedEquipmentService.Delete(Assigned);

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  assigned Equipment number " + Assigned.AssignedEquipmentID);

            }

            return Json(new[] { AssignedEquipment }.ToDataSourceResult(request));
        }




        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AcceptEquipment([DataSourceRequest] DataSourceRequest request, AssignedEquipment assignedEquipment)
        {
            //_FleetService = new FleetService();
            var fleet = db.Fleets.FirstOrDefault(x=>x.FleetsID==assignedEquipment.FleetId);
            var AssignedEquipment = _AssignedEquipmentService.GetById(assignedEquipment.AssignedEquipmentID);
            if (AssignedEquipment != null)
            {
                AssignedEquipment.IsAssigned = false;
                AssignedEquipment.OnlineTransferredTime = DateTime.Now;
                AssignedEquipment.ModifiedBy = User.Identity.Name;
                AssignedEquipment.IsOnlineTransferred = true;
                AssignedEquipment.Returned = true;
                db.Entry(AssignedEquipment).State = EntityState.Modified;
                db.SaveChanges();
                if (fleet != null)
                {
                    fleet.Status = MaintenanceConst.ACTIVE;
                    fleet.Location = MaintenanceConst.UnAssigned;
                    db.Entry(fleet).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(new[] { AssignedEquipment }.ToDataSourceResult(request));
        }
        //public ActionResult AssignmentDDBind(string readRequestNumber)
        //{
        //    _AssignedEquipmentService = new AssignedEquipmentService();

        //    return Json(db.RequestedEquipments.Where(RequestedEquipment => RequestedEquipment.RequestNumber.ToString() == readRequestNumber && RequestedEquipment.Remark == MaintenanceConst.R).Select(a => new { RequestedEquipmentId = a.RequestedEquipmentId, RequestNumber = a.RequestNumber, SubEquipmentName = a.SubEquipmentName, EquipmentID = a.EquipmentID, ManufacturedYear = a.ManufacturedYear, AssignedDate = a.AssignedDate, Type = a.Type, EquipmentName = a.EquipmentName + " -- " + a.Quantity, Quantity = a.Quantity, StartDate = a.StartDate, EndDate = a.EndDate, CategoryRef = a.CategoryRef }).ToList(), JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult RequestedEquipment(string readRequestNumber, string readType, [DataSourceRequest] DataSourceRequest request)
        //{


        //    return Json(db.RequestedEquipments.Where(RequestedEquipment => RequestedEquipment.RequestNumber.ToString() == readRequestNumber && RequestedEquipment.IsAssigned == false).Select(a => new { RequestedEquipmentId = a.RequestedEquipmentId, RequestNumber = a.RequestNumber, SubEquipmentName = a.SubEquipmentName, EquipmentID = a.EquipmentID, ManufacturedYear = a.ManufacturedYear, AssignedDate = a.AssignedDate, Type = a.Type, EquipmentName = a.EquipmentName, RequestDesc = a.Type + " -- " + a.EquipmentName + " -- " + a.Quantity + " -- Year " + a.ManufacturedYear, Quantity = a.Quantity, StartDate = a.StartDate, EndDate = a.EndDate, EquipmentStatus = a.EquipmentStatus, CategoryRef = a.CategoryRef }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult RequestedViewEquipment(string readRequestNumber, string readType, [DataSourceRequest] DataSourceRequest request)
        {
            var query =
                from req in db.RequestedEquipments
                join en in db.EquipmentName
                    on req.EquipmentNameId equals en.EquipmentNameID into enJoin
                from en in enJoin.DefaultIfEmpty()
                join et in db.EquipmentType
                    on req.EquipmentTypeID equals et.EquipmentTypeID into etJoin
                from et in etJoin.DefaultIfEmpty()
                where req.RequestNumber.ToString() == readRequestNumber// && req.IsAssigned == false
                select new
                {
                    RequestedEquipmentId = req.RequestedEquipmentId,
                    RequestNumber = req.RequestNumber,
                    NumberHourKM = req.NumberHourKM,
                    NumberofPersonston = req.NumberofPersonston,
                    Remarks = req.Remarks,
                    EquipmentIdentityID = req.EquipmentIdentityID,
                    EquipmentTypeID = req.EquipmentTypeID,
                    EquipmentNameId = req.EquipmentNameId,
                    ManufacturedYear = req.ManufacturedYear,
                    GravelKm = req.GravelKm,
                    AsphaltKm = req.AsphaltKm,
                    AssignedDate = req.AssignedDate,
                    RequestDesc = req.Quantity + " -- Year " + req.ManufacturedYear,
                    Quantity = req.Quantity,
                    StartDate = req.StartDate,
                    EndDate = req.EndDate,
                    NumberHourPerKm = req.NumberHourPerKm,
                    Destination = req.Destination,
                    NumberofPersonOrTon = req.NumberofPersonOrTon,
                    EquipmentStatus = req.EquipmentStatus,
                    VehicleDemandId = req.VehicleDemandId,
                    EquipmentName = en.Name,                // 🚀 from join
                    EquipmentTypeName = et.EquipmentTypeName // 🚀 from join
        };

            return Json(query.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        // public JsonResult RequestedViewEquipment(string readRequestNumber, string readType, [DataSourceRequest] DataSourceRequest request)
        //{
        //    var Request = db.RequestedEquipments
        //        .Where(x => x.RequestNumber.ToString() == readRequestNumber && x.IsAssigned == false)
        //        .Select(a => new
        //        {
        //            RequestedEquipmentId = a.RequestedEquipmentId,
        //            RequestNumber = a.RequestNumber,
        //            NumberHourKM = a.NumberHourKM,
        //            NumberofPersonston = a.NumberofPersonston,
        //            Remarks = a.Remarks,
        //            EquipmentIdentityID = a.EquipmentIdentityID,
        //            EquipmentTypeID = a.EquipmentTypeID,
        //            ManufacturedYear = a.ManufacturedYear,
        //            GravelKm = a.GravelKm,
        //            AsphaltKm = a.AsphaltKm,
        //            AssignedDate = a.AssignedDate,
        //            RequestDesc = a.Quantity + " -- Year " + a.ManufacturedYear,
        //            Quantity = a.Quantity,
        //            StartDate = a.StartDate,
        //            EndDate = a.EndDate,
        //            NumberHourPerKm = a.NumberHourPerKm,
        //            Destination = a.Destination,
        //            NumberofPersonOrTon = a.NumberofPersonOrTon,
        //            EquipmentStatus = a.EquipmentStatus,
        //            VehicleDemandId = a.VehicleDemandId,
        //            EquipmentNameId = a.EquipmentNameId
        //        }).ToList();
        //    return Json(Request.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult AllEquipment(int readRequestNumber, string readType, [DataSourceRequest] DataSourceRequest request)
        {
            _AssignedEquipmentService = new AssignedEquipmentService();

            return Json(_AssignedEquipmentService.GetAll().Where(AssignedEquipment => AssignedEquipment.RequestNumber == Guid.Parse(readRequestNumber.ToString()) && AssignedEquipment.Remark != MaintenanceConst.R).Select(a => new AssignedEquipment
            { AssignedEquipmentID = a.AssignedEquipmentID,
                RequestNumber = a.RequestNumber,
                AssignedDate = a.AssignedDate,
                FleetId = a.FleetId,
                EquipmentNameID = a.EquipmentNameID,
                EquipmentName = a.EquipmentName,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                EquipmentStatus = a.EquipmentStatus,
                IsAssigned = a.IsAssigned,
                IsArrived = a.IsArrived,
                StartOdometr = a.StartOdometr,
                EndOdometer = a.EndOdometer,
                NetOdometer = a.NetOdometer,
                OperatorId = a.OperatorId,
                ReturnDate = a.ReturnDate,
                VehicleDemandId = a.VehicleDemandId,
                Returned = a.Returned,
                CreatedBy = a.CreatedBy,
                DateCreated = a.DateCreated,
                Destination = a.Destination,
                IsOnlineApproved = a.IsOnlineApproved,
                IsOnlineTransferred = a.IsOnlineTransferred,
                IsOnlineTransferredBy = a.IsOnlineTransferredBy,
                IsSendForApproval = a.IsSendForApproval,
                IsVoid = a.IsVoid,
                

            }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Equipment_Read([DataSourceRequest] DataSourceRequest request, int vehicleDemandId)
        {
            var assignedEquipments = db.AssignedEquipment.Where(x => x.VehicleDemandId == vehicleDemandId);
           var result = assignedEquipments.ToDataSourceResult(request, assignedEquipment => new
            {
                AssignedEquipmentID = assignedEquipment.AssignedEquipmentID,
                RequestNumber = assignedEquipment.RequestNumber,
                AssignedDate = assignedEquipment.AssignedDate,
                FleetId = assignedEquipment.FleetId,
                EquipmentNameID = assignedEquipment.EquipmentNameID,
                //EquipmentName = GetEName(assignedEquipment.EquipmentNameID),
                EquipmentName = assignedEquipment.EquipmentName,
                StartDate = assignedEquipment.StartDate,
                EndDate = assignedEquipment.EndDate,
                EquipmentStatus = assignedEquipment.EquipmentStatus,
                IsAssigned = assignedEquipment.IsAssigned,
                IsArrived = assignedEquipment.IsArrived,
                StartOdometr = assignedEquipment.StartOdometr,
                EndOdometer = assignedEquipment.EndOdometer,
                NetOdometer = assignedEquipment.NetOdometer,
                OperatorId = assignedEquipment.OperatorId,
                OperatorFullName=GetOperatorFullName(assignedEquipment.OperatorId),
                ReturnDate = assignedEquipment.ReturnDate,
                VehicleDemandId = assignedEquipment.VehicleDemandId,
                Returned = assignedEquipment.Returned,
                CreatedBy = assignedEquipment.CreatedBy,
                DateCreated = assignedEquipment.DateCreated,
                Destination = assignedEquipment.Destination,
                IsOnlineApproved = assignedEquipment.IsOnlineApproved,
                IsOnlineTransferred = assignedEquipment.IsOnlineTransferred,
                IsOnlineTransferredBy = assignedEquipment.IsOnlineTransferredBy,
                IsSendForApproval = assignedEquipment.IsSendForApproval,
                IsVoid = assignedEquipment.IsVoid,
            });
            return Json(result);
        }
        private string GetOperatorFullName(Guid? OId)
        {
            var returnValue = "";
            var OperatorFullName = db.Operator.FirstOrDefault(x => x.OperatorID == OId);
               if(OperatorFullName!=null)
            {
                var fetchEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == OperatorFullName.EmployeeId);
                returnValue = OperatorFullName.OperatorID+" "+fetchEmployee.FirstNameEnglish + " " + fetchEmployee.MiddleNameEnglish+" "+fetchEmployee.LastNameEnglish;
            }
            return returnValue;
        }
        // Updated Read method for AssignedEquipment
        public ActionResult AssignedEquipmentSelf_Read([DataSourceRequest] DataSourceRequest request, int num, string vDemandId)
        {
            //var assignedEquipment = db.AssignedEquipment.Where(x => x.VehicleDemandId == num);
            var requestEquipment = db.RequestedEquipments.FirstOrDefault(x => x.RequestNumber.ToString() == vDemandId);
            var assignedEquipment = db.AssignedEquipment.Where(x => x.VehicleDemandId == requestEquipment.VehicleDemandId);
            DataSourceResult result = assignedEquipment.ToDataSourceResult(request, assignedEquipments => new
            {
                RequestNumber = assignedEquipments.RequestNumber,
                AssignedDate = assignedEquipments.AssignedDate,
                FleetId = assignedEquipments.FleetId,
                EquipmentNameID = assignedEquipments.EquipmentNameID,
                EquipmentName = assignedEquipments.EquipmentName,
                StartDate = assignedEquipments.StartDate,
                EndDate = assignedEquipments.EndDate,
                EquipmentStatus = assignedEquipments.EquipmentStatus,
                IsAssigned = assignedEquipments.IsAssigned,
                IsArrived = assignedEquipments.IsArrived,
                StartOdometr = assignedEquipments.StartOdometr,
                EndOdometer = assignedEquipments.EndOdometer,
                NetOdometer = assignedEquipments.NetOdometer,
                OperatorId = assignedEquipments.OperatorId
            });
            return Json(result);
        }
        public JsonResult ArrivedEquipment(string readRequestNumber, string readType, [DataSourceRequest] DataSourceRequest request)
        {
            _AssignedEquipmentService = new AssignedEquipmentService();
            var requestedEquips = db.RequestedEquipments.ToList().Where(q => q.RequestNumber == Guid.Parse(readRequestNumber.ToString())).Select(s => s.RequestedEquipmentId);
            var assignedEquipmentList = _AssignedEquipmentService.GetAll().ToList().Where(x => requestedEquips.Contains(x.RequestedEquipmentId));
            return Json(assignedEquipmentList.Where(AssignedEquipment => AssignedEquipment.Remark != MaintenanceConst.R)
            .Select(a => new AssignedEquipment {
                AssignedEquipmentID = a.AssignedEquipmentID,
                RequestNumber = a.RequestNumber,
                RequestedEquipmentId = a.RequestedEquipmentId,
                EquipmentNameID = a.EquipmentNameID,
                FleetId = a.FleetId,
                AssignedDate = a.AssignedDate,
                RequestDesc = " -- " + a.EquipmentName + " -- " + a.Quantity,
                EquipmentName = a.EquipmentName,
                //Type = a.Type,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                EquipmentStatus = a.EquipmentStatus,
                StartOdometr = a.StartOdometr,
                EndOdometer = a.EndOdometer,
                NetOdometer = a.NetOdometer,
                //CategoryRef = a.CategoryRef,
                IsAssigned = a.IsAssigned,
                CreatedBy = a.CreatedBy,
                DateCreated = a.DateCreated,
                Destination = a.Destination,
                IsArrived = a.IsArrived,
                IsOnlineApproved = a.IsOnlineApproved,
                IsOnlineTransferred = a.IsOnlineTransferred,
                IsOnlineTransferredBy = a.IsOnlineTransferredBy,
                IsSendForApproval = a.IsSendForApproval,
                IsVoid = a.IsVoid,
                LastModified = a.LastModified,
                ModifiedBy = a.ModifiedBy,
                NumberHourPerKm = a.NumberHourPerKm,
                NumberofPersonOrTon = a.NumberofPersonOrTon,
                OnlineApprovedBy = a.OnlineApprovedBy,
                OnlineApprovedTime = a.OnlineApprovedTime,
                OnlineTransferredTime = a.OnlineTransferredTime,
                OperatorId = a.OperatorId,
                OrgBranchName = a.OrgBranchName,
                Quantity = a.Quantity,
                Remark = a.Remark,
                RequestedDate = a.RequestedDate,
                SendForApprovalBy = a.SendForApprovalBy,
                SendForApprovalTime = a.SendForApprovalTime,
                Status = a.Status,
                VoidBy = a.VoidBy,
                VoidTime = a.VoidTime,
                Returned = a.Returned,
                VehicleDemandId = a.VehicleDemandId,
                ReturnDate = a.ReturnDate
                

            }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult AssignedConfirmedEquipment(string rentRequest, string type, [DataSourceRequest] DataSourceRequest request)
        //{
        //    _AssignedEquipmentService = new AssignedEquipmentService();

        //    return Json(_AssignedEquipmentService.GetAll().Where(AssignedEquipment => AssignedEquipment.RequestNumber == Guid.Parse(rentRequest.ToString()) && AssignedEquipment.IsAssigned == true).Select(a => new AssignedEquipment { AssignedEquipmentID = a.AssignedEquipmentID, RequestNumber = a.RequestNumber, EquipmentID = a.EquipmentID, AssignedDate = a.AssignedDate, Type = a.Type, EquipmentName = a.EquipmentName, Quantity = a.Quantity, StartDate = a.StartDate, EndDate = a.EndDate, EquipmentStatus = a.EquipmentStatus, CategoryRef = a.CategoryRef }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}
        // Upadate new 
        public ActionResult AssignedEquipment_Update([DataSourceRequest] DataSourceRequest request, AssignedEquipment assignedEquipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssignedEquipment
                {
                    EndOdometer = assignedEquipment.EndOdometer,
                    NetOdometer = assignedEquipment.EndOdometer - assignedEquipment.StartOdometr,

                };
            
                db.AssignedEquipment.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                assignedEquipment.NetOdometer = entity.NetOdometer;
            }

            return Json(new[] { assignedEquipment }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult AssignEquipment([DataSourceRequest] DataSourceRequest request, AssignedEquipment AssignedEquipment)
        {

            if (AssignedEquipment != null)
            {
                //if (AssignedEquipment.IsAssigned)
                //{
                //    ModelState.Clear();
                //    ModelState.AddModelError("ExistingName", "  Equipment is Already Assigned !!! ");
                //}
                //else
                //{
                    IFleetService _fleetService = new FleetService();

                    Fleets fleet = _fleetService.GetAll().Where(x => x.FleetsID == AssignedEquipment.FleetId).FirstOrDefault();
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
                    AssignedEquipment.NetOdometer = AssignedEquipment.EndOdometer - AssignedEquipment.StartOdometr;
                AssignedEquipment.ReturnDate = DateTime.Now;

                    _AssignedEquipmentService.Update(AssignedEquipment);
                //}
            }

            return Json(new[] { AssignedEquipment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, AssignedEquipment assignedEquipment)
        {
            ModelState.Remove("AssignedEquipmentID");
            ModelState.Remove("RequestNumber");
            ModelState.Remove("RequestedEquipmentId");
            ModelState.Remove("StartDate");
            ModelState.Remove("EndDate");       
            ModelState.Remove("Quantity");
            ModelState.Remove("NumberofPersonOrTon");
            ModelState.Remove("EndOdometer");
            ModelState.Remove("StartOdometr");
            ModelState.Remove("AsphaltKm");
            ModelState.Remove("GravelKm");
            var AllassignedE = db.AssignedEquipment;
            foreach(var assignedE in AllassignedE)
            {
                if (assignedE != null)
                {
                    assignedE.NumberHourPerKm = assignedE.NumberHourPerKm;
                    db.Entry(assignedE).State = EntityState.Modified;
                }
            }
            db.SaveChanges();

            if (assignedEquipment != null && ModelState.IsValid)
            {
                var assignedE = db.AssignedEquipment.Find(assignedEquipment.AssignedEquipmentID);
                if(assignedE != null)
                {
                    assignedE.EquipmentNameID = assignedEquipment.EquipmentNameID;
                    assignedE.FleetId = assignedEquipment.FleetId;
                    assignedE.OperatorId = assignedEquipment.OperatorId;
                    assignedE.StartOdometr = assignedEquipment.StartOdometr;
                    assignedE.NetOdometer = 0;
                    db.Entry(assignedE).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(new[] { assignedEquipment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult AssignEqupment_Update([DataSourceRequest] DataSourceRequest request,AssignedEquipment assignedEquipment)
        {
            if(assignedEquipment != null && ModelState.IsValid)
            {
                var assignedEquip = db.AssignedEquipment.Find(assignedEquipment.AssignedEquipmentID);
                if(assignedEquip != null)
                {
                    assignedEquip.EndOdometer = assignedEquipment.EndOdometer;
                    assignedEquip.ReturnDate = DateTime.Now;
                    
                    assignedEquip.ModifiedBy = User.Identity.Name;
                    assignedEquip.LastModified = DateTime.Now;
                    assignedEquip.NetOdometer = assignedEquipment.EndOdometer - assignedEquipment.StartOdometr;

                   
                    db.Entry(assignedEquip).State = EntityState.Modified;
                    db.SaveChanges();

                    assignedEquipment.ReturnDate = assignedEquip.ReturnDate;
                    assignedEquipment.NetOdometer = assignedEquip.NetOdometer;

                }
            }
            return Json(new[] { assignedEquipment }.ToDataSourceResult(request, ModelState));
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
                    db.VehicleDemand.Where(x => x.VehicleDemandId == RequestID).FirstOrDefault();
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}