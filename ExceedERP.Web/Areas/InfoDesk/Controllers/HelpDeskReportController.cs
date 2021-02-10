using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    public class HelpDeskReportController : Controller
    {
        // GET: InfoDesk/HelpDeskReport
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MaintenanceForm()
        {
            return View();
        }
    }
}