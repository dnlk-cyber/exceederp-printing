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
    public class AccidentCreditServiceDepartmentValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
       

        public ActionResult Index()
        {
            return View();
        }
        //[AuthorizeRoles(BudgetRoles.BudgetUser, BudgetRoles.BudgetApprover)]
        public ActionResult AccidentCreditServiceDepartmentValidation_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AccidentCreditServiceDepartmentApproval> requestApprovals = _db.AccidentCreditServiceDepartmentApprovals.Where(x=>x.EmployeeAccidentCreditServiceId == id);
            DataSourceResult result = requestApprovals.ToDataSourceResult(request, requestApproval => new {
                AccidentCreditServiceDepartmentApprovalId = requestApproval.AccidentCreditServiceDepartmentApprovalId,
                EmployeeAccidentCreditServiceId = requestApproval.EmployeeAccidentCreditServiceId,
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
        public ActionResult AccidentCreditServiceDepartmentValidation_Create(int id,[DataSourceRequest]DataSourceRequest request, AccidentCreditServiceDepartmentApproval requestApproval)
        {
            
            var record = _db.EmployeeAccidentCreditServices.Find(id);
            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            }

            requestApproval.EmployeeAccidentCreditServiceId = id;
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
                    _db.AccidentCreditServiceDepartmentApprovals.Add(entity);
                    _db.SaveChanges();
                    requestApproval.AccidentCreditServiceDepartmentApprovalId = entity.AccidentCreditServiceDepartmentApprovalId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Accident Credit Service Department Approval", "Success", User.Identity.Name, $"Approved plan {entity.AccidentCreditServiceDepartmentApprovalId} ", Modules.Insurance);
                    #endregion
                }
            }

            return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccidentCreditServiceDepartmentValidation_Update([DataSourceRequest]DataSourceRequest request, AccidentCreditServiceDepartmentApproval requestApproval)
        {
            if (ModelState.IsValid)
            {
                requestApproval.LastModified = DateTime.Now;
                requestApproval.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(requestApproval);
                _db.AccidentCreditServiceDepartmentApprovals.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
        }

       // [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccidentCreditServiceDepartmentValidation_Destroy([DataSourceRequest]DataSourceRequest request, AccidentCreditServiceDepartmentApproval requestApproval)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(requestApproval);
                _db.AccidentCreditServiceDepartmentApprovals.Attach(entity);
                _db.AccidentCreditServiceDepartmentApprovals.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
        }

       private AccidentCreditServiceDepartmentApproval GetMappedObject(AccidentCreditServiceDepartmentApproval requestApproval)
        {
            var entity = new AccidentCreditServiceDepartmentApproval
            {
                AccidentCreditServiceDepartmentApprovalId = requestApproval.AccidentCreditServiceDepartmentApprovalId,
                EmployeeAccidentCreditServiceId = requestApproval.EmployeeAccidentCreditServiceId,
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
