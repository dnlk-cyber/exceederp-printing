using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.HR;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Report
{
   
    [AllowAnonymous]
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
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Long_Term_Education, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeeSummary()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Summary, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult ContractEmployee()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Contract_Employee, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult TerminationSummary()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Termination_Summary, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult TerminationReasonSummary()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Termination_Summary, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult DisciplineMeasureSummary()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Discipline_Measure_Summary, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult RetirementNotification()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Retirement_Notification, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult EmployeeBirthDate()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Birth_Date, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult EmployeeDisciplinaryMeasure()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Disciplinary_Measure, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeeJobTitle()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Job_Title, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult EmployeeSalary()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Salary, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult EmployeePension()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Pension, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult Age()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Summary_by_Age, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmploymentType()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employment_Type, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult JobTitle()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Job_Title, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EducationalLevel()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Educational_Level, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EducationalField()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Educational_Field, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult Nation()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Summary_By_Nation, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult Rank()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Summary_By_Rank, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeeTermination()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Termination, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeeTraining()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Training, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeeEducation()
        {

            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Education, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeeExperience()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Experience, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeeByBranch()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_By_Branch, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
      
        [AuthorizeRoles(HrmConstants.HRRecruitmentReport)]
        public ActionResult VacantPostion()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Vacant_Postion, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeeCourtInsuranceReport()
        {
            return View();
        }
        public ActionResult EmployeeJudgeByRegion()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Judge_By_Region, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
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
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Promotion_PerYear, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
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
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Attendance_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult DayAbsentAndLateComingReport()
        {
            ViewBag.UserName = User.Identity.Name;
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Day_Absent_And_Late_Coming_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
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
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Work_and_Cost_Summary, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeeMasterReport()
        {

            return View();
        }
        public ActionResult TobePromoted()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_to_be_Promoted, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult Clinic()
        {
            return View();
        }
        public ActionResult EmployeeHealthPayment()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Health_and_Value_Payment, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeeHealthPaymentSelfCare()
        {
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            ViewBag.id = empId;
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Health_and_Value_Payment_selfCare, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EducationAndTrainingOneByOne()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Education_and_Training_One_By_One, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult TrainingOneByOne()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Training_One_By_One, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult TrainingRequestReport()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Training_Request_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult NotPlacedEmployees()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Not_Placed_Employees, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult CompanyStructure()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Company_Structure, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult MonthlySalaryPerPeriod()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Monthly_Salary_Per_Period, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeePlacement()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Placement_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeeProfileReport()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Profile_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult SeverancePayableReport(int id)
        {
            ViewBag.id = id;
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Severance_Payable_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult OverTimeHours()
        {
            GetBranchList();
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Over_Time_Hours, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult BenefitReport()
        {
            GetBranchList();
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Benefit_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult NotEvaluatedEmployeeReport()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Not_Evaluated_Employee_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
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
            ViewBag.UserName = User.Identity.Name;
        }
        public ActionResult EmployeeTypeSummary()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Type_Summary_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult EmployeeSummaryByBusinessUnit()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Summary_By_Business_Unit, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeeTerminationSummaryByBusinessUnit()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Termination_Summary_By_Business_Unit, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult FiscalYearTrainingPlanActionPlan()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Training_Action_Plan, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult TrainingNeedRequestReport()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Training_Need_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

    }
}