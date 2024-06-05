using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
      //[AuthorizeRoles(FleetManagementRoles.MaintenanceServiceApprover)] 
    public class WorkShopController : Controller
    {
          ExceedDbContext db = new ExceedDbContext();

        //
        // GET: /FleetManagement/WorkShop/
        public ActionResult Index()
        {
            ViewData["Mechanic"] = db.MechanicCrew.ToList();
            ViewData["JobOrderDetail"] = db.JobOrderDetail.ToList();
            ViewBag.FailureReason = db.FailureReasons.ToList();
            return View();
        }
	}
}