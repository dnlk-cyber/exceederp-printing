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
    public class LoadOrRoadSelectorSelectorController : Controller
    {
        // GET: FleetManagement/HaeavyOrDumpTruckSelector
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LoadOrRoadSelector_Read([DataSourceRequest] DataSourceRequest request,int id)
        {
            var haeavyOrDumpTruckSelector = db.RoadORLoads.Where(x=>x.HeavyORDumpTruckId==id).AsEnumerable();
            DataSourceResult result = haeavyOrDumpTruckSelector.ToDataSourceResult(request, haeavyOrDumpTruckSelectors => new
            {
                RoadOrLoadId = haeavyOrDumpTruckSelectors.RoadOrLoadId,
                HeavyORDumpTruckId = haeavyOrDumpTruckSelectors.HeavyORDumpTruckId,
                Name = haeavyOrDumpTruckSelectors.Name,
                Description= haeavyOrDumpTruckSelectors.Description,
            });
           return Json(result);

        }
        public ActionResult LoadOrRoadSelector_Create([DataSourceRequest] DataSourceRequest request, RoadORLoad haeavyOrDumpTruckSelector, int id)
        {
            if (ModelState.IsValid)
            {
                RoadORLoad heavyORDumpTruck = new RoadORLoad();
                heavyORDumpTruck.RoadOrLoadId = haeavyOrDumpTruckSelector.RoadOrLoadId;
                heavyORDumpTruck.HeavyORDumpTruckId = id;
                heavyORDumpTruck.Name = haeavyOrDumpTruckSelector.Name;
                heavyORDumpTruck.Description = haeavyOrDumpTruckSelector.Description;
                db.RoadORLoads.Add(heavyORDumpTruck);
                db.SaveChanges();
                

            }
            return Json(new[] { haeavyOrDumpTruckSelector }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult LoadOrRoadSelector_Update([DataSourceRequest] DataSourceRequest request, RoadORLoad heavyORDumpTruck)
        {
            var FetchheavyorDumpTruck = db.RoadORLoads.SingleOrDefault(x => x.RoadOrLoadId == heavyORDumpTruck.RoadOrLoadId);
            if (FetchheavyorDumpTruck == null)
                ModelState.AddModelError("empty model", "No data with this id");
            if (ModelState.IsValid)
            {
                FetchheavyorDumpTruck.HeavyORDumpTruckId = heavyORDumpTruck.HeavyORDumpTruckId;
                FetchheavyorDumpTruck.RoadOrLoadId = heavyORDumpTruck.RoadOrLoadId;
                FetchheavyorDumpTruck.Name = heavyORDumpTruck.Name;
                FetchheavyorDumpTruck.Description = heavyORDumpTruck.Description;
                db.Entry(FetchheavyorDumpTruck).State = EntityState.Modified;
                db.SaveChanges();

            }
            return Json(new[] { heavyORDumpTruck }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult LoadOrRoadSelector_Destroy([DataSourceRequest] DataSourceRequest request, RoadORLoad heavyORDumpTruck)
        {
            var FetchheavyorDumpTruck = db.RoadORLoads.SingleOrDefault(x => x.RoadOrLoadId == heavyORDumpTruck.RoadOrLoadId);
            if (FetchheavyorDumpTruck == null)
                ModelState.AddModelError("empty model","No data with this id");
            if (ModelState.IsValid)
            {

                db.RoadORLoads.Remove(FetchheavyorDumpTruck);
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