using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    public class BudgetPlanRevisionValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [AuthorizeRoles(BudgetRoles.BudgetUser, BudgetRoles.BudgetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(BudgetRoles.BudgetUser, BudgetRoles.BudgetApprover)]
        public ActionResult BudgetPlanRevisionApprovals_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetPlanRevisionApproval> BudgetPlanRevisionApprovals = _db.BudgetPlanRevisionApprovals.Where(x=>x.BudgetPlanId == id);
            DataSourceResult result = BudgetPlanRevisionApprovals.ToDataSourceResult(request, budgetPlanRevisionApproval => new {
                BudgetPlanRevisionApprovalId = budgetPlanRevisionApproval.BudgetPlanRevisionApprovalId,
                Status = budgetPlanRevisionApproval.Status,
                Remark = budgetPlanRevisionApproval.Remark,
                OrgBranchName = budgetPlanRevisionApproval.OrgBranchName,
                DateCreated = budgetPlanRevisionApproval.DateCreated,
                LastModified = budgetPlanRevisionApproval.LastModified,
                CreatedBy = budgetPlanRevisionApproval.CreatedBy,
                ModifiedBy = budgetPlanRevisionApproval.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanRevisionApprovals_Create(int id,[DataSourceRequest]DataSourceRequest request, BudgetPlanRevisionApproval budgetPlanRevisionApproval)
        {
            
            var record = _db.PlanCategorys.Find(id);
            if (record != null && record.IsRevisionOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            
            if (ModelState.IsValid)
            {

                if (record != null)
                {
                    if (budgetPlanRevisionApproval.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Revision Approved";
                        record.IsOpenforrevision = false;
                        record.IsRevisionOnlineApproved = true;
                        record.RevisionOnlineApprovedBy = User.Identity.Name;
                        record.RevisionSendForApprovalTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                    }

                    if (budgetPlanRevisionApproval.Status == ApprovalStatuses.Reject)
                    {
                        record.IsRevisionSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new BudgetPlanRevisionApproval
                    {
                        BudgetPlanId = id,
                        Status = budgetPlanRevisionApproval.Status,
                        Remark = budgetPlanRevisionApproval.Remark,
                        OrgBranchName = budgetPlanRevisionApproval.OrgBranchName,
                        DateCreated = budgetPlanRevisionApproval.DateCreated,
                        LastModified = budgetPlanRevisionApproval.LastModified,
                        CreatedBy = budgetPlanRevisionApproval.CreatedBy,
                        ModifiedBy = budgetPlanRevisionApproval.ModifiedBy
                    };

                    _db.BudgetPlanRevisionApprovals.Add(entity);
                    _db.SaveChanges();
                    budgetPlanRevisionApproval.BudgetPlanRevisionApprovalId = entity.BudgetPlanRevisionApprovalId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Budget plan revision approval", "Success", User.Identity.Name, $"approved plan {entity.BudgetPlanId} ", Modules.Budget);
                    #endregion
                }
            }

            return Json(new[] { budgetPlanRevisionApproval }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanRevisionApprovals_Update([DataSourceRequest]DataSourceRequest request, BudgetPlanRevisionApproval BudgetPlanRevisionApproval)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetPlanRevisionApproval
                {
                    BudgetPlanRevisionApprovalId = BudgetPlanRevisionApproval.BudgetPlanRevisionApprovalId,
                    Status = BudgetPlanRevisionApproval.Status,
                    Remark = BudgetPlanRevisionApproval.Remark,
                    OrgBranchName = BudgetPlanRevisionApproval.OrgBranchName,
                    DateCreated = BudgetPlanRevisionApproval.DateCreated,
                    LastModified = BudgetPlanRevisionApproval.LastModified,
                    CreatedBy = BudgetPlanRevisionApproval.CreatedBy,
                    ModifiedBy = BudgetPlanRevisionApproval.ModifiedBy
                };

                _db.BudgetPlanRevisionApprovals.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { BudgetPlanRevisionApproval }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanRevisionApprovals_Destroy([DataSourceRequest]DataSourceRequest request, BudgetPlanRevisionApproval BudgetPlanRevisionApproval)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetPlanRevisionApproval
                {
                    BudgetPlanRevisionApprovalId = BudgetPlanRevisionApproval.BudgetPlanRevisionApprovalId,
                    Status = BudgetPlanRevisionApproval.Status,
                    Remark = BudgetPlanRevisionApproval.Remark,
                    OrgBranchName = BudgetPlanRevisionApproval.OrgBranchName,
                    DateCreated = BudgetPlanRevisionApproval.DateCreated,
                    LastModified = BudgetPlanRevisionApproval.LastModified,
                    CreatedBy = BudgetPlanRevisionApproval.CreatedBy,
                    ModifiedBy = BudgetPlanRevisionApproval.ModifiedBy
                };

                _db.BudgetPlanRevisionApprovals.Attach(entity);
                _db.BudgetPlanRevisionApprovals.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { BudgetPlanRevisionApproval }.ToDataSourceResult(request, ModelState));
        }

       
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
