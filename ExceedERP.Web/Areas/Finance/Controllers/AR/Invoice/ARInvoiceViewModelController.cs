using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Invoice
{
    public class ArInvoiceViewModelController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ARInvoiceViewModels_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArInvoiceViewModel> arinvoiceviewmodels = _db.ARInvoiceViewModels;
            DataSourceResult result = arinvoiceviewmodels.ToDataSourceResult(request, aRInvoiceViewModel => new {
                ARInvoiceViewModelID = aRInvoiceViewModel.ArInvoiceViewModelId,
                CustomerName = aRInvoiceViewModel.CustomerName,
                CustomerID = aRInvoiceViewModel.CustomerId,
                TransactionClass = aRInvoiceViewModel.TransactionClass,
                Date = aRInvoiceViewModel.Date,
                PaymentTerm = aRInvoiceViewModel.PaymentTerm,
                DueDate = aRInvoiceViewModel.DueDate,
                LineAmount = aRInvoiceViewModel.LineAmount,
                TaxAmount = aRInvoiceViewModel.TaxAmount,
                TotalAmount = aRInvoiceViewModel.TotalAmount,
                AmountDue = aRInvoiceViewModel.AmountDue,
                InvoiceStatus = aRInvoiceViewModel.InvoiceStatus
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoiceViewModels_Create([DataSourceRequest]DataSourceRequest request, ArInvoiceViewModel aRInvoiceViewModel)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArInvoiceViewModel
                {
                    CustomerName = aRInvoiceViewModel.CustomerName,
                    CustomerId = aRInvoiceViewModel.CustomerId,
                    TransactionClass = aRInvoiceViewModel.TransactionClass,
                    Date = aRInvoiceViewModel.Date,
                    PaymentTerm = aRInvoiceViewModel.PaymentTerm,
                    DueDate = aRInvoiceViewModel.DueDate,
                    LineAmount = aRInvoiceViewModel.LineAmount,
                    TaxAmount = aRInvoiceViewModel.TaxAmount,
                    TotalAmount = aRInvoiceViewModel.TotalAmount,
                    AmountDue = aRInvoiceViewModel.AmountDue,
                    InvoiceStatus = aRInvoiceViewModel.InvoiceStatus
                };

                _db.ARInvoiceViewModels.Add(entity);
                _db.SaveChanges();
                aRInvoiceViewModel.ArInvoiceViewModelId = entity.ArInvoiceViewModelId;
            }

            return Json(new[] { aRInvoiceViewModel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoiceViewModels_Update([DataSourceRequest]DataSourceRequest request, ArInvoiceViewModel aRInvoiceViewModel)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArInvoiceViewModel
                {
                    ArInvoiceViewModelId = aRInvoiceViewModel.ArInvoiceViewModelId,
                    CustomerName = aRInvoiceViewModel.CustomerName,
                    CustomerId = aRInvoiceViewModel.CustomerId,
                    TransactionClass = aRInvoiceViewModel.TransactionClass,
                    Date = aRInvoiceViewModel.Date,
                    PaymentTerm = aRInvoiceViewModel.PaymentTerm,
                    DueDate = aRInvoiceViewModel.DueDate,
                    LineAmount = aRInvoiceViewModel.LineAmount,
                    TaxAmount = aRInvoiceViewModel.TaxAmount,
                    TotalAmount = aRInvoiceViewModel.TotalAmount,
                    AmountDue = aRInvoiceViewModel.AmountDue,
                    InvoiceStatus = aRInvoiceViewModel.InvoiceStatus
                };

                _db.ARInvoiceViewModels.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { aRInvoiceViewModel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoiceViewModels_Destroy([DataSourceRequest]DataSourceRequest request, ArInvoiceViewModel aRInvoiceViewModel)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArInvoiceViewModel
                {
                    ArInvoiceViewModelId = aRInvoiceViewModel.ArInvoiceViewModelId,
                    CustomerName = aRInvoiceViewModel.CustomerName,
                    CustomerId = aRInvoiceViewModel.CustomerId,
                    TransactionClass = aRInvoiceViewModel.TransactionClass,
                    Date = aRInvoiceViewModel.Date,
                    PaymentTerm = aRInvoiceViewModel.PaymentTerm,
                    DueDate = aRInvoiceViewModel.DueDate,
                    LineAmount = aRInvoiceViewModel.LineAmount,
                    TaxAmount = aRInvoiceViewModel.TaxAmount,
                    TotalAmount = aRInvoiceViewModel.TotalAmount,
                    AmountDue = aRInvoiceViewModel.AmountDue,
                    InvoiceStatus = aRInvoiceViewModel.InvoiceStatus
                };

                _db.ARInvoiceViewModels.Attach(entity);
                _db.ARInvoiceViewModels.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { aRInvoiceViewModel }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
