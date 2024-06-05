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

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class VehicleReceptionFormController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
       
        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers.ToList();
            ViewData["Employee"] = db.Person_Employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).ToList();
            ViewData["VehiclePartDamageType"] = db.VehiclePartDamageType.ToList();
            ViewData["VehicleParts"] = db.VehiclePart.ToList();
            ViewData["category"] = db.EquipmentIdentity.Select(s => new {
                Id = s.EquipmentIdentityID,
                Name = s.EquipmentCategoryName
            }).ToList();
            return View();
        }

        public ActionResult Print(string id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult VehicleReceptionForm_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<VehicleReceptionForm> vehiclereceptionform = db.VehicleReceptionForm;
            DataSourceResult result = vehiclereceptionform.ToDataSourceResult(request, vehicleReceptionForm => new {
                VehicleReceptionFormId = vehicleReceptionForm.VehicleReceptionFormId,
                Source= vehicleReceptionForm.Source,
                CustomerId = vehicleReceptionForm.CustomerId,
                VehicleCategoryId = vehicleReceptionForm.VehicleCategoryId,
                RecipientId = vehicleReceptionForm.RecipientId,
                TechnicianAssignedId = vehicleReceptionForm.TechnicianAssignedId,
                PlateNo = vehicleReceptionForm.PlateNo,
                VehicleType = vehicleReceptionForm.VehicleType,
                VehicleModel = vehicleReceptionForm.VehicleModel,
                Mileage = vehicleReceptionForm.Mileage,
                Fuel = vehicleReceptionForm.Fuel,
                Remark = vehicleReceptionForm.Remark,
                DateCreated = vehicleReceptionForm.DateCreated,
                LastModified = vehicleReceptionForm.LastModified,
                CreatedBy = vehicleReceptionForm.CreatedBy,
                ModifiedBy = vehicleReceptionForm.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult VehicleReceptionForm_Create([DataSourceRequest]DataSourceRequest request, VehicleReceptionForm vehicleReceptionForm)
        {



            
            if (ModelState.IsValid)
            {
                var vehicle = db.EquipmentInformation.Where(q => q.LicensePlate == vehicleReceptionForm.PlateNo).FirstOrDefault();
                if(vehicle!=null)
                    {
                    vehicleReceptionForm.VehicleModel = vehicle.DeviceNumber ;
                    vehicleReceptionForm.VehicleType = vehicle.GroupName ;
                }
                if (vehicleReceptionForm.Source == Source.External && vehicleReceptionForm.CustomerId == null)
                    ModelState.AddModelError("CustomerNull", "Please select customer !");
                var entity = new VehicleReceptionForm
                {
                    VehicleReceptionFormId = vehicleReceptionForm.VehicleReceptionFormId,
                    Source = vehicleReceptionForm.Source,
                    CustomerId = vehicleReceptionForm.CustomerId,
                    VehicleCategoryId = vehicleReceptionForm.VehicleCategoryId,
                    Mileage = vehicleReceptionForm.Mileage,
                    PlateNo = vehicleReceptionForm.PlateNo,
                    VehicleType = vehicleReceptionForm.VehicleType,
                    Fuel = vehicleReceptionForm.Fuel,
                    TechnicianAssignedId = vehicleReceptionForm.TechnicianAssignedId,
                    VehicleModel = vehicleReceptionForm.VehicleModel,
                    Remark = vehicleReceptionForm.Remark,
                    DateCreated=DateTime.Now,
                    CreatedBy=User.Identity.Name,



                };


                db.VehicleReceptionForm.Add(entity);
                db.SaveChanges();

                var parts = db.VehiclePart.Where(x=>x.VehicleCategoryId== vehicleReceptionForm.VehicleCategoryId).ToList();
                IList<VehiclePartManagement> vehiclePartManagements = new List<VehiclePartManagement>();
                foreach(var part in parts)
                {
                    var vehiclePartManagement = new VehiclePartManagement();
                    vehiclePartManagement.VehiclePartId = part.VehiclePartId;
                    vehiclePartManagement.VehicleReceptionFormId = entity.VehicleReceptionFormId;
                    db.VehiclePartManagement.Add(vehiclePartManagement);

                }
                db.SaveChanges();
                vehicleReceptionForm.VehicleReceptionFormId = entity.VehicleReceptionFormId;
            }

            return Json(new[] { vehicleReceptionForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleReceptionForm_Update([DataSourceRequest]DataSourceRequest request, VehicleReceptionForm vehicleReceptionForm)
        {

            if (ModelState.IsValid)
            {
                var vehicle = db.EquipmentInformation.Where(q => q.LicensePlate == vehicleReceptionForm.PlateNo).FirstOrDefault();
                if (vehicle != null)
                {
                    vehicleReceptionForm.VehicleModel = vehicle.DeviceNumber;
                    vehicleReceptionForm.VehicleType = vehicle.GroupName;
                }
                if (vehicleReceptionForm.Source==Source.External && vehicleReceptionForm.CustomerId == null)
                    ModelState.AddModelError("CustomerNull", "Please select customer !");
                var entity = new VehicleReceptionForm
                {

                    VehicleReceptionFormId = vehicleReceptionForm.VehicleReceptionFormId,
                    Source = vehicleReceptionForm.Source,
                    CustomerId = vehicleReceptionForm.CustomerId,
                    VehicleCategoryId = vehicleReceptionForm.VehicleCategoryId,
                    Mileage = vehicleReceptionForm.Mileage,
                    PlateNo = vehicleReceptionForm.PlateNo,
                    VehicleType = vehicleReceptionForm.VehicleType,
                    Fuel = vehicleReceptionForm.Fuel,
                    TechnicianAssignedId = vehicleReceptionForm.TechnicianAssignedId,
                    VehicleModel = vehicleReceptionForm.VehicleModel,
                    Remark = vehicleReceptionForm.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name


                };
                db.VehicleReceptionForm.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { vehicleReceptionForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleReceptionForm_Destroy([DataSourceRequest]DataSourceRequest request, VehicleReceptionForm VehicleReceptionForm)
        {

            if (ModelState.IsValid)
            {
                var entity = (VehicleReceptionForm)this.GetObject(VehicleReceptionForm);

                db.VehicleReceptionForm.Attach(entity);
                db.VehicleReceptionForm.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { VehicleReceptionForm }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(VehicleReceptionForm vehicleReceptionForm)
        {
            var userName = User.Identity.Name;

            var entity = new VehicleReceptionForm
            {
                VehicleReceptionFormId = vehicleReceptionForm.VehicleReceptionFormId,
                CustomerId = vehicleReceptionForm.CustomerId,
                RecipientId = UserInformationHelper.GetEmployeeId(userName),
                TechnicianAssignedId = vehicleReceptionForm.TechnicianAssignedId,
                PlateNo = vehicleReceptionForm.PlateNo,
                VehicleType = vehicleReceptionForm.VehicleType,
                VehicleCategoryId = vehicleReceptionForm.VehicleCategoryId,
                VehicleModel = vehicleReceptionForm.VehicleModel,
                Mileage = vehicleReceptionForm.Mileage,
                Fuel = vehicleReceptionForm.Fuel,
                Remark = vehicleReceptionForm.Remark,
                DateCreated = vehicleReceptionForm.DateCreated,
                LastModified = vehicleReceptionForm.LastModified,
                CreatedBy = vehicleReceptionForm.CreatedBy,
                ModifiedBy = vehicleReceptionForm.ModifiedBy

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
