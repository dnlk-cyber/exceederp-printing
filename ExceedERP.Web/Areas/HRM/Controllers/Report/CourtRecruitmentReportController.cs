using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Provider;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Report
{
     [Authorize(Roles = HR_Report + "," + HR_Recruitment_Report+","+HR_Admin )]
    public class CourtRecruitmentReportController : BaseController
    {
        // GET: HRM/EmployeeReport
        private const string HR_Report = "HR_Report";
        private const string HR_Recruitment_Report = "HR_Recruitment_Report";
        private const string HR_Admin = "HR_Admin";
        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult RecruitmentSummary()
        {
            return View();
        }    
        
        public ActionResult EmployeeRecruitment()
        {
            return View();
        }

      
       
        public ActionResult VacantPostion()
        {
            return View();
        }

      
      public ActionResult   VacantPostionDetail()
        {
            return View();
        }
      
    }
}