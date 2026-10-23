using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.TravelRequests
{
    public class TravelExpenseBudgetValidationBudgetController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult ApTravelExpenseBudgetValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelExpenseBudgetValidation> ApTravelExpenseBudgetValidations = _db.ApTravelExpenseBudgetValidations.Where(x=>x.ApTravelExpenseClaimId==id);
            DataSourceResult result = ApTravelExpenseBudgetValidations.ToDataSourceResult(request, ApTravelExpenseBudgetValidation => new {
                ApTravelExpenseBudgetValidationId = ApTravelExpenseBudgetValidation.ApTravelExpenseBudgetValidationId,
                ApTravelExpenseClaimId=id,
                Status = ApTravelExpenseBudgetValidation.Status,
                Remark = ApTravelExpenseBudgetValidation.Remark,
                OrgBranchName = ApTravelExpenseBudgetValidation.OrgBranchName,
                DateCreated = ApTravelExpenseBudgetValidation.DateCreated,
                LastModified = ApTravelExpenseBudgetValidation.LastModified,
                CreatedBy = ApTravelExpenseBudgetValidation.CreatedBy,
                ModifiedBy = ApTravelExpenseBudgetValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(BudgetRoles.BudgetPaymentApproval)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseBudgetValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApTravelExpenseBudgetValidation ApTravelExpenseBudgetValidation)
        {
            var record = _db.ApTravelExpenseClaims.Find(id);
            if (record != null && record.IsBudgetApproved)
                ModelState.AddModelError("approved","You cannot manipulate this record,it is already approved");

            if (record != null && !record.IsDepartmentOnlineApproved)
                ModelState.AddModelError("notdepapproved", "The record should be approved by requestor department first.");

            var lines = _db.ApTravelExpenseClaims.Where(x => x.ApTravelExpenseClaimId == id);
            if (!lines.Any())
                ModelState.AddModelError("lines", "Please add departure lines");
          
          

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (ApTravelExpenseBudgetValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Budget Approved";
                        record.IsBudgetApproved = true;
                        record.BudgetApprovedBy = User.Identity.Name;
                        record.BudgetApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                      
                    }

                    if (ApTravelExpenseBudgetValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsBudgetApproved = false;
                        record.Status = "Budget Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new ApTravelExpenseBudgetValidation
                    {
                        ApTravelExpenseClaimId = id,
                        Status = ApTravelExpenseBudgetValidation.Status,
                        Remark = ApTravelExpenseBudgetValidation.Remark,
                        OrgBranchName = ApTravelExpenseBudgetValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = ApTravelExpenseBudgetValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = ApTravelExpenseBudgetValidation.ModifiedBy
                    };

                    _db.ApTravelExpenseBudgetValidations.Add(entity);
                    _db.SaveChanges();
                    ApTravelExpenseBudgetValidation.ApTravelExpenseBudgetValidationId = entity.ApTravelExpenseBudgetValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request budget approval", "Success", User.Identity.Name, $"approved request {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { ApTravelExpenseBudgetValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPaymentApproval)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseBudgetValidations_Update(int id,[DataSourceRequest]DataSourceRequest request, ApTravelExpenseBudgetValidation ApTravelExpenseBudgetValidation)
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
                    if (ApTravelExpenseBudgetValidation.Status ==ApprovalStatuses.Approve)
                    {
                       
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                    }
                    if (ApTravelExpenseBudgetValidation.Status ==ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
                #endregion

                if (isvalid)
                {
                    var entity = new ApTravelExpenseBudgetValidation
                    {
                        ApTravelExpenseClaimId = id,
                        ApTravelExpenseBudgetValidationId = ApTravelExpenseBudgetValidation.ApTravelExpenseBudgetValidationId,
                        Status = ApTravelExpenseBudgetValidation.Status,
                        Remark = ApTravelExpenseBudgetValidation.Remark,
                        OrgBranchName = ApTravelExpenseBudgetValidation.OrgBranchName,
                        DateCreated = ApTravelExpenseBudgetValidation.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = ApTravelExpenseBudgetValidation.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ApTravelExpenseBudgetValidations.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            return Json(new[] { ApTravelExpenseBudgetValidation }.ToDataSourceResult(request, ModelState));
        }

        

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
