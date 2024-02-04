using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers.Estimation
{
    public class PrintingEstimationReportController : Controller
    {
        // GET: PrintingEstimation/PrintingReport
        public ActionResult PrintingEstimationForms(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ProformaPrint(int id=5)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult JobOrder(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult ChangeOfOrder(int id)
        {
            ViewBag.id = id;
            return View();
        }
    }
}