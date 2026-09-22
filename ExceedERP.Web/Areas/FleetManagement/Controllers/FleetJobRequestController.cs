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

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(MaintenanceRoles.MaintenanceRequestor, MaintenanceRoles.MaintenanceRequestApprover, MaintenanceRoles.MaintenanceServiceApprover)]
    public class FleetJobRequestController : Controller
    {
        private readonly IFleetService _FleetService;
        private IAdministrativeApprovalService _AdministrativeApprovalService;
        private readonly ITechnicalJobApprovalService _TechnicalJobApprovalService;
        private readonly IEquipmentIdentityService _EquipmentIdentityService;
        public const string Finished = "Finished";
        public const string Pending = "Pending";
        public const string Active = "Active";
        private ExceedDbContext db = new ExceedDbContext();
        public FleetJobRequestController()
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
            ViewData["Customer"] = db.OrganizationCustomers.ToList();
            ViewData["equipmentInfo"] = db.EquipmentInformation.Select(s => new
            {
                PlateNumber = s.LicensePlate,
                Name = s.LicensePlate
            }).ToList();
            ViewData["MaintenanceCenter"] = db.MaintenanceCenter.Select(s => new
            {
                Id = s.MaintenanceCenterID,
                Name = s.Name
            }).ToList();

            PopulateViewData();
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
        public ActionResult JobRequest_Read([DataSourceRequest] DataSourceRequest request, string JobID)
        {

            var JobRequest = db.JobRequest.Where(x => x.JobOrderStatus != JobOrderStatus.Delivered).ToList();
           // var printJobRequest = db.JobRequest.Where(jid => jid.JobRequestId.ToString() == JobID).ToList();
            var Result = JobRequest.ToDataSourceResult(request, jobRequest => new
            {
                BranchId = jobRequest.BranchId,
                BusinessUnitId = jobRequest.BusinessUnitId,
                CreatedAtBranchId = jobRequest.CreatedAtBranchId,
                CreatedAtBusinessUnitId = jobRequest.CreatedAtBusinessUnitId,
                CustomerId = jobRequest.CustomerId,
                EquipmentInformationId = jobRequest.EquipmentInformationId,
                EquipmentName = jobRequest.EquipmentName,
                EquipmentType = jobRequest.EquipmentType,
                EstmatedCostAmount = jobRequest.EstmatedCostAmount,
                FuelAmount = jobRequest.FuelAmount,
                HasInsurance = jobRequest.HasInsurance,
                IsHighPriority = jobRequest.IsHighPriority,
                IsOnlineApproved = jobRequest.IsOnlineApproved,
                IsOnlineTransferred = jobRequest.IsOnlineTransferred,
                IsOnlineTransferredBy = jobRequest.IsOnlineTransferredBy,
                IsPM = jobRequest.IsPM,
                IsSendForApproval = jobRequest.IsSendForApproval,
                IsVoid = jobRequest.IsVoid,
                JobOrderStatus = jobRequest.JobOrderStatus,
                JobRequestId = jobRequest.JobRequestId,
                LastKmHr = jobRequest.LastKmHr,
                LastModified = jobRequest.LastModified,
                LicensePlate = jobRequest.LicensePlate,
                MaintenanceCenter = jobRequest.MaintenanceCenter,
                MaintenanceCenterId = jobRequest.MaintenanceCenterId,
                Model = jobRequest.Model,
                ModifiedBy = jobRequest.ModifiedBy,
                Num = jobRequest.Num,
                DateCreated = jobRequest.DateCreated,
                Odometer = jobRequest.Odometer,
                OnlineApprovedBy = jobRequest.OnlineApprovedBy,
                OnlineApprovedTime = jobRequest.OnlineApprovedTime,
                OnlineTransferredTime = jobRequest.OnlineTransferredTime,
                OperatorName = jobRequest.OperatorName,
                OrgBranchName = jobRequest.OrgBranchName,
                OrgStructureId = jobRequest.OrgStructureId,
                Remark = jobRequest.Remark,
                Requester = jobRequest.Requester,
                RequesterId = jobRequest.RequesterId,
                RequestNumber = jobRequest.RequestNumber,
                SendForApprovalBy = jobRequest.SendForApprovalBy,
                SendForApprovalTime = jobRequest.SendForApprovalTime,
                Source = jobRequest.Source,
                Status = jobRequest.Status,
                VeheicleReceptionFormId = jobRequest.VeheicleReceptionFormId,
                VoidBy = jobRequest.VoidBy,
                VoidTime = jobRequest.VoidTime
                
            });

            return Json(Result);
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
                    //var Fleet = _FleetService.GetById(Request.FleetsID);
                    //if (Fleet != null)
                    //{

                    //    var FleetType = _EquipmentIdentityService.GetById(Fleet.EquipmentIdentityID);
                    //    if (FleetType != null)
                    //    {
                    //        EquipmentType = FleetType.EquipmentCategoryName;
                    //        EquipmentTypeId = FleetType.EquipmentIdentityID;
                    //    }
                    //    PlateNum = Fleet.PlateNumber;
                    //    AssetNum = Fleet.UnitNum;

                    //}

                    if (model.Status == ApprovalStatuses.Approve)
                    {
                        var JobOrder = new JobOrder();

                        JobOrder.JobOrderID = Guid.NewGuid();
                        JobOrder.EquipmentInformationId = Request.EquipmentInformationId == null ? Guid.Empty : Request.EquipmentInformationId.Value;
                        //JobOrder.ProjectName = Request.ProjectName;
                        JobOrder.CurrentFuel = Request.FuelAmount;
                        JobOrder.LicensePlate = Request.LicensePlate;
                        JobOrder.EquipmentName = Request.EquipmentName;
                        JobOrder.OperatorName = Request.OperatorName;
                        JobOrder.AssetNum = AssetNum;
                        JobOrder.Referance = Request.Num;
                        JobOrder.IsPM = Request.IsPM;
                        JobOrder.Odometer = Request.Odometer;
                        JobOrder.EquipmentType = EquipmentType;
                        //JobOrder.EquipmentTypeId = EquipmentTypeId;
                        JobOrder.MaintenanceCenter = string.Empty;
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

                        db.JobOrder.Add(JobOrder);
                        db.SaveChanges();

                        //Fleet.Status = MaintenanceConst.OnMaintenance;
                        //Fleet.CurentOdoMeter = Request.Odometer;

                        //_FleetService.Update(Fleet);

                        Request.IsOnlineApproved = true;
                        Request.OnlineApprovedBy = User.Identity.Name;
                        Request.OnlineApprovedTime = DateTime.Now;

                        Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Request with No " + Request.Num + " Approved ", Title = " Approved Job Request ", URL = Url.Action("Index", "FleetJobRequest", new { area = "FleetManagment" }) });

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
            ModelState.Remove("EquipmentInformationId");
            ModelState.Remove("Num");
            ModelState.Remove("DateCreated");
            ModelState.Remove("OnlineApprovedTime");
            ModelState.Remove("DeliveredDate");
            ModelState.Remove("EstimatedCostAmount");
            ModelState.Remove("Status");
            ModelState.Remove("JobOrderStatus");
           // ModelState.Remove("RequestId");
          

            IFleetService _FleetService = new FleetService();
            IEquipmentIdentityService _EquipmentIdentityService = new EquipmentIdentityService();
            string EquipmentType = string.Empty;

            //Fleets fleet = _FleetService.GetAll().Where(x => x.FleetsID == JobRequest.FleetsID).FirstOrDefault();
            //if (fleet != null)
            //{
            //EquipmentIdentity Type = _EquipmentIdentityService.GetById(fleet.EquipmentIdentityID);

            //if (Type != null)
            //{
            //    EquipmentType = Type.EquipmentCategoryName;
            //}
            #region data 
            if (JobRequest.RequesterId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }

            if (JobRequest.RequesterId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == JobRequest.RequesterId && x.isCurrent);
                if (placement != null)
                {
                    JobRequest.OrgStructureId = placement.OrgStructureId;
                    JobRequest.BranchId = placement.BranchId;
                    JobRequest.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }

            JobRequest.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            JobRequest.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            #endregion
            JobRequest.JobRequestId = Guid.NewGuid();
            JobRequest.Source = JobRequest.Source;
            //JobRequest.EquipmentName = fleet.PlateNumber + " - " + fleet.UnitNum;
            JobRequest.EquipmentType = EquipmentType;
            //JobRequest.Model = fleet.Model;
            JobRequest.IsPM = JobRequest.IsPM;
            JobRequest.Remark = JobRequest.Remark;
            JobRequest.EquipmentInformationId = JobRequest.EquipmentInformationId;
            JobRequest.Status = MaintenanceConst.Pending;
            //JobRequest.ProjectName = fleet.Location;
            JobRequest.Odometer = JobRequest.Odometer;
            JobRequest.LicensePlate = JobRequest.LicensePlate;
            
            JobRequest.CustomerId = JobRequest.CustomerId;
            JobRequest.RequestNumber = JobRequest.RequestNumber;
            //JobRequest.OperatorName = fleet.OperatorName;
            //JobRequest.AssetNum = fleet.UnitNum;
            JobRequest.CreatedBy = User.Identity.Name;
            JobRequest.DateCreated = DateTime.Now;
            JobRequest.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
            //if(JobRequest.Source == JobRequestSource.Internal)
            //{
            //    var eqt = db.EquipmentInformation.FirstOrDefault(w => w.LicensePlate == JobRequest.LicensePlate);
            //    if(eqt != null)
            //    {
            //        JobRequest.EquipmentInformationId = eqt.EquipmentInformationId;
            //    }
            //}
            db.JobRequest.Add(JobRequest);
            db.SaveChanges();
            //}
            return Json(new[] { JobRequest }.ToDataSourceResult(request));
        }

        public ActionResult JobRequest_Update([DataSourceRequest] DataSourceRequest request, JobRequest JobRequest)
        {
            ModelState.Remove("JobRequestID");
            ModelState.Remove("EquipmentInformationId");
            IFleetService _FleetService = new FleetService();
            IEquipmentIdentityService _EquipmentIdentityService = new EquipmentIdentityService();
            string EquipmentType = string.Empty;

            //Fleets fleet = _FleetService.GetAll().Where(x => x.FleetsID == JobRequest.FleetsID).FirstOrDefault();
            //if (fleet != null)
            //{
            //    EquipmentIdentity Type = _EquipmentIdentityService.GetById(fleet.EquipmentIdentityID);

            //    if (Type != null)
            //    {
            //        EquipmentType = Type.EquipmentCategoryName;
            //    }

            //JobRequest.Source = JobRequest.Source;
            //JobRequest.EquipmentName = fleet.PlateNumber + " - " + fleet.UnitNum;
            //JobRequest.EquipmentType = EquipmentType;
            //JobRequest.Model = fleet.Model;
            //JobRequest.IsPM = JobRequest.IsPM;
            //JobRequest.Remark = JobRequest.Remark;
            //JobRequest.FleetsID = fleet.FleetsID;
            //JobRequest.Status = MaintenanceConst.Pending;
            //JobRequest.ProjectName = fleet.Location;
            //JobRequest.Odometer = fleet.CurentOdoMeter;
            //JobRequest.RequestNumber = JobRequest.RequestNumber;
            //JobRequest.OperatorName = fleet.OperatorName;
            //JobRequest.AssetNum = fleet.UnitNum;
            if (JobRequest.Source == JobRequestSource.CompanyVehicle)
            {
                //var eqt = db.EquipmentInformation.FirstOrDefault(w => w.LicensePlate == JobRequest.LicensePlate);
                //if (eqt != null)
                //{
                //    JobRequest.EquipmentInformationId = eqt.EquipmentInformationId;
                //}
            }
            JobRequest.ModifiedBy = User.Identity.Name;
            JobRequest.LastModified = DateTime.Now;
            JobRequest.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

            db.JobRequest.Attach(JobRequest);
            db.Entry(JobRequest).State = EntityState.Modified;
            db.SaveChanges();

            //}
            return Json(new[] { JobRequest }.ToDataSourceResult(request));
        }

        public ActionResult JobRequest_Destroy([DataSourceRequest] DataSourceRequest request, JobRequest JobRequest)
        {
            ModelState.Remove("JobRequestID");
            ModelState.Remove("EquipmentInformationId");

            var req = db.JobRequest.Find(JobRequest.JobRequestId);

            db.JobRequest.Attach(req);
            db.JobRequest.Remove(req);
            db.SaveChanges();

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

            try
            {

                string Odometer = string.Empty;
                string EquipmentType = string.Empty;
                string AssetNum = string.Empty;
                string PlateNum = string.Empty;
                Guid EquipmentTypeId = Guid.Empty;
                //  var model = db.AdministrativeJobApproval.Find(id);

                var Request = db.JobRequest.Where(x => x.JobRequestId == id).FirstOrDefault();
                var fleet = db.Fleets.FirstOrDefault(x => x.PlateNumber == Request.LicensePlate || x.SerialNo == Request.LicensePlate);
                if (Request != null && fleet != null)
                {

                    //if (model.Status == ApprovalStatuses.Approve)
                    //{
                    var JobOrder = new JobOrder();

                    JobOrder.JobOrderID = Guid.NewGuid();
                    JobOrder.EquipmentInformationId = Request.EquipmentInformationId == null ? Guid.Empty : Request.EquipmentInformationId.Value;
                    //JobOrder.ProjectName = Request.ProjectName;
                    JobOrder.CurrentFuel = Request.FuelAmount;
                    JobOrder.LicensePlate = Request.LicensePlate;
                    JobOrder.FleetsID = fleet.FleetsID;
                    JobOrder.EquipmentName = Request.EquipmentName;
                    JobOrder.OperatorName = Request.OperatorName;
                    JobOrder.AssetNum = AssetNum;
                    JobOrder.Referance = Request.Num;
                    JobOrder.IsPM = Request.IsPM;
                    JobOrder.Odometer = Request.Odometer;
                    JobOrder.EquipmentType = EquipmentType;
                    //JobOrder.EquipmentTypeId = EquipmentTypeId;
                    JobOrder.MaintenanceCenter = string.Empty;
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

                    db.JobOrder.Add(JobOrder);
                    db.SaveChanges();

                    //Fleet.Status = MaintenanceConst.OnMaintenance;
                    //Fleet.CurentOdoMeter = Request.Odometer;

                    //_FleetService.Update(Fleet);

                    Request.IsOnlineApproved = true;
                    Request.OnlineApprovedBy = User.Identity.Name;
                    Request.OnlineApprovedTime = DateTime.Now;

                    Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Request with No " + Request.Num + " Approved ", Title = " Approved Job Request ", URL = Url.Action("Index", "FleetJobRequest", new { area = "FleetManagment" }) });

                    // }
                    //if (model.Status == ApprovalStatuses.Reject)
                    //{

                    //    Request.IsSendForApproval = false;

                    //}
                    Request.Status = ApprovalStatuses.Approve.ToString();

                    db.JobRequest.Attach(Request);
                    db.Entry(Request).State = EntityState.Modified;
                    db.SaveChanges();




                    var adminApproval = new AdministrativeJobApproval();

                    adminApproval.AdministrativeJobId = Request.JobRequestId;
                    adminApproval.Status = ApprovalStatuses.Approve;
                    //  adminApproval.Remark = model.Remark;
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

                    Request.IsSendForApproval = false;
                    Request.IsOnlineApproved = false;

                    Request.Status = MaintenanceConst.WaitingAdminstrative;
                    db.JobRequest.Attach(Request);
                    db.Entry(Request).State = EntityState.Modified;
                    db.SaveChanges();
                    var adminApproval = new AdministrativeJobApproval();

                    adminApproval.AdministrativeJobId = Request.JobRequestId;
                    adminApproval.Status = ApprovalStatuses.Reject;
                    // adminApproval.Remark = model.Remark;
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }



}