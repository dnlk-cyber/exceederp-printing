using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.PurchasePayments
{
    public class PurchasePaymentAuthorizeValidationController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayablePaymentRequestAuthorizer)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayablePaymentRequestAuthorizer)]
        public ActionResult PurchasePaymentAuthorizeValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PurchasePaymentAuthorizeValidation> PurchasePaymentAuthorizeValidations = _db.PurchasePaymentAuthorizeValidations.Where(x=>x.PurchasePaymentId==id);
            DataSourceResult result = PurchasePaymentAuthorizeValidations.ToDataSourceResult(request, PurchasePaymentAuthorizeValidation => new {
                PurchasePaymentAuthorizeValidationId = PurchasePaymentAuthorizeValidation.PurchasePaymentAuthorizeValidationId,
                PurchasePaymentId=id,
                Status = PurchasePaymentAuthorizeValidation.Status,
                Remark = PurchasePaymentAuthorizeValidation.Remark,
                OrgBranchName = PurchasePaymentAuthorizeValidation.OrgBranchName,
                DateCreated = PurchasePaymentAuthorizeValidation.DateCreated,
                LastModified = PurchasePaymentAuthorizeValidation.LastModified,
                CreatedBy = PurchasePaymentAuthorizeValidation.CreatedBy,
                ModifiedBy = PurchasePaymentAuthorizeValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePaymentRequestAuthorizer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentAuthorizeValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, PurchasePaymentAuthorizeValidation PurchasePaymentAuthorizeValidation)
        {
            var record = _db.PurchasePayments.Find(id);
            if (record != null && record.IsAuthorized)
                ModelState.AddModelError("authorized", "You cannot manipulate this record,it is already authorized");


            if (ModelState.IsValid)
            {

                if (record != null)
                {
                    if (PurchasePaymentAuthorizeValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Authorized";
                        record.IsAuthorized = true;
                        record.AuthorizedBy = User.Identity.Name;
                        record.AuthorizedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                    }

                    if (PurchasePaymentAuthorizeValidation.Status == ApprovalStatuses.Reject)
                    {
                        //record.IsSendForApproval = false;
                        record.Status = "Authorization Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new PurchasePaymentAuthorizeValidation
                    {
                        PurchasePaymentId = id,
                        Status = PurchasePaymentAuthorizeValidation.Status,
                        Remark = PurchasePaymentAuthorizeValidation.Remark,
                        OrgBranchName = PurchasePaymentAuthorizeValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = PurchasePaymentAuthorizeValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = PurchasePaymentAuthorizeValidation.ModifiedBy
                    };

                    _db.PurchasePaymentAuthorizeValidations.Add(entity);
                    _db.SaveChanges();
                    PurchasePaymentAuthorizeValidation.PurchasePaymentAuthorizeValidationId = entity.PurchasePaymentAuthorizeValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP payment request authorization", "Success", User.Identity.Name, $"approved payment request {entity.PurchasePaymentId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { PurchasePaymentAuthorizeValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePaymentRequestAuthorizer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentAuthorizeValidations_Update(int id,[DataSourceRequest]DataSourceRequest request, PurchasePaymentAuthorizeValidation PurchasePaymentAuthorizeValidation)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region ger parent record status
                var record = _db.PurchasePayments.Find(id);

                if (record != null && record.IsAuthorized)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record,it is already approved";
                    //return View("Error");
                }
                else if (record != null && !record.IsOnlineApproved)
                {
                    //validate 
                    if (PurchasePaymentAuthorizeValidation.Status ==ApprovalStatuses.Approve)
                    {
                       
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                    }
                    if (PurchasePaymentAuthorizeValidation.Status ==ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
                #endregion

                if (isvalid)
                {
                    var entity = new PurchasePaymentAuthorizeValidation
                    {
                        PurchasePaymentId = id,
                        PurchasePaymentAuthorizeValidationId = PurchasePaymentAuthorizeValidation.PurchasePaymentAuthorizeValidationId,
                        Status = PurchasePaymentAuthorizeValidation.Status,
                        Remark = PurchasePaymentAuthorizeValidation.Remark,
                        OrgBranchName = PurchasePaymentAuthorizeValidation.OrgBranchName,
                        DateCreated = PurchasePaymentAuthorizeValidation.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = PurchasePaymentAuthorizeValidation.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.PurchasePaymentAuthorizeValidations.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            return Json(new[] { PurchasePaymentAuthorizeValidation }.ToDataSourceResult(request, ModelState));
        }

        

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
