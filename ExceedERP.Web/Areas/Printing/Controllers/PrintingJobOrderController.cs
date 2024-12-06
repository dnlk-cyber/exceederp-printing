using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
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
    }
}