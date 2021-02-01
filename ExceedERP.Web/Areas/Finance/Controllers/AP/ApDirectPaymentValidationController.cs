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
   
    public class ApDirectPaymentValidationController : Controller
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
        public ActionResult APDirectPaymentValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApDirectPaymentValidation> apDirectpaymentvalidations = _db.ApDirectPaymentValidations.Where(x=>x.ApDirectPaymentId==id);
            DataSourceResult result = apDirectpaymentvalidations.ToDataSourceResult(request, aPDirectPaymentValidation => new {
                ApDirectPaymentValidationId = aPDirectPaymentValidation.ApDirectPaymentValidationId,
                ApDirectPaymentId =id,
                Status = aPDirectPaymentValidation.Status,
                Remark = aPDirectPaymentValidation.Remark,
                OrgBranchName = aPDirectPaymentValidation.OrgBranchName,
                DateCreated = aPDirectPaymentValidation.DateCreated,
                LastModified = aPDirectPaymentValidation.LastModified,
                CreatedBy = aPDirectPaymentValidation.CreatedBy,
                ModifiedBy = aPDirectPaymentValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APDirectPaymentValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApDirectPaymentValidation aPDirectPaymentValidation)
        {
            #region validated record
            ApDirectPayment record = _db.ApDirectPayments.Find(id);

            if (record != null && record.IsOnlineApproved)
            {
               ModelState.AddModelError("approved","You cannot manipulate this record,it is already approved");
            }
            #endregion
            if (ModelState.IsValid)
            {
                if (record != null && !record.IsOnlineApproved)
                {
                    if (aPDirectPaymentValidation.Status == ApprovalStatuses.Approve)
                    {
                       
                        #region pass data for reconciliation

                        foreach (var distribution in record.ApDirectPaymentDistribution)
                        {
                            string sourceid = record.ApDirectPaymentId + "-" + distribution.ApDirectPaymentDistributionId;
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
                                    Date =record.PaymentDate,
                                    Type = "Direct Payment",
                                    ReferenceDoc = !String.IsNullOrWhiteSpace(record.Remark) ? record.Remark : "NA",
                                    Reference = !String.IsNullOrWhiteSpace(record.PaymentReference) ? record.PaymentReference : "NA",
                                    Source = JournalSources.AP_Direct_Payment.ToString(),
                                    SourceID = sourceid,
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
                        record.IsVoid = false;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                        
                    }

                    if (aPDirectPaymentValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }                  


                    var entity = new ApDirectPaymentValidation
                    {
                        ApDirectPaymentId = id,
                        Status = aPDirectPaymentValidation.Status,
                        Remark = aPDirectPaymentValidation.Remark,
                        OrgBranchName = aPDirectPaymentValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aPDirectPaymentValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aPDirectPaymentValidation.ModifiedBy
                    };

                    _db.ApDirectPaymentValidations.Add(entity);
                    _db.SaveChanges();
                    aPDirectPaymentValidation.ApDirectPaymentValidationId =
                        entity.ApDirectPaymentValidationId;

                    if (aPDirectPaymentValidation.Status == ApprovalStatuses.Approve)
                    {
                        #region update payment requests

                        int did = int.Parse(record.SourceId.Split('_')[1]);
                        string sourceType = record.SourceId.Split('_')[0];
                        switch (sourceType)
                        {
                            case "LC":
                                var lc = _db.ForeignPurchaseCosts.Find(did);
                                if (lc != null)
                                {
                                    lc.Paid = true;
                                    _db.Entry(lc).State = EntityState.Modified;
                                }
                                _db.SaveChanges();
                                break;
                            case "PAYMENT-REQUEST":
                                var paymentRequest = _db.PaymentRequests.Find(did);
                                if (paymentRequest != null)
                                {
                                    paymentRequest.Paid = true;
                                    _db.Entry(paymentRequest).State = EntityState.Modified;
                                }
                                _db.SaveChanges();
                                break;
                            case "TRAVEL-ORDER":

                                var travelOrder = _db.ApTravelOrders.Find(did);
                                if (travelOrder != null)
                                {
                                    travelOrder.Paid = true;
                                    _db.Entry(travelOrder).State = EntityState.Modified;
                                }
                                _db.SaveChanges();
                                break;
                            case "EXPENSE-CLAIM":

                                var expenseClaim = _db.ApTravelExpenseClaims.Find(did);
                                if (expenseClaim != null)
                                {
                                    expenseClaim.Paid = true;
                                    _db.Entry(expenseClaim).State = EntityState.Modified;
                                }
                                _db.SaveChanges();
                                break;
                        }
                        
                       
                        #endregion
                    }

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP direct payment approval", "Success", User.Identity.Name, $"approved direct payment {entity.ApDirectPaymentId} ", Modules.Finance);
                    #endregion
                }

            }

            return Json(new[] { aPDirectPaymentValidation }.ToDataSourceResult(request, ModelState));
        }



        private void PostToSecondaryLedger(int id)
        {
            SecondLedgerHelper secondLedgerHelper = new SecondLedgerHelper();
            string user = User.Identity.Name;
            var record = _db.ApDirectPayments.Find(id);
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
                    SourcesName = "Direct. Payment",
                    EffectiveDate = record.PaymentDate,
                    GlPeriodId = _fiscalYearHelper.GetPeriodOfDate(record.PaymentDate),
                    GlFiscalYearId = _fiscalYearHelper.GetFiscalYearOfDate(record.PaymentDate),
                    PostingSource = JournalSources.AP_Direct_Payment,
                    PostJournalLineVms = new List<PostJournalVm.PostJournalLineVm>(),
                };

                #endregion
                #region distribution

                var distributions = _db.ApDirectPaymentDistributions.Where(x => x.ApDirectPaymentId == id).ToList();

                foreach (var distribution in distributions)
                {
                    var ledgerline = new PostJournalVm.PostJournalLineVm
                    {
                        PostJournalLineVmId = distribution.ApDirectPaymentDistributionId,
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
