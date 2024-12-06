using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingProformaInvoiceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Printing/PrintingProformaInvoice
        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();
            ViewData["Taxes"] = db.GLTaxRates.Select(s => new
            {
                Value = s.GLTaxRateID,
                Text = s.TaxName
            }).ToList();
            ViewData["JobType"] = db.PrintingJobTypes.Select(s => new
            {
                Value = s.PrintingJobTypeId,
                Text = s.JobTypeName
            }).ToList();
            return View();
        }

        public ActionResult PrintingProformaInvoice_Read([DataSourceRequest]DataSourceRequest request)
        {
            var printingProformaInvoices = db.PrintingProformaInvoices;
            return Json(printingProformaInvoices.ToDataSourceResult(request));
        }

        public ActionResult PrintingProformaInvoice_Create([DataSourceRequest]DataSourceRequest request, PrintingProformaInvoice model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProformaInvoice
                {
                    AdvancePaymentInPercent = model.AdvancePaymentInPercent,
                    CreatedBy = User.Identity.Name,
                    CustomerId = model.CustomerId,
                    DateCreated = DateTime.Today,
                    DeliveryDate = model.DeliveryDate,
                    DeliveryPeriod = model.DeliveryPeriod,
                    HaveEstimation = model.HaveEstimation,
                    IsExistingCustomer = model.IsExistingCustomer,
                    ModeOfPayment = model.ModeOfPayment,
                    PaymentType = model.PaymentType,
                    Remark = model.Remark,
                    PriceValidityPeriod = model.PriceValidityPeriod,
                    Status = model.Status,
                    
                };
                db.PrintingProformaInvoices.Add(entity);
                db.SaveChanges();
                model.PrintingProformaInvoiceId = entity.PrintingProformaInvoiceId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult PrintingProformaInvoice_Update([DataSourceRequest]DataSourceRequest request, PrintingProformaInvoice model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProformaInvoice
                {
                    AdvancePaymentInPercent = model.AdvancePaymentInPercent,
                    CreatedBy = User.Identity.Name,
                    CustomerId = model.CustomerId,
                    DateCreated = DateTime.Today,
                    DeliveryDate = model.DeliveryDate,
                    DeliveryPeriod = model.DeliveryPeriod,
                    HaveEstimation = model.HaveEstimation,
                    IsExistingCustomer = model.IsExistingCustomer,
                    ModeOfPayment = model.ModeOfPayment,
                    PaymentType = model.PaymentType,
                    Remark = model.Remark,
                    PriceValidityPeriod = model.PriceValidityPeriod,
                    Status = model.Status,
                    PrintingProformaInvoiceId = model.PrintingProformaInvoiceId

                };
                db.PrintingProformaInvoices.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult PrintingProformaInvoice_Delete([DataSourceRequest]DataSourceRequest request, PrintingProformaInvoice model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.PrintingProformaInvoices.Find(model.PrintingProformaInvoiceId);
                if(entity != null)
                {
                    var items = db.PrintingProformaInvoiceItems.Where(x => x.PrintingProformaInvoiceId == entity.PrintingProformaInvoiceId);
                    db.PrintingProformaInvoiceItems.RemoveRange(items);
                    db.PrintingProformaInvoices.Remove(entity);
                    db.SaveChanges();
                }
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));

        }

        #region Approval
        public JsonResult CheckProforma(int id)
        {
            Object response = null;
            var proforma = db.PrintingProformaInvoices.Find(id);
            if (proforma != null)
            {
                proforma.IsSendForApproval = true;
                proforma.SendForApprovalBy = User.Identity.Name;
                proforma.SendForApprovalTime = DateTime.Today;
                db.Entry(proforma).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Successfully Checked" };
            }
            else
            {
                response = new { Success = false, Message = "Something's Wrong!" };
            }
            return Json(response);
        }

        public JsonResult ApproveProforma(int id)
        {
            Object response = null;
            var proformaInvoice = db.PrintingProformaInvoices.Find(id);
            if (proformaInvoice != null)
            {
                var proformaItems = db.PrintingProformaInvoiceItems.Where(x => x.PrintingProformaInvoiceId == proformaInvoice.PrintingProformaInvoiceId).ToList();
                if (proformaItems.Any())
                {
                    proformaInvoice.IsOnlineApproved = true;
                    proformaInvoice.OnlineApprovedBy = User.Identity.Name;
                    proformaInvoice.OnlineApprovedTime = DateTime.Now;

                    db.Entry(proformaInvoice).State = EntityState.Modified;
                    db.SaveChanges();


                    response = new { Success = true, Message = "Proforma Successfully Approved" };

                }
                else
                {
                    response = new { Success = false, Message = "Something's Wrong!, Proforma Item Not Found!" };
                }
            }
            else
            {
                response = new { Success = false, Message = "Something's Wrong!, Proforma Not Found!" };
            }
            return Json(response);
        }
        #endregion
    }
}