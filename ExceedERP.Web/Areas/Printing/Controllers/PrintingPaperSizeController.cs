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
using ExceedERP.Helpers.Printing;

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
                int upRatio = 0;
                if(model.PrintPaperSize == PrintPaperSize.A1 || model.PrintPaperSize == PrintPaperSize.A2 || 
                    model.PrintPaperSize == PrintPaperSize.A3 || model.PrintPaperSize == PrintPaperSize.A4 ||
                    model.PrintPaperSize == PrintPaperSize.A5 || model.PrintPaperSize == PrintPaperSize.A6)
                {
                    upRatio = (int)PaperSizeHelper.GetPaperUpRatio(PrintPaperSize.A1, model.PrintPaperSize);
                }
                else
                {
                    upRatio = (int)PaperSizeHelper.GetPaperUpRatio(PrintPaperSize.B1, model.PrintPaperSize);
                }
                
                var entity = new PrintingPaperSize
                {
                    PaperSizeName = model.PaperSizeName,
                    PrintPaperSize = model.PrintPaperSize,
                    Width = model.Width,
                    Length = model.Length,
                    NoOfPagesToA1Ratio = upRatio,
                    Remark = model.Remark,
                };

                db.PrintingPaperSizes.Add(entity);
                db.SaveChanges();
                model.PrintingPaperSizeId = entity.PrintingPaperSizeId;
                model.NoOfPagesToA1Ratio = entity.NoOfPagesToA1Ratio;
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingPaperSizes_Update([DataSourceRequest]DataSourceRequest request, PrintingPaperSize model)
        {
            if (ModelState.IsValid)
            {
                int upRatio = 0;
                if (model.PrintPaperSize == PrintPaperSize.A1 || model.PrintPaperSize == PrintPaperSize.A2 ||
                    model.PrintPaperSize == PrintPaperSize.A3 || model.PrintPaperSize == PrintPaperSize.A4 ||
                    model.PrintPaperSize == PrintPaperSize.A5 || model.PrintPaperSize == PrintPaperSize.A6)
                {
                    upRatio = (int)PaperSizeHelper.GetPaperUpRatio(PrintPaperSize.A1, model.PrintPaperSize);
                }
                else
                {
                    upRatio = (int)PaperSizeHelper.GetPaperUpRatio(PrintPaperSize.B1, model.PrintPaperSize);
                }
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

        
        private void GenerateAllStandardSizes()
        {
            var standardSizes = new List<PrintingPaperSize>
            {
                // A Series
                new PrintingPaperSize { PrintPaperSize = PrintPaperSize.A1, PaperSizeName = "A1", NoOfPagesToA1Ratio = 1, Width = 59.4, Length = 84.1 },
                new PrintingPaperSize { PrintPaperSize = PrintPaperSize.A2, PaperSizeName = "A2", NoOfPagesToA1Ratio = 2, Width = 42.0, Length = 59.4 },
                new PrintingPaperSize { PrintPaperSize = PrintPaperSize.A3, PaperSizeName = "A3", NoOfPagesToA1Ratio = 4, Width = 29.7, Length = 42.0 },
                new PrintingPaperSize { PrintPaperSize = PrintPaperSize.A4, PaperSizeName = "A4", NoOfPagesToA1Ratio = 8, Width = 21.0, Length = 29.7 },
                new PrintingPaperSize { PrintPaperSize = PrintPaperSize.A5, PaperSizeName = "A5", NoOfPagesToA1Ratio = 16, Width = 14.8, Length = 21.0 },
                new PrintingPaperSize { PrintPaperSize = PrintPaperSize.A6, PaperSizeName = "A6", NoOfPagesToA1Ratio = 32, Width = 10.5, Length = 14.8 },

                // B Series
                new PrintingPaperSize { PrintPaperSize = PrintPaperSize.B1, PaperSizeName = "B1", NoOfPagesToA1Ratio = 1, Width = 70.7, Length = 100.0 },
                new PrintingPaperSize { PrintPaperSize = PrintPaperSize.B2, PaperSizeName = "B2", NoOfPagesToA1Ratio = 2, Width = 50.0, Length = 70.7 },
                new PrintingPaperSize { PrintPaperSize = PrintPaperSize.B3, PaperSizeName = "B3", NoOfPagesToA1Ratio = 4, Width = 35.3, Length = 50.0 },
                new PrintingPaperSize { PrintPaperSize = PrintPaperSize.B4, PaperSizeName = "B4", NoOfPagesToA1Ratio = 8, Width = 25.0, Length = 35.3 },
                new PrintingPaperSize { PrintPaperSize = PrintPaperSize.B5, PaperSizeName = "B5", NoOfPagesToA1Ratio = 16, Width = 17.6, Length = 25.0 },
                new PrintingPaperSize { PrintPaperSize = PrintPaperSize.B6, PaperSizeName = "B6", NoOfPagesToA1Ratio = 32, Width = 12.5, Length = 17.6 }
            };

            foreach (var size in standardSizes) 
            {
                bool exists = db.PrintingPaperSizes.Any(p => p.PrintPaperSize == size.PrintPaperSize);
                if (!exists)
                {
                    size.Remark = "Auto-generated";
                    db.PrintingPaperSizes.Add(size);
                }
            }

            db.SaveChanges();
            
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
