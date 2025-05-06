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
           
            return Json(printingestimationlaborcosts.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create_LaborCost(int estimationId)
        {

            var msg = new
            {
                success = "Successfully Created Labor Cost",
                error = "Failed! something went wrong!",
                Exist = "Labor cost exists"

            };
            var laborCost = db.PrintingEstimationLaborCosts.Where(x => x.PrintingCostEstimationId == estimationId);
            if (laborCost.Any())
            {
                return Json(msg.Exist, JsonRequestBehavior.AllowGet);
            }
            var costEstimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            var printingProcesses = db.PrintingProcesses.ToList();
            
            if (costEstimation.PrintJobType == PrintJobType.Book || costEstimation.PrintJobType == PrintJobType.Magazine)
            {
                foreach (var item in printingProcesses) 
                {
                    switch (item.LabourActivity)
                    {
                        case LabourActivity.Design:
                            BookMagazineLaborCostEstimation.DesignCost(costEstimation, item);
                            break;
                        case LabourActivity.Composition:
                            BookMagazineLaborCostEstimation.CompositionCost(costEstimation, item);
                            break;                       
                        case LabourActivity.TextPlateMaking:
                            BookMagazineLaborCostEstimation.TextPlateMakingCost(costEstimation, item);
                            break;
                        case LabourActivity.CoverPlateMaking:
                            BookMagazineLaborCostEstimation.CoverPlateMakingCost(costEstimation, item);
                            break;                        
                        case LabourActivity.Proofing:
                            BookMagazineLaborCostEstimation.ProofingCost(costEstimation, item);
                            break;
                        case LabourActivity.MakeReadyText:
                            BookMagazineLaborCostEstimation.TextMakeReadyCost(costEstimation, item);
                            break;
                        case LabourActivity.MakeReadyCover:
                            BookMagazineLaborCostEstimation.CoverMakeReadyCost(costEstimation, item);
                            break;
                        case LabourActivity.TextPrint:
                            BookMagazineLaborCostEstimation.TextPrintCost(costEstimation, item);
                            break;
                        case LabourActivity.CoverPrint:
                            BookMagazineLaborCostEstimation.CoverPrintCost(costEstimation, item);
                            break;                      
                        case LabourActivity.CuttingForCover:
                            BookMagazineLaborCostEstimation.CuttingCoverCost(costEstimation, item);
                            break;
                        case LabourActivity.CuttingForText:
                            BookMagazineLaborCostEstimation.CuttingTextCost(costEstimation, item);
                            break; 
                        
                        case LabourActivity.FoldingCover:
                            BookMagazineLaborCostEstimation.FoldingCoverCost(costEstimation, item);
                            break;
                        case LabourActivity.Inserting:
                            BookMagazineLaborCostEstimation.InsertingCost(costEstimation, item);
                            break;
                        case LabourActivity.Stitching:
                            BookMagazineLaborCostEstimation.StitchingCost(costEstimation, item);
                            break;
                        case LabourActivity.Collating:
                            BookMagazineLaborCostEstimation.CollatingCost(costEstimation, item);
                            break;
                        case LabourActivity.PerfectBinding:
                            BookMagazineLaborCostEstimation.PerfectBindingCost(costEstimation, item);
                            break;
                        case LabourActivity.Trimming:
                            BookMagazineLaborCostEstimation.TrimmingCost(costEstimation, item);
                            break;
                        case LabourActivity.CheckingAndPacking:
                            BookMagazineLaborCostEstimation.CheckingPackingCost(costEstimation, item);
                            break;
                        case LabourActivity.LoadingAndUnloading:
                            BookMagazineLaborCostEstimation.LoadingUnloadingCost(costEstimation, item);
                            break;
                        case LabourActivity.DispatchingAndTransport:
                            BookMagazineLaborCostEstimation.DispatchingTransportingCost(costEstimation, item);
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (costEstimation.PrintJobType == PrintJobType.Brochure || costEstimation.PrintJobType == PrintJobType.Card || costEstimation.PrintJobType == PrintJobType.Card)
            {

            }
            return Json(msg.success, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingEstimationLaborCosts_Update([DataSourceRequest]DataSourceRequest request, PrintingEstimationLaborCost printingEstimationLaborCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingEstimationLaborCost
                {
                    PrintingEstimationLaborCostId = printingEstimationLaborCost.PrintingEstimationLaborCostId,
                    PressCategory = printingEstimationLaborCost.PressCategory,
                    PrintingProcessId = printingEstimationLaborCost.PrintingProcessId,
                    LaborRate = printingEstimationLaborCost.LaborRate,
                    TotalCost = printingEstimationLaborCost.TotalCost,
                    DateCreated = printingEstimationLaborCost.DateCreated,
                    LastModified = printingEstimationLaborCost.LastModified,
                    CreatedBy = printingEstimationLaborCost.CreatedBy,
                    ModifiedBy = printingEstimationLaborCost.ModifiedBy,
                    PrintingCostEstimationId = printingEstimationLaborCost.PrintingCostEstimationId,
                };

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
