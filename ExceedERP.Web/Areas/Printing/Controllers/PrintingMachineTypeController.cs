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
    public class PrintingMachineTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintingMachineType_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingMachineType> PrintingMachineType = db.PrintingMachineTypes;
            DataSourceResult result = PrintingMachineType.ToDataSourceResult(request, printingMachineList => new PrintingMachineType
            {
                PrintingMachineTypeId = printingMachineList.PrintingMachineTypeId,
                
                PrintingCostCenterId = printingMachineList.PrintingCostCenterId,
                PaperFeedStyle = printingMachineList.PaperFeedStyle,
                NumberUnits = printingMachineList.NumberUnits,
                MachineTypeName = printingMachineList.MachineTypeName,
                PrintingHourlyChargeRate = printingMachineList.PrintingHourlyChargeRate,
                MakeReadyHours = printingMachineList.MakeReadyHours,
                MachineStatus = printingMachineList.MachineStatus,
                NumberOfColors = printingMachineList.NumberOfColors,
                OutputRate = printingMachineList.OutputRate,
                Remark = printingMachineList.Remark,
                DateCreated = printingMachineList.DateCreated,
                LastModified = printingMachineList.LastModified,
                CreatedBy = printingMachineList.CreatedBy,
                ModifiedBy = printingMachineList.ModifiedBy,
                
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingMachineType_Create([DataSourceRequest]DataSourceRequest request, PrintingMachineType printingMachineList)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingMachineType
                {
                    
                    PrintingCostCenterId = printingMachineList.PrintingCostCenterId,
                    PaperFeedStyle = printingMachineList.PaperFeedStyle,
                    NumberUnits = printingMachineList.NumberUnits,
                    MachineTypeName = printingMachineList.MachineTypeName,
                    PrintingHourlyChargeRate = printingMachineList.PrintingHourlyChargeRate,
                    MakeReadyHours = printingMachineList.MakeReadyHours,
                    MachineStatus = printingMachineList.MachineStatus,
                    NumberOfColors = printingMachineList.NumberOfColors,
                    OutputRate = printingMachineList.OutputRate,
                    Remark = printingMachineList.Remark,
                    DateCreated = printingMachineList.DateCreated,
                    LastModified = printingMachineList.LastModified,
                    CreatedBy = printingMachineList.CreatedBy,
                    ModifiedBy = printingMachineList.ModifiedBy
                };

                db.PrintingMachineTypes.Add(entity);
                db.SaveChanges();
                printingMachineList.PrintingMachineTypeId = entity.PrintingMachineTypeId;
            }

            return Json(new[] { printingMachineList }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingMachineType_Update([DataSourceRequest]DataSourceRequest request, PrintingMachineType printingMachineList)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingMachineType
                {
                    PrintingMachineTypeId = printingMachineList.PrintingMachineTypeId,
                    
                    PrintingCostCenterId = printingMachineList.PrintingCostCenterId,
                    PaperFeedStyle = printingMachineList.PaperFeedStyle,
                    NumberUnits = printingMachineList.NumberUnits,
                    MachineTypeName = printingMachineList.MachineTypeName,
                    PrintingHourlyChargeRate = printingMachineList.PrintingHourlyChargeRate,
                    MakeReadyHours = printingMachineList.MakeReadyHours,
                    MachineStatus = printingMachineList.MachineStatus,
                    NumberOfColors = printingMachineList.NumberOfColors,
                    OutputRate = printingMachineList.OutputRate,
                    Remark = printingMachineList.Remark,
                    DateCreated = printingMachineList.DateCreated,
                    LastModified = printingMachineList.LastModified,
                    CreatedBy = printingMachineList.CreatedBy,
                    ModifiedBy = printingMachineList.ModifiedBy
                };

                db.PrintingMachineTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printingMachineList }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingMachineType_Delete([DataSourceRequest]DataSourceRequest request, PrintingMachineType printingMachineList)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingMachineType
                {
                    PrintingMachineTypeId = printingMachineList.PrintingMachineTypeId,
                    
                    PrintingCostCenterId = printingMachineList.PrintingCostCenterId,
                    PaperFeedStyle = printingMachineList.PaperFeedStyle,
                    NumberUnits = printingMachineList.NumberUnits,
                    MachineTypeName = printingMachineList.MachineTypeName,
                    PrintingHourlyChargeRate = printingMachineList.PrintingHourlyChargeRate,
                    MakeReadyHours = printingMachineList.MakeReadyHours,
                    MachineStatus = printingMachineList.MachineStatus,
                    NumberOfColors = printingMachineList.NumberOfColors,
                    OutputRate = printingMachineList.OutputRate,
                    Remark = printingMachineList.Remark,
                    DateCreated = printingMachineList.DateCreated,
                    LastModified = printingMachineList.LastModified,
                    CreatedBy = printingMachineList.CreatedBy,
                    ModifiedBy = printingMachineList.ModifiedBy
                };

                db.PrintingMachineTypes.Attach(entity);
                db.PrintingMachineTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { printingMachineList }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
