using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class MaintenanceHomeController : Controller
    {
        // GET: Maintenance/MaintenanceHome
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}