using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.Sales
{

    [DataObject]
    public class SaleInvoiceObjectDataSource
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SaleInvoiceVM> GetSaleInvoice(string periodFrom, string periodTo, string customer,string product, string items)
        {
            List<SaleInvoiceVM> saleInvoiceList = new List<SaleInvoiceVM>();

            if (!String.IsNullOrEmpty(periodFrom) && !String.IsNullOrEmpty(periodTo))
            {
                int from = int.Parse(periodFrom);
                int to = int.Parse(periodTo);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo;


                var invoices = _db.ARInvoices.Where(x => x.ArSalesOrderId !=0)
                    .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Where(x => x.Date >= dt11)
                    .Where(x => x.Date <=dt22).AsQueryable();

                

                if (!String.IsNullOrEmpty(customer))
                {
                    invoices = invoices.Where(x => x.CustomerName == customer).AsQueryable();
                }

                foreach (var arInvoice in invoices)
                {
                    
                    var order = _db.ArSalesOrders.Find(arInvoice.ArSalesOrderId);
                    var arInvoiceLineses = _db.ARInvoiceLines.Where(x => x.ArInvoiceId == arInvoice.ArInvoiceId).AsQueryable();
                    if (!String.IsNullOrEmpty(product))
                    {
                        var myproduct = _db.ARSetupProductServiceAccounts.Find(int.Parse(product));
                        arInvoiceLineses = arInvoiceLineses.Where(x => x.Items == myproduct.Product).AsQueryable();
                    }

                    if (!String.IsNullOrEmpty(items))
                    {
                        var myproductitem = _db.ArProductService.Find(int.Parse(items));
                        arInvoiceLineses = arInvoiceLineses.Where(x => x.Description == myproductitem.ItemName).AsQueryable();
                    }

                    foreach (var line in arInvoiceLineses)
                    {
                        var saleInvoice = new SaleInvoiceVM();
                        saleInvoice.Reference = line.ArInvoice.Reference;
                        if (order != null) saleInvoice.OrderDate = order.OrderDate;

                        var orderline = _db.ArSalesOrderLines.Where(x => x.ArSalesOrderId == arInvoice.ArSalesOrderId)
                            .FirstOrDefault(x => x.SubItem == line.Items);

                        var productItem = _db.ArProductService.FirstOrDefault(x => x.ItemName == line.Description);

                        saleInvoice.CustomerName = arInvoice.CustomerName;
                        saleInvoice.CustomerId = arInvoice.CustomerId;
                        if (productItem != null)saleInvoice.ProductCode = productItem.Code;
                        saleInvoice.InvoiceNo = arInvoice.ArInvoiceId;
                        saleInvoice.InvoiceDate = arInvoice.Date;
                        saleInvoice.OrderNo = arInvoice.ArSalesOrderId;
                        if (orderline != null) saleInvoice.OrderQuantity = orderline.Quantity;
                        saleInvoice.ProductName = line.Items;
                        saleInvoice.Quantity = line.Quantity;
                        saleInvoice.UnitPrice = line.UnitPrice;
                        saleInvoice.SalesTax = line.TaxAmount;
                        saleInvoice.TotalAmount = line.TotalAmount;

                        saleInvoiceList.Add(saleInvoice);
                    }
                }
            }

            return saleInvoiceList;
        }
    }
}
