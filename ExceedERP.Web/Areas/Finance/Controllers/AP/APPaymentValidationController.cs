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


            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record");
            if (record != null && record.IsPrepayment)
            {
                if (record.PoTypeEnum == PurchaseType.PurchaseOrder)
                {
                    #region purchase order
                    var purchaseOrder = _db.PurchaseOrders.Find(int.Parse(record.PurchaseReference));
                    if (purchaseOrder != null && purchaseOrder.IsVoid)
                        ModelState.AddModelError("void", "Related purchase order "+purchaseOrder.PurchaseOrderID+" is void,hence can't approve.");
                    if (purchaseOrder != null && !purchaseOrder.IsOnlineApproved)
                        ModelState.AddModelError("notapproved", "Related purchase order " + purchaseOrder.PurchaseOrderID + " is not approved,hence can't approve.");
                    #endregion 
                }

                else if (record.PoTypeEnum == PurchaseType.TenderAward)
                {
                    #region award

                    var bidAwardedSupplier = _db.BidAwardedSuppliers.Find(int.Parse(record.PurchaseReference));

                    if (bidAwardedSupplier != null)
                    {
                        var lot = _db.Lots.Find(bidAwardedSupplier.LotID);
                        if (lot != null)
                        {
                            var tender = _db.Tenders.Find(lot.TenderID);

                            if (tender != null)
                            {
                                if (tender.IsVoid)
                                    ModelState.AddModelError("void", "Related tender " +tender.TenderReference+" is void,hence can't approve.");
                                if (!tender.IsOnlineApproved)
                                    ModelState.AddModelError("notapproved", "Related tender " + tender.TenderReference + "  is not approved,hence can't approve.");
                            }
                        }
                    }
                    #endregion 
                }
            }
            #region validated record

            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("error", "You cannot manipulate this record,it is already approved");
            var invoice = _db.APPaybleInvoices.FirstOrDefault(x => x.InvoiceNumber == record.InvoiceNumber);
            decimal? amountdue = invoice?.AmountDue - record.PaymentAmount;
            if (amountdue < 0)
                ModelState.AddModelError("error",
                    $"You are paying more than invoice due amount by {Math.Abs((decimal) amountdue)} for {record.InvoiceNumber}");

            #endregion

            #endregion

            if (ModelState.IsValid)
            {
               
                #region ger record record status

                if (record != null && !record.IsOnlineApproved)
                {
                    //validate 
                    if (aPPaymentValidation.Status == ApprovalStatuses.Approve)
                    {
                        
                        #region update po or award
                        if (!String.IsNullOrWhiteSpace(record.PurchaseReference))
                        {
                            if(record.PoType == "Purchase Order")
                            {
                             var totalTaken = _db.APPayments
                               .Where(x => x.PurchaseReference == record.PurchaseReference)
                               .Where(x => x.PoType == "Purchase Order")
                               .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                               .Select(x => new { x.PrepaymentAmount,x.PaymentAmount })
                               .AsQueryable();
                                if (totalTaken.Any())
                                {
                                    decimal totalAmount = _db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == int.Parse(record.PurchaseReference)).Sum(x => x.Total);                                 
                                    decimal totalTakenAmount = totalTaken.Sum(x => x.PrepaymentAmount) + totalTaken.Sum(x => x.PaymentAmount);
                                    if (totalTakenAmount >= totalAmount)
                                    {
                                        var po = _db.PurchaseOrders.Find(int.Parse(record.PurchaseReference));
                                        po.IsPaid = true;
                                        _db.Entry(po).State = EntityState.Modified;
                                    }
                                }
                            }
                            else if (record.PoType == "Tender Award")
                            {
                                var totalTaken = _db.APPayments
                                  .Where(x => x.PurchaseReference == record.PurchaseReference)
                                  .Where(x => x.PoType == "Tender Award")
                                  .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                  .Select(x => new { x.PrepaymentAmount, x.PaymentAmount })
                                  .AsQueryable();
                                if (totalTaken.Any())
                                {
                                    var contract = _db.ContractManagements
                                        .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                        .Where(x => x.ProcurementRefNo == int.Parse(record.PurchaseReference)).FirstOrDefault();
                                    decimal totalAmount = _db.ContractManagementItems.Where(x => x.ContractManagementID == contract.ContractManagementID).Sum(x => x.Total); 
                                    decimal totalTakenAmount = totalTaken.Sum(x => x.PrepaymentAmount) + totalTaken.Sum(x => x.PaymentAmount);
                                    if (totalTakenAmount >= totalAmount)
                                    {                                       
                                        contract.IsPaid = true;
                                        _db.Entry(contract).State = EntityState.Modified;
                                    }
                                }
                            }
                        }
                        #endregion
                        //get the invoice
                        var paybleInvoice = _db.APPaybleInvoices.FirstOrDefault(x => x.InvoiceNumber == record.InvoiceNumber);
                        if (paybleInvoice != null)
                        {
                            decimal invoiceamountdue = paybleInvoice.AmountDue - record.PaymentAmount;
                            paybleInvoice.AmountDue = invoiceamountdue >= 0 ? invoiceamountdue : 0;
                            if (paybleInvoice.AmountDue <= 0)
                            {
                                paybleInvoice.InvoiceStatus = InvoiceStatus.Close;
                            }
                            else
                            {
                                ;
                            }

                            _db.Entry(paybleInvoice).State = EntityState.Modified;
                            //_db.SaveChanges();
                        }

                        _db.SaveChanges();

                        #region pass data for reconciliation

                        foreach (ApPaymentDistribution distribution in record.ApPaymentDistribution)
                        {
                            string sourceid = record.ApPaymentId + "-" + distribution.ApPaymentDistributionId;
                            //check if this is bank account
                            if (_accountHelper.IsBankAccount(distribution.AccountCode))
                            {
                                var book = new ReconciliationBookAccount
                                {
                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Now,
                                    AccountCode = distribution.AccountCode,
                                    Debit = distribution.Debit,
                                    Credit = distribution.Credit,
                                    Date = record.PaymentDate,
                                    Type = "Payment",
                                    ReferenceDoc = !String.IsNullOrWhiteSpace(record.Remark) ? record.Remark : "NA",
                                    Reference = !String.IsNullOrWhiteSpace(record.PaymentReference)
                                        ? record.PaymentReference
                                        : "NA",
                                    Source = record.IsPrepayment
                                        ? JournalSources.AP_PrePayment.ToString()
                                        : JournalSources.AP_Payment.ToString(),
                                    SourceID = sourceid,
                                    AccountSegment = _accountHelper.GetNaturalAccountSegment(distribution.AccountCode),
                                    SubAccountSegment = _accountHelper.GetSubAccountSegment(distribution.AccountCode)
                                };
                                _db.ReconcilationBookAccounts.Add(book);
                                //_db.SaveChanges();
                            }
                        }

                        #endregion

                        //PostToSecondaryLedger(id);
                        record.IsPostedToSecondLedger = true;
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


        private void PostToSecondaryLedger(int id)
        {
            SecondLedgerHelper secondLedgerHelper = new SecondLedgerHelper();
            string user = User.Identity.Name;
            var record = _db.APPayments.Find(id);
            if (record != null)
            {
                #region header

                var ledger = new PostJournalVm
                {
                    PostJournalVmId = id,
                    JournalName = "",
                    JournalNumber = "",
                    Description = "",
                    CategoryName = "",
                    SourcesName = "Payment",
                    EffectiveDate = record.PaymentDate,
                    GlPeriodId = _fiscalYearHelper.GetPeriodOfDate(record.PaymentDate),
                    GlFiscalYearId = _fiscalYearHelper.GetFiscalYearOfDate(record.PaymentDate),
                    PostingSource =record.IsPrepayment? JournalSources.AP_PrePayment: JournalSources.AP_Payment,
                    PostJournalLineVms = new List<PostJournalVm.PostJournalLineVm>(),
                };

                #endregion
                #region distribution

                var distributions = _db.APPaymentDistributions.Where(x => x.ApPaymentId == id).ToList();

                foreach (var distribution in distributions)
                {
                    var ledgerline = new PostJournalVm.PostJournalLineVm
                    {
                        PostJournalLineVmId = distribution.ApPaymentDistributionId,
                        AccountCode = distribution.AccountCode,
                        Debit = distribution.Debit,
                        Credit = distribution.Credit
                    };
                    ledger.PostJournalLineVms.Add(ledgerline);
                }

                #endregion

                secondLedgerHelper.PostToSecondLedger(ledger, user);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
