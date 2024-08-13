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
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
    [Authorize]
	public class PolicyRenewalValidationController : BaseController
	{
		private ExceedDbContext _db = new ExceedDbContext();
		public ActionResult PolicyRenewalValidation_Read(int id,[DataSourceRequest]DataSourceRequest request)
		{
			IQueryable<PolicyRenewalApproval> requestApprovals = _db.PolicyRenewalApprovals.Where(x=>x.InsurancePolicyRenewalId==id);
			DataSourceResult result = requestApprovals.ToDataSourceResult(request, requestApproval => new {
                PolicyRenewalApprovalId = requestApproval.PolicyRenewalApprovalId,
                InsurancePolicyRenewalId = requestApproval.InsurancePolicyRenewalId,
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
		public ActionResult PolicyRenewalValidation_Create(int id,[DataSourceRequest]DataSourceRequest request, PolicyRenewalApproval requestApproval)
		{
			
			var record = _db.InsurancePolicyRenewals.Find(id);
			if (record != null && record.IsOnlineApproved)
				ModelState.AddModelError("approved", "You can't manipulate this record,it is already Approved");
            requestApproval.InsurancePolicyRenewalId = id;
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

                    if (requestApproval.Status == ApprovalStatuses.Reject)
					{
						record.IsSendForApproval = false;
						record.Status = "Pending";
						_db.Entry(record).State = EntityState.Modified;
						_db.SaveChanges();
					}

                    var entity = GetMappedObject(requestApproval);

					_db.PolicyRenewalApprovals.Add(entity);
					_db.SaveChanges();
					requestApproval.PolicyRenewalApprovalId = entity.PolicyRenewalApprovalId;
					#region operation log
					UserHelper.OperationLog(Request.UserHostAddress, "Policy Renewal Approval approval", "Success", User.Identity.Name, $"approved Policy Renewal approval {entity.PolicyRenewalApprovalId} ", Modules.Insurance);
					#endregion
				}
			}

			return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
		}

		//[AuthorizeRoles(FinanceConstants.AccountPayableFundTransferApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult PolicyRenewalValidation_Update([DataSourceRequest]DataSourceRequest request, PolicyRenewalApproval requestApproval)
		{
			if (ModelState.IsValid)
			{
                requestApproval.LastModified = DateTime.Now;
                requestApproval.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(requestApproval);
				_db.PolicyRenewalApprovals.Attach(entity);
				_db.Entry(entity).State = EntityState.Modified;
				_db.SaveChanges();
			}

			return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
		}

		//[AuthorizeRoles(FinanceConstants.AccountPayableFundTransferApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult PolicyRenewalValidation_Destroy([DataSourceRequest]DataSourceRequest request, PolicyRenewalApproval requestApproval)
		{
			if (ModelState.IsValid)
			{
                var entity = GetMappedObject(requestApproval);
				_db.PolicyRenewalApprovals.Attach(entity);
				_db.PolicyRenewalApprovals.Remove(entity);
				_db.SaveChanges();
			}

			return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
		}

        private PolicyRenewalApproval GetMappedObject(PolicyRenewalApproval requestApproval)
        {
            var entity = new PolicyRenewalApproval
            {
                PolicyRenewalApprovalId = requestApproval.PolicyRenewalApprovalId,
                InsurancePolicyRenewalId = requestApproval.InsurancePolicyRenewalId,
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
