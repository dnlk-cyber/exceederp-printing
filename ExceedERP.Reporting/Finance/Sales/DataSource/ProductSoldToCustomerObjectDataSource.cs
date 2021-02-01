using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.Sales.DataSource
{

    [DataObject]
    public class ProductSoldToCustomerObjectDataSource
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProductSoldToCustomerVm> GetProductSoldCustomer(string periodFrom, string periodTo, string customer, string items,string partner,string ordertype)
        {
            List<ProductSoldToCustomerVm> productSoldList = new List<ProductSoldToCustomerVm>();

            if (!String.IsNullOrEmpty(periodFrom) && !String.IsNullOrEmpty(periodTo))
            {
                int from = int.Parse(periodFrom);
                int to = int.Parse(periodTo);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo;

                var salesOrders = _db.ArSalesOrders
                    .Where(x => x.IsOnlineApproved && x.isVoid == false)
                    .Where(x => x.OrderDate >= dt11)
                    .Where(x => x.OrderDate <=dt22).AsQueryable();
               
                if (!String.IsNullOrEmpty(customer))
                {
                    salesOrders = salesOrders.Where(x => x.CustomerName==customer).AsQueryable();
                }

                if (!String.IsNullOrEmpty(partner))
                {
                    salesOrders = salesOrders.Where(x => x.PartnerId.ToString() == partner).AsQueryable();
                }
                if (!String.IsNullOrEmpty(ordertype))
                {
                    if(ordertype=="Cash")
                    salesOrders = salesOrders.Where(x => x.OrderType ==OrderType.Cash).AsQueryable();
                    else if  (ordertype == "Credit")
                        {
                        salesOrders = salesOrders.Where(x => x.OrderType == OrderType.Credit).AsQueryable();
                    }
                }

                foreach (var salesOrder in salesOrders)
                {
                   

                    var saleOrderLine = _db.ArSalesOrderLines.Where(x => x.ArSalesOrderId == salesOrder.ArSalesOrderId).AsQueryable();
                    if (!String.IsNullOrEmpty(items))
                    {
                        saleOrderLine = saleOrderLine.Where(x => x.SubItem == items).AsQueryable();
                    }

                    foreach (var line in saleOrderLine)
                    {
                        var productSold = new ProductSoldToCustomerVm();
                        var productItem = _db.ArProductService.FirstOrDefault(x => x.ArProductServiceId.ToString() == line.SubItem);
                        if (productItem != null)
                        {
                            productSold.ProductCode = productItem.Code;
                            productSold.ProductName = productItem.ItemName;
                        }

                        var partnerObj= _db.Partners.FirstOrDefault(x => x.PartnerId == salesOrder.PartnerId);
                        if (partnerObj != null) productSold.PartnerName = partnerObj.Name;
                        productSold.CustomerId = salesOrder.CustomerId;
                        productSold.CustomerName = salesOrder.CustomerName;
                        productSold.OrderType = salesOrder.OrderType;
                        productSold.OrderNo = salesOrder.ArSalesOrderId;
                        productSold.OrderReference = salesOrder.OrderReference;
                        productSold.OrderDate = salesOrder.OrderDate;
                        productSold.Quantity = line.Quantity;
                        productSold.UnitPrice = line.UnitPrice;
                        productSold.SalesTax = line.TaxAmount;
                        productSold.TotalAmount = line.TotalAmount;

                        productSoldList.Add(productSold);
                    }


                }
            }
            
           

            return productSoldList;
        }
    }
}
