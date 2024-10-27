using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.ProcurementPaymentRequest
{
    public class PurchasePaymentFinanceValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment, FinanceConstants.AccountPayableApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment, FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover)]
        public ActionResult PurchasePaymentFinanceValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PurchasePaymentFinanceValidation> PurchasePaymentFinanceValidations = _db.PurchasePaymentFinanceValidations.Where(x=>x.PurchasePaymentId==id);
            DataSourceResult result = PurchasePaymentFinanceValidations.ToDataSourceResult(request, PurchasePaymentFinanceValidation => new {
                PurchasePaymentFinanceValidationId = PurchasePaymentFinanceValidation.PurchasePaymentFinanceValidationId,
                Status = PurchasePaymentFinanceValidation.Status,
                Remark = PurchasePaymentFinanceValidation.Remark,
                OrgBranchName = PurchasePaymentFinanceValidation.OrgBranchName,
                DateCreated = PurchasePaymentFinanceValidation.DateCreated,
                LastModified = PurchasePaymentFinanceValidation.LastModified,
                CreatedBy = PurchasePaymentFinanceValidation.CreatedBy,
                ModifiedBy = PurchasePaymentFinanceValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentFinanceValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, PurchasePaymentFinanceValidation PurchasePaymentFinanceValidation)
        {
            var record = _db.PurchasePayments.Find(id);

            if (record != null && record.IsFinanceApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: It is Already Approved");
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (PurchasePaymentFinanceValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.IsFinanceApproved = true;
                        record.Status = "Finance Approved";
                        record.FinanceApprovedBy = User.Identity.Name;
                        record.FinanceApprovedTime = DateTime.Now;
                        _db.PurchasePayments.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    if (PurchasePaymentFinanceValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.IsFinanceSendForApproval = false;
                        _db.PurchasePayments.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                  
                var entity = new PurchasePaymentFinanceValidation
                {
                    PurchasePaymentId = id,
                    Status = PurchasePaymentFinanceValidation.Status,
                    Remark = PurchasePaymentFinanceValidation.Remark,
                    OrgBranchName = PurchasePaymentFinanceValidation.OrgBranchName,
                    DateCreated =DateTime.Now,
                    LastModified = PurchasePaymentFinanceValidation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = PurchasePaymentFinanceValidation.ModifiedBy
                };

                _db.PurchasePaymentFinanceValidations.Add(entity);
                _db.SaveChanges();
                PurchasePaymentFinanceValidation.PurchasePaymentFinanceValidationId = entity.PurchasePaymentFinanceValidationId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment finance approval", "Success", User.Identity.Name, $"approved payment {entity.PurchasePaymentId} ", Modules.Finance);
                #endregion
                }
            }

            return Json(new[] { PurchasePaymentFinanceValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentFinanceValidations_Update([DataSourceRequest]DataSourceRequest request, PurchasePaymentFinanceValidation PurchasePaymentFinanceValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchasePaymentFinanceValidation
                {
                    PurchasePaymentFinanceValidationId = PurchasePaymentFinanceValidation.PurchasePaymentFinanceValidationId,
                    Status = PurchasePaymentFinanceValidation.Status,
                    Remark = PurchasePaymentFinanceValidation.Remark,
                    OrgBranchName = PurchasePaymentFinanceValidation.OrgBranchName,
                    DateCreated = PurchasePaymentFinanceValidation.DateCreated,
                    LastModified = PurchasePaymentFinanceValidation.LastModified,
                    CreatedBy = PurchasePaymentFinanceValidation.CreatedBy,
                    ModifiedBy = PurchasePaymentFinanceValidation.ModifiedBy
                };

                _db.PurchasePaymentFinanceValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { PurchasePaymentFinanceValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentFinanceValidations_Destroy([DataSourceRequest]DataSourceRequest request, PurchasePaymentFinanceValidation PurchasePaymentFinanceValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchasePaymentFinanceValidation
                {
                    PurchasePaymentFinanceValidationId = PurchasePaymentFinanceValidation.PurchasePaymentFinanceValidationId,
                    Status = PurchasePaymentFinanceValidation.Status,
                    Remark = PurchasePaymentFinanceValidation.Remark,
                    OrgBranchName = PurchasePaymentFinanceValidation.OrgBranchName,
                    DateCreated = PurchasePaymentFinanceValidation.DateCreated,
                    LastModified = PurchasePaymentFinanceValidation.LastModified,
                    CreatedBy = PurchasePaymentFinanceValidation.CreatedBy,
                    ModifiedBy = PurchasePaymentFinanceValidation.ModifiedBy
                };

                _db.PurchasePaymentFinanceValidations.Attach(entity);
                _db.PurchasePaymentFinanceValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { PurchasePaymentFinanceValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
