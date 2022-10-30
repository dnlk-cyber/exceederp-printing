using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.Printing.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using ExceedERP.Core.Domain.Common.HRM;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.CRM.Printing.DataSource
{
    [DataObject]
    public class SalesInventoryDataSource
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
        public List<SalesInventory> GetSalesinvoiceList(string invoiceNumber)
        {
            try
            {
                List<CashSales> CashSalesInvoice = new List<CashSales>();
                SalesInventory SalesInvoiceViewModel;
                List<SalesInventory> StoreIssues = new List<SalesInventory>();

                List<CashSalesInvoice> CashSalesInvoices = new List<CashSalesInvoice>();
                var OrgCustomers = db.OrganizationCustomers.ToList();
                var product = db.Products.ToList();



                string vat = "";
                string tin = "";

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
                    var tinIdentification = db.Identifications.FirstOrDefault(a =>
                            a.Type == Core.Domain.Common.IdentificationType.TIN &&
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company");
                    var companyVat = db.Identifications.FirstOrDefault(a =>
                               a.Type == Core.Domain.Common.IdentificationType.VAT_NUMBER &&
                               a.Reference == organization.MainCompanyId.ToString() &&
                               a.ReferenceType.ToLower() == "company");
                    if (tinIdentification != null)
                    {
                        tin = tinIdentification.IdNumber;
                    }
                    if (companyVat != null)
                    {
                        vat = companyVat.IdNumber;
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

                List<CashSalesInvoice> SalesInvoices = new List<CashSalesInvoice>();

                if (CashSalesInvoices != null)
                {
                    SalesInvoices = CashSalesInvoices;


                    string address = "";

                    if (SalesInvoices.Any())
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
                           
                            string CustomerType = "CASH";
                            int Index = 0;

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
                                catch (Exception e)
                                {
                                    uOM = string.Empty;
                                }

                                var history = db.IssueHistorys.Where(x => x.InvoiceId == sov.CashSalesInvoiceID && x.Remark == l.ThingID.ToString()).ToList();
                                if (OrgCustomer.IsCredit)
                                {
                                    CustomerType = "CREDIT";
                                    Address adr = db.Addresses.Where(x => x.Reference == OrgCustomer.OrganizationCustomerID.ToString() && x.ReferenceType == "Organization_Customer").FirstOrDefault();

                                    if (adr != null)
                                    {
                                        CompanyAddress = adr.Value;
                                    }
                                }


                                if (history.Any())
                                {
                                    decimal RemainingInvoice = decimal.Zero;
                                    if (Index == 0 || Index != l.LineItemID)
                                    {
                                        RemainingInvoice = decimal.Zero;
                                        Index = l.LineItemID;
                                    }

                                    foreach (var h in history.OrderBy(x => x.DateCreated))
                                    {

                                        SalesInvoiceViewModel = new SalesInventory();
                                        SalesInvoiceViewModel.Consignee = sov.OrganizationCustomer.TradeName;
                                        SalesInvoiceViewModel.CustomerType = CustomerType;

                                        SalesInvoiceViewModel.InvoiceNumber = sov.CashSalesInvoiceID;
                                        SalesInvoiceViewModel.OrderDate = DateTime.Now;
                                        SalesInvoiceViewModel.ProductName = l.Product.Name;
                                        SalesInvoiceViewModel.UOM = uOM;
                                        SalesInvoiceViewModel.Store = l.SalesStore.Description;
                                        SalesInvoiceViewModel.IssuedItem = h.Remark;
                                        SalesInvoiceViewModel.IssuedQuantity = h.Amount;
                                        SalesInvoiceViewModel.IssuedDate = h.DateCreated;
                                        SalesInvoiceViewModel.IssueHistoryId = h.IssueHistoryId;
                                        SalesInvoiceViewModel.InvoiceQuantity = (l.CalculatedCost / l.UnitAmount);

                                        SalesInvoiceViewModel.AmharicName = AmharicName;
                                        SalesInvoiceViewModel.Name = Name;
                                        SalesInvoiceViewModel.LogoFileName = LogoFileName;
                                        SalesInvoiceViewModel.Address = Address;
                                        SalesInvoiceViewModel.OnlineApprovedBy = h.CreatedBy;
                                        StoreIssues.Add(SalesInvoiceViewModel);
                                        RemainingInvoice = RemainingInvoice + h.Amount;
                                    }
                                }
                                else
                                {
                                    SalesInvoiceViewModel = new SalesInventory();
                                    SalesInvoiceViewModel.Consignee = sov.OrganizationCustomer.TradeName;
                                    SalesInvoiceViewModel.CustomerAddress = address;
                                    SalesInvoiceViewModel.InvoiceNumber = sov.CashSalesInvoiceID;
                                    SalesInvoiceViewModel.InvoiceQuantity = l.CalculatedCost / l.UnitAmount;
                                    SalesInvoiceViewModel.Store = l.SalesStore.Description;
                                    SalesInvoiceViewModel.OrderNumber = sov.References;
                                    SalesInvoiceViewModel.GrandTotal = sov.GrandTotal;
                                    SalesInvoiceViewModel.InvoiceDate = sov.IssuedDate;
                                    SalesInvoiceViewModel.Status = sov.LastObjectState;
                                    SalesInvoiceViewModel.OrderDate = DateTime.Now;
                                    SalesInvoiceViewModel.ProductName = l.Product.Name;
                                    SalesInvoiceViewModel.UOM = uOM;
                                    SalesInvoiceViewModel.Class = l.Class;
                                    SalesInvoiceViewModel.Variety = l.Variety;
                                    SalesInvoiceViewModel.Tax = l.TaxAmount;
                                    SalesInvoiceViewModel.SubTotoal = l.CalculatedCost;
                                    SalesInvoiceViewModel.GrandTotal = l.TotalAmount;
                                    SalesInvoiceViewModel.OnlineApprovedBy = l.CreatedBy;

                                    SalesInvoiceViewModel.AmharicName = AmharicName;
                                    SalesInvoiceViewModel.Name = Name;
                                    SalesInvoiceViewModel.LogoFileName = LogoFileName;
                                    SalesInvoiceViewModel.Address = Address;

                                    SalesInvoiceViewModel.RequestedBy = sov.CreatedBy;
                                    SalesInvoiceViewModel.CheckedBy = sov.SendForApprovalBy;
                                    SalesInvoiceViewModel.ApprovedBy = sov.OnlineApprovedBy;

                                    StoreIssues.Add(SalesInvoiceViewModel);

                                }
                            }

                        }
                    }

                }
                return StoreIssues.DistinctByField(x => x.IssueHistoryId).ToList();
            }
            catch (Exception e)
            {


            }

            return null;

        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesInventory> GetSalesinvoiceHeader(string invoiceNumber)
        {
            try
            {
                List<CashSales> CashSalesInvoice = new List<CashSales>();
                SalesInventory SalesInvoiceViewModel;
                List<SalesInventory> StoreIssues = new List<SalesInventory>();

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

                List<CashSalesInvoice> SalesInvoices = new List<CashSalesInvoice>();

                if (CashSalesInvoices != null)
                {
                    SalesInvoices = CashSalesInvoices;


                    string address = "";

                    if (SalesInvoices.Any())
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
                        CashSalesInvoice sov = SalesInvoices.FirstOrDefault();

                        string CustomerType = "CASH";
                        int Index = 0;

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
                            catch (Exception e)
                            {
                                uOM = string.Empty;
                            }

                            var history = db.IssueHistorys.Where(x => x.InvoiceId == sov.CashSalesInvoiceID && x.Remark == l.ThingID.ToString()).ToList();
                            if (OrgCustomer.IsCredit)
                            {
                                CustomerType = "CREDIT";
                                Address adr = db.Addresses.Where(x => x.Reference == OrgCustomer.OrganizationCustomerID.ToString() && x.ReferenceType == "Organization_Customer").FirstOrDefault();

                                if (adr != null)
                                {
                                    CompanyAddress = adr.Value;
                                }
                            }



                            SalesInvoiceViewModel = new SalesInventory();
                            SalesInvoiceViewModel.Consignee = sov.OrganizationCustomer.TradeName;
                            SalesInvoiceViewModel.CustomerAddress = address;
                            SalesInvoiceViewModel.OnlineApprovedBy = sov.CreatedBy;

                            SalesInvoiceViewModel.AmharicName = AmharicName;
                            SalesInvoiceViewModel.Name = Name;
                            SalesInvoiceViewModel.LogoFileName = LogoFileName;
                            SalesInvoiceViewModel.Address = Address;

                            StoreIssues.Add(SalesInvoiceViewModel);

                        }
                    }





                    return StoreIssues.ToList();
                }
                else
                {
                    return new List<SalesInventory>();
                }
            }
            catch (Exception e)
            {


            }

            return null;

        }
    }
}
