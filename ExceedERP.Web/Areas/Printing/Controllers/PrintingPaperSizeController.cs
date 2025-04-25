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
            
            return Json(printingpapersizes.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingPaperSizes_Create([DataSourceRequest]DataSourceRequest request, PrintingPaperSize model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingPaperSize
                {
                    PaperSizeName = model.PaperSizeName,
                    PrintPaperSize = model.PrintPaperSize,
                    Width = model.Width,
                    Length = model.Length,
                    NoOfPagesToA1Ratio = model.NoOfPagesToA1Ratio,
                    Remark = model.Remark,
                };

                db.PrintingPaperSizes.Add(entity);
                db.SaveChanges();
                model.PrintingPaperSizeId = entity.PrintingPaperSizeId;
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingPaperSizes_Update([DataSourceRequest]DataSourceRequest request, PrintingPaperSize model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingPaperSize
                {
                    PrintingPaperSizeId = model.PrintingPaperSizeId,
                    PaperSizeName = model.PaperSizeName,
                    Width = model.Width,
                    Length = model.Length,
                    NoOfPagesToA1Ratio = model.NoOfPagesToA1Ratio,
                    Remark = model.Remark,
                    PrintPaperSize = model.PrintPaperSize,
                };

                db.PrintingPaperSizes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingPaperSizes_Destroy([DataSourceRequest]DataSourceRequest request, PrintingPaperSize model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingPaperSize
                {
                    PrintingPaperSizeId = model.PrintingPaperSizeId,
                    PaperSizeName = model.PaperSizeName,
                    Width = model.Width,
                    Length = model.Length,
                    NoOfPagesToA1Ratio = model.NoOfPagesToA1Ratio,
                    Remark = model.Remark,
                    PrintPaperSize = model.PrintPaperSize,
                };

                db.PrintingPaperSizes.Attach(entity);
                db.PrintingPaperSizes.Remove(entity);
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
