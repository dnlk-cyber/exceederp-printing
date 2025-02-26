using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingReportController : Controller
    {
        // GET: Printing/PrintingReport
        public ActionResult ProformaPrint(int id)
        {
            ViewBag.Id = id;
            return View(); 
        }
        public ActionResult EstimationPrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult EstimationSummaryPrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}