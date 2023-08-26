using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.AP.FundTransfer;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.Travel
{
	public class ApFundTransferValidationController : Controller
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
		public ActionResult FundTransferValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
		{
			IQueryable<FundTransferValidation> FundTransferValidations = _db.FundTransferValidations.Where(x=>x.FundTransferId==id);
			DataSourceResult result = FundTransferValidations.ToDataSourceResult(request, FundTransferValidation => new {
				FundTransferValidationId = FundTransferValidation.FundTransferValidationId,
				Status = FundTransferValidation.Status,
				Remark = FundTransferValidation.Remark,
				OrgBranchName = FundTransferValidation.OrgBranchName,
				DateCreated = FundTransferValidation.DateCreated,
				LastModified = FundTransferValidation.LastModified,
				CreatedBy = FundTransferValidation.CreatedBy,
				ModifiedBy = FundTransferValidation.ModifiedBy
			});

			return Json(result);
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableFundTransferApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FundTransferValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, FundTransferValidation FundTransferValidation)
		{
			
			var record = _db.FundTransfers.Find(id);
			if (record != null && record.IsOnlineApproved)
				ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

			if (ModelState.IsValid)
			{

				if (record != null)
				{
					if (FundTransferValidation.Status == ApprovalStatuses.Approve)
					{
						record.Status = "Approved";
						record.IsOnlineApproved = true;
						record.OnlineApprovedBy = User.Identity.Name;
						record.OnlineApprovedTime = DateTime.Now;
						_db.Entry(record).State = EntityState.Modified;
						_db.SaveChanges();

					}

					if (FundTransferValidation.Status == ApprovalStatuses.Reject)
					{
						record.IsSendForApproval = false;
						record.Status = "Finance Rejected";
						_db.Entry(record).State = EntityState.Modified;
						_db.SaveChanges();
					}

					var entity = new FundTransferValidation
					{
						FundTransferId = id,
						Status = FundTransferValidation.Status,
						Remark = FundTransferValidation.Remark,
						OrgBranchName = FundTransferValidation.OrgBranchName,

						DateCreated = DateTime.Now,
						CreatedBy = User.Identity.Name,
						LastModified = null,
						ModifiedBy = null
					};

					_db.FundTransferValidations.Add(entity);
					_db.SaveChanges();
					FundTransferValidation.FundTransferValidationId = entity.FundTransferValidationId;
					#region operation log
					UserHelper.OperationLog(Request.UserHostAddress, "AP fund transfer approval", "Success", User.Identity.Name, $"approved fund transfer {entity.FundTransferId} ", Modules.Finance);
					#endregion
				}
			}

			return Json(new[] { FundTransferValidation }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableFundTransferApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FundTransferValidations_Update([DataSourceRequest]DataSourceRequest request, FundTransferValidation FundTransferValidation)
		{
			if (ModelState.IsValid)
			{
				var entity = new FundTransferValidation
				{
					FundTransferValidationId = FundTransferValidation.FundTransferValidationId,
					Status = FundTransferValidation.Status,
					Remark = FundTransferValidation.Remark,
					OrgBranchName = FundTransferValidation.OrgBranchName,
					DateCreated = FundTransferValidation.DateCreated,
					LastModified = FundTransferValidation.LastModified,
					CreatedBy = FundTransferValidation.CreatedBy,
					ModifiedBy = FundTransferValidation.ModifiedBy
				};

				_db.FundTransferValidations.Attach(entity);
				_db.Entry(entity).State = EntityState.Modified;
				_db.SaveChanges();
			}

			return Json(new[] { FundTransferValidation }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableFundTransferApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FundTransferValidations_Destroy([DataSourceRequest]DataSourceRequest request, FundTransferValidation FundTransferValidation)
		{
			if (ModelState.IsValid)
			{
				var entity = new FundTransferValidation
				{
					FundTransferValidationId = FundTransferValidation.FundTransferValidationId,
					Status = FundTransferValidation.Status,
					Remark = FundTransferValidation.Remark,
					OrgBranchName = FundTransferValidation.OrgBranchName,
					DateCreated = FundTransferValidation.DateCreated,
					LastModified = FundTransferValidation.LastModified,
					CreatedBy = FundTransferValidation.CreatedBy,
					ModifiedBy = FundTransferValidation.ModifiedBy
				};

				_db.FundTransferValidations.Attach(entity);
				_db.FundTransferValidations.Remove(entity);
				_db.SaveChanges();
			}

			return Json(new[] { FundTransferValidation }.ToDataSourceResult(request, ModelState));
		}

	   
		protected override void Dispose(bool disposing)
		{
			_db.Dispose();
			base.Dispose(disposing);
		}
	}
}
