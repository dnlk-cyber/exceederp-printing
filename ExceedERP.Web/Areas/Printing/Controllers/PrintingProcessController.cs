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
            return View();
        }

        public ActionResult PrintingProcesses_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingProcess> printingprocesses = db.PrintingProcesses;
            DataSourceResult result = printingprocesses.ToDataSourceResult(request, printingProcess => new PrintingProcess
            {
                PrintingProcessId = printingProcess.PrintingProcessId,
                PrintingCategory = printingProcess.PrintingCategory,
               
                ProcessName = printingProcess.ProcessName,
                HoursRequired = printingProcess.HoursRequired,
                Quantity = printingProcess.Quantity,
                ProcessScope = printingProcess.ProcessScope,
                SetupCost = printingProcess.SetupCost,
                CostPerItem = printingProcess.CostPerItem,
                Remark = printingProcess.Remark,
                DateCreated = printingProcess.DateCreated,
                LastModified = printingProcess.LastModified,
                CreatedBy = printingProcess.CreatedBy,
                ModifiedBy = printingProcess.ModifiedBy,
                EstProductionPerHour = printingProcess.EstProductionPerHour,
                LaborRate = printingProcess.LaborRate,
                PrintingProcessName = printingProcess.PrintingProcessName
                
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingProcesses_Create([DataSourceRequest]DataSourceRequest request, PrintingProcess model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProcess
                {
                    PrintingCategory = model.PrintingCategory,
                   
                    ProcessName = model.ProcessName,
                    HoursRequired = model.HoursRequired,
                    Quantity = model.Quantity,
                    ProcessScope = model.ProcessScope,
                    SetupCost = model.SetupCost,
                    CostPerItem = model.CostPerItem,
                    Remark = model.Remark,
                    DateCreated = model.DateCreated,
                    LastModified = model.LastModified,
                    CreatedBy = model.CreatedBy,
                    ModifiedBy = model.ModifiedBy,
                    PrintingProcessName = model.PrintingProcessName,
                    LaborRate = model.LaborRate,
                    EstProductionPerHour = model.EstProductionPerHour
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
            if (ModelState.IsValid)
            {
                var entity = new PrintingProcess
                {
                    PrintingProcessId = model.PrintingProcessId,
                    PrintingCategory = model.PrintingCategory,
                   
                    ProcessName = model.ProcessName,
                    HoursRequired = model.HoursRequired,
                    Quantity = model.Quantity,
                    ProcessScope = model.ProcessScope,
                    SetupCost = model.SetupCost,
                    CostPerItem = model.CostPerItem,
                    Remark = model.Remark,
                    DateCreated = model.DateCreated,
                    LastModified = model.LastModified,
                    CreatedBy = model.CreatedBy,
                    ModifiedBy = model.ModifiedBy,
                    EstProductionPerHour = model.EstProductionPerHour,
                    LaborRate = model.LaborRate,
                    PrintingProcessName = model.PrintingProcessName
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
                    PrintingProcessId = model.PrintingProcessId,
                    PrintingCategory = model.PrintingCategory,
                   
                    ProcessName = model.ProcessName,
                    HoursRequired = model.HoursRequired,
                    Quantity = model.Quantity,
                    ProcessScope = model.ProcessScope,
                    SetupCost = model.SetupCost,
                    CostPerItem = model.CostPerItem,
                    Remark = model.Remark,
                    DateCreated = model.DateCreated,
                    LastModified = model.LastModified,
                    CreatedBy = model.CreatedBy,
                    ModifiedBy = model.ModifiedBy,
                    PrintingProcessName = model.PrintingProcessName,
                    LaborRate = model.LaborRate,
                    EstProductionPerHour = model.EstProductionPerHour
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
