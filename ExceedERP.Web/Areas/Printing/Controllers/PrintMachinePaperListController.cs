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
    public class PrintMachinePaperListController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintMachinePaperLists_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintMachinePaperList> printmachinepaperlists = db.PrintMachinePaperLists;
            DataSourceResult result = printmachinepaperlists.ToDataSourceResult(request, printMachinePaperList => new {
                PrintMachinePaperListId = printMachinePaperList.PrintMachinePaperListId,
                PrintMachineTypeId = printMachinePaperList.PrintMachineTypeId,
                PaperSizeCode = printMachinePaperList.PaperSizeCode,
                Width = printMachinePaperList.Width,
                Length = printMachinePaperList.Length,
                PaperName = printMachinePaperList.PaperName,
                NumberOfPagesPerSheet = printMachinePaperList.NumberOfPagesPerSheet,
                NumberOfPagesPerPlate = printMachinePaperList.NumberOfPagesPerPlate,
                DateCreated = printMachinePaperList.DateCreated,
                LastModified = printMachinePaperList.LastModified,
                CreatedBy = printMachinePaperList.CreatedBy,
                ModifiedBy = printMachinePaperList.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintMachinePaperLists_Create([DataSourceRequest]DataSourceRequest request, PrintMachinePaperList printMachinePaperList)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintMachinePaperList
                {
                    PrintMachineTypeId = printMachinePaperList.PrintMachineTypeId,
                    PaperSizeCode = printMachinePaperList.PaperSizeCode,
                    Width = printMachinePaperList.Width,
                    Length = printMachinePaperList.Length,
                    PaperName = printMachinePaperList.PaperName,
                    NumberOfPagesPerSheet = printMachinePaperList.NumberOfPagesPerSheet,
                    NumberOfPagesPerPlate = printMachinePaperList.NumberOfPagesPerPlate,
                    DateCreated = printMachinePaperList.DateCreated,
                    LastModified = printMachinePaperList.LastModified,
                    CreatedBy = printMachinePaperList.CreatedBy,
                    ModifiedBy = printMachinePaperList.ModifiedBy
                };

                db.PrintMachinePaperLists.Add(entity);
                db.SaveChanges();
                printMachinePaperList.PrintMachinePaperListId = entity.PrintMachinePaperListId;
            }

            return Json(new[] { printMachinePaperList }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintMachinePaperLists_Update([DataSourceRequest]DataSourceRequest request, PrintMachinePaperList printMachinePaperList)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintMachinePaperList
                {
                    PrintMachinePaperListId = printMachinePaperList.PrintMachinePaperListId,
                    PrintMachineTypeId = printMachinePaperList.PrintMachineTypeId,
                    PaperSizeCode = printMachinePaperList.PaperSizeCode,
                    Width = printMachinePaperList.Width,
                    Length = printMachinePaperList.Length,
                    PaperName = printMachinePaperList.PaperName,
                    NumberOfPagesPerSheet = printMachinePaperList.NumberOfPagesPerSheet,
                    NumberOfPagesPerPlate = printMachinePaperList.NumberOfPagesPerPlate,
                    DateCreated = printMachinePaperList.DateCreated,
                    LastModified = printMachinePaperList.LastModified,
                    CreatedBy = printMachinePaperList.CreatedBy,
                    ModifiedBy = printMachinePaperList.ModifiedBy
                };

                db.PrintMachinePaperLists.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printMachinePaperList }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintMachinePaperLists_Destroy([DataSourceRequest]DataSourceRequest request, PrintMachinePaperList printMachinePaperList)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintMachinePaperList
                {
                    PrintMachinePaperListId = printMachinePaperList.PrintMachinePaperListId,
                    PrintMachineTypeId = printMachinePaperList.PrintMachineTypeId,
                    PaperSizeCode = printMachinePaperList.PaperSizeCode,
                    Width = printMachinePaperList.Width,
                    Length = printMachinePaperList.Length,
                    PaperName = printMachinePaperList.PaperName,
                    NumberOfPagesPerSheet = printMachinePaperList.NumberOfPagesPerSheet,
                    NumberOfPagesPerPlate = printMachinePaperList.NumberOfPagesPerPlate,
                    DateCreated = printMachinePaperList.DateCreated,
                    LastModified = printMachinePaperList.LastModified,
                    CreatedBy = printMachinePaperList.CreatedBy,
                    ModifiedBy = printMachinePaperList.ModifiedBy
                };

                db.PrintMachinePaperLists.Attach(entity);
                db.PrintMachinePaperLists.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { printMachinePaperList }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
