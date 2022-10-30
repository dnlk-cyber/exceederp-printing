using ExceedERP.DataAccess.Context;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Reporting.CRM.ViewModel;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Reporting.SharedDataSources;
using System.Drawing;

namespace ExceedERP.Reporting.CRM.DataSource
{
    [DataObject]
    public class SalesOrderObjectDataSource : List<OrderStatus>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<OrderStatus> GetSalesOrderList(string salesOrderNumber, string customer, DateTime startingDate, DateTime endingDate)
        {
            List<OrderStatus> salesOrderViewList = new List<OrderStatus>();
            OrderStatus SalesOrderViewModel;
            List<SalesOrderVoucher> SalesOrderVouchers;
            var product = db.Products.ToList();



            string AmharicName = "";
            string Name = "";
            string Address = "";
            Image LogoFileName = null;
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {

                        if (Address == null)
                        {
                            Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }


            SalesOrderVouchers = db.SalesOrderVouchers.ToList();

            if (!string.IsNullOrEmpty(salesOrderNumber))
             
                {
                    SalesOrderVouchers = SalesOrderVouchers.Where(so => so.SalesOrderVoucherID.ToString() == salesOrderNumber).ToList();
                }
                if (!string.IsNullOrEmpty(customer))
                {
                    SalesOrderVouchers = SalesOrderVouchers.Where(so => so.OrganizationCustomer.OrganizationCustomerID.ToString() == customer  ).ToList();
                }
               
           
     
          

              List<SalesOrderVoucher> SalesOrders =
                 SalesOrderVouchers.Where(x => x.IssuedDate >= startingDate && x.IssuedDate <= endingDate).ToList();
              if (SalesOrders.Any())
              {
                  foreach (SalesOrderVoucher sov in SalesOrders)
                  {
                      List<LineItem> lineItems = db.LineItems.Where(li => li.Voucher == sov.SalesOrderVoucherID.ToString() && li.VoucherDefinitionID == "1").ToList();
                      foreach (LineItem l in lineItems)
                      {

                          
                             
                          SalesOrderViewModel = new OrderStatus();
                          SalesOrderViewModel.Consignee = sov.OrganizationCustomer.TradeName;
                          SalesOrderViewModel.OrderNumber = sov.SalesOrderVoucherID.ToString();
                          SalesOrderViewModel.GrandTotal = sov.GrandTotal;
                          SalesOrderViewModel.OrderDate = sov.IssuedDate;
                          SalesOrderViewModel.UOM = l.Product.UOM;
                          SalesOrderViewModel.QuoteNumber = sov.References;
                          SalesOrderViewModel.QuoteQuantity = l.CalculatedCost / l.UnitAmount ;
                          SalesOrderViewModel.Status = sov.LastObjectState;
                          SalesOrderViewModel.ProductName = l.Product.Name;
                          SalesOrderViewModel.Class = l.Class;
                          SalesOrderViewModel.Variety = l.Variety;

                          SalesOrderViewModel.Store = l.SalesStore.Description;
                          SalesOrderViewModel.OrderQuantity = l.Quantity;
                          SalesOrderViewModel.SellingPrice = l.UnitAmount;
                          SalesOrderViewModel.Tax = l.TaxAmount;
                          SalesOrderViewModel.SubTotoal = l.CalculatedCost;
                          SalesOrderViewModel.GrandTotal = l.TotalAmount;
                          SalesOrderViewModel.AmharicName = AmharicName;
                          SalesOrderViewModel.Name = Name;
                          SalesOrderViewModel.LogoFileName = LogoFileName;
                          SalesOrderViewModel.Address = Address;

                          salesOrderViewList.Add(SalesOrderViewModel);

                      }
                  }
              }
            return salesOrderViewList;
        }


    }
}







