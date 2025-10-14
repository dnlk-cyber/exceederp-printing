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
        public ActionResult PrintJobOrder(int id)
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
        public ActionResult PrintJobCard(int id) 
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ProductionFollowUpPrint(int id)
        {
            ViewBag.Id = id; 
            return View();
        }

        public ActionResult PrintEstimationMonthlyReport()
        {
            return View();
        }
        public ActionResult PrintEstimationWeeklyReport()
        {
            return View();
        }
    }
}