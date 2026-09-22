using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class DistanceSelectorDataProviderController : Controller
    {
        // GET: FleetManagement/DistanceSelectorDataProvider
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult DumpTruckDistanceTo()
        {
            var fetchFromRates = db.EquipmentRates.Where(x => x.RoadType == null).ToList();
            var result = fetchFromRates.Select(s => new
            {
                value = s.DistanceTo,
                text = s.DistanceTo
            });
            return Json(result,JsonRequestBehavior.AllowGet);
        }
        public JsonResult DumpTruckDistanceFrom()
        {
            var fetchFromRates = db.EquipmentRates.Where(x => x.RoadType == null).ToList();
            var result = fetchFromRates.Select(s => new
            {
                value = s.DistanceFrom,
                text = s.DistanceFrom
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OtherTruckDistanceTo()
        {
            var fetchFromRates = db.EquipmentRates.Where(x => x.DumpTruckTypeOfWork == null).ToList();
            var result = fetchFromRates.Select(s => new
            {
                value = s.DistanceTo,
                text = s.DistanceTo
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OtherTruckDistanceFrom()
        {
            var fetchFromRates = db.EquipmentRates.Where(x => x.DumpTruckTypeOfWork == null).ToList();
            var result = fetchFromRates.Select(s => new
            {
                value = s.DistanceFrom,
                text = s.DistanceFrom
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}