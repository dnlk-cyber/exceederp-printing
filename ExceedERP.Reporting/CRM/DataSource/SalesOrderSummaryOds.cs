using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.CRM.DataSource
{
    [DataObject]
    public class SalesOrderSummaryOds
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesOrderSummaryViewModel> GetSalesOrderData(string branchId, string storeId)
        {
            if (branchId == null)
            {
                return null;
            }

            int bId = int.Parse(branchId);

            List<SalesOrderSummaryViewModel> list = new List<SalesOrderSummaryViewModel>();
            //var customers = _db.OrganizationCustomers.ToList();
            var orders = _db.SalesOrderVouchers.Where(s => s.BranchId == bId).ToList();
            var tempList = new List<SalesOrderVoucher>();
            tempList.AddRange(orders);
            if (!string.IsNullOrEmpty(storeId))
            {
                int sId = int.Parse(storeId);
                var lineItemsInStore =
                    _db.LineItems.Where(l => l.VoucherDefinitionID == "1" && l.StoreID == sId).ToList();
                var vIds = new HashSet<string>(lineItemsInStore.Select(l => l.Voucher));
                foreach (SalesOrderVoucher sq in tempList)
                {
                    var voucher = sq.SalesOrderVoucherID.ToString();
                    if (!vIds.Contains(voucher))
                    {
                        orders.Remove(sq);
                    }
                }
            }
            var cIds = new HashSet<int>(orders.Select(o =>o.OrganizationCustomerId));
            var customersWithOrder =
                _db.OrganizationCustomers.Where(c => cIds.Contains(c.OrganizationCustomerID)).ToList();
            foreach (OrganizationCustomer customer in customersWithOrder)
            {
                var customerOrders = orders.Where(o => o.OrganizationCustomerId == customer.OrganizationCustomerID)
                    .ToList();
                //var balance = customerOrders.Any() ? customerOrders.Sum(c => c.GrandTotal) : 0;
                var totalBalance = decimal.Zero;
                foreach (SalesOrderVoucher order in customerOrders)
                {
                    var lineItems = _db.LineItems.Where(t => t.Voucher == order.SalesOrderVoucherID.ToString() && t.VoucherDefinitionID == "1").ToList();
                    if (lineItems.Any())
                    {
                        totalBalance += lineItems.Sum(l => l.TotalAmount);
                    }
                }
                var record = new SalesOrderSummaryViewModel
                {
                    CustomerId = customer.OrganizationCustomerID,
                    Customer = customer.TradeName,
                    PreviousBalance = 0,
                    CurrentBalance =  totalBalance,
                    TotalBalance = totalBalance
                };
                list.Add(record);
            }
            
            
            return list;
        }
    }

}
