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
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Printing.Controllers
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
            IQueryable<Machine> machines = db.Machines;
            DataSourceResult result = machines.ToDataSourceResult(request, machine => new {
                MachineId = machine.MachineId,
                MachineName = machine.MachineName,
                Category = machine.Category,
                SubCategory = machine.SubCategory,
                Description = machine.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Machines_Create([DataSourceRequest]DataSourceRequest request, Machine machine)
        {
            if (ModelState.IsValid)
            {
                var entity = new Machine
                {
                    MachineName = machine.MachineName,
                    Category = machine.Category,
                    SubCategory = machine.SubCategory,
                    Description = machine.Description
                };

                db.Machines.Add(entity);
                db.SaveChanges();
                machine.MachineId = entity.MachineId;
            }

            return Json(new[] { machine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Machines_Update([DataSourceRequest]DataSourceRequest request, Machine machine)
        {
            if (ModelState.IsValid)
            {
                var entity = new Machine
                {
                    MachineId = machine.MachineId,
                    MachineName = machine.MachineName,
                    Category = machine.Category,
                    SubCategory = machine.SubCategory,
                    Description = machine.Description
                };

                db.Machines.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { machine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Machines_Destroy([DataSourceRequest]DataSourceRequest request, Machine machine)
        {
            if (ModelState.IsValid)
            {
                var entity = new Machine
                {
                    MachineId = machine.MachineId,
                    MachineName = machine.MachineName,
                    Category = machine.Category,
                    SubCategory = machine.SubCategory,
                    Description = machine.Description
                };

                db.Machines.Attach(entity);
                db.Machines.Remove(entity);
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
