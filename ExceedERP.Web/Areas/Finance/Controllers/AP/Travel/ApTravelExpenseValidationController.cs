using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.Travel
{
    public class ApTravelExpenseValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableTravelExpenseClaimApprover, FinanceConstants.AccountPayableTravelExpenseClaim)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableTravelExpenseClaimApprover, FinanceConstants.AccountPayableTravelExpenseClaim)]
        public ActionResult ApTravelExpenseValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelExpenseValidation> aptravelexpensevalidations = _db.ApTravelExpenseValidations.Where(x => x.ApTravelExpenseClaimId == id); ;
            DataSourceResult result = aptravelexpensevalidations.ToDataSourceResult(request, apTravelExpenseValidation => new {
                ApTravelExpenseValidationId = apTravelExpenseValidation.ApTravelExpenseValidationId,
                Status = apTravelExpenseValidation.Status,
                Remark = apTravelExpenseValidation.Remark,
                OrgBranchName = apTravelExpenseValidation.OrgBranchName,
                DateCreated = apTravelExpenseValidation.DateCreated,
                LastModified = apTravelExpenseValidation.LastModified,
                CreatedBy = apTravelExpenseValidation.CreatedBy,
                ModifiedBy = apTravelExpenseValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelExpenseClaimApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApTravelExpenseValidation apTravelExpenseValidation)
        {
            var record = _db.ApTravelExpenseClaims.Find(id);
            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            if (record != null && record.ForwardedTo != User.Identity.Name)
                ModelState.AddModelError("notcorrectuser", "You can't approve this record,it is forwarded to another user.");

            var hasApproved = _db.PurchaseOrderValidations
                .Where(x => x.PurchaseOrderID == id)
                .Where(x => x.CreatedBy == User.Identity.Name && x.Status == ApprovalStatuses.Approve).AsQueryable();
            if (hasApproved.Any())
                ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (apTravelExpenseValidation.Status == ApprovalStatuses.Approve)
                    {
                        if (record.IsFinalApproval)
                        {
                            //PostToSecondaryLedger(id);
                            record.IsPostedToSecondLedger = true;
                            record.Status = "Approved";
                            record.IsOnlineApproved = true;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            _db.Entry(record).State = EntityState.Modified;
                            _db.SaveChanges();
                            #region pass data for reconciliation

                            foreach (ApTravelExpenseDistribution distribution in record.Distributions)
                            {
                                string sourceid = record.ApTravelExpenseClaimId + "-" + distribution.ApTravelExpenseDistributionId;
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
                                        Date = record.ClaimDate,
                                        Type = "Travel Expense Claim",
                                        ReferenceDoc = !String.IsNullOrWhiteSpace(record.Reference) ? record.Reference : "NA",
                                        Reference = !String.IsNullOrWhiteSpace(distribution.AdditionalDescription) ? distribution.AdditionalDescription : "NA",
                                        Source = JournalSources.AP_TravelExpenseClaim.ToString(),
                                        SourceID = sourceid,
                                        AccountSegment = _accountHelper.GetNaturalAccountSegment(distribution.AccountCode),
                                        SubAccountSegment = _accountHelper.GetSubAccountSegment(distribution.AccountCode)
                                    };
                                    _db.ReconcilationBookAccounts.Add(book);
                                    _db.SaveChanges();
                                }
                            }
                            #endregion
                            #region approval notification

                            NotifModels notif = new NotifModels
                            {
                                UserId = record.ForwardedFrom,
                                Message =
                                    $"Travel expense claim document {record.ApTravelOrderId} is approved by User: {record.ForwardedTo} at {DateTime.Now}"
                            };

                            Notify.NotifyUser(notif);

                            #endregion
                        }
                        else
                        {
                            record.Status = "Partialy Approved";
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            _db.Entry(record).State = EntityState.Modified;
                            _db.SaveChanges();
                            #region approval notification

                            NotifModels notif = new NotifModels
                            {
                                UserId = record.ForwardedFrom,
                                Message =
                                    $"Travel expense claim document {record.ApTravelOrderId} is approved by User: {record.ForwardedTo} at {DateTime.Now} and forwarded to next user."
                            };

                            Notify.NotifyUser(notif);

                            #endregion
                            #region forward to next approver

                            decimal docamount = record.Payments.Sum(x=>x.Amount);
                            DocApprovalWorkflowHelper.ForwardToApprover(record.ApTravelExpenseClaimId, docamount, "",
                                record.ForwardedTo, ApprovalDocumentType.ApTravelExpenseClaim, false);

                            #endregion
                        }

                    }



                    if (apTravelExpenseValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new ApTravelExpenseValidation
                    {
                        ApTravelExpenseClaimId = id,
                        Status = apTravelExpenseValidation.Status,
                        Remark = apTravelExpenseValidation.Remark,
                        OrgBranchName = apTravelExpenseValidation.OrgBranchName,
                        DateCreated = apTravelExpenseValidation.DateCreated,
                        LastModified = apTravelExpenseValidation.LastModified,
                        CreatedBy = apTravelExpenseValidation.CreatedBy,
                        ModifiedBy = apTravelExpenseValidation.ModifiedBy
                    };

                    _db.ApTravelExpenseValidations.Add(entity);
                    _db.SaveChanges();
                    apTravelExpenseValidation.ApTravelExpenseValidationId = entity.ApTravelExpenseValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP travel expense claim approval", "Success", User.Identity.Name, $"approved travel expense claim {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { apTravelExpenseValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelExpenseClaimApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseValidations_Update([DataSourceRequest]DataSourceRequest request, ApTravelExpenseValidation apTravelExpenseValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseValidation
                {
                    ApTravelExpenseValidationId = apTravelExpenseValidation.ApTravelExpenseValidationId,
                    Status = apTravelExpenseValidation.Status,
                    Remark = apTravelExpenseValidation.Remark,
                    OrgBranchName = apTravelExpenseValidation.OrgBranchName,
                    DateCreated = apTravelExpenseValidation.DateCreated,
                    LastModified = apTravelExpenseValidation.LastModified,
                    CreatedBy = apTravelExpenseValidation.CreatedBy,
                    ModifiedBy = apTravelExpenseValidation.ModifiedBy
                };

                _db.ApTravelExpenseValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { apTravelExpenseValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelExpenseClaimApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseValidations_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelExpenseValidation apTravelExpenseValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseValidation
                {
                    ApTravelExpenseValidationId = apTravelExpenseValidation.ApTravelExpenseValidationId,
                    Status = apTravelExpenseValidation.Status,
                    Remark = apTravelExpenseValidation.Remark,
                    OrgBranchName = apTravelExpenseValidation.OrgBranchName,
                    DateCreated = apTravelExpenseValidation.DateCreated,
                    LastModified = apTravelExpenseValidation.LastModified,
                    CreatedBy = apTravelExpenseValidation.CreatedBy,
                    ModifiedBy = apTravelExpenseValidation.ModifiedBy
                };

                _db.ApTravelExpenseValidations.Attach(entity);
                _db.ApTravelExpenseValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { apTravelExpenseValidation }.ToDataSourceResult(request, ModelState));
        }

        private void PostToSecondaryLedger(int id)
        {
            SecondLedgerHelper secondLedgerHelper = new SecondLedgerHelper();
            string user = User.Identity.Name;
            var record = _db.ApTravelExpenseClaims.Find(id);
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
                    SourcesName = "Expense Claim",
                    EffectiveDate = record.ClaimDate,
                    GlPeriodId = _fiscalYearHelper.GetPeriodOfDate(record.ClaimDate),
                    GlFiscalYearId = _fiscalYearHelper.GetFiscalYearOfDate(record.ClaimDate),
                    PostingSource = JournalSources.AP_TravelExpenseClaim,
                    PostJournalLineVms = new List<PostJournalVm.PostJournalLineVm>(),
                };

                #endregion
                #region distribution

                var distributions = _db.ApTravelExpenseDistributions.Where(x => x.ApTravelExpenseDistributionId == id).ToList();

                foreach (var distribution in distributions)
                {
                    var ledgerline = new PostJournalVm.PostJournalLineVm
                    {
                        PostJournalLineVmId = distribution.ApTravelExpenseDistributionId,
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
