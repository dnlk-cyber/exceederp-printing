using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.PaymentRequests
{
    public class PurchasePaymentBudgetValidationBudgetController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult PurchasePaymentBudgetValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PurchasePaymentBudgetValidation> PurchasePaymentBudgetValidations = _db.PurchasePaymentBudgetValidations.Where(x=>x.PurchasePaymentId==id);
            DataSourceResult result = PurchasePaymentBudgetValidations.ToDataSourceResult(request, PurchasePaymentBudgetValidation => new {
                PurchasePaymentBudgetValidationId = PurchasePaymentBudgetValidation.PurchasePaymentBudgetValidationId,
                PurchasePaymentId=id,
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

        [AuthorizeRoles(BudgetRoles.BudgetPaymentApproval)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentBudgetValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, PurchasePaymentBudgetValidation PurchasePaymentBudgetValidation)
        {
            var record = _db.PurchasePayments.Find(id);
            if (record != null && record.IsBudgetApproved)
                ModelState.AddModelError("approved","You cannot manipulate this record,it is already approved");

            if (record != null && !record.IsOnlineApproved)
                ModelState.AddModelError("notdepapproved", "The record should be approved by requestor department first.");
                   
          
          

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (PurchasePaymentBudgetValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Budget Approved";
                        record.IsBudgetApproved = true;
                        record.BudgetApprovedBy = User.Identity.Name;
                        record.BudgetApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                      
                    }

                    if (PurchasePaymentBudgetValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsBudgetApproved = false;
                        record.Status = "Budget Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new PurchasePaymentBudgetValidation
                    {
                        PurchasePaymentId = id,
                        Status = PurchasePaymentBudgetValidation.Status,
                        Remark = PurchasePaymentBudgetValidation.Remark,
                        OrgBranchName = PurchasePaymentBudgetValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = PurchasePaymentBudgetValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = PurchasePaymentBudgetValidation.ModifiedBy
                    };

                    _db.PurchasePaymentBudgetValidations.Add(entity);
                    _db.SaveChanges();
                    PurchasePaymentBudgetValidation.PurchasePaymentBudgetValidationId = entity.PurchasePaymentBudgetValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request budget approval", "Success", User.Identity.Name, $"approved request {entity.PurchasePaymentId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { PurchasePaymentBudgetValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPaymentApproval)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentBudgetValidations_Update(int id,[DataSourceRequest]DataSourceRequest request, PurchasePaymentBudgetValidation PurchasePaymentBudgetValidation)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region ger parent record status
                var record = _db.PaymentRequests.Find(id);

                if (record != null && record.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record,it is already approved";
                    //return View("Error");
                }
                else if (record != null && !record.IsOnlineApproved)
                {
                    //validate 
                    if (PurchasePaymentBudgetValidation.Status ==ApprovalStatuses.Approve)
                    {
                       
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                    }
                    if (PurchasePaymentBudgetValidation.Status ==ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
                #endregion

                if (isvalid)
                {
                    var entity = new PurchasePaymentBudgetValidation
                    {
                        PurchasePaymentId = id,
                        PurchasePaymentBudgetValidationId = PurchasePaymentBudgetValidation.PurchasePaymentBudgetValidationId,
                        Status = PurchasePaymentBudgetValidation.Status,
                        Remark = PurchasePaymentBudgetValidation.Remark,
                        OrgBranchName = PurchasePaymentBudgetValidation.OrgBranchName,
                        DateCreated = PurchasePaymentBudgetValidation.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = PurchasePaymentBudgetValidation.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.PurchasePaymentBudgetValidations.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                }
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
