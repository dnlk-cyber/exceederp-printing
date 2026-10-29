using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.Helpers.Planning;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Planning.Controllers
{

    [Authorize]
    public class PlanningReportsController : BaseController
    {
        // GET: TDS/TDSReport
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Plan()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Plan_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult StrategicPlan()
        {
            ViewBag.FormTitle = "Strategic Plan Report";
            ViewBag.FormNumber = string.Empty;
            return View();
        }
        public ActionResult QuarterlyPlan()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Quarterly_Plan_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult PlanVsActualQuarterly()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Plan_VS_Actual_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult PlanVsActualDynamic()
        {
            ViewBag.FormTitle = "Plan Vs Actual Report (Quarter Selection)";
            ViewBag.FormNumber = string.Empty;
            return View();
        }
        public ActionResult QuickFixPlan()
        {
            ViewBag.FormTitle = "Quick-Fix Plan";
            ViewBag.FormNumber = string.Empty;
            return View();
        }
        public ActionResult PlanWOWeek()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Plan_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult PlanSelfCare()
        {

            return View();
        }
        public ActionResult Actual()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Actual_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ActualWOWReport()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Actual_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult PlanVsActualWOWeek()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Plan_VS_Actual_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult PlanVsActualForWeeklyReport()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Plan_Vs_Actual_for_Weekly_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ProjectPlan()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Project_Plan_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ProjectActual()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Project_Actual_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult PlanningAndActual()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Planning_And_Actual_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ProjectPlanVSActual()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Project_Plan_VS_Actual_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ProjectPlanVsActualForWeeklyReport()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Project_Plan_Vs_Project_Actual_for_Weekly_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult RiskMitigationPlanReport()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Risk_Mitigation_Plan_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        
        public ActionResult RiskFollowUpReport()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Risk_Follow_Up_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ExcutiveSummary()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Excutive_Summary_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;

            return View();
        }
        public ActionResult ProjectExcutiveSummary()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Project_Excutive_Summary, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ProjectInformationPlanQuarteryReport()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Project_Information_Plan_Quartery_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ProjectInformationQurterlySummaryReport()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Project_Qurterly_Summary_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ProjectSummaryBySectors()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Project_Summary_By_Sectors, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ProjectExcutiveSummaryPerProject()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.Project_Excutive_Summary_Per_Project, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult KpiProgressTrackerReport()
        {
            PlanningIsoDocumentHelper.GetIsoDocumentInfo(Modules.Planning, PlanningModuleReportType.KPI_Progress_Tracker_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
    }
}
