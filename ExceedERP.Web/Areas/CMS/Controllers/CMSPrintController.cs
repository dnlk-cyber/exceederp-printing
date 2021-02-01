using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CMS.Controllers
{
    public class CMSPrintController : Controller
    {
        public ActionResult RegistrationSlip(int id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}