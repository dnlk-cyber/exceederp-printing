using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.PrintableLetters
{

    [AuthorizeRoles(HrmConstants.HRPrintableDocumentsUser, HrmConstants.HREmployeeSelfService, HrmConstants.HRAdmin)]
    public class PrintableLetterController : BaseController
    {

        // GET: HRM/PrintableLetter
        public ActionResult PayableLeave()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Payable_Leave, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult LeavePlan()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Leave_Plan, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult TerminationLetter()
        {

            return View();
        }

        public ActionResult RecruitmentExt()
        {
            return View();
        }

        public ActionResult RecruitmentForProject()
        {
            return View();
        }

        public ActionResult PatientTransferFormat()
        {
            return View();
        }

        public ActionResult InternalVacancy()
        {
            return View();
        }

        public ActionResult InternalVacancyForSelfCare()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Internal_Vacancy_For_SelfCare, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult InternalApplicant()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Internal_Applicant, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ExperienceLetter()
        {
            return View();
        }
        public ActionResult ExperienceLetterTwo()
        {
            return View();
        }

        public ActionResult ClearanceLetter()
        {
            return View();
        }
        public ActionResult ExternalApplicant()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.External_Applicant, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult VacancyForMedia()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Vacancy_For_Media, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult VacancyForInternal()
        {
            return View();
        }
        public ActionResult Promotion()
        {
            return View();
        }
        public ActionResult PromotionLetterTwo()
        {
            return View();
        }
        public ActionResult Transfer()
        {
            return View();
        }
        public ActionResult TransferLetterTwo()
        {
            return View();
        }
        public ActionResult Contract()
        {
            return View();
        }
        public ActionResult Recruitment()
        {
            return View();
        }
        public ActionResult RecruitmentLetterTwo()
        {
            return View();
        }
        public ActionResult WarrantyLetter()
        {
            return View();
        }
        public ActionResult RealeseLetter()
        {
            return View();
        }
        public ActionResult LeaveRequestLetter(int id)
        {

            ViewBag.id = id;

            return View();
        }
        public ActionResult ManPowerPlanningRequestLetter(int id)
        {

            ViewBag.id = id;

            return View();
        }
        public ActionResult RecruitmentRequestLetter(int id)
        {

            ViewBag.id = id;

            return View();
        }
        public ActionResult TrainingRequestLetter(int id)
        {

            ViewBag.id = id;

            return View();
        }
        public ActionResult ReferalSheet(int id)
        {

            ViewBag.id = id;

            return View();
        }
        public ActionResult ExperienceEnglishTwo()
        {
            return View();
        }
        public ActionResult ExperienceAmharicTwo()
        {
            return View();
        }
        public ActionResult ExperienceEnglishFBC()
        {
            return View();
        }
        public ActionResult ExperienceAmharicFBC()
        {
            return View();
        }
        public ActionResult Deligation(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult Commitment(int id)
        {
            ViewBag.id = id;
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Commitment_report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ComplainResponseDepartmentLetter(int id)
        {
            ViewBag.id = id;
            return View();
        }

        public ActionResult ComplainDescriptionLetter(int id)
        {
            ViewBag.id = id;
            return View();
        }

        public ActionResult LaboratoryRequestForm(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult TerminationAcceptanceLetter()
        {
            return View();
        }
        public ActionResult EmployeeProfile()
        {
           
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Profile_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ChangeOfPayroll(int id)
        {
            ViewBag.id = id;
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Change_Of_Payroll, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult TrainingEvaluation(int traineeId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var trainee = db.Trainee.FirstOrDefault(x => x.TraineeId == traineeId);
            if(trainee != null)
            {
                ViewBag.courseTrackingId = trainee.CourseTrackingId;
                ViewBag.empId = trainee.EmployeeId;
            }
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Training_Evaluation, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult AttendanceLetter()
        {
            ExceedDbContext db = new ExceedDbContext();
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            string depname = "";
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == id && x.isCurrent);
            if (placement != null)
            {
                var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                if (mainCompany != null)
                {
                    depname = mainCompany.Name;
                }
            }

            ViewBag.id = id;
            ViewBag.depname = depname;
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Attendance_Letter, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
    }
}