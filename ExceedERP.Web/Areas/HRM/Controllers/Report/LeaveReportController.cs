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
    
     [AuthorizeRoles(HrmConstants.HRLeaveReport, HrmConstants.HRReport, HrmConstants.HRAdmin, HrmConstants.HREmployeeSelfService, HrmConstants.HRSelfServiceManager)]
    public class LeaveReportController : BaseController
    {
        // GET: HRM/EmployeeReport
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
        public ActionResult EmployeeLeaveUsageSelfCare()
        {
            var users = new ApplicationDbContext();
            var db = new ExceedDbContext();
            var user = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                if (user.EmployeeId != null)
                {
                    int empid = Convert.ToInt32(user.EmployeeId);
                    var employes = db.Person_Employee.Where(x => x.EmployeeId == empid).FirstOrDefault();
                    if (employes != null)
                    {
                        ViewBag.id = employes.EmployeeId;
                    }
                }
                else
                {
                    ViewBag.id = null;
                }
            }
            return View();
        }
        public ActionResult LeavePlan()
        {
            return View();
        }
        public ActionResult LeaveRequestStatusReport()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Leave_Request_Status, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult LeavePayableReport(int id)
        {
            ViewBag.id = id;
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Leave_Payable_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

    }
}