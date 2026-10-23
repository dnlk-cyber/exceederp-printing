using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
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
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class BudgetPlanDepartmentValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
        public ActionResult BudgetPlanDepartmentApprovals_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetPlanDepartmentApproval> BudgetPlanDepartmentApprovals = _db.BudgetPlanDepartmentApprovals.Where(x=>x.BudgetPlanId == id);
            DataSourceResult result = BudgetPlanDepartmentApprovals.ToDataSourceResult(request, BudgetPlanDepartmentApproval => new {
                BudgetPlanDepartmentApprovalId = BudgetPlanDepartmentApproval.BudgetPlanDepartmentApprovalId,
                Status = BudgetPlanDepartmentApproval.Status,
                Remark = BudgetPlanDepartmentApproval.Remark,
                OrgBranchName = BudgetPlanDepartmentApproval.OrgBranchName,
                DateCreated = BudgetPlanDepartmentApproval.DateCreated,
                LastModified = BudgetPlanDepartmentApproval.LastModified,
                CreatedBy = BudgetPlanDepartmentApproval.CreatedBy,
                ModifiedBy = BudgetPlanDepartmentApproval.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanDepartmentApprovals_Create(int id,[DataSourceRequest]DataSourceRequest request, BudgetPlanDepartmentApproval BudgetPlanDepartmentApproval)
        {
            
            var record = _db.PlanCategorys.Find(id);
            if (record != null && record.IsDepartmentOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            
            if (ModelState.IsValid)
            {

                if (record != null)
                {
                    if (BudgetPlanDepartmentApproval.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Department Approved";
                        record.IsVoid = false;
                        record.IsDepartmentOnlineApproved = true;
                        record.DepartmentOnlineApprovedBy = User.Identity.Name;
                        record.DepartmentOnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                    }

                    if (BudgetPlanDepartmentApproval.Status == ApprovalStatuses.Reject)
                    {
                        record.IsDepartmentSendForApproval = false;
                        record.Status = "Department Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new BudgetPlanDepartmentApproval
                    {
                        BudgetPlanId = id,
                        Status = BudgetPlanDepartmentApproval.Status,
                        Remark = BudgetPlanDepartmentApproval.Remark,
                        OrgBranchName = BudgetPlanDepartmentApproval.OrgBranchName,
                        DateCreated = BudgetPlanDepartmentApproval.DateCreated,
                        LastModified = BudgetPlanDepartmentApproval.LastModified,
                        CreatedBy = BudgetPlanDepartmentApproval.CreatedBy,
                        ModifiedBy = BudgetPlanDepartmentApproval.ModifiedBy
                    };

                    _db.BudgetPlanDepartmentApprovals.Add(entity);
                    _db.SaveChanges();
                    BudgetPlanDepartmentApproval.BudgetPlanDepartmentApprovalId = entity.BudgetPlanDepartmentApprovalId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Budget plan approval", "Success", User.Identity.Name, $"approved plan {entity.BudgetPlanId} ", Modules.Budget);
                    #endregion
                }
            }

            return Json(new[] { BudgetPlanDepartmentApproval }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanDepartmentApprovals_Update([DataSourceRequest]DataSourceRequest request, BudgetPlanDepartmentApproval BudgetPlanDepartmentApproval)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetPlanDepartmentApproval
                {
                    BudgetPlanDepartmentApprovalId = BudgetPlanDepartmentApproval.BudgetPlanDepartmentApprovalId,
                    Status = BudgetPlanDepartmentApproval.Status,
                    Remark = BudgetPlanDepartmentApproval.Remark,
                    OrgBranchName = BudgetPlanDepartmentApproval.OrgBranchName,
                    DateCreated = BudgetPlanDepartmentApproval.DateCreated,
                    LastModified = BudgetPlanDepartmentApproval.LastModified,
                    CreatedBy = BudgetPlanDepartmentApproval.CreatedBy,
                    ModifiedBy = BudgetPlanDepartmentApproval.ModifiedBy
                };

                _db.BudgetPlanDepartmentApprovals.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { BudgetPlanDepartmentApproval }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanDepartmentApprovals_Destroy([DataSourceRequest]DataSourceRequest request, BudgetPlanDepartmentApproval BudgetPlanDepartmentApproval)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetPlanDepartmentApproval
                {
                    BudgetPlanDepartmentApprovalId = BudgetPlanDepartmentApproval.BudgetPlanDepartmentApprovalId,
                    Status = BudgetPlanDepartmentApproval.Status,
                    Remark = BudgetPlanDepartmentApproval.Remark,
                    OrgBranchName = BudgetPlanDepartmentApproval.OrgBranchName,
                    DateCreated = BudgetPlanDepartmentApproval.DateCreated,
                    LastModified = BudgetPlanDepartmentApproval.LastModified,
                    CreatedBy = BudgetPlanDepartmentApproval.CreatedBy,
                    ModifiedBy = BudgetPlanDepartmentApproval.ModifiedBy
                };

                _db.BudgetPlanDepartmentApprovals.Attach(entity);
                _db.BudgetPlanDepartmentApprovals.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { BudgetPlanDepartmentApproval }.ToDataSourceResult(request, ModelState));
        }

       
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
