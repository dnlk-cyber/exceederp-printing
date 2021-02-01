using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [Authorize]
    public class FleetPrintController : Controller
    {
        //
        // GET: /FleetManagement/FleetReport/




        public ActionResult SiteVehicleTransfer(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

        public ActionResult FuelPrint(string from, string to ,string plateno)
        {
            ViewBag.from = from;
            ViewBag.to = to;
            ViewBag.plateno = plateno;
            return View();
        }

        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

        public ActionResult EquipmentPrint(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
 

        [AuthorizeRoles(FleetManagementRoles.ResourceRequestor)]

        public ActionResult EquipmentDemand(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.ResourceDistributor)]

        public ActionResult EquipmentDistribution(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }


    }
}