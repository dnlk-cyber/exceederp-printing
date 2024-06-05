using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    //[AuthorizeRoles(FleetManagementRoles.MaintenanceReport)]
    public class MaintenancePrintController : Controller
    {
        //
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DailyCare(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult JobRequest(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult EquipmentReturn(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult HistoryJacket(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
       

        public ActionResult JobOrderReferance(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult JobOrder(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult WorkShop(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        
        public ActionResult Cannibalization(string Id)
        {
            ViewBag.JobId = Id;
            return View();
        }

    }
}