using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Web.Areas.FleetManagement.Models;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.FleetManagement
{
    //  [AuthorizeRoles(FleetManagementRoles.ServiceUser)] 
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
            _ServiceCrewService= new ServiceCrewService();
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
                Type = "",

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
                                Type = Request.IsExternal ? "EXTERNAL" : "INTERNAL",
                                MaintenanceCenter = Request.MaintenanceCenter

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
        public ActionResult Technical_Approval([DataSourceRequest] DataSourceRequest request, TechnicalApprovalViewModel jobOrder)
        {
            try
            {
                ExceedDbContext db = new ExceedDbContext();
                TechnicalJobApproval approval = new TechnicalJobApproval();
               var JobOrder = db.JobOrder.Where(x => x.JobOrderID == jobOrder.JobOrderID).FirstOrDefault();

                if (jobOrder.ApproveStatus == ApprovalStatuses.Approve && JobOrder != null)
                {
              

                    if (jobOrder.Type == "EXTERNAL")
                    {
                        JobOrder.IsExternal = true;
                    }

                    if (jobOrder.Type == "INTERNAL")
                    {
                        JobOrder.IsExternal = false;
                    }
                        var Jobs = db.JobOrder.Where(x => x.JobOrderID == jobOrder.JobOrderID).FirstOrDefault();
                        var JobRequest = db.JobRequest.Where(x => x.Num == Jobs.Referance).FirstOrDefault();

                        var JobServices =  new List<JobOrderDetail>();
                            
                        if(JobRequest != null)
                          JobServices = _JobOrderDetailService.GetAll()
                            .Where(x => x.JobOrderID == jobOrder.JobOrderID || x.JobOrderID == JobRequest.JobRequestId).ToList();
                  
                        else
                        {

                            JobServices = _JobOrderDetailService.GetAll()
                        .Where(x => x.JobOrderID == jobOrder.JobOrderID ).ToList();
                  
                        }
                        List<ServiceCrew> JobCrew = new List<ServiceCrew>();

                        foreach (var js in JobServices)
                        {
                            if (js != null)
                            {
                                var Service = _ServiceService.GetById(js.ServiceID);
                                IMechanicCrewService _MechanicCrewService = new MechanicCrewService();

                                var Crews = _MechanicCrewService.GetAll()
                                    .Where(s => s.SectionId == Service.SectionId)
                                    .Select(
                                        f =>
                                            new ServiceCrew
                                            {
                                                ServiceCrewID = Guid.NewGuid(),
                                                ServiceID = js.JobOrderDetailID,
                                                MechanicCrewID = f.MechanicCrewID,
                                                ServiceName = f.FullName,
                                                DateCreated = f.DateHired,
                                                HourlyWage = f.HourlyWage
                                            }).ToList();
                                JobCrew.AddRange(Crews);

                            }
                        }
                        _ServiceCrewService.InsertBatch(JobCrew.DistinctByField(x => x.MechanicCrewID).ToList());

                        JobOrder.MaintenanceCenter = jobOrder.MaintenanceCenter;
                        JobOrder.OnlineApprovedBy = User.Identity.Name;
                        JobOrder.OnlineApprovedTime = DateTime.Now;
                        JobOrder.IsOnlineApproved = true;
                        JobOrder.Status = MaintenanceConst.Open;

                        db.JobOrder.Attach(JobOrder);
                        db.Entry(JobOrder).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        var Fleet = db.Fleets.Where(x=>x.FleetsID == JobOrder.FleetsID).FirstOrDefault();
                        if (Fleet != null)
                        {
                            Fleet.Status = MaintenanceConst.OnMaintenance;

                            db.Fleets.Attach(Fleet);
                            db.Entry(Fleet).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
               


                if (JobOrder != null)
                {
             

                    approval = new TechnicalJobApproval();

                    approval.TechnicalJobId = JobOrder.JobOrderID;
                    approval.Status = jobOrder.ApproveStatus;
                    approval.Remark = jobOrder.Remark;
                    approval.CreatedBy = User.Identity.Name;
                    approval.DateCreated = DateTime.Now;

                    _TechnicalJobApprovalService.Insert(approval);
                }


         
            }
            catch (Exception e)
            {
                
            }
            return Json(new[] { jobOrder }.ToDataSourceResult(request));
        }


    }


}