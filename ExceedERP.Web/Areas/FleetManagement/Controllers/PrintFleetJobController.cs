using ExceedERP.Web.Helpers;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.FleetReport)]
    public class PrintFleetJobController : Controller
    {
        //
        // GET: /FleetManagement/PrintJob/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DailyCare(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult FleetJobRequest(string Id)
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
        public ActionResult JobOrder(int Id)
        {
            ViewBag.JobId = Id;
            return View();
        }
        public ActionResult Cannibalization(string Id)
        {
            ViewBag.JobId = Id;
            return View();
        }

    }
}