using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Insurance
{
    public class InsuranceReportsController : Controller
    {
        // GET: HRM/InsuranceReports
        public ActionResult YearlyTotalCarAccident()
        {
            return View();
        }
        public ActionResult AccidentFromProjects()
        {
            return View();
        }
        public ActionResult AccidentWithMoney()
        {
            return View();
        }
        public ActionResult AccidentCoveredByInsuranceAndAskedForInsurance()
        {
            return View();
        }
        
        public ActionResult TotalAccidentInOurEmployees()
        {
            return View();
        }
        public ActionResult CostsToGetMednInsurance()
        {
            return View();
        }
        public ActionResult ArbonPaidForAssets()
        {
            return View();
        }
        public ActionResult ComplatedWorksInBRPSchedule()
        {
            return View();
        }
    }
}