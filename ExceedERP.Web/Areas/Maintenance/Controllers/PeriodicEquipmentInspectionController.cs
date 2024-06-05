using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using System.Data.Entity;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
     // [AuthorizeRoles(FleetManagementRoles.PeriodicEquipmentInspectionUser)]  
    public class PeriodicEquipmentInspectionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/PeriodicEquipmentInspection
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Inspections_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PeriodicEquipmentInspection> tools = db.PeriodicEquipmentInspection;
            DataSourceResult result = tools.ToDataSourceResult(request, inspection => new
            {
                PeriodicEquipmentInspectionId = inspection.PeriodicEquipmentInspectionId,
                Date = inspection.Date,
                Owner = inspection.Owner,
                PlateNumber = inspection.PlateNumber,
                Capacity = inspection.Capacity,
                AmountPaid = inspection.AmountPaid,
                VehicleType = inspection.VehicleType,
                Remark = inspection.Remark,

                DateCreated = inspection.DateCreated,
                LastModified = inspection.LastModified,
                CreatedBy = inspection.CreatedBy,
                ModifiedBy = inspection.ModifiedBy
            });

            return Json(result);
        }
        //private bool CategoryExists(PeriodicEquipmentInspection model)
        //{
        //    return db.PeriodicEquipmentInspection.FirstOrDefault(me => me.MaintenanceToolID == model.MaintenanceToolID && 
        //    me.MechanicCrewID == model.MechanicCrewID && me.SpareMaterialCategoryId != model.SpareMaterialCategoryId) != null;
        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Inspection_Create([DataSourceRequest]DataSourceRequest request, PeriodicEquipmentInspection inspection)
        {
            //ModelState.Remove("SpareMaterialCategoryId");
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                //if (CategoryExists(inspection))
                //{
                //    ModelState.AddModelError("SpareMaterialCategortyId", "Duplicate inspection not allowed!");
                //}
                //else
                //{
                    var entity = new PeriodicEquipmentInspection
                    {
                        PeriodicEquipmentInspectionId = Guid.NewGuid(),
                        Date = inspection.Date,
                        Owner = inspection.Owner,
                        PlateNumber = inspection.PlateNumber,
                        Capacity = inspection.Capacity,
                        AmountPaid = inspection.AmountPaid,
                        VehicleType = inspection.VehicleType,
                        Remark = inspection.Remark,


                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };

                    db.PeriodicEquipmentInspection.Add(entity);
                    db.SaveChanges();
                    inspection.PeriodicEquipmentInspectionId = entity.PeriodicEquipmentInspectionId;

                    //this.UpdateAllEquipmentMaintenanceIntervalOnIntervalCreate(entity);
                //}
            }

            return Json(new[] { inspection }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Inspection_Update([DataSourceRequest]DataSourceRequest request, PeriodicEquipmentInspection inspection)
        {
            if (ModelState.IsValid)
            {


                //if (CategoryExists(inspection))
                //{
                //    ModelState.AddModelError("SpareMaterialCategoryId", "Duplicate inspection not allowed!");
                //}
                //else
                //{
                    var entity = new PeriodicEquipmentInspection
                    {

                        PeriodicEquipmentInspectionId = inspection.PeriodicEquipmentInspectionId,
                        Date = inspection.Date,
                        Owner = inspection.Owner,
                        PlateNumber = inspection.PlateNumber,
                        Capacity = inspection.Capacity,
                        AmountPaid = inspection.AmountPaid,
                        VehicleType = inspection.VehicleType,
                        Remark = inspection.Remark,


                        DateCreated = inspection.DateCreated,
                        CreatedBy = inspection.CreatedBy,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };



                    db.PeriodicEquipmentInspection.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                //}

            }

            return Json(new[] { inspection }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Inspection_Destroy([DataSourceRequest]DataSourceRequest request, PeriodicEquipmentInspection inspection)
        {
            if (ModelState.IsValid)
            {
                var entity = new PeriodicEquipmentInspection
                {
                    PeriodicEquipmentInspectionId = inspection.PeriodicEquipmentInspectionId,
                    Date = inspection.Date,
                    Owner = inspection.Owner,
                    PlateNumber = inspection.PlateNumber,
                    Capacity = inspection.Capacity,
                    AmountPaid = inspection.AmountPaid,
                    VehicleType = inspection.VehicleType,
                    Remark = inspection.Remark,


                    DateCreated = inspection.DateCreated,
                    LastModified = inspection.LastModified,
                    CreatedBy = inspection.CreatedBy,
                    ModifiedBy = inspection.ModifiedBy
                };

                db.PeriodicEquipmentInspection.Attach(entity);
                db.PeriodicEquipmentInspection.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { inspection }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}

