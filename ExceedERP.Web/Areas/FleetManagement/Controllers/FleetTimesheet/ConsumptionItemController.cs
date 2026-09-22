using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetTimesheet
{
    public class ConsumptionItemController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/ConsumptionItem
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request,int id,string TimeSheetType)
        {
            var Items = new List<TimeSheetConsumptionItem>();
            if (TimeSheetType=="DumpTruck")
            {
                Items = db.TimeSheetConsumptionItem.Where(x => x.DumpTruckTimeSheetId == id).ToList();
            }if (TimeSheetType=="DryLoading")
            {
                Items = db.TimeSheetConsumptionItem.Where(x => x.DryLoadingTimeSheetId == id).ToList();
            }if (TimeSheetType=="Wagondrill")
            {
                Items = db.TimeSheetConsumptionItem.Where(x => x.WagonDrillTimesheetId == id).ToList();
            }if (TimeSheetType=="Machinery")
            {
                Items = db.TimeSheetConsumptionItem.Where(x => x.MachineryWorkingTimeSheetId == id).ToList();
            }if (TimeSheetType=="LightVehicle")
            {
                Items = db.TimeSheetConsumptionItem.Where(x => x.LightVehicleTimeSheetId == id).ToList();
            }
            var result = Items.ToDataSourceResult(request, x => new
            {
                DumpTruckTimeSheetId=x.DumpTruckTimeSheetId,
                TimeSheetConsumptionItemId = x.TimeSheetConsumptionItemId,
                LightVehicleTimeSheetId=x.LightVehicleTimeSheetId,
                DryLoadingTimeSheetId=x.DryLoadingTimeSheetId,
                MachineryWorkingTimeSheetId=x.MachineryWorkingTimeSheetId,
                WagonDrillTimesheetId=x.WagonDrillTimesheetId,
                OilType=x.OilType,
                OilConsumption=x.OilConsumption,
                OilCost = x.OilCost,
                CreatedBy=x.CreatedBy,
                DateCreated=x.DateCreated,
                ModifiedBy=x.ModifiedBy,
                LastModified= x.LastModified,

            });
            return Json(result);
        }

        public ActionResult Create([DataSourceRequest]DataSourceRequest request, TimeSheetConsumptionItem item,int id,string TimeSheetType)
        {
            if(TimeSheetType=="DumpTruck")
            {
                item.DumpTruckTimeSheetId = id;
                var parent = db.DumpTruckTimeSheets.FirstOrDefault(x => x.DumpTruckTimeSheetId == id);
                parent.OilCost += item.OilCost;
                parent.OilConsumption += item.OilConsumption;
                db.Entry(parent).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            if(TimeSheetType== "LightVehicle")
            {
                item.LightVehicleTimeSheetId = id;
                var parent = db.LightVehicleTimeSheets.FirstOrDefault(x => x.LightVehicleTimeSheetId == id);
                parent.OilCost += item.OilCost;
                parent.OilConsumption += item.OilConsumption;
                db.Entry(parent).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            if(TimeSheetType== "DryLoading")
            {
                item.DryLoadingTimeSheetId = id;
                var parent = db.DryLoadingTimeSheets.FirstOrDefault(x => x.DryLoadingTimeSheetId == id);
                parent.OilCost += item.OilCost;
                parent.OilConsumption += item.OilConsumption;
                db.Entry(parent).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            
            if(TimeSheetType== "Machinery")
            {
                item.MachineryWorkingTimeSheetId = id;
                var parent = db.MachineryWorkingTimeSheets.FirstOrDefault(x => x.MachineryWorkingTimeSheetId == item.MachineryWorkingTimeSheetId);
                parent.OilCost += item.OilCost;
                parent.OilConsumption += item.OilConsumption;
                db.Entry(parent).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            if (ModelState.IsValid)
            {
                db.TimeSheetConsumptionItem.Add(item);
                db.SaveChanges();
            }
            return Json(new[]{item}.ToDataSourceResult(request,ModelState));
        }

        public ActionResult Update([DataSourceRequest]DataSourceRequest request,TimeSheetConsumptionItem item,string TimeSheetType)
        {
            var existingConsumption = db.TimeSheetConsumptionItem.FirstOrDefault(x=>x.TimeSheetConsumptionItemId==item.TimeSheetConsumptionItemId);
            existingConsumption.OilType = item.OilType;
            existingConsumption.OilCost = item.OilCost;
            existingConsumption.OilConsumption = item.OilConsumption;
            
            if(ModelState.IsValid)
            {
                if (TimeSheetType == "DumpTruck")
                {
                    var parent = db.DumpTruckTimeSheets.FirstOrDefault(x => x.DumpTruckTimeSheetId == item.DumpTruckTimeSheetId);
                    parent.OilCost += item.OilCost;
                    parent.OilConsumption += item.OilConsumption;
                    db.Entry(parent).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (TimeSheetType == "DryLoading")
                {
                    var parent = db.DryLoadingTimeSheets.FirstOrDefault(x => x.DryLoadingTimeSheetId == item.DryLoadingTimeSheetId);
                    parent.OilCost += item.OilCost;
                    parent.OilConsumption += item.OilConsumption;
                    db.Entry(parent).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (TimeSheetType == "LightVehicle")
                {
                    var parent = db.LightVehicleTimeSheets.FirstOrDefault(x => x.LightVehicleTimeSheetId == item.LightVehicleTimeSheetId);

                    parent.GreaseConsumption += item.OilConsumption;
                    parent.OilCost += item.OilCost;
                    parent.OilConsumption += item.OilConsumption;
                    db.Entry(parent).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (TimeSheetType == "Machinery")
                {
                    var parent = db.MachineryWorkingTimeSheets.FirstOrDefault(x => x.MachineryWorkingTimeSheetId == item.MachineryWorkingTimeSheetId);
                    parent.OilCost += item.OilCost;
                    parent.OilConsumption += item.OilConsumption;
                    db.Entry(parent).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                db.Entry(existingConsumption).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { item}.ToDataSourceResult(request,ModelState));
        }
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request,TimeSheetConsumptionItem item,string TimeSheetType)
        {
            var existingConsumption = db.TimeSheetConsumptionItem.FirstOrDefault(x=>x.TimeSheetConsumptionItemId==item.TimeSheetConsumptionItemId);
            
            if(ModelState.IsValid)
            {
                if (TimeSheetType == "DumpTruck")
                {
                    var parent = db.DumpTruckTimeSheets.FirstOrDefault(x => x.DumpTruckTimeSheetId == item.DumpTruckTimeSheetId);

                    parent.OilCost += item.OilCost;
                    parent.OilConsumption += item.OilConsumption;
                    db.Entry(parent).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (TimeSheetType == "DryLoading")
                {
                    var parent = db.DryLoadingTimeSheets.FirstOrDefault(x => x.DryLoadingTimeSheetId == item.DryLoadingTimeSheetId);

                    parent.OilCost += item.OilCost;
                    parent.OilConsumption += item.OilConsumption;
                    db.Entry(parent).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                  if (TimeSheetType == "LightVehicle")
                {
                    var parent = db.LightVehicleTimeSheets.FirstOrDefault(x => x.LightVehicleTimeSheetId == item.LightVehicleTimeSheetId);

                    parent.OilCost += item.OilCost;
                    parent.OilConsumption += item.OilConsumption;
                    db.Entry(parent).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

                if (TimeSheetType == "Machinery")
                {
                    var parent = db.MachineryWorkingTimeSheets.FirstOrDefault(x => x.MachineryWorkingTimeSheetId == item.MachineryWorkingTimeSheetId);

                    parent.OilCost += item.OilCost;
                    parent.OilConsumption += item.OilConsumption;
                    db.Entry(parent).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                db.TimeSheetConsumptionItem.Remove(existingConsumption);
                db.SaveChanges();
            }

            return Json(new[] { item}.ToDataSourceResult(request,ModelState));
        }


    }
}