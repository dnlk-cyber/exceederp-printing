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
            ViewData["PrintingProcess"] = db.PrintingProcesses.Select(s => new
            {
                Value = s.PrintingProcessId,
                Text = s.ProcessName
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
            var estimationSummary = db.PrintingEstimationSummaries.FirstOrDefault(x => x.CustomerId == model.CustomerId && !x.IsCalculated);
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
                    JobTypeId = model.JobTypeId,
                    MaterialCost = OverAllCost.MaterialCost,
                    LabourCost = OverAllCost.LaborCost,
                    AdministrativeCost = OverAllCost.AdminstrativeCost,
                    OverHeadCost = OverAllCost.OverHeadCost,
                    ManufacturingCost = OverAllCost.MaterialCost,
                    GrandTotalCost = OverAllCost.GrandTotal,
                    Quantity = model.ProductQuantity,
                    PrintingEstimationSummaryId = summary.PrintingEstimationSummaryId,
                };
                db.PrintingEstimationDetails.Add(estimationDetailNew);
                db.SaveChanges();
            }
            else
            {
                PrintingEstimationDetail estimationDetail = new PrintingEstimationDetail
                {
                    JobTypeId = model.JobTypeId,
                    MaterialCost = OverAllCost.MaterialCost,
                    LabourCost = OverAllCost.LaborCost,
                    AdministrativeCost = OverAllCost.AdminstrativeCost,
                    OverHeadCost = OverAllCost.OverHeadCost,
                    ManufacturingCost = OverAllCost.MaterialCost,
                    GrandTotalCost = OverAllCost.GrandTotal,
                    Quantity = model.ProductQuantity,
                    PrintingEstimationSummaryId = estimationSummary.PrintingEstimationSummaryId,
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
