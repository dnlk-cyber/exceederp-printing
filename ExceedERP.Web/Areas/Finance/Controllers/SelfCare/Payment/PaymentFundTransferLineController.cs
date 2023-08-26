using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare
{
	public class FundTransferLineController : Controller
	{
		private ExceedDbContext db = new ExceedDbContext();
		// GET: Finance/FundTransferLine

		public ActionResult FundTransferLine_Read([DataSourceRequest]DataSourceRequest request, int id)
		{
			IQueryable<FundTransferLine> FundTransferLines = db.FundTransferLines.Where(x => x.PaymentRequestId == id);

			DataSourceResult result = FundTransferLines.ToDataSourceResult(request, paymentRequest => new FundTransferLine
			{
				PaymentRequestId = paymentRequest.PaymentRequestId,
				FundTransferLineId = paymentRequest.FundTransferLineId,
				BranchId = paymentRequest.BranchId,
				Material = paymentRequest.Material,
				FuelAndLubricant = paymentRequest.FuelAndLubricant,
				ManPower = paymentRequest.ManPower,
				FoodAndPerDiem = paymentRequest.FoodAndPerDiem,
				Machinery = paymentRequest.Machinery,
				CompensationAndSeverance = paymentRequest.CompensationAndSeverance,
				TotalCost = paymentRequest.TotalCost,
				TotalApproved = paymentRequest.TotalApproved,
				TotalPermited = paymentRequest.TotalPermited,
				Remark = paymentRequest.Remark,
			});

			return Json(result);
		}
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FundTransferLine_Create([DataSourceRequest]DataSourceRequest request, FundTransferLine paymentRequest, int id)
		{
			#region data
			paymentRequest.TotalCost = paymentRequest.Material
				+ paymentRequest.FuelAndLubricant
				+ paymentRequest.ManPower
				+ paymentRequest.FoodAndPerDiem;

			paymentRequest.TotalApproved = paymentRequest.TotalCost;
			paymentRequest.TotalPermited = paymentRequest.TotalCost;
			#endregion

			if (ModelState.IsValid)
			{
				var entity = new FundTransferLine
				{
					BranchId = paymentRequest.BranchId,
					PaymentRequestId = id,
					Material = paymentRequest.Material,
					FuelAndLubricant = paymentRequest.FuelAndLubricant,
					ManPower = paymentRequest.ManPower,
					FoodAndPerDiem = paymentRequest.FoodAndPerDiem,
					Machinery = paymentRequest.Machinery,
					CompensationAndSeverance = paymentRequest.CompensationAndSeverance,
					TotalCost = paymentRequest.TotalCost,
					TotalApproved = paymentRequest.TotalApproved,
					TotalPermited = paymentRequest.TotalPermited,
					Remark = paymentRequest.Remark
				};

				db.FundTransferLines.Add(entity);
				db.SaveChanges();
				paymentRequest.FundTransferLineId = entity.FundTransferLineId;

				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request item", "Success", User.Identity.Name, $"created payment request item {entity.FuelAndLubricant} under record {entity.PaymentRequestId} ", Modules.Finance);
				#endregion
			}
			return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FundTransferLine_Update([DataSourceRequest]DataSourceRequest request, FundTransferLine paymentRequest)
		{
			#region data
			paymentRequest.TotalCost = paymentRequest.Material
				+ paymentRequest.FuelAndLubricant
				+ paymentRequest.ManPower
				+ paymentRequest.FoodAndPerDiem;

			paymentRequest.TotalApproved = paymentRequest.TotalCost;
			paymentRequest.TotalPermited = paymentRequest.TotalCost;
			#endregion
			if (ModelState.IsValid)
			{
				var entity = new FundTransferLine
				{
					FundTransferLineId = paymentRequest.FundTransferLineId,
					BranchId = paymentRequest.BranchId,
					PaymentRequestId = paymentRequest.PaymentRequestId,
					Material = paymentRequest.Material,
					FuelAndLubricant = paymentRequest.FuelAndLubricant,
					ManPower = paymentRequest.ManPower,
					FoodAndPerDiem = paymentRequest.FoodAndPerDiem,
					Machinery = paymentRequest.Machinery,
					CompensationAndSeverance = paymentRequest.CompensationAndSeverance,
					TotalCost = paymentRequest.TotalCost,
					TotalApproved = paymentRequest.TotalApproved,
					TotalPermited = paymentRequest.TotalPermited,
					Remark = paymentRequest.Remark
				};

				db.FundTransferLines.Attach(entity);
				db.Entry(entity).State = EntityState.Modified;
				db.SaveChanges();
				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request item", "Success", User.Identity.Name, $"updated payment request item {entity.FuelAndLubricant} under record {entity.PaymentRequestId} ", Modules.Finance);
				#endregion
			}
			return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FundTransferLine_Destroy([DataSourceRequest]DataSourceRequest request, FundTransferLine paymentRequest)
		{
			if (ModelState.IsValid)
			{
				var entity = new FundTransferLine
				{
					FundTransferLineId = paymentRequest.FundTransferLineId,
					BranchId = paymentRequest.BranchId,
					PaymentRequestId = paymentRequest.PaymentRequestId,
					Material = paymentRequest.Material,
					FuelAndLubricant = paymentRequest.FuelAndLubricant,
					ManPower = paymentRequest.ManPower,
					FoodAndPerDiem = paymentRequest.FoodAndPerDiem,
					Machinery = paymentRequest.Machinery,
					CompensationAndSeverance = paymentRequest.CompensationAndSeverance,
					TotalCost = paymentRequest.TotalCost,
					TotalApproved = paymentRequest.TotalApproved,
					TotalPermited = paymentRequest.TotalPermited,
					Remark = paymentRequest.Remark
				};
				#region delete log
				//UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Payment request of employee code  " + paymentRequest.EmployeeCode);
				#endregion
				db.FundTransferLines.Attach(entity);
				db.FundTransferLines.Remove(entity);
				db.SaveChanges();
			}
			return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
		}

		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}
	}
}
