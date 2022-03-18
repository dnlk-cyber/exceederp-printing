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
   
    public class ApMiscPaymentValidationController : Controller
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
        public ActionResult APMiscellaneousPaymentValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApMiscellaneousPaymentValidation> apmiscellaneouspaymentvalidations = _db.APMiscellaneousPaymentValidations.Where(x=>x.ApMiscellaneousPaymentId==id);
            DataSourceResult result = apmiscellaneouspaymentvalidations.ToDataSourceResult(request, aPMiscellaneousPaymentValidation => new {
                ApMiscellaneousPaymentValidationId = aPMiscellaneousPaymentValidation.ApMiscellaneousPaymentValidationId,
                ApMiscellaneousPaymentId =id,
                Status = aPMiscellaneousPaymentValidation.Status,
                Remark = aPMiscellaneousPaymentValidation.Remark,
                OrgBranchName = aPMiscellaneousPaymentValidation.OrgBranchName,
                DateCreated = aPMiscellaneousPaymentValidation.DateCreated,
                LastModified = aPMiscellaneousPaymentValidation.LastModified,
                CreatedBy = aPMiscellaneousPaymentValidation.CreatedBy,
                ModifiedBy = aPMiscellaneousPaymentValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APMiscellaneousPaymentValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApMiscellaneousPaymentValidation aPMiscellaneousPaymentValidation)
        {
            #region validated record
            ApMiscellaneousPayment record = _db.APMiscellaneousPayments.Find(id);

            if (record != null && record.IsOnlineApproved)
            {
               ModelState.AddModelError("approved","You cannot manipulate this record,it is already approved");
            }
            #endregion
            if (ModelState.IsValid)
            {
                if (record != null && !record.IsOnlineApproved)
                {
                    if (aPMiscellaneousPaymentValidation.Status == ApprovalStatuses.Approve)
                    {
                        
                        #region pass data for reconciliation

                        foreach (var distribution in record.ApMiscellaneousPaymentDistribution)
                        {
                            string sourceid = record.ApMiscellaneousPaymentId + "-" + distribution.ApMiscellaneousPaymentDistributionId;
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
                                    Type = "Misc Payment",
                                    ReferenceDoc = !String.IsNullOrWhiteSpace(record.Remark) ? record.Remark : "NA",
                                    Reference = !String.IsNullOrWhiteSpace(record.PaymentReference) ? record.PaymentReference : "NA",
                                    Source = JournalSources.AP_MiscPayment.ToString(),
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
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    if (aPMiscellaneousPaymentValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                   


                    var entity = new ApMiscellaneousPaymentValidation
                    {
                        ApMiscellaneousPaymentId = id,
                        Status = aPMiscellaneousPaymentValidation.Status,
                        Remark = aPMiscellaneousPaymentValidation.Remark,
                        OrgBranchName = aPMiscellaneousPaymentValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aPMiscellaneousPaymentValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aPMiscellaneousPaymentValidation.ModifiedBy
                    };

                    _db.APMiscellaneousPaymentValidations.Add(entity);
                    _db.SaveChanges();
                    aPMiscellaneousPaymentValidation.ApMiscellaneousPaymentValidationId =
                        entity.ApMiscellaneousPaymentValidationId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP misc payment approval", "Success", User.Identity.Name, $"approved misc payment {entity.ApMiscellaneousPaymentId} ", Modules.Finance);
                    #endregion
                }

            }

            return Json(new[] { aPMiscellaneousPaymentValidation }.ToDataSourceResult(request, ModelState));
        }



        private void PostToSecondaryLedger(int id)
        {
            SecondLedgerHelper secondLedgerHelper = new SecondLedgerHelper();
            string user = User.Identity.Name;
            var record = _db.APMiscellaneousPayments.Find(id);
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
                    SourcesName = "Misc. Payment",
                    EffectiveDate = record.PaymentDate,
                    GlPeriodId = _fiscalYearHelper.GetPeriodOfDate(record.PaymentDate),
                    GlFiscalYearId = _fiscalYearHelper.GetFiscalYearOfDate(record.PaymentDate),
                    PostingSource = JournalSources.AP_MiscPayment,
                    PostJournalLineVms = new List<PostJournalVm.PostJournalLineVm>(),
                };

                #endregion
                #region distribution

                var distributions = _db.APMiscellaneousPaymentDistributions.Where(x => x.ApMiscellaneousPaymentId == id).ToList();

                foreach (var distribution in distributions)
                {
                    var ledgerline = new PostJournalVm.PostJournalLineVm
                    {
                        PostJournalLineVmId = distribution.ApMiscellaneousPaymentDistributionId,
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
