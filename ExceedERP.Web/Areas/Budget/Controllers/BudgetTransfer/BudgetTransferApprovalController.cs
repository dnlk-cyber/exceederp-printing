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
    public class BudgetTransferApprovalController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
    
        [AuthorizeRoles(BudgetRoles.BudgetUser, BudgetRoles.BudgetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(BudgetRoles.BudgetUser, BudgetRoles.BudgetApprover)]
        public ActionResult BudgetTransferApprovals_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetTransferApproval> BudgetTransferApprovals = _db.BudgetTransferApprovals.Where(x=>x.BudgetTransferID == id);
            DataSourceResult result = BudgetTransferApprovals.ToDataSourceResult(request, BudgetTransferApproval => new {
                BudgetTransferApprovalId = BudgetTransferApproval.BudgetTransferApprovalId,
                Status = BudgetTransferApproval.Status,
                Remark = BudgetTransferApproval.Remark,
                OrgBranchName = BudgetTransferApproval.OrgBranchName,
                DateCreated = BudgetTransferApproval.DateCreated,
                LastModified = BudgetTransferApproval.LastModified,
                CreatedBy = BudgetTransferApproval.CreatedBy,
                ModifiedBy = BudgetTransferApproval.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetTransferApprovals_Create(int id,[DataSourceRequest]DataSourceRequest request, BudgetTransferApproval BudgetTransferApproval)
        {
            
            var record = _db.BudgetTransfers.Find(id);
            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            
            if (ModelState.IsValid)
            {

                if (record != null)
                {
                    if (BudgetTransferApproval.Status == ApprovalStatuses.Approve)
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

                    if (BudgetTransferApproval.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new BudgetTransferApproval
                    {
                        BudgetTransferID = id,
                        Status = BudgetTransferApproval.Status,
                        Remark = BudgetTransferApproval.Remark,
                        OrgBranchName = BudgetTransferApproval.OrgBranchName,
                        DateCreated = BudgetTransferApproval.DateCreated,
                        LastModified = BudgetTransferApproval.LastModified,
                        CreatedBy = BudgetTransferApproval.CreatedBy,
                        ModifiedBy = BudgetTransferApproval.ModifiedBy
                    };

                    _db.BudgetTransferApprovals.Add(entity);
                    _db.SaveChanges();
                    BudgetTransferApproval.BudgetTransferApprovalId = entity.BudgetTransferApprovalId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Budget transfer approval", "Success", User.Identity.Name, $"approved plan {entity.BudgetTransferID} ", Modules.Budget);
                    #endregion
                }
            }

            return Json(new[] { BudgetTransferApproval }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetTransferApprovals_Update([DataSourceRequest]DataSourceRequest request, BudgetTransferApproval BudgetTransferApproval)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetTransferApproval
                {
                    BudgetTransferApprovalId = BudgetTransferApproval.BudgetTransferApprovalId,
                    Status = BudgetTransferApproval.Status,
                    Remark = BudgetTransferApproval.Remark,
                    OrgBranchName = BudgetTransferApproval.OrgBranchName,
                    DateCreated = BudgetTransferApproval.DateCreated,
                    LastModified = BudgetTransferApproval.LastModified,
                    CreatedBy = BudgetTransferApproval.CreatedBy,
                    ModifiedBy = BudgetTransferApproval.ModifiedBy
                };

                _db.BudgetTransferApprovals.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { BudgetTransferApproval }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetTransferApprovals_Destroy([DataSourceRequest]DataSourceRequest request, BudgetTransferApproval BudgetTransferApproval)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetTransferApproval
                {
                    BudgetTransferApprovalId = BudgetTransferApproval.BudgetTransferApprovalId,
                    Status = BudgetTransferApproval.Status,
                    Remark = BudgetTransferApproval.Remark,
                    OrgBranchName = BudgetTransferApproval.OrgBranchName,
                    DateCreated = BudgetTransferApproval.DateCreated,
                    LastModified = BudgetTransferApproval.LastModified,
                    CreatedBy = BudgetTransferApproval.CreatedBy,
                    ModifiedBy = BudgetTransferApproval.ModifiedBy
                };

                _db.BudgetTransferApprovals.Attach(entity);
                _db.BudgetTransferApprovals.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { BudgetTransferApproval }.ToDataSourceResult(request, ModelState));
        }

       
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
