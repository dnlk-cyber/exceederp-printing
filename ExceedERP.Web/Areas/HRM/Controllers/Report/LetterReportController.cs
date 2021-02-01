using ExceedERP.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Report
{
    
    [AuthorizeRoles(HrmConstants.HRArchiveReport, HrmConstants.HRReport)]
    public class LetterReportController : BaseController
    {
        // GET: HRM/LetterReport
        public ActionResult LetterReport()
        {
            return View();
        }
    }
}