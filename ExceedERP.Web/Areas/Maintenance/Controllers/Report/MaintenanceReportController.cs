using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.Report
{
    public class MaintenanceReportController : Controller
    {
        // GET: Maintenance/MaintenanceSummary
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MaintenanceSummary()
        {

            return View();
        }
        public ActionResult MaintenanceDuration()
        {

            return View();
        }
        public ActionResult MaintenanceReport()
        {
            return View();
        }
        public ActionResult InternalMaintenanceCost()
        {
            return View();
        }

        public ActionResult DailyMaintenanceReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.DailyMaintenanceReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult MechanicPerformance()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.MechanicPerformance, out string title, out string number);
            ViewBag.FormTitle = "Mechanic Performance Report";
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult DailyMaintenanceReports()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.DailyMaintenanceReports, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult MaintenanceCostSheetReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.MaintenanceCostSheetReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult MaintenancePlanVsActual()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.MaintenancePlanVsActual, out string title, out string number);
            ViewBag.FormTitle = "Maintenance Plan vs Actual Report";
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ExternalMaintenancePaymentPrint(string id)
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.ExternalMaintenancePaymentPrint, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            ViewBag.Id = id;
            return View();
        }
        public ActionResult OutSourcedMaintenancePaymentRequest(string id)
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.OurSourcedMaintenancePaymentRequest, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            ViewBag.id = id;
            return View();
        }
        public ActionResult OutSourcedMaintenanceReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.OutSourcedMaintenanceReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;           
            return View();
        }
        public ActionResult MachineAndVehicleMaintenanceAndRepair()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.MachineAndVehicleMaintenanceAndRepairReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult MachinaryAndVehicelMaintenanceReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.MachinaryAndVehicelMaintenanceReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult StandardVsActualTimeReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.StandardVsActualTimeReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult WeeklyCMPMPerformanceReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.WeeklyCMPMPerformanceReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult WeeklyPMAndCMPerformanceReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.WeeklyMaintenancePlanVsActual, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult WeeklyPMCMPerfomanceReportSummary()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.WeeklyPMCMPerfomanceReportSummary, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult GarageMaintenanceCostBreakDownReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.GarageMaintenanceCostBreakDownReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult MVPmExecutionReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.MVPmExcecutionReprot, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        } 
       public ActionResult DailyJobRegistration()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.DailyJobRegistration, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult WeeklyCMPMPerformanceSummaryReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.WeeklyCMPMPerformanceReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult InternalMaintenanceCostReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.InternalMaintenanceCostReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

    }
}