using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Controllers
{
    public class GlobalReportsController : Controller
    {
        // GET: GlobalRepors
        public ActionResult UserGroupsRoles()
        {
            return View();
        }
    }
}