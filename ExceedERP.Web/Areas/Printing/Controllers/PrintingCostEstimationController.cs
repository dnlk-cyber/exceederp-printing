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
    public class PrintingCostEstimationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();
            ViewData["JobType"] = db.PrintingJobTypes.Select(s => new
            {
                Value = s.PrintingJobTypeId,
                Text = s.JobTypeName
            }).ToList();
            ViewData["PaperSize"] = db.PrintingPaperSizes.Select(s => new
            {
                Value = s.PrintingPaperSizeId,
                Text = s.PaperSizeName
            }).ToList();
            ViewData["BindingStyle"] = db.PrintingBindingStyles.Select(s => new
            {
                Value = s.PrintingBindingStyleId,
                Text = s.BindingStyleName
            }).ToList();

            ViewData["Category"] = db.PrintingMaterialCategories.Select(s => new
            {
                Value = s.PrintingMaterialCategoryId,
                Text = s.CategoryName
            }).ToList();
            ViewData["Item"] = db.PrintingMaterialCategoryItems.Select(s => new
            {
                Value = s.PrintingMaterialCategoryItemId,
                Text = s.ItemName
            }).ToList();

            return View();
        }

        public ActionResult PrintingCostEstimations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingCostEstimation> printingcostestimations = db.PrintingCostEstimations;
            DataSourceResult result = printingcostestimations.ToDataSourceResult(request, printingCostEstimation => new PrintingCostEstimation
            {
                PrintingCostEstimationId = printingCostEstimation.PrintingCostEstimationId,
                PaperSizeId = printingCostEstimation.PaperSizeId,
                CustomerId = printingCostEstimation.CustomerId,
                JobTypeId = printingCostEstimation.JobTypeId,
                ProductQuantity = printingCostEstimation.ProductQuantity,
                TotalTextNoPages = printingCostEstimation.TotalTextNoPages,
                BindingStyleId = printingCostEstimation.BindingStyleId,
                Width = printingCostEstimation.Width,
                Length = printingCostEstimation.Length,
                TextNoOfColors = printingCostEstimation.TextNoOfColors,
                CoverNoOfColor = printingCostEstimation.CoverNoOfColor,
                CoverPaperSize = printingCostEstimation.CoverPaperSize,
                CoverRequired = printingCostEstimation.CoverRequired,
                OnlyOneSideCover = printingCostEstimation.OnlyOneSideCover,
                NoUpPagesPerA1 = printingCostEstimation.NoUpPagesPerA1,
                ProcessCategoryId = printingCostEstimation.ProcessCategoryId,
                TypeOfWorkId = printingCostEstimation.TypeOfWorkId,
                EstimatedHrs = printingCostEstimation.EstimatedHrs,
                DLaborRate = printingCostEstimation.DLaborRate,
                TotalCost = printingCostEstimation.TotalCost,
                OverHeadRate = printingCostEstimation.OverHeadRate,
                OverHeadCost = printingCostEstimation.OverHeadCost,
                TotalLaborCost = printingCostEstimation.TotalLaborCost,
                DateCreated = printingCostEstimation.DateCreated,
                LastModified = printingCostEstimation.LastModified,
                CreatedBy = printingCostEstimation.CreatedBy,
                ModifiedBy = printingCostEstimation.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingCostEstimations_Create([DataSourceRequest]DataSourceRequest request, PrintingCostEstimation printingCostEstimation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingCostEstimation
                {
                    PaperSizeId = printingCostEstimation.PaperSizeId,
                    CustomerId = printingCostEstimation.CustomerId,
                    JobTypeId = printingCostEstimation.JobTypeId,
                    ProductQuantity = printingCostEstimation.ProductQuantity,
                    TotalTextNoPages = printingCostEstimation.TotalTextNoPages,
                    BindingStyleId = printingCostEstimation.BindingStyleId,
                    Width = printingCostEstimation.Width,
                    Length = printingCostEstimation.Length,
                    TextNoOfColors = printingCostEstimation.TextNoOfColors,
                    CoverNoOfColor = printingCostEstimation.CoverNoOfColor,
                    CoverPaperSize = printingCostEstimation.CoverPaperSize,
                    CoverRequired = printingCostEstimation.CoverRequired,
                    OnlyOneSideCover = printingCostEstimation.OnlyOneSideCover,
                    NoUpPagesPerA1 = printingCostEstimation.NoUpPagesPerA1,
                    ProcessCategoryId = printingCostEstimation.ProcessCategoryId,
                    TypeOfWorkId = printingCostEstimation.TypeOfWorkId,
                    EstimatedHrs = printingCostEstimation.EstimatedHrs,
                    DLaborRate = printingCostEstimation.DLaborRate,
                    TotalCost = printingCostEstimation.TotalCost,
                    OverHeadRate = printingCostEstimation.OverHeadRate,
                    OverHeadCost = printingCostEstimation.OverHeadCost,
                    TotalLaborCost = printingCostEstimation.TotalLaborCost,
                    DateCreated = DateTime.Today,
                    LastModified = printingCostEstimation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = printingCostEstimation.ModifiedBy
                };

                db.PrintingCostEstimations.Add(entity);
                db.SaveChanges();
                printingCostEstimation.PrintingCostEstimationId = entity.PrintingCostEstimationId;
            }

            return Json(new[] { printingCostEstimation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingCostEstimations_Update([DataSourceRequest]DataSourceRequest request, PrintingCostEstimation printingCostEstimation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingCostEstimation
                {
                    PrintingCostEstimationId = printingCostEstimation.PrintingCostEstimationId,
                    PaperSizeId = printingCostEstimation.PaperSizeId,
                    CustomerId = printingCostEstimation.CustomerId,
                    JobTypeId = printingCostEstimation.JobTypeId,
                    ProductQuantity = printingCostEstimation.ProductQuantity,
                    TotalTextNoPages = printingCostEstimation.TotalTextNoPages,
                    BindingStyleId = printingCostEstimation.BindingStyleId,
                    Width = printingCostEstimation.Width,
                    Length = printingCostEstimation.Length,
                    TextNoOfColors = printingCostEstimation.TextNoOfColors,
                    CoverNoOfColor = printingCostEstimation.CoverNoOfColor,
                    CoverPaperSize = printingCostEstimation.CoverPaperSize,
                    CoverRequired = printingCostEstimation.CoverRequired,
                    OnlyOneSideCover = printingCostEstimation.OnlyOneSideCover,
                    NoUpPagesPerA1 = printingCostEstimation.NoUpPagesPerA1,
                    ProcessCategoryId = printingCostEstimation.ProcessCategoryId,
                    TypeOfWorkId = printingCostEstimation.TypeOfWorkId,
                    EstimatedHrs = printingCostEstimation.EstimatedHrs,
                    DLaborRate = printingCostEstimation.DLaborRate,
                    TotalCost = printingCostEstimation.TotalCost,
                    OverHeadRate = printingCostEstimation.OverHeadRate,
                    OverHeadCost = printingCostEstimation.OverHeadCost,
                    TotalLaborCost = printingCostEstimation.TotalLaborCost,
                    DateCreated = printingCostEstimation.DateCreated,
                    LastModified = printingCostEstimation.LastModified,
                    CreatedBy = printingCostEstimation.CreatedBy,
                    ModifiedBy = printingCostEstimation.ModifiedBy
                };

                db.PrintingCostEstimations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printingCostEstimation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingCostEstimations_Destroy([DataSourceRequest]DataSourceRequest request, PrintingCostEstimation printingCostEstimation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingCostEstimation
                {
                    PrintingCostEstimationId = printingCostEstimation.PrintingCostEstimationId,
                    PaperSizeId = printingCostEstimation.PaperSizeId,
                    CustomerId = printingCostEstimation.CustomerId,
                    JobTypeId = printingCostEstimation.JobTypeId,
                    ProductQuantity = printingCostEstimation.ProductQuantity,
                    TotalTextNoPages = printingCostEstimation.TotalTextNoPages,
                    BindingStyleId = printingCostEstimation.BindingStyleId,
                    Width = printingCostEstimation.Width,
                    Length = printingCostEstimation.Length,
                    TextNoOfColors = printingCostEstimation.TextNoOfColors,
                    CoverNoOfColor = printingCostEstimation.CoverNoOfColor,
                    CoverPaperSize = printingCostEstimation.CoverPaperSize,
                    CoverRequired = printingCostEstimation.CoverRequired,
                    OnlyOneSideCover = printingCostEstimation.OnlyOneSideCover,
                    NoUpPagesPerA1 = printingCostEstimation.NoUpPagesPerA1,
                    ProcessCategoryId = printingCostEstimation.ProcessCategoryId,
                    TypeOfWorkId = printingCostEstimation.TypeOfWorkId,
                    EstimatedHrs = printingCostEstimation.EstimatedHrs,
                    DLaborRate = printingCostEstimation.DLaborRate,
                    TotalCost = printingCostEstimation.TotalCost,
                    OverHeadRate = printingCostEstimation.OverHeadRate,
                    OverHeadCost = printingCostEstimation.OverHeadCost,
                    TotalLaborCost = printingCostEstimation.TotalLaborCost,
                    DateCreated = printingCostEstimation.DateCreated,
                    LastModified = printingCostEstimation.LastModified,
                    CreatedBy = printingCostEstimation.CreatedBy,
                    ModifiedBy = printingCostEstimation.ModifiedBy
                };

                db.PrintingCostEstimations.Attach(entity);
                db.PrintingCostEstimations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { printingCostEstimation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
