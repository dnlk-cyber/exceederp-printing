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
        public ActionResult MachinaryAndVehicelMaintenanceReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.MachinaryAndVehicelMaintenanceReport, out string title, out string number);
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