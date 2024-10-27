using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP.FundTransfer;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.Travel
{
	public class ApFundReceiveValidationController : Controller
	{
		private ExceedDbContext _db = new ExceedDbContext();
		readonly AccountHelper _accountHelper = new AccountHelper();
		private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

		[AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableFundTransfer, FinanceConstants.AccountPayableFundTransferApprover)]
		public ActionResult Index()
		{
			return View();
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableFundTransfer, FinanceConstants.AccountPayableFundTransferApprover)]
		public ActionResult FundReceiveValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
		{
			IQueryable<FundReceiveValidation> FundReceiveValidations = _db.FundReceiveValidations.Where(x=>x.FundReceiveId==id);
			DataSourceResult result = FundReceiveValidations.ToDataSourceResult(request, FundReceiveValidation => new {
				FundReceiveValidationId = FundReceiveValidation.FundReceiveValidationId,
				Status = FundReceiveValidation.Status,
				Remark = FundReceiveValidation.Remark,
				OrgBranchName = FundReceiveValidation.OrgBranchName,
				DateCreated = FundReceiveValidation.DateCreated,
				LastModified = FundReceiveValidation.LastModified,
				CreatedBy = FundReceiveValidation.CreatedBy,
				ModifiedBy = FundReceiveValidation.ModifiedBy
			});

			return Json(result);
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableFundTransferApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FundReceiveValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, FundReceiveValidation FundReceiveValidation)
		{
			
			var record = _db.FundReceives.Find(id);
			if (record != null && record.IsOnlineApproved)
				ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

			if (ModelState.IsValid)
			{

				if (record != null)
				{
					if (FundReceiveValidation.Status == ApprovalStatuses.Approve)
					{
						record.Status = "Approved";
						record.IsOnlineApproved = true;
						record.OnlineApprovedBy = User.Identity.Name;
						record.OnlineApprovedTime = DateTime.Now;
						_db.Entry(record).State = EntityState.Modified;
						_db.SaveChanges();

					}

					if (FundReceiveValidation.Status == ApprovalStatuses.Reject)
					{
						record.IsSendForApproval = false;
						record.Status = "Finance Rejected";
						_db.Entry(record).State = EntityState.Modified;
						_db.SaveChanges();
					}

					var entity = new FundReceiveValidation
					{
						FundReceiveId = id,
						Status = FundReceiveValidation.Status,
						Remark = FundReceiveValidation.Remark,
						OrgBranchName = FundReceiveValidation.OrgBranchName,

						DateCreated = DateTime.Now,
						CreatedBy = User.Identity.Name,
						LastModified = null,
						ModifiedBy = null
					};

					_db.FundReceiveValidations.Add(entity);
					_db.SaveChanges();
					FundReceiveValidation.FundReceiveValidationId = entity.FundReceiveValidationId;
					#region operation log
					UserHelper.OperationLog(Request.UserHostAddress, "AP fund transfer receive approval", "Success", User.Identity.Name, $"approved fund receive {entity.FundReceiveId} ", Modules.Finance);
					#endregion
				}
			}

			return Json(new[] { FundReceiveValidation }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableFundTransferApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FundReceiveValidations_Update([DataSourceRequest]DataSourceRequest request, FundReceiveValidation FundReceiveValidation)
		{
			if (ModelState.IsValid)
			{
				var entity = new FundReceiveValidation
				{
					FundReceiveValidationId = FundReceiveValidation.FundReceiveValidationId,
					Status = FundReceiveValidation.Status,
					Remark = FundReceiveValidation.Remark,
					OrgBranchName = FundReceiveValidation.OrgBranchName,
					DateCreated = FundReceiveValidation.DateCreated,
					LastModified = FundReceiveValidation.LastModified,
					CreatedBy = FundReceiveValidation.CreatedBy,
					ModifiedBy = FundReceiveValidation.ModifiedBy
				};

				_db.FundReceiveValidations.Attach(entity);
				_db.Entry(entity).State = EntityState.Modified;
				_db.SaveChanges();
			}

			return Json(new[] { FundReceiveValidation }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableFundTransferApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FundReceiveValidations_Destroy([DataSourceRequest]DataSourceRequest request, FundReceiveValidation FundReceiveValidation)
		{
			if (ModelState.IsValid)
			{
				var entity = new FundReceiveValidation
				{
					FundReceiveValidationId = FundReceiveValidation.FundReceiveValidationId,
					Status = FundReceiveValidation.Status,
					Remark = FundReceiveValidation.Remark,
					OrgBranchName = FundReceiveValidation.OrgBranchName,
					DateCreated = FundReceiveValidation.DateCreated,
					LastModified = FundReceiveValidation.LastModified,
					CreatedBy = FundReceiveValidation.CreatedBy,
					ModifiedBy = FundReceiveValidation.ModifiedBy
				};

				_db.FundReceiveValidations.Attach(entity);
				_db.FundReceiveValidations.Remove(entity);
				_db.SaveChanges();
			}

			return Json(new[] { FundReceiveValidation }.ToDataSourceResult(request, ModelState));
		}

	   
		protected override void Dispose(bool disposing)
		{
			_db.Dispose();
			base.Dispose(disposing);
		}
	}
}
