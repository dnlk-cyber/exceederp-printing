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
        public ActionResult FullRegistrationSlip(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult QualificationCertificate( string regn, int ti, string name)
        {
            ViewBag.Id = ti;
            return View();
        }
        public ActionResult WeldersPerformanceQualification(string regn, int ti, string name)
        {
            ViewBag.Id = ti;
            return View();
        }
    }
}