using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.ViewModel;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Reporting.SharedDataSources;
using System.Drawing;

namespace ExceedERP.Reporting.CRM.Printing.DataSource
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
                OrgCustomer = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject();
                OrgCustomer.Name = c.TradeName;
                OrgCustomer.Id = c.OrganizationCustomerID;
                Customer.Add(OrgCustomer);
            }

            return Customer;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetStatus()
        {
            ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject Status = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject();
            List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> Statues = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject>();

            Status.Name = "UNFORWARDED";
            Status.Id = 1;
            Statues.Add(Status);


            Status.Name = "FORWARDED";
            Status.Id = 2;
            Statues.Add(Status);

            return Statues;
        }



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CashSales> GetSalesinvoiceList(string invoiceNumber)
        {
            List<CashSales> CashSalesInvoice = new List<CashSales>();
            CashSales SalesInvoiceViewModel;
            List<CashSalesInvoice> CashSalesInvoices = new List<CashSalesInvoice>();
            var OrgCustomers = db.OrganizationCustomers.ToList();
            var product = db.Products.ToList();



            string tradeName = "";
            string CompanyAddress = "";

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



            if (!string.IsNullOrEmpty(invoiceNumber))
            {

                CashSalesInvoices = db.CashSalesInvoices.Where(so => so.CashSalesInvoiceID.ToString() == invoiceNumber).ToList();

            }




            if (CashSalesInvoices.Any())
            {
                tradeName = CashSalesInvoices.FirstOrDefault().OrganizationCustomer.TradeName;
            }
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

                    CompanyAddress = addresss.Value;
                }
            }
            foreach (CashSalesInvoice sov in CashSalesInvoices)
            {
                List<LineItem> lineItems = db.LineItems.Where(li => li.Voucher.ToString() == sov.CashSalesInvoiceID.ToString() && li.VoucherDefinitionID == "2").ToList();
                foreach (LineItem l in lineItems)
                {
                    int uOMID = 0;
                    string CustomerType = "CASH";
                    string uOM = "";
                  
                        try
                        {
                            uOMID = int.Parse(l.Product.UOM);
                            uOM = db.Lookups.Where(x => x.LookupId == uOMID).FirstOrDefault().Description;
                        }
                        catch(Exception e)
                        {
                            uOM = string.Empty;
                        }
                    
                    if (OrgCustomer.IsCredit)
                    {
                       
                        CustomerType = "CREDIT";
                        Address adr = db.Addresses.Where(x => x.Reference == OrgCustomer.OrganizationCustomerID.ToString() && x.ReferenceType == "Organization_Customer").FirstOrDefault();
                  
                        if(adr != null)
                        {
                            Address = adr.Value;
                        }
                    }

                    SalesInvoiceViewModel = new CashSales();
                    SalesInvoiceViewModel.Consignee = sov.OrganizationCustomer.TradeName;
                    SalesInvoiceViewModel.CustomerType = CustomerType;
                    SalesInvoiceViewModel.CustomerAddress = Address;
                    SalesInvoiceViewModel.InvoiceNumber = sov.CashSalesInvoiceID;
                    SalesInvoiceViewModel.Name = sov.RecieveMethod;

                    SalesInvoiceViewModel.InvoiceQuantity = l.CalculatedCost / l.UnitAmount;
                    SalesInvoiceViewModel.Store = l.SalesStore.Description;
                    SalesInvoiceViewModel.OrderNumber = sov.References;
                    SalesInvoiceViewModel.GrandTotal = sov.GrandTotal;
                    SalesInvoiceViewModel.Variety = l.Product.ItemCode;
                    SalesInvoiceViewModel.SalesPerson = sov.CreatedBy;
                    SalesInvoiceViewModel.PaymentMethod = sov.PaymentMethod;
                     SalesInvoiceViewModel.InvoiceDate = sov.IssuedDate.ToString("dd/MM/yyyy");
                    SalesInvoiceViewModel.Status = sov.LastObjectState;
                    SalesInvoiceViewModel.OrderDate = DateTime.Now;
                    SalesInvoiceViewModel.ProductName = l.Product.Name;
                    SalesInvoiceViewModel.UOM = uOM;
                      SalesInvoiceViewModel.Class = l.Class;
                    SalesInvoiceViewModel.Variety = l.Variety;
                  
                     SalesInvoiceViewModel.SellingPrice = l.UnitAmount ;
                    SalesInvoiceViewModel.Tax = l.TaxAmount;
                    SalesInvoiceViewModel.SubTotoal = l.CalculatedCost;
                    SalesInvoiceViewModel.GrandTotal = l.TotalAmount;
                    SalesInvoiceViewModel.OnlineApprovedBy = sov.OnlineApprovedBy;
                    SalesInvoiceViewModel.SendForApprovalBy = sov.SendForApprovalBy;
                    SalesInvoiceViewModel.SendForApprovalTime = sov.SendForApprovalTime;
                    SalesInvoiceViewModel.OnlineApprovedTime = sov.OnlineApprovedTime;

                    SalesInvoiceViewModel.AmharicName = AmharicName;
                    SalesInvoiceViewModel.Name = Name;
                    SalesInvoiceViewModel.Address = Address;
                    SalesInvoiceViewModel.LogoFileName = LogoFileName;


                    SalesInvoiceViewModel.RequestedBy = sov.CreatedBy;
                    SalesInvoiceViewModel.CheckedBy = sov.SendForApprovalBy;
                    SalesInvoiceViewModel.ApprovedBy = sov.OnlineApprovedBy;


                    CashSalesInvoice.Add(SalesInvoiceViewModel);

                }
            }
            return CashSalesInvoice;
        }


    }
}





