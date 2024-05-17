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
    public class PrintingPaperSizeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintingPaperSizes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingPaperSize> printingpapersizes = db.PrintingPaperSizes;
            DataSourceResult result = printingpapersizes.ToDataSourceResult(request, printingPaperSize => new {
                PrintingPaperSizeId = printingPaperSize.PrintingPaperSizeId,
                PaperSizeName = printingPaperSize.PaperSizeName,
                NoOfPagesPerSheet = printingPaperSize.NoOfPagesPerSheet,
                NoOfPagesPerPlate = printingPaperSize.NoOfPagesPerPlate,
                Width = printingPaperSize.Width,
                Length = printingPaperSize.Length,
                NoOfPagesToA1Ratio = printingPaperSize.NoOfPagesToA1Ratio,
                Remark = printingPaperSize.Remark,
                PrintingMachineTypeId = printingPaperSize.PrintingMachineTypeId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingPaperSizes_Create([DataSourceRequest]DataSourceRequest request, PrintingPaperSize printingPaperSize)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingPaperSize
                {
                    PaperSizeName = printingPaperSize.PaperSizeName,
                    NoOfPagesPerSheet = printingPaperSize.NoOfPagesPerSheet,
                    NoOfPagesPerPlate = printingPaperSize.NoOfPagesPerPlate,
                    Width = printingPaperSize.Width,
                    Length = printingPaperSize.Length,
                    NoOfPagesToA1Ratio = printingPaperSize.NoOfPagesToA1Ratio,
                    Remark = printingPaperSize.Remark,
                    PrintingMachineTypeId = printingPaperSize.PrintingMachineTypeId
                };

                db.PrintingPaperSizes.Add(entity);
                db.SaveChanges();
                printingPaperSize.PrintingPaperSizeId = entity.PrintingPaperSizeId;
            }

            return Json(new[] { printingPaperSize }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingPaperSizes_Update([DataSourceRequest]DataSourceRequest request, PrintingPaperSize printingPaperSize)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingPaperSize
                {
                    PrintingPaperSizeId = printingPaperSize.PrintingPaperSizeId,
                    PaperSizeName = printingPaperSize.PaperSizeName,
                    NoOfPagesPerSheet = printingPaperSize.NoOfPagesPerSheet,
                    NoOfPagesPerPlate = printingPaperSize.NoOfPagesPerPlate,
                    Width = printingPaperSize.Width,
                    Length = printingPaperSize.Length,
                    NoOfPagesToA1Ratio = printingPaperSize.NoOfPagesToA1Ratio,
                    Remark = printingPaperSize.Remark,
                    PrintingMachineTypeId = printingPaperSize.PrintingMachineTypeId
                };

                db.PrintingPaperSizes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printingPaperSize }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingPaperSizes_Destroy([DataSourceRequest]DataSourceRequest request, PrintingPaperSize printingPaperSize)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingPaperSize
                {
                    PrintingPaperSizeId = printingPaperSize.PrintingPaperSizeId,
                    PaperSizeName = printingPaperSize.PaperSizeName,
                    NoOfPagesPerSheet = printingPaperSize.NoOfPagesPerSheet,
                    NoOfPagesPerPlate = printingPaperSize.NoOfPagesPerPlate,
                    Width = printingPaperSize.Width,
                    Length = printingPaperSize.Length,
                    NoOfPagesToA1Ratio = printingPaperSize.NoOfPagesToA1Ratio,
                    Remark = printingPaperSize.Remark,
                    PrintingMachineTypeId = printingPaperSize.PrintingMachineTypeId
                };

                db.PrintingPaperSizes.Attach(entity);
                db.PrintingPaperSizes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { printingPaperSize }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
