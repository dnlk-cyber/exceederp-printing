using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
	public class ApDirectPaymentsController : Controller
	{

		private readonly ExceedDbContext _db = new ExceedDbContext();
		private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();


		[AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayablePayment)]
		public ActionResult Index()
		{
			PopulateViewData();

			return View();
		}

		private void PopulateViewData()
		{
			var paymentMethod = _db.PaymentMethods.ToList();

			ViewData["methods"] = paymentMethod;

			var tax = _db.GLTaxRates.Where(x => x.ApplyToAp).ToList();

			ViewData["taxes"] = tax;

			var witholdings = _db.GLTaxRates.Where(x => x.ApplyToWithholding).Where(x => x.ApplyToAp).ToList();

			ViewData["witholdingType"] = witholdings;

			var employees = _db.Person_Employee;
			var selectedEmployees = employees.Select(
			  i => new
			  {
				  Name = i.EmployeeCode + " " + i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
				  Code = i.EmployeeId
			  }).AsQueryable();
			ViewData["employees"] = selectedEmployees;
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayablePayment)]
		public ActionResult APDirectPayments_Read([DataSourceRequest]DataSourceRequest request, bool isNotTransffered)
		{
			List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
			IQueryable<ApDirectPayment> apDirectpayments = _db.ApDirectPayments
				 .Where(x => mybraches.Contains(x.CreatedAtBranchId));

			if (GlobalHelper.IsMultipleBusinessUnit())
			{
				List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
				apDirectpayments = apDirectpayments.Where(x => myBusinessUnits.Contains(x.CreatedAtBusinessUnitId));
			}


			if (isNotTransffered)
			{
				List<int> transfferableList = FinanceTransferHelper.GetApDirectPaymentTranferableList();
				apDirectpayments =
					apDirectpayments.Where(x => transfferableList.Contains(x.ApDirectPaymentId));
			}
			DataSourceResult result = apDirectpayments.ToDataSourceResult(request, aPDirectPayments => new
			{
				ApDirectPaymentId = aPDirectPayments.ApDirectPaymentId,
				BranchId = aPDirectPayments.BranchId,
				BusinessUnitId = aPDirectPayments.BusinessUnitId,
				CreatedAtBranchId = aPDirectPayments.CreatedAtBranchId,
				CreatedAtBusinessUnitId = aPDirectPayments.CreatedAtBusinessUnitId,
				IsDirectPayment = aPDirectPayments.IsDirectPayment,
				SourceId = aPDirectPayments.SourceId,
				SupplierId = aPDirectPayments.SupplierId,
				IsNewSupplier = aPDirectPayments.IsNewSupplier,
				SupplierName = aPDirectPayments.SupplierName,
				Purchaser = aPDirectPayments.Purchaser,
				Grn = aPDirectPayments.Grn,
				Po = aPDirectPayments.Po,
				PaymentType = aPDirectPayments.PaymentType,
				PaymentMethod = aPDirectPayments.PaymentMethod,
				PaymentReference = aPDirectPayments.PaymentReference,
				PaymentDate = aPDirectPayments.PaymentDate,
				Remark = aPDirectPayments.Remark,
				IsSettled = aPDirectPayments.IsSettled,
				IsSendForApproval = aPDirectPayments.IsSendForApproval,
				SendForApprovalBy = aPDirectPayments.SendForApprovalBy,
				SendForApprovalTime = aPDirectPayments.SendForApprovalTime,
				IsOnlineTransferred = aPDirectPayments.IsOnlineTransferred,
				IsOnlineTransferredBy = aPDirectPayments.IsOnlineTransferredBy,
				OnlineTransferredTime = aPDirectPayments.OnlineTransferredTime,
				Status = aPDirectPayments.Status,
				IsOnlineApproved = aPDirectPayments.IsOnlineApproved,
				OnlineApprovedBy = aPDirectPayments.OnlineApprovedBy,
				OnlineApprovedTime = aPDirectPayments.OnlineApprovedTime,
				IsVoid = aPDirectPayments.IsVoid,
				VoidBy = aPDirectPayments.VoidBy,
				VoidTime = aPDirectPayments.VoidTime,
				OrgBranchName = aPDirectPayments.OrgBranchName,
				DateCreated = aPDirectPayments.DateCreated,
				LastModified = aPDirectPayments.LastModified,
				CreatedBy = aPDirectPayments.CreatedBy,
				ModifiedBy = aPDirectPayments.ModifiedBy
			});

			return Json(result);
		}

		[AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult APDirectPayments_Create([DataSourceRequest]DataSourceRequest request, ApDirectPayment aPDirectPayments)
		{
			#region validation


			if (aPDirectPayments.IsNewSupplier && String.IsNullOrWhiteSpace(aPDirectPayments.SourceId))
				ModelState.AddModelError("source", "Please select payment source.");

			if (!string.IsNullOrEmpty(aPDirectPayments.PaymentReference))
			{
				var exists = _db.ApDirectPayments.Where(x => x.PaymentReference == aPDirectPayments.PaymentReference && x.IsVoid == false).ToList();

				if (exists.Any())
					ModelState.AddModelError("Payment Reference", $"{aPDirectPayments.PaymentReference}  already exists in the system.");
			}

			if (string.IsNullOrEmpty(aPDirectPayments.PaymentMethod))
				ModelState.AddModelError("Payment method", $"Please select payment method.");

			if (!string.IsNullOrEmpty(aPDirectPayments.SourceId))
			{
				int did = int.Parse(aPDirectPayments.SourceId.Split('_')[1]);
				string sourceType = aPDirectPayments.SourceId.Split('_')[0];

				var exists = _db.ApDirectPayments.Where(x => x.SourceId == aPDirectPayments.SourceId && x.IsVoid == false).ToList();

				if (exists.Any() && sourceType!= "PAYMENT-REQUEST")
					ModelState.AddModelError("Payment Source", $"{aPDirectPayments.SourceId}  already exists in the system.");

				if (sourceType=="PAYMENT-REQUEST")
				{
					List<int> Ids = exists.Select(x => x.ApDirectPaymentId).ToList();
					var paymentLines = _db.ApDirectPaymentLines.Where(x => Ids.Contains(x.ApDirectPaymentId)).ToList();
					var requestLines = _db.PaymentRequestLines.Where(x => x.PaymentRequestId==did).ToList();
					if(paymentLines.Any())
					{
						decimal payamount= paymentLines.Sum(x => x.Amount);
						decimal requestamount = requestLines.Any() ? requestLines.Sum(x => x.Amount) : 0;

						if (payamount >= requestamount)
							ModelState.AddModelError("over", "The amount of payment paid and under process is more than the request. ");

					}
				}
			}
			#endregion

			if (!String.IsNullOrWhiteSpace(aPDirectPayments.SourceId))
			{

				#region data

				int did = int.Parse(aPDirectPayments.SourceId.Split('_')[1]);
				string sourceType = aPDirectPayments.SourceId.Split('_')[0];
				switch (sourceType)
				{
					case "LC":
						var purchaseCost = _db.ForeignPurchaseCosts.Find(did);
						if (purchaseCost != null)
							aPDirectPayments.SupplierName = purchaseCost.Payee;
						break;
					case "PAYMENT-REQUEST":
						var paymentRequest = _db.PaymentRequests.Find(did);
						if (paymentRequest != null)
						{
							aPDirectPayments.BranchId = paymentRequest.BranchId;
							aPDirectPayments.BusinessUnitId = paymentRequest.BusinessUnitId;
							aPDirectPayments.SupplierName = paymentRequest.Supplier;
							aPDirectPayments.Purchaser = paymentRequest.EmployeeId;
							aPDirectPayments.Grn = paymentRequest.GRN;
							aPDirectPayments.Po = paymentRequest.PONo;
						}
						break;
					case "TRAVEL-ORDER":
						var travelOrder = _db.ApTravelOrders.Find(did);
						if (travelOrder != null)
						{
							aPDirectPayments.BranchId = travelOrder.BranchId;
							aPDirectPayments.BusinessUnitId = travelOrder.BusinessUnitId;
							aPDirectPayments.Purchaser = travelOrder.EmployeeId;
						}
						break;
					case "EXPENSE-CLAIM":
						var claim = _db.ApTravelExpenseClaims.Find(did);
						if (claim != null)
						{
							aPDirectPayments.BranchId = claim.BranchId;
							aPDirectPayments.BusinessUnitId = claim.BusinessUnitId;
							aPDirectPayments.Purchaser = claim.EmployeeId;
						}
						break;
					case "FUND-TRANSFER":
						var fund = _db.FundTransfers.Find(did);
						if (fund != null)
						{
							aPDirectPayments.BranchId = fund.BranchId;
							aPDirectPayments.BusinessUnitId = fund.BusinessUnitId;
							aPDirectPayments.Purchaser = fund.EmployeeId;
						}
						break;
					case "SALES-RETURN":
						var refund = _db.SalesReturn.Find(did);
						if (refund != null)
						{
							aPDirectPayments.BranchId = refund.BranchId;
							aPDirectPayments.BusinessUnitId = refund.BusinessUnitId;
						}
						break;

				}

				aPDirectPayments.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
				aPDirectPayments.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
				#endregion

			}

			if (ModelState.IsValid)
			{
				var entity = new ApDirectPayment
				{
					BranchId = aPDirectPayments.BranchId,
					BusinessUnitId = aPDirectPayments.BusinessUnitId,
					CreatedAtBranchId = aPDirectPayments.CreatedAtBranchId,
					CreatedAtBusinessUnitId = aPDirectPayments.CreatedAtBusinessUnitId,
					SourceId = aPDirectPayments.SourceId,
					IsDirectPayment = aPDirectPayments.IsDirectPayment,
					SupplierId = aPDirectPayments.SupplierId,
					IsNewSupplier = aPDirectPayments.IsNewSupplier,
					SupplierName = aPDirectPayments.SupplierName,
					Purchaser = aPDirectPayments.Purchaser,
					Grn = aPDirectPayments.Grn,
					Po = aPDirectPayments.Po,
					PaymentType = aPDirectPayments.PaymentType,
					PaymentMethod = aPDirectPayments.PaymentMethod,
					PaymentReference = aPDirectPayments.PaymentReference,
					PaymentDate = aPDirectPayments.PaymentDate.Date,
					IsSettled = aPDirectPayments.IsSettled,
					Remark = aPDirectPayments.Remark,
					IsSendForApproval = aPDirectPayments.IsSendForApproval,
					IsOnlineTransferred = aPDirectPayments.IsOnlineTransferred,
					IsOnlineTransferredBy = aPDirectPayments.IsOnlineTransferredBy,
					OnlineTransferredTime = aPDirectPayments.OnlineTransferredTime,
					Status = aPDirectPayments.Status,
					IsOnlineApproved = aPDirectPayments.IsOnlineApproved,
					OnlineApprovedBy = aPDirectPayments.OnlineApprovedBy,
					OnlineApprovedTime = aPDirectPayments.OnlineApprovedTime,
					IsVoid = aPDirectPayments.IsVoid,
					VoidBy = aPDirectPayments.VoidBy,
					VoidTime = aPDirectPayments.VoidTime,
					OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
					DateCreated = DateTime.Now,
					LastModified = aPDirectPayments.LastModified,
					CreatedBy = User.Identity.Name,
					ModifiedBy = aPDirectPayments.ModifiedBy
				};

				_db.ApDirectPayments.Add(entity);
				_db.SaveChanges();
				aPDirectPayments.ApDirectPaymentId = entity.ApDirectPaymentId;

				GetPaymentItems(entity);

				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "direct payment", "Success", User.Identity.Name, $"created direct payment {entity.ApDirectPaymentId} ", Modules.Finance);
				#endregion
			}

			return Json(new[] { aPDirectPayments }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult APDirectPayments_Update([DataSourceRequest]DataSourceRequest request, ApDirectPayment aPDirectPayments)
		{
			#region validation


			if (aPDirectPayments.IsNewSupplier && String.IsNullOrWhiteSpace(aPDirectPayments.SourceId))
				ModelState.AddModelError("source", "Please select payment source.");

			if (!String.IsNullOrEmpty(aPDirectPayments.PaymentReference))
			{
				var exists = _db.ApDirectPayments
					.Where(x => x.ApDirectPaymentId != aPDirectPayments.ApDirectPaymentId)
					.Where(x => x.PaymentReference == aPDirectPayments.PaymentReference && x.IsVoid == false).ToList();

				if (exists.Any())
					ModelState.AddModelError("Payment Reference", $"{aPDirectPayments.PaymentReference}  already exists in the system.");
			}

			if (string.IsNullOrEmpty(aPDirectPayments.PaymentMethod))
				ModelState.AddModelError("Payment method", $"Please select payment method.");

			if (!String.IsNullOrEmpty(aPDirectPayments.SourceId))
			{
				int did = int.Parse(aPDirectPayments.SourceId.Split('_')[1]);
				string sourceType = aPDirectPayments.SourceId.Split('_')[0];
				var exists = _db.ApDirectPayments
					.Where(x => x.ApDirectPaymentId != aPDirectPayments.ApDirectPaymentId)
					.Where(x => x.SourceId == aPDirectPayments.SourceId && x.IsVoid == false).ToList();

				if (exists.Any() && sourceType != "PAYMENT-REQUEST")
					ModelState.AddModelError("Payment Source", $"{aPDirectPayments.SourceId}  already exists in the system.");
							   

				if (sourceType == "PAYMENT-REQUEST")
				{
					List<int> Ids = exists.Select(x => x.ApDirectPaymentId).ToList();
					var paymentLines = _db.ApDirectPaymentLines.Where(x => Ids.Contains(x.ApDirectPaymentId)).ToList();
					var requestLines = _db.PaymentRequestLines.Where(x => x.PaymentRequestId == did).ToList();
					if (paymentLines.Any())
					{
						decimal payamount = paymentLines.Sum(x => x.Amount);
						decimal requestamount = requestLines.Any() ? requestLines.Sum(x => x.Amount) : 0;

						if (payamount >= requestamount)
							ModelState.AddModelError("over", "The amount of payment paid and under process is more than the request. ");

					}
				}
			}

			#endregion

			if (!String.IsNullOrWhiteSpace(aPDirectPayments.SourceId))
			{

				#region data

				int did = int.Parse(aPDirectPayments.SourceId.Split('_')[1]);
				string sourceType = aPDirectPayments.SourceId.Split('_')[0];
				switch (sourceType)
				{
					case "LC":
						var purchaseCost = _db.ForeignPurchaseCosts.Find(did);
						if (purchaseCost != null)
							aPDirectPayments.SupplierName = purchaseCost.Payee;
						break;
					case "PAYMENT-REQUEST":
						var paymentRequest = _db.PaymentRequests.Find(did);
						if (paymentRequest != null)
						{
							aPDirectPayments.BranchId = paymentRequest.BranchId;
							aPDirectPayments.BusinessUnitId = paymentRequest.BusinessUnitId;
							aPDirectPayments.SupplierName = paymentRequest.Supplier;
							aPDirectPayments.Purchaser = paymentRequest.EmployeeId;
							aPDirectPayments.Grn = paymentRequest.GRN;
							aPDirectPayments.Po = paymentRequest.PONo;
						}
						break;
					case "TRAVEL-ORDER":
						var travelOrder = _db.ApTravelOrders.Find(did);
						if (travelOrder != null)
						{
							aPDirectPayments.BranchId = travelOrder.BranchId;
							aPDirectPayments.BusinessUnitId = travelOrder.BusinessUnitId;
							aPDirectPayments.Purchaser = travelOrder.EmployeeId;
						}
						break;
					case "EXPENSE-CLAIM":
						var claim = _db.ApTravelExpenseClaims.Find(did);
						if (claim != null)
						{
							aPDirectPayments.BranchId = claim.BranchId;
							aPDirectPayments.BusinessUnitId = claim.BusinessUnitId;
							aPDirectPayments.Purchaser = claim.EmployeeId;
						}
						break;
					case "FUND-TRANSFER":
						var fund = _db.FundTransfers.Find(did);
						if (fund != null)
						{
							aPDirectPayments.BranchId = fund.BranchId;
							aPDirectPayments.BusinessUnitId = fund.BusinessUnitId;
							aPDirectPayments.Purchaser = fund.EmployeeId;
						}
						break;
					case "SALES-RETURN":
						var refund = _db.SalesReturn.Find(did);
						if (refund != null)
						{
							aPDirectPayments.BranchId = refund.BranchId;
							aPDirectPayments.BusinessUnitId = refund.BusinessUnitId;
						}
						break;
				}

				#endregion

			}

			#region validated record

			var parent = _db.ApDirectPayments.Find(aPDirectPayments.ApDirectPaymentId);
			if (parent != null)
			{
				if (parent.IsOnlineApproved)
					ModelState.AddModelError("approved", "You cannot manipulate this record");

				#region detach local version
				var local = _db.Set<ApDirectPayment>()
					.Local
					.FirstOrDefault(f => f.ApDirectPaymentId == parent.ApDirectPaymentId);
				if (local != null)
				{
					_db.Entry(local).State = EntityState.Detached;
				}
				#endregion

			}


			#endregion


			if (ModelState.IsValid)
			{
				var entity = new ApDirectPayment
				{
					BranchId = aPDirectPayments.BranchId,
					BusinessUnitId = aPDirectPayments.BusinessUnitId,
					CreatedAtBranchId = aPDirectPayments.CreatedAtBranchId,
					CreatedAtBusinessUnitId = aPDirectPayments.CreatedAtBusinessUnitId,
					SourceId = aPDirectPayments.SourceId,
					IsDirectPayment = aPDirectPayments.IsDirectPayment,
					ApDirectPaymentId = aPDirectPayments.ApDirectPaymentId,
					SupplierId = aPDirectPayments.SupplierId,
					IsNewSupplier = aPDirectPayments.IsNewSupplier,
					SupplierName = aPDirectPayments.SupplierName,
					Purchaser = aPDirectPayments.Purchaser,
					Grn = aPDirectPayments.Grn,
					Po = aPDirectPayments.Po,
					PaymentType = aPDirectPayments.PaymentType,
					PaymentMethod = aPDirectPayments.PaymentMethod,
					PaymentReference = aPDirectPayments.PaymentReference,
					PaymentDate = aPDirectPayments.PaymentDate,
					IsSettled = aPDirectPayments.IsSettled,
					Remark = aPDirectPayments.Remark,
					IsSendForApproval = aPDirectPayments.IsSendForApproval,
					IsOnlineTransferred = aPDirectPayments.IsOnlineTransferred,
					IsOnlineTransferredBy = aPDirectPayments.IsOnlineTransferredBy,
					OnlineTransferredTime = aPDirectPayments.OnlineTransferredTime,
					Status = aPDirectPayments.Status,
					IsOnlineApproved = aPDirectPayments.IsOnlineApproved,
					OnlineApprovedBy = aPDirectPayments.OnlineApprovedBy,
					OnlineApprovedTime = aPDirectPayments.OnlineApprovedTime,
					IsVoid = aPDirectPayments.IsVoid,
					VoidBy = aPDirectPayments.VoidBy,
					VoidTime = aPDirectPayments.VoidTime,
					OrgBranchName = aPDirectPayments.OrgBranchName,
					DateCreated = aPDirectPayments.DateCreated,
					LastModified = DateTime.Now,
					CreatedBy = aPDirectPayments.CreatedBy,
					ModifiedBy = User.Identity.Name
				};

				_db.ApDirectPayments.Attach(entity);
				_db.Entry(entity).State = EntityState.Modified;
				_db.SaveChanges();

				GetPaymentItems(entity);

				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "direct payment", "Success", User.Identity.Name, $"updated direct payment {entity.ApDirectPaymentId} ", Modules.Finance);
				#endregion
			}
			return Json(new[] { aPDirectPayments }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult APDirectPayments_Destroy([DataSourceRequest]DataSourceRequest request, ApDirectPayment aPDirectPayments)
		{



			if (ModelState.IsValid)
			{
				var entity = new ApDirectPayment
				{
					BranchId = aPDirectPayments.BranchId,
					BusinessUnitId = aPDirectPayments.BusinessUnitId,
					CreatedAtBranchId = aPDirectPayments.CreatedAtBranchId,
					CreatedAtBusinessUnitId = aPDirectPayments.CreatedAtBusinessUnitId,
					IsDirectPayment = aPDirectPayments.IsDirectPayment,
					ApDirectPaymentId = aPDirectPayments.ApDirectPaymentId,
					SupplierId = aPDirectPayments.SupplierId,
					IsNewSupplier = aPDirectPayments.IsNewSupplier,
					SupplierName = aPDirectPayments.SupplierName,
					Purchaser = aPDirectPayments.Purchaser,
					Grn = aPDirectPayments.Grn,
					Po = aPDirectPayments.Po,
					PaymentType = aPDirectPayments.PaymentType,
					PaymentMethod = aPDirectPayments.PaymentMethod,
					PaymentReference = aPDirectPayments.PaymentReference,
					PaymentDate = aPDirectPayments.PaymentDate,
					Remark = aPDirectPayments.Remark,
					IsSendForApproval = aPDirectPayments.IsSendForApproval,
					IsOnlineTransferred = aPDirectPayments.IsOnlineTransferred,
					IsOnlineTransferredBy = aPDirectPayments.IsOnlineTransferredBy,
					OnlineTransferredTime = aPDirectPayments.OnlineTransferredTime,
					Status = aPDirectPayments.Status,
					IsOnlineApproved = aPDirectPayments.IsOnlineApproved,
					OnlineApprovedBy = aPDirectPayments.OnlineApprovedBy,
					OnlineApprovedTime = aPDirectPayments.OnlineApprovedTime,
					IsVoid = aPDirectPayments.IsVoid,
					VoidBy = aPDirectPayments.VoidBy,
					VoidTime = aPDirectPayments.VoidTime,
					OrgBranchName = aPDirectPayments.OrgBranchName,
					DateCreated = aPDirectPayments.DateCreated,
					LastModified = aPDirectPayments.LastModified,
					CreatedBy = aPDirectPayments.CreatedBy,
					ModifiedBy = aPDirectPayments.ModifiedBy
				};

				_db.ApDirectPayments.Attach(entity);
				_db.ApDirectPayments.Remove(entity);
				_db.SaveChanges();

				#region delete log
				UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted direct payment" + aPDirectPayments.ApDirectPaymentId + " " + aPDirectPayments.PaymentReference, Modules.Finance);
				#endregion
			}
			return Json(new[] { aPDirectPayments }.ToDataSourceResult(request, ModelState));
		}

		private void GetPaymentItems(ApDirectPayment record)
		{
			int did = int.Parse(record.SourceId.Split('_')[1]);
			string sourceType = record.SourceId.Split('_')[0];
			switch (sourceType)
			{
				case "LC":
					#region foreign purchase

					var purchaseCost = _db.ForeignPurchaseCosts.Find(did);
					if (purchaseCost != null)
					{
						var entity = new ApDirectPaymentLine
						{
							ApDirectPaymentId = record.ApDirectPaymentId,
							Purpose = purchaseCost.CostType,
							Amount = purchaseCost.Amount,
							ForexAmount = purchaseCost.ForexAmount,
							ExchangeRate = purchaseCost.ExchangeRate,
							Currency = purchaseCost.Currency,
							AccountCode = "NA",
							GLTaxRateID = 0,
							TaxAmount = 0,
							WithholdingType = 0,
							WithholdingAmount = 0,
							WithholdTax = false,
							WithholdTaxAmount = 0,
							TotalAmount = 0,
							NetPayable = 0,
							Remark = purchaseCost.Remark,
							DateCreated = DateTime.Now,
							LastModified = purchaseCost.LastModified,
							CreatedBy = User.Identity.Name,
							ModifiedBy = purchaseCost.ModifiedBy
						};

						_db.ApDirectPaymentLines.Add(entity);
						_db.SaveChanges();
					}

					#endregion
					break;
				case "PAYMENT-REQUEST":
					#region payment request

					var paymentRequestLines = _db.PaymentRequestLines.Where(x => x.PaymentRequestId == did).ToList();
					var paymentrequest = _db.PaymentRequests.Find(did);
					foreach (var requestLine in paymentRequestLines)
					{
						var apitem = _db.ApSetupItems.Find(int.Parse(requestLine.ItemName));
						if (apitem != null)
						{
							var entity = new ApDirectPaymentLine
							{
								ApDirectPaymentId = record.ApDirectPaymentId,
								Purpose = paymentrequest.ReasonOfPayment != null ? paymentrequest.ReasonOfPayment : "NA",
								Amount = requestLine.Amount,
								ForexAmount = requestLine.Amount,
								ExchangeRate = 1,
								Currency = "NA",
								AccountCode = apitem.AccountCode,
								GLTaxRateID = requestLine?.GLTaxRateID??0,
								TaxAmount = requestLine.TaxAmount,
								WithholdingType = 0,
								WithholdingAmount = 0,
								WithholdTax = false,
								WithholdTaxAmount = 0,
								TotalAmount = requestLine.TotalAmount,
								NetPayable = requestLine.TotalAmount,
								Remark = requestLine.Remark,
								DateCreated = DateTime.Now,
								LastModified = requestLine.LastModified,
								CreatedBy = User.Identity.Name,
								ModifiedBy = requestLine.ModifiedBy
							};

							_db.ApDirectPaymentLines.Add(entity);
						}

						_db.SaveChanges();
					}

					#endregion
					break;
				case "TRAVEL-ORDER":
					#region travel order

					var adbvancePyamnets = _db.ApTravelOrderAdvancePayments.Where(x => x.ApTravelOrderId == did).ToList();
					var travelOder = _db.ApTravelOrders.Find(did);
					foreach (var advance in adbvancePyamnets)
					{
						string acc = "";
						if (!string.IsNullOrEmpty(advance.ItemName))
						{
							var apitem = _db.ApSetupItems.Find(int.Parse(advance.ItemName));
							acc = apitem.AccountCode;
						}


						var entity = new ApDirectPaymentLine
						{
							ApDirectPaymentId = record.ApDirectPaymentId,
							Purpose = travelOder.PurposeOfTravel ?? "NA",
							Amount = advance.Total,
							ForexAmount = advance.Total,
							ExchangeRate = 1,
							Currency = "NA",
							AccountCode = acc,
							GLTaxRateID = 0,
							TaxAmount = 0,
							WithholdingType = 0,
							WithholdingAmount = 0,
							WithholdTax = false,
							WithholdTaxAmount = 0,
							TotalAmount = advance.Total,
							NetPayable = advance.Total,
							Remark = "",
							DateCreated = DateTime.Now,
							LastModified = advance.LastModified,
							CreatedBy = User.Identity.Name,
							ModifiedBy = advance.ModifiedBy
						};

						_db.ApDirectPaymentLines.Add(entity);

						_db.SaveChanges();
					}

					#endregion
					break;
				case "EXPENSE-CLAIM":
					#region travel expense claim

					var expensePayments = _db.ApTravelExpensePayments.Where(x => x.ApTravelExpenseClaimId == did).ToList();
					var expenseclaim = _db.ApTravelExpenseClaims.Find(did);
					foreach (var payment in expensePayments)
					{
						var entity = new ApDirectPaymentLine
						{
							ApDirectPaymentId = record.ApDirectPaymentId,
							Purpose = expenseclaim.PurposeOfTravel,
							Amount = payment.Amount,
							ForexAmount = payment.Amount,
							ExchangeRate = 1,
							Currency = "NA",
							AccountCode = "NA",
							GLTaxRateID = 0,
							TaxAmount = 0,
							WithholdingType = 0,
							WithholdingAmount = 0,
							WithholdTax = false,
							WithholdTaxAmount = 0,
							TotalAmount = payment.Amount,
							NetPayable = payment.Amount,
							Remark = "",
							DateCreated = DateTime.Now,
							LastModified = payment.LastModified,
							CreatedBy = User.Identity.Name,
							ModifiedBy = payment.ModifiedBy
						};

						_db.ApDirectPaymentLines.Add(entity);
						_db.SaveChanges();
					}

					#endregion
					break;
				case "FUND-TRANSFER":
					#region fund transfer

					var fundTransferDetails = _db.FundTransferDetails.Where(x => x.FundTransferId == did).ToList();
					var fundTransfer = _db.FundTransfers.Find(did);
					foreach (var detail in fundTransferDetails)
					{
						string acc = "NA";


						var entity = new ApDirectPaymentLine
						{
							ApDirectPaymentId = record.ApDirectPaymentId,
							Purpose = fundTransfer.ReasonOfTransfer,
							Amount = detail.Approved,
							ForexAmount = detail.Approved,
							ExchangeRate = 1,
							Currency = "NA",
							AccountCode = acc,
							GLTaxRateID = 0,
							TaxAmount = 0,
							WithholdingType = 0,
							WithholdingAmount = 0,
							WithholdTax = false,
							WithholdTaxAmount = 0,
							TotalAmount = detail.Approved,
							NetPayable = detail.Approved,
							Remark = "",
							DateCreated = DateTime.Now,
							LastModified = detail.LastModified,
							CreatedBy = User.Identity.Name,
							ModifiedBy = detail.ModifiedBy
						};

						_db.ApDirectPaymentLines.Add(entity);

						_db.SaveChanges();
					}

					#endregion
					break;
				case "SALES-RETURN":
					#region refund

					var refundDetails = _db.SalesReturnLines.Where(x => x.SalesReturnId == did).ToList();
					var refund = _db.SalesReturn.Find(did);
					foreach (var detail in refundDetails)
					{
						string acc = "NA";

						var entity = new ApDirectPaymentLine
						{
							ApDirectPaymentId = record.ApDirectPaymentId,
							Purpose = refund.Reason,
							Amount = detail.SubTotal,
							ForexAmount = detail.SubTotal,
							ExchangeRate = 1,
							Currency = "NA",
							AccountCode = acc,
							GLTaxRateID = detail.TaxTypeId!=null? (int)detail.TaxTypeId:0,
							TaxAmount = detail.TaxAmount,
							WithholdingType = 0,
							WithholdingAmount = 0,
							WithholdTax = false,
							WithholdTaxAmount = 0,
							TotalAmount = detail.SubTotal+detail.TaxAmount,
							NetPayable = detail.SubTotal + detail.TaxAmount,
							Remark = "",
							DateCreated = DateTime.Now,
							LastModified = detail.LastModified,
							CreatedBy = User.Identity.Name,
							ModifiedBy = detail.ModifiedBy
						};

						_db.ApDirectPaymentLines.Add(entity);

						_db.SaveChanges();
					}

					#endregion
					break;
			}
			#region update distribution
			_distributionHelper.UpdateApDirectPaymentDistribution(record.ApDirectPaymentId);
			#endregion
		}

		[AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
		public JsonResult SendforApproval(int id = 0)
		{
			var record = _db.ApDirectPayments.Find(id);
			string errMsg = "";
			decimal sumTotalDisDebit = 0;
			decimal sumTotalDisCredit = 0;
			#region check segment length validation

			var distributions = _db.ApDirectPaymentDistributions.Where(x => x.ApDirectPaymentId == id).ToList();
			AccountHelper accountHelper = new AccountHelper();
			foreach (var dist in distributions)
			{
				string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
				if (!String.IsNullOrEmpty(message))
				{
					var formattedData = new
					{
						Message = message,
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

				#region debit credit validation
				if (dist.Debit != 0 && dist.Credit != 0)
				{
					message = $"{dist.AccountCode} Account has both debit and credit value.";
					var formattedData = new
					{
						Message = message,
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

				if (dist.Debit == 0 && dist.Credit == 0)
				{
					message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
					var formattedData = new
					{
						Message = message,
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

				if (dist.Debit > 0 && dist.Credit > 0)
				{
					message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
					var formattedData = new
					{
						Message = message,
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

				if (dist.Debit < 0 || dist.Credit < 0)
				{
					message = $"{dist.AccountCode} Negative values are not allowed.";
					var formattedData = new
					{
						Message = message,
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}
				#endregion

			}


			#endregion

			if (record != null && record.IsOnlineApproved)
			{
				errMsg = "You cannot manipulate this record";
				var formattedData = new
				{
					Message = errMsg,
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			if (record != null && record.IsSendForApproval)
			{
				errMsg = "This record is already sent for approval.";
				var formattedData = new
				{
					Message = errMsg,
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}



			try
			{
				if (record != null)
				{
					sumTotalDisDebit = record.ApDirectPaymentDistribution.Sum(x => x.Debit);
					sumTotalDisCredit = record.ApDirectPaymentDistribution.Sum(x => x.Credit);
				}
			}
			catch (Exception)
			{
				errMsg = "Please add distribution and try again";
				var formattedData = new
				{
					Message = errMsg,
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (sumTotalDisDebit != sumTotalDisCredit)
			{
				errMsg = "Total Debit and Total Credit is not equal.Please correct it and try again";
				var formattedData = new
				{
					Message = errMsg,
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (sumTotalDisDebit == 0 || sumTotalDisCredit == 0)
			{
				errMsg = "Distribution should be above 0.";
				var formattedData = new
				{
					Message = errMsg,
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			var lines = _db.ApDirectPaymentLines.Where(x => x.ApDirectPaymentId == id);
			if (!lines.Any())
			{
				errMsg = "Please add lines first";
				var formattedData = new
				{
					Message = errMsg,
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record != null && !record.IsOnlineApproved && !record.IsSendForApproval)
			{
				record.Status = Enum.GetName(typeof(OperationStatuses), 4);
				record.IsSendForApproval = true;
				record.SendForApprovalBy = User.Identity.Name;
				record.SendForApprovalTime = DateTime.Now;
				_db.Entry(record).State = EntityState.Modified;
				_db.SaveChanges();

				var formattedData = new
				{
					Message = "Successfully Sent.",
					Status = "OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);

			}
			return null;
		}

		[AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
		public JsonResult TransferDistribution(string id, string description)
		{
			int myid = int.Parse(id);
			var record = _db.ApDirectPayments.Find(myid);
			if (record != null && !record.IsOnlineApproved)
			{
				var formattedData = new
				{
					Message = "The record is not approved.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record != null && !record.IsOnlineTransferred)
			{
				#region check if already transffered 
				var exist = _db.GLRecordJournals
					.Where(x => x.Source == JournalSources.AP_Direct_Payment.ToString())
					.Where(x => x.SourceId == record.ApDirectPaymentId.ToString())
					.FirstOrDefault();
				if (exist != null)
				{
					var formattedData = new
					{
						Message = "The record is already transffered, it can't be retransferred.",
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

				#endregion
				#region check segment length validation
				AccountHelper accountHelper = new AccountHelper();
				foreach (var dist in record.ApDirectPaymentDistribution)
				{
					string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
					if (!String.IsNullOrEmpty(message))
					{
						var formattedData = new
						{
							Message = message,
							Status = "NOT_OK"
						};
						return Json(formattedData, JsonRequestBehavior.AllowGet);
					}

					#region debit credit validation
					if (dist.Debit != 0 && dist.Credit != 0)
					{
						message = $"{dist.AccountCode} Account has both debit and credit value.";
						var formattedData = new
						{
							Message = message,
							Status = "NOT_OK"
						};
						return Json(formattedData, JsonRequestBehavior.AllowGet);
					}

					if (dist.Debit == 0 && dist.Credit == 0)
					{
						message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
						var formattedData = new
						{
							Message = message,
							Status = "NOT_OK"
						};
						return Json(formattedData, JsonRequestBehavior.AllowGet);
					}

					if (dist.Debit > 0 && dist.Credit > 0)
					{
						message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
						var formattedData = new
						{
							Message = message,
							Status = "NOT_OK"
						};
						return Json(formattedData, JsonRequestBehavior.AllowGet);
					}

					if (dist.Debit < 0 || dist.Credit < 0)
					{
						message = $"{dist.AccountCode} Negative values are not allowed.";
						var formattedData = new
						{
							Message = message,
							Status = "NOT_OK"
						};
						return Json(formattedData, JsonRequestBehavior.AllowGet);
					}
					#endregion

				}


				#endregion
				#region check if debit and credit are equal
				decimal debit = 0;
				decimal credit = 0;
				var distribution = record.ApDirectPaymentDistribution.ToList();
				if (distribution.Any())
				{
					debit = distribution.Sum(x => x.Debit);
					credit = distribution.Sum(x => x.Credit);
				}
				if (debit != credit)
				{
					var formattedData = new
					{
						Message = "Total debit and total credit is not balanced.",
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}
				if (debit == 0)
				{
					var formattedData = new
					{
						Message = "Total debit and total credit is zero.",
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}
				#endregion
				#region send to GL

				string glyear = "";
				string glperiod = "";
				#region get period
				var period = _db.GLPeriods.Where(x => x.DateFrom <= record.PaymentDate.Date)
					.Where(x => x.DateTo >= record.PaymentDate.Date).ToList();
				if (period.Any())
				{

					if (period.Count() == 1)
					{
						var myPeriod = period.FirstOrDefault();
						if (myPeriod != null)
						{
							glperiod = myPeriod.Name;
							glyear = myPeriod.GlFiscalYear.Name.ToString();
							if (myPeriod.Status == FiscalYearStatus.Close)
							{
								var formattedData = new
								{
									Message = "The period " + glperiod +
											  " is closed,the record cannot be transfered to closed period.",
									Status = "NOT_OK"
								};
								return Json(formattedData, JsonRequestBehavior.AllowGet);
							}
						}
						else
						{
							var formattedData = new
							{
								Message = "Period not found.",
								Status = "NOT_OK"
							};
							return Json(formattedData, JsonRequestBehavior.AllowGet);
						}
					}

					else
					{
						var formattedData = new
						{
							Message = "There are multiple periods defined that contain the Invoice date.",
							Status = "NOT_OK"
						};
						return Json(formattedData, JsonRequestBehavior.AllowGet);
					}

				}
				else
				{
					var formattedData = new
					{
						Message = "There are no periods defined that contain the payment date.",
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

				#endregion

				if (record.ApDirectPaymentDistribution.Any())
				{
					GlRecordJournal journal = new GlRecordJournal
					{
						DateCreated = DateTime.Now,
						CreatedBy = User.Identity.Name,
						JournalName = "AP Direct Payment",
						Description = !String.IsNullOrEmpty(description) ? description : "AP Direct Payment",
						FiscalYear = glyear,
						PeriodName = glperiod,
						CategoryName = "AP Direct Payment",
						SourcesName = "AP",
						JournalNumber = "APDPAY." + record.ApDirectPaymentId.ToString(),
						SourceId = record.ApDirectPaymentId.ToString(),
						Source = JournalSources.AP_Direct_Payment.ToString(),
						EffectiveDate = record.PaymentDate.Date,
						OrgBranchName = record.OrgBranchName,
						Status = "Approved",
						BranchId = record.CreatedAtBranchId,
						BusinessUnitId = record.CreatedAtBusinessUnitId,
						Remark = record.PaymentReference,
						PaymentReference = record.PaymentReference,
						ReferenceDoc = record.Remark,
						InvoiceReferenceDoc = "",
						DebitSum=debit,
						IsOnlineApproved = true,
						OnlineApprovedBy = User.Identity.Name,
						OnlineApprovedTime = DateTime.Now
					};
					_db.GLRecordJournals.Add(journal);
					_db.SaveChanges();
					foreach (var i in record.ApDirectPaymentDistribution)
					{
						#region debit and credit

						var journalDetail = new GLRecordJournalDetail
						{
							DateCreated = DateTime.Now,
							CreatedBy = User.Identity.Name,
							GLRecordJournal = journal,
							AccountCode = i.AccountCode,
							AccountDesc = i.AccountDesc,
							Debit = i.Debit,
							Credit = i.Credit,
							AdditionalDescription = i.AdditionalDescription,
						};
						_db.GLRecordJournalDetails.Add(journalDetail);
						_db.SaveChanges();
						#endregion
					}

					#region update record

					record.IsOnlineTransferred = true;
					_db.Entry(record).State = EntityState.Modified;
					_db.SaveChanges();

					#endregion

					#region split journal
					var journaldetails = _db.GLRecordJournalDetails
						.Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
					foreach (var detail in journaldetails)
					{
						AccountHelper helper = new AccountHelper();
						helper.SplitJournal(detail.GlRecordJournalDetailId);
					}
					#endregion

					var formattedData = new
					{
						Message = "Successfully transfered.",
						Status = "OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);

				}
				#endregion

			}
			else
			{
				var formattedData = new
				{
					Message = "You cannot transfer this transaction, it is already transfered.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			return null;
		}

		[AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
		public JsonResult Batchtransfer(string id)
		{

			string _response = "SUCCESS";
			int did = int.Parse(id);
			var period = _db.GLPeriods.Find(did);

			List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

			var untransffered = _db.ApDirectPayments
				.Where(x=>x.PaymentDate>=period.DateFrom && x.PaymentDate<=period.DateTo)
				.Where(x => x.IsOnlineApproved && x.IsVoid == false && x.IsOnlineTransferred==false)
				 //.Where(x => mybraches.Contains(x.CreatedAtBranchId))
				 .Where(x => x.CreatedBy==User.Identity.Name)
				.AsQueryable();

			if (GlobalHelper.IsMultipleBusinessUnit())
			{
				List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
				untransffered = untransffered.Where(x => myBusinessUnits.Contains(x.CreatedAtBusinessUnitId));
			}

			if (!untransffered.Any())
			{
				var formattedData1 = new
				{
					Message = "Data not found for transffer.",
					Status = "NOT_OK"
				};
				return Json(formattedData1, JsonRequestBehavior.AllowGet);
			}

			foreach (var record in untransffered)
			{
				_response = FinanceTransferHelper.ApDirectPayment(record.ApDirectPaymentId, "Batch Transfer", User.Identity.Name);
				if (_response != "SUCCESS") ;
					//Logger.Debug("AP Direct payment transfer ID:" + record.ApDirectPaymentId + _response);
			}

			#region auto job



			#endregion

			#region operation log
			UserHelper.OperationLog(Request.UserHostAddress, "AP direct payment", "Success", User.Identity.Name, $"batch transfer for period {id} ", Modules.Finance);
			#endregion

			var formattedData = new
			{
				Message = $"Operation Completed. {untransffered.Count()} records transffred.",
				Status = "OK"
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableVoid)]
		public JsonResult Void(int id)
		{
			var record = _db.ApDirectPayments.Find(id);
			if (record == null)
			{
				var formattedData = new
				{
					Message = "The record is not found.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred )
			{
				record.Status = Enum.GetName(typeof(OperationStatuses), 10);
				record.VoidBy = User.Identity.Name;
				record.VoidTime = DateTime.Now;
				record.IsVoid = true;
				_db.ApDirectPayments.Attach(record);
				_db.Entry(record).State = EntityState.Modified;
				//_db.SaveChanges();

				#region update payment request/invoice
			 

				int did = int.Parse(record.SourceId.Split('_')[1]);
				string sourceType = record.SourceId.Split('_')[0];
				switch (sourceType)
				{
					case "LC":
						var purchaseCost = _db.ForeignPurchaseCosts.Find(did);
						if (purchaseCost != null)
						{
							purchaseCost.Paid = false;
							_db.ForeignPurchaseCosts.Attach(purchaseCost);
							_db.Entry(purchaseCost).State = EntityState.Modified;

						}
						break;
					case "PAYMENT-REQUEST":
						var paymentRequest = _db.PaymentRequests.Find(did);
						if (paymentRequest != null)
						{
							paymentRequest.Paid = false;
							_db.PaymentRequests.Attach(paymentRequest);
							_db.Entry(paymentRequest).State = EntityState.Modified;
						}
						break;
					case "TRAVEL-ORDER":
						var travelOrder = _db.ApTravelOrders.Find(did);
						if (travelOrder != null)
						{
							travelOrder.Paid = false;
							_db.ApTravelOrders.Attach(travelOrder);
							_db.Entry(travelOrder).State = EntityState.Modified;
						}
						break;
					case "EXPENSE-CLAIM":
						var claim = _db.ApTravelExpenseClaims.Find(did);
						if (claim != null)
						{
							claim.Paid = false;
							_db.ApTravelExpenseClaims.Attach(claim);
							_db.Entry(claim).State = EntityState.Modified;
						}
						break;
					case "FUND-TRANSFER":
						var fund = _db.FundTransfers.Find(did);
						if (fund != null)
						{
							fund.Paid = false;
							_db.FundTransfers.Attach(fund);
							_db.Entry(fund).State = EntityState.Modified;
						}
						break;
				}

				#endregion

				_db.SaveChanges();
				
				//copy
				//CopyRecord(id);

				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "direct payment", "Success", User.Identity.Name, $"void direct payment {id} ", Modules.Finance);
				#endregion

				var formattedData = new
				{
					Message = "Void Successful.",
					Status = "OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			

			if (record.IsVoid)
			{
				var formattedData = new
				{
					Message = "The record is already void.You can not void again.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			if (record.IsOnlineTransferred)
			{
				var formattedData = new
				{
					Message = "The record is transferred to GL.You can not void .",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			if (!record.IsOnlineApproved)
			{
				var formattedData = new
				{
					Message = "The record is not approved hence you can modify by editing.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}


			return null;
		}

		protected override void Dispose(bool disposing)
		{
			_db.Dispose();
			base.Dispose(disposing);
		}
	}
}
