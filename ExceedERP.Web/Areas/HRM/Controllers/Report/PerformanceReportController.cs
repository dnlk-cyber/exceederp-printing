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
            return View();
        }
        public ActionResult EmployeePerformanceDepartment()
        {
            ApplicationDbContext adb = new ApplicationDbContext();
            ExceedDbContext db = new ExceedDbContext();
            string username = User.Identity.Name;
            var user = adb.Users.FirstOrDefault(x => x.UserName == username);

            if (user != null)
            {
                ViewBag.id = user.EmployeeId;

                int.TryParse(user.EmployeeId, out int empId);
                var place = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);

                if(place != null)
                {
                    ViewBag.dep = place.OrgStructureId;
                }
            }

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
        [AllowAnonymous]
        public ActionResult MyPerformance()
        {
            var adb = new ApplicationDbContext();

            string username = User.Identity.Name;

            var user = adb.Users.FirstOrDefault(x => x.UserName == username);

            if (user != null)
            {
                ViewBag.id = user.EmployeeId;
            }

            return View();
        }
    }
}