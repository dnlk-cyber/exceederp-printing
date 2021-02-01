using ExceedERP.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Report
{
    [AuthorizeRoles( HrmConstants.HRAdmin, HrmConstants.HRReport)]
    public class InsuranceReportController : BaseController
    {
        // GET: HRM/LetterReport
        public ActionResult VehicleInsuranceReport()
        {
            return View();
        }
        public ActionResult VehicleInsuranceByTypeReport()
        {
            return View();
        }
        public ActionResult VehicleInsuranceByLocationReport()
        {
            return View();
        }
        public ActionResult EmployeeRestitution()
        {
            return View();
        }
        public ActionResult InsurancePayment()
        {
            return View();
        }
        public ActionResult YearlyTotalCarAccident()
        {
            return View();
        }
    }
}