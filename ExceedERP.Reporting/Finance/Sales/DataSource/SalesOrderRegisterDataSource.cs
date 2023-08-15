using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Sales.DataSource
{
    
    [DataObject]
    public class SalesOrderRegisterDataSource
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesOrderRegisterViewModel> GetSalesOrder(string fiscalYear, string periodFrom, string periodTo,string orderType,string customer)
        {
            List<SalesOrderRegisterViewModel> salesOrderRegisterList = new List<SalesOrderRegisterViewModel>();
            if (!String.IsNullOrEmpty(periodFrom) && !String.IsNullOrEmpty(periodTo))
            {
                int from = int.Parse(periodFrom);
                int to = int.Parse(periodTo);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo;

                var saleOrderRegisters = _db.ArSalesOrders
                      .Where(x => x.isVoid == false)
                   .Where(x => x.IsOnlineApproved)
                    .Where(x => x.OrderDate >= dt11 && x.OrderDate <= dt22);

                if (orderType != null)
                {
                    if (orderType == "Cash")
                        saleOrderRegisters = saleOrderRegisters.Where(x => x.OrderType == Core.Domain.Finance.AR.SalesOrder.OrderType.Cash);
                    else
                        saleOrderRegisters = saleOrderRegisters.Where(x => x.OrderType == Core.Domain.Finance.AR.SalesOrder.OrderType.Credit);
                }
                if (customer != null)
                {
                    saleOrderRegisters = saleOrderRegisters.Where(x => x.CustomerName == customer);
                }

                foreach (var saleOrderRegister in saleOrderRegisters)
                {
                    var saleOrder = new SalesOrderRegisterViewModel();
                    var broadcasts = _db.ArSalesOrderBroadcastTimes.Where(x => x.ArSalesOrderId == saleOrderRegister.ArSalesOrderId).ToList();
                    foreach (var broadcast in broadcasts)
                    {
                        saleOrder.BroadCastDate = saleOrder.BroadCastDate + " " + broadcast.BroadcastDateTime.ToString();
                    }


                    saleOrder.SaleOrderNo = saleOrderRegister.ArSalesOrderId;
                    saleOrder.SaleOrderDate = saleOrderRegister.OrderDate;                   
                    saleOrder.OrderStatus = saleOrderRegister.OrderStatus.ToString();
                    saleOrder.CustomerName = saleOrderRegister.CustomerName;
                    saleOrder.CustomerType = saleOrderRegister.OrderType.ToString();
                    saleOrder.OrderAmount = saleOrderRegister.ArSalesOrderLines.Sum(x=>x.TotalAmount);

                    salesOrderRegisterList.Add(saleOrder);
                }

            }

            return salesOrderRegisterList;
        }
    }
}
