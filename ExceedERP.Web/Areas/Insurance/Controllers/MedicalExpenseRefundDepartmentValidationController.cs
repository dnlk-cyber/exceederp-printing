using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Insurance.InsuranceNew;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class MedicalExpenseRefundDepartmentValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
       

        public ActionResult Index()
        {
            return View();
        }
        //[AuthorizeRoles(BudgetRoles.BudgetUser, BudgetRoles.BudgetApprover)]
        public ActionResult MedicalExpenseRefundDepartmentValidation_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MedicalExpenseRefundDepartmentApproval> requestApprovals = _db.MedicalExpenseRefundDepartmentApprovals.Where(x=>x.EmployeeMedicalExpenseRefundId == id);
            DataSourceResult result = requestApprovals.ToDataSourceResult(request, requestApproval => new {
                MedicalExpenseRefundDepartmentApprovalId = requestApproval.MedicalExpenseRefundDepartmentApprovalId,
                EmployeeMedicalExpenseRefundId = requestApproval.EmployeeMedicalExpenseRefundId,
                DepartmentAproverId = requestApproval.DepartmentAproverId,
                Status = requestApproval.Status,
                Remark = requestApproval.Remark,
                OrgBranchName = requestApproval.OrgBranchName,
                DateCreated = requestApproval.DateCreated,
                LastModified = requestApproval.LastModified,
                CreatedBy = requestApproval.CreatedBy,
                ModifiedBy = requestApproval.ModifiedBy
            });

            return Json(result);
        }

       // [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MedicalExpenseRefundDepartmentValidation_Create(int id,[DataSourceRequest]DataSourceRequest request, MedicalExpenseRefundDepartmentApproval requestApproval)
        {

            requestApproval.DepartmentAproverId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var record = _db.EmployeeMedicalExpenseRefunds.Find(id);
            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            }

            requestApproval.EmployeeMedicalExpenseRefundId = id;
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
                    _db.MedicalExpenseRefundDepartmentApprovals.Add(entity);
                    _db.SaveChanges();
                    requestApproval.MedicalExpenseRefundDepartmentApprovalId = entity.MedicalExpenseRefundDepartmentApprovalId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, ".Medical Expense Refund Department Approval", "Success", User.Identity.Name, $"approved plan {entity.MedicalExpenseRefundDepartmentApprovalId} ", Modules.Insurance);
                    #endregion
                }
            }

            return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MedicalExpenseRefundDepartmentValidation_Update([DataSourceRequest]DataSourceRequest request, MedicalExpenseRefundDepartmentApproval requestApproval)
        {
            requestApproval.DepartmentAproverId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            if (ModelState.IsValid)
            {
                requestApproval.LastModified = DateTime.Now;
                requestApproval.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(requestApproval);
                _db.MedicalExpenseRefundDepartmentApprovals.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
        }

       // [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MedicalExpenseRefundDepartmentValidation_Destroy([DataSourceRequest]DataSourceRequest request, MedicalExpenseRefundDepartmentApproval requestApproval)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(requestApproval);
                _db.MedicalExpenseRefundDepartmentApprovals.Attach(entity);
                _db.MedicalExpenseRefundDepartmentApprovals.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
        }

       private MedicalExpenseRefundDepartmentApproval GetMappedObject(MedicalExpenseRefundDepartmentApproval requestApproval)
        {
            var entity = new MedicalExpenseRefundDepartmentApproval
            {
                MedicalExpenseRefundDepartmentApprovalId = requestApproval.MedicalExpenseRefundDepartmentApprovalId,
                EmployeeMedicalExpenseRefundId = requestApproval.EmployeeMedicalExpenseRefundId,
                DepartmentAproverId = requestApproval.DepartmentAproverId,
                Status = requestApproval.Status,
                Remark = requestApproval.Remark,
                OrgBranchName = requestApproval.OrgBranchName,
                DateCreated = requestApproval.DateCreated,
                LastModified = requestApproval.LastModified,
                CreatedBy = requestApproval.CreatedBy,
                ModifiedBy = requestApproval.ModifiedBy
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
