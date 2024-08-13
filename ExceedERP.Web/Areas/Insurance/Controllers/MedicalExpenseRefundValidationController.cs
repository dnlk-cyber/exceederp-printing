using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Insurance.InsuranceNew;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
    [Authorize]
	public class MedicalExpenseRefundValidationController : BaseController
	{
		private ExceedDbContext _db = new ExceedDbContext();
		public ActionResult MedicalExpenseRefundValidation_Read(int id,[DataSourceRequest]DataSourceRequest request)
		{
			IQueryable<MedicalExpenseRefundApproval> requestApprovals = _db.MedicalExpenseRefundApprovals.Where(x=>x.EmployeeMedicalExpenseRefundId==id);
			DataSourceResult result = requestApprovals.ToDataSourceResult(request, requestApproval => new {
                MedicalExpenseRefundApprovalId = requestApproval.MedicalExpenseRefundApprovalId,
                EmployeeMedicalExpenseRefundId = requestApproval.EmployeeMedicalExpenseRefundId,
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

		//[AuthorizeRoles(FinanceConstants.AccountPayableFundTransferApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult MedicalExpenseRefundValidation_Create(int id,[DataSourceRequest]DataSourceRequest request, MedicalExpenseRefundApproval requestApproval)
		{
            requestApproval.AproverId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var record = _db.EmployeeMedicalExpenseRefunds.Find(id);
			if (record != null && record.IsOnlineApproved)
				ModelState.AddModelError("approved", "You cannot Manipulate this record, It is already approved");
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
						record.IsOnlineApproved = true;
						record.OnlineApprovedBy = User.Identity.Name;
						record.OnlineApprovedTime = DateTime.Now;
						_db.Entry(record).State = EntityState.Modified;
						_db.SaveChanges();

					}
					if (requestApproval.Status == ApprovalStatuses.Reject)
					{
						record.IsSendForApproval = false;
						record.Status = "Rejected";
						_db.Entry(record).State = EntityState.Modified;
						_db.SaveChanges();
					}

                    if (requestApproval.Status == ApprovalStatuses.Pending)
					{
						record.IsSendForApproval = false;
						record.Status = "Pending";
						_db.Entry(record).State = EntityState.Modified;
						_db.SaveChanges();
					}

                    var entity = GetMappedObject(requestApproval);
					_db.MedicalExpenseRefundApprovals.Add(entity);
					_db.SaveChanges();
					requestApproval.MedicalExpenseRefundApprovalId = entity.EmployeeMedicalExpenseRefundId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Medical Expense Refund  approval", "Success", User.Identity.Name, $"approved Medical Expense Refund  approval {entity.MedicalExpenseRefundApprovalId} ", Modules.Insurance);
					#endregion


				}
			}

			return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
		}

		//[AuthorizeRoles(FinanceConstants.AccountPayableFundTransferApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult MedicalExpenseRefundValidation_Update([DataSourceRequest]DataSourceRequest request, MedicalExpenseRefundApproval requestApproval)
		{
            requestApproval.AproverId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            if (ModelState.IsValid)
			{
                requestApproval.LastModified = DateTime.Now;
                requestApproval.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(requestApproval);
				_db.MedicalExpenseRefundApprovals.Attach(entity);
				_db.Entry(entity).State = EntityState.Modified;
				_db.SaveChanges();
			}

			return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
		}

		//[AuthorizeRoles(FinanceConstants.AccountPayableFundTransferApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult MedicalExpenseRefundValidation_Destroy([DataSourceRequest]DataSourceRequest request, MedicalExpenseRefundApproval requestApproval)
		{
			if (ModelState.IsValid)
			{
                var entity = GetMappedObject(requestApproval);
				_db.MedicalExpenseRefundApprovals.Attach(entity);
				_db.MedicalExpenseRefundApprovals.Remove(entity);
				_db.SaveChanges();
			}

			return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
		}

        private MedicalExpenseRefundApproval GetMappedObject(MedicalExpenseRefundApproval requestApproval)
        {
            var entity = new MedicalExpenseRefundApproval
            {
                MedicalExpenseRefundApprovalId = requestApproval.MedicalExpenseRefundApprovalId,
                EmployeeMedicalExpenseRefundId = requestApproval.EmployeeMedicalExpenseRefundId,
                AproverId = requestApproval.AproverId,
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
