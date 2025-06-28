using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers.TenderManagement
{
    public class WinnerPrintableController : Controller
    {
       
        public ActionResult ItemWinnerPrint(int id)
        {

            ViewBag.id = id;              
            return View();
        }
    }
}




