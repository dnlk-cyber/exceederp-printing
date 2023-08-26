using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.Travel
{
    public class ApTravelExpenseValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableTravelExpenseClaim, FinanceConstants.AccountPayableTravelExpenseClaimApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableTravelExpenseClaim, FinanceConstants.AccountPayableTravelExpenseClaimApprover)]
        public ActionResult ApTravelExpenseValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelExpenseValidation> aptravelexpensevalidations = _db.ApTravelExpenseValidations.Where(x => x.ApTravelExpenseClaimId == id); ;
            DataSourceResult result = aptravelexpensevalidations.ToDataSourceResult(request, apTravelExpenseValidation => new {
                ApTravelExpenseValidationId = apTravelExpenseValidation.ApTravelExpenseValidationId,
                Status = apTravelExpenseValidation.Status,
                Remark = apTravelExpenseValidation.Remark,
                OrgBranchName = apTravelExpenseValidation.OrgBranchName,
                DateCreated = apTravelExpenseValidation.DateCreated,
                LastModified = apTravelExpenseValidation.LastModified,
                CreatedBy = apTravelExpenseValidation.CreatedBy,
                ModifiedBy = apTravelExpenseValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelExpenseClaimApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApTravelExpenseValidation apTravelExpenseValidation)
        {
            var record = _db.ApTravelExpenseClaims.Find(id);
            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

          
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (apTravelExpenseValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                    }

                    if (apTravelExpenseValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Finance Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new ApTravelExpenseValidation
                    {
                        ApTravelExpenseClaimId = id,
                        Status = apTravelExpenseValidation.Status,
                        Remark = apTravelExpenseValidation.Remark,
                        OrgBranchName = apTravelExpenseValidation.OrgBranchName,
                        DateCreated = apTravelExpenseValidation.DateCreated,
                        LastModified = apTravelExpenseValidation.LastModified,
                        CreatedBy = apTravelExpenseValidation.CreatedBy,
                        ModifiedBy = apTravelExpenseValidation.ModifiedBy
                    };

                    _db.ApTravelExpenseValidations.Add(entity);
                    _db.SaveChanges();
                    apTravelExpenseValidation.ApTravelExpenseValidationId = entity.ApTravelExpenseValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP travel expense claim approval", "Success", User.Identity.Name, $"approved travel expense claim {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { apTravelExpenseValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelExpenseClaimApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseValidations_Update([DataSourceRequest]DataSourceRequest request, ApTravelExpenseValidation apTravelExpenseValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseValidation
                {
                    ApTravelExpenseValidationId = apTravelExpenseValidation.ApTravelExpenseValidationId,
                    Status = apTravelExpenseValidation.Status,
                    Remark = apTravelExpenseValidation.Remark,
                    OrgBranchName = apTravelExpenseValidation.OrgBranchName,
                    DateCreated = apTravelExpenseValidation.DateCreated,
                    LastModified = apTravelExpenseValidation.LastModified,
                    CreatedBy = apTravelExpenseValidation.CreatedBy,
                    ModifiedBy = apTravelExpenseValidation.ModifiedBy
                };

                _db.ApTravelExpenseValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { apTravelExpenseValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelExpenseClaimApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseValidations_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelExpenseValidation apTravelExpenseValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseValidation
                {
                    ApTravelExpenseValidationId = apTravelExpenseValidation.ApTravelExpenseValidationId,
                    Status = apTravelExpenseValidation.Status,
                    Remark = apTravelExpenseValidation.Remark,
                    OrgBranchName = apTravelExpenseValidation.OrgBranchName,
                    DateCreated = apTravelExpenseValidation.DateCreated,
                    LastModified = apTravelExpenseValidation.LastModified,
                    CreatedBy = apTravelExpenseValidation.CreatedBy,
                    ModifiedBy = apTravelExpenseValidation.ModifiedBy
                };

                _db.ApTravelExpenseValidations.Attach(entity);
                _db.ApTravelExpenseValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { apTravelExpenseValidation }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
