﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core;
﻿using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{
       [AuthorizeRoles(CRMRoles.CashSalesUser)] 
    public class NonCashTransactionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NonCashTransactions_Read([DataSourceRequest]DataSourceRequest request, int voucherID)
        {
            IQueryable<NonCashTransaction> noncashtransactions = db.NonCashTransactions.Where(nct => nct.Voucher == voucherID.ToString());
            DataSourceResult result = noncashtransactions.ToDataSourceResult(request, nonCashTransaction => new {
                NonCashTransactionID = nonCashTransaction.NonCashTransactionID,
                Voucher = nonCashTransaction.Voucher,
                Consignee = nonCashTransaction.Consignee,
                IsIncoming = nonCashTransaction.IsIncoming,
                ReceiveMethod = nonCashTransaction.ReceiveMethod,
                ReceiveProcessor = nonCashTransaction.ReceiveProcessor,
                Index = nonCashTransaction.Index,
                IssueDate = nonCashTransaction.IssueDate,
                MaturityDate = nonCashTransaction.MaturityDate,
                Number = nonCashTransaction.Number,
                Amount = nonCashTransaction.Amount,
                Executed = nonCashTransaction.Executed,
                CurrencyCode = nonCashTransaction.CurrencyCode,
                    CreatedBy =  nonCashTransaction.CreatedBy,
                DateCreated = nonCashTransaction.DateCreated,
                LastModified = nonCashTransaction.LastModified,
                ModifiedBy = nonCashTransaction.ModifiedBy,
                OrgBranchName = nonCashTransaction.OrgBranchName,
                Remark = nonCashTransaction.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NonCashTransactions_Create([DataSourceRequest]DataSourceRequest request, NonCashTransaction nonCashTransaction, int voucherID)
            {

            CashSalesInvoice cashSalesInvoice =
                db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID == voucherID).FirstOrDefault();
            if (ModelState.IsValid)
            {
                var entity = new NonCashTransaction
                {
                    Voucher = voucherID.ToString(),
                    Consignee = cashSalesInvoice.OrganizationCustomer.TradeName,
                    IsIncoming = nonCashTransaction.IsIncoming,
                    ReceiveMethod = cashSalesInvoice.RecieveMethod,
                    ReceiveProcessor = nonCashTransaction.ReceiveProcessor,
                    Index = nonCashTransaction.Index,
                    IssueDate = nonCashTransaction.IssueDate,
                    MaturityDate = nonCashTransaction.MaturityDate,
                    Number = nonCashTransaction.Number,
                    Amount = nonCashTransaction.Amount,
                    Executed = nonCashTransaction.Executed,
                    CurrencyCode = nonCashTransaction.CurrencyCode,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = nonCashTransaction.Remark
                };

                db.NonCashTransactions.Add(entity);
                db.SaveChanges();
                nonCashTransaction.NonCashTransactionID = entity.NonCashTransactionID;
            }

            return Json(new[] { nonCashTransaction }.ToDataSourceResult(request, ModelState));
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NonCashTransactionSettelments_Create([DataSourceRequest]DataSourceRequest request, NonCashTransaction nonCashTransaction, int voucherID)
        {

            CreditSettlement settelment =
                db.CreditSettlement.Where(x => x.CreditSettlementId == voucherID).FirstOrDefault();
            if (ModelState.IsValid)
            {
                var entity = new NonCashTransaction
                {
                    Voucher = voucherID.ToString(),
                    Consignee = settelment.OrganizationCustomer.TradeName,
                    IsIncoming = nonCashTransaction.IsIncoming,
                    ReceiveMethod = settelment.PaymentMethod.Description,
                    ReceiveProcessor = nonCashTransaction.ReceiveProcessor,
                    Index = nonCashTransaction.Index,
                    IssueDate = nonCashTransaction.IssueDate,
                    MaturityDate = nonCashTransaction.MaturityDate,
                    Number = nonCashTransaction.Number,
                    Amount = nonCashTransaction.Amount,
                    Executed = nonCashTransaction.Executed,
                    CurrencyCode = nonCashTransaction.CurrencyCode,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = nonCashTransaction.Remark
                };

                db.NonCashTransactions.Add(entity);
                db.SaveChanges();
                nonCashTransaction.NonCashTransactionID = entity.NonCashTransactionID;
            }

            return Json(new[] { nonCashTransaction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NonCashTransactions_Update([DataSourceRequest]DataSourceRequest request, NonCashTransaction nonCashTransaction)
        {
            if (ModelState.IsValid)
            {
                var entity = new NonCashTransaction
                {
                    NonCashTransactionID = nonCashTransaction.NonCashTransactionID,
                    Voucher = nonCashTransaction.Voucher,
                    Consignee = nonCashTransaction.Consignee,
                    IsIncoming = nonCashTransaction.IsIncoming,
                    ReceiveMethod = nonCashTransaction.ReceiveMethod,
                    ReceiveProcessor = nonCashTransaction.ReceiveProcessor,
                    Index = nonCashTransaction.Index,
                    IssueDate = nonCashTransaction.IssueDate,
                    MaturityDate = nonCashTransaction.MaturityDate,
                    Number = nonCashTransaction.Number,
                    Amount = nonCashTransaction.Amount,
                    Executed = nonCashTransaction.Executed,
                    CurrencyCode = nonCashTransaction.CurrencyCode,
                    Remark = nonCashTransaction.Remark
                };

                db.NonCashTransactions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { nonCashTransaction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NonCashTransactions_Destroy([DataSourceRequest]DataSourceRequest request, NonCashTransaction nonCashTransaction)
        {
            if (ModelState.IsValid)
            {
                var entity = new NonCashTransaction
                {
                    NonCashTransactionID = nonCashTransaction.NonCashTransactionID,
                    Voucher = nonCashTransaction.Voucher,
                    Consignee = nonCashTransaction.Consignee,
                    IsIncoming = nonCashTransaction.IsIncoming,
                    ReceiveMethod = nonCashTransaction.ReceiveMethod,
                    ReceiveProcessor = nonCashTransaction.ReceiveProcessor,
                    Index = nonCashTransaction.Index,
                    IssueDate = nonCashTransaction.IssueDate,
                    MaturityDate = nonCashTransaction.MaturityDate,
                    Number = nonCashTransaction.Number,
                    Amount = nonCashTransaction.Amount,
                    Executed = nonCashTransaction.Executed,
                    CurrencyCode = nonCashTransaction.CurrencyCode,
                    Remark = nonCashTransaction.Remark
                };

                db.NonCashTransactions.Attach(entity);
                db.NonCashTransactions.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted NonCash Transaction  number " + nonCashTransaction.NonCashTransactionID);
      
            }

            return Json(new[] { nonCashTransaction }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
