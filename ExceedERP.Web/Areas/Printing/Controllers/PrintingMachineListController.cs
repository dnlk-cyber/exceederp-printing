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
    public class PrintingMachineListController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintingMachineLists_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingMachineList> printingmachinelists = db.PrintingMachineLists;
            DataSourceResult result = printingmachinelists.ToDataSourceResult(request, printingMachineList => new {
                PrintingMachineListId = printingMachineList.PrintingMachineListId,
                MachineTypeCode = printingMachineList.MachineTypeCode,
                PrintingCostCenterId = printingMachineList.PrintingCostCenterId,
                PaperFeedStyle = printingMachineList.PaperFeedStyle,
                NumberUnits = printingMachineList.NumberUnits,
                MachineTypeName = printingMachineList.MachineTypeName,
                PrintingHourlyChargeRate = printingMachineList.PrintingHourlyChargeRate,
                MakereadyHours = printingMachineList.MakereadyHours,
                MachineStatus = printingMachineList.MachineStatus,
                NumberOfColors = printingMachineList.NumberOfColors,
                OutputRate = printingMachineList.OutputRate,
                Remark = printingMachineList.Remark,
                DateCreated = printingMachineList.DateCreated,
                LastModified = printingMachineList.LastModified,
                CreatedBy = printingMachineList.CreatedBy,
                ModifiedBy = printingMachineList.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingMachineLists_Create([DataSourceRequest]DataSourceRequest request, PrintingMachineList printingMachineList)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingMachineList
                {
                    MachineTypeCode = printingMachineList.MachineTypeCode,
                    PrintingCostCenterId = printingMachineList.PrintingCostCenterId,
                    PaperFeedStyle = printingMachineList.PaperFeedStyle,
                    NumberUnits = printingMachineList.NumberUnits,
                    MachineTypeName = printingMachineList.MachineTypeName,
                    PrintingHourlyChargeRate = printingMachineList.PrintingHourlyChargeRate,
                    MakereadyHours = printingMachineList.MakereadyHours,
                    MachineStatus = printingMachineList.MachineStatus,
                    NumberOfColors = printingMachineList.NumberOfColors,
                    OutputRate = printingMachineList.OutputRate,
                    Remark = printingMachineList.Remark,
                    DateCreated = printingMachineList.DateCreated,
                    LastModified = printingMachineList.LastModified,
                    CreatedBy = printingMachineList.CreatedBy,
                    ModifiedBy = printingMachineList.ModifiedBy
                };

                db.PrintingMachineLists.Add(entity);
                db.SaveChanges();
                printingMachineList.PrintingMachineListId = entity.PrintingMachineListId;
            }

            return Json(new[] { printingMachineList }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingMachineLists_Update([DataSourceRequest]DataSourceRequest request, PrintingMachineList printingMachineList)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingMachineList
                {
                    PrintingMachineListId = printingMachineList.PrintingMachineListId,
                    MachineTypeCode = printingMachineList.MachineTypeCode,
                    PrintingCostCenterId = printingMachineList.PrintingCostCenterId,
                    PaperFeedStyle = printingMachineList.PaperFeedStyle,
                    NumberUnits = printingMachineList.NumberUnits,
                    MachineTypeName = printingMachineList.MachineTypeName,
                    PrintingHourlyChargeRate = printingMachineList.PrintingHourlyChargeRate,
                    MakereadyHours = printingMachineList.MakereadyHours,
                    MachineStatus = printingMachineList.MachineStatus,
                    NumberOfColors = printingMachineList.NumberOfColors,
                    OutputRate = printingMachineList.OutputRate,
                    Remark = printingMachineList.Remark,
                    DateCreated = printingMachineList.DateCreated,
                    LastModified = printingMachineList.LastModified,
                    CreatedBy = printingMachineList.CreatedBy,
                    ModifiedBy = printingMachineList.ModifiedBy
                };

                db.PrintingMachineLists.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printingMachineList }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingMachineLists_Destroy([DataSourceRequest]DataSourceRequest request, PrintingMachineList printingMachineList)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingMachineList
                {
                    PrintingMachineListId = printingMachineList.PrintingMachineListId,
                    MachineTypeCode = printingMachineList.MachineTypeCode,
                    PrintingCostCenterId = printingMachineList.PrintingCostCenterId,
                    PaperFeedStyle = printingMachineList.PaperFeedStyle,
                    NumberUnits = printingMachineList.NumberUnits,
                    MachineTypeName = printingMachineList.MachineTypeName,
                    PrintingHourlyChargeRate = printingMachineList.PrintingHourlyChargeRate,
                    MakereadyHours = printingMachineList.MakereadyHours,
                    MachineStatus = printingMachineList.MachineStatus,
                    NumberOfColors = printingMachineList.NumberOfColors,
                    OutputRate = printingMachineList.OutputRate,
                    Remark = printingMachineList.Remark,
                    DateCreated = printingMachineList.DateCreated,
                    LastModified = printingMachineList.LastModified,
                    CreatedBy = printingMachineList.CreatedBy,
                    ModifiedBy = printingMachineList.ModifiedBy
                };

                db.PrintingMachineLists.Attach(entity);
                db.PrintingMachineLists.Remove(entity);
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
