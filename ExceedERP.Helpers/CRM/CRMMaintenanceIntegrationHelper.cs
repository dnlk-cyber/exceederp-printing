using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Inventory;

using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExceedERP.Helpers.CRM
{
	public static class CRMMaintenanceIntegrationHelper
	{
		public static CRMTransactionValidationStatusViewModel CreateShipmentFromMaintenance(StoreRequisition storeRequisition, string userName)
		{
			CRMTransactionValidationStatusViewModel statusViewModel = new CRMTransactionValidationStatusViewModel();
			var db = new ExceedDbContext();
			var currentStoreRequisition = db.StoreRequisitions.Find(storeRequisition.StoreRequisitionID);
			if (currentStoreRequisition != null)
			{
				var salesStore = db.SalesStores.FirstOrDefault(s => s.StoreCode == currentStoreRequisition.StoreCode);
				var glPeriod = db.InventoryPeriods.Find(currentStoreRequisition.GLPeriodId);
				var pName = glPeriod?.Name;
				var crmPeriod = db.CRMPeriods.FirstOrDefault(p => p.Name == pName);
				//var serviceJob = db.ServiceCenterJob.FirstOrDefault(s => s.ServiceCenterJobId.ToString() == storeRequisition.ServiceJobOrderNo);
				//if (serviceJob != null)
				//{
				//	var jobOrder = db.JobOrder.Find(serviceJob.JobOrderID);
				//	int? customerId = null;
				//	string plateNo = "";
				//	if (jobOrder != null && jobOrder.Referance != null)
				//	{
				//		var reqNumber = jobOrder.Referance.Value;
				//		var jobReq = db.JobRequest.FirstOrDefault(r => r.Num == reqNumber);
				//		//var equipment = db.EquipmentInformation.Find(jobOrder.EquipmentInformationId);
				//		if (jobReq != null)
				//		{
				//			//customerId = jobReq.CustomerId;
				//			//plateNo = jobReq.LicensePlate;
				//		}
				//	}
				//	if (salesStore != null && crmPeriod != null)
				//	{
				//		#region create shipment header
				//		var shipment = new SalesShipment
				//		{
				//			Source = ShipmentSource.MaintenanceStoreRequisition,
				//			BranchId = currentStoreRequisition.BranchId,
				//			StoreID = salesStore.SalesStoreId,
				//			JobOrderNumber = currentStoreRequisition.JobOrderNo,
				//			StoreRequisitionNumber = currentStoreRequisition.StoreRequisitionID.ToString(),
				//			PlateNo = plateNo, // added to indicate the vehicle being maintained not the shipping vehicle in this scenario

				//			CRMPeriodId = crmPeriod.CRMPeriodId,
				//			ShipmentDate = DateTime.Now,
				//			OrganizationCustomerId = customerId.HasValue ? customerId.Value : 0,
				//			Remark = $"Maintenance JO No. {jobOrder.Num} Store Requi. No. {currentStoreRequisition.StoreRequisitionID}",
				//			Status = OperationStatuses.New.ToString(),

				//			OnlineApprovedTime = null,
				//			OnlineTransferredTime = null,
				//			IsOnlineTransferredBy = null,
				//			MethodOfShipment = null,
				//			SendForApprovalTime = null,
				//			VoidTime = null,
				//			VoidBy = null,
				//			OrgBranchName = null,

				//			IsVoid = false,
				//			IsSendForApproval = false,
				//			FsInvoiceNumber = null,
				//			CashSalesInvoiceID = null,
				//			SalesOrderVoucherID = null,
				//			IsOnlineApproved = false,
				//			IsOnlineTransferred = false,
				//			OnlineApprovedBy = null,
				//			SendForApprovalBy = null,



				//			CreatedBy = userName,
				//			DateCreated = DateTime.Now,
				//			ModifiedBy = null,
				//			LastModified = null,
				//		};
				//		db.SalesShipments.Add(shipment);
				//		var res = db.SaveChanges();
				//		if (res > 0)
				//		{
				//			#region generate shipment lines
				//			var requisitionLines = db.StoreRequisitionItems.Where(l => l.StoreRequisitionID == currentStoreRequisition.StoreRequisitionID).ToList();
				//			foreach (var item in requisitionLines)
				//			{
				//				var product = db.Products.FirstOrDefault(p => p.ItemCode == item.ItemCode);
				//				if (product != null)
				//				{
				//					var shipmentLine = new SalesShipmentLineItem
				//					{
				//						ProductId = product.ProductID,
				//						Class = null,
				//						Variety = null,
				//						SalesStoreId = salesStore.SalesStoreId,
				//						LineItemId = 0,
				//						SalesShipmentId = shipment.SalesShipmentId,
				//						CreatedBy = userName,
				//						DateCreated = DateTime.Now,
				//						LastModified = null,
				//						ModifiedBy = null,
				//						Price = 0, // no sales price at this step. price will be auto updated after sales
				//						TotalQuantity = item.ApprovedQuantity,
				//						RemainingQuantity = 0,
				//						ShipedQuantity = item.ApprovedQuantity,
				//						ShipedQuantityTilPreShipment = 0,

				//						TotalAmount = 0,
				//						TaxAmount = 0,
				//						SubTotal = 0,
				//					};
				//					db.SalesShipmentLineItem.Add(shipmentLine);

				//				}
				//				db.SaveChanges(); // commit changes all at once.
				//			}
				//			#endregion

				//			#region generate distribution
				//			new CRMPaymentDistibutionHelper().UpdateShipmentDistribution(shipment);
				//			#endregion
				//		}
				//		#endregion
				//	}
				//}
			}

			return statusViewModel;
		}

		
		/// <summary>
		/// update saling price and regenerate shipment distribution when sales invoice is approved
		/// shipment source has to be from maintenance - regular shipments not affected
		/// </summary>
		/// <param name="invoice"></param>
		/// <param name="userName"></param>
		/// <returns></returns>
		public static CRMTransactionValidationStatusViewModel UpdateMaintenanceShipmentOnInvoiceApprove(CashSalesInvoice invoice, string userName)
		{
			CRMTransactionValidationStatusViewModel statusViewModel = new CRMTransactionValidationStatusViewModel();
			var db = new ExceedDbContext();
			var invoiceLines = db.LineItems
				.Where(l => l.Voucher == invoice.CashSalesInvoiceID.ToString() && l.VoucherDefinitionID == "2")
				.ToList();
			if (!string.IsNullOrEmpty(invoice.JobOrderNumber))
			{
				HashSet<string> jOrders = new HashSet<string>();
				#region for multiple JO references from Credit sales
				// iterate through comma separeted JO numbers
				if (invoice.PaymentMethod == "CREDIT")
				{
					var concatnatedRef = invoice.JobOrderNumber.Trim();
					if (!string.IsNullOrEmpty(concatnatedRef))
					{
						var idsArray = concatnatedRef.Split(',');
						if (idsArray.Length > 0)
						{
							for (int i = 0; i < idsArray.Length; i++)
							{
								jOrders.Add(idsArray[i]);
							}
						}
					}
				}
				else
				{
					jOrders.Add(invoice.JobOrderNumber); // just one JO
				}

				#endregion
				var shipments = db.SalesShipments.Where(s => s.Source == ShipmentSource.MaintenanceStoreRequisition && jOrders.Contains(s.JobOrderNumber)).ToList();
				foreach (var shipment in shipments)
				{
					var shipmentLines = db.SalesShipmentLineItem.Where(l => l.SalesShipmentId == shipment.SalesShipmentId).ToList();

					foreach (var shipmentLine in shipmentLines)
					{
						var item = invoiceLines
							.FirstOrDefault(l => l.ShipmentNumber == shipment.SalesShipmentId.ToString() &&
													l.StoreRequisitionNumber == shipment.StoreRequisitionNumber &&
													l.ThingID == shipmentLine.ProductId
							);
						if (item != null)
						{
							shipmentLine.Price = item.UnitAmount;
							shipmentLine.SubTotal = item.CalculatedCost;
							shipmentLine.TaxAmount = item.TaxAmount;
							shipmentLine.TotalAmount = item.TotalAmount;
							shipmentLine.LineItemId = item.LineItemID;
							shipmentLine.Remark = $"updated on Invoice {invoice.CashSalesInvoiceID} approve";

							db.SalesShipmentLineItem.Attach(shipmentLine);
							db.Entry(shipmentLine).State = System.Data.Entity.EntityState.Modified;
							db.SaveChanges();
						}
					}

					// regenerate distribution according to new prices
					new CRMPaymentDistibutionHelper().UpdateShipmentDistribution(shipment);

				}
			}

			return statusViewModel;
		}

		/// <summary>
		/// update saling price and regenerate shipment distribution when sales quote is approved
		/// </summary>
		/// <param name="salesQuote"></param>
		/// <param name="userName"></param>
		/// <returns></returns>
		public static CRMTransactionValidationStatusViewModel UpdateMaintenanceShipmentOnSalesQuoteApprove(SalesQuoteVoucher salesQuote, string userName)
		{
			CRMTransactionValidationStatusViewModel statusViewModel = new CRMTransactionValidationStatusViewModel();
			var db = new ExceedDbContext();
			var lines = db.LineItems
				.Where(l => l.Voucher == salesQuote.SalesQuoteVoucherID.ToString() && l.VoucherDefinitionID == "0")
				.ToList();
			if (!string.IsNullOrEmpty(salesQuote.JobOrderNumber))
			{
				var shipments = db.SalesShipments.Where(s => s.Source == ShipmentSource.MaintenanceStoreRequisition && s.JobOrderNumber == salesQuote.JobOrderNumber).ToList();
				foreach (var shipment in shipments)
				{
					var shipmentLines = db.SalesShipmentLineItem.Where(l => l.SalesShipmentId == shipment.SalesShipmentId).ToList();

					foreach (var shipmentLine in shipmentLines)
					{
						var item = lines
							.FirstOrDefault(l => l.ShipmentNumber == shipment.SalesShipmentId.ToString() &&
													l.StoreRequisitionNumber == shipment.StoreRequisitionNumber &&
													l.ThingID == shipmentLine.ProductId
							);
						if (item != null)
						{
							shipmentLine.Price = item.UnitAmount;
							shipmentLine.SubTotal = item.CalculatedCost;
							shipmentLine.TaxAmount = item.TaxAmount;
							shipmentLine.TotalAmount = item.TotalAmount;
							shipmentLine.LineItemId = item.LineItemID;
							shipmentLine.Remark = $"updated on SQ {salesQuote.SalesQuoteVoucherID} approve";

							db.SalesShipmentLineItem.Attach(shipmentLine);
							db.Entry(shipmentLine).State = System.Data.Entity.EntityState.Modified;
							db.SaveChanges();
						}
					}

					// regenerate distribution according to new prices
					new CRMPaymentDistibutionHelper().UpdateShipmentDistribution(shipment);

				}
			}

			return statusViewModel;
		}
		public static CRMTransactionValidationStatusViewModel UpdateSalesQuoteOnStoreReturnApprove(StoreReturn storeReturn, string userName)
		{
			var db = new ExceedDbContext();
			CRMTransactionValidationStatusViewModel model = new CRMTransactionValidationStatusViewModel();
		
			return model;
		}

		public static bool IsJobOrderClosed(string jobOrderNumber)
		{
			bool isClosed = false;
			var db = new ExceedDbContext();
			var job = db.JobOrder.FirstOrDefault(j => j.Num.ToString() == jobOrderNumber);
			if (job != null && job.IsSecondOnlineApproved)
			{
				isClosed = true;
			}
			return isClosed;
		}
	}
}
