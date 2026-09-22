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
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class TireAndAccesseryInfoController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
       
        public ActionResult Index()
        {
           
            return View();
        }

        public ActionResult TyreSerial_Read([DataSourceRequest]DataSourceRequest request,int Id)
        {
            IQueryable<TyreSerialInfo> vehiclereceptionform = db.TyreSerialInfo.Where(x=>x.VehicleReceptionFormId==Id);
            DataSourceResult result = vehiclereceptionform.ToDataSourceResult(request, vehicleReceptionForm => new {
                TyreSerialInfoId = vehicleReceptionForm.TyreSerialInfoId,
                VehicleReceptionFormId = vehicleReceptionForm.VehicleReceptionFormId,
                TireName = vehicleReceptionForm.TireName,
                TireSize = vehicleReceptionForm.TireSize,
                TireModel = vehicleReceptionForm.TireModel,
                NumberOfTire = vehicleReceptionForm.NumberOfTire,
                SerialNumber = vehicleReceptionForm.SerialNumber,
                Description = vehicleReceptionForm.Description,
                LastModified = vehicleReceptionForm.LastModified,
                CreatedBy = vehicleReceptionForm.CreatedBy,
                ModifiedBy = vehicleReceptionForm.ModifiedBy,
                
            });

            return Json(result);
        }

        public ActionResult TyreSerial_Create([DataSourceRequest]DataSourceRequest request, TyreSerialInfo vehicleReceptionForm,int Id)
        {
            vehicleReceptionForm.DateCreated = DateTime.Now;
            vehicleReceptionForm.CreatedBy = User.Identity.Name;
            vehicleReceptionForm.VehicleReceptionFormId = Id;
            if (ModelState.IsValid)
            {                
                db.TyreSerialInfo.Add(vehicleReceptionForm);
                db.SaveChanges();

               }

            return Json(new[] { vehicleReceptionForm }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TyreSerial_Update([DataSourceRequest]DataSourceRequest request, TyreSerialInfo vehicleReceptionForm)
        {
            var fetchTyre = db.TyreSerialInfo.FirstOrDefault(x => x.TyreSerialInfoId == vehicleReceptionForm.TyreSerialInfoId);
            fetchTyre.TireSize = vehicleReceptionForm.TireSize;
            fetchTyre.TireName = vehicleReceptionForm.TireName;
            fetchTyre.TireModel = vehicleReceptionForm.TireModel;
            fetchTyre.NumberOfTire = vehicleReceptionForm.NumberOfTire;
            fetchTyre.SerialNumber = vehicleReceptionForm.SerialNumber;
            fetchTyre.LastModified = vehicleReceptionForm.LastModified;
            fetchTyre.ModifiedBy = vehicleReceptionForm.ModifiedBy;

            if (ModelState.IsValid)
            {
               
                db.Entry(fetchTyre).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { vehicleReceptionForm }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult TyreSerial_Delete([DataSourceRequest]DataSourceRequest request, TyreSerialInfo vehicleReceptionForm)
        {
            var fetchTyre = db.TyreSerialInfo.FirstOrDefault(x => x.TyreSerialInfoId == vehicleReceptionForm.TyreSerialInfoId);
           

            if (ModelState.IsValid)
            {

                db.TyreSerialInfo.Remove(fetchTyre);
                db.SaveChanges();
            }

            return Json(new[] { vehicleReceptionForm }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Acc_Read([DataSourceRequest]DataSourceRequest request,int Id)
        {
            IQueryable<AccesseryInfo> vehiclereceptionform = db.AccesseryInfo.Where(x=>x.VehricleReceptionFormId==Id);
            DataSourceResult result = vehiclereceptionform.ToDataSourceResult(request, vehicleReceptionForm => new {
                AccesseryInfoId = vehicleReceptionForm.AccesseryInfoId,
                VehicleReceptionFormId = vehicleReceptionForm.VehricleReceptionFormId,
                AccName = vehicleReceptionForm.AccName,
                AccModel = vehicleReceptionForm.AccModel,
                NumberOfAcc = vehicleReceptionForm.NumberOfAcc,
                Description = vehicleReceptionForm.Description,
                LastModified = vehicleReceptionForm.LastModified,
                CreatedBy = vehicleReceptionForm.CreatedBy,
                ModifiedBy = vehicleReceptionForm.ModifiedBy,
                Lastmodified=vehicleReceptionForm.LastModified
                
            });

            return Json(result);
        }

        public ActionResult Acc_Create([DataSourceRequest]DataSourceRequest request, AccesseryInfo vehicleReceptionForm,int Id)
        {
            vehicleReceptionForm.DateCreated = DateTime.Now;
            vehicleReceptionForm.CreatedBy = User.Identity.Name;
            vehicleReceptionForm.VehricleReceptionFormId = Id;
            if (ModelState.IsValid)
            {                
                db.AccesseryInfo.Add(vehicleReceptionForm);
                db.SaveChanges();

               }

            return Json(new[] { vehicleReceptionForm }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Acc_Update([DataSourceRequest]DataSourceRequest request, AccesseryInfo vehicleReceptionForm)
        {
            var fetchTyre = db.AccesseryInfo.FirstOrDefault(x => x.AccesseryInfoId == vehicleReceptionForm.AccesseryInfoId);
            fetchTyre.AccName = vehicleReceptionForm.AccName;
            fetchTyre.AccModel = vehicleReceptionForm.AccModel;
            fetchTyre.NumberOfAcc = vehicleReceptionForm.NumberOfAcc;
            fetchTyre.Description = vehicleReceptionForm.Description;
            fetchTyre.LastModified = vehicleReceptionForm.LastModified;
            fetchTyre.ModifiedBy = vehicleReceptionForm.ModifiedBy;

            if (ModelState.IsValid)
            {
               
                db.Entry(fetchTyre).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { vehicleReceptionForm }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Acc_Delete([DataSourceRequest]DataSourceRequest request, AccesseryInfo vehicleReceptionForm)
        {
            var fetchTyre = db.AccesseryInfo.FirstOrDefault(x => x.AccesseryInfoId == vehicleReceptionForm.AccesseryInfoId);
           

            if (ModelState.IsValid)
            {

                db.AccesseryInfo.Remove(fetchTyre);
                db.SaveChanges();
            }

            return Json(new[] { vehicleReceptionForm }.ToDataSourceResult(request, ModelState));
        }
      
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
