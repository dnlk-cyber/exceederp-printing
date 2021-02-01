using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace ExceedERP.Reporting.CRM.DataSource
{
    [DataObject]
    public class SalesInvoiceObjectDataSource : List<CashSales>
    {

        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetOrganizationCustomers()
        {
            ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject OrgCustomer = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject();
            List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> Customer = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject>();

            var Customers = db.OrganizationCustomers.ToList();

            foreach (var c in Customers)
            {
                OrgCustomer = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
                {
                    Name = c.TradeName,
                    Id = c.OrganizationCustomerID,
                    Value = c.OrganizationCustomerID.ToString()
                };
                Customer.Add(OrgCustomer);
            }

            return Customer;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CashSales> GetSalesinvoiceList(string invoiceNumber, string customer, DateTime startingDate, DateTime endingDate)
        {
            List<CashSales> CashSalesInvoice = new List<CashSales>();
            CashSales SalesInvoiceViewModel;
            List<CashSalesInvoice> CashSalesInvoices = new List<CashSalesInvoice>();
            var OrgCustomers = db.OrganizationCustomers.ToList();
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


            CashSalesInvoices = db.CashSalesInvoices.ToList();

            if (!string.IsNullOrEmpty(invoiceNumber))
            {
                CashSalesInvoices = CashSalesInvoices.Where(so => so.CashSalesInvoiceID.ToString() == invoiceNumber).ToList();

            }
           else if (!string.IsNullOrEmpty(customer))
            {
                CashSalesInvoices = CashSalesInvoices.Where(so => so.OrganizationCustomerId.ToString() == customer ).ToList();
            }

 

            List<CashSalesInvoice> SalesInvoices = new List<CashSalesInvoice>();

            if (CashSalesInvoices.Any())
            {
                SalesInvoices =
     CashSalesInvoices.Where(x => x.IssuedDate >= startingDate && x.IssuedDate <= endingDate).ToList();


                string address = "";
                string tradeName = "";
                if (SalesInvoices.Count > 0)
                {
                    tradeName = SalesInvoices.FirstOrDefault().OrganizationCustomer.TradeName;

                    var OrgCustomer = OrgCustomers.Where(x => x.TradeName == tradeName).FirstOrDefault();
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

                            address = addresss.Value;
                        }
                    }
                    foreach (CashSalesInvoice sov in SalesInvoices)
                    {
                        List<LineItem> lineItems = db.LineItems.Where(li => li.Voucher.ToString() == sov.CashSalesInvoiceID.ToString() && li.VoucherDefinitionID == "2").ToList();
                        foreach (LineItem l in lineItems)
                        {
                            int uOMID = 0;
                            string uOM = "";

                            try
                            {
                                uOMID = int.Parse(l.Product.UOM);
                                uOM = db.Lookups.Where(x => x.LookupId == uOMID).FirstOrDefault().Description;
                            }
                            catch (Exception)
                            {
                                uOM = string.Empty;
                            }

                            SalesInvoiceViewModel = new CashSales
                            {
                                Consignee = sov.OrganizationCustomer.TradeName,
                                CustomerAddress = address,
                                InvoiceNumber = sov.CashSalesInvoiceID,
                                InvoiceQuantity = l.CalculatedCost / l.UnitAmount,
                                Store = l.SalesStore.Description,
                                OrderNumber = sov.References,
                                GrandTotal = sov.GrandTotal,
                                InvoiceDate = sov.IssuedDate.ToString("dd/MM/yyyy"),
                                Status = sov.LastObjectState,
                                OrderDate = DateTime.Now,
                                ProductName = l.Product.Name,
                                UOM = uOM,
                                Class = l.Class,
                                Variety = l.Variety,
                                SellingPrice = l.UnitAmount,
                                Tax = l.TaxAmount,
                                SubTotoal = l.CalculatedCost
                            };
                            SalesInvoiceViewModel.GrandTotal = l.TotalAmount;

                            SalesInvoiceViewModel.AmharicName = AmharicName;
                            SalesInvoiceViewModel.Name = Name;
                            SalesInvoiceViewModel.LogoFileName = LogoFileName;
                            SalesInvoiceViewModel.Address = Address;

                            CashSalesInvoice.Add(SalesInvoiceViewModel);

                        }

                    }
                }

            }
            return CashSalesInvoice;
        }


    }
}





