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
    public class HaeavyOrDumpTruckSelectorController : Controller
    {
        // GET: FleetManagement/HaeavyOrDumpTruckSelector
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult HaeavyOrDumpTruckSelector_Read([DataSourceRequest] DataSourceRequest request)
        {
            var haeavyOrDumpTruckSelector = db.HaeavyOrDumpTruckSelectors;
            DataSourceResult result = haeavyOrDumpTruckSelector.ToDataSourceResult(request, haeavyOrDumpTruckSelectors => new
            {
                HeavyORDumpTruckId = haeavyOrDumpTruckSelectors.HeavyORDumpTruckId,
                Name = haeavyOrDumpTruckSelectors.Name,
                Description= haeavyOrDumpTruckSelectors.Description,
            });
           return Json(result);

        }
        public ActionResult HaeavyOrDumpTruckSelector_Create([DataSourceRequest] DataSourceRequest request, HeavyORDumpTruck heavyORDumpTruck)
        {
            if (ModelState.IsValid)
            {
                HeavyORDumpTruck heavyorDumpTruck = new HeavyORDumpTruck();
                heavyorDumpTruck.HeavyORDumpTruckId = heavyORDumpTruck.HeavyORDumpTruckId;
                heavyorDumpTruck.Name = heavyORDumpTruck.Name;
                heavyorDumpTruck.Description = heavyORDumpTruck.Description;
                db.HaeavyOrDumpTruckSelectors.Add(heavyorDumpTruck);
                db.SaveChanges();
                

            }
            return Json(new[] { heavyORDumpTruck }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult HaeavyOrDumpTruckSelector_Update([DataSourceRequest] DataSourceRequest request, HeavyORDumpTruck heavyORDumpTruck)
        {
            var FetchheavyorDumpTruck = db.HaeavyOrDumpTruckSelectors.SingleOrDefault(x => x.HeavyORDumpTruckId == heavyORDumpTruck.HeavyORDumpTruckId);
            if (FetchheavyorDumpTruck == null)
                ModelState.AddModelError("empty model", "No data with this id");
            if (ModelState.IsValid)
            {
                FetchheavyorDumpTruck.HeavyORDumpTruckId = heavyORDumpTruck.HeavyORDumpTruckId;
                FetchheavyorDumpTruck.Name = heavyORDumpTruck.Name;
                FetchheavyorDumpTruck.Description = heavyORDumpTruck.Description;
                db.Entry(FetchheavyorDumpTruck).State = EntityState.Modified;
                db.SaveChanges();

            }
            return Json(new[] { heavyORDumpTruck }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult HaeavyOrDumpTruckSelector_Destroy([DataSourceRequest] DataSourceRequest request, HeavyORDumpTruck heavyORDumpTruck)
        {
            var FetchheavyorDumpTruck = db.HaeavyOrDumpTruckSelectors.SingleOrDefault(x => x.HeavyORDumpTruckId == heavyORDumpTruck.HeavyORDumpTruckId);
            if (FetchheavyorDumpTruck == null)
                ModelState.AddModelError("empty model","No data with this id");
            if (ModelState.IsValid)
            {

                db.HaeavyOrDumpTruckSelectors.Remove(FetchheavyorDumpTruck);
                db.SaveChanges();

            }
            return Json(new[] { heavyORDumpTruck }.ToDataSourceResult(request, ModelState));

        }


    }
}