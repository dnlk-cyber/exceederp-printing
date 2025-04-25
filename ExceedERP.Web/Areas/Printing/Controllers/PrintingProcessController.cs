﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingProcessController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Machine"] = db.PrintingMachineTypes.Select(s => new
            {
                Text = s.MachineTypeName,
                Value = s.PrintingMachineTypeId
            }).ToList();
            return View();
        }

        public ActionResult PrintingProcesses_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingProcess> printingProcesses = db.PrintingProcesses;
            
            return Json(printingProcesses.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingProcesses_Create([DataSourceRequest]DataSourceRequest request, PrintingProcess model)
        {
            if (model.LaborRate == 0M)
                ModelState.AddModelError(string.Empty, "Labor rate can not be 0.00");
            if (ModelState.IsValid)
            {
                var entity = new PrintingProcess
                {
                    LabourActivity = model.LabourActivity,
                    PressCategory = model.PressCategory,
                    UOM = model.UOM,
                    DateCreated = DateTime.Today,
                    LastModified = model.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = model.ModifiedBy,
                    PrintingMachineTypeId = model.PrintingMachineTypeId,
                    LaborRate = model.LaborRate,
                    
                };

                db.PrintingProcesses.Add(entity);
                db.SaveChanges();
                model.PrintingProcessId = entity.PrintingProcessId;
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingProcesses_Update([DataSourceRequest]DataSourceRequest request, PrintingProcess model)
        {
            if (model.LaborRate == 0M)
                ModelState.AddModelError(string.Empty, "Labor rate can not be 0.00");
            if (ModelState.IsValid)
            {
                var entity = new PrintingProcess
                {
                    LabourActivity = model.LabourActivity,
                    PressCategory = model.PressCategory,
                    UOM = model.UOM,
                    DateCreated = model.DateCreated,
                    LastModified = DateTime.Today,
                    CreatedBy = model.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    LaborRate = model.LaborRate,
                    PrintingMachineTypeId = model.PrintingMachineTypeId,
                    PrintingProcessId = model.PrintingProcessId
                };

                db.PrintingProcesses.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingProcesses_Destroy([DataSourceRequest]DataSourceRequest request, PrintingProcess model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProcess
                {
                    LabourActivity = model.LabourActivity,
                    PressCategory = model.PressCategory,
                    UOM = model.UOM,
                    DateCreated = model.DateCreated,
                    LastModified = model.LastModified,
                    CreatedBy = model.CreatedBy,
                    ModifiedBy = model.ModifiedBy,
                    LaborRate = model.LaborRate,
                    PrintingMachineTypeId = model.PrintingMachineTypeId,
                    PrintingProcessId = model.PrintingProcessId
                };

                db.PrintingProcesses.Attach(entity);
                db.PrintingProcesses.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
