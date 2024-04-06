using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Microsoft.Owin.Security.Provider;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Report
{
    [AuthorizeRoles(HrmConstants.HRPerformanceReport, HrmConstants.HRReport, HrmConstants.HRAdmin)]
    public class PerformanceReportController : BaseController
    {
        // GET: HRM/EmployeeReport
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult EmployeePerformance()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Performance, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeePerformanceDepartment()
        {
            ApplicationDbContext adb = new ApplicationDbContext();
            string username = User.Identity.Name;
            var user = adb.Users.FirstOrDefault(x => x.UserName == username);

            if (user != null)
            {
                ViewBag.id = user.EmployeeId;
            }
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Performance_Department, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeeYearlyPerformance()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Yearly_Performance, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult EmployeeBscPerformance()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Bsc_Performance, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ProbationMeasure()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Probation_Measure, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult MyPerformance()
        {
            var adb = new ApplicationDbContext();

            string username = User.Identity.Name;

            var user = adb.Users.FirstOrDefault(x => x.UserName == username);

            if (user != null)
            {
                ViewBag.id = user.EmployeeId;
            }
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.My_Performance, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
    }
}