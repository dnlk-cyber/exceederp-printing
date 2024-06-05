﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Maintenance.Settings;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.VehicleReception
{
    public class VehiclePartDamageTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetVehiclePartDamageTypes()
        {
            var data = db.VehiclePartDamageType.Select(s =>
            new {
                s.VehiclePartDamageTypeId,
                s.Name
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VehiclePartDamageType_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<VehiclePartDamageType> vehiclepartdamagetype = db.VehiclePartDamageType;
            DataSourceResult result = vehiclepartdamagetype.ToDataSourceResult(request, vehiclePartDamageType => new {
                VehiclePartDamageTypeId = vehiclePartDamageType.VehiclePartDamageTypeId,
                Code = vehiclePartDamageType.Code,
                Name = vehiclePartDamageType.Name,
                Remark = vehiclePartDamageType.Remark,
                Description = vehiclePartDamageType.Description,
                DateCreated = vehiclePartDamageType.DateCreated,
                LastModified = vehiclePartDamageType.LastModified,
                CreatedBy = vehiclePartDamageType.CreatedBy,
                ModifiedBy = vehiclePartDamageType.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult VehiclePartDamageType_Create([DataSourceRequest]DataSourceRequest request, VehiclePartDamageType VehiclePartDamageType)
        {
            if (ModelState.IsValid)
            {
                var entity = (VehiclePartDamageType)this.GetObject(VehiclePartDamageType);
                db.VehiclePartDamageType.Add(entity);
                db.SaveChanges();
                VehiclePartDamageType.VehiclePartDamageTypeId = entity.VehiclePartDamageTypeId;
            }

            return Json(new[] { VehiclePartDamageType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehiclePartDamageType_Update([DataSourceRequest]DataSourceRequest request, VehiclePartDamageType VehiclePartDamageType)
        {
            if (ModelState.IsValid)
            {
                var entity = (VehiclePartDamageType)this.GetObject(VehiclePartDamageType);

                db.VehiclePartDamageType.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { VehiclePartDamageType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehiclePartDamageType_Destroy([DataSourceRequest]DataSourceRequest request, VehiclePartDamageType VehiclePartDamageType)
        {

            if (ModelState.IsValid)
            {
                var entity = (VehiclePartDamageType)this.GetObject(VehiclePartDamageType);

                db.VehiclePartDamageType.Attach(entity);
                db.VehiclePartDamageType.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { VehiclePartDamageType }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(VehiclePartDamageType vehiclePartDamageType)
        {
            var entity = new VehiclePartDamageType
            {
                VehiclePartDamageTypeId = vehiclePartDamageType.VehiclePartDamageTypeId,
                Name = vehiclePartDamageType.Name,
                Code = vehiclePartDamageType.Code,
                Remark = vehiclePartDamageType.Remark,
                Description = vehiclePartDamageType.Description,
                DateCreated = vehiclePartDamageType.DateCreated,
                LastModified = vehiclePartDamageType.LastModified,
                CreatedBy = vehiclePartDamageType.CreatedBy,
                ModifiedBy = vehiclePartDamageType.ModifiedBy

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
