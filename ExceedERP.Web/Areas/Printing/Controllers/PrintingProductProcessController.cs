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
    public class PrintingProductProcessController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            ViewData["JobType"] = db.PrintingJobTypes.Select(s => new
            {
                Text = s.JobTypeName,
                Value = s.PrintingJobTypeId
            }).ToList();

            ViewData["PrintingProcess"] = db.PrintingProcesses.Select(s => new
            {
                Text = s.ProcessName,
                Value = s.PrintingProcessId
            }).ToList();

            return View();
        }

        public ActionResult PrintingProductProcesses_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingProductProcess> printingproductprocesses = db.PrintingProductProcesses;
            DataSourceResult result = printingproductprocesses.ToDataSourceResult(request, printingProductProcess => new {
                PrintingProductProcessId = printingProductProcess.PrintingProductProcessId,
                JobTypeId = printingProductProcess.JobTypeId,
                PrintingProcessCategory = printingProductProcess.PrintingProcessCategory,
                ProcessName = printingProductProcess.ProcessName,
                HoursRequired = printingProductProcess.HoursRequired,
                QtyToDone = printingProductProcess.QtyToDone,
                PrintingProcessId = printingProductProcess.PrintingProcessId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingProductProcesses_Create([DataSourceRequest]DataSourceRequest request, PrintingProductProcess printingProductProcess)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProductProcess
                {
                    JobTypeId = printingProductProcess.JobTypeId,
                    PrintingProcessCategory = printingProductProcess.PrintingProcessCategory,
                    ProcessName = printingProductProcess.ProcessName,
                    HoursRequired = printingProductProcess.HoursRequired,
                    QtyToDone = printingProductProcess.QtyToDone,
                    PrintingProcessId = printingProductProcess.PrintingProcessId
                };

                db.PrintingProductProcesses.Add(entity);
                db.SaveChanges();
                printingProductProcess.PrintingProductProcessId = entity.PrintingProductProcessId;
            }

            return Json(new[] { printingProductProcess }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingProductProcesses_Update([DataSourceRequest]DataSourceRequest request, PrintingProductProcess printingProductProcess)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProductProcess
                {
                    PrintingProductProcessId = printingProductProcess.PrintingProductProcessId,
                    JobTypeId = printingProductProcess.JobTypeId,
                    PrintingProcessCategory = printingProductProcess.PrintingProcessCategory,
                    ProcessName = printingProductProcess.ProcessName,
                    HoursRequired = printingProductProcess.HoursRequired,
                    QtyToDone = printingProductProcess.QtyToDone,
                    PrintingProcessId = printingProductProcess.PrintingProcessId
                };

                db.PrintingProductProcesses.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printingProductProcess }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingProductProcesses_Destroy([DataSourceRequest]DataSourceRequest request, PrintingProductProcess printingProductProcess)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProductProcess
                {
                    PrintingProductProcessId = printingProductProcess.PrintingProductProcessId,
                    JobTypeId = printingProductProcess.JobTypeId,
                    PrintingProcessCategory = printingProductProcess.PrintingProcessCategory,
                    ProcessName = printingProductProcess.ProcessName,
                    HoursRequired = printingProductProcess.HoursRequired,
                    QtyToDone = printingProductProcess.QtyToDone,
                    PrintingProcessId = printingProductProcess.PrintingProcessId
                };

                db.PrintingProductProcesses.Attach(entity);
                db.PrintingProductProcesses.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { printingProductProcess }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
