using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.StandardCollection
{
    public class ArStandardCollectionValidationController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover,FinanceConstants.AccountReceivableCollection)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover,FinanceConstants.AccountReceivableCollection)]
        public ActionResult ARStandardCollectionValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArStandardCollectionValidation> arstandardcollectionvalidations = _db.ARStandardCollectionValidations.Where(x=>x.ArStandardCollectionId==id);
            DataSourceResult result = arstandardcollectionvalidations.ToDataSourceResult(request, aRStandardCollectionValidation => new
            {
                ArStandardCollectionValidationId = aRStandardCollectionValidation.ArStandardCollectionValidationId,
                ArStandardCollectionId=id,
                Status = aRStandardCollectionValidation.Status,
                Remark = aRStandardCollectionValidation.Remark,
                OrgBranchName = aRStandardCollectionValidation.OrgBranchName,
                DateCreated = aRStandardCollectionValidation.DateCreated,
                LastModified = aRStandardCollectionValidation.LastModified,
                CreatedBy = aRStandardCollectionValidation.CreatedBy,
                ModifiedBy = aRStandardCollectionValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARStandardCollectionValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ArStandardCollectionValidation aRStandardCollectionValidation)
        {
            var record = _db.ARStandardCollections.Find(id);
            var lines = _db.ARStandardCollectionLines.Where(x => x.ArStandardCollectionId == id).ToList();
            #region validated record

            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("error", "You cannot manipulate this record,it is already approved");

            if (aRStandardCollectionValidation.Status == ApprovalStatuses.Approve)
            {
                foreach (var line in lines)
                {
                    if (line.Source == InvoiceSource.Invoice)
                    {
                        #region validate collection and invoice due amount

                        string invoiceno = line.InvoiceNumber.Split('_')[1];
                        var invoice = _db.ARInvoices.FirstOrDefault(x => x.ArInvoiceId.ToString() == invoiceno);

                        decimal? amountdue = invoice?.AmountDue - line.ApplyAmount;
                        if (amountdue < 0)
                            ModelState.AddModelError("error", $"You are collecting more than invoice due amount by {Math.Abs((decimal)amountdue)} for {line.InvoiceNumber}");

                        #endregion
                    }


                }
            }

            #endregion
            if (ModelState.IsValid)
            {
                #region ger record record status
                if (record != null && !record.IsOnlineApproved)
                {
                    //validate 
                    if (aRStandardCollectionValidation.Status == ApprovalStatuses.Approve)
                    {
                        foreach (ArStandardCollectionLines line in lines)
                        {
                            //update status
                            if (line.Source == InvoiceSource.Invoice)
                            {
                                #region update-invoice status

                                string invoiceno = line.InvoiceNumber.Split('_')[1];
                                ArInvoice invoice = _db.ARInvoices.FirstOrDefault(x => x.ArInvoiceId.ToString() == invoiceno);

                                if (invoice != null)
                                {
                                    decimal amountdue = invoice.AmountDue - line.ApplyAmount;
                                    invoice.AmountDue = amountdue>=0?amountdue:0;
                                    if (invoice.AmountDue <= 0)
                                    {
                                        invoice.InvoiceStatus = InvoiceStatus.Close;
                                    }
                                   
                                    _db.Entry(invoice).State = EntityState.Modified;
                                }
                                _db.SaveChanges();
                                #endregion
                            }
                            else if (line.Source == InvoiceSource.SalesOrder)
                            {
                                #region update-sales order status

                                string invoiceno = line.InvoiceNumber.Split('_')[1];
                                ArSalesOrder salesOrder = _db.ArSalesOrders.FirstOrDefault(x => x.ArSalesOrderId.ToString() == invoiceno);

                                if (salesOrder != null)
                                {
                                    decimal amountdue = salesOrder.AmountDue - line.ApplyAmount;
                                    salesOrder.AmountDue = amountdue >= 0 ? amountdue : 0;
                                    if (salesOrder.AmountDue <= 0)
                                    {
                                        salesOrder.InvoiceStatus = InvoiceStatus.Close;
                                    }
                                   
                                    _db.Entry(salesOrder).State = EntityState.Modified;
                                }
                                _db.SaveChanges();
                                #endregion
                            }

                        }
                        #region pass data for reconciliation

                        foreach (ArStandardCollectionDistribution distribution in record.ARStandardCollectionDistribution)
                        {
                            string sourceid = record.ArStandardCollectionId + "-" + distribution.ArStandardCollectionDistributionId;
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
                                    Date = record.ReceiptDate,
                                    Type = "Collection",
                                    ReferenceDoc = !String.IsNullOrWhiteSpace(record.ReceiptNumber) ? record.ReceiptNumber : "NA",
                                    Reference = !String.IsNullOrWhiteSpace(record.Reference) ? record.Reference : "NA",
                                    Source = JournalSources.AR_Collection.ToString(),
                                    SourceID =sourceid,
                                    AccountSegment = _accountHelper.GetNaturalAccountSegment(distribution.AccountCode),
                                    SubAccountSegment = _accountHelper.GetSubAccountSegment(distribution.AccountCode)
                                };
                                _db.ReconcilationBookAccounts.Add(book);
                                _db.SaveChanges();
                            }
                        }
                        #endregion
                        //PostToSecondaryLedger(id);
                        record.IsPostedToSecondLedger = true;
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;

                    }
                    if (aRStandardCollectionValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }

                #endregion

               
                    var entity = new ArStandardCollectionValidation
                    {
                        ArStandardCollectionId = id,
                        Status = aRStandardCollectionValidation.Status,
                        Remark = aRStandardCollectionValidation.Remark,
                        OrgBranchName = aRStandardCollectionValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aRStandardCollectionValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aRStandardCollectionValidation.ModifiedBy
                    };

                    _db.ARStandardCollectionValidations.Add(entity);
                    _db.SaveChanges();
                    aRStandardCollectionValidation.ArStandardCollectionValidationId =
                        entity.ArStandardCollectionValidationId;
                
            }
            return Json(new[] { aRStandardCollectionValidation }.ToDataSourceResult(request, ModelState));
        }

        private void PostToSecondaryLedger(int id)
        {
            SecondLedgerHelper secondLedgerHelper = new SecondLedgerHelper();
            string user = User.Identity.Name;
            var record = _db.ARStandardCollections.Find(id);
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
                    SourcesName = "Standard Collection",
                    EffectiveDate = record.ReceiptDate,
                    GlPeriodId = _fiscalYearHelper.GetPeriodOfDate(record.ReceiptDate),
                    GlFiscalYearId = _fiscalYearHelper.GetFiscalYearOfDate(record.ReceiptDate),
                    PostingSource = JournalSources.AR_Collection,
                    PostJournalLineVms = new List<PostJournalVm.PostJournalLineVm>(),
                };

                #endregion
                #region distribution

                var distributions = _db.ARStandardCollectionDistributions.Where(x => x.ArStandardCollectionId == id).ToList();

                foreach (var distribution in distributions)
                {
                    var ledgerline = new PostJournalVm.PostJournalLineVm
                    {
                        PostJournalLineVmId = distribution.ArStandardCollectionDistributionId,
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
