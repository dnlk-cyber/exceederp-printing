using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Provider;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Report
{
     [Authorize(Roles = HR_Leave_Report + "," + HR_Report+","+HR_Admin )]
    public class CourtLeaveReportController : BaseController
    {
        // GET: HRM/EmployeeReport
         private const string HR_Leave_Report = "HR_Leave_Report";
         private const string HR_Report = "HR_Report";
         private const string HR_Admin = "HR_Admin";
        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult LeaveSummary()
        {
            return View();
        }

      

        public ActionResult EmployeeInLeave()
        {
            return View();
        }

       
        public ActionResult EmployeeLeaveSummary()
        {
            return View();
        }

        public ActionResult EmployeeLeaveUsage()
        {
            return View();
        }

     
    }
}