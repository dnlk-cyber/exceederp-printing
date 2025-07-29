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
    [AuthorizeRoles(MaintenanceRoles.MaintenanceOrderUser, MaintenanceRoles.ServiceUser)]
    public class ServiceJobOrderDetailController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        IServiceJobOrderDetailService _ServiceJobOrderDetailService;
        IServiceCenterJobService _ServiceJobOrderService;
        IFleetService _FleetService;
        Guid joID = Guid.Empty;

        public ServiceJobOrderDetailController()
        {
            _ServiceJobOrderDetailService = new ServiceJobOrderDetailService();
            _FleetService = new FleetService();
            _ServiceJobOrderService = new ServiceCenterJobService();
        }



        public ActionResult GenrateStoreRequistion([DataSourceRequest] DataSourceRequest request, SelectStoreViewModel model, Guid Id)
        {

            //var JobService = _JobOrderService.GetById(Id);


            //if (JobService != null)
            //{
            //    var Job = _JobOrderService.GetById(JobService.JobOrderID);
            //    var Branch = db.Branch.Where(x => x.Name == Job.ProjectName).FirstOrDefault();

            //    //get branch for requistion
            //    // if(Branch ! = null)
            //    //  {

            //    //  }
            //    List<StoreRequisitionItem> StoreRequisition = new List<StoreRequisitionItem>();
            //    var Requestions = db.ServiceSpare.Where(x => x.ServiceID == Id).ToList();


            //    StoreRequisition Request = new StoreRequisition
            //    {
            //        RequestDate = DateTime.Now,
            //        StoreCode = model.Name,
            //        ToStoreCode = model.Remark,
            //        //      BranchId = 
            //    };

            //    db.StoreRequisitions.Add(Request);
            //    db.SaveChanges();

            //    foreach (var Requestion in Requestions)
            //    {

            //        var SpareRequest = new StoreRequisitionItem
            //        {

            //            RequestedQuantity = Requestion.Quantity,
            //            PartNo = Requestion.InventoryItems.PartNo,
            //            ItemSpecification = Requestion.InventoryItems.Name,
            //            GeneratePurchaseReq = true,
            //            DateCreated = DateTime.Now,
            //            CreatedBy = User.Identity.Name,
            //            StoreRequisitionID = Request.StoreRequisitionID
            //        };
            //        db.StoreRequisitionItems.Add(SpareRequest);
            //        db.SaveChanges();

            //        var JobStoreRequistion = new JobStoreRequisition
            //        {
            //            JobStoreRequisitionId = Guid.NewGuid(),
            //            JobId = Requestion.ServiceSpareID,
            //            RequisitionId = SpareRequest.StoreRequisitionItemID,
            //            CreatedBy = User.Identity.Name,
            //            DateCreated = DateTime.Now,
            //            Type = "SPAREPART"

            //        };

            //    }
            //}



            return RedirectToAction("Index", "serviceJobOrder");
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
            _ServiceJobOrderService = new ServiceCenterJobService();
            _ServiceJobOrderDetailService = new ServiceJobOrderDetailService();

            var jobrequest = db.JobRequest.Where(x => x.JobRequestId.ToString() == jobOrderID).FirstOrDefault();

            return Json(_ServiceJobOrderDetailService.GetAll().Where(jobOrderDetail => jobOrderDetail.ServiceCenterJobId.ToString() == jobOrderID).Select(j => new JobOrderServiceCenterDetail
            {

                ServiceID = j.ServiceID,
                ServiceCenterJobId = j.ServiceCenterJobId,
                Status = j.Status,
                JobOrderServiceCenterDetailId = j.JobOrderServiceCenterDetailId,
                StartingDate = j.StartingDate,
                ToDate = j.ToDate,
                ApprovalStatus = j.ApprovalStatus,
                RepairTypeID = j.RepairTypeID

            }
            ).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        public JsonResult SingleJobOrder(string jobOrderID, [DataSourceRequest] DataSourceRequest request)
        {
            _ServiceJobOrderService = new ServiceCenterJobService();
            _ServiceJobOrderDetailService = new ServiceJobOrderDetailService();

            var JobOrder = _ServiceJobOrderService.GetById(Guid.Parse(jobOrderID));
            var jobrequest = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();
            if (jobrequest != null)
            {
                return Json(_ServiceJobOrderDetailService.GetAll().Where(jobOrderDetail => jobOrderDetail.ServiceCenterJobId.ToString() == jobOrderID.ToString()).Select(j => new JobOrderServiceCenterDetail
                {

                    ServiceID = j.ServiceID,
                    ServiceCenterJobId = j.ServiceCenterJobId,
                    Status = j.Status,
                    JobOrderServiceCenterDetailId = j.JobOrderServiceCenterDetailId,
                    StartingDate = j.StartingDate,
                    ToDate = j.ToDate,
                    ApprovalStatus = j.ApprovalStatus,
                    //ServiceName = j.Service.ServiceName
                    RepairTypeID = j.RepairTypeID

                }
                ).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            return Json(_ServiceJobOrderDetailService.GetAll().Where(jobOrderDetail => jobOrderDetail.ServiceCenterJobId.ToString() == jobOrderID.ToString()).Select(j => new JobOrderServiceCenterDetail
            {

                ServiceID = j.ServiceID,
                ServiceCenterJobId = j.ServiceCenterJobId,
                Status = j.Status,
                JobOrderServiceCenterDetailId = j.JobOrderServiceCenterDetailId,
                StartingDate = j.StartingDate,
                ToDate = j.ToDate,
                ApprovalStatus = j.ApprovalStatus,
                // ServiceName = j.Service.ServiceName
                RepairTypeID = j.RepairTypeID

            }
               ).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult OrderService(string jobOrderID, [DataSourceRequest] DataSourceRequest request)
        {

            _ServiceJobOrderDetailService = new ServiceJobOrderDetailService();

            var service = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == jobOrderID);
            if (service == null)
            {
                return Json(new { error = "ServiceCenterJob not found for the given Job Order ID" });
            }

            var order = db.JobOrder.FirstOrDefault(x => x.JobOrderID == service.JobOrderID);
            if (order == null)
            {
                return Json(new { error = "JobOrder not found for the given ServiceCenterJob" });
            }

            var requestData = db.JobRequest.FirstOrDefault(x => x.Num == order.Referance);
            if (requestData == null)
            {
                return Json(new { error = "JobRequest not found for the given reference" });
            }
            var serviceOrderDetails = _ServiceJobOrderDetailService.GetAll().Where(jobOrderDetail => jobOrderDetail.ServiceCenterJobId == service.ServiceCenterJobId).ToList();


            return Json(serviceOrderDetails.Select(j => new JobOrderServiceCenterDetail
            {
                JobOrderServiceCenterDetailId = j.JobOrderServiceCenterDetailId,
                ServiceID = j.ServiceID,
                ServiceCenterJobId = j.ServiceCenterJobId,
                JobCausesId = j.JobCausesId,
                Status = j.Status,
                Cause = j.JobCauses.Description,
                ServiceGroup = j.ServiceGroup,
                SubCategoryCode = j.SubCategoryCode,
                StartingDate = j.StartingDate,
                ToDate = j.ToDate,
                ApprovalStatus = j.ApprovalStatus,
                DateCreated = j.DateCreated,
                OnlineApprovedTime = j.OnlineApprovedTime,
                //Section = GetSection(j.Service.SectionId),
                //ServiceName = j.Service.RepairType.Code + " - "+ j.Service.ServiceName,
                RepairTypeID = j.RepairTypeID,
                IsOnlineTransferred = j.IsOnlineTransferred,
                Remark = j.Remark,
                ServiceCenterId = j.ServiceCenterId,


            }
            ).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
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

            _ServiceJobOrderDetailService = new ServiceJobOrderDetailService();
            var service = db.ServiceCenterJob.Where(x => x.ServiceCenterJobId.ToString() == jobOrderID).FirstOrDefault();
            var order = db.JobOrder.Where(x => x.JobOrderID == service.JobOrderID).FirstOrDefault();
            var Request = db.JobRequest.Where(x => x.Num == order.Referance).FirstOrDefault();


            return Json(_ServiceJobOrderDetailService.GetAll().Where(jobOrderDetail => jobOrderDetail.ServiceCenterJobId == service.JobOrderID).Select(j => new JobOrderServiceCenterDetail
            {

                ServiceID = j.ServiceID,
                ServiceCenterJobId = j.ServiceCenterJobId,
                JobCausesId = j.JobCausesId,
                Status = j.Status,
                Cause = j.JobCauses.Description,
                JobOrderServiceCenterDetailId = j.JobOrderServiceCenterDetailId,
                ServiceGroup = j.ServiceGroup,
                SubCategoryCode = j.SubCategoryCode,
                StartingDate = j.StartingDate,
                ToDate = j.ToDate,
                ApprovalStatus = j.ApprovalStatus,
                DateCreated = j.DateCreated,
                OnlineApprovedTime = j.OnlineApprovedTime,
                //Section = GetSection(j.Service.SectionId),
                //ServiceName = j.Service.RepairType.Code + " - "+ j.Service.ServiceName,
                RepairTypeID = j.RepairTypeID,
                IsOnlineTransferred = j.IsOnlineTransferred,
                Remark = j.Remark,
                ServiceCenterId = j.ServiceCenterId,

            }
            ).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        //public ActionResult AdminJobOrderDetail_Create(string jobDetailOrderID, [DataSourceRequest] DataSourceRequest request, JobOrderServiceCenterDetail JobOrderDetail)
        //{
        //    ModelState.Remove("ServiceID");
        //    ModelState.Remove("JobOrderDetailID");
        //    ModelState.Remove("JobOrderID");
        //    ModelState.Remove("ApprovalStatus");
        //    IServiceService mservice = new ServiceService();

        //    _ServiceJobOrderDetailService = new ServiceJobOrderDetailService();

        //    List<JobOrderServiceCenterDetail> JobOrderDetails = _ServiceJobOrderDetailService.GetAll().Where(f => f.RepairTypeID == JobOrderDetail.RepairTypeID && f.JobOrderID == Guid.Parse(jobDetailOrderID)).ToList();
        //    if (JobOrderDetails.Any())
        //    {
        //        ModelState.Clear();
        //        ModelState.AddModelError("ExistingService", " Service Already Exists ! ");
        //    }
        //    else if (JobOrderDetail.StartingDate > JobOrderDetail.ToDate)
        //    {
        //        ModelState.Clear();
        //        ModelState.AddModelError("ExistingService", " Start Date Is Above End Date ! ");

        //    }
        //    else
        //    {

        //        JobOrderDetail.JobOrderDetailID = Guid.NewGuid();
        //        JobOrderDetail.ServiceID =
        //            mservice.GetAll()
        //                .Where(s => s.ServiceName == JobOrderDetail.RepairTypeID.ToString())
        //                .Select(x => x.ServiceID)
        //                .FirstOrDefault();
        //        JobOrderDetail.JobOrderID = Guid.Parse(jobDetailOrderID);

        //        if (JobOrderDetail != null)
        //        {
        //            JobOrderDetail.ModifiedBy = User.Identity.Name;
        //            JobOrderDetail.LastModified = DateTime.Now;
        //            JobOrderDetail.Status = MaintenanceConst.Pending;
        //            JobOrderDetail.ApprovalStatus = ApprovalStatuses.Pending;
        //            _ServiceJobOrderDetailService.Insert(JobOrderDetail);

        //            IServiceService _ServiceService = new ServiceService();
        //            ServiceCrewService _ServiceCrewService = new ServiceCrewService();
        //            List<ServiceCrew> JobCrew = new List<ServiceCrew>();

        //            var Service = _ServiceService.GetById(JobOrderDetail.ServiceID);

        //            IMechanicCrewService _MechanicCrewService = new MechanicCrewService();

        //            var Crews = _MechanicCrewService.GetAll()
        //                .Where(s => s.SectionId == Service.SectionId)
        //                .Select(
        //                    f =>
        //                        new ServiceCrew
        //                        {
        //                            ServiceCrewID = Guid.NewGuid(),
        //                            ServiceID = JobOrderDetail.JobOrderDetailID,
        //                            MechanicCrewID = f.MechanicCrewID,
        //                            ServiceName = f.FullName,
        //                            DateCreated = f.DateHired,
        //                            HourlyWage = f.HourlyWage
        //                        }).ToList();
        //            JobCrew.AddRange(Crews);
        //            _ServiceCrewService.InsertBatch(JobCrew.DistinctByField(x => x.MechanicCrewID).ToList());

        //        }

        //    }
        //    return Json(new[] { JobOrderDetail }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        //}


        public ActionResult JobOrderDetail_Create(string jobDetailOrderID, [DataSourceRequest] DataSourceRequest request, JobOrderServiceCenterDetail JobOrderDetail)
        {
            ModelState.Remove("ServiceID");
            ModelState.Remove("JobOrderDetailID");
            ModelState.Remove("JobOrderID");

            IServiceService mservice = new ServiceService();
            _ServiceJobOrderService = new ServiceCenterJobService();
            _ServiceJobOrderDetailService = new ServiceJobOrderDetailService();

            var serviceJobOrder = _ServiceJobOrderService.GetAll().Where(x => x.ServiceCenterJobId.ToString() == jobDetailOrderID).FirstOrDefault();

            if (serviceJobOrder != null)
            {
                var JobRequest = db.JobRequest.Where(x => x.Num == serviceJobOrder.Referance).FirstOrDefault();

                //List<JobOrderDetail> JobOrderDetails = _JobOrderDetailService.GetAll().Where(f => f.RepairTypeID == JobOrderDetail.RepairTypeID && f.JobOrderID == JobOrder.JobOrderID).ToList();
                //if (JobOrderDetails.Any())
                //{
                //    ModelState.Clear();
                //    ModelState.AddModelError("ExistingService", " Service Already Exists ! ");
                //    return Json(new[] { JobOrderDetail }.ToDataSourceResult(request, ModelState));
                //}

                if (JobRequest != null)
                {
                    var serviceJobDetails = db.JobOrderServiceCenterDetails.Where(x => x.RepairTypeID == JobOrderDetail.RepairTypeID && x.JobCausesId == JobOrderDetail.JobCausesId
                                                                            && x.ServiceGroup == JobOrderDetail.ServiceGroup
                                                                            && x.ServiceCenterJobId.ToString() == jobDetailOrderID)
                                                                            .ToList();
                    //var serviceJobDetails = _ServiceJobOrderDetailService.GetAll().Where(f => f.RepairTypeID == JobOrderDetail.RepairTypeID
                    //                                                        && f.JobCausesId == JobOrderDetail.JobCausesId
                    //                                                        && f.ServiceGroup == JobOrderDetail.ServiceGroup
                    //                                                        && f.ServiceCenterJobId.ToString() == jobDetailOrderID)
                    //                                                        .ToList();
                    if (serviceJobDetails.Any())
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

            //JobOrderDetail.JobOrderDetailID = Guid.NewGuid();
            JobOrderDetail.ServiceID = JobOrderDetail.ServiceID;
            //mservice.GetAll()
            //    .Where(s => s.ServiceName == JobOrderDetail.ServiceName)
            //    .Select(x => x.ServiceID)
            //    .FirstOrDefault();
            JobOrderDetail.ServiceCenterJobId = Guid.Parse(jobDetailOrderID);
            JobOrderDetail.JobOrderServiceCenterDetailId = Guid.NewGuid();

            if (JobOrderDetail != null)
            {
                JobOrderDetail.ModifiedBy = User.Identity.Name;
                JobOrderDetail.DateCreated = DateTime.Now;

                //    JobOrderDetail.OnlineApprovedTime = DateTime.Now;
                JobOrderDetail.Status = MaintenanceConst.Pending;
                JobOrderDetail.ApprovalStatus = ApprovalStatuses.Pending;
                _ServiceJobOrderDetailService.Insert(JobOrderDetail);
            }

            return Json(new[] { JobOrderDetail }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderDetail_Update([DataSourceRequest] DataSourceRequest request, JobOrderServiceCenterDetail JobOrderDetail)
        {
            IServiceService mservice = new ServiceService();

            _ServiceJobOrderDetailService = new ServiceJobOrderDetailService();

            List<JobOrderServiceCenterDetail> JobOrderDetails = _ServiceJobOrderDetailService.GetAll().Where(f => f.RepairTypeID == JobOrderDetail.RepairTypeID && f.JobCausesId == JobOrderDetail.JobCausesId && f.JobOrderServiceCenterDetailId != JobOrderDetail.JobOrderServiceCenterDetailId).ToList();
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
                    _ServiceJobOrderDetailService = new ServiceJobOrderDetailService();

                    _ServiceJobOrderDetailService.Update(JobOrderDetail);
                }
            }

            return Json(new[] { JobOrderDetail }.ToDataSourceResult(request));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderDetail_Approve([DataSourceRequest] DataSourceRequest request, JobOrderServiceCenterDetail JobOrderDetail)
        {
            if (JobOrderDetail != null)
            {
                if (JobOrderDetail.ApprovalStatus == ApprovalStatuses.Approve)
                {
                    JobOrderDetail.IsOnlineApproved = true;
                    JobOrderDetail.OnlineApprovedTime = DateTime.Now;
                    JobOrderDetail.OnlineApprovedBy = User.Identity.Name;
                    db.JobOrderServiceCenterDetails.Attach(JobOrderDetail);
                    db.Entry(JobOrderDetail).State = EntityState.Modified;
                    db.SaveChanges();
                }


            }

            return Json(new[] { JobOrderDetail }.ToDataSourceResult(request));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderDetail_Destroy([DataSourceRequest] DataSourceRequest request, JobOrderServiceCenterDetail jobOrderDetail)
        {
            var JobOrderDetail = _ServiceJobOrderDetailService.GetById(jobOrderDetail.JobOrderServiceCenterDetailId);
            if (JobOrderDetail != null)
            {
                _ServiceJobOrderDetailService.Delete(JobOrderDetail);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Job Detail number " + jobOrderDetail.JobOrderServiceCenterDetailId);

            }

            return Json(new[] { JobOrderDetail }.ToDataSourceResult(request));
        }

        
        public ActionResult JobDetailDDBind(string Id)
        {
            var Service = _ServiceJobOrderDetailService.GetAll().Where(x => x.ServiceCenterJobId.ToString() == Id).Select(s => new Service
            {
                // ServiceName = s.Service.RepairType.Name + " - " + s.Service.RepairType.Code,
                ServiceName = s.RepairTypeID.ToString(),
                ServiceID = s.JobOrderServiceCenterDetailId
            }).ToList();

            return Json(Service, JsonRequestBehavior.AllowGet);
        }


    }
}

