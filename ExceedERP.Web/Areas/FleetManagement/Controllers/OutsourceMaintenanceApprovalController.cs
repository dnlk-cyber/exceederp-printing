using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;                
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.FleetManagement.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
     [AuthorizeRoles(FleetManagementRoles.MaintenanceRequestor, FleetManagementRoles.MaintenanceRequestApprover)]
      
    public class OutsourceMaintenanceApprovalController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();


         public ActionResult OutsourceMaintenanceApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<OutsourceMaintenanceApproval> fleetApproval = db.OutsourceMaintenanceApproval.Where(x => x.JobOrderId == Id).ToList();
            List<JobOrder> Category = db.JobOrder.Where(x => x.JobOrderID == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSecondSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineTransferred == false)
            {
                fleetApproval.Add(new OutsourceMaintenanceApproval()
                {
                    JobOrderId = Id,
                    Remark = string.Empty,
                   Status = ApprovalStatuses.Pending,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new OutsourceMaintenanceApproval()
            {
                OutsourceMaintenanceApprovalId = approval.OutsourceMaintenanceApprovalId,
                JobOrderId = approval.JobOrderId,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        [AuthorizeRoles(FleetManagementRoles.MaintenanceRequestor)]

        public ActionResult OutsourceMaintenance_SendForOnlineApproval(string EId)
        {
            IJobOrderService _JobOrderService = new JobOrderService();
            Guid Id = Guid.Parse(EId);
            var JobOrder = db.JobOrder.Where(x => x.JobOrderID == Id && x.IsExternal == true).FirstOrDefault();
            if (JobOrder != null)
            {
                JobOrder.IsSecondSendForApproval = true;
                JobOrder.SecondSendForApprovalTime = DateTime.Now;
                JobOrder.SecondSendForApprovalBy = User.Identity.Name;

                _JobOrderService.Update(JobOrder);
            }


            return RedirectToActionPermanent("Index", "OutsourceMaintenance", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.MaintenanceRequestor)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OutsourceMaintenance_Approve([DataSourceRequest]DataSourceRequest request, OutsourceMaintenanceApproval OutsourceMaintenanceApproval, Guid Id)
        {
            try
            {
                IJobOrderService _JobOrderService = new JobOrderService();

               
                if (OutsourceMaintenanceApproval.Status == ApprovalStatuses.Approve)
                {
                    var JobOrder = db.JobOrder.Where(x => x.JobOrderID == Id && x.IsExternal == true).FirstOrDefault();
                    if (JobOrder != null)
                    {
                        JobOrder.IsOnlineTransferred = true;
                        JobOrder.OnlineTransferredTime = DateTime.Now;
                        JobOrder.IsOnlineTransferredBy = User.Identity.Name;
                        JobOrder.Status = MaintenanceConst.SentForClosing;
                        _JobOrderService.Update(JobOrder);
                    }

                }

                var approval = new OutsourceMaintenanceApproval()
                {

                    Status = OutsourceMaintenanceApproval.Status,
                    JobOrderId = Id,
                    Remark = OutsourceMaintenanceApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.OutsourceMaintenanceApproval.Add(approval);
                db.SaveChanges();
                OutsourceMaintenanceApproval.OutsourceMaintenanceApprovalId = approval.OutsourceMaintenanceApprovalId;

            }
            catch (Exception e)
            {

            
            }
            return Json(new[] { OutsourceMaintenanceApproval }.ToDataSourceResult(request));
        }




    }
}
