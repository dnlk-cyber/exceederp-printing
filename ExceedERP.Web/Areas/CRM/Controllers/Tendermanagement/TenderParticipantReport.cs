using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers.TenderManagement
{
    public class TenderParticipantReportController : Controller
    {
       
        public ActionResult TenderParticipant(int id)
        {

            ViewBag.id = id;
            ViewBag.FormTitle = "Tender Participants List";
            ViewBag.FormNumber = id;
            return View();
        }
        public ActionResult TenderParticipantWinner(int id)
        {

            ViewBag.id = id;
            ViewBag.FormTitle = "Tender Participant Winner ";
            ViewBag.FormNumber = id;
            return View();
        }
    }
}




