using ExceedERP.Core.Domain.Finance.AP.FundTransfer;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.FundTransfers
{
	public class ApFundTransferDetailController : Controller
	{
		private ExceedDbContext db = new ExceedDbContext();
		// GET: Finance/FundTransferDetail

		public ActionResult FundTransferDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
		{
			IQueryable<FundTransferDetail> FundTransferDetails = db.FundTransferDetails.Where(x => x.FundTransferId == id);

			DataSourceResult result = FundTransferDetails.ToDataSourceResult(request, detail => new FundTransferDetail
			{
				FundTransferId = detail.FundTransferId,
				FundTransferDetailId = detail.FundTransferDetailId,
				Description = detail.Description,
				Requested = detail.Requested,
				Approved = detail.Approved,
				Remark = detail.Remark,
			});

			return Json(result);
		}
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FundTransferDetail_Create([DataSourceRequest]DataSourceRequest request, FundTransferDetail detail, int id)
		{
			detail.Approved = detail.Requested;

			if (detail.Approved > detail.Requested)
				ModelState.AddModelError("above","Approved should not be more than requested");

			if (ModelState.IsValid)
			{
				var entity = new FundTransferDetail
				{
					FundTransferId = id,
					Description = detail.Description,
					Requested = detail.Requested,
					Approved = detail.Approved,
					Remark = detail.Remark
				};

				db.FundTransferDetails.Add(entity);
				db.SaveChanges();
				detail.FundTransferDetailId = entity.FundTransferDetailId;

				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "Self care fund transfer item", "Success", User.Identity.Name, $"created fund transfer item under record {entity.FundTransferId} ", Modules.Finance);
				#endregion
			}
			return Json(new[] { detail }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FundTransferDetail_Update([DataSourceRequest]DataSourceRequest request, FundTransferDetail detail)
		{
			if (detail.Approved > detail.Requested)
				ModelState.AddModelError("above", "Approved should not be more than requested");

			if (ModelState.IsValid)
			{
				var entity = new FundTransferDetail
				{
					FundTransferDetailId = detail.FundTransferDetailId,
					Description = detail.Description,
					Requested = detail.Requested,
					FundTransferId = detail.FundTransferId,
					Approved = detail.Approved,
					Remark = detail.Remark
				};

				db.FundTransferDetails.Attach(entity);
				db.Entry(entity).State = EntityState.Modified;
				db.SaveChanges();
				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "Self care fund transfer item", "Success", User.Identity.Name, $"updated fund transfer item under record {entity.FundTransferId} ", Modules.Finance);
				#endregion
			}
			return Json(new[] { detail }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FundTransferDetail_Destroy([DataSourceRequest]DataSourceRequest request, FundTransferDetail detail)
		{
			if (ModelState.IsValid)
			{
				var entity = new FundTransferDetail
				{
					FundTransferDetailId = detail.FundTransferDetailId,
					Description = detail.Description,
					Requested = detail.Requested,
					FundTransferId = detail.FundTransferId,
					Approved = detail.Approved,
					Remark = detail.Remark
				};
				#region delete log
				//UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Payment request of employee code  " + paymentRequest.EmployeeCode);
				#endregion
				db.FundTransferDetails.Attach(entity);
				db.FundTransferDetails.Remove(entity);
				db.SaveChanges();
			}
			return Json(new[] { detail }.ToDataSourceResult(request, ModelState));
		}

		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}
	}
}
