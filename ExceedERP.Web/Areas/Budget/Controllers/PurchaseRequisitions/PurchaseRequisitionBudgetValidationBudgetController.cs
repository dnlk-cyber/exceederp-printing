using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Budget.Controllers.PaymentRequests
{
    public class PurchaseRequisitionBudgetValidationBudgetController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult PurchaseRequisitionBudgetValidations_Read(int id, [DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<PurchaseRequisitionBudgetValidation> PurchaseRequisitionBudgetValidations = _db.PurchaseRequisitionBudgetValidations.Where(x => x.PurchaseRequisitionID == id);
            DataSourceResult result = PurchaseRequisitionBudgetValidations.ToDataSourceResult(request, PurchaseRequisitionBudgetValidation => new
            {
                PurchaseRequisitionBudgetValidationId = PurchaseRequisitionBudgetValidation.PurchaseRequisitionBudgetValidationId,
                PurchaseRequisitionID = id,
                Status = PurchaseRequisitionBudgetValidation.Status,
                Remark = PurchaseRequisitionBudgetValidation.Remark,
                DateCreated = PurchaseRequisitionBudgetValidation.DateCreated,
                LastModified = PurchaseRequisitionBudgetValidation.LastModified,
                CreatedBy = PurchaseRequisitionBudgetValidation.CreatedBy,
                ModifiedBy = PurchaseRequisitionBudgetValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(BudgetRoles.BudgetPaymentApproval)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionBudgetValidations_Create(int id, [DataSourceRequest] DataSourceRequest request, PurchaseRequisitionBudgetValidation PurchaseRequisitionBudgetValidation)
        {
            var record = _db.PurchaseRequisitions.Find(id);
            if (record != null && record.IsBudgetApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");


            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (PurchaseRequisitionBudgetValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Budget Approved";
                        record.IsBudgetApproved = true;
                        record.BudgetApprovedBy = User.Identity.Name;
                        record.BudgetApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                    }

                    if (PurchaseRequisitionBudgetValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsBudgetApproved = false;
                        record.Status = "Budget Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new PurchaseRequisitionBudgetValidation
                    {
                        PurchaseRequisitionID = id,
                        Status = PurchaseRequisitionBudgetValidation.Status,
                        Remark = PurchaseRequisitionBudgetValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = PurchaseRequisitionBudgetValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = PurchaseRequisitionBudgetValidation.ModifiedBy
                    };

                    _db.PurchaseRequisitionBudgetValidations.Add(entity);
                    _db.SaveChanges();
                    PurchaseRequisitionBudgetValidation.PurchaseRequisitionBudgetValidationId = entity.PurchaseRequisitionBudgetValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Purchase Requisition budget approval", "Success", User.Identity.Name, $"approved request {entity.PurchaseRequisitionID} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { PurchaseRequisitionBudgetValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPaymentApproval)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionBudgetValidations_Update(int id, [DataSourceRequest] DataSourceRequest request, PurchaseRequisitionBudgetValidation PurchaseRequisitionBudgetValidation)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region ger parent record status
                var record = _db.PurchaseRequisitions.Find(id);

                if (record != null && record.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record,it is already approved";
                    //return View("Error");
                }
                else if (record != null && !record.IsOnlineApproved)
                {
                    //validate 
                    if (PurchaseRequisitionBudgetValidation.Status == ApprovalStatuses.Approve)
                    {

                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                    }
                    if (PurchaseRequisitionBudgetValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
                #endregion

                if (isvalid)
                {
                    var entity = new PurchaseRequisitionBudgetValidation
                    {
                        PurchaseRequisitionID = id,
                        PurchaseRequisitionBudgetValidationId = PurchaseRequisitionBudgetValidation.PurchaseRequisitionBudgetValidationId,
                        Status = PurchaseRequisitionBudgetValidation.Status,
                        Remark = PurchaseRequisitionBudgetValidation.Remark,
                        DateCreated = PurchaseRequisitionBudgetValidation.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = PurchaseRequisitionBudgetValidation.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.PurchaseRequisitionBudgetValidations.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            return Json(new[] { PurchaseRequisitionBudgetValidation }.ToDataSourceResult(request, ModelState));
        }



        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
