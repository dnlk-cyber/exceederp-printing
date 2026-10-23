using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.PaymentRequests
{
    public class PaymentRequestAuthorizeValidationController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayablePaymentRequestAuthorizer)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayablePaymentRequestAuthorizer)]
        public ActionResult PaymentRequestAuthorizeValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PaymentRequestAuthorizeValidation> PaymentRequestAuthorizeValidations = _db.PaymentRequestAuthorizeValidations.Where(x=>x.PaymentRequestId==id);
            DataSourceResult result = PaymentRequestAuthorizeValidations.ToDataSourceResult(request, PaymentRequestAuthorizeValidation => new {
                PaymentRequestAuthorizeValidationId = PaymentRequestAuthorizeValidation.PaymentRequestAuthorizeValidationId,
                PaymentRequestId=id,
                Status = PaymentRequestAuthorizeValidation.Status,
                Remark = PaymentRequestAuthorizeValidation.Remark,
                OrgBranchName = PaymentRequestAuthorizeValidation.OrgBranchName,
                DateCreated = PaymentRequestAuthorizeValidation.DateCreated,
                LastModified = PaymentRequestAuthorizeValidation.LastModified,
                CreatedBy = PaymentRequestAuthorizeValidation.CreatedBy,
                ModifiedBy = PaymentRequestAuthorizeValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePaymentRequestAuthorizer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequestAuthorizeValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, PaymentRequestAuthorizeValidation paymentRequestAuthorizeValidation)
        {
            var record = _db.PaymentRequests.Find(id);
            if (record != null && record.IsAuthorized)
                ModelState.AddModelError("authorized", "You cannot manipulate this record,it is already authorized");


            if (ModelState.IsValid)
            {

                if (record != null)
                {
                    if (paymentRequestAuthorizeValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Authorized";
                        record.IsAuthorized = true;
                        record.AuthorizedBy = User.Identity.Name;
                        record.AuthorizedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                    }

                    if (paymentRequestAuthorizeValidation.Status == ApprovalStatuses.Reject)
                    {
                        //record.IsSendForApproval = false;
                        record.Status = "Authorization Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new PaymentRequestAuthorizeValidation
                    {
                        PaymentRequestId = id,
                        Status = paymentRequestAuthorizeValidation.Status,
                        Remark = paymentRequestAuthorizeValidation.Remark,
                        OrgBranchName = paymentRequestAuthorizeValidation.OrgBranchName,
                        DateCreated = paymentRequestAuthorizeValidation.DateCreated,
                        LastModified = paymentRequestAuthorizeValidation.LastModified,
                        CreatedBy = paymentRequestAuthorizeValidation.CreatedBy,
                        ModifiedBy = paymentRequestAuthorizeValidation.ModifiedBy
                    };

                    _db.PaymentRequestAuthorizeValidations.Add(entity);
                    _db.SaveChanges();
                    paymentRequestAuthorizeValidation.PaymentRequestAuthorizeValidationId = entity.PaymentRequestAuthorizeValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP payment request authorization", "Success", User.Identity.Name, $"approved payment request {entity.PaymentRequestId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { paymentRequestAuthorizeValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePaymentRequestAuthorizer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequestAuthorizeValidations_Update(int id,[DataSourceRequest]DataSourceRequest request, PaymentRequestAuthorizeValidation PaymentRequestAuthorizeValidation)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region ger parent record status
                var record = _db.PaymentRequests.Find(id);

                if (record != null && record.IsAuthorized)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record,it is already approved";
                    //return View("Error");
                }
                else if (record != null && !record.IsOnlineApproved)
                {
                    //validate 
                    if (PaymentRequestAuthorizeValidation.Status ==ApprovalStatuses.Approve)
                    {
                       
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                    }
                    if (PaymentRequestAuthorizeValidation.Status ==ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
                #endregion

                if (isvalid)
                {
                    var entity = new PaymentRequestAuthorizeValidation
                    {
                        PaymentRequestId = id,
                        PaymentRequestAuthorizeValidationId = PaymentRequestAuthorizeValidation.PaymentRequestAuthorizeValidationId,
                        Status = PaymentRequestAuthorizeValidation.Status,
                        Remark = PaymentRequestAuthorizeValidation.Remark,
                        OrgBranchName = PaymentRequestAuthorizeValidation.OrgBranchName,
                        DateCreated = PaymentRequestAuthorizeValidation.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = PaymentRequestAuthorizeValidation.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.PaymentRequestAuthorizeValidations.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            return Json(new[] { PaymentRequestAuthorizeValidation }.ToDataSourceResult(request, ModelState));
        }

        

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
