using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using DocumentFormat.OpenXml.Drawing.Diagrams;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Core.Domain.Maintenance;
using System.Data.Entity;
using ExceedERP.Web.Areas.FleetManagement.Models;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
   // [AuthorizeRoles(MaintenanceRoles.MaintenanceOrderUser, MaintenanceRoles.ServiceUser)]
    public class JobOrderDetailController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        IJobOrderDetailService _JobOrderDetailService;
        IJobOrderService _JobOrderService;
        IFleetService _FleetService;
        Guid joID = Guid.Empty;

        public JobOrderDetailController()
        {
            _JobOrderDetailService = new JobOrderDetailService();
            _FleetService = new FleetService();
            _JobOrderService = new JobOrderService();
        }



        public ActionResult GenrateStoreRequistion([DataSourceRequest] DataSourceRequest request, SelectStoreViewModel model, Guid Id)
        {
            return RedirectToAction("Index", "JobOrder");
        }

        public ActionResult GetServiceRequisition([DataSourceRequest] DataSourceRequest request, Guid Id)
        {
            List<StoreRequisitionItem> StoreRequisitions = new List<StoreRequisitionItem>();


            var Spares = db.ServiceSpare.Where(x => x.ServiceID == Id).ToList();

            foreach (var Spare in Spares)
            {

                var Requestions = db.JobStoreRequisition.Where(x => x.JobId == Spare.ServiceSpareID).ToList();

                foreach (var Requestion in Requestions)
                {

                    var RequesitionItem = db.StoreRequisitionItems.Where(x => x.StoreRequisitionItemID == Requestion.RequisitionId).FirstOrDefault();
                    if (RequesitionItem != null)
                    {
                        StoreRequisitions.Add(RequesitionItem);
                    }
                }
            }
            return Json(StoreRequisitions.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public JsonResult RequestService(string jobOrderID, [DataSourceRequest] DataSourceRequest request)
        {
            _JobOrderService = new JobOrderService();
            _JobOrderDetailService = new JobOrderDetailService();

            var jobrequest = db.JobRequest.Where(x => x.JobRequestId.ToString() == jobOrderID).FirstOrDefault();

            return Json(db.JobOrderDetail.Where(jobOrderDetail => jobOrderDetail.JobOrderID == jobrequest.JobRequestId).Select(j => new JobOrderDetail
                {

                    ServiceID = j.ServiceID,
                    JobOrderID = j.JobOrderID,
                    Status = j.Status,
                    JobOrderDetailID = j.JobOrderDetailID,
                    StartingDate = j.StartingDate,
                    ToDate = j.ToDate,
                    ApprovalStatus = j.ApprovalStatus,
                    RepairTypeID = j.RepairTypeID

            }
            ).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        public JsonResult SingleJobOrder(string jobOrderID, [DataSourceRequest] DataSourceRequest request)
        {
            _JobOrderService = new JobOrderService();
            _JobOrderDetailService = new JobOrderDetailService();

            var JobOrder = _JobOrderService.GetById(Guid.Parse(jobOrderID));
            var jobrequest = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();
            if (jobrequest != null)
            {
                return Json(db.JobOrderDetail.Where(jobOrderDetail => jobOrderDetail.JobOrderID.ToString() == jobOrderID.ToString() || jobOrderDetail.JobOrderID == jobrequest.JobRequestId).Select(j => new JobOrderDetail
                    {

                        ServiceID = j.ServiceID,
                        JobOrderID = j.JobOrderID,
                        Status = j.Status,
                        JobOrderDetailID = j.JobOrderDetailID,
                        StartingDate = j.StartingDate,
                        ToDate = j.ToDate,
                        ApprovalStatus = j.ApprovalStatus,
                    //ServiceName = j.Service.ServiceName
                    RepairTypeID = j.RepairTypeID

                }
                ).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            return Json(db.JobOrderDetail.Where(jobOrderDetail => jobOrderDetail.JobOrderID.ToString() == jobOrderID.ToString()).Select(j => new JobOrderDetail
               {

                   ServiceID = j.ServiceID,
                   JobOrderID = j.JobOrderID,
                   Status = j.Status,
                   JobOrderDetailID = j.JobOrderDetailID,
                   StartingDate = j.StartingDate,
                   ToDate = j.ToDate,
                   ApprovalStatus = j.ApprovalStatus,
                  // ServiceName = j.Service.ServiceName
                   RepairTypeID = j.RepairTypeID

            }
               ).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult OrderService(string jobOrderID, [DataSourceRequest] DataSourceRequest request)
        //{

        //    _JobOrderDetailService = new JobOrderDetailService();
        //    var Order = db.JobOrder.Where(x => x.JobOrderID.ToString() == jobOrderID).FirstOrDefault();
        //    var Request = db.JobRequest.Where(x => x.Num == Order.Referance).FirstOrDefault();


        //    return Json(db.JobOrderDetail.Where(jobOrderDetail => jobOrderDetail.JobOrderID.ToString() == jobOrderID || jobOrderDetail.JobOrderID == Request.JobRequestId).Select(j => new JobOrderDetail
        //        {

        //            ServiceID = j.ServiceID,
        //            JobOrderID = j.JobOrderID,
        //            JobCausesId = j.JobCausesId,
        //            Status = j.Status,
        //            Cause = j.JobCauses.Description,
        //            JobOrderDetailID = j.JobOrderDetailID,
        //            ServiceGroup = j.ServiceGroup,
        //            ServiceGroupSettingId = j.ServiceGroupSettingId,
        //            IsFromWorkShop = j.IsFromWorkShop,
        //            IsFromWorkShopRejected = j.IsFromWorkShopRejected,
        //            IsFromWorkShopApproved = j.IsFromWorkShopApproved,
        //            SubCategoryCode =j.SubCategoryCode,
        //            StartingDate = j.StartingDate,
        //            ToDate = j.ToDate,
        //            IsCreatedAfterApproval=CheckApprvalCreationTime(j.JobOrderID,j.DateCreated),
        //            DescriptionOfJobs = j.DescriptionOfJobs,
        //            ApprovalStatus = j.ApprovalStatus,
        //            DateCreated = j.DateCreated,
        //            OnlineApprovedTime = j.OnlineApprovedTime,
        //            //Section = GetSection(j.Service.SectionId),
        //            //ServiceName = j.Service.RepairType.Code + " - "+ j.Service.ServiceName,
        //            RepairTypeID = j.RepairTypeID,
        //            IsOnlineTransferred = Order.IsOnlineTransferred,
        //            Remark = Order.Remark,
        //            ServiceCenterId = j.ServiceCenterId,


        //        }
        //    ).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult OrderService(string jobOrderID, [DataSourceRequest] DataSourceRequest request)
        {
            var Order = db.JobOrder.FirstOrDefault(x => x.JobOrderID.ToString() == jobOrderID);
            var Request = db.JobRequest.FirstOrDefault(x => x.Num == Order.Referance);

            var jobOrderDetails = db.JobOrderDetail
                .Where(j => j.JobOrderID.ToString() == jobOrderID || j.JobOrderID == Request.JobRequestId)
                .AsEnumerable() // <<-- Forces EF to execute SQL, projection below is done in memory
                .Select(j => new JobOrderDetail
                {
                    ServiceID = j.ServiceID,
                    JobOrderID = j.JobOrderID,
                    JobCausesId = j.JobCausesId,
                    Status = j.Status,
                    Cause = j.JobCauses?.Description,
                    JobOrderDetailID = j.JobOrderDetailID,
                    ServiceGroup = j.ServiceGroup,
                    ServiceGroupSettingId = j.ServiceGroupSettingId,
                    IsFromWorkShop = j.IsFromWorkShop,
                    IsFromWorkShopRejected = j.IsFromWorkShopRejected,
                    IsFromWorkShopApproved = j.IsFromWorkShopApproved,
                    SubCategoryCode = j.SubCategoryCode,
                    StartingDate = j.StartingDate,
                    ToDate = j.ToDate,
                    IsCreatedAfterApproval = CheckApprvalCreationTime(j.JobOrderID, j.DateCreated),
                    DescriptionOfJobs = j.DescriptionOfJobs,
                    ApprovalStatus = j.ApprovalStatus,
                    DateCreated = j.DateCreated,
                    OnlineApprovedTime = j.OnlineApprovedTime,
                    RepairTypeID = j.RepairTypeID,
                    //IsOnlineTransferred = Order?.IsOnlineTransferred,
                    Remark = Order?.Remark,
                    ServiceCenterId = j.ServiceCenterId
                })
                .ToList();

            return Json(jobOrderDetails.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        private bool CheckApprvalCreationTime(Guid JId,DateTime? DateCreatedd)
        {
            bool IsAfterApproval = false;
            var fetchTechnicalApproval = db.TechnicalJobApproval.FirstOrDefault(x => x.TechnicalJobId == JId);
                if(fetchTechnicalApproval!=null)
            {
                IsAfterApproval = fetchTechnicalApproval.DateCreated < DateCreatedd;
            }
            return IsAfterApproval;
        }
            
        private string GetSubCategoryName(string Code)
        {
            string SubCategoryCode = null;
            if (Guid.TryParse(Code, out Guid subCode))
            {
                var fetchCode = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == subCode);
                if (fetchCode != null)
                {
                    SubCategoryCode = fetchCode.EquipmentTypeName;
                }
            }
            return SubCategoryCode;
        }
        private string GetSection(Guid Id)
        {
            string SectionName = string.Empty;

            var Section = db.WorkUnitSection.Where(x => x.WorkUnitSectionId == Id).FirstOrDefault();
            if (Section != null)
            {
                SectionName = Section.Name;
            }

            return SectionName;

        }


        public JsonResult ApproveOrderService(string jobOrderID, [DataSourceRequest] DataSourceRequest request)
        {

            _JobOrderDetailService = new JobOrderDetailService();
            var Order = db.JobOrder.Where(x => x.JobOrderID.ToString() == jobOrderID).FirstOrDefault();
            var Request = db.JobRequest.Where(x => x.Num == Order.Referance).FirstOrDefault();


            return Json(db.JobOrderDetail.Where(jobOrderDetail => jobOrderDetail.JobOrderID.ToString() == jobOrderID || jobOrderDetail.JobOrderID == Request.JobRequestId).Select(j => new JobOrderDetail
                {

                    ServiceID = j.ServiceID,
                    JobOrderID = j.JobOrderID,
                    Status = j.Status,
                    JobOrderDetailID = j.JobOrderDetailID,
                    StartingDate = j.StartingDate,
                    ToDate = j.ToDate,
                    ApprovalStatus = j.ApprovalStatus,
                    //ServiceName = j.Service.ServiceName,
                    RepairTypeID = j.RepairTypeID,
                    //Section = GetSection(j.Service.SectionId),
                    Remark = j.Remark,
                    IsOnlineTransferred = Order.IsOnlineTransferred

                }
            ).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdminJobOrderDetail_Create(string jobDetailOrderID, [DataSourceRequest] DataSourceRequest request, JobOrderDetail JobOrderDetail)
        {
            ModelState.Remove("ServiceID");
            ModelState.Remove("JobOrderDetailID");
            ModelState.Remove("JobOrderID");
            ModelState.Remove("ApprovalStatus");
            IServiceService mservice = new ServiceService();

            _JobOrderDetailService = new JobOrderDetailService();

            List<JobOrderDetail> JobOrderDetails = db.JobOrderDetail.Where(f => f.RepairTypeID == JobOrderDetail.RepairTypeID && f.JobOrderID == Guid.Parse(jobDetailOrderID)).ToList();
            if (JobOrderDetails.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingService", " Service Already Exists ! ");
            }
            else if (JobOrderDetail.StartingDate > JobOrderDetail.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingService", " Start Date Is Above End Date ! ");

            }
            else
            {

                JobOrderDetail.JobOrderDetailID = Guid.NewGuid();
                JobOrderDetail.ServiceID =
                    mservice.GetAll()
                        .Where(s => s.ServiceName == JobOrderDetail.RepairTypeID.ToString())
                        .Select(x => x.ServiceID)
                        .FirstOrDefault();
                JobOrderDetail.JobOrderID = Guid.Parse(jobDetailOrderID);

                if (JobOrderDetail != null)
                {
                    JobOrderDetail.ModifiedBy = User.Identity.Name;
                    JobOrderDetail.LastModified = DateTime.Now;
                    JobOrderDetail.Status = MaintenanceConst.Pending;
                    JobOrderDetail.ApprovalStatus = ApprovalStatuses.Pending;
                    _JobOrderDetailService.Insert(JobOrderDetail);

                    IServiceService _ServiceService = new ServiceService();
                    ServiceCrewService _ServiceCrewService = new ServiceCrewService();
                    List<ServiceCrew> JobCrew = new List<ServiceCrew>();

                    var Service = _ServiceService.GetById(JobOrderDetail.ServiceID);

                    IMechanicCrewService _MechanicCrewService = new MechanicCrewService();

                    var Crews = _MechanicCrewService.GetAll()
                        .Where(s => s.SectionId == Service.SectionId)
                        .Select(
                            f =>
                                new ServiceCrew
                                {
                                    ServiceCrewID = Guid.NewGuid(),
                                    ServiceID = JobOrderDetail.JobOrderDetailID,
                                    MechanicCrewID = f.MechanicCrewID,
                                    ServiceName = f.FullName,
                                    DateCreated = f.DateHired,
                                    HourlyWage = f.HourlyWage
                                }).ToList();
                    JobCrew.AddRange(Crews);
                    _ServiceCrewService.InsertBatch(JobCrew.DistinctByField(x => x.MechanicCrewID).ToList());

                }

            }
            return Json(new[] { JobOrderDetail }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        public JsonResult OpenServiceCenter(string DetailId)
        {
            var d = db.JobOrderDetail.FirstOrDefault(x => x.JobOrderDetailID.ToString() == DetailId);
            d.IsFromWorkShopApproved = true;
            d.ApprovalStatus = ApprovalStatuses.Approve;
            db.Entry(d).State = EntityState.Modified;
            db.SaveChanges();

            var jobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID == d.JobOrderID);

            ServiceCenterJob  jobOrderServiceCenter = new ServiceCenterJob
                            {
                                ServiceCenterJobId = Guid.NewGuid(),
                                AdvanceMeasure = jobOrder.AdvanceMeasure,
                                AssetNum = jobOrder.AssetNum,
                                BattreyCost = jobOrder.BattreyCost,
                                ComponentCost = jobOrder.ComponentCost,
                                CreatedBy = User.Identity.Name,
                                CurrentFuel = jobOrder.CurrentFuel,
                                CustomerId = jobOrder.CustomerId,
                                DateCompleted = jobOrder.DateCompleted,
                                DateCreated = jobOrder.DateCreated,
                                DateIssued = jobOrder.DateIssued,
                                Efficiency = jobOrder.Efficiency,
                                EquipmentInformationId = jobOrder.EquipmentInformationId,
                                EquipmentName = jobOrder.EquipmentName,
                                EquipmentNo = jobOrder.EquipmentNo,
                                EquipmentType = jobOrder.EquipmentType,
                                FailureReason = jobOrder.FailureReason,
                                FleetsID = jobOrder.FleetsID,
                                InspectedBy = jobOrder.InspectedBy,
                                InspectorComments = jobOrder.InspectorComments,
                                IsExternal = jobOrder.IsExternal,
                                IsOnlineApproved = jobOrder.IsOnlineApproved,
                                IsOnlineTransferred = jobOrder.IsOnlineTransferred,
                                IsOnlineTransferredBy = jobOrder.IsOnlineTransferredBy,
                                IsPM = jobOrder.IsPM,
                                IsRework = jobOrder.IsRework,
                                JobOrderID = jobOrder.JobOrderID,
                                JobOrderNum = jobOrder.JobOrderNum,
                                LabourCost = jobOrder.LabourCost,
                                LicensePlate = jobOrder.LicensePlate,
                                MaintenanceCenter = jobOrder.MaintenanceCenter,
                                MaintenanceCenterId = jobOrder.MaintenanceCenterId,
                                ServiceCenterId = d.ServiceCenterId,
                                Num = jobOrder.Num,
                                Odometer = jobOrder.Odometer,
                                OilLubricantCost = jobOrder.OilLubricantCost,
                                OperatorName = jobOrder.OperatorName,
                                OrgBranchName = jobOrder.OrgBranchName,
                                OtherCost = jobOrder.OtherCost,
                                Progress = jobOrder.Progress,
                                PercentCompleted = jobOrder.PercentCompleted,
                                ProjectName = jobOrder.ProjectName,
                                Referance = jobOrder.Referance,
                                RejectionReason = jobOrder.RejectionReason,
                                SalvageCost = jobOrder.SalvageCost,
                                ServiceCenterStatus = jobOrder.ServiceCenterStatus,
                                WorkShopRejectedReason = jobOrder.WorkShopRejectedReason,
                                Type = jobOrder.Type,
                                TyreCost = jobOrder.TyreCost,
                                Site = jobOrder.Site,
                                Status = jobOrder.Status,
                                SpareCost = jobOrder.SpareCost,
                                IsSentToWorkShop = jobOrder.IsSentToWorkShop,
                                IsSubmittedToServiceCenter = jobOrder.IsSubmittedToServiceCenter,
                                IsSecondOnlineApproved = jobOrder.IsSecondOnlineApproved,
                                IsSecondSendForApproval = jobOrder.IsSecondSendForApproval,
                                IsSendForApproval = jobOrder.IsSendForApproval,
                                IsVoid = jobOrder.IsVoid,
                                Remark = jobOrder.Remark,
                                IsExternalMaintenanceCenter = false,
                                IsInsured = jobOrder.IsInsured
                            };

                            db.ServiceCenterJob.Add(jobOrderServiceCenter);
                            db.SaveChanges();

            JobOrderServiceCenterDetail jobOrderServiceCenterDetail = new JobOrderServiceCenterDetail
                            {
                                JobOrderServiceCenterDetailId = Guid.NewGuid(),
                                ServiceCenterJobId = jobOrderServiceCenter.ServiceCenterJobId,
                                SubCategoryCode = d.SubCategoryCode,
                                JobCausesId = d.JobCausesId,
                                ServiceGroup = d.ServiceGroup,
                                RepairTypeID = d.RepairTypeID,
                                ServiceCenterId = d.ServiceCenterId,
                                Status = d.Status,
                                ApprovalStatus = d.ApprovalStatus,
                                Remark = d.Remark,
                                ServiceID = d.ServiceID,
                                ModifiedBy = d.ModifiedBy,
                                DateCreated = d.DateCreated,
                                JobOrderDetailId = d.JobOrderDetailID
                            };
                            db.JobOrderServiceCenterDetails.Add(jobOrderServiceCenterDetail);
                            db.SaveChanges();
                        

            var FormattedData = new
            {
                Message = "New Service Approved",
                Status = "OK"
            };
            return Json(FormattedData,JsonRequestBehavior.AllowGet);
        }public JsonResult RejectServiceCenter(string DetailId)
        {
            var d = db.JobOrderDetail.FirstOrDefault(x => x.JobOrderDetailID.ToString() == DetailId);
            d.IsFromWorkShopRejected = true;
            
            d.ApprovalStatus = ApprovalStatuses.Reject;
            db.Entry(d).State = EntityState.Modified;
            db.SaveChanges();
            

            var FormattedData = new
            {
                Message = "Service Rejected",
                Status = "OK"
            };
            return Json(FormattedData,JsonRequestBehavior.AllowGet);
        }

        public ActionResult JobOrderDetail_Create(string jobDetailOrderID, [DataSourceRequest] DataSourceRequest request, JobOrderDetail JobOrderDetail)
        {
            ModelState.Remove("ServiceID");
            ModelState.Remove("JobOrderDetailID");
            ModelState.Remove("JobOrderID");

            try
            {
                IServiceService mservice = new ServiceService();
                _JobOrderService = new JobOrderService();
                _JobOrderDetailService = new JobOrderDetailService();

                var JobOrder = db.JobOrder.Where(x => x.JobOrderID.ToString() == jobDetailOrderID).FirstOrDefault();

                if (JobOrder != null)
                {
                    var JobRequest = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();

                    //List<JobOrderDetail> JobOrderDetails = _JobOrderDetailService.GetAll().Where(f => f.RepairTypeID == JobOrderDetail.RepairTypeID && f.JobOrderID == JobOrder.JobOrderID).ToList();
                    //if (JobOrderDetails.Any())
                    //{
                    //    ModelState.Clear();
                    //    ModelState.AddModelError("ExistingService", " Service Already Exists ! ");
                    //    return Json(new[] { JobOrderDetail }.ToDataSourceResult(request, ModelState));
                    //}

                    if (JobRequest != null)
                    {

                        List<JobOrderDetail> JobDetail = db.JobOrderDetail.Where(f => f.RepairTypeID == JobOrderDetail.RepairTypeID
                                                                                && f.JobCausesId == JobOrderDetail.JobCausesId
                                                                                && f.ServiceGroup == JobOrderDetail.ServiceGroup
                                                                                && f.JobOrderID.ToString() == jobDetailOrderID)
                                                                                .ToList();
                        if (JobDetail.Any())
                        {
                            ModelState.Clear();
                            ModelState.AddModelError("ExistingService", " Service Already Exists ! ");
                            return Json(new[] { JobOrderDetail }.ToDataSourceResult(request, ModelState));
                        }
                    }
                }




                if (JobOrderDetail.StartingDate > JobOrderDetail.ToDate)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("ExistingService", " Start Date Is Above End Date ! ");
                    return Json(new[] { JobOrderDetail }.ToDataSourceResult(request, ModelState));
                }

                JobOrderDetail.JobOrderDetailID = Guid.NewGuid();
                JobOrderDetail.ServiceID = JobOrderDetail.ServiceID;
                    //mservice.GetAll()
                    //    .Where(s => s.ServiceName == JobOrderDetail.ServiceName)
                    //    .Select(x => x.ServiceID)
                    //    .FirstOrDefault();
                JobOrderDetail.JobOrderID = Guid.Parse(jobDetailOrderID);

                if (JobOrderDetail != null)
                {
                    JobOrderDetail.ModifiedBy = User.Identity.Name;
                    JobOrderDetail.DateCreated = DateTime.Now;
                    
                //    JobOrderDetail.OnlineApprovedTime = DateTime.Now;
                    JobOrderDetail.Status = MaintenanceConst.Pending;
                    JobOrderDetail.ApprovalStatus = ApprovalStatuses.Pending;
                    _JobOrderDetailService.Insert(JobOrderDetail);
                }


            }
            catch (Exception e)
            {
            }
            return Json(new[] { JobOrderDetail }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderDetail_Update([DataSourceRequest] DataSourceRequest request, JobOrderDetail JobOrderDetail)
        {
           // ModelState.Remove("SubCategoryCode");
            IServiceService mservice = new ServiceService();

            _JobOrderDetailService = new JobOrderDetailService();

            List<JobOrderDetail> JobOrderDetails = db.JobOrderDetail.Where(f => f.RepairTypeID == JobOrderDetail.RepairTypeID && f.JobCausesId == JobOrderDetail.JobCausesId && f.JobOrderDetailID != JobOrderDetail.JobOrderDetailID).ToList();
            if (JobOrderDetails.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingService", " Service Already Exists ! ");
                return Json(new[] { JobOrderDetail }.ToDataSourceResult(request, ModelState));
            }
            else
            {
                if (JobOrderDetail != null)
                {
                   
                    //var fetchPlate = db.JobOrder.FirstOrDefault(x => x.JobOrderID == JobOrderDetail.JobOrderID).LicensePlate;
                    //var FetchPlate = db.Fleets.FirstOrDefault(x => x.PlateNumber == fetchPlate);
                    //var FetchSubCategoryId = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == FetchPlate.EquipmentTypeID).EquipmentTypeID;
                    //JobOrderDetail.SubCategoryCode = FetchSubCategoryId.ToString();
                    _JobOrderDetailService = new JobOrderDetailService();

                    _JobOrderDetailService.Update(JobOrderDetail);
                }
            }

            return Json(new[] { JobOrderDetail }.ToDataSourceResult(request));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderDetail_Approve([DataSourceRequest] DataSourceRequest request, JobOrderDetail JobOrderDetail)
        {
            if (JobOrderDetail != null)
            {
                if (JobOrderDetail.ApprovalStatus == ApprovalStatuses.Approve)
                {
                    JobOrderDetail.IsOnlineApproved = true;
                    JobOrderDetail.OnlineApprovedTime = DateTime.Now;
                    JobOrderDetail.OnlineApprovedBy = User.Identity.Name;
                    db.JobOrderDetail.Attach(JobOrderDetail);
                    db.Entry(JobOrderDetail).State = EntityState.Modified;
                    db.SaveChanges();
                }


            }

            return Json(new[] { JobOrderDetail }.ToDataSourceResult(request));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderDetail_Destroy([DataSourceRequest] DataSourceRequest request, JobOrderDetail jobOrderDetail)
        {
            var JobOrderDetail = _JobOrderDetailService.GetById(jobOrderDetail.JobOrderDetailID);
            if (JobOrderDetail != null)
            {
                _JobOrderDetailService.Delete(JobOrderDetail);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Job Detail number " + jobOrderDetail.JobOrderDetailID);

            }

            return Json(new[] { JobOrderDetail }.ToDataSourceResult(request));
        }

        public JsonResult FromWorkShop_SendForApproval(string id)
        {
            var fetchApproval = db.TechnicalJobApproval.FirstOrDefault(x => x.TechnicalJobId.ToString() == id);
             if(fetchApproval!=null)
            {
                fetchApproval.IsFromWorkShop = true;
                db.Entry(fetchApproval).State = EntityState.Modified;
                db.SaveChanges();
            }
            var dt = new
            {
                Message = "Success",
                Status = "OK"
            };
            return Json(dt, JsonRequestBehavior.AllowGet);


        }
        public JsonResult ApproveService(string id)
        {
            var fetchApproval = db.TechnicalJobApproval.FirstOrDefault(x => x.TechnicalJobId.ToString() == id);
             if(fetchApproval!=null)
            {
                fetchApproval.IsApproved = true;
                db.Entry(fetchApproval).State = EntityState.Modified;
                db.SaveChanges();
                var jobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID.ToString() == id);
                if (jobOrder != null)
                {
                   
                    var jobOrderDetails = db.JobOrderDetail.Where(x => x.JobOrderID == jobOrder.JobOrderID&&x.IsFromWorkShop==true).ToList();
                    foreach (var d in jobOrderDetails)
                    {
                        d.IsFromWorkShopApproved = true;
                        db.Entry(d).State = EntityState.Modified;
                        db.SaveChanges();
                        var jobOrderServiceCenter = new ServiceCenterJob
                        {
                            ServiceCenterJobId = Guid.NewGuid(),
                            AdvanceMeasure = jobOrder.AdvanceMeasure,
                            AssetNum = jobOrder.AssetNum,
                            BattreyCost = jobOrder.BattreyCost,
                            ComponentCost = jobOrder.ComponentCost,
                            CreatedBy = User.Identity.Name,
                            CurrentFuel = jobOrder.CurrentFuel,
                            CustomerId = jobOrder.CustomerId,
                            DateCompleted = jobOrder.DateCompleted,
                            DateCreated = jobOrder.DateCreated,
                            DateIssued = jobOrder.DateIssued,
                            Efficiency = jobOrder.Efficiency,
                            EquipmentInformationId = jobOrder.EquipmentInformationId,
                            EquipmentName = jobOrder.EquipmentName,
                            EquipmentNo = jobOrder.EquipmentNo,
                            EquipmentType = jobOrder.EquipmentType,
                            FailureReason = jobOrder.FailureReason,
                            FleetsID = jobOrder.FleetsID,
                            InspectedBy = jobOrder.InspectedBy,
                            InspectorComments = jobOrder.InspectorComments,
                            IsExternal = jobOrder.IsExternal,
                            IsOnlineApproved = jobOrder.IsOnlineApproved,
                            IsOnlineTransferred = jobOrder.IsOnlineTransferred,
                            IsOnlineTransferredBy = jobOrder.IsOnlineTransferredBy,
                            IsPM = jobOrder.IsPM,
                            IsRework = jobOrder.IsRework,
                            JobOrderID = jobOrder.JobOrderID,
                            JobOrderNum = jobOrder.JobOrderNum,
                            LabourCost = jobOrder.LabourCost,
                            LicensePlate = jobOrder.LicensePlate,
                            MaintenanceCenter = jobOrder.MaintenanceCenter,
                            MaintenanceCenterId = jobOrder.MaintenanceCenterId,
                            ServiceCenterId = d.ServiceCenterId,
                            Num = jobOrder.Num,
                            Odometer = jobOrder.Odometer,
                            OilLubricantCost = jobOrder.OilLubricantCost,
                            OperatorName = jobOrder.OperatorName,
                            OrgBranchName = jobOrder.OrgBranchName,
                            OtherCost = jobOrder.OtherCost,
                            Progress = jobOrder.Progress,
                            PercentCompleted = jobOrder.PercentCompleted,
                            ProjectName = jobOrder.ProjectName,
                            Referance = jobOrder.Referance,
                            RejectionReason = jobOrder.RejectionReason,
                            SalvageCost = jobOrder.SalvageCost,
                            ServiceCenterStatus = jobOrder.ServiceCenterStatus,
                            WorkShopRejectedReason = jobOrder.WorkShopRejectedReason,
                            Type = jobOrder.Type,
                            TyreCost = jobOrder.TyreCost,
                            Site = jobOrder.Site,
                            Status = jobOrder.Status,
                            SpareCost = jobOrder.SpareCost,
                            IsSentToWorkShop = jobOrder.IsSentToWorkShop,
                            IsSubmittedToServiceCenter = jobOrder.IsSubmittedToServiceCenter,
                            IsSecondOnlineApproved = jobOrder.IsSecondOnlineApproved,
                            IsSecondSendForApproval = jobOrder.IsSecondSendForApproval,
                            IsSendForApproval = jobOrder.IsSendForApproval,
                            IsVoid = jobOrder.IsVoid,
                            Remark = jobOrder.Remark,
                            IsExternalMaintenanceCenter = false,
                            IsInsured = jobOrder.IsInsured
                        };

                        db.ServiceCenterJob.Add(jobOrderServiceCenter);
                        db.SaveChanges();

                        var jobOrderServiceCenterDetail = new JobOrderServiceCenterDetail
                        {
                            JobOrderServiceCenterDetailId = Guid.NewGuid(),
                            ServiceCenterJobId = jobOrderServiceCenter.ServiceCenterJobId,
                            SubCategoryCode = d.SubCategoryCode,
                            JobCausesId = d.JobCausesId,
                            ServiceGroup = d.ServiceGroup,
                            RepairTypeID = d.RepairTypeID,
                            ServiceCenterId = d.ServiceCenterId,
                            Status = d.Status,
                            ApprovalStatus = d.ApprovalStatus,
                            Remark = d.Remark,
                            ServiceID = d.ServiceID,
                            ModifiedBy = d.ModifiedBy,
                            DateCreated = d.DateCreated,
                            JobOrderDetailId = d.JobOrderDetailID
                        };
                        db.JobOrderServiceCenterDetails.Add(jobOrderServiceCenterDetail);
                        db.SaveChanges();
                    }
                }
            }
            var dt = new
            {
                Message = "Approved",
                Status = "OK"
            };
            return Json(dt, JsonRequestBehavior.AllowGet);


        }
        public JsonResult RejectService(string id)
        {
            var fetchApproval = db.TechnicalJobApproval.FirstOrDefault(x => x.TechnicalJobId.ToString() == id);
            if (fetchApproval != null)
            {
                fetchApproval.IsApproved = true;
                db.Entry(fetchApproval).State = EntityState.Modified;
                db.SaveChanges();
                var jobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID.ToString() == id);
                if (jobOrder != null)
                {

                    var jobOrderDetails = db.JobOrderDetail.Where(x => x.JobOrderID == jobOrder.JobOrderID && x.IsFromWorkShop == true).ToList();
                    foreach (var d in jobOrderDetails)
                    {
                        d.IsFromWorkShopRejected = true;
                        db.Entry(d).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            var dt = new
            {
                Message = "Rejected",
                Status = "OK"
            };
            return Json(dt, JsonRequestBehavior.AllowGet);

        }
        public ActionResult JobDetailDDBind(string Id)
        {
            var Service = db.JobOrderDetail.Where(x => x.JobOrderID.ToString() == Id).Select(s => new Service
            {
                // ServiceName = s.Service.RepairType.Name + " - " + s.Service.RepairType.Code,
                ServiceName = s.RepairTypeID.ToString(),
                ServiceID = s.JobOrderDetailID
            }).ToList();

            return Json(Service, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

