using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class FleetHomeController : Controller
    {
        // GET: FleetManagement/FleetHome
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}