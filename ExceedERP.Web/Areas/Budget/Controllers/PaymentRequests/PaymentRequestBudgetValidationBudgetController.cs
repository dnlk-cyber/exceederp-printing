using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
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
    public class PaymentRequestBudgetValidationBudgetController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult PaymentRequestBudgetValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PaymentRequestBudgetValidation> PaymentRequestBudgetValidations = _db.PaymentRequestBudgetValidations.Where(x=>x.PaymentRequestId==id);
            DataSourceResult result = PaymentRequestBudgetValidations.ToDataSourceResult(request, PaymentRequestBudgetValidation => new {
                PaymentRequestBudgetValidationId = PaymentRequestBudgetValidation.PaymentRequestBudgetValidationId,
                PaymentRequestId=id,
                Status = PaymentRequestBudgetValidation.Status,
                Remark = PaymentRequestBudgetValidation.Remark,
                OrgBranchName = PaymentRequestBudgetValidation.OrgBranchName,
                DateCreated = PaymentRequestBudgetValidation.DateCreated,
                LastModified = PaymentRequestBudgetValidation.LastModified,
                CreatedBy = PaymentRequestBudgetValidation.CreatedBy,
                ModifiedBy = PaymentRequestBudgetValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(BudgetRoles.BudgetPaymentApproval)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequestBudgetValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, PaymentRequestBudgetValidation PaymentRequestBudgetValidation)
        {
            var record = _db.PaymentRequests.Find(id);
            if (record != null && record.IsBudgetApproved)
                ModelState.AddModelError("approved","You cannot manipulate this record,it is already approved");

            if (record != null && !record.IsDepartmentOnlineApproved)
                ModelState.AddModelError("notdepapproved", "The record should be approved by requestor department first.");

            var lines = _db.PaymentRequestLines.Where(x => x.PaymentRequestId == id);
            if (!lines.Any())
                ModelState.AddModelError("lines", "Please add line items");
          
          

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (PaymentRequestBudgetValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Budget Approved";
                        record.IsBudgetApproved = true;
                        record.BudgetApprovedBy = User.Identity.Name;
                        record.BudgetApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                      
                    }

                    if (PaymentRequestBudgetValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsBudgetApproved = false;
                        record.Status = "Budget Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new PaymentRequestBudgetValidation
                    {
                        PaymentRequestId = id,
                        Status = PaymentRequestBudgetValidation.Status,
                        Remark = PaymentRequestBudgetValidation.Remark,
                        OrgBranchName = PaymentRequestBudgetValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = PaymentRequestBudgetValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = PaymentRequestBudgetValidation.ModifiedBy
                    };

                    _db.PaymentRequestBudgetValidations.Add(entity);
                    _db.SaveChanges();
                    PaymentRequestBudgetValidation.PaymentRequestBudgetValidationId = entity.PaymentRequestBudgetValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request budget approval", "Success", User.Identity.Name, $"approved request {entity.PaymentRequestId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { PaymentRequestBudgetValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPaymentApproval)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequestBudgetValidations_Update(int id,[DataSourceRequest]DataSourceRequest request, PaymentRequestBudgetValidation PaymentRequestBudgetValidation)
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
                    if (PaymentRequestBudgetValidation.Status ==ApprovalStatuses.Approve)
                    {
                       
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                    }
                    if (PaymentRequestBudgetValidation.Status ==ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
                #endregion

                if (isvalid)
                {
                    var entity = new PaymentRequestBudgetValidation
                    {
                        PaymentRequestId = id,
                        PaymentRequestBudgetValidationId = PaymentRequestBudgetValidation.PaymentRequestBudgetValidationId,
                        Status = PaymentRequestBudgetValidation.Status,
                        Remark = PaymentRequestBudgetValidation.Remark,
                        OrgBranchName = PaymentRequestBudgetValidation.OrgBranchName,
                        DateCreated = PaymentRequestBudgetValidation.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = PaymentRequestBudgetValidation.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.PaymentRequestBudgetValidations.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            return Json(new[] { PaymentRequestBudgetValidation }.ToDataSourceResult(request, ModelState));
        }

        

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
