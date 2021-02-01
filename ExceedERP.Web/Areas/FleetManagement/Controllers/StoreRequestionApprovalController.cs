using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
      [AuthorizeRoles(FleetManagementRoles.MaintenanceServiceApprover, FleetManagementRoles.ServiceUser)]
    public class StoreRequestionApprovalController : Controller
    {
          ExceedDbContext db = new ExceedDbContext();

        //
        // GET: /FleetManagement/WorkShop/
        public ActionResult Index()
        {
            ViewData["Mechanic"] = db.MechanicCrew.ToList();
            return View();
        }

        public JsonResult ServiceSpareByService( [DataSourceRequest] DataSourceRequest request, string Id)
        {
            IServiceSpareService _ServiceSpareService = new ServiceSpareService();
            IJobOrderDetailService _JobOrderDetailService = new JobOrderDetailService();
            var Services = new List<ServiceSpare>();
            var Jobs = _JobOrderDetailService.GetAll().Where(x => x.JobOrderID.ToString() == Id).ToList();
            foreach(var Job in Jobs)
            {
               var Service = _ServiceSpareService.GetAll().Where(s => s.ServiceID == Job.JobOrderID).ToList();
                Services.AddRange(Service);
            }
            return Json(Services.Select(f => new { ServiceSpareID = f.ServiceSpareID,Type = f.Type, ServiceID = f.ServiceID, InventoryItemId = f.InventoryItemId, ServiceName = f.ServiceName, Quantity = f.Quantity, Date = f.Date, ApprovalStatus = f.ApprovalStatus, Remark = f.Remark, CreatedBy = f.CreatedBy, DateCreated = f.DateCreated }).ToDataSourceResult(request));
        }
	}
}