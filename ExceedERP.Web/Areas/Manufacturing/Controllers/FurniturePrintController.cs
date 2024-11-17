using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class FurniturePrintController : Controller
    {
        // GET: Manufacturing/FurniturePrint
        public ActionResult Proforma(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult FurnitureSummary()
        {            
            return View();
        }
    }
}