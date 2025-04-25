using System;
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
            ViewData["Taxes"] = db.GLTaxRates.Select(s => new
            {
                Value = s.GLTaxRateID,
                Text = s.TaxName
            }).ToList();
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
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
            ViewData["PrintingProcess"] = db.PrintingProcesses.Select(s => new
            {
                Value = s.PrintingProcessId,
                Text = s.LabourActivity
            }).ToList();
            ViewData["PrintingMachineType"] = db.PrintingMachineTypes.Select(s => new
            {
                Value = s.PrintingMachineTypeId,
                Text = s.MachineTypeName
            }).ToList();

            return View();
        }

        public ActionResult PrintingCostEstimations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingCostEstimation> printingcostestimations = db.PrintingCostEstimations;

            return Json(printingcostestimations.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingCostEstimations_Create([DataSourceRequest]DataSourceRequest request, PrintingCostEstimation model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingCostEstimation
                {
                    PrintJobType = model.PrintJobType,
                    TextPaperSizeId = model.TextPaperSizeId,
                    CustomerId = model.CustomerId,
                    
                    ProductQuantity = model.ProductQuantity,
                    Width = model.Width,
                    Length = model.Length,
                    TextNoOfColors = model.TextNoOfColors,
                    CoverNoOfColor = model.CoverNoOfColor,
                    CoverRequired = model.CoverRequired,
                    OnlyOneSideCover = model.OnlyOneSideCover,
                    
                    DateCreated = DateTime.Today,
                    LastModified = model.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = model.ModifiedBy,
                    JobDescription = model.JobDescription,
                    CoverMachineTypeId = model.CoverMachineTypeId,
                    TextMachineTypeId = model.TextMachineTypeId,
                    CoverPaperSizeId = model.CoverPaperSizeId,
                    Pages = model.Pages,
                    Pack = model.Pack,
                    UseLength = model.UseLength,
                    BindingStyle = model.BindingStyle,
                    TextPrintSide = model.TextPrintSide,
                    CoverPrintSide = model.CoverPrintSide
                };

                db.PrintingCostEstimations.Add(entity);
                db.SaveChanges();
                model.PrintingCostEstimationId = entity.PrintingCostEstimationId;
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingCostEstimations_Update([DataSourceRequest]DataSourceRequest request, PrintingCostEstimation model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingCostEstimation
                {
                    PrintJobType = model.PrintJobType,
                    PrintingCostEstimationId = model.PrintingCostEstimationId,
                    TextPaperSizeId = model.TextPaperSizeId,
                    CustomerId = model.CustomerId,
                    
                    ProductQuantity = model.ProductQuantity,
                    Width = model.Width,
                    Length = model.Length,
                    TextNoOfColors = model.TextNoOfColors,
                    CoverNoOfColor = model.CoverNoOfColor,
                    CoverRequired = model.CoverRequired,
                    OnlyOneSideCover = model.OnlyOneSideCover,
                   
                    DateCreated = model.DateCreated,
                    LastModified = model.LastModified,
                    CreatedBy = model.CreatedBy,
                    ModifiedBy = model.ModifiedBy,
                    JobDescription = model.JobDescription,
                    TextMachineTypeId = model.TextMachineTypeId,
                    CoverMachineTypeId = model.CoverMachineTypeId,
                    CoverPaperSizeId = model.CoverPaperSizeId,
                    Pages = model.Pages,
                    Pack = model.Pack,
                    UseLength = model.UseLength,
                    BindingStyle = model.BindingStyle,
                    TextPrintSide = model.TextPrintSide,
                    CoverPrintSide = model.CoverPrintSide
                };

                db.PrintingCostEstimations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingCostEstimations_Destroy([DataSourceRequest]DataSourceRequest request, PrintingCostEstimation model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingCostEstimation
                {
                    PrintJobType = model.PrintJobType,
                    PrintingCostEstimationId = model.PrintingCostEstimationId,
                    TextPaperSizeId = model.TextPaperSizeId,
                    CustomerId = model.CustomerId,
                    
                    ProductQuantity = model.ProductQuantity,
                    Width = model.Width,
                    Length = model.Length,
                    TextNoOfColors = model.TextNoOfColors,
                    CoverNoOfColor = model.CoverNoOfColor,
                    CoverRequired = model.CoverRequired,
                    OnlyOneSideCover = model.OnlyOneSideCover,
                    
                    DateCreated = model.DateCreated,
                    LastModified = model.LastModified,
                    CreatedBy = model.CreatedBy,
                    ModifiedBy = model.ModifiedBy,
                    JobDescription = model.JobDescription,
                    TextMachineTypeId = model.TextMachineTypeId,
                    CoverMachineTypeId = model.CoverMachineTypeId,
                    CoverPaperSizeId = model.CoverPaperSizeId,
                    Pages = model.Pages,
                    Pack = model.Pack,
                    UseLength = model.UseLength,
                    BindingStyle = model.BindingStyle,
                    TextPrintSide = model.TextPrintSide,
                    CoverPrintSide = model.CoverPrintSide
                };

                db.PrintingCostEstimations.Attach(entity);
                db.PrintingCostEstimations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult CalculateOverAllCost(int id)
        {
            var msg = new
            {
                success = "Calculation succeeded",
                error = "Failed to calculate make sure you have material and labour cost",
                exists = "Already calculated"
            };
            var estimation = db.PrintingCostEstimations.Find(id);
            if (estimation != null)
            {
                var overAllCost = db.PrintingOverAllCosts.Where(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId).ToList();
                if (overAllCost.Any()) return Json(msg.exists);
                var materialCosts = db.PrintingEstimationMaterialCosts.Where(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId).ToList();
                var labourCosts = db.PrintingEstimationLaborCosts.Where(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId).ToList();
                if (labourCosts.Any() && materialCosts.Any())
                {

                    var totalLabourCost = labourCosts.Sum(x => (decimal?)x.TotalCost) ?? 0;
                    var totalMaterialCost = materialCosts.Sum(s => (decimal?)s.TotalCost) ?? 0;
                    var entity = new PrintingOverAllCost
                    {
                        PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                        AdminstrativeCost = 0,
                        IndirectMaterialCost = totalMaterialCost * 0.03m,

                        GrandTotal = 0,
                        GraphicsCost = 0,
                        LaborCost = totalLabourCost,
                        MaterialCost = totalMaterialCost,
                        ProfitMargin = 0,
                        TotalProductionCost = totalMaterialCost + totalLabourCost,
                    };
                    db.PrintingOverAllCosts.Add(entity);
                    db.SaveChanges();

                    return Json(msg.success, JsonRequestBehavior.AllowGet);

                }
            }
            return Json(msg.error, JsonRequestBehavior.AllowGet);
        }

        #region Approval
        public JsonResult CheckEstimation(int id)
        {
            var estimation = db.PrintingCostEstimations.Find(id);
            object response = null;
            var overAllCost = db.PrintingOverAllCosts.FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId);
            if (overAllCost == null)
            {
                response = new { Success = false, Message = "⚠️ You forgot to calculate overall cost" };
                return Json(response);
            }
            if (estimation != null)
            {
                estimation.Status = "Checked";
                estimation.IsOnlineTransferred = true;
                estimation.IsOnlineTransferredBy = User.Identity.Name;
                estimation.OnlineTransferredTime = DateTime.Today;

                db.Entry(estimation).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Estimation has been checked Successfully!" };

            }
            else
            {
                response = new { Success = false, Message = "Estimation not found!" };
            }

            return Json(response);

        }

        public JsonResult RejectEstimation(int id)
        {
            var estimation = db.PrintingCostEstimations.Find(id);
            object response = null;

            if (estimation != null)
            {
                estimation.Status = "Rejected";
                db.PrintingCostEstimations.Attach(estimation);
                db.Entry(estimation).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Estimation has been rejected Successfully!" };

            }
            else
            {
                response = new { Success = false, Message = "Estimation not found!" };
            }

            return Json(response);

        }
        public JsonResult ApproveEstimationForm(int id)
        {
            Object response = null;
            var estimationForm = db.PrintingCostEstimations.Find(id);
            var overAllCost = db.PrintingOverAllCosts.Where(x => x.PrintingCostEstimationId == estimationForm.PrintingCostEstimationId).ToList();
            if (overAllCost.Any())
            {
                estimationForm.IsOnlineApproved = true;
                estimationForm.OnlineApprovedBy = User.Identity.Name;
                estimationForm.OnlineApprovedTime = DateTime.Today;
                // Generates Estimation Summary
                CreateEstimationSummary(estimationForm);
                db.Entry(estimationForm).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Successfully Approved!" };
            }
            else
            {
                response = new { Success = false, Message = "" };

            }
            return Json(response);
        }
        private void CreateEstimationSummary(PrintingCostEstimation model)
        {
            var OverAllCost = db.PrintingOverAllCosts.FirstOrDefault(x => x.PrintingCostEstimationId == model.PrintingCostEstimationId);
            var estimationSummary = db.PrintingEstimationSummaries.FirstOrDefault(x => x.CustomerId == model.CustomerId && !x.IsOnlineApproved);
            if (estimationSummary == null)
            {
                PrintingEstimationSummary summary = new PrintingEstimationSummary
                {
                    CustomerId = model.CustomerId,
                    DateCreated = DateTime.Today,
                    CreatedBy = User.Identity.Name,
                    Status = "Pending"
                };

                db.PrintingEstimationSummaries.Add(summary);
                db.SaveChanges();

                PrintingEstimationDetail estimationDetailNew = new PrintingEstimationDetail
                {
                    PrintJobType = model.PrintJobType,
                    MaterialCost = OverAllCost.MaterialCost,
                    LabourCost = OverAllCost.LaborCost,
                    GraphicsCost = OverAllCost.GraphicsCost,
                    AdministrativeCost = (decimal)OverAllCost.AdminstrativeCost,
                    OverHeadCost = (decimal)OverAllCost.OverHeadCost,
                    ManufacturingCost = OverAllCost.MaterialCost,
                    TotalCost = OverAllCost.GrandTotal,
                    Quantity = model.ProductQuantity,
                    PrintingEstimationSummaryId = summary.PrintingEstimationSummaryId,
                    PrintingCostEstimationId = model.PrintingCostEstimationId,
                    UnitCost = OverAllCost.GrandTotal / model.ProductQuantity,
                    JobDescription = model.JobDescription,

                };
                db.PrintingEstimationDetails.Add(estimationDetailNew);
                db.SaveChanges();
            }
            else
            {
                PrintingEstimationDetail estimationDetail = new PrintingEstimationDetail
                {
                    PrintJobType = model.PrintJobType,
                    MaterialCost = OverAllCost.MaterialCost,
                    LabourCost = OverAllCost.LaborCost,
                    AdministrativeCost = (decimal)OverAllCost.AdminstrativeCost,
                    OverHeadCost = (decimal)OverAllCost.OverHeadCost,
                    ManufacturingCost = OverAllCost.MaterialCost,
                    TotalCost = OverAllCost.GrandTotal,
                    Quantity = model.ProductQuantity,
                    PrintingEstimationSummaryId = estimationSummary.PrintingEstimationSummaryId,
                    PrintingCostEstimationId = model.PrintingCostEstimationId,
                    JobDescription = model.JobDescription,
                    GraphicsCost = OverAllCost.GraphicsCost,
                    UnitCost = OverAllCost.GrandTotal / model.ProductQuantity,
                };

                db.PrintingEstimationDetails.Add(estimationDetail);
                db.SaveChanges();
            }
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
