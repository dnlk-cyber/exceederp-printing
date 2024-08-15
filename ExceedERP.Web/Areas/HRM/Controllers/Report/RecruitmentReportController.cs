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
     [AuthorizeRoles(HrmConstants.HRRecruitmentReport, 
        HrmConstants.HRReport, 
        HrmConstants.HRAdmin)]
    public class RecruitmentReportController : BaseController
    {
        // GET: HRM/EmployeeReport
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