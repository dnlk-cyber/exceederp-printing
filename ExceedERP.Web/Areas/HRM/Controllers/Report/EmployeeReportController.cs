using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Microsoft.Owin.Security.Provider;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Report
{
   
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentReport, HrmConstants.HRReport, HrmConstants.HRDepartmentHead)]
    public class EmployeeReportController : BaseController
    {
        // GET: HRM/EmployeeReport
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult PatientReferralReport()
        {
            return View();
        }
        
        [AuthorizeRoles(HrmConstants.HREmployeeSelfService)]
        public ActionResult EmployeeFixedAsset()
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
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LongTermEducation()
        {
            return View();
        }
        public ActionResult EmployeeSummary()
        {
            return View();
        }

        public ActionResult TerminationSummary()
        {
            return View();
        }

        public ActionResult TerminationReasonSummary()
        {
            return View();
        }

        public ActionResult DisciplineMeasureSummary()
        {
            return View();
        }
        public ActionResult RetirementNotification()
        {
            return View();
        }

        public ActionResult EmployeeBirthDate()
        {

            return View();
        }

        public ActionResult EmployeeDisciplinaryMeasure()
        {
            return View();
        }
        public ActionResult EmployeeJobTitle()
        {
            return View();
        }

        public ActionResult EmployeeSalary()
        {
            return View();
        }

        public ActionResult EmployeePension()
        {
            return View();
        }
        public ActionResult Age()
        {
            return View();
        }
        public ActionResult EmploymentType()
        {
            return View();
        }
        public ActionResult JobTitle()
        {
            return View();
        }
        public ActionResult EducationalLevel()
        {
            return View();
        }
        public ActionResult EducationalField()
        {
            return View();
        }
        public ActionResult Nation()
        {
            return View();
        }
        public ActionResult Rank()
        {
            return View();
        }
        public ActionResult EmployeeTermination()
        {
            return View();
        }
        public ActionResult EmployeeTraining()
        {

            return View();
        }
        public ActionResult EmployeeEducation()
        {

            return View();
        }
        public ActionResult EmployeeExperience()
        {

            return View();
        }
        public ActionResult EmployeeByBranch()
        {
            return View();
        }
      
        [AuthorizeRoles(HrmConstants.HRRecruitmentReport)]
        public ActionResult VacantPostion()
        {
            return View();
        }
        public ActionResult EmployeeCourtInsuranceReport()
        {
            return View();
        }
        public ActionResult EmployeeJudgeByRegion()
        {
            return View();
        }
        public ActionResult JudgeStatistics()
        {
            return View();
        }
        public ActionResult JudgeByAge()
        {
            return View();
        }
        public ActionResult JudgeByAgeEducation()
        {
            return View();
        }
        public ActionResult Judgbyassignment()
        {
            return View();
        }
        public ActionResult EmployeePromotionPerYear()
        {
            return View();
        }
        public ActionResult JudgebyBranchCount()
        {
            return View();
        }
        public ActionResult JudgeList()
        {
            return View();
        }
        public ActionResult JudgebyEduLevelCount()
        {
            return View();
        }
        public ActionResult JudgebyJobTitle()
        {
            return View();
        }
        public ActionResult JudgebyCourse()
        {
            return View();
        }
        public ActionResult AttendanceReport()
        {
            ViewBag.UserName = User.Identity.Name;
            return View();
        }
        public ActionResult DepartmentAttendanceReport()
        {
            ViewBag.Dept = 0;
            ViewBag.UserName = User.Identity.Name;

            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            ViewBag.BossEmpId = empId;
            var placment = db.Placements.FirstOrDefault(x => x.isCurrent && x.isCurrent && x.EmployeeId == empId);
            if (placment!=null)
            {
                ViewBag.Dept = placment.OrgStructureId;
            }

            return View();
        }
        public ActionResult DepartmentLateComingReport()
        {
            ViewBag.Dept = 0;
            ViewBag.UserName = User.Identity.Name;
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var placment = db.Placements.FirstOrDefault(x => x.isCurrent && x.isCurrent && x.EmployeeId == empId);
            if (placment != null)
            {
                ViewBag.Dept = placment.OrgStructureId;
            }

            return View();
        }
        public ActionResult WorkandCostSummary()
        {
            return View();
        }
        public ActionResult EmployeeMasterReport()
        {
            return View();
        }
        public ActionResult TobePromoted()
        {

            return View();
        }
        public ActionResult Clinic()
        {
            return View();
        }
        public ActionResult EmployeeHealthPayment()
        {
            return View();
        }
        public ActionResult EmployeeHealthPaymentSelfCare()
        {
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            ViewBag.id = empId;
            return View();
        }
        public ActionResult EducationAndTrainingOneByOne()
        {
            return View();
        }
        public ActionResult TrainingOneByOne()
        {
            return View();
        }
        public ActionResult TrainingRequestReport()
        {
            return View();
        }
        public ActionResult NotPlacedEmployees()
        {
            return View();
        }
        public ActionResult CompanyStructure()
        {
            return View();
        }
        public ActionResult MonthlySalaryPerPeriod()
        {
            return View();
        }

    }
}