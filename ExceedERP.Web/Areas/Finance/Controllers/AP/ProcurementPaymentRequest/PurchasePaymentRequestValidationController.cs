using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.ProcurementPaymentRequest
{
    public class PurchasePaymentRequestValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment, FinanceConstants.AccountPayableApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment, FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover)]
        public ActionResult PurchasePaymentValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PurchasePaymentValidation> PurchasePaymentValidations = _db.PurchasePaymentValidations.Where(x=>x.PurchasePaymentId==id);
            DataSourceResult result = PurchasePaymentValidations.ToDataSourceResult(request, PurchasePaymentValidation => new {
                PurchasePaymentValidationId = PurchasePaymentValidation.PurchasePaymentValidationId,
                Status = PurchasePaymentValidation.Status,
                Remark = PurchasePaymentValidation.Remark,
                OrgBranchName = PurchasePaymentValidation.OrgBranchName,
                DateCreated = PurchasePaymentValidation.DateCreated,
                LastModified = PurchasePaymentValidation.LastModified,
                CreatedBy = PurchasePaymentValidation.CreatedBy,
                ModifiedBy = PurchasePaymentValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, PurchasePaymentValidation PurchasePaymentValidation)
        {
            var record = _db.PurchasePayments.Find(id);

            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: It is Already Approved");
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (PurchasePaymentValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.IsOnlineApproved = true;
                        record.Status = "Approved";
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        _db.PurchasePayments.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    if (PurchasePaymentValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        _db.PurchasePayments.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                  
                var entity = new PurchasePaymentValidation
                {
                    PurchasePaymentId = id,
                    Status = PurchasePaymentValidation.Status,
                    Remark = PurchasePaymentValidation.Remark,
                    OrgBranchName = PurchasePaymentValidation.OrgBranchName,
                    DateCreated =DateTime.Now,
                    LastModified = PurchasePaymentValidation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = PurchasePaymentValidation.ModifiedBy
                };

                _db.PurchasePaymentValidations.Add(entity);
                _db.SaveChanges();
                PurchasePaymentValidation.PurchasePaymentValidationId = entity.PurchasePaymentValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment approval", "Success", User.Identity.Name, $"approved payment {entity.PurchasePaymentId} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { PurchasePaymentValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentValidations_Update([DataSourceRequest]DataSourceRequest request, PurchasePaymentValidation PurchasePaymentValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchasePaymentValidation
                {
                    PurchasePaymentValidationId = PurchasePaymentValidation.PurchasePaymentValidationId,
                    Status = PurchasePaymentValidation.Status,
                    Remark = PurchasePaymentValidation.Remark,
                    OrgBranchName = PurchasePaymentValidation.OrgBranchName,
                    DateCreated = PurchasePaymentValidation.DateCreated,
                    LastModified = PurchasePaymentValidation.LastModified,
                    CreatedBy = PurchasePaymentValidation.CreatedBy,
                    ModifiedBy = PurchasePaymentValidation.ModifiedBy
                };

                _db.PurchasePaymentValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { PurchasePaymentValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentValidations_Destroy([DataSourceRequest]DataSourceRequest request, PurchasePaymentValidation PurchasePaymentValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchasePaymentValidation
                {
                    PurchasePaymentValidationId = PurchasePaymentValidation.PurchasePaymentValidationId,
                    Status = PurchasePaymentValidation.Status,
                    Remark = PurchasePaymentValidation.Remark,
                    OrgBranchName = PurchasePaymentValidation.OrgBranchName,
                    DateCreated = PurchasePaymentValidation.DateCreated,
                    LastModified = PurchasePaymentValidation.LastModified,
                    CreatedBy = PurchasePaymentValidation.CreatedBy,
                    ModifiedBy = PurchasePaymentValidation.ModifiedBy
                };

                _db.PurchasePaymentValidations.Attach(entity);
                _db.PurchasePaymentValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { PurchasePaymentValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
