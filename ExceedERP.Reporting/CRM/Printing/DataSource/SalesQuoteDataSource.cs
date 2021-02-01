using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.ViewModel;
using ExceedERP.Core.Domain.CRM;
using System.Drawing;
using ExceedERP.Reporting.SharedDataSources;


namespace ExceedERP.Reporting.CRM.Printing.DataSource
    {
    [DataObject]
    public class SalesQuoteObjectDataSource : List<QuoteStatus>
        {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<QuoteStatus> GetSalesQuoteList(string salesQuoteNumber)
            {
            List<QuoteStatus> quoteList = new List<QuoteStatus>();
            QuoteStatus salesQuoteViewModel;
            List<SalesQuoteVoucher> SalesQuoteVouchers=new List<SalesQuoteVoucher>();
            var product = db.Products.ToList();
             var orgCustomer = db.OrganizationCustomers.ToList();
             string customeraddress = "";
             string tradeName = "";


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




            if (!string.IsNullOrEmpty(salesQuoteNumber))
                {
                           SalesQuoteVouchers =
                                db.SalesQuoteVouchers.Where(so => so.SalesQuoteVoucherID.ToString() == salesQuoteNumber)
                                    .ToList();
                        }
            if (SalesQuoteVouchers.Count >0)
            
                      
                tradeName = SalesQuoteVouchers.FirstOrDefault().OrganizationCustomer.TradeName ;
            
            var OrgCustomer = orgCustomer.Where(x => x.TradeName == tradeName).FirstOrDefault();
            if (OrgCustomer != null)
            {
                var OrgCustomerID = OrgCustomer.OrganizationCustomerID;
                var addresss =
                    db.Addresses.ToList()
                        .Where(
                            x =>
                                x.Reference == OrgCustomerID.ToString() &&
                                x.ReferenceType == "Organization_Customer")
                        .SingleOrDefault();
                if (addresss != null)
                {

                    customeraddress = addresss.Value;
                }
            }
          foreach (SalesQuoteVoucher sov in SalesQuoteVouchers)
                {
                    List<LineItem> lineItems =
                        db.LineItems.Where(li => li.Voucher.ToString() == sov.SalesQuoteVoucherID.ToString() && li.VoucherDefinitionID=="0").ToList();
                    foreach (LineItem l in lineItems)
                        {
                            int uOMID = 0;
                            string CustomerType = "CASH";
                            string uOM = "";
                            var Store = db.SalesStores.Where(x => x.SalesStoreId == l.StoreID).FirstOrDefault();
                            var Product = product.Where(x => x.ProductID == l.ThingID).FirstOrDefault();
                            if (Product != null)
                            {
                                try
                                {
                                    uOMID = int.Parse(Product.UOM);
                                    uOM = db.Lookups.Where(x => x.LookupId == uOMID).FirstOrDefault().Description;
                                }
                                catch (Exception e)
                                {
                                    uOM = uOM = Product.UOM;
                                }
                            }
                            if (OrgCustomer.IsCredit)
                            {
                                CustomerType = "CREDIT";
                            }
                            salesQuoteViewModel = new QuoteStatus();
                              salesQuoteViewModel.CustomerType = CustomerType;
                  
                            salesQuoteViewModel.CustomerAddress = customeraddress;
                            salesQuoteViewModel.GrandTotal = sov.GrandTotal;
                        
                            salesQuoteViewModel.QuoteNumber = sov.SalesQuoteVoucherID;
                            salesQuoteViewModel.Quantity = l.CalculatedCost / l.UnitAmount;
                            salesQuoteViewModel.Status = sov.LastObjectState;
                            salesQuoteViewModel.Consignee = sov.OrganizationCustomer.TradeName;
                            salesQuoteViewModel.ProductName = Product.Name;
                            salesQuoteViewModel.UOM = uOM;
                            salesQuoteViewModel.Class = l.Class;
                            salesQuoteViewModel.Variety=l.Product.ItemCode;
                            salesQuoteViewModel.SalesPerson = sov.CreatedBy;
                            salesQuoteViewModel.PaymentMethod = sov.PaymentMethod;
                            salesQuoteViewModel.Variety = l.Product.ItemCode;
                            salesQuoteViewModel.Tax = l.TaxAmount;
                            salesQuoteViewModel.QuoteDate = sov.IssuedDate;
                            salesQuoteViewModel.SellingPrice = l.UnitAmount;
                            salesQuoteViewModel.SubTotoal = l.CalculatedCost;
                            salesQuoteViewModel.Store = Store.Description;
                            salesQuoteViewModel.GrandTotal = l.TotalAmount;


                            salesQuoteViewModel.AmharicName = AmharicName;
                            salesQuoteViewModel.Name = Name;
                            salesQuoteViewModel.Address = Address;
                            salesQuoteViewModel.LogoFileName = LogoFileName;

                    salesQuoteViewModel.OnlineApprovedBy = sov.OnlineApprovedBy;
                    salesQuoteViewModel.SendForApprovalBy = sov.SendForApprovalBy;
                    salesQuoteViewModel.SendForApprovalTime = sov.SendForApprovalTime;
                    salesQuoteViewModel.OnlineApprovedTime = sov.OnlineApprovedTime;


                    salesQuoteViewModel.RequestedBy = sov.CreatedBy;
                    salesQuoteViewModel.CheckedBy = sov.OnlineApprovedBy;
                    salesQuoteViewModel.ApprovedBy = sov.OnlineApprovedBy;

                    quoteList.Add(salesQuoteViewModel);

                        }
                }
            return quoteList;
            }


        }
    }