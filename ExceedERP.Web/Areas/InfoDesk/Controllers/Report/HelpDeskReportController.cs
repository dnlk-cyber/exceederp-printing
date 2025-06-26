using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.InfoDesk;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers.Report
{
    public class HelpDeskReportController : Controller
    {
        // GET: InfoDesk/HelpDeskReport
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RepTicketFrequency()
        {

            return View();
        }
        public ActionResult MaintenanceForm()
        {
            InfoDeskIsoDocumentHelper.GetIsoDocumentInfo(Modules.InfoDesk, InfoDeskModuleReportType.ICT_Directorate_Maintainance_Form, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ITSupportReport(int id)
        {
            InfoDeskIsoDocumentHelper.GetIsoDocumentInfo(Modules.InfoDesk, InfoDeskModuleReportType.IT_Support_Report, out string title, out string number);
            ViewBag.id = id;
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult SoftwareLicense()
        {
            InfoDeskIsoDocumentHelper.GetIsoDocumentInfo(Modules.InfoDesk, InfoDeskModuleReportType.Software_License, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
    }
}