using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using System.Data.Entity;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    //[AuthorizeRoles(MaintenanceRoles.MaintenanceAdmin)]  
    public class MaintenanceCostDetailController : Controller
    {
        IRepairTypeService _RepairTypeService;
        ExceedDbContext db = new ExceedDbContext();

       
        
        public ActionResult Index()
        {
           
            return View();
        }

        public JsonResult WagonDrill_Read([DataSourceRequest] DataSourceRequest request,int id)
        {
            try
            {
                var data = db.MaintenanceCostDetail.Where(x=>x.TimeSheetId==id&&x.IsWagonDrill==true)
                    .Select(o => new
                    {
                        o.MaintenanceCostDetailId,
                        o.TimeSheetId,
                        o.RepairTypeId,
                        o.Cost,
                        o.SparePartCost,
                        o.FuelConsumption,
                        o.FuelCost,
                        o.GreaseCost,
                        o.LabourCost,
                        o.MiscellaneousCost,
                        o.MechanicName,
                        o.TimeTaken,
                        o.IsDryLoading,
                        o.IsDumpTruck,
                        o.IsLightVehicle,
                        o.IsMachinery,
                        o.IsWagonDrill,
                        o.OtherCost,
                        o.GreaseConsumption,
                        o.ServiceCost,
                        // SparePartCost OtherCost GreaseConsumption GreaseCost ServiceCost LabourCost
                    });

                return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log exception (recommended)
                return Json(new { success = false, message = ex.Message });
            }
        } public JsonResult DryLoading_Read([DataSourceRequest] DataSourceRequest request,int id)
        {
            try
            {
                var data = db.MaintenanceCostDetail.Where(x=>x.TimeSheetId==id&&x.IsDumpTruck==true)
                    .Select(o => new
                    {
                        o.MaintenanceCostDetailId,
                        o.TimeSheetId,
                        o.RepairTypeId,
                        o.Cost,
                        o.SparePartCost,
                        o.FuelConsumption,
                        o.FuelCost,
                        o.GreaseCost,
                        o.LabourCost,
                        o.MiscellaneousCost,
                        o.MechanicName,
                        o.TimeTaken,
                        o.IsDryLoading,
                        o.IsDumpTruck,
                        o.IsLightVehicle,
                        o.IsMachinery,
                        o.IsWagonDrill,
                        o.OtherCost,
                        o.GreaseConsumption,
                        o.ServiceCost,

                    });

                return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log exception (recommended)
                return Json(new { success = false, message = ex.Message });
            }
        }
        public JsonResult DumpTruck_Read([DataSourceRequest] DataSourceRequest request,int id)
        {
            try
            {
                var data = db.MaintenanceCostDetail.Where(x=>x.TimeSheetId==id&&x.IsDumpTruck==true)
                    .Select(o => new
                    {
                        o.MaintenanceCostDetailId,
                        o.TimeSheetId,
                        o.RepairTypeId,
                        o.Cost,
                        o.SparePartCost,
                        o.FuelConsumption,
                        o.FuelCost,
                        o.GreaseCost,
                        o.LabourCost,
                        o.MiscellaneousCost,
                        o.MechanicName,
                        o.TimeTaken,
                        o.IsDryLoading,
                        o.IsDumpTruck,
                        o.IsLightVehicle,
                        o.IsMachinery,
                        o.IsWagonDrill,
                        o.OtherCost,
                        o.GreaseConsumption,
                        o.ServiceCost,

                    });

                return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log exception (recommended)
                return Json(new { success = false, message = ex.Message });
            }
        } public JsonResult Machinery_Read([DataSourceRequest] DataSourceRequest request,int id)
        {
            try
            {
                var data = db.MaintenanceCostDetail.Where(x=>x.TimeSheetId==id&&x.IsMachinery==true)
                    .Select(o => new
                    {
                        o.MaintenanceCostDetailId,
                        o.TimeSheetId,
                        o.RepairTypeId,
                        o.Cost,
                        o.SparePartCost,
                        o.FuelConsumption,
                        o.FuelCost,
                        o.GreaseCost,
                        o.LabourCost,
                        o.MiscellaneousCost,
                        o.MechanicName,
                        o.TimeTaken,
                        o.IsDryLoading,
                        o.IsDumpTruck,
                        o.IsLightVehicle,
                        o.IsMachinery,
                        o.IsWagonDrill,
                        o.OtherCost,
                        o.GreaseConsumption,
                        o.ServiceCost,

                    });

                return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log exception (recommended)
                return Json(new { success = false, message = ex.Message });
            }
        } public JsonResult LightVehicle_Read([DataSourceRequest] DataSourceRequest request,int id)
        {
            try
            {
                var data = db.MaintenanceCostDetail.Where(x=>x.TimeSheetId==id&&x.IsLightVehicle==true)
                    .Select(o => new
                    {
                        o.MaintenanceCostDetailId,
                        o.TimeSheetId,
                        o.RepairTypeId,
                        o.Cost,
                        o.SparePartCost,
                        o.FuelConsumption,
                        o.FuelCost,
                        o.GreaseCost,
                        o.LabourCost,
                        o.MiscellaneousCost,
                        o.MechanicName,
                        o.TimeTaken,
                        o.IsDryLoading,
                        o.IsDumpTruck,
                        o.IsLightVehicle,
                        o.IsMachinery,
                        o.IsWagonDrill,
                        o.OtherCost,
                        o.GreaseConsumption,
                        o.ServiceCost,

                    });

                return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log exception (recommended)
                return Json(new { success = false, message = ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RepairType_Create([DataSourceRequest] DataSourceRequest request, MaintenanceCostDetail RepairType,int id, string Text)
        {
            RepairType.Cost = RepairType.SparePartCost +RepairType.FuelCost + RepairType.MiscellaneousCost + RepairType.GreaseCost + RepairType.LabourCost + RepairType.OtherCost + RepairType.ServiceCost;
            if (Text == "LightVehicle")
            {
                RepairType.IsLightVehicle = true;
                var LightMaintenance = db.LightVehicleTimeSheets.FirstOrDefault(x => x.LightVehicleTimeSheetId == id);
                 LightMaintenance.MaintenanceCost += RepairType.Cost;
                db.Entry(LightMaintenance).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (Text == "DumpTruck")
            {
                RepairType.IsDumpTruck = true;
                var LightMaintenance = db.DumpTruckTimeSheets.FirstOrDefault(x => x.DumpTruckTimeSheetId == id);
                 LightMaintenance.MaintenanceCost += RepairType.Cost;
                db.Entry(LightMaintenance).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (Text == "DryLoading")
            {
                RepairType.IsDryLoading = true;
                var LightMaintenance = db.DryLoadingTimeSheets.FirstOrDefault(x => x.DryLoadingTimeSheetId == id);
                LightMaintenance.MaintenanceCost += RepairType.Cost;
                db.Entry(LightMaintenance).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (Text == "Machinery")
            {
                RepairType.IsMachinery = true;
                var LightMaintenance = db.MachineryWorkingTimeSheets.FirstOrDefault(x => x.MachineryWorkingTimeSheetId == id);
                LightMaintenance.MaintenanceCost += RepairType.Cost;
                db.Entry(LightMaintenance).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (Text == "WagonDrill")
            {
                RepairType.IsWagonDrill = true;
                var LightMaintenance = db.WagonDrillTimesheets.FirstOrDefault(x => x.WagonDrillTimesheetId == id);
                LightMaintenance.MaintenanceCost += RepairType.Cost;
                db.Entry(LightMaintenance).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (ModelState.IsValid)
            {

                var entity = new MaintenanceCostDetail
                {
                   TimeSheetId=id,
                    SparePartCost = RepairType.SparePartCost,
                    GreaseCost = RepairType.GreaseCost,
                    GreaseConsumption = RepairType.GreaseConsumption,
                    LabourCost = RepairType.LabourCost,
                    ServiceCost = RepairType.ServiceCost,
                    FuelCost = RepairType.FuelCost,
                    FuelConsumption = RepairType.FuelConsumption,
                    OtherCost = RepairType.OtherCost,
                    MiscellaneousCost = RepairType.MiscellaneousCost,
                   Cost=RepairType.Cost,
                   RepairTypeId=RepairType.RepairTypeId,
                    MechanicName=RepairType.MechanicName,
                    TimeTaken=RepairType.TimeTaken,
                    IsDumpTruck = RepairType.IsDumpTruck,
                    IsDryLoading = RepairType.IsDryLoading,
                   IsLightVehicle=RepairType.IsLightVehicle,
                   IsMachinery=RepairType.IsMachinery,
                   IsWagonDrill=RepairType.IsWagonDrill,
                };
                
                db.MaintenanceCostDetail.Add(entity);
                db.SaveChanges();
                
            }
            
            
            return this.Json(new[] { RepairType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RepairType_Update([DataSourceRequest] DataSourceRequest request, MaintenanceCostDetail RepairType,string Text)
        {
            RepairType.Cost = RepairType.SparePartCost + RepairType.GreaseCost +RepairType.FuelCost + RepairType.LabourCost + RepairType.OtherCost+RepairType.MiscellaneousCost + RepairType.ServiceCost;

            var existingDetail = db.MaintenanceCostDetail.FirstOrDefault(x => x.MaintenanceCostDetailId == RepairType.MaintenanceCostDetailId);
            if (Text == "LightVehicle")
            {
                var LightMaintenance = db.LightVehicleTimeSheets.FirstOrDefault(x => x.LightVehicleTimeSheetId == RepairType.TimeSheetId);
                LightMaintenance.MaintenanceCost += RepairType.Cost;
                LightMaintenance.MaintenanceCost -= existingDetail.Cost;
                db.Entry(LightMaintenance).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (Text == "DumpTruck")
            {
                var LightMaintenance = db.DumpTruckTimeSheets.FirstOrDefault(x => x.DumpTruckTimeSheetId == RepairType.TimeSheetId);
                LightMaintenance.MaintenanceCost += RepairType.Cost;
                LightMaintenance.MaintenanceCost -= existingDetail.Cost;
                db.Entry(LightMaintenance).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (Text == "DryLoading")
            {
                var LightMaintenance = db.DryLoadingTimeSheets.FirstOrDefault(x => x.DryLoadingTimeSheetId == RepairType.TimeSheetId);
                LightMaintenance.MaintenanceCost += RepairType.Cost;
                LightMaintenance.MaintenanceCost -= existingDetail.Cost;
                db.Entry(LightMaintenance).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (Text == "Machinery")
            {
                var LightMaintenance = db.MachineryWorkingTimeSheets.FirstOrDefault(x => x.MachineryWorkingTimeSheetId == RepairType.TimeSheetId);
                LightMaintenance.MaintenanceCost += RepairType.Cost;
                LightMaintenance.MaintenanceCost -= existingDetail.Cost;
                db.Entry(LightMaintenance).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (Text == "WagonDrill")
            {
                var LightMaintenance = db.WagonDrillTimesheets.FirstOrDefault(x => x.WagonDrillTimesheetId == RepairType.TimeSheetId);
                LightMaintenance.MaintenanceCost += RepairType.Cost;
                LightMaintenance.MaintenanceCost -= existingDetail.Cost;
                db.Entry(LightMaintenance).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (ModelState.IsValid)
            {
                existingDetail.SparePartCost = RepairType.SparePartCost;
                existingDetail.GreaseCost = RepairType.GreaseCost;
                existingDetail.GreaseConsumption = RepairType.GreaseConsumption;
                existingDetail.LabourCost = RepairType.LabourCost;
                existingDetail.ServiceCost = RepairType.ServiceCost;
                existingDetail.FuelCost = RepairType.FuelCost;
                existingDetail.FuelConsumption = RepairType.FuelConsumption;
                existingDetail.OtherCost = RepairType.OtherCost;
                existingDetail.MiscellaneousCost = RepairType.MiscellaneousCost;
                existingDetail.Cost = RepairType.Cost;
                existingDetail.MechanicName = RepairType.MechanicName;
                existingDetail.TimeTaken = RepairType.TimeTaken;
                existingDetail.RepairTypeId = RepairType.RepairTypeId;
                db.Entry(existingDetail).State = EntityState.Modified;
                db.SaveChanges();
            }


            return this.Json(new[] { RepairType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RepairType_Destroy([DataSourceRequest] DataSourceRequest request, MaintenanceCostDetail RepairType,string Text)
        {

            var existingDetail = db.MaintenanceCostDetail.FirstOrDefault(x => x.MaintenanceCostDetailId == RepairType.MaintenanceCostDetailId);
            //if (Text == "LightVehicle")
            //{
            //    var LightMaintenance = db.LightVehicleTimeSheets.FirstOrDefault(x => x.LightVehicleTimeSheetId == RepairType.TimeSheetId);
            //    LightMaintenance.MaintenanceCost -= existingDetail.Cost;
            //    db.Entry(LightMaintenance).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            //if (Text == "DumpTruck")
            //{
            //    var LightMaintenance = db.DumpTruckTimeSheets.FirstOrDefault(x => x.DumpTruckTimeSheetId == RepairType.TimeSheetId);
            //    LightMaintenance.MaintenanceCost -= existingDetail.Cost;
            //    db.Entry(LightMaintenance).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            //if (Text == "DryLoading")
            //{
            //    var LightMaintenance = db.DryLoadingTimeSheets.FirstOrDefault(x => x.DryLoadingTimeSheetId == RepairType.TimeSheetId);
            //    LightMaintenance.MaintenanceCost -= existingDetail.Cost;
            //    db.Entry(LightMaintenance).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            //if (Text == "Machinery")
            //{
            //    var LightMaintenance = db.MachineryWorkingTimeSheets.FirstOrDefault(x => x.MachineryWorkingTimeSheetId == RepairType.TimeSheetId);
            //    LightMaintenance.MaintenanceCost -= existingDetail.Cost;
            //    db.Entry(LightMaintenance).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            //if (Text == "WagonDrill")
            //{
            //    var LightMaintenance = db.WagonDrillTimesheets.FirstOrDefault(x => x.WagonDrillTimesheetId == RepairType.TimeSheetId);
            //    LightMaintenance.MaintenanceCost -= existingDetail.Cost;
            //    db.Entry(LightMaintenance).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            if (ModelState.IsValid)
            {
                db.MaintenanceCostDetail.Remove(existingDetail);
                db.SaveChanges();
            }

            return this.Json(new[] { RepairType }.ToDataSourceResult(request));
        }
        
        public ActionResult RepairTypeDDBind()
        {

            var fleets = _RepairTypeService.GetAll().Select(s => new
            {
                Name = s.Code + " - " + s.Name,
                Value = s.RepairTypeID
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}