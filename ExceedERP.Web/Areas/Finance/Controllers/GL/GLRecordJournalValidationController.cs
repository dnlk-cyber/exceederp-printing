using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlRecordJournalValidationController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();
        private FinanceHelper _financeHelper = new FinanceHelper();


        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser ,FinanceConstants.GeneralLedgerApprover ,FinanceConstants.GeneralLedgerView)]
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser ,FinanceConstants.GeneralLedgerApprover ,FinanceConstants.GeneralLedgerView)]
        public ActionResult GLRecordJournalValidations_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLRecordJournalValidation> glrecordjournalvalidations = _db.GLRecordJournalValidations.Where(x => x.GlRecordJournalId == id);
            DataSourceResult result = glrecordjournalvalidations.ToDataSourceResult(request, gLRecordJournalValidation => new {
                GlRecordJournalValidationId = gLRecordJournalValidation.GlRecordJournalValidationId,
                GlRecordJournalId = gLRecordJournalValidation.GlRecordJournalId,
                Status = gLRecordJournalValidation.Status,
                Remark = gLRecordJournalValidation.Remark,
                OrgBranchName = gLRecordJournalValidation.OrgBranchName,
                DateCreated = gLRecordJournalValidation.DateCreated,
                LastModified = gLRecordJournalValidation.LastModified,
                CreatedBy = gLRecordJournalValidation.CreatedBy,
                ModifiedBy = gLRecordJournalValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLRecordJournalValidations_Create(int id, [DataSourceRequest]DataSourceRequest request, GLRecordJournalValidation gLRecordJournalValidation)
        {
            if (id == 0)
                ModelState.AddModelError("id", "Add Journal First");
            string errMsg = "";
            var record = _db.GLRecordJournals.Find(id);

            if (gLRecordJournalValidation.Status == ApprovalStatuses.Approve)
            {
                #region get parent record status              

                if (record != null && record.IsOnlineApproved)
                {
                    errMsg = "You cannot manipulate this record,it is already approved";
                    ModelState.AddModelError("validated", errMsg);
                }
                else if (record != null && !record.IsSendForApproval)
                {
                    errMsg = "You cannot approve this record,it is not sent for approval";
                    ModelState.AddModelError("notsent", errMsg);
                }
                else if (record != null && !record.IsVoidDoc && Math.Abs(record.GLRecordJournalDetail.Sum(x => x.Credit) - record.GLRecordJournalDetail.Sum(x => x.Debit)) > 0)
                {
                    errMsg = "Total debit and total credit is not balanced";
                    ModelState.AddModelError("balance", errMsg);
                }

                var lines = _db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == id).AsNoTracking();
                AccountHelper accountHelper = new AccountHelper();
                errMsg = accountHelper.IsValidJournal(lines);
                if (!String.IsNullOrEmpty(errMsg) && !record.IsVoidDoc)
                    ModelState.AddModelError("balance", errMsg);

                #endregion
            }
            if (ModelState.IsValid)
            {
                //validate 
                if (gLRecordJournalValidation.Status == ApprovalStatuses.Approve)
                {
                    if (record != null)
                    {                       
                         record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.GLRecordJournals.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                        // auto post ..to change by preference setup
                        //_financeHelper.PostSingleToLedger(record.GlRecordJournalId, User.Identity.Name);
                    }

                    //if (record != null && record.SourcesName == "GL" && !record.IsVoidDoc)
                    //{
                    //    #region pass data for bank reconciliation

                    //    foreach (GLRecordJournalDetail distribution in record.GLRecordJournalDetail)
                    //    {
                    //        string sourceid = record.GlRecordJournalId + "-" + distribution.GlRecordJournalDetailId;
                    //        //check if this is bank account
                    //        if (_accountHelper.IsBankAccount(distribution.AccountCode))
                    //        {
                    //            var book = new ReconciliationBookAccount
                    //            {
                    //                CreatedBy = User.Identity.Name,
                    //                DateCreated = DateTime.Now,
                    //                AccountCode = distribution.AccountCode,
                    //                Debit = distribution.Debit,
                    //                Credit = distribution.Credit,
                    //                Date = distribution.GLRecordJournal.EffectiveDate,
                    //                Type = "GL Record",
                    //                ReferenceDoc = !String.IsNullOrWhiteSpace(record.JournalNumber) ? record.JournalNumber : "NA",
                    //                Reference = !String.IsNullOrWhiteSpace(distribution.AdditionalDescription) ? distribution.AdditionalDescription : "NA",
                    //                Source = JournalSources.GeneralLedger.ToString(),
                    //                SourceID = sourceid,
                    //                AccountSegment = _accountHelper.GetNaturalAccountSegment(distribution.AccountCode),
                    //                SubAccountSegment = _accountHelper.GetSubAccountSegment(distribution.AccountCode)
                    //            };
                    //            _db.ReconcilationBookAccounts.Add(book);
                    //            _db.SaveChanges();
                    //        }
                    //    }

                    //    #endregion

                    //}

                    if (record != null && !record.IsVoidDoc)
                    {
                        #region split journal

                        foreach (GLRecordJournalDetail glRecordJournalDetail in record.GLRecordJournalDetail)
                        {
                            AccountHelper helper = new AccountHelper();
                            helper.SplitJournal(glRecordJournalDetail.GlRecordJournalDetailId);
                        }

                        #endregion
                    }

                }
                if (gLRecordJournalValidation.Status == ApprovalStatuses.Reject)
                {
                    if (record != null)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.GLRecordJournals.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }
                var entity = new GLRecordJournalValidation
                {
                    GlRecordJournalId = id,
                    Status = gLRecordJournalValidation.Status,
                    Remark = gLRecordJournalValidation.Remark,
                    OrgBranchName = gLRecordJournalValidation.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = gLRecordJournalValidation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = gLRecordJournalValidation.ModifiedBy
                };
                _db.GLRecordJournalValidations.Add(entity);
                _db.SaveChanges();
                gLRecordJournalValidation.GlRecordJournalValidationId = entity.GlRecordJournalValidationId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL Journal approval", "Success", User.Identity.Name, $"approved journal {entity.GlRecordJournalId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLRecordJournalValidation }.ToDataSourceResult(request, ModelState));
        }

        

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
