using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    //[AuthorizeRoles(FleetManagementRoles.MileageImportAndExport, FleetManagementRoles.MaintenanceEquipmentRegistrationUser)]
    public class MaintenanceIntervalController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Intervals_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MaintenanceInterval> equipments = db.MaintenanceIntervals;
            DataSourceResult result = equipments.ToDataSourceResult(request, interval => new
            {
                MaintenanceIntervalId = interval.MaintenanceIntervalId,
                Type = interval.Type,
                Value = interval.Value,

                DateCreated = interval.DateCreated,
                LastModified = interval.LastModified,
                CreatedBy = interval.CreatedBy,
                ModifiedBy = interval.ModifiedBy
            });

            return Json(result);
        }
        private bool IntervalSettingExists(MaintenanceInterval model)
        {
            return db.MaintenanceIntervals.FirstOrDefault(me => me.Type == model.Type && me.MaintenanceIntervalId != model.MaintenanceIntervalId) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Interval_Create([DataSourceRequest]DataSourceRequest request, MaintenanceInterval interval)
        {
            ModelState.Remove("MaintenanceIntervalId");
            if (ModelState.IsValid)
            {
                if (IntervalSettingExists(interval))
                {
                    ModelState.AddModelError("MaintenanceIntervalId", "Duplicate Setting not allowed!");
                }
                else
                {
                    var entity = new MaintenanceInterval
                    {
                        MaintenanceIntervalId = Guid.NewGuid(),
                        Type = interval.Type,
                        Value = interval.Value,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };

                    db.MaintenanceIntervals.Add(entity);
                    db.SaveChanges();
                    interval.MaintenanceIntervalId = entity.MaintenanceIntervalId;

                    this.UpdateAllEquipmentMaintenanceIntervalOnIntervalCreate(entity);
                }
            }

            return Json(new[] { interval }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Interval_Update([DataSourceRequest]DataSourceRequest request, MaintenanceInterval interval)
        {
            if (ModelState.IsValid)
            {
                var priorValue = new decimal();
                var entityBeforeUpdate = db.MaintenanceIntervals.Find(interval.MaintenanceIntervalId);
                if(entityBeforeUpdate != null)
                {
                    priorValue = entityBeforeUpdate.Value;

                    if (IntervalSettingExists(interval))
                    {
                        ModelState.AddModelError("MaintenanceIntervalId", "Duplicate Setting not allowed!");
                    }
                    else
                    {
                        //var entity = new MaintenanceInterval
                        //{
                        //    MaintenanceIntervalId = interval.MaintenanceIntervalId,
                        //    Type = interval.Type,
                        //    Value = interval.Value,

                        //    DateCreated = interval.DateCreated,
                        //    CreatedBy = interval.CreatedBy,
                        //    LastModified = DateTime.Now,
                        //    ModifiedBy = User.Identity.Name
                        //};

                        entityBeforeUpdate.Type = interval.Type;
                        entityBeforeUpdate.Value = interval.Value;

                        entityBeforeUpdate.LastModified = DateTime.Now;
                        entityBeforeUpdate.ModifiedBy = User.Identity.Name;

                        db.MaintenanceIntervals.Attach(entityBeforeUpdate);
                        db.Entry(entityBeforeUpdate).State = EntityState.Modified;
                        db.SaveChanges();

                        this.UpdateAllEquipmentMaintenanceIntervalOnIntervalUpdate(entityBeforeUpdate, priorValue);
                    }
                }
                
            }

            return Json(new[] { interval }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Interval_Destroy([DataSourceRequest]DataSourceRequest request, MaintenanceInterval interval)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaintenanceInterval
                {
                    MaintenanceIntervalId = interval.MaintenanceIntervalId,
                    Type = interval.Type,
                    Value = interval.Value,

                    DateCreated = interval.DateCreated,
                    LastModified = interval.LastModified,
                    CreatedBy = interval.CreatedBy,
                    ModifiedBy = interval.ModifiedBy
                };

                db.MaintenanceIntervals.Attach(entity);
                db.MaintenanceIntervals.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { interval }.ToDataSourceResult(request, ModelState));
        }
        private void UpdateAllEquipmentMaintenanceIntervalOnIntervalCreate(MaintenanceInterval record)
        {
            var equipments = db.EquipmentInformation.Where(e => e.RegularMaintenanceInterval == 0 || e.NotifyUpComingMaintenanceBefore == 0).ToList();
            foreach (EquipmentInformation equipment in equipments)
            {
                if (equipment.RegularMaintenanceInterval == 0 && record.Type == MaintenanceIntervalType.RegularMaintenanceInterval)
                {
                    equipment.RegularMaintenanceInterval = record.Value;

                    equipment.OdometerOnNextRegularMaintenance = equipment.OdometerOnLastMaintenance + record.Value;
                    equipment.RemainingKmForUpComingMaintenance = equipment.OdometerOnNextRegularMaintenance - equipment.CurrentOdometer;
                }
                if (equipment.NotifyUpComingMaintenanceBefore == 0 && record.Type == MaintenanceIntervalType.NotifyUpComingMaintenanceBefore)
                {
                    equipment.NotifyUpComingMaintenanceBefore = record.Value;

                    if (equipment.RemainingKmForUpComingMaintenance <= record.Value)
                    {
                        equipment.FutureMaintenanceAlert = FutureMaintenanceAlert.On;
                    }
                }
                db.EquipmentInformation.Attach(equipment);
                db.Entry(equipment).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        private void UpdateAllEquipmentMaintenanceIntervalOnIntervalUpdate(MaintenanceInterval record, decimal previousValue)
        {
            var equipments = db.EquipmentInformation
                .Where(e => e.RegularMaintenanceInterval == previousValue ||
                            e.NotifyUpComingMaintenanceBefore == previousValue ||
                            e.RegularMaintenanceInterval == 0 ||
                            e.NotifyUpComingMaintenanceBefore == 0
                        ).ToList();
            foreach (EquipmentInformation equipment in equipments)
            {
                if (record.Type == MaintenanceIntervalType.RegularMaintenanceInterval &&(equipment.RegularMaintenanceInterval == 0 || equipment.RegularMaintenanceInterval == previousValue))
                {
                    equipment.RegularMaintenanceInterval = record.Value;

                    equipment.OdometerOnNextRegularMaintenance = equipment.OdometerOnLastMaintenance + record.Value;
                    equipment.RemainingKmForUpComingMaintenance = equipment.OdometerOnNextRegularMaintenance - equipment.CurrentOdometer;
                }
                if (record.Type == MaintenanceIntervalType.NotifyUpComingMaintenanceBefore && (equipment.NotifyUpComingMaintenanceBefore == 0 || equipment.NotifyUpComingMaintenanceBefore == previousValue))
                {
                    equipment.NotifyUpComingMaintenanceBefore = record.Value;

                    if (equipment.RemainingKmForUpComingMaintenance <= record.Value)
                    {
                        equipment.FutureMaintenanceAlert = FutureMaintenanceAlert.On;
                    }
                }
                db.EquipmentInformation.Attach(equipment);
                db.Entry(equipment).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}