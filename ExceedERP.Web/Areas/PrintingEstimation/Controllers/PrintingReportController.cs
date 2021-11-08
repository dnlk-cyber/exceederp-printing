using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.PrintingEstimation.Controllers
{
    public class PrintingReportController : Controller
    {
        // GET: PrintingEstimation/PrintingReport
        public ActionResult PrintingEstimationForms(string id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}