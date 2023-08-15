using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    public class ApPaymentValidationController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment)]
        public ActionResult APPaymentValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApPaymentValidation> appaymentvalidations = _db.APPaymentValidations.Where(x=>x.ApPaymentId==id);
            DataSourceResult result = appaymentvalidations.ToDataSourceResult(request, aPPaymentValidation => new
            {
                ApPaymentValidationId = aPPaymentValidation.ApPaymentValidationId,
                ApPaymentId=id,
                Status = aPPaymentValidation.Status,
                Remark = aPPaymentValidation.Remark,
                OrgBranchName = aPPaymentValidation.OrgBranchName,
                DateCreated = aPPaymentValidation.DateCreated,
                LastModified = aPPaymentValidation.LastModified,
                CreatedBy = aPPaymentValidation.CreatedBy,
                ModifiedBy = aPPaymentValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaymentValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApPaymentValidation aPPaymentValidation)
        {
            var record = _db.APPayments.Find(id);
            #region validated record

            if (record == null )
                ModelState.AddModelError("notfound", "Record is not found");
            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record");

            if (record != null && record.PoType== "Purchase Payment")
            {
                #region purchase payment request
                var purchasePayment = _db.PurchasePayments.Find(int.Parse(record.PurchaseReference));
                if (purchasePayment != null && purchasePayment.IsVoid)
                    ModelState.AddModelError("void", "The purchase payment request " + purchasePayment.PurchasePaymentId + " is void,hence can't approve payment.");
                if (purchasePayment != null && !purchasePayment.IsFinanceApproved)
                    ModelState.AddModelError("notapproved", "The purchase payment request " + purchasePayment.PurchasePaymentId + " is not approved,hence can't approve payment.");

                decimal paymentAmount = record.IsPrepayment ? record.PrepaymentAmount : record.PaymentAmount;
                decimal ? amountdue = purchasePayment?.AmountDue - paymentAmount;
                if (amountdue < 0)
                    ModelState.AddModelError("error",
                        $"You are paying more than purchase payment due amount by {Math.Abs((decimal)amountdue)} for {record.PurchaseReference}");
                if (amountdue > 0)
                    ModelState.AddModelError("error",
                        $"You are paying less than purchase payment due amount by {Math.Abs((decimal)amountdue)} for {record.PurchaseReference}");
                #endregion

            }
            if (record != null && record.PoType == "Invoice")
            {
                #region payable invoice
                var invoice = _db.APPaybleInvoices.Find(int.Parse(record.PurchaseReference));
                if (invoice != null && invoice.IsVoid)
                    ModelState.AddModelError("void", "The invoice number " + invoice.ApPaybleInvoicesId + " is void,hence can't approve payment.");
                if (invoice != null && !invoice.IsOnlineApproved)
                    ModelState.AddModelError("notapproved", "The invoice number " + invoice.ApPaybleInvoicesId + " is not approved,hence can't approve payment.");

                decimal paymentAmount = record.IsPrepayment ? record.PrepaymentAmount : record.PaymentAmount;
                decimal? amountdue = invoice?.AmountDue - paymentAmount;
                if (amountdue < 0)
                    ModelState.AddModelError("error",
                        $"You are paying more than invoice due amount by {Math.Abs((decimal)amountdue)} for {record.PurchaseReference}");
                if (amountdue > 0)
                    ModelState.AddModelError("error",
                        $"You are paying less than invoice due amount by {Math.Abs((decimal)amountdue)} for {record.PurchaseReference}");

                #endregion
            }          

            #endregion

            if (ModelState.IsValid)
            {
               
                #region ger record record status

                if (record != null && !record.IsOnlineApproved)
                {
                    //validate 
                    if (aPPaymentValidation.Status == ApprovalStatuses.Approve)
                    {
                        if (record.PoType == "Purchase Payment")
                        {
                            int did = int.Parse(record.PurchaseReference);
                            var purchasePament = _db.PurchasePayments.Find(did);
                            if (purchasePament != null)
                            {
                                //decimal invoiceamountdue = purchasePament.NetPayable - record.PrepaymentAmount;
                                
                                //if (invoiceamountdue <= 0)
                                //{
                                    purchasePament.Paid = true;
                                    purchasePament.Status = "Paid";
                                //}
                                //else
                                //{
                                //    ;
                                //}

                                _db.Entry(purchasePament).State = EntityState.Modified;
                                //_db.SaveChanges();
                            }
                        }
                        else if(record.PoType == "Invoice")
                        {
                            int did = int.Parse(record.PurchaseReference);
                            var paybleInvoice = _db.APPaybleInvoices.Find(did);
                            if (paybleInvoice != null)
                            {
                                //decimal invoiceamountdue = paybleInvoice.AmountDue - record.PaymentAmount;
                                //paybleInvoice.AmountDue = invoiceamountdue >= 0 ? invoiceamountdue : 0;
                                //if (paybleInvoice.AmountDue <= 0)
                                //{
                                    paybleInvoice.InvoiceStatus = InvoiceStatus.Close;
                                    paybleInvoice.Status = "Paid";
                                //}
                                //else
                                //{
                                //    ;
                                //}

                                _db.Entry(paybleInvoice).State = EntityState.Modified;
                                //_db.SaveChanges();
                            }
                        }
                        //_db.SaveChanges();

                        #region pass data for reconciliation

                        //foreach (ApPaymentDistribution distribution in record.ApPaymentDistribution)
                        //{
                        //    string sourceid = record.ApPaymentId + "-" + distribution.ApPaymentDistributionId;
                        //    //check if this is bank account
                        //    if (_accountHelper.IsBankAccount(distribution.AccountCode))
                        //    {
                        //        var book = new ReconciliationBookAccount
                        //        {
                        //            CreatedBy = User.Identity.Name,
                        //            DateCreated = DateTime.Now,
                        //            AccountCode = distribution.AccountCode,
                        //            Debit = distribution.Debit,
                        //            Credit = distribution.Credit,
                        //            Date = record.PaymentDate,
                        //            Type = "Payment",
                        //            ReferenceDoc = !String.IsNullOrWhiteSpace(record.Remark) ? record.Remark : "NA",
                        //            Reference = !String.IsNullOrWhiteSpace(record.PaymentReference)
                        //                ? record.PaymentReference
                        //                : "NA",
                        //            Source = record.IsPrepayment
                        //                ? JournalSources.AP_PrePayment.ToString()
                        //                : JournalSources.AP_Payment.ToString(),
                        //            SourceID = sourceid,
                        //            AccountSegment = _accountHelper.GetNaturalAccountSegment(distribution.AccountCode),
                        //            SubAccountSegment = _accountHelper.GetSubAccountSegment(distribution.AccountCode)
                        //        };
                        //        _db.ReconcilationBookAccounts.Add(book);
                        //        //_db.SaveChanges();
                        //    }
                        //}

                        #endregion

          
                        record.Status = "Approved";
                        record.IsVoid = false;
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        //_db.SaveChanges();

                    }

                    if (aPPaymentValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        //_db.SaveChanges();
                    }

                    #endregion

                    var entity = new ApPaymentValidation
                    {
                        ApPaymentId = id,
                        Status = aPPaymentValidation.Status,
                        Remark = aPPaymentValidation.Remark,
                        OrgBranchName = aPPaymentValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aPPaymentValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aPPaymentValidation.ModifiedBy
                    };

                    _db.APPaymentValidations.Add(entity);
                    _db.SaveChanges();
                    aPPaymentValidation.ApPaymentValidationId = entity.ApPaymentValidationId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP payment approval", "Success", User.Identity.Name, $"approved payment {entity.ApPaymentId} ", Modules.Finance);
                    #endregion
                }

            }

            return Json(new[] { aPPaymentValidation }.ToDataSourceResult(request, ModelState));
        }


       
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
