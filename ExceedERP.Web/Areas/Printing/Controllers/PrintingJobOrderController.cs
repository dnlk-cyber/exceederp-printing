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
            ViewData["JobType"] = db.PrintingJobTypes.Select(s => new
            {
                Value = s.PrintingJobTypeId,
                Text = s.JobTypeName
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

        //public ActionResult PrintingJobOrder_Update([DataSourceRequest]DataSourceRequest request, PrintingJobOrder model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var jobOrder = db.PrintingJobOrders.Find(model.PrintingJobOrderId);
        //        if(jobOrder.CustomerId != model.CustomerId)
        //        {
        //            var items = db.PrintingJobOrderItems.Where(x => x.PrintingJobOrderId == jobOrder.PrintingJobOrderId).ToList();
        //            if (items.Any())
        //            {
        //                db.PrintingJobOrderItems.RemoveRange(items);
        //                db.SaveChanges();
        //            }
        //            var jobOrderN = new PrintingJobOrder
        //            {
        //                CustomerId = model.CustomerId,
        //                JobStartDate = model.JobStartDate,
        //                DeliveryDate = model.JobStartDate.AddDays(model.DeliveryPeriod),
        //                PrintingProformaInvoiceId = proformaInvoice.PrintingProformaInvoiceId,
        //                DateCreated = DateTime.Today,
        //                CreatedBy = User.Identity.Name,
        //                CRVNo = model.CRVNo,
        //                PaymentType = model.PaymentType,
        //                AdvancePaymentInPercent = model.AdvancePaymentInPercent,
        //                Remark = model.Remark
        //            };
        //            jobOrder.DeliveryDate = DateTime.Today.AddDays(proformaInvoice.DeliveryPeriod);
        //        }
        //    }
        //}
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
                        DeliveryDate = model.JobStartDate.AddDays(proformaInvoice.DeliveryPeriod),
                        PrintingProformaInvoiceId = proformaInvoice.PrintingProformaInvoiceId,
                        DateCreated = DateTime.Today,
                        CreatedBy = User.Identity.Name,
                        CRVNo = model.CRVNo,
                        PaymentType = model.PaymentType,
                        AdvancePaymentInPercent = model.AdvancePaymentInPercent,
                        Remark = model.Remark
                    };
                    jobOrder.DeliveryDate = DateTime.Today.AddDays(proformaInvoice.DeliveryPeriod);
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
                            Description = "",
                            NumberOfPages = proformaItem.NumberOfPages,
                            PrintingJobOrderId = jobOrder.PrintingJobOrderId,
                            Remark = "",
                            TotalPrice = proformaItem.TotalPrice,
                            UnitPrice = proformaItem.UnitPrice,                            
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
                    var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.JobTypeId == item.JobTypeId);
                    if (estimation != null)
                    {
                        var materialCosts = db.PrintingEstimationMaterialCosts.Where(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId);
                        var laborCosts = db.PrintingEstimationLaborCosts.Where(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId);

                        if (materialCosts != null && laborCosts != null)
                        {
                            var jobOrderProduction = new PrintingJobOrderProduction
                            {
                                CustomerId = jobOrder.CustomerId,
                                DeliveryDate = jobOrder.DeliveryDate,
                                DateOrdered = jobOrder.JobStartDate,
                                JobOrderNo = "",
                                JobTypeId = item.JobTypeId,
                                Quantity = item.Quantity,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Today,
                                Status = "Pending",
                                JobDescription = jobOrder.Remark,
                                BindingStyleId = estimation.BindingStyleId,
                                CoverNoOfColor = estimation.CoverNoOfColor,
                                PaperSizeId = estimation.PaperSizeId,
                                TextNoOfColors = estimation.TextNoOfColors,
                                TotalTextNoPages = estimation.TotalTextNoPages,
                                
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
                                    EstimatedHours = laborCost.EstimatedHours,
                                    PrintingMachineTypeId = laborCost.PrintingMachineTypeId,
                                    PrintingProcessId = laborCost.PrintingProcessId,
                                    PrintingJobOrderProductionId = jobOrderProduction.PrintingJobOrderProductionId,
                                    ProcessCategory = laborCost.ProcessCategory,
                                };
                                db.PrintingProductionLaborCosts.Add(productionLaborCost);
                            }
                            db.SaveChanges();
                        }
                    }
                }

            }
        }

    }
}