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
	public class AccidentCreditServiceValidationController : BaseController
	{
		private ExceedDbContext _db = new ExceedDbContext();
		public ActionResult AccidentCreditServiceValidation_Read(int id,[DataSourceRequest]DataSourceRequest request)
		{
			IQueryable<AccidentInsuranceClaimApproval> requestApprovals = _db.AccidentInsuranceClaimApprovals.Where(x=>x.EmployeeAccidentInsuranceClaimId==id);
			DataSourceResult result = requestApprovals.ToDataSourceResult(request, requestApproval => new {
                AccidentInsuranceClaimApprovalId = requestApproval.AccidentInsuranceClaimApprovalId,
                EmployeeAccidentInsuranceClaimId = requestApproval.EmployeeAccidentInsuranceClaimId,
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
		public ActionResult AccidentCreditServiceValidation_Create(int id,[DataSourceRequest]DataSourceRequest request, AccidentCreditServiceApproval requestApproval)
		{
			
			var record = _db.EmployeeAccidentInsuranceClaims.Find(id);
			if (record != null && record.IsOnlineApproved)
				ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");
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

                    var entity = GetMappedObject(requestApproval);

					_db.AccidentCreditServiceApprovals.Add(entity);
					_db.SaveChanges();
					requestApproval.AccidentCreditServiceApprovalId = entity.AccidentCreditServiceApprovalId;
					#region operation log
					UserHelper.OperationLog(Request.UserHostAddress, "Accident Credit Service approval", "Success", User.Identity.Name, $"approved Accident Credit Service  approval {entity.AccidentCreditServiceApprovalId} ", Modules.Insurance);
					#endregion
				}
			}

			return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
		}

		//[AuthorizeRoles(FinanceConstants.AccountPayableFundTransferApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AccidentCreditServiceValidation_Update([DataSourceRequest]DataSourceRequest request, AccidentCreditServiceApproval requestApproval)
		{
			if (ModelState.IsValid)
			{
                requestApproval.LastModified = DateTime.Now;
                requestApproval.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(requestApproval);
				_db.AccidentCreditServiceApprovals.Attach(entity);
				_db.Entry(entity).State = EntityState.Modified;
				_db.SaveChanges();
			}

			return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
		}

		//[AuthorizeRoles(FinanceConstants.AccountPayableFundTransferApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AccidentCreditServiceValidation_Destroy([DataSourceRequest]DataSourceRequest request, AccidentCreditServiceApproval requestApproval)
		{
			if (ModelState.IsValid)
			{
                var entity = GetMappedObject(requestApproval);
				_db.AccidentCreditServiceApprovals.Attach(entity);
				_db.AccidentCreditServiceApprovals.Remove(entity);
				_db.SaveChanges();
			}

			return Json(new[] { requestApproval }.ToDataSourceResult(request, ModelState));
		}

        private AccidentCreditServiceApproval GetMappedObject(AccidentCreditServiceApproval requestApproval)
        {
            var entity = new AccidentCreditServiceApproval
            {
                AccidentCreditServiceApprovalId = requestApproval.AccidentCreditServiceApprovalId,
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
