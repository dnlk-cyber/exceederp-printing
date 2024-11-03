using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class LeavePlanDeparmentalValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
       
        public ActionResult LeavePlanDeparmentalValidation_Read(int gId, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LeavePlanValidation> requestApprovals = _db.LeavePlanValidation.Where(x=>x.LeavePlanId == gId);
            DataSourceResult result = requestApprovals.ToDataSourceResult(request, leavePlanValidation => new {
                LeavePlanValidationId = leavePlanValidation.LeavePlanValidationId,
                LeavePlanId = leavePlanValidation.LeavePlanId,
                Status = leavePlanValidation.Status,
                Remark = leavePlanValidation.Remark,
                OrgBranchName = leavePlanValidation.OrgBranchName,
                DateCreated = leavePlanValidation.DateCreated,
                LastModified = leavePlanValidation.LastModified,
                CreatedBy = leavePlanValidation.CreatedBy,
                ModifiedBy = leavePlanValidation.ModifiedBy
            });

            return Json(result);
        }

       // [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePlanDeparmentalValidation_Create(int gId, [DataSourceRequest]DataSourceRequest request, LeavePlanValidation requestApproval)
        {
            
            var record = _db.LeavePlans.Find(gId);
            if (record != null && record.IsDepartmentOnlineApproved)
            {
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");
                return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));

            }

            requestApproval.LeavePlanId = gId;
            requestApproval.DateCreated = DateTime.Now;
            requestApproval.CreatedBy = User.Identity.Name;

            if (ModelState.IsValid)
            {

                if (record != null)
                {
                    if (requestApproval.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Approved";
                        record.IsVoid = false;
                        record.IsDepartmentOnlineApproved = true;
                        record.DepartmentOnlineApprovedBy = User.Identity.Name;
                        record.DepartmentOnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                    }

                    if (requestApproval.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = true;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    if (requestApproval.Status == ApprovalStatuses.Pending)
                    {
                        record.IsSendForApproval = true;
                        record.Status = "Pending";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    var entity = GetMappedObject(requestApproval);
                    _db.LeavePlanValidation.Add(entity);
                    _db.SaveChanges();
                    requestApproval.LeavePlanValidationId = entity.LeavePlanValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Leav Plan approval", "Success", User.Identity.Name, $"approved plan {entity.LeavePlanId} ", Modules.HRM);
                    #endregion
                }
            }

            return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
        }

       // [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePlanDeparmentalValidation_Update([DataSourceRequest]DataSourceRequest request, LeavePlanValidation requestApproval)
        {
            if (ModelState.IsValid)
            {
                requestApproval.LastModified = DateTime.Now;
                requestApproval.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(requestApproval);
                _db.LeavePlanValidation.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
        }

       // [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePlanDeparmentalValidation_Destroy([DataSourceRequest]DataSourceRequest request, LeavePlanValidation requestApproval)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(requestApproval);
                _db.LeavePlanValidation.Attach(entity);
                _db.LeavePlanValidation.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
        }

       private LeavePlanValidation GetMappedObject(LeavePlanValidation leavePlanValidation)
        {
            var entity = new LeavePlanValidation
            {
                LeavePlanValidationId = leavePlanValidation.LeavePlanValidationId,
                LeavePlanId = leavePlanValidation.LeavePlanId,
                Status = leavePlanValidation.Status,
                Remark = leavePlanValidation.Remark,
                OrgBranchName = leavePlanValidation.OrgBranchName,
                DateCreated = leavePlanValidation.DateCreated,
                LastModified = leavePlanValidation.LastModified,
                CreatedBy = leavePlanValidation.CreatedBy,
                ModifiedBy = leavePlanValidation.ModifiedBy
            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
