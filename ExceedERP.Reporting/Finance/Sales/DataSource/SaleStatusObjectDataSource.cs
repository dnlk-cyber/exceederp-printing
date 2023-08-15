using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.Sales
{

    [DataObject]
    public class SaleStatusObjectDataSource
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SaleStatusVm> GetSaleStatus(string periodFrom, string periodTo, string customer, string items,string branchid)
        {
            List<SaleStatusVm> saleStatusList = new List<SaleStatusVm>();

            if (!String.IsNullOrEmpty(periodFrom) && !String.IsNullOrEmpty(periodTo))
            {
                int from = int.Parse(periodFrom);
                int to = int.Parse(periodTo);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo;

                var salesOrders = _db.ArSalesOrders.Where(x => x.OrderType == OrderType.Credit)
                    .Where(x => x.IsOnlineApproved && x.isVoid == false)
                    .Where(x => x.OrderDate >= dt11 && x.OrderDate <= dt22).AsQueryable();

               

                if (!String.IsNullOrEmpty(customer))
                {
                    salesOrders = salesOrders.Where(x => x.CustomerName == customer).AsQueryable();
                }

                if (!String.IsNullOrEmpty(branchid))
                    salesOrders = salesOrders.Where(x => x.BranchId.ToString() == branchid).AsQueryable();


                foreach (var salesOrder in salesOrders)
                {
                    var invoices = _db.ARInvoices
                        .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                        .Where(x => x.ArSalesOrderId == salesOrder.ArSalesOrderId).AsQueryable();

                    foreach (var invoice in invoices)
                    {
                        var saleStatus = new SaleStatusVm
                        {
                            CustomerName = salesOrder.CustomerName,
                            CustomerId = salesOrder.CustomerId,
                            Reference=salesOrder.OrderReference,
                            OrderNo = salesOrder.ArSalesOrderId.ToString(),
                            OrderDate = salesOrder.OrderDate,
                            OrderAmount = salesOrder.ArSalesOrderLines.Sum(x => x.TotalAmount),
                            OrderStatus = salesOrder.OrderStatus.ToString(),
                            InvoiceNo = invoice.ArInvoiceId.ToString(),
                            InvoiceDate = invoice.Date,
                            InvoiceStatus = invoice.InvoiceStatus.ToString(),
                            InvoiceAmount = invoice.TotalAmount,
                            InvoiceBalance = salesOrder.ArSalesOrderLines.Sum(x => x.TotalAmount) - invoice.TotalAmount,//invoice.AmountDue
                            RemainingQty = salesOrder.ArSalesOrderLines.Sum(x => x.Quantity) - invoice.ArInvoiceLines.Sum(x=>x.Quantity)
                        };


                        saleStatusList.Add(saleStatus);
                    }
                }
            }

            return saleStatusList;

        }
    }
}
