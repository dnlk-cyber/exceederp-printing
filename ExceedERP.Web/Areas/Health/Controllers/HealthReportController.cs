using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.HealthAndInsurance;
using ExceedERP.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Health.Controllers
{
    public class HealthReportController : BaseController
    {
        // GET: HealthAndInsurance/HealthReport
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EmployeeHealthPayment()
        {
            HealthAndInsuranceIsoDocumentHelper.GetIsoDocumentInfo(Modules.Health, HealthAndInsuranceModuleReportType.Employee_Health_and_Value_Payment, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult Clinic()
        {
            HealthAndInsuranceIsoDocumentHelper.GetIsoDocumentInfo(Modules.Health, HealthAndInsuranceModuleReportType.Clinic_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult PatientReferralReport()
        {
            HealthAndInsuranceIsoDocumentHelper.GetIsoDocumentInfo(Modules.Health, HealthAndInsuranceModuleReportType.Patient_Referral_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult MedicalRequestLetter(int id)
        {

            ViewBag.id = id;

            return View();
        }
    }
}