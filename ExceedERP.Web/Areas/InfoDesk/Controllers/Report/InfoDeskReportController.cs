using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers.Report
{
    public class InfoDeskReportController : Controller
    {
        // GET: CRM/CRMCustomerReport
        public ActionResult Index()
        {
            return View();
        }
    
      
        public ActionResult RepTicketFrequency()
        {
       
            return View();
        }
    }
}