
using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    [AuthorizeRoles(MaintenanceRoles.MaintenanceRequestDepartmentApprover)]
    public class JobRequestDepartmentController : Controller
    {
        private readonly IFleetService _FleetService;
        private IAdministrativeApprovalService _AdministrativeApprovalService;
        private readonly ITechnicalJobApprovalService _TechnicalJobApprovalService;
        private readonly IEquipmentIdentityService _EquipmentIdentityService;
        public const string Finished = "Finished";
        public const string Pending = "Pending";
        public const string Active = "Active";
        private ExceedDbContext db = new ExceedDbContext();
        public JobRequestDepartmentController()
        {

            _TechnicalJobApprovalService = new TechnicalJobApprovalService();
            _AdministrativeApprovalService = new AdministrativeJobApprovalService();
            _EquipmentIdentityService = new EquipmentIdentityService();
            _FleetService = new FleetService();
        }

        // GET: /FleetManagement/JobRequest/
        public ActionResult Index()
        {
            PopulateViewData();
            //ViewData["EquipmentsList"] = db.EquipmentInformation.Select(e => new { e.EquipmentInformationId, e.LicensePlate, e.DeviceNumber }).ToList();
            ViewData["Customer"] = db.OrganizationCustomers.ToList();
            ViewData["MaintenanceCenter"] = db.MaintenanceCenter.Select(s => new
            {
                Id = s.MaintenanceCenterID,
                Name = s.Name
            }).ToList();
            return View();
        }
        private void PopulateViewData()
        {
            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;
            var branches = db.Branch;
            var structure = db.MainCompanyStructures;


            ViewData["Structure"] = structure;

            ViewData["Branches"] = branches;

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var businessUnits = db.BusinessUnits.ToList();
                ViewData["BusinessUnits"] = businessUnits;
            }

            var items = db.ApSetupItems.ToList();
            ViewData["Items"] = items;
        }
        public ActionResult JobRequest_Read([DataSourceRequest] DataSourceRequest request)
        {

            var JobRequest = db.JobRequest.Where(x => x.IsOnlineApproved).ToList();
            //   System.Runtime.Caching.MemoryCache.Default["AssetList"] = db.AssetRegistrations.ToList();
            var result = JobRequest.ToDataSourceResult(request);

            return Json(result);
        }

        public ActionResult Admin_Approve([DataSourceRequest] DataSourceRequest request, AdministrativeJobDepartmentApproval model, string Id)
        {
            try
            {

                string Odometer = string.Empty;
                string EquipmentType = string.Empty;
                string AssetNum = string.Empty;
                string PlateNum = string.Empty;
                Guid EquipmentTypeId = Guid.Empty;
                var Request = db.JobRequest.Where(x => x.JobRequestId.ToString() == Id).FirstOrDefault();
                if (Request != null)
                {
                    if (model.Status == ApprovalStatuses.Approve)
                    {
                        var FetchSetting = db.RequestToInspections.FirstOrDefault();
                        if (FetchSetting.GoToInspection == false)
                        {
                            var JobOrder = new JobOrder();
                            JobOrder.JobOrderID = Guid.NewGuid();
                            JobOrder.EquipmentInformationId = Request.EquipmentInformationId == null ? Guid.Empty : Request.EquipmentInformationId.Value;
                            JobOrder.CurrentFuel = Request.FuelAmount;
                            JobOrder.LicensePlate = Request.LicensePlate;
                            JobOrder.FleetsID = db.Fleets.FirstOrDefault(x => x.PlateNumber == Request.LicensePlate || x.SerialNo == Request.LicensePlate).FleetsID;
                            JobOrder.EquipmentName = Request.EquipmentName;
                            JobOrder.OperatorName = Request.OperatorName;
                            JobOrder.AssetNum = AssetNum;
                            JobOrder.Referance = Request.Num;
                            JobOrder.IsPM = Request.IsPM;
                            JobOrder.Odometer = Request.Odometer;
                            JobOrder.EquipmentType = EquipmentType;
                            JobOrder.MaintenanceCenter = Request.MaintenanceCenter; // added
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
                            JobOrder.SendForApprovalTime = DateTime.Now;
                            JobOrder.OnlineApprovedTime = DateTime.Now;
                            JobOrder.IsSentToWorkShop = false;
                            JobOrder.IsSubmittedToServiceCenter = false;
                            JobOrder.CreatedBy = User.Identity.Name;
                            JobOrder.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                            JobOrder.BranchId = Request.BranchId;
                            JobOrder.BusinessUnitId = Request.BusinessUnitId;
                            JobOrder.OrgStructureId = Request.OrgStructureId;
                            JobOrder.CreatedAtBranchId = Request.CreatedAtBranchId;
                            JobOrder.CreatedAtBusinessUnitId = Request.CreatedAtBusinessUnitId;
                            JobOrder.RequesterId = Request.RequesterId;
                            JobOrder.EquipmentName = Request.EquipmentName;
                            JobOrder.IsInsured = Request.HasInsurance;
                            db.JobOrder.Add(JobOrder);
                            db.SaveChanges();
                        }
                        else
                        {
                            var newReception = new VehicleReceptionForm();
                            ModelState.Remove("CustomerId");
                            ModelState.Remove("TechnicianAssignedId");
                            ModelState.Remove("Fuel");
                            newReception.DateCreated = DateTime.Now;
                            newReception.CreatedBy = User.Identity.Name;
                            newReception.VehicleReceptionFormId = newReception.VehicleReceptionFormId;
                            newReception.Source = Source.Internal;
                            newReception.VehicleModel = "";
                            newReception.VehicleType = "";
                            newReception.ReferenceNumber = Request.Num.ToString();
                            newReception.PlateNo = Request.LicensePlate;
                            var fleet = db.Fleets.FirstOrDefault(x => x.PlateNumber == Request.LicensePlate || x.SerialNo == Request.LicensePlate);
                            if (fleet != null)
                            {
                                var equipmentName = fleet.Model;
                                var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == fleet.EquipmentTypeID).EquipmentTypeName;
                                var equipmentCategory = db.EquipmentIdentity.FirstOrDefault(x => x.EquipmentIdentityID == fleet.EquipmentIdentityID).EquipmentIdentityID;
                             
                                newReception.VehicleCategoryId = equipmentCategory;
                                newReception.VehicleModel = equipmentName;
                                newReception.VehicleType = equipmentType;
                            }
                            db.VehicleReceptionForm.Add(newReception);
                            db.SaveChanges();
                            var FetchCauses = db.JobCauses.Where(x => x.OrderId == Request.JobRequestId).ToList();
                            if(FetchCauses.Any())
                            {
                                foreach(var cause in FetchCauses)
                                {
                                    cause.ShowOnReception = true;
                                    db.Entry(cause).State = EntityState.Modified;
                                    
                                }
                                db.SaveChanges();
                            }
                            
                        }
                        Request.IsSecondOnlineApproved = true;
                        Request.SecondOnlineApprovedBy = User.Identity.Name;
                        Request.SecondOnlineApprovedTime = DateTime.Now;
                        //Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Request with No " + Request.Num + " Approved ", Title = " Approved Job Request ", URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });
                        //List<UserVM> UserList = new List<UserVM>();
                        ////UserList = UserInformationHelper.GetUsersInRole(MaintenanceRoles.MaintenanceOrderUser);
                        //foreach (var user in UserList)
                        //{
                        //    NotifModels jobRequesterNotif = new NotifModels
                        //    {
                        //        UserId = user.UserId,
                        //        Title = "New Maintenace Request",
                        //        Message = "New Maintenance Request",
                        //        DeadLine = DateTime.Now,
                        //       // URL = "/JobOrderClosed/"

                        //    };
                        //    Notify.NotifyUser(jobRequesterNotif);
                        //}
                    }
                    if (model.Status == ApprovalStatuses.Reject)
                    {

                        Request.IsSendForApproval = false;

                    }
                    Request.Status = model.Status.ToString();
                    db.JobRequest.Attach(Request);
                    db.Entry(Request).State = EntityState.Modified;
                    db.SaveChanges();
                    var adminApproval = new AdministrativeJobDepartmentApproval();
                    adminApproval.AdministrativeJobId = Guid.Parse(Id);
                    adminApproval.Status = model.Status;
                    adminApproval.Remark = model.Remark;
                    adminApproval.CreatedBy = User.Identity.Name;
                    adminApproval.DateCreated = DateTime.Now;
                    db.AdministrativeJobDepartmentApproval.Add(adminApproval);
                    db.SaveChanges();
                }

            }

            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);

            }
            return Json("");
        }

        public ActionResult Admin_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            var Admin = new List<AdministrativeJobDepartmentApproval>();
            var Request = db.JobRequest.Where(x => x.JobRequestId.ToString() == Id).FirstOrDefault();
            if (Request != null)
            {

                Admin =
                    db.AdministrativeJobDepartmentApproval
                        .Where(x => x.AdministrativeJobId.ToString() == Id)
                        .ToList();
            }

            if (Request.IsOnlineApproved && !Request.IsSecondOnlineApproved)
            {



                var adminApproval = new AdministrativeJobDepartmentApproval();
                adminApproval.AdministrativeJobDepartmentApprovalID = 0;
                adminApproval.AdministrativeJobId = Guid.NewGuid();
                adminApproval.Status = ApprovalStatuses.Pending;
                adminApproval.Remark = string.Empty;
                adminApproval.CreatedBy = User.Identity.Name;
                adminApproval.DateCreated = DateTime.Now;

                Admin.Add(adminApproval);

            }
            return Json(Admin.ToDataSourceResult(request));
        }




        public ActionResult SendForOnlineApproval(string Id)
        {
            JobRequest JobRequest = db.JobRequest.Where(x => x.JobRequestId.ToString() == Id).FirstOrDefault();
            if (JobRequest != null)
            {
                JobRequest.Status = MaintenanceConst.WaitingAdminstrative;
                JobRequest.IsSendForApproval = true;
                JobRequest.SendForApprovalBy = User.Identity.Name;
                JobRequest.SendForApprovalTime = DateTime.Now;
                var JobCauses = db.JobCauses.Where(x => x.OrderId.ToString() == Id).ToList();
                if (JobCauses.Any())
                {
                    if (JobRequest != null)
                    {

                        db.JobRequest.Attach(JobRequest);
                        db.Entry(JobRequest).State = EntityState.Modified;
                        db.SaveChanges();


                    }
                }


                else
                {

                    return Json(" Add Cause For Your Request !", JsonRequestBehavior.AllowGet);
                }
            }
            return Json(" Job Request Sent For Approval successfully !", JsonRequestBehavior.AllowGet);

        }

        public ActionResult VoidJobRequest(string id)
        {
            var jobRequest = db.JobRequest.Where(q => q.JobRequestId.ToString() == id && q.Status == ApprovalStatuses.Approve.ToString()).FirstOrDefault();
            if (jobRequest != null)
            {
                var jobOrder = db.JobOrder.Where(q => q.Referance == jobRequest.Num && (q.Status == MaintenanceConst.Pending || q.Status == MaintenanceConst.Open || q.Status == MaintenanceConst.WaitingApproval))
                    .FirstOrDefault();

                db.JobRequest.Attach(jobRequest);
                db.JobOrder.Remove(jobOrder);
                db.SaveChanges();
            }
            return Json(jobRequest, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApproveJobRequest(Guid id)
        {
            var request = new DataSourceRequest();
            // var jobRequest = db.JobRequest.Find(id);

            //try
            //{

            string Odometer = string.Empty;
            string EquipmentType = string.Empty;
            string AssetNum = string.Empty;
            string PlateNum = string.Empty;
            Guid EquipmentTypeId = Guid.Empty;
            //  var model = db.AdministrativeJobApproval.Find(id);

            var Request = db.JobRequest.FirstOrDefault(x => x.JobRequestId == id);
            var maintenanceCenter = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID == Request.MaintenanceCenterId);
            var fleet = db.Fleets.FirstOrDefault(x => x.PlateNumber == Request.LicensePlate || x.SerialNo == Request.LicensePlate);

            if (Request != null/* && fleet != null*/)
            {

                //if (model.Status == ApprovalStatuses.Approve)
                //{
                var FetchSetting = db.RequestToInspections.FirstOrDefault();
                if (FetchSetting.GoToInspection == false)
                {
                    var JobOrder = new JobOrder();

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
                }
                else
                {
                    var newReception = new VehicleReceptionForm();
                    ModelState.Remove("CustomerId");
                    ModelState.Remove("TechnicianAssignedId");
                    ModelState.Remove("Fuel");
                    newReception.DateCreated = DateTime.Now;
                    newReception.CreatedBy = User.Identity.Name;
                    newReception.VehicleReceptionFormId = newReception.VehicleReceptionFormId;
                    newReception.Source = Source.Internal;
                    newReception.VehicleModel = "";
                    newReception.VehicleType = "";
                    newReception.ReferenceNumber = Request.Num.ToString();
                    newReception.JobRequestId =Request.JobRequestId.ToString();
                    newReception.PlateNo = Request.LicensePlate;
                    if (fleet != null)
                    {
                        var equipmentName = fleet.Model;
                        var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == fleet.EquipmentTypeID).EquipmentTypeName;
                        var equipmentCategory = db.EquipmentIdentity.FirstOrDefault(x => x.EquipmentIdentityID == fleet.EquipmentIdentityID).EquipmentIdentityID;

                        newReception.VehicleCategoryId = equipmentCategory;
                        newReception.VehicleModel = equipmentName;
                        newReception.VehicleType = equipmentType;
                    }
                    db.VehicleReceptionForm.Add(newReception);
                    db.SaveChanges();
                    var parts = db.VehiclePart.Where(x => x.VehicleCategoryId == newReception.VehicleCategoryId).ToList();
                    IList<VehiclePartManagement> vehiclePartManagements = new List<VehiclePartManagement>();
                    foreach (var part in parts)
                    {
                        var vehiclePartManagement = new VehiclePartManagement();
                        vehiclePartManagement.VehiclePartId = part.VehiclePartId;
                        vehiclePartManagement.VehicleReceptionFormId = newReception.VehicleReceptionFormId;
                        db.VehiclePartManagement.Add(vehiclePartManagement);
                    }
                    db.SaveChanges();
                    var FetchCauses = db.JobCauses.Where(x => x.OrderId == Request.JobRequestId).ToList();
                    if (FetchCauses.Any())
                    {
                        foreach (var cause in FetchCauses)
                        {
                            cause.ShowOnReception = true;
                            db.Entry(cause).State = EntityState.Modified;

                        }
                        db.SaveChanges();
                    }

                }

                Request.IsSecondOnlineApproved = true;
                    Request.SecondSendForApprovalBy = User.Identity.Name;
                    Request.SecondOnlineApprovedTime = DateTime.Now;
                    Request.JobOrderStatus = JobOrderStatus.Progress;
                    Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Request with No " + Request.Num + " Approved ", Title = " Approved Job Request ", URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

                    Request.Status = ApprovalStatuses.Approve.ToString();

                    db.JobRequest.Attach(Request);
                    db.Entry(Request).State = EntityState.Modified;
                    db.SaveChanges();




                    var adminApproval = new AdministrativeJobDepartmentApproval();

                    adminApproval.AdministrativeJobId = Request.JobRequestId;
                    adminApproval.Status = ApprovalStatuses.Approve;
                    //  adminApproval.Remark = model.Remark;
                    adminApproval.CreatedBy = User.Identity.Name;
                    adminApproval.DateCreated = DateTime.Now;

                    db.AdministrativeJobDepartmentApproval.Add(adminApproval);
                    db.SaveChanges();
                var formattedDataa = new
                {
                    Message = "Approved Successefuly.",
                    Status = "Approved"
                };
                return Json(formattedDataa, JsonRequestBehavior.AllowGet);
            }


            //}

            //catch (Exception e)
            //{
            //    return Json(e.Message, JsonRequestBehavior.AllowGet);

            //}
            var formattedData = new
            {
                Message = "Approved Successefuly.",
                Status = "Approved"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RejectJobRequest(Guid id)

        {
            var request = new DataSourceRequest();
            // var jobRequest = db.JobRequest.Find(id);

            try
            {

                string Odometer = string.Empty;
                string EquipmentType = string.Empty;
                string AssetNum = string.Empty;
                string PlateNum = string.Empty;
                Guid EquipmentTypeId = Guid.Empty;

                var Request = db.JobRequest.Where(x => x.JobRequestId == id).FirstOrDefault();
                if (Request != null)
                {


                    Request.IsSecondOnlineApproved = false;
                    Request.Status = MaintenanceConst.Rejected;
                    db.JobRequest.Attach(Request);
                    db.Entry(Request).State = EntityState.Modified;
                    db.SaveChanges();
                    var adminApproval = new AdministrativeJobDepartmentApproval();

                    adminApproval.AdministrativeJobId = Request.JobRequestId;
                    adminApproval.Status = ApprovalStatuses.Reject;
                    // adminApproval.Remark = model.Remark;
                    adminApproval.CreatedBy = User.Identity.Name;
                    adminApproval.DateCreated = DateTime.Now;

                    db.AdministrativeJobDepartmentApproval.Add(adminApproval);
                    db.SaveChanges();

                }
            }

            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);

            }
            var formattedData = new
            {
                Message = "Rejected Successefuly.",
                Status = "Rejected"
            };
            return Json("Rejected Successefuly.", JsonRequestBehavior.AllowGet);
        }


        public string GetJobStatus(string Id)
        {

            JobRequest JobRequests = db.JobRequest.Where(j => j.JobRequestId.ToString() == Id && (j.IsOnlineApproved == true || j.IsSendForApproval == true)).FirstOrDefault();
            if (JobRequests != null)
            {
                return JobRequests.Status;
            }

            return "Pending";


        }

        public JsonResult EquipmentNameReturn(string plateNo)
        {
            var fleet = db.Fleets.FirstOrDefault(x => x.PlateNumber == plateNo || x.SerialNo == plateNo);
            //var equip = db.EquipmentInformation.FirstOrDefault(x => x.LicensePlate == plateNo);
            if (fleet != null)
            {
                var equipmentName = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == fleet.EquipmentNameID);
                var data = new
                {
                    EquipmentName = equipmentName.Name
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            else
            {
                var equip = db.EquipmentInformation.FirstOrDefault(x => x.LicensePlate == plateNo);
                var data = new
                {
                    EquipmentName = equip?.GroupName ?? ""
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }
        [AuthorizeRoles(MaintenanceRoles.MaintenanceVoid)]
        public ActionResult MakeVoid(string Id)
        {
            Guid.TryParse(Id, out Guid requestId);
            var jobRequest = db.JobRequest.Find(requestId);
            if (jobRequest == null) return RedirectToAction("Index");
            var jobOrder = db.JobOrder.FirstOrDefault(x => x.Referance == jobRequest.Num);
            if (jobOrder != null)
            {
                var jobDetail = db.JobOrderDetail.FirstOrDefault(x => x.JobOrderID == jobOrder.JobOrderID);
                if (jobDetail != null)
                {
                    db.JobOrderDetail.Remove(jobDetail);
                    db.SaveChanges();
                }
            }


            var requestApproval = db.AdministrativeJobApproval.Where(x => x.AdministrativeJobId == requestId);
            db.AdministrativeJobApproval.RemoveRange(requestApproval);

            jobRequest.Status = MaintenanceConst.Editing;
            jobRequest.IsOnlineApproved = false;
            jobRequest.IsSendForApproval = false;
            db.Entry(jobRequest).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }



}