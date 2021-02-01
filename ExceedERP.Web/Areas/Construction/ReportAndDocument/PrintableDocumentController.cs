using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.ReportAndDocument
{
    public class PrintableDocumentController : Controller
    {
        // GET: Construction/PrintableDocument
        public ActionResult BOQSummary()
        {
            return View();
        }
        public ActionResult BOQGrandSummary()
        {
            return View();
        }
    }
}