using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Archive;
using ExceedERP.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class ArchiveReportController : Controller
    {
        // GET: LC/LCReport

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IncomingLettersReport()
        {
            ArchiveIsoDocumentHelper.GetIsoDocumentInfo(Modules.ArchiveAndLetters, ArchiveModuleReportType.Incoming_Letters_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult OutgoingLettersReport()
        {
            ArchiveIsoDocumentHelper.GetIsoDocumentInfo(Modules.ArchiveAndLetters, ArchiveModuleReportType.Outgoing_Letters_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
       
        //public ActionResult GetResponse(string id)
        //{
        //    ViewBag.id = id;
        //    LegalCaseIsoDocumentHelper.GetIsoDocumentInfo(Modules.LegalCase, LegalCaseModuleReportType.Get_Response, out string title, out string number);
        //    ViewBag.FormTitle = title;
        //    ViewBag.FormNumber = number;
        //    return View();
        //}
    }
}