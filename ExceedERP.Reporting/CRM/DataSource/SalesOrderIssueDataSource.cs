 using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.CRM.ViewModel;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Reporting.SharedDataSources;
using System.Drawing;

namespace ExceedERP.Reporting.CRM.DataSource
{
    [DataObject]
    public class SalesOrderIssueDataSource : List<OrderStatus>
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



            if (!string.IsNullOrEmpty(salesOrderNumber))
            {
                if (!string.IsNullOrEmpty(customer))
                {
                    SalesOrderVouchers = db.SalesOrderVouchers.Where(so => so.OrganizationCustomer.TradeName == customer && so.SalesOrderVoucherID.ToString() == salesOrderNumber).ToList();
                }
                else
                {
                    SalesOrderVouchers = db.SalesOrderVouchers.Where(so => so.SalesOrderVoucherID.ToString() == salesOrderNumber).ToList();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(customer))
                {
                    SalesOrderVouchers = db.SalesOrderVouchers.Where(so => so.OrganizationCustomerId.ToString() == customer).ToList();
                }
                else
                {
                    SalesOrderVouchers = db.SalesOrderVouchers.ToList();
                }
            }
          

              List<SalesOrderVoucher> SalesOrders =
                 SalesOrderVouchers.Where(x => x.IssuedDate > startingDate && x.IssuedDate < endingDate).ToList();
              if (SalesOrders.Any())
              {
                  foreach (SalesOrderVoucher sov in SalesOrders)
                  {
                      List<LineItem> lineItems = db.LineItems.Where(li => li.Voucher == sov.SalesOrderVoucherID.ToString() && li.VoucherDefinitionID == "1").ToList();
                      var Invoice = db.CashSalesInvoices.Where(x => x.References == sov.SalesOrderVoucherID.ToString()).FirstOrDefault();

                      decimal IssueAmount = decimal.Zero;

                      if (Invoice != null)
                      {
                          var Issues = db.IssueHistorys.Where(x => x.InvoiceId == Invoice.CashSalesInvoiceID).ToList();
                          IssueAmount = Issues.Sum(x => x.Amount);
                      }
                      foreach (LineItem l in lineItems)
                      {

                          int uOMID = 0;
                          string uOM = "";
                         
                              try
                              {
                                  uOMID = int.Parse(l.Product.UOM);
                                  uOM = db.Lookups.Where(x => x.LookupId == uOMID).FirstOrDefault().Description;
                              }
                              catch (Exception e)
                              {
                                  uOM =string.Empty;
                              }
                          
                          SalesOrderViewModel = new OrderStatus();
                          SalesOrderViewModel.Consignee = sov.OrganizationCustomer.TradeName;
                          SalesOrderViewModel.CustomerType = sov.OrganizationCustomer.CustomerType.ToString();
                          SalesOrderViewModel.OrderNumber = sov.SalesOrderVoucherID.ToString();
                          SalesOrderViewModel.GrandTotal = sov.GrandTotal;
                          SalesOrderViewModel.OrderDate = sov.IssuedDate;
                          SalesOrderViewModel.UOM = uOM;
                          SalesOrderViewModel.QuoteNumber = sov.References;
                          SalesOrderViewModel.QuoteQuantity = l.CalculatedCost / l.UnitAmount ;
                          SalesOrderViewModel.Status = sov.LastObjectState;
                          SalesOrderViewModel.ProductName = l.Product.Name;
                          SalesOrderViewModel.Class = l.Class;
                          SalesOrderViewModel.Variety = l.Variety;
                          SalesOrderViewModel.IssuedAmount = IssueAmount;
                          SalesOrderViewModel.RemainingAmount = lineItems.Sum(x=>x.Quantity) - IssueAmount;
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







