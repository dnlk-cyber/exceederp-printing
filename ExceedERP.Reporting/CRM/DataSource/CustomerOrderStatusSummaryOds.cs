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
    public class CustomerOrderStatusSummaryOds
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CustomerOrderStatusSummaryViewModel> GetSalesOrderData(string branchId, string storeId, DateTime start, DateTime end)
        {
            List<CustomerOrderStatusSummaryViewModel> list = new List<CustomerOrderStatusSummaryViewModel>();
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
            var cIds = new HashSet<int>(orders.Select(o => o.OrganizationCustomerId));
            var customers = _db.OrganizationCustomers.Where(o => cIds.Contains(o.OrganizationCustomerID)).ToList();
            foreach (OrganizationCustomer customer in customers)
            {
                var cOrders = orders.Where(o => o.OrganizationCustomerId == customer.OrganizationCustomerID).ToList();
                decimal ordered = 0;
                decimal shiped = 0;

                foreach (SalesOrderVoucher so in cOrders)
                {
                    var shipmentRecordOfCurrentOrder =
                       _db.SalesShipments.Where(s => s.Source == ShipmentSource.SalesOrder && s.SalesOrderVoucherID == so.SalesOrderVoucherID).ToList();
                    var shipmentIds = new HashSet<int>(shipmentRecordOfCurrentOrder.Select(s => s.SalesShipmentId));

                    var shipmentLineItems = _db.SalesShipmentLineItem.Where(sl => shipmentIds.Contains(sl.SalesShipmentId)).ToList();
                    var lineItems = _db.LineItems.Where(l => l.VoucherDefinitionID == "1" && l.Voucher == so.SalesOrderVoucherID.ToString()).ToList();

                    if (lineItems.Any())
                    {
                        ordered += lineItems.Sum(l => l.Quantity);
                    }
                    if (shipmentLineItems.Any())
                    {
                        shiped += shipmentLineItems.Sum(l => l.ShipedQuantity);
                    }
                }
                var record = new CustomerOrderStatusSummaryViewModel
                {
                    CustomerId = customer.OrganizationCustomerID,
                    CustomerName = customer.TradeName,
                    OrderedQuantity = ordered,
                    ShipedQuantity = shiped,
                    RemainingQuantity = ordered - shiped
                };
                list.Add(record);
            }
            
            return list;
        }


        private string GetProductCategory(string categoryId)
        {
            return _db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId.ToString() == categoryId)?.Description;
        }
    }
}
