﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.VehicleReception
{
    public class FleetVehiclePartManagementController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FleetVehiclePartManagement_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<VehiclePartManagement> vehiclepartmanagement = db.VehiclePartManagement.Where(q=>q.VehicleReceptionFormId == id);
            DataSourceResult result = vehiclepartmanagement.ToDataSourceResult(request, vehiclePartManagement => new {
                VehiclePartManagementId = vehiclePartManagement.VehiclePartManagementId,
                VehicleReceptionFormId = vehiclePartManagement.VehicleReceptionFormId,
                VehiclePartDamageTypeId = vehiclePartManagement.VehiclePartDamageTypeId,
                VehiclePartId = vehiclePartManagement.VehiclePartId,
                Quantity = vehiclePartManagement.Quantity,
                Remark = vehiclePartManagement.Remark,
                DateCreated = vehiclePartManagement.DateCreated,
                LastModified = vehiclePartManagement.LastModified,
                CreatedBy = vehiclePartManagement.CreatedBy,
                ModifiedBy = vehiclePartManagement.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult FleetVehiclePartManagement_Create([DataSourceRequest]DataSourceRequest request, VehiclePartManagement VehiclePartManagement, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = (VehiclePartManagement)this.GetObject(VehiclePartManagement,true,id);
                db.VehiclePartManagement.Add(entity);
                db.SaveChanges();
                VehiclePartManagement.VehiclePartManagementId = entity.VehiclePartManagementId;
            }

            return Json(new[] { VehiclePartManagement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetVehiclePartManagement_Update([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<VehiclePartManagement> vehiclePartManagements)
        {
            if (ModelState.IsValid)
            {
                foreach (var vehiclePartManagement in vehiclePartManagements)
                {
                    var entity = (VehiclePartManagement)this.GetObject(vehiclePartManagement, false);
                    db.VehiclePartManagement.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                }

                db.SaveChanges();

            }
          
            

            return Json(new[] { vehiclePartManagements }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetVehiclePartManagement_Destroy([DataSourceRequest]DataSourceRequest request, VehiclePartManagement VehiclePartManagement)
        {

            if (ModelState.IsValid)
            {
                var entity = (VehiclePartManagement)this.GetObject(VehiclePartManagement,false);

                db.VehiclePartManagement.Attach(entity);
                db.VehiclePartManagement.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { VehiclePartManagement }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(VehiclePartManagement vehiclePartManagement, bool status, int id = 0)
        {
            if (status)
            {
                vehiclePartManagement.VehicleReceptionFormId = id;
            }
            var entity = new VehiclePartManagement
            {
                VehiclePartManagementId = vehiclePartManagement.VehiclePartManagementId,
                VehicleReceptionFormId = vehiclePartManagement.VehicleReceptionFormId,
                VehiclePartDamageTypeId = vehiclePartManagement.VehiclePartDamageTypeId,
                VehiclePartId = vehiclePartManagement.VehiclePartId,
                Quantity = vehiclePartManagement.Quantity,
                Remark = vehiclePartManagement.Remark,
                DateCreated = vehiclePartManagement.DateCreated,
                LastModified = vehiclePartManagement.LastModified,
                CreatedBy = vehiclePartManagement.CreatedBy,
                ModifiedBy = vehiclePartManagement.ModifiedBy

            };
            return entity;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
