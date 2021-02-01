using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Web.Areas.FleetManagement.Models;
using Kendo.Mvc.UI;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
 using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
namespace ExceedERP.Web.Areas.FleetManagement
{

    public class AdministrativeJobApprovalController : Controller
    {

        IJobOrderService _JobOrderService;
        IFleetService _FleetService;
        IAdministrativeApprovalService _AdministrativeApprovalService;
        public const string Finished = "Finished";
        public const string Pending = "Pending";
        public const string Active = "Active";
        public AdministrativeJobApprovalController()
        {
            _JobOrderService = new JobOrderService();
            _FleetService = new FleetService();
            _AdministrativeApprovalService = new AdministrativeJobApprovalService();
        }

        //
        // GET: /FleetManagement/AdministrativeJobApproval/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Adminstrative_Read([DataSourceRequest] DataSourceRequest request, string AdminID)
        {
            var JobOrder = _AdministrativeApprovalService.GetAll().Where(x => x.AdministrativeJobId.ToString() == AdminID).Select(j => new AdministrativeJobApproval
            {
                Status = j.Status,
                Remark = j.Remark

            });

            return Json(JobOrder.ToDataSourceResult(request));
        }
         [AuthorizeRoles(FleetManagementRoles.MaintenanceRequestApprover)] 
        public ActionResult Administrative_Create([DataSourceRequest] DataSourceRequest request, AdministrativeJobApproval administrativeJobApproval, string JobID)
        {



            if (administrativeJobApproval != null)
            {
                administrativeJobApproval.AdministrativeJobId = Guid.Parse(JobID);
                _AdministrativeApprovalService.Insert(administrativeJobApproval);

            }

            return Json(new[] { administrativeJobApproval }.ToDataSourceResult(request));
        }


        public ActionResult Administrative_Update([DataSourceRequest] DataSourceRequest request, AdministrativeJobApproval administrativeJobApproval)
        {



            if (administrativeJobApproval != null)
            {

                _AdministrativeApprovalService.Update(administrativeJobApproval);

            }

            return Json(new[] { administrativeJobApproval }.ToDataSourceResult(request));
        }

           [AuthorizeRoles(FleetManagementRoles.MaintenanceRequestApprover)] 
        public ActionResult Admin_Approval([DataSourceRequest] DataSourceRequest request, TechnicalApprovalViewModel jobOrder)
        {
            try
            {
                ExceedDbContext db = new ExceedDbContext();
                 
 
                 var JobOrder = db.JobOrder.AsNoTracking().ToList().Where(x => x.JobOrderID == jobOrder.JobOrderID).FirstOrDefault();
                JobOrder.Status = MaintenanceConst.WaitingTechnical;
                JobOrder.OnlineApprovedBy = User.Identity.Name;
                JobOrder.OnlineApprovedTime = DateTime.Now;
                JobOrder.IsOnlineApproved = true;
                JobOrder.SecondSendForApprovalBy = User.Identity.Name;
                JobOrder.SecondSendForApprovalTime = DateTime.Now;
                JobOrder.IsSecondSendForApproval = true;
                AdministrativeJobApproval approval = new AdministrativeJobApproval();
                approval.AdministrativeJobId = jobOrder.JobOrderID;
                approval.Status = jobOrder.ApproveStatus;
                approval.Remark = jobOrder.Remark;

                if (JobOrder != null)
                {
                    _AdministrativeApprovalService.Insert(approval);
                    if (jobOrder.ApproveStatus == ApprovalStatuses.Approve)
                    {
                        _JobOrderService.Update(JobOrder);
                    }
                }
            }
            catch(Exception){}
            return Json(new[] { jobOrder }.ToDataSourceResult(request));
        }
    }
}