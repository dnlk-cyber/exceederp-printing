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
            ViewData["MachineType"] = db.PrintingMachineTypes.Select(s => new
            {
                Value = s.PrintingMachineTypeId,
                Text = s.MachineTypeName
            }).ToList();
            ViewData["PaperSize"] = db.PrintingPaperSizes.Select(s => new
            {
                Value = s.PrintingPaperSizeId,
                Text = s.PaperSizeName
            }).ToList();
            return View();
        }

        public ActionResult PrintMachinePaperLists_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintMachinePaperList> printmachinepaperlists = db.PrintMachinePaperLists;
            DataSourceResult result = printmachinepaperlists.ToDataSourceResult(request, printMachinePaperList => new PrintMachinePaperList
            {
                PrintMachinePaperListId = printMachinePaperList.PrintMachinePaperListId,
                PrintMachineTypeId = printMachinePaperList.PrintMachineTypeId,
                
                Width = printMachinePaperList.Width,
                Length = printMachinePaperList.Length,
                
                NumberOfPagesPerSheet = printMachinePaperList.NumberOfPagesPerSheet,
                NumberOfPagesPerPlate = printMachinePaperList.NumberOfPagesPerPlate,
                DateCreated = printMachinePaperList.DateCreated,
                LastModified = printMachinePaperList.LastModified,
                CreatedBy = printMachinePaperList.CreatedBy,
                ModifiedBy = printMachinePaperList.ModifiedBy,
                PrintingPaperSizeId = printMachinePaperList.PrintingPaperSizeId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintMachinePaperLists_Create([DataSourceRequest]DataSourceRequest request, PrintMachinePaperList printMachinePaperList)
        {
            if (ModelState.IsValid)
            {
                var printingPaperSize = db.PrintingPaperSizes.Find(printMachinePaperList.PrintingPaperSizeId);
                printMachinePaperList.Width = printingPaperSize.Width;
                printMachinePaperList.Length = printingPaperSize.Length;
                printMachinePaperList.NumberOfPagesPerSheet = printingPaperSize.NoOfPagesPerSheet;
                printMachinePaperList.NumberOfPagesPerPlate = printingPaperSize.NoOfPagesPerPlate;
                var entity = new PrintMachinePaperList
                {
                    PrintMachineTypeId = printMachinePaperList.PrintMachineTypeId,
                    PrintingPaperSizeId = printMachinePaperList.PrintingPaperSizeId,
                    
                    Width = printingPaperSize.Width,
                    Length = printingPaperSize.Length,
                    
                    NumberOfPagesPerSheet = printingPaperSize.NoOfPagesPerSheet,
                    NumberOfPagesPerPlate = printingPaperSize.NoOfPagesPerPlate,
                    DateCreated = DateTime.Today,
                    LastModified = printMachinePaperList.LastModified,
                    CreatedBy = User.Identity.Name,
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
                var printingPaperSize = db.PrintingPaperSizes.Find(printMachinePaperList.PrintingPaperSizeId);
                printMachinePaperList.Width = printingPaperSize.Width;
                printMachinePaperList.Length = printingPaperSize.Length;
                printMachinePaperList.NumberOfPagesPerSheet = printingPaperSize.NoOfPagesPerSheet;
                printMachinePaperList.NumberOfPagesPerPlate = printingPaperSize.NoOfPagesPerPlate;
                var entity = new PrintMachinePaperList
                {
                    PrintMachinePaperListId = printMachinePaperList.PrintMachinePaperListId,
                    PrintMachineTypeId = printMachinePaperList.PrintMachineTypeId,
                    PrintingPaperSizeId = printMachinePaperList.PrintingPaperSizeId,
                    Width = printingPaperSize.Width,
                    Length = printingPaperSize.Length,

                    NumberOfPagesPerSheet = printingPaperSize.NoOfPagesPerSheet,
                    NumberOfPagesPerPlate = printingPaperSize.NoOfPagesPerPlate,
                    DateCreated = printMachinePaperList.DateCreated,
                    LastModified = DateTime.Today,
                    CreatedBy = printMachinePaperList.CreatedBy,
                    ModifiedBy = User.Identity.Name
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
                    PrintingPaperSizeId = printMachinePaperList.PrintingPaperSizeId,
                    Width = printMachinePaperList.Width,
                    Length = printMachinePaperList.Length,
                    
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
