﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Setting
{ 

   // [AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpAdmin)]
public class FurnitureMachineController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            
            return View();
        }

        public JsonResult GetAllMachineTypes()
        {
            var data = db.FurnitureMachines;
            return Json(data.Select(s => new { Code = s.FurnitureMachineId, Name = s.MachineName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Machines_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureMachine> Machines = db.FurnitureMachines;
            DataSourceResult result = Machines.ToDataSourceResult(request, Machine => new {
                MachineId = Machine.FurnitureMachineId,
                MachineName = Machine.MachineName,
                Category = Machine.Category,
                SubCategory = Machine.SubCategory,
                Description = Machine.Description
              
            });

            return Json(result);
        }

        public ActionResult Machines_Create([DataSourceRequest]DataSourceRequest request, FurnitureMachine Machine)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureMachine)this.GetObject(Machine);
                db.FurnitureMachines.Add(entity);
                db.SaveChanges();
                Machine.FurnitureMachineId = entity.FurnitureMachineId;
            }

            return Json(new[] { Machine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Machines_Update([DataSourceRequest]DataSourceRequest request, FurnitureMachine Machine)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureMachine)this.GetObject(Machine);

                db.FurnitureMachines.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { Machine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Machines_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureMachine Machine)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureMachine)this.GetObject(Machine);

                db.FurnitureMachines.Attach(entity);
                db.FurnitureMachines.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { Machine }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureMachine Machine)
        {
            var entity = new FurnitureMachine
            {
                FurnitureMachineId = Machine.FurnitureMachineId,
                MachineName = Machine.MachineName,
                Category = Machine.Category,
                SubCategory = Machine.SubCategory,
                Description = Machine.Description

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
