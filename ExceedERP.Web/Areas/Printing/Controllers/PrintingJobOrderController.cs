using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingJobOrderController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Printing/PrintingJobOrder
        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();
            ViewData["TextPaper"] = db.PrintingPaperSizes.Select(s => new
            {
                Value = s.PrintingPaperSizeId,
                Text = s.PrintPaperSize
            }).ToList();
            return View();
        }

        public ActionResult PrintingJobOrder_Read([DataSourceRequest]DataSourceRequest request)
        {
            var jobOrders = db.PrintingJobOrders;
            return Json(jobOrders.ToDataSourceResult(request));
        }

        public ActionResult PrintingJobOrder_Create([DataSourceRequest]DataSourceRequest request, PrintingJobOrder model)
        {

            if (ModelState.IsValid)
            {
                if (!GenerateJobOrder(model))
                {
                    ModelState.AddModelError("", "Something's wrong!");
                }
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult PrintingJobOrder_Update([DataSourceRequest]DataSourceRequest request, PrintingJobOrder model)
        {
            if (ModelState.IsValid)
            {
                var jobOrder = db.PrintingJobOrders.Find(model.PrintingJobOrderId);
                if (jobOrder.PrintingProformaInvoiceId != model.PrintingProformaInvoiceId)
                {
                    var items = db.PrintingJobOrderItems.Where(x => x.PrintingJobOrderId == jobOrder.PrintingJobOrderId).ToList();
                    if (items.Any())
                    {
                        db.PrintingJobOrderItems.RemoveRange(items);
                        db.SaveChanges();
                    }
                    db.Entry(model).State = EntityState.Modified;
                    GenerateDetail(model.PrintingJobOrderId);
                    db.SaveChanges();
                }
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult PrintingJobOrder_Delete([DataSourceRequest]DataSourceRequest request, PrintingJobOrder model)
        {
            if (ModelState.IsValid)
            {
                var printingJobOrder = db.PrintingJobOrders.Find(model.PrintingJobOrderId);
                if(printingJobOrder != null)
                {
                    var items = db.PrintingJobOrderItems.Where(x => x.PrintingJobOrderId == printingJobOrder.PrintingJobOrderId).ToList();
                    db.PrintingJobOrderItems.RemoveRange(items);
                    db.PrintingJobOrders.Remove(printingJobOrder);
                    db.SaveChanges();
                }
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private bool GenerateJobOrder(PrintingJobOrder model)
        {
            var proformaInvoice = db.PrintingProformaInvoices.Find(model.PrintingProformaInvoiceId);
            if(proformaInvoice != null)
            {
                var proformaItems = db.PrintingProformaInvoiceItems
                    .Where(x => x.PrintingProformaInvoiceId == proformaInvoice.PrintingProformaInvoiceId)
                    .ToList();
                if (proformaItems.Any())
                {
                    var jobOrder = new PrintingJobOrder
                    {

                        CustomerId = proformaInvoice.CustomerId,
                        JobStartDate = model.JobStartDate,
                        DeliveryDate = model.DeliveryDate,
                        PrintingProformaInvoiceId = proformaInvoice.PrintingProformaInvoiceId,
                        DateCreated = DateTime.Today,
                        CreatedBy = User.Identity.Name,
                        CRVNo = model.CRVNo,
                        PaymentType = model.PaymentType,
                        AdvancePaymentInPercent = model.AdvancePaymentInPercent,
                        Remark = model.Remark
                    };
                    db.PrintingJobOrders.Add(jobOrder);
                    db.SaveChanges();
                    foreach (var proformaItem in proformaItems)
                    {
                        var jobOrderItem = new PrintingJobOrderItem
                        {
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Today,
                            Quantity = proformaItem.Quantity,
                            JobTypeId = proformaItem.JobTypeId,
                            PrintJobType = proformaItem.PrintJobType,
                            JobDescription = proformaItem.JobDescription,
                            BindingStyle = proformaItem.BindingStyle,
                            CoverPaperSizeId = proformaItem.CoverPaperSizeId,
                            TextPaperSizeId = proformaItem.TextPaperSizeId,
                            Description = "",
                            NumberOfPages = proformaItem.NumberOfPages,
                            PrintingJobOrderId = jobOrder.PrintingJobOrderId,
                            Remark = "",
                            TotalPrice = proformaItem.TotalPrice,
                            UnitPrice = proformaItem.UnitPrice,
                            PrintingCostEstimationId = proformaItem.PrintingCostEstimationId
                        };
                        db.PrintingJobOrderItems.Add(jobOrderItem);
                        db.SaveChanges();
                        model.PrintingJobOrderId = jobOrder.PrintingJobOrderId;
                    }
                    return true;
                }
            }
            return false;
        }
        private void GenerateDetail(int id)
        {
            var proformaItems = db.PrintingProformaInvoiceItems
                   .Where(x => x.PrintingProformaInvoiceId == id)
                   .ToList();
            foreach (var proformaItem in proformaItems)
            {
                var jobOrderItem = new PrintingJobOrderItem
                {
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today,
                    Quantity = proformaItem.Quantity,
                    JobTypeId = proformaItem.JobTypeId,
                    PrintJobType = proformaItem.PrintJobType,
                    JobDescription = proformaItem.JobDescription,
                    TextPaperSizeId = proformaItem.TextPaperSizeId,
                    CoverPaperSizeId = proformaItem.CoverPaperSizeId,
                    BindingStyle = proformaItem.BindingStyle,
                    Description = "",
                    NumberOfPages = proformaItem.NumberOfPages,
                    PrintingJobOrderId = id,
                    Remark = "",
                    TotalPrice = proformaItem.TotalPrice,
                    UnitPrice = proformaItem.UnitPrice,
                    PrintingCostEstimationId = proformaItem.PrintingCostEstimationId
                };
                db.PrintingJobOrderItems.Add(jobOrderItem);
                db.SaveChanges();
            }
        }
        public JsonResult CheckJob(int id)
        {
            object response = null;
            var jobOrder = db.PrintingJobOrders.Find(id);
            if (jobOrder != null)
            {
                var validity = CheckJobItemValidity(jobOrder.PrintingJobOrderId);
                if (validity)
                {
                    jobOrder.IsSendForApproval = true;
                    jobOrder.SendForApprovalBy = User.Identity.Name;
                    jobOrder.SendForApprovalTime = DateTime.Today;

                    db.PrintingJobOrders.Attach(jobOrder);
                    db.Entry(jobOrder).State = EntityState.Modified;
                    db.SaveChanges();
                    response = new { Success = true, Message = "Job Order Checked Successfully!" };
                }
                else
                {
                    response = new { Success = false, Message = "Something is wrong! Please assign Job.No under job items!" };
                }
            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }

        private bool CheckJobItemValidity(int id)
        {
            var jobItems = db.PrintingJobOrderItems.Where(x => x.PrintingJobOrderId == id).ToList();
            if (!jobItems.Any())
            {
                return false;
            }
           
            return true;
        }

        public JsonResult ApproveJobOrder(int id)
        {
            object response = null;
            var jobOrder = db.PrintingJobOrders.Find(id);
            if (jobOrder != null)
            {
                
                jobOrder.IsOnlineApproved = true;
                jobOrder.OnlineApprovedBy = User.Identity.Name;
                jobOrder.OnlineApprovedTime = DateTime.Today;


                db.PrintingJobOrders.Attach(jobOrder);
                db.Entry(jobOrder).State = EntityState.Modified;
                db.SaveChanges();
                //--Generate Productions--//
                GenerateProduction(jobOrder);

                response = new { Success = true, Message = "Successfully Approved and Created Production!" };
            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }
        private void GenerateProduction(PrintingJobOrder jobOrder)
        {
            var jobItems = db.PrintingJobOrderItems.Where(x => x.PrintingJobOrderId == jobOrder.PrintingJobOrderId).ToList();
            if (jobItems.Any())
            {
                foreach (var item in jobItems)
                {
                    var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == item.PrintingCostEstimationId);
                    if (estimation != null)
                    {
                        var materialCosts = db.PrintingEstimationMaterialCosts
                            .Where(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId)
                            .ToList();
                        var laborCosts = db.PrintingEstimationLaborCosts
                            .Where(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId)
                            .ToList();
                        var designs = db.PrintingJobOrderJobDesigns
                            .Where(x => x.ReferenceId == jobOrder.PrintingJobOrderId)
                            .ToList();
                        if (materialCosts != null && laborCosts != null)
                        {
                            var jobOrderProduction = new PrintingJobOrderProduction
                            {
                                CustomerId = jobOrder.CustomerId,
                                DeliveryDate = jobOrder.DeliveryDate,
                                DateOrdered = jobOrder.JobStartDate,
                                JobOrderNo = "",
                                PrintJobType = item.PrintJobType,
                                Quantity = item.Quantity,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Today,
                                Status = "Pending",
                                JobDescription = item.JobDescription,
                                CoverNoOfColor = estimation.CoverNoOfColor,
                                TextPaperSizeId = estimation.TextPaperSizeId,
                                BindingStyle = estimation.BindingStyle,
                                CoverPaperSizeId = estimation.CoverPaperSizeId,
                                TextNoOfColors = estimation.TextNoOfColors,
                                TotalTextNoPages = estimation.Pages,                                
                                PrintingJobOrderId = jobOrder.PrintingJobOrderId
                            };
                            db.PrintingJobOrderProductions.Add(jobOrderProduction);
                            db.SaveChanges();


                            foreach (var materialCost in materialCosts)
                            {
                                var productionMaterialCost = new PrintingProductionMaterialCost
                                {
                                    Quantity = materialCost.Quantity,
                                    DateCreated = DateTime.Today,
                                    CreatedBy = User.Identity.Name,
                                    PrintingJobOrderProductionId = jobOrderProduction.PrintingJobOrderProductionId,
                                    PrintingMaterialCategoryId = materialCost.PrintingMaterialCategoryId,
                                    PrintingMaterialCategoryItemId = materialCost.PrintingMaterialCategoryItemId,
                                    TotalCost = materialCost.TotalCost,
                                    UnitCost = materialCost.UnitCost,
                                    UnitOfMeasurment = materialCost.UnitOfMeasurment,
                                };
                                db.PrintingProductionMaterialCosts.Add(productionMaterialCost);
                            }
                            foreach (var laborCost in laborCosts)
                            {
                                var productionLaborCost = new PrintingProductionLaborCost
                                {
                                    TotalCost = laborCost.TotalCost,
                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Today,
                                    
                                    PrintingProcessId = laborCost.PrintingProcessId,
                                    PrintingJobOrderProductionId = jobOrderProduction.PrintingJobOrderProductionId,
                                    PressCategory = laborCost.PressCategory,
                                };
                                db.PrintingProductionLaborCosts.Add(productionLaborCost);
                            }
                            foreach (var design in designs)
                            {
                                var entity = new PrintingJobOrderProductionJobDesign
                                {
                                    ReferenceId = jobOrderProduction.PrintingJobOrderProductionId,
                                    Description = design.Description,
                                    FileExtension = design.FileExtension,
                                    FileName = design.FileName,
                                    FilePath = design.FilePath,
                                    ShortPath = design.ShortPath,                                    
                                };
                                db.PrintingJobOrderProductionJobDesigns.Add(entity);
                            }
                            db.SaveChanges();
                        }
                    }
                }

            }
        }

    }
}