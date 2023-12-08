using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Reporting.SharedDataSources;
using System.Drawing;
using XAPI;
using ExceedERP.Reporting.CRM.Printing.ViewModel;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.CRM.Printing.DataSource
{
    [DataObject]
    public class SalesInvoiceObjectDataSource
    {

        private readonly ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesInvoicePrintViewMode> GetSalesinvoiceList(string invoiceNumber)
        {
            List<SalesInvoicePrintViewMode> CashSalesInvoice = new List<SalesInvoicePrintViewMode>();
            SalesInvoicePrintViewMode SalesInvoiceViewModel = new SalesInvoicePrintViewMode();
            List<CashSalesInvoice> CashSalesInvoices = new List<CashSalesInvoice>();
            var OrgCustomers = db.OrganizationCustomers.ToList();
            var product = db.Products.ToList();



            string tradeName = "";
            //string CompanyAddress = "";

            string customerAddress = null;

            string AmharicName = "";
            string Name = "";
            //string Address = "";

            Image LogoFileName = null;

            #region Optional company information
            string subCity = "";
            string houseNumber = "";
            string phoneNumber = "";
            string poBox = "";
            string vatRegNumber = "";
            string dateOfRegistration = "";
            string tinNumber = "";

            #endregion
            HashSet<string> storeNames = new HashSet<string>();
            HashSet<string> shopNames = new HashSet<string>();
            HashSet<int> shopAndStoreIds = new HashSet<int>();

            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                dateOfRegistration = organization.DateCreated?.ToString("dd/MM/yyyy");
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();

                var tinIdentification = db.Identifications.FirstOrDefault(a =>
                            a.Type == Core.Domain.Common.IdentificationType.TIN &&
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company");
                var companyVatReg = db.Identifications.FirstOrDefault(a =>
                           a.Type == Core.Domain.Common.IdentificationType.VAT_NUMBER &&
                           a.Reference == organization.MainCompanyId.ToString() &&
                           a.ReferenceType.ToLower() == "company");
                if (tinIdentification != null)
                {
                    tinNumber = tinIdentification.IdNumber;
                }
                if (companyVatReg != null)
                {
                    vatRegNumber = companyVatReg.IdNumber;
                }
                if (address.Any())
                {
                   
                    var subCityAddress = address.FirstOrDefault(a => a.Category == Core.Domain.Common.AddressType.KIFLEKETEMA);
                    var houseNumberAddress = address.FirstOrDefault(a => a.Category == Core.Domain.Common.AddressType.HOUSE_NO);
                    var phoneNumberAddress = address.FirstOrDefault(a => a.Category == Core.Domain.Common.AddressType.OFFICE_PHONE);
                    var poBoxAddress = address.FirstOrDefault(a => a.Category == Core.Domain.Common.AddressType.POBOX);
                    //var vatRegNumber = address.FirstOrDefault(a => a.Category == Core.Domain.Common.AddressType.);
                    //var subCityAddress = address.FirstOrDefault(a => a.Category == Core.Domain.Common.AddressType.KIFLEKETEMA);
                    //var subCityAddress = address.FirstOrDefault(a => a.Category == Core.Domain.Common.AddressType.KIFLEKETEMA);


                    if (subCityAddress != null)
                    {
                        subCity = subCityAddress.Value;
                    }
                    if (houseNumberAddress != null)
                    {
                        houseNumber = houseNumberAddress.Value;
                    }
                    if (phoneNumberAddress != null)
                    {
                        phoneNumber = phoneNumberAddress.Value;
                    }
                    if (poBoxAddress != null)
                    {
                        poBox = poBoxAddress.Value;
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
                List<Address> customerAddresses = db.Addresses.Where(x => x.Reference == OrgCustomer.OrganizationCustomerID.ToString() && x.ReferenceType == "Organization_Customer").ToList();
                if (customerAddresses.Any())
                {
                    foreach (var a in customerAddresses)
                    {
                        if (customerAddress == null)
                        {
                            customerAddress = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            customerAddress += ", " + a.Category + "-" + a.Value;
                        }
                    }
                }
                var OrgCustomerID = OrgCustomer.OrganizationCustomerID;
            }
            foreach (CashSalesInvoice sov in CashSalesInvoices)
            {
                var selectedCustomerName = "";
                var selectedCustomerTin = "";
                var currentCustomer = db.OrganizationCustomers.Find(sov.OrganizationCustomerId);
                if (currentCustomer != null)
                {
                    selectedCustomerName = currentCustomer.TradeName;
                    selectedCustomerTin = currentCustomer.TinNo;
                }
                var paymentTerm = string.Empty;
                var invoicePaymentTerm = db.PaymentTerms.Find(sov.PaymentTermsID);
                //tinNumber = sov.OrganizationCustomer.TinNo;
                //dateOfRegistration = sov.DateCreated?.ToString("dd/MM/yyyy");

                if (invoicePaymentTerm != null)
                {
                    paymentTerm = invoicePaymentTerm.PaymentTermsDays + " days";
                }

                List<LineItem> lineItems = db.LineItems.Where(li => li.Voucher.ToString() == sov.CashSalesInvoiceID.ToString() && li.VoucherDefinitionID == "2").ToList();
                List<ServiceLineItem> serviceLineItems = db.ServiceLineItems.Where(li => li.VoucherId == sov.CashSalesInvoiceID && li.VoucherType == CRMVoucherType.SalesInvoice).ToList();

                decimal totalAmount = 0;
                decimal totalTax = 0;
                decimal totalWithholding = 0;
                decimal VATWithholdingAmount = 0;
                var totalAmountInwords = string.Empty;

                if (lineItems.Any())
                {
                    totalAmount += lineItems.Sum(l => l.TotalAmount);
                    totalTax += lineItems.Sum(l => l.TaxAmount);
                    totalWithholding += lineItems.Sum(l => l.WithholdingAmount);
                    VATWithholdingAmount += lineItems.Sum(l => l.WithholdTaxAmount);
                }
                if (serviceLineItems.Any())
                {
                    totalAmount += serviceLineItems.Sum(l => l.TotalAmount);
                    totalTax += serviceLineItems.Sum(l => l.TaxAmount);
                    totalWithholding += serviceLineItems.Sum(l => l.WithholdingAmount);
                    VATWithholdingAmount += serviceLineItems.Sum(l => l.WithholdTaxAmount);
                }

                totalAmountInwords = NumberExtensions.toWords((int)totalAmount) + " birr ";
                var afterDecimal = totalAmount - Math.Floor(totalAmount);
                var roundedAfterDecimal = (int)Math.Round(afterDecimal * 100);

                if (roundedAfterDecimal > 0 && roundedAfterDecimal < 100)
                {
                    totalAmountInwords += " and " + NumberExtensions.toWords(roundedAfterDecimal) + " cents";
                }
                var branchName = "";
                var invoiceBranch = db.Branch.FirstOrDefault(b => b.BranchId == sov.BranchId);
                if (invoiceBranch != null)
                {
                    branchName = invoiceBranch.Name;
                }

                string CustomerType = "CASH";

                if (OrgCustomer.IsCredit)
                {
                    CustomerType = "CREDIT";
                }
                var store = db.SalesStores.Find(sov.StoreID);
                SalesInvoiceViewModel = new SalesInvoicePrintViewMode();

                SalesInvoiceViewModel.Consignee = selectedCustomerName;
                SalesInvoiceViewModel.Branch = branchName;
                SalesInvoiceViewModel.CustomerType = CustomerType;
                SalesInvoiceViewModel.CustomerAddress = customerAddress;
                SalesInvoiceViewModel.CustomerTinNumber = selectedCustomerTin;
                SalesInvoiceViewModel.InvoiceNumber = sov.CashSalesInvoiceID;
                SalesInvoiceViewModel.Name = sov.RecieveMethod;
                SalesInvoiceViewModel.PaymentTerm = paymentTerm;

                SalesInvoiceViewModel.Remark = sov.PosPrintType == PosPrintType.CRM ? sov.Remark : "";

                SalesInvoiceViewModel.Store = store?.Description;
                SalesInvoiceViewModel.OrderNumber = sov.Source == SalesInvoiceSource.SalesOrder ? "Sales Order # " + sov.References : sov.References;
                SalesInvoiceViewModel.ReceiptReference = sov.ReceiptReference;
                SalesInvoiceViewModel.FsInvoiceNumber = sov.FsInvoiceNo;
                SalesInvoiceViewModel.SalesPerson = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(sov.CreatedBy));
                SalesInvoiceViewModel.PaymentMethod = sov.PaymentMethod;
                SalesInvoiceViewModel.InvoiceDate = sov.IssuedDate.ToString("dd/MM/yyyy");
                SalesInvoiceViewModel.Status = sov.LastObjectState;
                SalesInvoiceViewModel.OrderDate = DateTime.Now;

                SalesInvoiceViewModel.TotalAmount = totalAmount;
                SalesInvoiceViewModel.TotalTax = totalTax;
                SalesInvoiceViewModel.TotalWithholding = totalWithholding;
                SalesInvoiceViewModel.NetPay = totalAmount - totalTax - totalWithholding;
                SalesInvoiceViewModel.TotalAmountInWords = totalAmountInwords;
                SalesInvoiceViewModel.OnlineApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(sov.OnlineApprovedBy));
                SalesInvoiceViewModel.SendForApprovalBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(sov.SendForApprovalBy));
                SalesInvoiceViewModel.SendForApprovalTime = sov.SendForApprovalTime;
                SalesInvoiceViewModel.OnlineApprovedTime = sov.OnlineApprovedTime;
                SalesInvoiceViewModel.PrintStatus = sov.IsReceiptPrinted ? "Printed" : "Not Printed";

                SalesInvoiceViewModel.AmharicName = AmharicName;
                SalesInvoiceViewModel.Name = Name;
                SalesInvoiceViewModel.LogoFileName = LogoFileName;

                SalesInvoiceViewModel.SubCity = subCity;
                SalesInvoiceViewModel.HouseNumber = houseNumber;
                SalesInvoiceViewModel.PhoneNumber = phoneNumber;
                SalesInvoiceViewModel.PoBox = poBox;
                SalesInvoiceViewModel.DateOfRegistration = dateOfRegistration;
                SalesInvoiceViewModel.VatRegNumber = vatRegNumber;
                SalesInvoiceViewModel.TinNumber = tinNumber;

                SalesInvoiceViewModel.RequestedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(sov.CreatedBy));
                SalesInvoiceViewModel.CheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(sov.SendForApprovalBy));
                SalesInvoiceViewModel.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(sov.OnlineApprovedBy));

                SalesInvoiceViewModel.TotalAmount = totalAmount;
                SalesInvoiceViewModel.TotalTax = totalTax;
                SalesInvoiceViewModel.TotalWithholding = totalWithholding;
                SalesInvoiceViewModel.TotalVATWithholdingAmount = VATWithholdingAmount;
                SalesInvoiceViewModel.NetPay = totalAmount - VATWithholdingAmount - totalWithholding;
                string netPayDisplay = "Net Pay";
                if (totalWithholding > 0 && VATWithholdingAmount > 0)
                {
                    netPayDisplay = "Net Pay (Less W tax & VAT)";
                }
                else if (totalWithholding > 0 && VATWithholdingAmount == 0)
                {
                    netPayDisplay = "Net Pay (Less W tax)";
                }
                else if (totalWithholding == 0 && VATWithholdingAmount > 0)
                {
                    netPayDisplay = "Net Pay (Less VAT)";
                }
                SalesInvoiceViewModel.NetPayDisplayText = netPayDisplay;

                foreach (LineItem l in lineItems)
                {
                    var lineItem = new SalesInvoicePrintLineItemViewModel
                    {
                        ProductName = $"{l.Product.Name} {l.Product.ItemCode}",
                        UOM = l.Product.UOM,
                        Quantity = l.Quantity,
                        SellingPrice = l.UnitAmount,
                        SubTotoal = l.CalculatedCost,
                        TaxAmount = l.TaxAmount,
                        WithholdingAmount = l.WithholdingAmount,
                        TotalAmount = l.TotalAmount
                    };
                    SalesInvoiceViewModel.LineItemList.Add(lineItem);
                }
                foreach (var service in serviceLineItems)
                {
                    var serviceRecord = new SalesInvoicePrintServiceLineItemViewModel
                    {
                        ServiceName = GetServiceName(service.ServiceTypeId),
                        UnitAmount = service.UnitAmount,
                        SubTotoal = service.SubTotal,
                        TaxAmount = service.TaxAmount,
                        WithholdingAmount = service.WithholdingAmount,
                        TotalAmount = service.TotalAmount
                    };
                    SalesInvoiceViewModel.ServiceLineItemList.Add(serviceRecord);
                }
                CashSalesInvoice.Add(SalesInvoiceViewModel);
            }
            return CashSalesInvoice;
        }
        private string GetServiceName(string id)
        {
            var name = "";
            var ser = db.RepairType.FirstOrDefault(l => l.RepairTypeID.ToString() == id);
            if (ser != null)
            {
                name = ser.Name;
            }
            return name;
        }

    }
}





