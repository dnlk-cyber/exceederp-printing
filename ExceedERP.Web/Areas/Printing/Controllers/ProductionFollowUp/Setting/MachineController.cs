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
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Setting
{ 
   // [AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpAdmin)]
    public class MachineController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            
            return View();
        }

        public JsonResult GetAllMachineTypes()
        {
            var data = db.Machines;
            return Json(data.Select(s => new { Code = s.MachineId, Name = s.MachineName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Machines_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Machine> Machines = db.Machines;
            DataSourceResult result = Machines.ToDataSourceResult(request, Machine => new {
                MachineId = Machine.MachineId,
                MachineName = Machine.MachineName,
                Category = Machine.Category,
                SubCategory = Machine.SubCategory,
                Description = Machine.Description
              
            });

            return Json(result);
        }

        public ActionResult Machines_Create([DataSourceRequest]DataSourceRequest request, Machine Machine)
        {
            if (ModelState.IsValid)
            {
                var entity = (Machine)this.GetObject(Machine);
                db.Machines.Add(entity);
                db.SaveChanges();
                Machine.MachineId = entity.MachineId;
            }

            return Json(new[] { Machine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Machines_Update([DataSourceRequest]DataSourceRequest request, Machine Machine)
        {
            if (ModelState.IsValid)
            {
                var entity = (Machine)this.GetObject(Machine);

                db.Machines.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { Machine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Machines_Destroy([DataSourceRequest]DataSourceRequest request, Machine Machine)
        {

            if (ModelState.IsValid)
            {
                var entity = (Machine)this.GetObject(Machine);

                db.Machines.Attach(entity);
                db.Machines.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { Machine }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(Machine Machine)
        {
            var entity = new Machine
            {
                MachineId = Machine.MachineId,
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
