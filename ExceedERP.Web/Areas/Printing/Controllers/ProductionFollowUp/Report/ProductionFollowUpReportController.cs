using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Report

{
    public class ProductionFollowUpReportController : Controller
    {
        // GET: ProductionFollowUp/ReportProductionFollowUp
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult ReportViewerView1(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult FinishedGoodReceival(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult ProductDelivery(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult GetFinishedProductsReceivingNote()
        {
            ViewBag.UserName = User.Identity.Name;
            return View();
        }
        public ActionResult GetPrintingAndFinishingTeamPerformance()
        {
            return View();

        }
        public ActionResult GetPrintingAndFinishingTeamPerformanceWp()
        {
            return View();

        }

        public ActionResult GetDeliveryTeamPerformance()
        {
            return View();

        }
        public ActionResult GetDeliveryTeamPerformanceWp()
        {
            return View();

        }

        public ActionResult GetDeliveryNote(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult GetJobOrders()
        {
            return View();

        }
        public ActionResult GetJobSupply()
        {
            return View();

        }
        public ActionResult GetJobSupplyandSales()
        {
            return View();

        }
        public ActionResult GetJobSupplyandSalesWp()
        {
            return View();

        }
        public ActionResult GetDeliveryFollowUpSheet()
        {
            return View();
        }
        public ActionResult ProformaPrint(int id)
        {
            ViewBag.id = id;
            return View();
        }
    }
}