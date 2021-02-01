using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.Finance.Sales.DataSource
{

    [DataObject]
    public class SalesOrderDataSource
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesOrderViewModel> GetSalesOrder(string fiscalYear, string periodFrom, string periodTo, string orderType, string customer)
        {
            List<SalesOrderViewModel> salesOrderList = new List<SalesOrderViewModel>();

            if (!String.IsNullOrEmpty(periodFrom) && !String.IsNullOrEmpty(periodTo))
            {
                int from = int.Parse(periodFrom);
                int to = int.Parse(periodTo);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo;

                var saleOrders = _db.ArSalesOrders
                      .Where(x => x.isVoid == false)
                   .Where(x => x.IsOnlineApproved)
                    .Where(x => x.OrderDate >= dt11 && x.OrderDate <= dt22);

                if (orderType != null)
                {
                    if (orderType == "Cash")
                        saleOrders = saleOrders.Where(x => x.OrderType == OrderType.Cash);
                    else
                        saleOrders = saleOrders.Where(x => x.OrderType == OrderType.Credit);
                }
                if (customer != null)
                {
                    saleOrders = saleOrders.Where(x => x.CustomerName == customer);
                }

                foreach (var saleOrder in saleOrders)
                {
                    var invoices = _db.ARInvoices.Where(x => x.ArSalesOrderId == saleOrder.ArSalesOrderId)
                        .Where(x => x.IsOnlineApproved && x.IsVoid == false).Select(x => x.ArInvoiceId).ToList();

                    var broadcasts = _db.ArSalesOrderBroadcastTimes.Where(x => x.ArSalesOrderId == saleOrder.ArSalesOrderId).ToList();
                    var paymentSchedules = _db.ArSalesOrderPaymentSchedules.Where(x => x.ArSalesOrderId == saleOrder.ArSalesOrderId).ToList();
                    var saleOrderLine = _db.ArSalesOrderLines.Where(x => x.ArSalesOrderId == saleOrder.ArSalesOrderId).AsQueryable();

                    foreach (var line in saleOrderLine)
                    {
                        var sale = new SalesOrderViewModel();


                        foreach (var broadcast in broadcasts)
                        {
                            sale.BroadCastDate = sale.BroadCastDate+" "+broadcast.BroadcastDateTime;
                        }
                        foreach (var schedule in paymentSchedules)
                        {
                            sale.PaymentSchedule = sale.PaymentSchedule + "," + schedule.Amount;
                        }

                        // sale.BroadCastStatus = saleOrder.OrderStatus.ToString();
                        sale.SaleOrderNo = saleOrder.ArSalesOrderId;
                        sale.SaleOrderStatus = saleOrder.OrderStatus.ToString();
                        sale.SaleOrderDate = saleOrder.OrderDate;

                        sale.CustomerName = saleOrder.CustomerName;
                        sale.CustomerType = saleOrder.OrderType.ToString();

                        var productItem = _db.ArProductService.FirstOrDefault(x => x.ArProductServiceId.ToString() == line.SubItem);
                        if (productItem != null) sale.ItemDescription = productItem.ItemName;

                        sale.QtyOrdered = line.Quantity;
                        sale.AmountOrdered = line.TotalAmount;

                        var orderitem = _db.ArProductService.Find(int.Parse(line.SubItem));

                        var invoiceLines = _db.ARInvoiceLines
                            .Where(x => invoices.Contains(x.ArInvoiceId))
                            .Where(x => x.ArSalesOrderLineId == line.ArSalesOrderLineId).ToList();

                        sale.QtyInvoiced =line.ArSalesOrder.OrderType==OrderType.Cash? line.Quantity: invoiceLines.Sum(x => x.Quantity);
                        //sale.QtyInvoiced =  invoiceLines.Sum(x => x.Quantity);
                        sale.QtyRemaining = sale.QtyOrdered - sale.QtyInvoiced;

                        sale.AmountInvoiced = line.ArSalesOrder.OrderType == OrderType.Cash ? line.TotalAmount : invoiceLines.Sum(x => x.TotalAmount);
                        //sale.AmountInvoiced =  invoiceLines.Sum(x => x.TotalAmount);
                        sale.AmountRemaining = sale.AmountOrdered - sale.AmountInvoiced;

                        salesOrderList.Add(sale);
                    }
                }
            }


            return salesOrderList;
        }
    }
}
