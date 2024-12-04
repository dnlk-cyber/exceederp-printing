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
    public class PrintingEstimationLaborCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintingEstimationLaborCosts_Read([DataSourceRequest]DataSourceRequest request, int estId)
        {
            IQueryable<PrintingEstimationLaborCost> printingestimationlaborcosts = db.PrintingEstimationLaborCosts.Where(x => x.PrintingCostEstimationId == estId);
            DataSourceResult result = printingestimationlaborcosts.ToDataSourceResult(request, printingEstimationLaborCost => new PrintingEstimationLaborCost
            {
                PrintingEstimationLaborCostId = printingEstimationLaborCost.PrintingEstimationLaborCostId,
                ProcessCategory = printingEstimationLaborCost.ProcessCategory,
                PrintingProcessId = printingEstimationLaborCost.PrintingProcessId,
                EstimatedHours = printingEstimationLaborCost.EstimatedHours,
                LaborRate = printingEstimationLaborCost.LaborRate,
                TotalCost = printingEstimationLaborCost.TotalCost,
                DateCreated = printingEstimationLaborCost.DateCreated,
                LastModified = printingEstimationLaborCost.LastModified,
                CreatedBy = printingEstimationLaborCost.CreatedBy,
                ModifiedBy = printingEstimationLaborCost.ModifiedBy,
                PrintingCostEstimationId = printingEstimationLaborCost.PrintingCostEstimationId,
                PrintingMachineTypeId =printingEstimationLaborCost.PrintingMachineTypeId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingEstimationLaborCosts_Create([DataSourceRequest]DataSourceRequest request, PrintingEstimationLaborCost printingEstimationLaborCost, int estId)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingEstimationLaborCost
                {
                    ProcessCategory = printingEstimationLaborCost.ProcessCategory,
                    PrintingProcessId = printingEstimationLaborCost.PrintingProcessId,
                    EstimatedHours = printingEstimationLaborCost.EstimatedHours,
                    LaborRate = printingEstimationLaborCost.LaborRate,
                    TotalCost = printingEstimationLaborCost.TotalCost,
                    DateCreated = DateTime.Today,
                    LastModified = printingEstimationLaborCost.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = printingEstimationLaborCost.ModifiedBy,
                    PrintingCostEstimationId = estId,
                    PrintingMachineTypeId = printingEstimationLaborCost.PrintingMachineTypeId,
                    
                };
                entity.TotalCost = entity.LaborRate * entity.EstimatedHours;
                db.PrintingEstimationLaborCosts.Add(entity);
                db.SaveChanges();
                printingEstimationLaborCost.TotalCost = entity.TotalCost;
                printingEstimationLaborCost.PrintingEstimationLaborCostId = entity.PrintingEstimationLaborCostId;
            }

            return Json(new[] { printingEstimationLaborCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingEstimationLaborCosts_Update([DataSourceRequest]DataSourceRequest request, PrintingEstimationLaborCost printingEstimationLaborCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingEstimationLaborCost
                {
                    PrintingEstimationLaborCostId = printingEstimationLaborCost.PrintingEstimationLaborCostId,
                    ProcessCategory = printingEstimationLaborCost.ProcessCategory,
                    PrintingProcessId = printingEstimationLaborCost.PrintingProcessId,
                    EstimatedHours = printingEstimationLaborCost.EstimatedHours,
                    LaborRate = printingEstimationLaborCost.LaborRate,
                    TotalCost = printingEstimationLaborCost.TotalCost,
                    DateCreated = printingEstimationLaborCost.DateCreated,
                    LastModified = printingEstimationLaborCost.LastModified,
                    CreatedBy = printingEstimationLaborCost.CreatedBy,
                    ModifiedBy = printingEstimationLaborCost.ModifiedBy,
                    PrintingCostEstimationId = printingEstimationLaborCost.PrintingCostEstimationId,
                    PrintingMachineTypeId = printingEstimationLaborCost.PrintingMachineTypeId
                };
                entity.TotalCost = entity.LaborRate * entity.EstimatedHours;

                db.PrintingEstimationLaborCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printingEstimationLaborCost }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult PrintingEstimationLaborCosts_Delete([DataSourceRequest]DataSourceRequest request, PrintingEstimationLaborCost printingEstimationLaborCost)
        {
            if (ModelState.IsValid)
            {
                var laborCost = db.PrintingEstimationLaborCosts.Find(printingEstimationLaborCost.PrintingEstimationLaborCostId);

                db.PrintingEstimationLaborCosts.Remove(laborCost);              
                db.SaveChanges();
            }

            return Json(new[] { printingEstimationLaborCost }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
