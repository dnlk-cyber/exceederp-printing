using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.CRM.DataSource
{
    [DataObject]
    public class SalesOrderStatusDetailOds
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesOrderStatusDetailViewModel> GetSalesOrderData(string branchId, string storeId, string customerId, DateTime start, DateTime end)
        {
            List<SalesOrderStatusDetailViewModel> list = new List<SalesOrderStatusDetailViewModel>();
            var temp = _db.SalesOrderVouchers.Where(s => s.PaymentMethod == "CREDIT").ToList();
            var orders = new List<SalesOrderVoucher>();

            foreach (var o in temp)
            {
                if (o.Date.Date <= end.Date && o.Date.Date >= start.Date)
                {
                    orders.Add(o);
                }
            }
            if (branchId != null)
            {
                int bId = int.Parse(branchId);
                orders = orders.Where(o => o.BranchId == bId).ToList();
            }
            if (customerId != null)
            {
                int cId = int.Parse(customerId);
                orders = orders.Where(o => o.OrganizationCustomerId == cId).ToList();
            }
            var tempList = new List<SalesOrderVoucher>();
            tempList.AddRange(orders);
            if (!string.IsNullOrEmpty(storeId))
            {
                int sId = int.Parse(storeId);
                var lineItemsInStore =
                    _db.LineItems.Where(l => l.VoucherDefinitionID == "1" && l.StoreID == sId).ToList();
                var vIds = new HashSet<string>(lineItemsInStore.Select(l => l.Voucher));
                foreach (SalesOrderVoucher so in tempList)
                {
                    var voucher = so.SalesOrderVoucherID.ToString();
                    if (!vIds.Contains(voucher))
                    {
                        orders.Remove(so);
                    }
                }
            }
            foreach (SalesOrderVoucher so in orders)
            {
                //var record = new SalesOrderStatusDetailViewModel
                //{
                //    Customer = so.OrganizationCustomer.TradeName,
                //    OrderType = so.PaymentMethod,
                //    OrderNumber = so.SalesOrderVoucherID,
                //    OrderReference = "",
                //    OrderDate = so.Date,
                //    OrderStatus = so.LastObjectState,
                //};
                // shipment recors that refere this current sales order, it can be referenced from multiple shipment records
                var shipmentRecordOfCurrentOrder = 
                    _db.SalesShipments.Where(s => s.Source == ShipmentSource.SalesOrder && s.SalesOrderVoucherID == so.SalesOrderVoucherID).ToList();
                var shipmentIds = new HashSet<int>(shipmentRecordOfCurrentOrder.Select(s => s.SalesShipmentId));
                // all line items that the given sales order is referenced
                var shipmentLineItems = _db.SalesShipmentLineItem.Where(sl => shipmentIds.Contains(sl.SalesShipmentId)).ToList();
                var lineItems = _db.LineItems.Where(l => l.VoucherDefinitionID == "1" && l.Voucher == so.SalesOrderVoucherID.ToString()).ToList();
                if (lineItems.Any())
                {
                    foreach (LineItem lineItem in lineItems)
                    {
                        // shipment line items for this current sales order line item
                        var lineItemsInShipment = shipmentLineItems.Where(sl => sl.ProductId == lineItem.Product.ProductID).ToList();
                        var orderedQty = lineItem.Quantity;
                        decimal shipedQty = 0;
                        decimal remainingQty = 0;
                        if (lineItemsInShipment.Any())
                        {
                            shipedQty = lineItemsInShipment.Sum(l => l.ShipedQuantity);
                            remainingQty = orderedQty - shipedQty;
                        }
                        var lineRecord = new SalesOrderStatusDetailViewModel
                        {

                            Customer = so.OrganizationCustomer.TradeName,
                            OrderType = so.PaymentMethod,
                            OrderNumber = so.SalesOrderVoucherID,
                            OrderReference = "",
                            OrderDate = so.Date,
                            OrderStatus = so.LastObjectState,

                            Class = lineItem.Class,
                            Variety = lineItem.Variety,
                            ItemCategory = GetProductCategory(lineItem.Product.CategoryID),
                            ItemCode = lineItem.Product.ItemCode,
                            ItemName = lineItem.Product.Name,
                            Store = lineItem.SalesStore.Description,
                            UoM = lineItem.Product.UOM,
                            UnitPrice = lineItem.UnitAmount,
                            OrderQuantity = orderedQty,
                            ShipedQuantity = shipedQty,
                            RemainingQuantity = remainingQty,
                            Remarks = lineItem.Remark
                        };
                        list.Add(lineRecord);
                    }
                }

            }
            return list;
        }

        
        private string GetProductCategory(string categoryId)
        {
            return _db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId.ToString() == categoryId)?.Description;
        }
    }
}
