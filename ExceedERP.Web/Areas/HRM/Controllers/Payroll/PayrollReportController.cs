using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
     [Authorize(Roles = Payroll_Report + "," + Payroll_View + "," + HR_Employee_SelfService + "," + HR_Admin)]

    public class PayrollReportController : BaseController
    {
         private const string Payroll_Report = "Payroll_Report";
         private const string Payroll_View = "Payroll_View";
         private const string HR_Employee_SelfService = "HR_Employee_SelfService";
         private const string HR_Admin = "HR_Admin";
        // GET: HR/PayrollReport
        public ActionResult BankPaymentList()
        {
            GetBranchList();
            return View();
        }

         public void GetBranchList()
         {
             List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
             string branch = "";
             foreach (var b in mybraches)
             {
                 if (branch != "")
                 {
                     branch += "-" + b;
                 }
                 else
                 {
                     branch = b;
                 }

             }
             ViewBag.Branches = branch;
             ViewBag.UserName = User .Identity .Name ;
         }
        public ActionResult BasicSalaryList()
         {
             GetBranchList();
            return View();
        }

         public ActionResult DaynamicPayrollDetail()
        {
            GetBranchList();
             return View();
         }
        public ActionResult Loan()
         {
             GetBranchList();
            return View();
        }
        public ActionResult Advance()
        {
            GetBranchList();
          
            return View();
        }

        public ActionResult BonusincomeTax()
        {
            return View();
        }

        public ActionResult GetMonthlyPensionReport()
        {
            GetBranchList();
            return View();
        }
        public ActionResult GetMonthlyProvidentReport()
        {
            GetBranchList();
            return View();
        }
        public ActionResult IncomeTax()
        {
            GetBranchList();
            return View();
        }

        public ActionResult MonthlyLabourAndCredit()
        {
            GetBranchList();
            return View();
        }
        public ActionResult PayrollInput()
        {
            GetBranchList();
            return View();
        }
        public ActionResult PaySlip()
        {
            GetBranchList();
            return View();
        }
           [Authorize(Roles = HR_Employee_SelfService)]
        public ActionResult PaySlipByEmployee()
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
                        ViewBag.employee = employes.EmployeeCode;
                    }
                }
                else
                {
                    ViewBag.employee = null;
                }
            }
            return View();
        }
        public ActionResult OverTimeHours()
        {
            GetBranchList();
            return View();
        }

        public ActionResult PayrollDetail()
        {
            GetBranchList();
            return View();
        }

        public ActionResult PayrollSummary()
        {
            GetBranchList();
            return View();
        }

        public ActionResult ReportByContributionType()
        {
            GetBranchList();
            return View();
        }
        public ActionResult AllowanceReport()
        {
            GetBranchList();
            return View();
        }
         public ActionResult PayrollSummaryReconcilation()
        {
            GetBranchList();
            return View();
        }
         public ActionResult BonusBankPayment()
         {
             return View();
         }

        
    }
}