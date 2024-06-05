using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class EquipmentReportController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        // GET: FleetManagement/EquipmentReport
        public ActionResult GetEquipmentReport()
        {
            return View();
        }
    }
}