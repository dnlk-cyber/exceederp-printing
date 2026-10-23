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
    public class BudgetPlanValidationController : Controller
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
        public ActionResult BudgetPlanApprovals_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetPlanApproval> BudgetPlanApprovals = _db.BudgetPlanApprovals.Where(x=>x.BudgetPlanId == id);
            DataSourceResult result = BudgetPlanApprovals.ToDataSourceResult(request, BudgetPlanApproval => new {
                BudgetPlanApprovalId = BudgetPlanApproval.BudgetPlanApprovalId,
                Status = BudgetPlanApproval.Status,
                Remark = BudgetPlanApproval.Remark,
                OrgBranchName = BudgetPlanApproval.OrgBranchName,
                DateCreated = BudgetPlanApproval.DateCreated,
                LastModified = BudgetPlanApproval.LastModified,
                CreatedBy = BudgetPlanApproval.CreatedBy,
                ModifiedBy = BudgetPlanApproval.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanApprovals_Create(int id,[DataSourceRequest]DataSourceRequest request, BudgetPlanApproval budgetPlanApproval)
        {
            
            var record = _db.PlanCategorys.Find(id);
            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            
            if (ModelState.IsValid)
            {

                if (record != null)
                {
                    if (budgetPlanApproval.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        record.CreatedBy = User.Identity.Name;
                        record.DateCreated = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                    }

                    if (budgetPlanApproval.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new BudgetPlanApproval
                    {
                        BudgetPlanId = id,
                        Status = budgetPlanApproval.Status,
                        Remark = budgetPlanApproval.Remark,
                        OrgBranchName = budgetPlanApproval.OrgBranchName,
                        DateCreated = budgetPlanApproval.DateCreated,
                        LastModified = budgetPlanApproval.LastModified,
                        CreatedBy = budgetPlanApproval.CreatedBy,
                        ModifiedBy = budgetPlanApproval.ModifiedBy
                    };

                    _db.BudgetPlanApprovals.Add(entity);
                    _db.SaveChanges();
                    budgetPlanApproval.BudgetPlanApprovalId = entity.BudgetPlanApprovalId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Budget plan approval", "Success", User.Identity.Name, $"approved plan {entity.BudgetPlanId} ", Modules.Budget);
                    #endregion
                }
            }

            return Json(new[] { budgetPlanApproval }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanApprovals_Update([DataSourceRequest]DataSourceRequest request, BudgetPlanApproval BudgetPlanApproval)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetPlanApproval
                {
                    BudgetPlanApprovalId = BudgetPlanApproval.BudgetPlanApprovalId,
                    Status = BudgetPlanApproval.Status,
                    Remark = BudgetPlanApproval.Remark,
                    OrgBranchName = BudgetPlanApproval.OrgBranchName,
                    DateCreated = BudgetPlanApproval.DateCreated,
                    LastModified = BudgetPlanApproval.LastModified,
                    CreatedBy = BudgetPlanApproval.CreatedBy,
                    ModifiedBy = BudgetPlanApproval.ModifiedBy
                };

                _db.BudgetPlanApprovals.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { BudgetPlanApproval }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanApprovals_Destroy([DataSourceRequest]DataSourceRequest request, BudgetPlanApproval BudgetPlanApproval)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetPlanApproval
                {
                    BudgetPlanApprovalId = BudgetPlanApproval.BudgetPlanApprovalId,
                    Status = BudgetPlanApproval.Status,
                    Remark = BudgetPlanApproval.Remark,
                    OrgBranchName = BudgetPlanApproval.OrgBranchName,
                    DateCreated = BudgetPlanApproval.DateCreated,
                    LastModified = BudgetPlanApproval.LastModified,
                    CreatedBy = BudgetPlanApproval.CreatedBy,
                    ModifiedBy = BudgetPlanApproval.ModifiedBy
                };

                _db.BudgetPlanApprovals.Attach(entity);
                _db.BudgetPlanApprovals.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { BudgetPlanApproval }.ToDataSourceResult(request, ModelState));
        }

       
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
