using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Microsoft.Owin.Security.Provider;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Report
{
     [AuthorizeRoles(HrmConstants.HRPerformanceReport, HrmConstants.HRReport,HrmConstants.HRAdmin)]
    public class PerformanceReportController : BaseController
    {
        // GET: HRM/EmployeeReport
        public ActionResult Index()
        {
            return View();
        }

        

        public ActionResult EmployeePerformance()
        {
            return View();
        }
        public ActionResult EmployeePerformanceDepartment()
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
        public ActionResult ProbationMeasure()
        {
            return View();
        }
    }
}