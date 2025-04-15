using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.JobCosting.Report
{
    public class JobCostingReportController : Controller
    {
        // GET: JobCosting/ReportJobCosting
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult JobOrderTimeSheet(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult GetJobOrderCard(int id)
        {          
            ViewBag.Id = id;          
            return View();
        }
        public ActionResult JobCostReportForAll()
        {
            
            return View();
        }
    }
}