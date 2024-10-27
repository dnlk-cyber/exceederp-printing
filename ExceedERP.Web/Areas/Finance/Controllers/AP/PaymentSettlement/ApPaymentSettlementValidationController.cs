using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{

    public class ApPaymentSettlementValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayableSettlement)]
        public ActionResult Index()  
        {
            return View();
        }

         [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayableSettlement)]
        public ActionResult ApPaymentSettlementValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApPaymentSettlementValidation> ApPaymentSettlementValidations = _db.ApPaymentSettlementValidations.Where(x=>x.ApPaymentSettlementId==id);
            DataSourceResult result = ApPaymentSettlementValidations.ToDataSourceResult(request, ApPaymentSettlementValidation => new {
                ApPaymentSettlementValidationId = ApPaymentSettlementValidation.ApPaymentSettlementValidationId,
                ApPaymentSettlementId =id,
                Status = ApPaymentSettlementValidation.Status,
                Remark = ApPaymentSettlementValidation.Remark,
                OrgBranchName = ApPaymentSettlementValidation.OrgBranchName,
                DateCreated = ApPaymentSettlementValidation.DateCreated,
                LastModified = ApPaymentSettlementValidation.LastModified,
                CreatedBy = ApPaymentSettlementValidation.CreatedBy,
                ModifiedBy = ApPaymentSettlementValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlementValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApPaymentSettlementValidation ApPaymentSettlementValidation)
        {
            #region validated record
            var record = _db.ApPaymentSettlements.Find(id);

            if (record != null && record.IsOnlineApproved)
            {
               ModelState.AddModelError("approved","You cannot manipulate this record,it is already approved");
            }
            #endregion
            if (ModelState.IsValid)
            {
                if (record != null && !record.IsOnlineApproved)
                {
                    if (ApPaymentSettlementValidation.Status == ApprovalStatuses.Approve)
                    {
                       
                        #region pass data for reconciliation

                        //foreach (var distribution in record.ApPaymentSettlementDistributions)
                        //{
                        //    string sourceid = record.ApPaymentSettlementId + "-" + distribution.ApPaymentSettlementDistributionId;
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
                        //            Date =record.PaymentDate,
                        //            Type = "Direct Payment Settlement",
                        //            ReferenceDoc = !String.IsNullOrWhiteSpace(record.Remark) ? record.Remark : "NA",
                        //            Reference = !String.IsNullOrWhiteSpace(record.Reference) ? record.Reference : "NA",
                        //            Source = JournalSources.DirectPaymentSettlement.ToString(),
                        //            SourceID = sourceid,
                        //            AccountSegment = _accountHelper.GetNaturalAccountSegment(distribution.AccountCode),
                        //            SubAccountSegment = _accountHelper.GetSubAccountSegment(distribution.AccountCode)
                        //        };
                        //        _db.ReconcilationBookAccounts.Add(book);
                        //        _db.SaveChanges();
                        //    }
                        //}

                        #endregion
                        //PostToSecondaryLedger(id);
                        //record.IsPostedToSecondLedger = true;
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.IsVoid = false;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                        #region update direct payment

                        //int requestid = int.Parse(record.SourceId);
                        //var payment = _db.ForeignPurchaseCosts.Find(requestid);
                        //if (payment != null)
                        //{
                        //    payment.Paid = true;
                        //    _db.Entry(payment).State = EntityState.Modified;
                        //}

                        //_db.SaveChanges();
                        #endregion
                    }

                    if (ApPaymentSettlementValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                   


                    var entity = new ApPaymentSettlementValidation
                    {
                        ApPaymentSettlementId = id,
                        Status = ApPaymentSettlementValidation.Status,
                        Remark = ApPaymentSettlementValidation.Remark,
                        OrgBranchName = ApPaymentSettlementValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = ApPaymentSettlementValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = ApPaymentSettlementValidation.ModifiedBy
                    };

                    _db.ApPaymentSettlementValidations.Add(entity);
                    _db.SaveChanges();
                    ApPaymentSettlementValidation.ApPaymentSettlementValidationId =
                        entity.ApPaymentSettlementValidationId;
                }

            }

            return Json(new[] { ApPaymentSettlementValidation }.ToDataSourceResult(request, ModelState));
        }



       

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
