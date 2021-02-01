using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using DocumentFormat.OpenXml.Drawing.Diagrams;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.MaintenanceOrderUser)]
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

            var JobService = _JobOrderService.GetById(Id);


            if (JobService != null)
            {
                var Job = _JobOrderService.GetById(JobService.JobOrderID);
                var Branch = db.Branch.Where(x => x.Name == Job.ProjectName).FirstOrDefault();

                //get branch for requistion
                // if(Branch ! = null)
                //  {

                //  }
                List<StoreRequisitionItem> StoreRequisition = new List<StoreRequisitionItem>();
                var Requestions = db.ServiceSpare.Where(x => x.ServiceID == Id).ToList();


                StoreRequisition Request = new StoreRequisition
                {
                    RequestDate = DateTime.Now,
                    StoreCode = model.Name,
                    ToStoreCode = model.Remark,
                    //      BranchId = 
                };

                db.StoreRequisitions.Add(Request);
                db.SaveChanges();

                foreach (var Requestion in Requestions)
                {

                    var SpareRequest = new StoreRequisitionItem
                    {

                        RequestedQuantity = Requestion.Quantity,
                        PartNo = Requestion.InventoryItems.PartNo,
                        ItemSpecification = Requestion.InventoryItems.Name,
                        GeneratePurchaseReq = true,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        StoreRequisitionID = Request.StoreRequisitionID
                    };
                    db.StoreRequisitionItems.Add(SpareRequest);
                    db.SaveChanges();

                    var JobStoreRequistion = new JobStoreRequisition
                    {
                        JobStoreRequisitionId = Guid.NewGuid(),
                        JobId = Requestion.ServiceSpareID,
                        RequisitionId = SpareRequest.StoreRequisitionItemID,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Type = "SPAREPART"

                    };

                }
            }



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

            return Json(_JobOrderDetailService.GetAll().Where(jobOrderDetail => jobOrderDetail.JobOrderID == jobrequest.JobRequestId).Select(j => new JobOrderDetail
                {

                    ServiceID = j.ServiceID,
                    JobOrderID = j.JobOrderID,
                    Status = j.Status,
                    JobOrderDetailID = j.JobOrderDetailID,
                    StartingDate = j.StartingDate,
                    ToDate = j.ToDate,
                    ApprovalStatus = j.ApprovalStatus,
                    ServiceName = j.Service.ServiceName

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
                return Json(_JobOrderDetailService.GetAll().Where(jobOrderDetail => jobOrderDetail.JobOrderID.ToString() == jobOrderID.ToString() || jobOrderDetail.JobOrderID == jobrequest.JobRequestId).Select(j => new JobOrderDetail
                    {

                        ServiceID = j.ServiceID,
                        JobOrderID = j.JobOrderID,
                        Status = j.Status,
                        JobOrderDetailID = j.JobOrderDetailID,
                        StartingDate = j.StartingDate,
                        ToDate = j.ToDate,
                        ApprovalStatus = j.ApprovalStatus,
                        ServiceName = j.Service.ServiceName

                    }
                ).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            return Json(_JobOrderDetailService.GetAll().Where(jobOrderDetail => jobOrderDetail.JobOrderID.ToString() == jobOrderID.ToString()).Select(j => new JobOrderDetail
               {

                   ServiceID = j.ServiceID,
                   JobOrderID = j.JobOrderID,
                   Status = j.Status,
                   JobOrderDetailID = j.JobOrderDetailID,
                   StartingDate = j.StartingDate,
                   ToDate = j.ToDate,
                   ApprovalStatus = j.ApprovalStatus,
                   ServiceName = j.Service.ServiceName

               }
               ).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult OrderService(string jobOrderID, [DataSourceRequest] DataSourceRequest request)
        {

            _JobOrderDetailService = new JobOrderDetailService();
            var Order = db.JobOrder.Where(x => x.JobOrderID.ToString() == jobOrderID).FirstOrDefault();
            var Request = db.JobRequest.Where(x => x.Num == Order.Referance).FirstOrDefault();


            return Json(_JobOrderDetailService.GetAll().Where(jobOrderDetail => jobOrderDetail.JobOrderID.ToString() == jobOrderID || jobOrderDetail.JobOrderID == Request.JobRequestId).Select(j => new JobOrderDetail
                {

                    ServiceID = j.ServiceID,
                    JobOrderID = j.JobOrderID,
                    JobCausesId = j.JobCausesId,
                    Status = j.Status,
                    Cause = j.JobCauses.Name,
                    JobOrderDetailID = j.JobOrderDetailID,
                    StartingDate = j.StartingDate,
                    ToDate = j.ToDate,
                    ApprovalStatus = j.ApprovalStatus,
                    DateCreated = j.DateCreated,
                    OnlineApprovedTime = j.OnlineApprovedTime,
                    Section = GetSection(j.Service.SectionId),
                    ServiceName = j.Service.RepairType.Code + " - "+ j.Service.ServiceName,
                    IsOnlineTransferred = Order.IsOnlineTransferred,
                    Remark = Order.Remark
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

            _JobOrderDetailService = new JobOrderDetailService();
            var Order = db.JobOrder.Where(x => x.JobOrderID.ToString() == jobOrderID).FirstOrDefault();
            var Request = db.JobRequest.Where(x => x.Num == Order.Referance).FirstOrDefault();


            return Json(_JobOrderDetailService.GetAll().Where(jobOrderDetail => jobOrderDetail.JobOrderID.ToString() == jobOrderID || jobOrderDetail.JobOrderID == Request.JobRequestId).Select(j => new JobOrderDetail
                {

                    ServiceID = j.ServiceID,
                    JobOrderID = j.JobOrderID,
                    Status = j.Status,
                    JobOrderDetailID = j.JobOrderDetailID,
                    StartingDate = j.StartingDate,
                    ToDate = j.ToDate,
                    ApprovalStatus = j.ApprovalStatus,
                    ServiceName = j.Service.ServiceName,
                    Section = GetSection(j.Service.SectionId),
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

            List<JobOrderDetail> JobOrderDetails = _JobOrderDetailService.GetAll().Where(f => f.ServiceName == JobOrderDetail.ServiceName && f.JobOrderID == Guid.Parse(jobDetailOrderID)).ToList();
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
                        .Where(s => s.ServiceName == JobOrderDetail.ServiceName)
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

                var JobOrder = _JobOrderService.GetAll().Where(x => x.JobOrderID.ToString() == jobDetailOrderID).FirstOrDefault();

                if (JobOrder != null)
                {
                    var JobRequest = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();

                    List<JobOrderDetail> JobOrderDetails = _JobOrderDetailService.GetAll().Where(f => f.ServiceName == JobOrderDetail.ServiceName && f.JobOrderID == JobOrder.JobOrderID).ToList();
                    if (JobOrderDetails.Any())
                    {
                        ModelState.Clear();
                        ModelState.AddModelError("ExistingService", " Service Already Exists ! ");
                        return Json(new[] { JobOrderDetail }.ToDataSourceResult(request, ModelState));
                    }

                    if (JobRequest != null)
                    {

                        List<JobOrderDetail> JobDetail = _JobOrderDetailService.GetAll().Where(f => f.ServiceName == JobOrderDetail.ServiceName && f.JobOrderID == JobRequest.JobRequestId).ToList();
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
                JobOrderDetail.ServiceID =
                    mservice.GetAll()
                        .Where(s => s.ServiceName == JobOrderDetail.ServiceName)
                        .Select(x => x.ServiceID)
                        .FirstOrDefault();
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
            IServiceService mservice = new ServiceService();

            _JobOrderDetailService = new JobOrderDetailService();

            List<JobOrderDetail> JobOrderDetails = _JobOrderDetailService.GetAll().Where(f => f.ServiceName == JobOrderDetail.ServiceName && f.JobOrderID == JobOrderDetail.JobOrderID && f.JobOrderDetailID != JobOrderDetail.JobOrderDetailID).ToList();
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
                }

                _JobOrderDetailService.Update(JobOrderDetail);

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

        [AllowAnonymous]
        public ActionResult JobDetailDDBind(string Id)
        {
            var Service = _JobOrderDetailService.GetAll().Where(x => x.JobOrderID.ToString() == Id).Select(s => new Service
       {
           ServiceName = s.Service.RepairType.Name + " - " + s.Service.RepairType.Code,
           ServiceID = s.JobOrderDetailID
       }).ToList();

            return Json(Service, JsonRequestBehavior.AllowGet);
        }


    }
}

