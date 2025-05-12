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
    public class PrintingMachineTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["PaperSize"] = db.PrintingPaperSizes.Select(s => new
            {
                Value = s.PrintingPaperSizeId,
                Text = s.PrintPaperSize.ToString()
            });
            return View();
        }

        public ActionResult PrintingMachineType_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingMachineType> PrintingMachineType = db.PrintingMachineTypes;
           
            return Json(PrintingMachineType.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingMachineType_Create([DataSourceRequest]DataSourceRequest request, PrintingMachineType model)
        {
            if(model.NumberOfColors == 0 || model.OutputRate == 0)
            {
                ModelState.AddModelError(string.Empty, "Output Rate or/and Number of color is required");
            }
            if (ModelState.IsValid)
            {
                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == model.PaperSizeId);
                var entity = new PrintingMachineType
                {
                                        
                    MachineTypeName = model.MachineTypeName,
                    
                    NumberOfColors = model.NumberOfColors,
                    OutputRate = model.OutputRate,
                    DateCreated = model.DateCreated,
                    LastModified = model.LastModified,
                    CreatedBy = model.CreatedBy,
                    ModifiedBy = model.ModifiedBy,
                    PrintSide = model.PrintSide,
                    PaperSizeId = model.PaperSizeId,
                    
                };

                db.PrintingMachineTypes.Add(entity);
                db.SaveChanges();
                GenerateSectionUp(entity);
                model.PrintingMachineTypeId = entity.PrintingMachineTypeId;
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingMachineType_Update([DataSourceRequest]DataSourceRequest request, PrintingMachineType model)
        {
            if (model.NumberOfColors == 0 || model.OutputRate == 0)
            {
                ModelState.AddModelError(string.Empty, "Output Rate or/and Number of color is required");
            }
            if (ModelState.IsValid)
            {
                var entity = new PrintingMachineType
                {
                    PrintingMachineTypeId = model.PrintingMachineTypeId,
                                     
                    MachineTypeName = model.MachineTypeName,
                    PaperSizeId = model.PaperSizeId,
                    PrintSide = model.PrintSide,
                    
                    NumberOfColors = model.NumberOfColors,
                    OutputRate = model.OutputRate,
                    DateCreated = model.DateCreated,
                    LastModified = model.LastModified,
                    CreatedBy = model.CreatedBy,
                    ModifiedBy = model.ModifiedBy,
                };

                db.PrintingMachineTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                DeleteDetailRecords(entity.PrintingMachineTypeId);
                GenerateSectionUp(entity);
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingMachineType_Delete([DataSourceRequest]DataSourceRequest request, PrintingMachineType printingMachineList)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingMachineType
                {
                    PrintingMachineTypeId = printingMachineList.PrintingMachineTypeId,
                    MachineTypeName = printingMachineList.MachineTypeName,
                    NumberOfColors = printingMachineList.NumberOfColors,
                    OutputRate = printingMachineList.OutputRate,
                    DateCreated = printingMachineList.DateCreated,
                    LastModified = printingMachineList.LastModified,
                    CreatedBy = printingMachineList.CreatedBy,
                    ModifiedBy = printingMachineList.ModifiedBy,
                    PaperSizeId = printingMachineList.PaperSizeId,
                    PrintSide = printingMachineList.PrintSide
                };
                DeleteDetailRecords(entity.PrintingMachineTypeId);
                db.PrintingMachineTypes.Attach(entity);
                db.PrintingMachineTypes.Remove(entity);
                db.SaveChanges();
                
            }

            return Json(new[] { printingMachineList }.ToDataSourceResult(request, ModelState));
        }
        
        private void GenerateSectionUp(PrintingMachineType machineType) 
        {
            var fromPaper = db.PrintingPaperSizes.Find(machineType.PaperSizeId);
            foreach (var toPaper in db.PrintingPaperSizes.ToList())
            {
                var upRatio = PaperSizeHelper.GetPaperUpRatio(fromPaper.PrintPaperSize, toPaper.PrintPaperSize);
                if (upRatio == null) continue; // Skip cross-series comparisons
                var entity = new PrintingMachineDetail
                {
                    PaperSizeId = toPaper.PrintingPaperSizeId,
                    PrintingMachineTypeId = machineType.PrintingMachineTypeId,
                    SectionUp = (int)upRatio 
                };
                db.PrintingMachineDetails.Add(entity);
                db.SaveChanges();
            }
        }
        private void DeleteDetailRecords(int machineTypeId)
        {
            var entity = db.PrintingMachineDetails.Where(x => x.PrintingMachineTypeId == machineTypeId);
            db.PrintingMachineDetails.RemoveRange(entity);
            db.SaveChanges();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
