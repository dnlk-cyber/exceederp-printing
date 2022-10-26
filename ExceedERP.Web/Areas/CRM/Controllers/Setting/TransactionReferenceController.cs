using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin)]
    public class TransactionReferenceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TransactionReferences_Read([DataSourceRequest]DataSourceRequest request, int voucherID)
        {
            DataSourceResult result = new DataSourceResult();
            List<SalesQuoteVoucher> quoteList = new List<SalesQuoteVoucher>();

            CashSalesInvoice cashsales = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID == voucherID).FirstOrDefault();

            decimal Grandtot = 0;
            if (cashsales != null)
            {
                //if (!cashsales.References.Contains("No"))
                //{
                //    int Id = 0;
                //    if (!cashsales.References.IsCaseInsensitiveEqual("Manual"))
                //    {
                //        Id = int.Parse(cashsales.References);
                //    }
                //    SalesOrderVoucher orders = db.SalesOrderVouchers.Where(x => x.SalesOrderVoucherID == Id).FirstOrDefault();
                //    if (orders != null)
                //    {
                //        if (!orders.References.Contains("No"))
                //        {
                //            int QId = int.Parse(orders.References);
                //            quoteList = db.SalesQuoteVouchers.Where(x => x.SalesQuoteVoucherID == QId).ToList();
                //            List<LineItem> items = db.LineItems.Where(l => l.Voucher == QId.ToString() && l.VoucherDefinitionID == "0").ToList();//.Sum(s => s.CalculatedCost);


                //            foreach (var item in items)
                //            {
                //                Grandtot = Grandtot + item.TotalAmount;
                //            }
                //        }
                //    }
                //}
            }
            return Json(quoteList.ToDataSourceResult(request, quote => new
            {
                TransactionReferenceID = quote.SalesQuoteVoucherID,
                ReferenceType = "Quote",
                Referring = quote.SalesQuoteVoucherID,
                Referenced = quote.SalesQuoteVoucherID,
                Value = Grandtot,
                CreatedBy = quote.CreatedBy,
                DateCreated = quote.DateCreated,
                LastModified = quote.LastModified,
                ModifiedBy = quote.ModifiedBy,
                OrgBranchName = quote.OrgBranchName,
                Remark = quote.Remark
            }));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TransactionReferences_Create([DataSourceRequest]DataSourceRequest request, TransactionReference transactionReference, int voucherID)
        {
            if (ModelState.IsValid)
            {
                var entity = new TransactionReference
                {
                    ReferenceType = transactionReference.ReferenceType,
                    Referring = voucherID.ToString(),
                    Referenced = transactionReference.Referenced,
                    Value = transactionReference.Value,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = transactionReference.Remark
                };

                db.TransactionReferences.Add(entity);
                db.SaveChanges();
                transactionReference.TransactionReferenceID = entity.TransactionReferenceID;
            }

            return Json(new[] { transactionReference }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TransactionReferences_Update([DataSourceRequest]DataSourceRequest request, TransactionReference transactionReference)
        {
            if (ModelState.IsValid)
            {
                var entity = new TransactionReference
                {
                    TransactionReferenceID = transactionReference.TransactionReferenceID,
                    ReferenceType = transactionReference.ReferenceType,
                    Referring = transactionReference.Referring,
                    Referenced = transactionReference.Referenced,
                    Value = transactionReference.Value,
                    Remark = transactionReference.Remark
                };

                db.TransactionReferences.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { transactionReference }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TransactionReferences_Destroy([DataSourceRequest]DataSourceRequest request, TransactionReference transactionReference)
        {
            if (ModelState.IsValid)
            {
                var entity = new TransactionReference
                {
                    TransactionReferenceID = transactionReference.TransactionReferenceID,
                    ReferenceType = transactionReference.ReferenceType,
                    Referring = transactionReference.Referring,
                    Referenced = transactionReference.Referenced,
                    Value = transactionReference.Value,
                    Remark = transactionReference.Remark
                };

                db.TransactionReferences.Attach(entity);
                db.TransactionReferences.Remove(entity);
                db.SaveChanges();

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Transaction Reference number " + transactionReference.TransactionReferenceID);

            }

            return Json(new[] { transactionReference }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
