using ExceedERP.DataAccess.Context;
using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetEquipmentRent
{
    public class RatesOfLoadOrRoadSelectorController : Controller
    {
        // GET: FleetManagement/HaeavyOrDumpTruckSelector
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RatesOfLoadOrRoadSelector_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var haeavyOrDumpTruckSelector = db.RateOfRoadORLoads.Where(x => x.RoadOrLoadId == id).AsQueryable();
            DataSourceResult result = haeavyOrDumpTruckSelector.ToDataSourceResult(request, haeavyOrDumpTruckSelectors => new
            {
                RoadOrLoadId = haeavyOrDumpTruckSelectors.RoadOrLoadId,
                RateOfRoadORLoadId = haeavyOrDumpTruckSelectors.RateOfRoadORLoadId,
                Name = haeavyOrDumpTruckSelectors.Name,
                Rate = haeavyOrDumpTruckSelectors.Rate,
            });
            return Json(result);

        }
        public ActionResult RatesOfLoadOrRoadSelector_Create([DataSourceRequest] DataSourceRequest request, RateOfRoadORLoad haeavyOrDumpTruckSelector, int id)
        {
            if (ModelState.IsValid)
            {
                RateOfRoadORLoad heavyORDumpTruck = new RateOfRoadORLoad();
                heavyORDumpTruck.RoadOrLoadId = haeavyOrDumpTruckSelector.RoadOrLoadId;
                heavyORDumpTruck.RateOfRoadORLoadId = id;
                heavyORDumpTruck.Name = haeavyOrDumpTruckSelector.Name;
                heavyORDumpTruck.Rate = haeavyOrDumpTruckSelector.Rate;
                db.RateOfRoadORLoads.Add(heavyORDumpTruck);
                db.SaveChanges();


            }
            return Json(new[] { haeavyOrDumpTruckSelector }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult RatesOfLoadOrRoadSelector_Update([DataSourceRequest] DataSourceRequest request, RateOfRoadORLoad heavyORDumpTruck)
        {
            var FetchheavyorDumpTruck = db.RateOfRoadORLoads.SingleOrDefault(x => x.RateOfRoadORLoadId == heavyORDumpTruck.RateOfRoadORLoadId);
            if (FetchheavyorDumpTruck == null)
                ModelState.AddModelError("empty model", "No data with this id");
            if (ModelState.IsValid)
            {
                FetchheavyorDumpTruck.RateOfRoadORLoadId = heavyORDumpTruck.RateOfRoadORLoadId;
                FetchheavyorDumpTruck.RoadOrLoadId = heavyORDumpTruck.RoadOrLoadId;
                FetchheavyorDumpTruck.Name = heavyORDumpTruck.Name;
                FetchheavyorDumpTruck.Rate = heavyORDumpTruck.Rate;
                db.Entry(FetchheavyorDumpTruck).State = EntityState.Modified;
                db.SaveChanges();

            }
            return Json(new[] { heavyORDumpTruck }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult RatesOfLoadOrRoadSelector_Destroy([DataSourceRequest] DataSourceRequest request, RoadORLoad heavyORDumpTruck)
        {
            var FetchheavyorDumpTruck = db.RateOfRoadORLoads.SingleOrDefault(x => x.RateOfRoadORLoadId == heavyORDumpTruck.HeavyORDumpTruckId);
            if (FetchheavyorDumpTruck == null)
                ModelState.AddModelError("empty model", "No data with this id");
            if (ModelState.IsValid)
            {

                db.RateOfRoadORLoads.Remove(FetchheavyorDumpTruck);
                db.SaveChanges();

            }
            return Json(new[] { heavyORDumpTruck }.ToDataSourceResult(request, ModelState));

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}