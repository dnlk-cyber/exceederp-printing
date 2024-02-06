using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    public class FurnitureEstimationReportController : Controller
    {
        // GET: PrintingEstimation/PrintingReport
        public ActionResult FurnitureEstimationForms(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ProformaFurniture(int id=5)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult JobOrder(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult ChangeOfOrder(int id)
        {
            ViewBag.id = id;
            return View();
        }
    }
}