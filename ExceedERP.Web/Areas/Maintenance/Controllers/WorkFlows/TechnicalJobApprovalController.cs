using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Web.Areas.Maintenance.Models;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    [AuthorizeRoles(MaintenanceRoles.ServiceUser, MaintenanceRoles.MaintenanceOrderUser)]
    public class TechnicalJobApprovalController : Controller
    {

        IJobOrderService _JobOrderService;
        IJobOrderDetailService _JobOrderDetailService;
        IFleetService _FleetService;
        ITechnicalJobApprovalService _TechnicalJobApprovalService;
        IServiceCrewService _ServiceCrewService;
        IServiceService _ServiceService;
        ExceedDbContext db = new ExceedDbContext();
        public const string Finished = "Finished";
        public const string Pending = "Pending";
        public const string Active = "Active";
        public TechnicalJobApprovalController()
        {
            _JobOrderService = new JobOrderService();
            _FleetService = new FleetService();
            _ServiceService = new ServiceService();
            _TechnicalJobApprovalService = new TechnicalJobApprovalService();
            _ServiceCrewService = new ServiceCrewService();
            _JobOrderDetailService = new JobOrderDetailService();
        }

        //
        // GET: /FleetManagement/TechnicalJobApproval/

        public ActionResult Technical_Read([DataSourceRequest] DataSourceRequest request, string TecnicalID)
        {
            var JobOrder = _TechnicalJobApprovalService.GetAll().Where(x => x.TechnicalJobId.ToString() == TecnicalID).Select(j => new TechnicalApprovalViewModel
            {
                Remark = j.Remark,
                MaintenanceCenter = "",
                //Type = "",

                Status = j.Status

            });

            return Json(JobOrder.ToDataSourceResult(request));
        }


        public ActionResult TechnicalApproval_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            var Admin = new List<TechnicalApprovalViewModel>();
            Guid JId = Guid.Parse(Id);
            var Request = db.JobOrder.Where(x => x.JobOrderID.ToString() == Id).FirstOrDefault();
            if (Request != null)
            {
                if (Request.IsOnlineApproved)
                {
                    Admin =
                        db.TechnicalJobApproval
                            .Where(x => x.TechnicalJobId.ToString() == Id).Select(s => new TechnicalApprovalViewModel
                            {
                                JobOrderID = JId,
                                DateCreated = s.DateCreated,
                                CreatedBy = s.CreatedBy,
                                ApproveStatus = ApprovalStatuses.Approve,
                                // Type = Request.IsExternal ? "EXTERNAL" : "INTERNAL",
                                MaintenanceCenter = Request.MaintenanceCenter,
                                MaintenanceCenterId = Request.MaintenanceCenterId

                            })
                            .ToList();

                    Admin = Admin;
                }

                else if (Request.IsSendForApproval && !Request.IsOnlineApproved)
                {
                    var adminApproval = new TechnicalApprovalViewModel();
                    adminApproval.JobOrderID = JId;
                    adminApproval.Status = ApprovalStatuses.Pending;
                    adminApproval.Remark = string.Empty;
                    adminApproval.CreatedBy = User.Identity.Name;
                    adminApproval.DateCreated = DateTime.Now;
                    Admin.Add(adminApproval);

                }
            }
            return Json(Admin.ToDataSourceResult(request));
        }



        public ActionResult Technical_Create([DataSourceRequest] DataSourceRequest request, TechnicalJobApproval TechnicalJobApproval, String JobID)
        {


            if (TechnicalJobApproval != null)
            {
                TechnicalJobApproval.TechnicalJobId = Guid.Parse(JobID);
                TechnicalJobApproval.DateCreated = DateTime.Now;
                TechnicalJobApproval.CreatedBy = User.Identity.Name;
                _TechnicalJobApprovalService.Insert(TechnicalJobApproval);

            }

            return Json(new[] { TechnicalJobApproval }.ToDataSourceResult(request));
        }

        public ActionResult Technical_Update([DataSourceRequest] DataSourceRequest request, TechnicalJobApproval TechnicalJobApproval)
        {


            if (TechnicalJobApproval != null)
            {
                TechnicalJobApproval.LastModified = DateTime.Now;
                TechnicalJobApproval.ModifiedBy = User.Identity.Name;
                _TechnicalJobApprovalService.Update(TechnicalJobApproval);

            }

            return Json(new[] { TechnicalJobApproval }.ToDataSourceResult(request));
        }

        public ActionResult Technical_Approval([DataSourceRequest] DataSourceRequest request, TechnicalApprovalViewModel technicalApprovalViewModel)
        {
            try
            {
                ExceedDbContext db = new ExceedDbContext();
                TechnicalJobApproval approval = new TechnicalJobApproval();
                _JobOrderDetailService = new JobOrderDetailService();
                var jobOrder = db.JobOrder.Where(x => x.JobOrderID == technicalApprovalViewModel.JobOrderID).FirstOrDefault();
                var maintenanceCenter = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID == technicalApprovalViewModel.MaintenanceCenterId);
                technicalApprovalViewModel.MaintenanceCenter = maintenanceCenter.Name;
                if (technicalApprovalViewModel.ApproveStatus == ApprovalStatuses.Approve && jobOrder != null && maintenanceCenter != null)
                {
                    var Jobs = db.JobOrder.Where(x => x.JobOrderID == technicalApprovalViewModel.JobOrderID).FirstOrDefault();
                    var JobRequest = db.JobRequest.Where(x => x.Num == Jobs.Referance).FirstOrDefault();

                    var jobOrderDetail = new List<JobOrderDetail>();
                    if (JobRequest != null)
                    {
                        jobOrderDetail = _JobOrderDetailService.GetAll()
                         .Where(x => x.JobOrderID == technicalApprovalViewModel.JobOrderID || x.JobOrderID == JobRequest.JobRequestId).ToList();
                        foreach (var item in jobOrderDetail)
                        {
                            item.ApprovalStatus = ApprovalStatuses.Approve;
                            _JobOrderDetailService.Update(item);
                        }
                    }
                    else
                    {

                        jobOrderDetail = _JobOrderDetailService.GetAll()
                    .Where(x => x.JobOrderID == technicalApprovalViewModel.JobOrderID).ToList();
                        foreach (var item in jobOrderDetail)
                        {
                            item.ApprovalStatus = ApprovalStatuses.Approve;
                            _JobOrderDetailService.Update(item);
                        }

                    }
                    List<ServiceCrew> JobCrew = new List<ServiceCrew>();
                    _ServiceCrewService.InsertBatch(JobCrew.DistinctByField(x => x.MechanicCrewID).ToList());
                    jobOrder.MaintenanceCenterId = technicalApprovalViewModel.MaintenanceCenterId;
                    jobOrder.MaintenanceCenter = technicalApprovalViewModel.MaintenanceCenter;
                    //jobOrder.KMReading = technicalApprovalViewModel.KMReading;
                    //jobOrder.GPSInfo = technicalApprovalViewModel.GPSInfo;
                    jobOrder.OnlineApprovedBy = User.Identity.Name;
                    jobOrder.OnlineApprovedTime = DateTime.Now;
                    jobOrder.IsOnlineApproved = true;
                    jobOrder.Status = MaintenanceConst.Open;
                    db.JobOrder.Attach(jobOrder);
                    db.Entry(jobOrder).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    if (jobOrder != null)
                    {
                        approval = new TechnicalJobApproval();
                        approval.TechnicalJobId = jobOrder.JobOrderID;
                        approval.Status = technicalApprovalViewModel.ApproveStatus;
                        approval.Remark = technicalApprovalViewModel.Remark;
                        approval.CreatedBy = User.Identity.Name;
                        approval.DateCreated = DateTime.Now;

                        _TechnicalJobApprovalService.Insert(approval);
                    }
                    if (maintenanceCenter.IsExternal)
                    {
                        #region create service center External


                        var jobOrderDetails = db.JobOrderDetail.Where(x => x.JobOrderID == jobOrder.JobOrderID).ToList();
                        var d = jobOrderDetails.First();
                        var jobOrderServiceCenter = new ServiceCenterJob
                        {
                            ServiceCenterJobId = Guid.NewGuid(),
                            AdvanceMeasure = jobOrder.AdvanceMeasure,
                            AssetNum = jobOrder.AssetNum,
                            BattreyCost = jobOrder.BattreyCost,
                            ComponentCost = jobOrder.ComponentCost,
                            CreatedBy = jobOrder.CreatedBy,
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
                            IsExternalMaintenanceCenter = true,
                            IsInsured = jobOrder.IsInsured

                        };
                        db.ServiceCenterJob.Add(jobOrderServiceCenter);
                        db.SaveChanges();
                        foreach (var dd in jobOrderDetails)
                        {
                            var jobOrderServiceCenterDetail = new JobOrderServiceCenterDetail
                            {
                                JobOrderServiceCenterDetailId = Guid.NewGuid(),
                                ServiceCenterJobId = jobOrderServiceCenter.ServiceCenterJobId,
                                SubCategoryCode = dd.SubCategoryCode,
                                JobCausesId = dd.JobCausesId,
                                ServiceGroup = dd.ServiceGroup,
                                RepairTypeID = dd.RepairTypeID,
                                ServiceCenterId = dd.ServiceCenterId,
                                Status = dd.Status,
                                ApprovalStatus = dd.ApprovalStatus,
                                Remark = dd.Remark,
                                ServiceID = dd.ServiceID,
                                ModifiedBy = dd.ModifiedBy,
                                DateCreated = dd.DateCreated,
                                JobOrderDetailId = dd.JobOrderDetailID

                            };
                            db.JobOrderServiceCenterDetails.Add(jobOrderServiceCenterDetail);
                            db.SaveChanges();
                        }
                        //foreach (var d in jobOrderDetails)
                        //{
                        //    var jobOrderServiceCenter = new ServiceCenterJob
                        //    {
                        //        ServiceCenterJobId = Guid.NewGuid(),
                        //        AdvanceMeasure = jobOrder.AdvanceMeasure,
                        //        AssetNum = jobOrder.AssetNum,
                        //        BattreyCost = jobOrder.BattreyCost,
                        //        ComponentCost = jobOrder.ComponentCost,
                        //        CreatedBy = jobOrder.CreatedBy,
                        //        CurrentFuel = jobOrder.CurrentFuel,
                        //        CustomerId = jobOrder.CustomerId,
                        //        DateCompleted = jobOrder.DateCompleted,
                        //        DateCreated = jobOrder.DateCreated,
                        //        DateIssued = jobOrder.DateIssued,
                        //        Efficiency = jobOrder.Efficiency,
                        //        EquipmentInformationId = jobOrder.EquipmentInformationId,
                        //        EquipmentName = jobOrder.EquipmentName,
                        //        EquipmentNo = jobOrder.EquipmentNo,
                        //        EquipmentType = jobOrder.EquipmentType,
                        //        FailureReason = jobOrder.FailureReason,
                        //        FleetsID = jobOrder.FleetsID,
                        //        InspectedBy = jobOrder.InspectedBy,
                        //        InspectorComments = jobOrder.InspectorComments,
                        //        IsExternal = jobOrder.IsExternal,
                        //        IsOnlineApproved = jobOrder.IsOnlineApproved,
                        //        IsOnlineTransferred = jobOrder.IsOnlineTransferred,
                        //        IsOnlineTransferredBy = jobOrder.IsOnlineTransferredBy,
                        //        IsPM = jobOrder.IsPM,
                        //        IsRework = jobOrder.IsRework,
                        //        JobOrderID = jobOrder.JobOrderID,
                        //        JobOrderNum = jobOrder.JobOrderNum,
                        //        LabourCost = jobOrder.LabourCost,
                        //        LicensePlate = jobOrder.LicensePlate,
                        //        MaintenanceCenter = jobOrder.MaintenanceCenter,
                        //        MaintenanceCenterId = jobOrder.MaintenanceCenterId,
                        //        ServiceCenterId = d.ServiceCenterId,
                        //        Num = jobOrder.Num,
                        //        Odometer = jobOrder.Odometer,
                        //        OilLubricantCost = jobOrder.OilLubricantCost,
                        //        OperatorName = jobOrder.OperatorName,
                        //        OrgBranchName = jobOrder.OrgBranchName,
                        //        OtherCost = jobOrder.OtherCost,
                        //        Progress = jobOrder.Progress,
                        //        PercentCompleted = jobOrder.PercentCompleted,
                        //        ProjectName = jobOrder.ProjectName,
                        //        Referance = jobOrder.Referance,
                        //        RejectionReason = jobOrder.RejectionReason,
                        //        SalvageCost = jobOrder.SalvageCost,
                        //        ServiceCenterStatus = jobOrder.ServiceCenterStatus,
                        //        WorkShopRejectedReason = jobOrder.WorkShopRejectedReason,
                        //        Type = jobOrder.Type,
                        //        TyreCost = jobOrder.TyreCost,
                        //        Site = jobOrder.Site,
                        //        Status = jobOrder.Status,
                        //        SpareCost = jobOrder.SpareCost,
                        //        IsSentToWorkShop = jobOrder.IsSentToWorkShop,
                        //        IsSubmittedToServiceCenter = jobOrder.IsSubmittedToServiceCenter,
                        //        IsSecondOnlineApproved = jobOrder.IsSecondOnlineApproved,
                        //        IsSecondSendForApproval = jobOrder.IsSecondSendForApproval,
                        //        IsSendForApproval = jobOrder.IsSendForApproval,
                        //        IsVoid = jobOrder.IsVoid,
                        //        Remark = jobOrder.Remark,
                        //        IsExternalMaintenanceCenter = true,
                        //        IsInsured = jobOrder.IsInsured
                        //    };

                        //    db.ServiceCenterJob.Add(jobOrderServiceCenter);
                        //    db.SaveChanges();

                        //    var jobOrderServiceCenterDetail = new JobOrderServiceCenterDetail
                        //    {
                        //        JobOrderServiceCenterDetailId = Guid.NewGuid(),
                        //        ServiceCenterJobId = jobOrderServiceCenter.ServiceCenterJobId,
                        //        SubCategoryCode = d.SubCategoryCode,
                        //        JobCausesId = d.JobCausesId,
                        //        ServiceGroup = d.ServiceGroup,
                        //        RepairTypeID = d.RepairTypeID,
                        //        ServiceCenterId = d.ServiceCenterId,
                        //        Status = d.Status,
                        //        ApprovalStatus = d.ApprovalStatus,
                        //        Remark = d.Remark,
                        //        ServiceID = d.ServiceID,
                        //        ModifiedBy = d.ModifiedBy,
                        //        DateCreated = d.DateCreated,
                        //        JobOrderDetailId = d.JobOrderDetailID

                        //    };
                        //    db.JobOrderServiceCenterDetails.Add(jobOrderServiceCenterDetail);
                        //    db.SaveChanges();
                        //}


                        #endregion
                    }
                    else
                    {
                        #region create service center


                        var jobOrderDetails = db.JobOrderDetail.Where(x => x.JobOrderID == jobOrder.JobOrderID).ToList();
                        foreach (var d in jobOrderDetails)
                        {
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


                        #endregion
                    }

                }
                else if (technicalApprovalViewModel.ApproveStatus == ApprovalStatuses.Reject && jobOrder != null)
                {

                    jobOrder.JobOrderID = technicalApprovalViewModel.JobOrderID;
                    jobOrder.MaintenanceCenter = technicalApprovalViewModel.MaintenanceCenter;
                    jobOrder.MaintenanceCenterId = technicalApprovalViewModel.MaintenanceCenterId;
                    jobOrder.OnlineApprovedBy = User.Identity.Name;
                    jobOrder.OnlineApprovedTime = DateTime.Now;
                    jobOrder.IsOnlineApproved = false;
                    jobOrder.Status = MaintenanceConst.Pending;
                    jobOrder.IsSendForApproval = false;

                    db.JobOrder.Attach(jobOrder);
                    db.Entry(jobOrder).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }






            }


            catch (Exception e)
            {

            }
            return Json(new[] { technicalApprovalViewModel }.ToDataSourceResult(request));
        }

        public JsonResult AfterApprovalAddJobOrderDetail(Guid jobOrderId)
        {
            var returnMessage = new
            {
                SuccessMessage = "Successfully Added!",
                ErrorMessage = "Failed to add"

            };
            var jobOrder = db.JobOrder.AsNoTracking().FirstOrDefault(x => x.JobOrderID == jobOrderId);
            if (jobOrder != null)
            {
                var jobOrderDetail = db.JobOrderDetail.AsNoTracking().Where(x => x.JobOrderID == jobOrder.JobOrderID).ToList();
                foreach (var d in jobOrderDetail)
                {
                    var serviceCenterJob = db.ServiceCenterJob.AsNoTracking()
                        .FirstOrDefault(x => x.ServiceCenterId == d.ServiceCenterId && x.JobOrderID == d.JobOrderID);
                    var serviceDetail = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.JobOrderDetailId == d.JobOrderDetailID);
                    if (serviceCenterJob == null) 
                    {
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
                    else if(serviceDetail == null)
                    {
                        var jobOrderServiceCenterDetail = new JobOrderServiceCenterDetail
                        {
                            JobOrderServiceCenterDetailId = Guid.NewGuid(),
                            ServiceCenterJobId = serviceCenterJob.ServiceCenterJobId,
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

                
                return Json(returnMessage.SuccessMessage, JsonRequestBehavior.AllowGet);

            }
            
            return Json(returnMessage.ErrorMessage, JsonRequestBehavior.AllowGet);
        }

    }


}