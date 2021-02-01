using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Provider;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Report
{
     [Authorize(Roles = HR_Report + "," + HR_Performance_Report)]
    public class CourtPerformanceReportController : BaseController
    {
        // GET: HRM/EmployeeReport
        private const string HR_Report = "HR_Report";
        private const string HR_Performance_Report = "HR_Performance_Report";
        public ActionResult Index()
        {
            return View();
        }

        

        public ActionResult EmployeePerformance()
        {
            return View();
        }

        public ActionResult EmployeeYearlyPerformance()
        {
            return View();
        }

        public ActionResult EmployeeBscPerformance()
        {
            return View();
        }
    }
}