using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
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
	public class ApTravelOrderValidationController : Controller
	{
		private ExceedDbContext _db = new ExceedDbContext();
		readonly AccountHelper _accountHelper = new AccountHelper();
		private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

		[AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableTravelOrder, FinanceConstants.AccountPayableTravelOrderApprover)]
		public ActionResult Index()
		{
			return View();
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableTravelOrder, FinanceConstants.AccountPayableTravelOrderApprover)]
		public ActionResult ApTravelOrderValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
		{
			IQueryable<ApTravelOrderValidation> aptravelordervalidations = _db.ApTravelOrderValidations.Where(x=>x.ApTravelOrderId==id);
			DataSourceResult result = aptravelordervalidations.ToDataSourceResult(request, apTravelOrderValidation => new {
				ApTravelOrderValidationId = apTravelOrderValidation.ApTravelOrderValidationId,
				Status = apTravelOrderValidation.Status,
				Remark = apTravelOrderValidation.Remark,
				OrgBranchName = apTravelOrderValidation.OrgBranchName,
				DateCreated = apTravelOrderValidation.DateCreated,
				LastModified = apTravelOrderValidation.LastModified,
				CreatedBy = apTravelOrderValidation.CreatedBy,
				ModifiedBy = apTravelOrderValidation.ModifiedBy
			});

			return Json(result);
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableTravelOrderApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ApTravelOrderValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApTravelOrderValidation apTravelOrderValidation)
		{
			
			var record = _db.ApTravelOrders.Find(id);
			if (record != null && record.IsOnlineApproved)
				ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

			if (ModelState.IsValid)
			{

				if (record != null)
				{
					if (apTravelOrderValidation.Status == ApprovalStatuses.Approve)
					{
						record.Status = "Approved";
						record.IsOnlineApproved = true;
						record.OnlineApprovedBy = User.Identity.Name;
						record.OnlineApprovedTime = DateTime.Now;
						_db.Entry(record).State = EntityState.Modified;
						_db.SaveChanges();

					}

					if (apTravelOrderValidation.Status == ApprovalStatuses.Reject)
					{
						record.IsSendForApproval = false;
						record.Status = "Finance Rejected";
						_db.Entry(record).State = EntityState.Modified;
						_db.SaveChanges();
					}

					var entity = new ApTravelOrderValidation
					{
						ApTravelOrderId = id,
						Status = apTravelOrderValidation.Status,
						Remark = apTravelOrderValidation.Remark,
						OrgBranchName = apTravelOrderValidation.OrgBranchName,

						DateCreated = DateTime.Now,
						CreatedBy = User.Identity.Name,
						LastModified = null,
						ModifiedBy = null
					};

					_db.ApTravelOrderValidations.Add(entity);
					_db.SaveChanges();
					apTravelOrderValidation.ApTravelOrderValidationId = entity.ApTravelOrderValidationId;
					#region operation log
					UserHelper.OperationLog(Request.UserHostAddress, "AP travel order approval", "Success", User.Identity.Name, $"approved travel order {entity.ApTravelOrderId} ", Modules.Finance);
					#endregion
				}
			}

			return Json(new[] { apTravelOrderValidation }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableTravelOrderApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ApTravelOrderValidations_Update([DataSourceRequest]DataSourceRequest request, ApTravelOrderValidation apTravelOrderValidation)
		{
			if (ModelState.IsValid)
			{
				var entity = new ApTravelOrderValidation
				{
					ApTravelOrderValidationId = apTravelOrderValidation.ApTravelOrderValidationId,
					Status = apTravelOrderValidation.Status,
					Remark = apTravelOrderValidation.Remark,
					OrgBranchName = apTravelOrderValidation.OrgBranchName,
					DateCreated = apTravelOrderValidation.DateCreated,
					LastModified = apTravelOrderValidation.LastModified,
					CreatedBy = apTravelOrderValidation.CreatedBy,
					ModifiedBy = apTravelOrderValidation.ModifiedBy
				};

				_db.ApTravelOrderValidations.Attach(entity);
				_db.Entry(entity).State = EntityState.Modified;
				_db.SaveChanges();
			}

			return Json(new[] { apTravelOrderValidation }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableTravelOrderApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ApTravelOrderValidations_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelOrderValidation apTravelOrderValidation)
		{
			if (ModelState.IsValid)
			{
				var entity = new ApTravelOrderValidation
				{
					ApTravelOrderValidationId = apTravelOrderValidation.ApTravelOrderValidationId,
					Status = apTravelOrderValidation.Status,
					Remark = apTravelOrderValidation.Remark,
					OrgBranchName = apTravelOrderValidation.OrgBranchName,
					DateCreated = apTravelOrderValidation.DateCreated,
					LastModified = apTravelOrderValidation.LastModified,
					CreatedBy = apTravelOrderValidation.CreatedBy,
					ModifiedBy = apTravelOrderValidation.ModifiedBy
				};

				_db.ApTravelOrderValidations.Attach(entity);
				_db.ApTravelOrderValidations.Remove(entity);
				_db.SaveChanges();
			}

			return Json(new[] { apTravelOrderValidation }.ToDataSourceResult(request, ModelState));
		}

	   
		protected override void Dispose(bool disposing)
		{
			_db.Dispose();
			base.Dispose(disposing);
		}
	}
}
