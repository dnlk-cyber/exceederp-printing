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
            DataSourceResult result = printingprocesses.ToDataSourceResult(request, printingProcess => new {
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
                ModifiedBy = printingProcess.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingProcesses_Create([DataSourceRequest]DataSourceRequest request, PrintingProcess printingProcess)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProcess
                {
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
                    ModifiedBy = printingProcess.ModifiedBy
                };

                db.PrintingProcesses.Add(entity);
                db.SaveChanges();
                printingProcess.PrintingProcessId = entity.PrintingProcessId;
            }

            return Json(new[] { printingProcess }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingProcesses_Update([DataSourceRequest]DataSourceRequest request, PrintingProcess printingProcess)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProcess
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
                    ModifiedBy = printingProcess.ModifiedBy
                };

                db.PrintingProcesses.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printingProcess }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingProcesses_Destroy([DataSourceRequest]DataSourceRequest request, PrintingProcess printingProcess)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProcess
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
                    ModifiedBy = printingProcess.ModifiedBy
                };

                db.PrintingProcesses.Attach(entity);
                db.PrintingProcesses.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { printingProcess }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
