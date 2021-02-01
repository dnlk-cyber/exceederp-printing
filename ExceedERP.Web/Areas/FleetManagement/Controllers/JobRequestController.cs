using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using System.Data.Entity;
using System.Data.Entity.Validation;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.MaintenanceRequestor, FleetManagementRoles.MaintenanceRequestApprover, FleetManagementRoles.MaintenanceServiceApprover)]
    public class JobRequestController : Controller
    {

        IFleetService _FleetService;
        IAdministrativeApprovalService _AdministrativeApprovalService;
        ITechnicalJobApprovalService _TechnicalJobApprovalService;
        IEquipmentIdentityService _EquipmentIdentityService;
        public const string Finished = "Finished";
        public const string Pending = "Pending";
        public const string Active = "Active";
        ExceedDbContext db = new ExceedDbContext();
        public JobRequestController()
        {

            _TechnicalJobApprovalService = new TechnicalJobApprovalService();
            _AdministrativeApprovalService = new AdministrativeJobApprovalService();
            _EquipmentIdentityService = new EquipmentIdentityService();
            _FleetService = new FleetService();
        }

        //
        // GET: /FleetManagement/JobRequest/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult JobRequest_Read([DataSourceRequest] DataSourceRequest request, string JobID)
        {

            var JobRequest = db.JobRequest.ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).ToList();

          
            System.Runtime.Caching.MemoryCache.Default["AssetList"] = db.AssetRegistrations.ToList();
          
            return Json(JobRequest.ToDataSourceResult(request));
        }

        public ActionResult Admin_Approve([DataSourceRequest] DataSourceRequest request, AdministrativeJobApproval model, string Id)
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
                    var Fleet = _FleetService.GetById(Request.FleetsID);
                    if (Fleet != null)
                    {

                        var FleetType = _EquipmentIdentityService.GetById(Fleet.EquipmentIdentityID);
                        if (FleetType != null)
                        {
                            EquipmentType = FleetType.EquipmentCategoryName;
                            EquipmentTypeId = FleetType.EquipmentIdentityID;
                        }
                        PlateNum = Fleet.PlateNumber;
                        AssetNum = Fleet.UnitNum;

                    }
                  
                        if (model.Status == ApprovalStatuses.Approve)
                        {
                            var JobOrder = new JobOrder();

                            JobOrder.JobOrderID = Guid.NewGuid();
                            JobOrder.FleetsID = Request.FleetsID;
                            JobOrder.ProjectName = Request.ProjectName;
                            JobOrder.CurrentFuel = Request.FuelAmount;
                            JobOrder.EquipmentName = Request.EquipmentName;
                            JobOrder.OperatorName = Request.OperatorName;
                            JobOrder.AssetNum = AssetNum;
                            JobOrder.Referance = Request.Num;
                            JobOrder.IsPM = Request.IsPM;
                            JobOrder.Odometer = Request.Odometer;
                            JobOrder.EquipmentType = EquipmentType;
                            JobOrder.EquipmentTypeId = EquipmentTypeId;
                            JobOrder.MaintenanceCenter = string.Empty;
                            JobOrder.PercentCompleted = string.Empty;
                            JobOrder.Odometer = Request.Odometer;
                            JobOrder.InspectorComments = string.Empty;
                            JobOrder.DateCompleted = DateTime.Now;
                            JobOrder.LabourCost = 0;
                            JobOrder.SpareCost = 0;
                            JobOrder.DateIssued = DateTime.Now;
                            JobOrder.Type = string.Empty;
                            JobOrder.EquipmentNo = PlateNum;
                            JobOrder.InspectedBy = Request.Requester;
                            JobOrder.Status = MaintenanceConst.Pending;
                            JobOrder.Site = Request.ProjectName;
                            JobOrder.SendForApprovalTime = DateTime.Now;
                            JobOrder.OnlineApprovedTime = DateTime.Now;
                            JobOrder.CreatedBy = User.Identity.Name;
                            JobOrder.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
               
                            db.JobOrder.Add(JobOrder);
                            db.SaveChanges();

                            Fleet.Status = MaintenanceConst.OnMaintenance;
                            Fleet.CurentOdoMeter = Request.Odometer;

                            _FleetService.Update(Fleet);

                            Request.IsOnlineApproved = true;
                            Request.OnlineApprovedBy = User.Identity.Name;
                            Request.OnlineApprovedTime = DateTime.Now;

                            Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Request with No " + Request.Num + " For Equipment " + Request.AssetNum + " Approved ", Title = " Approved Job Request ", URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

                        }
                        if (model.Status == ApprovalStatuses.Reject)
                        {

                            Request.IsSendForApproval = false;

                        }
                        Request.Status = model.Status.ToString();

                        db.JobRequest.Attach(Request);
                        db.Entry(Request).State = EntityState.Modified;
                        db.SaveChanges();




                        var adminApproval = new AdministrativeJobApproval();

                        adminApproval.AdministrativeJobId = Guid.Parse(Id);
                        adminApproval.Status = model.Status;
                        adminApproval.Remark = model.Remark;
                        adminApproval.CreatedBy = User.Identity.Name;
                        adminApproval.DateCreated = DateTime.Now;

                        db.AdministrativeJobApproval.Add(adminApproval);
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
            var Admin = new List<AdministrativeJobApproval>();
            var Request = db.JobRequest.Where(x => x.JobRequestId.ToString() == Id).FirstOrDefault();
            if (Request != null)
            {
                
                    Admin =
                        _AdministrativeApprovalService.GetAll()
                            .Where(x => x.AdministrativeJobId.ToString() == Id)
                            .ToList();


               

                if (Request.IsSendForApproval && !Request.IsOnlineApproved)
                {



                    var adminApproval = new AdministrativeJobApproval();
                    adminApproval.AdministrativeJobApprovalID = 0;
                    adminApproval.AdministrativeJobId = Guid.NewGuid();
                    adminApproval.Status = ApprovalStatuses.Pending;
                    adminApproval.Remark = string.Empty;
                    adminApproval.CreatedBy = User.Identity.Name;
                    adminApproval.DateCreated = DateTime.Now;

                    Admin.Add(adminApproval);

                }
            }
            return Json(Admin.ToDataSourceResult(request));
        }


        public ActionResult JobRequest_Create([DataSourceRequest] DataSourceRequest request, JobRequest JobRequest)
        {
            ModelState.Remove("JobRequestID");
            IFleetService _FleetService = new FleetService();
            IEquipmentIdentityService  _EquipmentIdentityService = new EquipmentIdentityService();
            string EquipmentType = string.Empty;

            Fleets fleet = _FleetService.GetAll().Where(x => x.FleetsID == JobRequest.FleetsID).FirstOrDefault();
            if (fleet != null)
            {
                EquipmentIdentity Type = _EquipmentIdentityService.GetById(fleet.EquipmentIdentityID);

                if (Type != null)
                {
                    EquipmentType = Type.EquipmentCategoryName;
                }

                JobRequest.JobRequestId = Guid.NewGuid();
                JobRequest.EquipmentName = fleet.PlateNumber +" - "+fleet.UnitNum;
                JobRequest.EquipmentType = EquipmentType;
                JobRequest.Model = fleet.Model;
                JobRequest.IsPM = JobRequest.IsPM;
                JobRequest.Remark = JobRequest.Remark;
                JobRequest.FleetsID = fleet.FleetsID;
                JobRequest.Status = MaintenanceConst.Pending;
                JobRequest.ProjectName = fleet.Location;
                JobRequest.Odometer = JobRequest.Odometer;
                JobRequest.RequestNumber = JobRequest.RequestNumber;
                JobRequest.OperatorName = fleet.OperatorName;
                JobRequest.AssetNum = fleet.UnitNum;
                JobRequest.CreatedBy = User.Identity.Name;
                JobRequest.DateCreated = DateTime.Now;
                JobRequest.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
               
                db.JobRequest.Add(JobRequest);
                db.SaveChanges();
            }
            return Json(new[] { JobRequest }.ToDataSourceResult(request));
        }

        public ActionResult JobRequest_Update([DataSourceRequest] DataSourceRequest request, JobRequest JobRequest)
        {
            ModelState.Remove("JobRequestID");
            IFleetService _FleetService = new FleetService();
            IEquipmentIdentityService _EquipmentIdentityService = new EquipmentIdentityService();
            string EquipmentType = string.Empty;

            Fleets fleet = _FleetService.GetAll().Where(x => x.FleetsID == JobRequest.FleetsID).FirstOrDefault();
            if (fleet != null)
            {
                EquipmentIdentity Type = _EquipmentIdentityService.GetById(fleet.EquipmentIdentityID);

                if (Type != null)
                {
                    EquipmentType = Type.EquipmentCategoryName;
                }

                 JobRequest.EquipmentName = fleet.PlateNumber + " - " + fleet.UnitNum;
                JobRequest.EquipmentType = EquipmentType;
                JobRequest.Model = fleet.Model;
                JobRequest.IsPM = JobRequest.IsPM;
                JobRequest.Remark = JobRequest.Remark;
                JobRequest.FleetsID = fleet.FleetsID;
                JobRequest.Status = MaintenanceConst.Pending;
                JobRequest.ProjectName = fleet.Location;
                JobRequest.Odometer = fleet.CurentOdoMeter;
                JobRequest.RequestNumber = JobRequest.RequestNumber;
                JobRequest.OperatorName = fleet.OperatorName;
                JobRequest.AssetNum = fleet.UnitNum;
                JobRequest.ModifiedBy = User.Identity.Name;
                JobRequest.LastModified = DateTime.Now;
                JobRequest.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
               
                db.JobRequest.Attach(JobRequest);
                db.Entry(JobRequest).State = EntityState.Modified;
                db.SaveChanges();
               
            }
            return Json(new[] { JobRequest }.ToDataSourceResult(request));
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




        public string GetJobStatus(string Id)
        {

            JobRequest JobRequests = db.JobRequest.Where(j => j.JobRequestId.ToString() == Id && (j.IsOnlineApproved == true || j.IsSendForApproval == true)).FirstOrDefault();
            if (JobRequests != null)
            {
                return JobRequests.Status;
            }
           
                return "Pending";
            

        }

    }



}