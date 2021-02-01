using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;

namespace ExceedERP.Web.Areas.HRM.Controllers.PrintableLetters
{

    [AuthorizeRoles(HrmConstants.HRPrintableDocumentsUser, HrmConstants.HREmployeeSelfService, HrmConstants.HRAdmin)]
    public class PrintableLetterController : BaseController
    {

        // GET: HRM/PrintableLetter
        public ActionResult PayableLeave()
        {
            return View();
        }
        public ActionResult LeavePlan()
        {
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
            return View();
        }
        public ActionResult InternalApplicant()
        {
            return View();
        }
        public ActionResult ExperienceLetter()
        {
            return View();
        }
        public ActionResult AmhraricExperienceLetter()
        {
            return View();
        }

        public ActionResult ClearanceLetter()
        {
            return View();
        }
        public ActionResult ExternalApplicant()
        {
            return View();
        }
        public ActionResult VacancyForMedia()
        {
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
        public ActionResult Transfer()
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
        public ActionResult ChangeOfPayroll(int id)
        {
            ViewBag.id = id; 
            return View();
        }

    }
}