using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.Global;

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
        public ActionResult JobPrintNew(string id)
        {
            ViewBag.FormTitle = "Job Order Print";
            ViewBag.FormNumber = "1";
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ExternalMaintenanceSatisfaction(string id)
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.SatisfactionNote, out string title, out string number);
            ViewBag.Id = id;
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult JobPrintNewExternal(string id)
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.VehiclesExternalMaintenanceOrderForm, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            ViewBag.Id = id;
            return View();
        }
        public ActionResult OutSourcedMaintenanceRequest(string id)
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.Maintenance, MaintenanceIsoConst.OutSourcedMaintenanceRequest, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            ViewBag.Id = id;
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