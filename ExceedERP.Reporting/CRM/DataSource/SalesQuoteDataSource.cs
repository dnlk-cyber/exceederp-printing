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
using ExceedERP.Reporting.SharedDataSources;
using System.Drawing;

namespace ExceedERP.Reporting.CRM.DataSource
    {
    [DataObject]
    public class SalesQuoteObjectDataSource : List<QuoteStatus>
        {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<QuoteStatus> GetSalesQuoteList(string salesQuoteNumber, string customer, string status,
            DateTime startingDate, DateTime endingDate)
            {
            List<QuoteStatus> quoteList = new List<QuoteStatus>();
            QuoteStatus salesQuoteViewModel;
            List<SalesQuoteVoucher> SalesQuoteVouchers;
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

            SalesQuoteVouchers =
                       db.SalesQuoteVouchers.ToList();

            if (!string.IsNullOrEmpty(salesQuoteNumber))
           
                        {
                    SalesQuoteVouchers =
                        SalesQuoteVouchers.Where(so => so.SalesQuoteVoucherID.ToString() == salesQuoteNumber)
                            .ToList();
                }
                 if (!string.IsNullOrEmpty(customer))
                {
                    SalesQuoteVouchers =
                        SalesQuoteVouchers.Where(
                            so =>
                                so.OrganizationCustomerId.ToString() == customer  )
                            .ToList();
                }
                
            if (SalesQuoteVouchers.Any())
            {
                List<SalesQuoteVoucher> SalesQuotes =
                      SalesQuoteVouchers.Where(x => x.IssuedDate > startingDate && x.IssuedDate < endingDate).ToList();
                foreach (SalesQuoteVoucher sov in SalesQuotes)
                {
                    List<LineItem> lineItems =
                        db.LineItems.Where(li => li.Voucher.ToString() == sov.SalesQuoteVoucherID.ToString() && li.VoucherDefinitionID=="0").ToList();
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
                                uOM = string.Empty;
                            }
                       
                        salesQuoteViewModel = new QuoteStatus();
                       
                        salesQuoteViewModel.GrandTotal = sov.GrandTotal;

                        salesQuoteViewModel.QuoteNumber = sov.SalesQuoteVoucherID;
                        salesQuoteViewModel.Quantity = l.CalculatedCost / l.UnitAmount;
                        salesQuoteViewModel.Status = sov.LastObjectState;
                        salesQuoteViewModel.Consignee = sov.OrganizationCustomer.TradeName ;
                        salesQuoteViewModel.ProductName = l.Product.Name;
                        salesQuoteViewModel.UOM = uOM;
                        salesQuoteViewModel.Store = l.SalesStore.Description;
                        salesQuoteViewModel.Class = l.Class;
                        salesQuoteViewModel.Variety = l.Variety;
                        salesQuoteViewModel.QuoteDate = sov.IssuedDate;
                        salesQuoteViewModel.Tax = l.TaxAmount;
                        salesQuoteViewModel.SellingPrice = l.UnitAmount;
                        salesQuoteViewModel.SubTotoal = l.CalculatedCost;
                        salesQuoteViewModel.GrandTotal = l.TotalAmount;
                        salesQuoteViewModel.AmharicName = AmharicName;
                        salesQuoteViewModel.Name = Name;
                        salesQuoteViewModel.LogoFileName = LogoFileName;
                        salesQuoteViewModel.Address = Address;

                        quoteList.Add(salesQuoteViewModel);

                    }
                }
            }
            return quoteList;
            }


        }
    }