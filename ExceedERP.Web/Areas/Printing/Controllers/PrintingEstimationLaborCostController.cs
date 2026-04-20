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
                Exist = "Labor cost exists",
                MissingMaterial = "Material cost missing"

            };
            var laborCost = db.PrintingEstimationLaborCosts.Where(x => x.PrintingCostEstimationId == estimationId);
            if (laborCost.Any())
            {
                return Json(msg.Exist, JsonRequestBehavior.AllowGet);
            }
            var costEstimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            var materialCost = db.PrintingEstimationMaterialCosts.Where(x => x.PrintingCostEstimationId == costEstimation.PrintingCostEstimationId).ToList();
            if (!materialCost.Any())
            {
                return Json(msg.MissingMaterial, JsonRequestBehavior.AllowGet);
            }
            var printingProcesses = db.PrintingProcesses.ToList();

            if (costEstimation.PrintJobType == PrintJobType.Book || costEstimation.PrintJobType == PrintJobType.Magazine || costEstimation.PrintJobType == PrintJobType.Newsletter)
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
            else if (costEstimation.PrintJobType == PrintJobType.Brochure || costEstimation.PrintJobType == PrintJobType.Card || costEstimation.PrintJobType == PrintJobType.Flyer ||
                    costEstimation.PrintJobType == PrintJobType.Format || costEstimation.PrintJobType == PrintJobType.Poster || costEstimation.PrintJobType == PrintJobType.Sticker)
            {
                foreach (var item in printingProcesses)
                {
                    switch (item.LabourActivity)
                    {
                        case LabourActivity.Design:
                            CardFlyerLaborCost.DesignCost(costEstimation, item);
                            break;
                        case LabourActivity.Composition:
                            CardFlyerLaborCost.CompositionCost(costEstimation, item);
                            break;
                        case LabourActivity.TextPlateMaking:
                            CardFlyerLaborCost.PlateMakingCost(costEstimation, item);
                            break;
                       
                        case LabourActivity.Proofing:
                            CardFlyerLaborCost.ProofingCost(costEstimation, item);
                            break;
                        case LabourActivity.MakeReadyText:
                            CardFlyerLaborCost.TextMakeReadyCost(costEstimation, item);
                            break;
                       
                        case LabourActivity.TextPrint:
                            CardFlyerLaborCost.TextPrintCost(costEstimation, item);
                            break;
                       
                        case LabourActivity.CuttingForText:
                            CardFlyerLaborCost.CuttingTextCost(costEstimation, item);
                            break;

                        case LabourActivity.Trimming:
                            CardFlyerLaborCost.TrimmingCost(costEstimation, item);
                            break;
                        case LabourActivity.CheckingAndPacking:
                            CardFlyerLaborCost.CheckingPackingCost(costEstimation, item);
                            break;
                        case LabourActivity.LoadingAndUnloading:
                            CardFlyerLaborCost.LoadingUnloadingCost(costEstimation, item);
                            break;
                        case LabourActivity.DispatchingAndTransport:
                            CardFlyerLaborCost.DispatchingTransportingCost(costEstimation, item);
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (costEstimation.PrintJobType == PrintJobType.Voucher)
            {
                foreach (var item in printingProcesses)
                {
                    switch (item.LabourActivity)
                    {
                        case LabourActivity.Design:
                            VoucherLaborCost.DesignCost(costEstimation, item);
                            break;
                        case LabourActivity.Composition:
                            VoucherLaborCost.CompositionCost(costEstimation, item);
                            break;
                        case LabourActivity.TextPlateMaking:
                            VoucherLaborCost.TextPlateMakingCost(costEstimation, item);
                            break;
                        case LabourActivity.CoverPlateMaking:
                            VoucherLaborCost.CoverPlateMakingCost(costEstimation, item);
                            break;
                        case LabourActivity.Proofing:
                            VoucherLaborCost.ProofingCost(costEstimation, item);
                            break;
                        case LabourActivity.MakeReadyText:
                            VoucherLaborCost.TextMakeReadyCost(costEstimation, item);
                            break;
                        case LabourActivity.MakeReadyNumbering:
                            VoucherLaborCost.MakeReadyNumberingCost(costEstimation, item);
                            break;
                        case LabourActivity.MakeReadyCover:
                            VoucherLaborCost.CoverMakeReadyCost(costEstimation, item);
                            break;
                        case LabourActivity.TextPrint:
                            VoucherLaborCost.OffsetPrint(costEstimation, item);
                            break;
                        case LabourActivity.LetterPressPrint:
                            VoucherLaborCost.LetterPressPrint(costEstimation, item);
                            break;
                        case LabourActivity.CuttingForCover:
                            VoucherLaborCost.CuttingCoverCost(costEstimation, item);
                            break;
                        case LabourActivity.CuttingForText:
                            VoucherLaborCost.CuttingTextCost(costEstimation, item);
                            break;

                        case LabourActivity.Perforation:
                            VoucherLaborCost.PerforationCost(costEstimation, item);
                            break;
                        case LabourActivity.CuttingForNumbering:
                            VoucherLaborCost.CuttingForNumberingCost(costEstimation, item);
                            break;
                        case LabourActivity.Stitching:
                            VoucherLaborCost.SideStitchingCost(costEstimation, item);
                            break;
                        case LabourActivity.Collating:
                            VoucherLaborCost.CollatingCost(costEstimation, item);
                            break;
                        
                        case LabourActivity.Trimming:
                            VoucherLaborCost.TrimmingCost(costEstimation, item);
                            break;
                        case LabourActivity.CheckingAndPacking:
                            VoucherLaborCost.CheckingPackingCost(costEstimation, item);
                            break;
                        case LabourActivity.LoadingAndUnloading:
                            VoucherLaborCost.LoadingUnloadingCost(costEstimation, item);
                            break;
                        case LabourActivity.DispatchingAndTransport:
                            VoucherLaborCost.DispatchingTransportingCost(costEstimation, item);
                            break;
                        default:
                            break;
                    }
                }
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
