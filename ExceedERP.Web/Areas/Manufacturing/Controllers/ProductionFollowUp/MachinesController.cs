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
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp
{
    public class MachinesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Machines_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureMachine> machines = db.FurnitureMachines;
            DataSourceResult result = machines.ToDataSourceResult(request, machine => new {
                MachineId = machine.FurnitureMachineId,
                MachineName = machine.MachineName,
                Category = machine.Category,
                SubCategory = machine.SubCategory,
                Description = machine.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Machines_Create([DataSourceRequest]DataSourceRequest request, FurnitureMachine machine)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureMachine
                {
                    MachineName = machine.MachineName,
                    Category = machine.Category,
                    SubCategory = machine.SubCategory,
                    Description = machine.Description
                };

                db.FurnitureMachines.Add(entity);
                db.SaveChanges();
                machine.FurnitureMachineId = entity.FurnitureMachineId;
            }

            return Json(new[] { machine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Machines_Update([DataSourceRequest]DataSourceRequest request, FurnitureMachine machine)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureMachine
                {
                   FurnitureMachineId = machine.FurnitureMachineId,
                    MachineName = machine.MachineName,
                    Category = machine.Category,
                    SubCategory = machine.SubCategory,
                    Description = machine.Description
                };

                db.FurnitureMachines.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { machine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Machines_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureMachine machine)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureMachine
                {
                    FurnitureMachineId = machine.FurnitureMachineId,
                    MachineName = machine.MachineName,
                    Category = machine.Category,
                    SubCategory = machine.SubCategory,
                    Description = machine.Description
                };

                db.FurnitureMachines.Attach(entity);
                db.FurnitureMachines.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { machine }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
