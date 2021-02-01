using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
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

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.PaymentRequest
{
    public class ApPaymentRequestValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        //[AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableTravelOrderApprover, FinanceConstants.AccountPayableTravelOrderApprover)]
        public ActionResult Index()
        {
            return View();
        }

       // [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableTravelOrderApprover, FinanceConstants.AccountPayableTravelOrderApprover)]
        public ActionResult PaymentRequestValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PaymentRequestValidation> paymentRequestValidations = _db.PaymentRequestValidations.Where(x=>x.PaymentRequestId==id);
            DataSourceResult result = paymentRequestValidations.ToDataSourceResult(request, paymentRequestValidation => new {
                PaymentRequestValidationId = paymentRequestValidation.PaymentRequestValidationId,
                Status = paymentRequestValidation.Status,
                Remark = paymentRequestValidation.Remark,
                OrgBranchName = paymentRequestValidation.OrgBranchName,
                DateCreated = paymentRequestValidation.DateCreated,
                LastModified = paymentRequestValidation.LastModified,
                CreatedBy = paymentRequestValidation.CreatedBy,
                ModifiedBy = paymentRequestValidation.ModifiedBy
            });

            return Json(result);
        }

        //[AuthorizeRoles(FinanceConstants.AccountPayableTravelOrderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequestValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, PaymentRequestValidation paymentRequestValidation)
        {
            
            var record = _db.PaymentRequests.Find(id);
            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            
            if (ModelState.IsValid)
            {

                if (record != null)
                {
                    if (paymentRequestValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                    }

                    if (paymentRequestValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new PaymentRequestValidation
                    {
                        PaymentRequestId = id,
                        Status = paymentRequestValidation.Status,
                        Remark = paymentRequestValidation.Remark,
                        OrgBranchName = paymentRequestValidation.OrgBranchName,
                        DateCreated = paymentRequestValidation.DateCreated,
                        LastModified = paymentRequestValidation.LastModified,
                        CreatedBy = paymentRequestValidation.CreatedBy,
                        ModifiedBy = paymentRequestValidation.ModifiedBy
                    };

                    _db.PaymentRequestValidations.Add(entity);
                    _db.SaveChanges();
                    paymentRequestValidation.PaymentRequestValidationId = entity.PaymentRequestValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP payment request approval", "Success", User.Identity.Name, $"approved payment request {entity.PaymentRequestId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { paymentRequestValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelOrderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequestValidations_Update([DataSourceRequest]DataSourceRequest request, PaymentRequestValidation paymentRequestValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PaymentRequestValidation
                {
                    PaymentRequestValidationId = paymentRequestValidation.PaymentRequestValidationId,
                    Status = paymentRequestValidation.Status,
                    Remark = paymentRequestValidation.Remark,
                    OrgBranchName = paymentRequestValidation.OrgBranchName,
                    DateCreated = paymentRequestValidation.DateCreated,
                    LastModified = paymentRequestValidation.LastModified,
                    CreatedBy = paymentRequestValidation.CreatedBy,
                    ModifiedBy = paymentRequestValidation.ModifiedBy
                };

                _db.PaymentRequestValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { paymentRequestValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelOrderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequestValidations_Destroy([DataSourceRequest]DataSourceRequest request, PaymentRequestValidation paymentRequestValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PaymentRequestValidation
                {
                    PaymentRequestValidationId = paymentRequestValidation.PaymentRequestValidationId,
                    Status = paymentRequestValidation.Status,
                    Remark = paymentRequestValidation.Remark,
                    OrgBranchName = paymentRequestValidation.OrgBranchName,
                    DateCreated = paymentRequestValidation.DateCreated,
                    LastModified = paymentRequestValidation.LastModified,
                    CreatedBy = paymentRequestValidation.CreatedBy,
                    ModifiedBy = paymentRequestValidation.ModifiedBy
                };

                _db.PaymentRequestValidations.Attach(entity);
                _db.PaymentRequestValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { paymentRequestValidation }.ToDataSourceResult(request, ModelState));
        }

       
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
