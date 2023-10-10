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
                var jobOrder = db.JobOrder.FirstOrDefault(j => j.Num.ToString() == currentStoreRequisition.JobOrderNo);
                int? customerId = null;
                string plateNo = "";
                if (jobOrder != null && jobOrder.Referance != null)
                {
                    var reqNumber = jobOrder.Referance.Value;
                    var jobReq = db.JobRequest.FirstOrDefault(r => r.Num == reqNumber);
                    //var equipment = db.EquipmentInformation.Find(jobOrder.EquipmentInformationId);
                    if (jobReq != null)
                    {
                        customerId = jobReq.CustomerId;
                        plateNo = jobReq.LicensePlate;
                    }
                }
                if (salesStore != null && crmPeriod != null)
                {
                    #region create shipment header
                    var shipment = new SalesShipment
                    {
                        Source = ShipmentSource.MaintenanceStoreRequisition,
                        BranchId = currentStoreRequisition.BranchId,
                        StoreID = salesStore.SalesStoreId,
                        JobOrderNumber = currentStoreRequisition.JobOrderNo,
                        StoreRequisitionNumber = currentStoreRequisition.StoreRequisitionID.ToString(),
                        PlateNo = plateNo, // added to indicate the vehicle being maintained not the shipping vehicle in this scenario

                        CRMPeriodId = crmPeriod.CRMPeriodId,
                        ShipmentDate = DateTime.Now,
                        OrganizationCustomerId = customerId.HasValue? customerId.Value : 0,
                        Remark = $"Maintenance JO No. {storeRequisition.JobOrderNo} Store Requi. No. {currentStoreRequisition.StoreRequisitionID}",
                        Status = OperationStatuses.New.ToString(),

                        OnlineApprovedTime = null,
                        OnlineTransferredTime = null,
                        IsOnlineTransferredBy = null,
                        MethodOfShipment = null,
                        SendForApprovalTime = null,
                        VoidTime = null,
                        VoidBy = null,
                        OrgBranchName = null,

                        IsVoid = false,
                        IsSendForApproval = false,
                        FsInvoiceNumber = null,
                        CashSalesInvoiceID = null,
                        SalesOrderVoucherID = null,
                        IsOnlineApproved = false,
                        IsOnlineTransferred = false,
                        OnlineApprovedBy = null,
                        SendForApprovalBy = null,



                        CreatedBy = userName,
                        DateCreated = DateTime.Now,
                        ModifiedBy = null,
                        LastModified = null,
                    };
                    db.SalesShipments.Add(shipment);
                    var res = db.SaveChanges();
                    if (res > 0)
                    {
                        #region generate shipment lines
                        var requisitionLines = db.StoreRequisitionItems.Where(l => l.StoreRequisitionID == currentStoreRequisition.StoreRequisitionID).ToList();
                        foreach (var item in requisitionLines)
                        {
                            var product = db.Products.FirstOrDefault(p => p.ItemCode == item.ItemCode);
                            if(product != null)
                            {
                                var shipmentLine = new SalesShipmentLineItem
                                {
                                    ProductId = product.ProductID,
                                    Class = null,
                                    Variety = null,
                                    SalesStoreId = salesStore.SalesStoreId,
                                    LineItemId = 0,
                                    SalesShipmentId = shipment.SalesShipmentId,
                                    CreatedBy = userName,
                                    DateCreated = DateTime.Now,
                                    LastModified = null,
                                    ModifiedBy = null,
                                    Price = 0, // no sales price at this step. price will be auto updated after sales
                                    TotalQuantity = item.ApprovedQuantity,
                                    RemainingQuantity = 0,
                                    ShipedQuantity = item.ApprovedQuantity,
                                    ShipedQuantityTilPreShipment = 0,
                                    
                                    TotalAmount = 0,
                                    TaxAmount = 0,
                                    SubTotal = 0,
                                };
                                db.SalesShipmentLineItem.Add(shipmentLine);
                                
                            }
                            db.SaveChanges(); // commit changes all at once.
                        }
                        #endregion

                        #region generate distribution
                        new CRMPaymentDistibutionHelper().UpdateShipmentDistribution(shipment);
                        #endregion
                    }
                    #endregion
                }

            }

            return statusViewModel;
        }

        public static CRMTransactionValidationStatusViewModel CreateSalesQuoteVoucherFromMaintenance(JobOrder jobOrder, string userName)
        {
            CRMTransactionValidationStatusViewModel statusViewModel = new CRMTransactionValidationStatusViewModel();
            var db = new ExceedDbContext();
            var currentJobOrder = db.JobOrder.Find(jobOrder.JobOrderID);
            if (currentJobOrder != null)
            {
                int? customerId = null;
                bool isInternalJoborder = false;
                string plateNo = "";

                if (jobOrder != null && jobOrder.Referance != null)
                {
                    var reqNumber = jobOrder.Referance.Value;
                    var jobReq = db.JobRequest.FirstOrDefault(r => r.Num == reqNumber);
                    if (jobReq != null)
                    {
                        if(jobReq.Source == JobRequestSource.CompanyVehicle)
                        {
                            isInternalJoborder = true;
                        }
                        customerId = jobReq.CustomerId;
                        plateNo = jobReq.LicensePlate;
                    }
                }

                var now = DateTime.Now.Date;
                var crmPeriod = db.CRMPeriods.FirstOrDefault(p => p.DateFrom <= now && p.DateTo >= now);
                var defaultBranch = db.Branch.FirstOrDefault();
                var defaultStore = db.SalesStores.FirstOrDefault();
                if(!isInternalJoborder && crmPeriod != null && defaultBranch != null && defaultStore != null && customerId.HasValue)
                {
                    #region create sales quote 
                    var salesQuote = new SalesQuoteVoucher
                    {
                        OrganizationCustomerId = customerId.Value,
                        BranchId = defaultBranch.BranchId,
                        StoreID = defaultStore.SalesStoreId,
                        CRMPeriodId = crmPeriod.CRMPeriodId,
                        JobOrderNumber = currentJobOrder.Num.ToString(),
                        LastObjectState = "Editing",
                        QuotationTermId = 0,
                        IsIssued = false,
                        IssuedDate = DateTime.Now,
                        PlateNo = plateNo , // added to indicate the vehicle being maintained
                        Remark = $"Generated from JO {currentJobOrder.Num}",

                        CreatedBy = userName,
                        DateCreated = DateTime.Now,
                        Date = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(1),
                        Source = SalesQuoteSource.Maintenance,

                        LastModified = null,
                        ModifiedBy = null,
                        OnlineApprovedTime = null,
                        OnlineTransferredTime = null,
                        IsOnlineTransferredBy = null,
                        SendForApprovalTime = null,
                        VoidTime = null,
                        VoidBy = null,
                        OrgBranchName = null,

                        IsVoid = false,
                        IsSendForApproval = false,
                        IsOnlineApproved = false,
                        IsOnlineTransferred = false,
                        OnlineApprovedBy = null,
                        SendForApprovalBy = null,
                    };

                    #endregion

                    var storeRequisitions = db.StoreRequisitions
                    .Where(s => s.IsForMaintenance &&
                    s.JobOrderNo == currentJobOrder.Num.ToString() &&
                    s.IsOnlineApproved &&
                    !s.IsVoid).ToList();
                    if (storeRequisitions.Any())
                    {
                        int[] branchIds = new HashSet<int>(storeRequisitions.Select(r => r.BranchId)).ToArray();
                        string[] storeCodes = new HashSet<string>(storeRequisitions.Select(r => r.StoreCode)).ToArray();
                        int branchId = branchIds.Length > 0 ? branchIds[0] : 0;
                        string storeCode = storeCodes.Length > 0 ? storeCodes[0] : "";
                        var salesStore = db.SalesStores.FirstOrDefault(s => s.StoreCode == storeCode);
                        if (salesStore != null)
                        {
                            salesQuote.BranchId = branchId;
                            salesQuote.StoreID = salesStore.SalesStoreId;
                        }
                    }
                    db.SalesQuoteVouchers.Add(salesQuote);
                    db.SaveChanges();


                    foreach (var storeReq in storeRequisitions)
                    {
                        var currentShipment = db.SalesShipments
                            .FirstOrDefault(s => s.Source == ShipmentSource.MaintenanceStoreRequisition &&
                            s.StoreRequisitionNumber == storeReq.StoreRequisitionID.ToString() &&
                            s.IsOnlineApproved &&
                            !s.IsVoid);

                        if (currentShipment != null)
                        {
                            var shipmentLines = db.SalesShipmentLineItem.Where(s => s.SalesShipmentId == currentShipment.SalesShipmentId).ToList();
                            foreach (SalesShipmentLineItem shipmentLineItem in shipmentLines)
                            {
                                #region create sales quote line
                                var quoteLine = new LineItem
                                {
                                    Voucher = salesQuote.SalesQuoteVoucherID.ToString(),
                                    VoucherDefinitionID = "0",
                                    ThingID = shipmentLineItem.ProductId,
                                    Quantity = shipmentLineItem.TotalQuantity,
                                    ShipmentNumber = currentShipment.SalesShipmentId.ToString(),
                                    References = shipmentLineItem.SalesShipmentLineItemId.ToString(),
                                    StoreID = salesQuote.StoreID,
                                    StoreRequisitionNumber = storeReq.StoreRequisitionID.ToString(),


                                    Class = shipmentLineItem.Class,
                                    Variety = shipmentLineItem.Variety,

                                    UnitAmount = 0,
                                    Discount = 0,
                                    CalculatedCost = 0,
                                    TotalAmount = 0,
                                    Vat = 0,
                                    TaxAmount = 0,
                                    WithholdingAmount = 0,
                                    WithholdTaxAmount = 0,
                                    Tax = 0.ToString(),
                                    //WithholdingType = 0.ToString(),

                                    CreatedBy = userName,
                                    DateCreated = DateTime.Now,
                                    Status = "Pending",

                                    LastModified = null,
                                    ModifiedBy = null,
                                    OnlineApprovedTime = null,
                                    OnlineTransferredTime = null,
                                    IsOnlineTransferredBy = null,
                                    SendForApprovalTime = null,
                                    VoidTime = null,
                                    VoidBy = null,
                                    OrgBranchName = null,

                                    IsVoid = false,
                                    IsSendForApproval = false,
                                    IsOnlineApproved = false,
                                    IsOnlineTransferred = false,
                                    OnlineApprovedBy = null,
                                    SendForApprovalBy = null,

                                };
                                db.LineItems.Add(quoteLine);
                                db.SaveChanges();
                                #endregion
                            }
                        }
                    }

                    #region job services
                    var jobServices = db.JobOrderDetail.Where(d => d.JobOrderID == currentJobOrder.JobOrderID).ToList();
                    foreach (var service in jobServices)
                    {
                        var repairType = db.RepairType.Find(service.RepairTypeID);
                        var workLog = db.ServiceCrewWorkLogs.FirstOrDefault(l => l.JobOrderDetailId == service.JobOrderDetailID);
                        if (repairType != null && workLog != null)
                        {
                            var amount = workLog.WorkShopType == WorkShopType.Internal_WorkShop ?
                                repairType.TotalAmountCompany :
                                repairType.TotalAmountCustomer;
                            var serviceLineItem = new ServiceLineItem
                            {
                                ServiceTypeCode = repairType.Code,
                                Quantity = 1,
                                UnitAmount = amount,
                                VoucherId = salesQuote.SalesQuoteVoucherID,
                                VoucherType = CRMVoucherType.SalesQuote,
                                ServiceTypeId = repairType.RepairTypeID.ToString(),
                                SubTotal = amount,
                                TotalAmount = amount,
                                TaxAmount = 0,

                                TaxTypeId = 0,
                                WithholdingTypeId = 0,
                                WithholdingAmount = 0,
                                WithholdTax = false,


                                CreatedBy = userName,
                                DateCreated = DateTime.Now,
                                ModifiedBy = null,
                                LastModified = null,
                            };
                            db.ServiceLineItems.Add(serviceLineItem);
                            db.SaveChanges();
                        }
                    }
                    #endregion
                }
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
                        if(item != null)
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
                        if(item != null)
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
            var currentJobOrder = db.JobOrder.FirstOrDefault(j => j.Num.ToString() == storeReturn.JobOrderId);
            var storeReturnItems = db.StoreReturnItemss.Where(i => i.StoreReturnID == storeReturn.StoreReturnID).ToList();
            // if current job order is not closed, there is not sales quote voucher to modify
            if(currentJobOrder != null && currentJobOrder.IsSecondOnlineApproved && currentJobOrder.Status == "Closed")
            {
                var referencedSalesQuote = db.SalesQuoteVouchers.FirstOrDefault(j => j.Source == SalesQuoteSource.Maintenance && j.JobOrderNumber == currentJobOrder.Num.ToString());
                // if voucher is in editing status consider the materials returned.
                if (referencedSalesQuote != null && !referencedSalesQuote.IsMiddleOnlineApproved && (referencedSalesQuote.LastObjectState == "Editing" || referencedSalesQuote.LastObjectState == "Unforwarded"))
                {
                    var quoteLines = db.LineItems.Where(l => l.Voucher == referencedSalesQuote.SalesQuoteVoucherID.ToString() && l.VoucherDefinitionID == "0").ToList();
                    foreach (var lineItem in quoteLines)
                    {
                        var returnItem = storeReturnItems.FirstOrDefault(s => s.ItemCode == lineItem.Product.ItemCode);
                        var shipmentItem =
                            db.SalesShipmentLineItem
                            .FirstOrDefault(l => l.LineItemId.ToString() == lineItem.References && 
                                l.SalesShipmentId.ToString() == lineItem.ShipmentNumber &&
                                l.SalesShipment.IsOnlineApproved);
                        if(returnItem != null && shipmentItem != null)
                        {
                            lineItem.Quantity = shipmentItem.ShipedQuantity - returnItem.Quantity;
                            lineItem.CalculatedCost = lineItem.Quantity * lineItem.UnitAmount;

                            var gLTaxRate = db.GLTaxRates.Where(t => t.GLTaxRateID.ToString() == lineItem.Tax).FirstOrDefault();

                            decimal taxPercent = gLTaxRate.Rate / 100;

                            decimal TaxAmount = (lineItem.CalculatedCost * taxPercent);
                            decimal TotalAmount = (lineItem.CalculatedCost) + TaxAmount - lineItem.Discount;

                            var Rates = db.GLTaxRates.ToList();

                            if (lineItem.WithholdingType != null)
                            {
                                var gLWithholdingTaxRate = db.GLTaxRates.FirstOrDefault(t => t.GLTaxRateID.ToString() == lineItem.WithholdingType);

                                if(gLWithholdingTaxRate != null)
                                {
                                    lineItem.WithholdingAmount = lineItem.CalculatedCost * gLWithholdingTaxRate.CalculatedRate;
                                }
                            }
                            if (lineItem.WithholdTax)
                            {
                                lineItem.WithholdTaxAmount = lineItem.TaxAmount;
                            }

                            db.LineItems.Attach(lineItem);
                            db.Entry(lineItem).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
            return model;
        }

        public static bool IsJobOrderClosed(string jobOrderNumber)
        {
            bool isClosed = false;
            var db = new ExceedDbContext();
            var job = db.JobOrder.FirstOrDefault(j => j.Num.ToString() == jobOrderNumber);
            if(job!= null && job.IsSecondOnlineApproved)
            {
                isClosed = true;
            }
            return isClosed;
        }
    }
}
