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
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.VehicleReception
{
    public class VehiclePartController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        public ActionResult Index()
        {
           
        
            ViewData["category"] = CacheData.GetEquipmentIdentityCaches().Select(
       i => new
       {
           Name = i.Name,
           Id = i.Id
       }).AsQueryable();
            return View();
        }

        public ActionResult VehiclePart_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<VehiclePart> vehiclepart = db.VehiclePart;
            DataSourceResult result = vehiclepart.ToDataSourceResult(request, vehiclePart => new {
                VehiclePartId = vehiclePart.VehiclePartId,
                Code = vehiclePart.Code,
                VehicleCategoryId = vehiclePart.VehicleCategoryId,
                Name = vehiclePart.Name,
                Remark = vehiclePart.Remark,
                Description = vehiclePart.Description,
                DateCreated = vehiclePart.DateCreated,
                LastModified = vehiclePart.LastModified,
                CreatedBy = vehiclePart.CreatedBy,
                ApplyForAll = vehiclePart.ApplyForAll,
                ModifiedBy = vehiclePart.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult VehiclePart_Create([DataSourceRequest]DataSourceRequest request, VehiclePart VehiclePart)
        {
            if (ModelState.IsValid)
            {
                var entity = (VehiclePart)this.GetObject(VehiclePart);
                entity.ApplyForAll = VehiclePart.ApplyForAll;
                db.VehiclePart.Add(entity);
                db.SaveChanges();
                VehiclePart.VehiclePartId = entity.VehiclePartId;
            }

            return Json(new[] { VehiclePart }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehiclePart_Update([DataSourceRequest]DataSourceRequest request, VehiclePart VehiclePart)
        {
            if (ModelState.IsValid)
            {
                var entity = (VehiclePart)this.GetObject(VehiclePart);
                entity.ApplyForAll = VehiclePart.ApplyForAll;
                db.VehiclePart.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { VehiclePart }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehiclePart_Destroy([DataSourceRequest]DataSourceRequest request, VehiclePart VehiclePart)
        {

            if (ModelState.IsValid)
            {
                var entity = (VehiclePart)this.GetObject(VehiclePart);

                db.VehiclePart.Attach(entity);
                db.VehiclePart.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { VehiclePart }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(VehiclePart vehiclePart)
        {
            var entity = new VehiclePart
            {
                VehiclePartId = vehiclePart.VehiclePartId,
                Code = vehiclePart.Code,
                VehicleCategoryId = vehiclePart.VehicleCategoryId,
                Name = vehiclePart.Name,
                Remark = vehiclePart.Remark,
                Description = vehiclePart.Description,
                DateCreated = vehiclePart.DateCreated,
                LastModified = vehiclePart.LastModified,
                CreatedBy = vehiclePart.CreatedBy,
                ModifiedBy = vehiclePart.ModifiedBy

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
