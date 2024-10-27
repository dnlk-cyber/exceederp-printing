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
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.ProcurementPaymentRequest
{
    public class PurchasePaymentRequestBudgetValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment, FinanceConstants.AccountPayableView, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult PurchasePaymentBudgetValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PurchasePaymentBudgetValidation> PurchasePaymentBudgetValidations = _db.PurchasePaymentBudgetValidations.Where(x=>x.PurchasePaymentId==id);
            DataSourceResult result = PurchasePaymentBudgetValidations.ToDataSourceResult(request, PurchasePaymentBudgetValidation => new {
                PurchasePaymentBudgetValidationId = PurchasePaymentBudgetValidation.PurchasePaymentBudgetValidationId,
                Status = PurchasePaymentBudgetValidation.Status,
                Remark = PurchasePaymentBudgetValidation.Remark,
                OrgBranchName = PurchasePaymentBudgetValidation.OrgBranchName,
                DateCreated = PurchasePaymentBudgetValidation.DateCreated,
                LastModified = PurchasePaymentBudgetValidation.LastModified,
                CreatedBy = PurchasePaymentBudgetValidation.CreatedBy,
                ModifiedBy = PurchasePaymentBudgetValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentBudgetValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, PurchasePaymentBudgetValidation PurchasePaymentBudgetValidation)
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
                    if (PurchasePaymentBudgetValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.IsOnlineApproved = true;
                        record.Status = "Approved";
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        _db.PurchasePayments.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    if (PurchasePaymentBudgetValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        _db.PurchasePayments.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                  
                var entity = new PurchasePaymentBudgetValidation
                {
                    PurchasePaymentId = id,
                    Status = PurchasePaymentBudgetValidation.Status,
                    Remark = PurchasePaymentBudgetValidation.Remark,
                    OrgBranchName = PurchasePaymentBudgetValidation.OrgBranchName,
                    DateCreated = PurchasePaymentBudgetValidation.DateCreated,
                    LastModified = PurchasePaymentBudgetValidation.LastModified,
                    CreatedBy = PurchasePaymentBudgetValidation.CreatedBy,
                    ModifiedBy = PurchasePaymentBudgetValidation.ModifiedBy
                };
                    
                _db.PurchasePaymentBudgetValidations.Add(entity);
                _db.SaveChanges();
                PurchasePaymentBudgetValidation.PurchasePaymentBudgetValidationId = entity.PurchasePaymentBudgetValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment approval", "Success", User.Identity.Name, $"approved payment {entity.PurchasePaymentId} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { PurchasePaymentBudgetValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentBudgetValidations_Update([DataSourceRequest]DataSourceRequest request, PurchasePaymentBudgetValidation PurchasePaymentBudgetValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchasePaymentBudgetValidation
                {
                    PurchasePaymentBudgetValidationId = PurchasePaymentBudgetValidation.PurchasePaymentBudgetValidationId,
                    Status = PurchasePaymentBudgetValidation.Status,
                    Remark = PurchasePaymentBudgetValidation.Remark,
                    OrgBranchName = PurchasePaymentBudgetValidation.OrgBranchName,
                    DateCreated = PurchasePaymentBudgetValidation.DateCreated,
                    LastModified = PurchasePaymentBudgetValidation.LastModified,
                    CreatedBy = PurchasePaymentBudgetValidation.CreatedBy,
                    ModifiedBy = PurchasePaymentBudgetValidation.ModifiedBy
                };

                _db.PurchasePaymentBudgetValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { PurchasePaymentBudgetValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentBudgetValidations_Destroy([DataSourceRequest]DataSourceRequest request, PurchasePaymentBudgetValidation PurchasePaymentBudgetValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchasePaymentBudgetValidation
                {
                    PurchasePaymentBudgetValidationId = PurchasePaymentBudgetValidation.PurchasePaymentBudgetValidationId,
                    Status = PurchasePaymentBudgetValidation.Status,
                    Remark = PurchasePaymentBudgetValidation.Remark,
                    OrgBranchName = PurchasePaymentBudgetValidation.OrgBranchName,
                    DateCreated = PurchasePaymentBudgetValidation.DateCreated,
                    LastModified = PurchasePaymentBudgetValidation.LastModified,
                    CreatedBy = PurchasePaymentBudgetValidation.CreatedBy,
                    ModifiedBy = PurchasePaymentBudgetValidation.ModifiedBy
                };

                _db.PurchasePaymentBudgetValidations.Attach(entity);
                _db.PurchasePaymentBudgetValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { PurchasePaymentBudgetValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
