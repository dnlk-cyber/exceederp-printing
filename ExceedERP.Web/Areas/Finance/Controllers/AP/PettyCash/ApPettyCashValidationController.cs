using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP.PettyCash;
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

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.PettyCash
{
    public class ApPettyCashValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayablePettyCashApprover, FinanceConstants.AccountPayablePettyCash)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayablePettyCashApprover, FinanceConstants.AccountPayablePettyCash)]
        public ActionResult ApPettyCashValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApPettyCashValidation> appettycashvalidations = _db.ApPettyCashValidations.Where(x => x.ApPettyCashId == id);
            DataSourceResult result = appettycashvalidations.ToDataSourceResult(request, apPettyCashValidation => new {
                ApPettyCashValidationId = apPettyCashValidation.ApPettyCashValidationId,
                Status = apPettyCashValidation.Status,
                Remark = apPettyCashValidation.Remark,
                OrgBranchName = apPettyCashValidation.OrgBranchName,
                DateCreated = apPettyCashValidation.DateCreated,
                LastModified = apPettyCashValidation.LastModified,
                CreatedBy = apPettyCashValidation.CreatedBy,
                ModifiedBy = apPettyCashValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePettyCashApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPettyCashValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApPettyCashValidation apPettyCashValidation)
        {
            #region validated record
            ApPettyCash record = _db.ApPettyCash.Find(id);

            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            if (record != null && record.ForwardedTo != User.Identity.Name)
                ModelState.AddModelError("notcorrectuser", "You can't approve this record,it is forwarded to another user.");

            var hasApproved = _db.PurchaseOrderValidations
                .Where(x => x.PurchaseOrderID == id)
                .Where(x => x.CreatedBy == User.Identity.Name && x.Status == ApprovalStatuses.Approve).AsQueryable();
            if (hasApproved.Any())
                ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");
            #endregion 
            if (ModelState.IsValid)
            {
                if (record != null && !record.IsOnlineApproved)
                {
                    if (apPettyCashValidation.Status == ApprovalStatuses.Approve)
                    {
                        if (record.IsFinalApproval)
                        {
                            //PostToSecondaryLedger(id);
                            record.IsPostedToSecondLedger = true;
                            record.Status = "Approved";
                            record.IsOnlineApproved = true;
                            record.IsVoid = false;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            _db.Entry(record).State = EntityState.Modified;
                            _db.SaveChanges();

                            #region pass data for reconciliation

                            foreach (ApPettyCashDistribution distribution in record.PettyCashDistributions)
                            {
                                string sourceid = record.ApPettyCashId + "-" + distribution.ApPettyCashDistributionId;
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
                                        Date = record.Date,
                                        Type = "Petty Cash",
                                        ReferenceDoc =
                                            !String.IsNullOrWhiteSpace(record.Reference) ? record.Reference : "NA",
                                        Reference = !String.IsNullOrWhiteSpace(distribution.AdditionalDescription)
                                            ? distribution.AdditionalDescription
                                            : "NA",
                                        Source = JournalSources.AP_PettyCash.ToString(),
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
                                    $"Petty Cash document {record.ApPettyCashId} is approved by User: {record.ForwardedTo} at {DateTime.Now}"
                            };

                            Notify.NotifyUser(notif);

                            #endregion
                        }
                        else
                        {
                            record.Status = "Partially Approved";
                            record.IsOnlineApproved = false;
                            record.IsVoid = false;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            _db.Entry(record).State = EntityState.Modified;
                            _db.SaveChanges();

                            #region approval notification

                            NotifModels notif = new NotifModels
                            {
                                UserId = record.ForwardedFrom,
                                Message =
                                    $"Petty Cash document {record.ApPettyCashId} is approved by User: {record.ForwardedTo} at {DateTime.Now} and forwarded to next user."
                            };

                            Notify.NotifyUser(notif);

                            #endregion

                            #region forward to next approver

                            decimal paymenttotal = record.PettyCashPayments.Sum(x => x.Amount);
                            decimal utilizationtotal = record.PettyCashUtilizations.Sum(x => x.Amount);
                            decimal docamount = 0;
                            docamount = utilizationtotal > paymenttotal ? utilizationtotal : paymenttotal;

                            DocApprovalWorkflowHelper.ForwardToApprover(record.ApPettyCashId, docamount, "",
                                record.ForwardedTo, ApprovalDocumentType.ApPettyCash, false);

                            #endregion

                        }
                    }

                    if (apPettyCashValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                        //notification
                    }

                    var entity = new ApPettyCashValidation
                    {
                        ApPettyCashId = id,
                        Status = apPettyCashValidation.Status,
                        Remark = apPettyCashValidation.Remark,
                        OrgBranchName = apPettyCashValidation.OrgBranchName,
                        DateCreated = apPettyCashValidation.DateCreated,
                        LastModified = apPettyCashValidation.LastModified,
                        CreatedBy = apPettyCashValidation.CreatedBy,
                        ModifiedBy = apPettyCashValidation.ModifiedBy
                    };

                    _db.ApPettyCashValidations.Add(entity);
                    _db.SaveChanges();
                    apPettyCashValidation.ApPettyCashValidationId = entity.ApPettyCashValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP petty cash approval", "Success", User.Identity.Name, $"approved petty cash {entity.ApPettyCashId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { apPettyCashValidation }.ToDataSourceResult(request, ModelState));
        }

        private void PostToSecondaryLedger(int id)
        {
            SecondLedgerHelper secondLedgerHelper = new SecondLedgerHelper();
            string user = User.Identity.Name;
            var record = _db.ApPettyCash.Find(id);
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
                    SourcesName = "Petty Cash",
                    EffectiveDate = record.Date,
                    GlPeriodId = _fiscalYearHelper.GetPeriodOfDate(record.Date),
                    GlFiscalYearId = _fiscalYearHelper.GetFiscalYearOfDate(record.Date),
                    PostingSource = JournalSources.AP_PettyCash,
                    PostJournalLineVms = new List<PostJournalVm.PostJournalLineVm>(),
                };

                #endregion
                #region distribution

                var distributions = _db.ApPettyCashDistributions.Where(x => x.ApPettyCashId == id).ToList();

                foreach (var distribution in distributions)
                {
                    var ledgerline = new PostJournalVm.PostJournalLineVm
                    {
                        PostJournalLineVmId = distribution.ApPettyCashDistributionId,
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
