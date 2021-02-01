using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Finance.Bank;
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
    public class ApTravelOrderValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableTravelOrderApprover, FinanceConstants.AccountPayableTravelOrderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableTravelOrderApprover, FinanceConstants.AccountPayableTravelOrderApprover)]
        public ActionResult ApTravelOrderValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelOrderValidation> aptravelordervalidations = _db.ApTravelOrderValidations.Where(x=>x.ApTravelOrderId==id);
            DataSourceResult result = aptravelordervalidations.ToDataSourceResult(request, apTravelOrderValidation => new {
                ApTravelOrderValidationId = apTravelOrderValidation.ApTravelOrderValidationId,
                Status = apTravelOrderValidation.Status,
                Remark = apTravelOrderValidation.Remark,
                OrgBranchName = apTravelOrderValidation.OrgBranchName,
                DateCreated = apTravelOrderValidation.DateCreated,
                LastModified = apTravelOrderValidation.LastModified,
                CreatedBy = apTravelOrderValidation.CreatedBy,
                ModifiedBy = apTravelOrderValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelOrderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApTravelOrderValidation apTravelOrderValidation)
        {
            
            var record = _db.ApTravelOrders.Find(id);
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
                    if (apTravelOrderValidation.Status == ApprovalStatuses.Approve)
                    {
                        if (record.IsFinalApproval)
                        {
                           // PostToSecondaryLedger(id);
                            record.IsPostedToSecondLedger = true;
                            record.Status = "Approved";
                            record.IsOnlineApproved = true;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            _db.Entry(record).State = EntityState.Modified;
                            _db.SaveChanges();
                            #region pass data for reconciliation

                            foreach (ApTravelOrderDistribution distribution in record.ApTravelOrderDistributions)
                            {
                                string sourceid = record.ApTravelOrderId + "-" + distribution.ApTravelOrderDistributionId;
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
                                        Date = record.RequestDate,
                                        Type = "Travel Order",
                                        ReferenceDoc = !String.IsNullOrWhiteSpace(record.Remark) ? record.Remark : "NA",
                                        Reference = !String.IsNullOrWhiteSpace(record.Reference) ? record.Reference : "NA",
                                        Source =JournalSources.AP_TravelOrder.ToString(),
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
                                Message = $"Travel order document {record.ApTravelOrderId} is approved by User: {record.ForwardedTo} at {DateTime.Now}"
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
                                Message = $"Travel order document {record.ApTravelOrderId} is approved by User: {record.ForwardedTo} at {DateTime.Now} and forwarded to next user."
                            };

                            Notify.NotifyUser(notif);

                            #endregion
                            #region forward to next approver

                            decimal docamount = record.AdvanceAmount;
                            DocApprovalWorkflowHelper.ForwardToApprover(record.ApTravelOrderId, docamount, "", record.ForwardedTo, ApprovalDocumentType.ApTravelOrder, false);

                            #endregion
                        }

                    }

                

                    if (apTravelOrderValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new ApTravelOrderValidation
                    {
                        ApTravelOrderId = id,
                        Status = apTravelOrderValidation.Status,
                        Remark = apTravelOrderValidation.Remark,
                        OrgBranchName = apTravelOrderValidation.OrgBranchName,
                        DateCreated = apTravelOrderValidation.DateCreated,
                        LastModified = apTravelOrderValidation.LastModified,
                        CreatedBy = apTravelOrderValidation.CreatedBy,
                        ModifiedBy = apTravelOrderValidation.ModifiedBy
                    };

                    _db.ApTravelOrderValidations.Add(entity);
                    _db.SaveChanges();
                    apTravelOrderValidation.ApTravelOrderValidationId = entity.ApTravelOrderValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP travel order approval", "Success", User.Identity.Name, $"approved travel order {entity.ApTravelOrderId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { apTravelOrderValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelOrderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderValidations_Update([DataSourceRequest]DataSourceRequest request, ApTravelOrderValidation apTravelOrderValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderValidation
                {
                    ApTravelOrderValidationId = apTravelOrderValidation.ApTravelOrderValidationId,
                    Status = apTravelOrderValidation.Status,
                    Remark = apTravelOrderValidation.Remark,
                    OrgBranchName = apTravelOrderValidation.OrgBranchName,
                    DateCreated = apTravelOrderValidation.DateCreated,
                    LastModified = apTravelOrderValidation.LastModified,
                    CreatedBy = apTravelOrderValidation.CreatedBy,
                    ModifiedBy = apTravelOrderValidation.ModifiedBy
                };

                _db.ApTravelOrderValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { apTravelOrderValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelOrderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderValidations_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelOrderValidation apTravelOrderValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderValidation
                {
                    ApTravelOrderValidationId = apTravelOrderValidation.ApTravelOrderValidationId,
                    Status = apTravelOrderValidation.Status,
                    Remark = apTravelOrderValidation.Remark,
                    OrgBranchName = apTravelOrderValidation.OrgBranchName,
                    DateCreated = apTravelOrderValidation.DateCreated,
                    LastModified = apTravelOrderValidation.LastModified,
                    CreatedBy = apTravelOrderValidation.CreatedBy,
                    ModifiedBy = apTravelOrderValidation.ModifiedBy
                };

                _db.ApTravelOrderValidations.Attach(entity);
                _db.ApTravelOrderValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { apTravelOrderValidation }.ToDataSourceResult(request, ModelState));
        }

        private void PostToSecondaryLedger(int id)
        {
            string user = User.Identity.Name;
            var record = _db.ApTravelOrders.Find(id);
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
                    SourcesName = "Travel Order",
                    EffectiveDate = record.RequestDate,
                    GlPeriodId = _fiscalYearHelper.GetPeriodOfDate(record.RequestDate),
                    GlFiscalYearId = _fiscalYearHelper.GetFiscalYearOfDate(record.RequestDate),
                    PostingSource = JournalSources.AP_TravelExpenseClaim,
                    PostJournalLineVms = new List<PostJournalVm.PostJournalLineVm>(),
                };

                #endregion
                #region distribution

                var distributions = _db.ApTravelOrderDistributions.Where(x => x.ApTravelOrderId == id).ToList();

                foreach (var distribution in distributions)
                {
                    var ledgerline = new PostJournalVm.PostJournalLineVm
                    {
                        PostJournalLineVmId = distribution.ApTravelOrderDistributionId,
                        AccountCode = distribution.AccountCode,
                        Debit = distribution.Debit,
                        Credit = distribution.Credit
                    };
                    ledger.PostJournalLineVms.Add(ledgerline);
                }

                #endregion
                SecondLedgerHelper secondLedgerHelper = new SecondLedgerHelper();
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
