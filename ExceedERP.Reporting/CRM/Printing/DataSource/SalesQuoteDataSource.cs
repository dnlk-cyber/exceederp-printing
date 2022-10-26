using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.Printing.ViewModel;
using ExceedERP.Reporting.CRM.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using XAPI;

namespace ExceedERP.Reporting.CRM.Printing.DataSource
{
    [DataObject]
    public class SalesQuoteObjectDataSource : List<QuotePrintViewModel>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<QuotePrintViewModel> GetSalesQuoteList(string salesQuoteNumber)
        {
            List<QuotePrintViewModel> quoteList = new List<QuotePrintViewModel>();
            QuotePrintViewModel salesQuoteViewModel;
            List<SalesQuoteVoucher> SalesQuoteVouchers = new List<SalesQuoteVoucher>();
            var product = db.Products.ToList();
            var orgCustomer = db.OrganizationCustomers.ToList();
            string customeraddress = "";
            string tradeName = "";

            string vat = "";
            string tin = "";


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

            if (!string.IsNullOrEmpty(salesQuoteNumber))
            {
                SalesQuoteVouchers =
                     db.SalesQuoteVouchers.Where(so => so.SalesQuoteVoucherID.ToString() == salesQuoteNumber)
                         .ToList();
            }
            if (SalesQuoteVouchers.Count > 0)

                tradeName = SalesQuoteVouchers.FirstOrDefault().OrganizationCustomer.TradeName;

            var OrgCustomer = orgCustomer.Where(x => x.TradeName == tradeName).FirstOrDefault();
            if (OrgCustomer != null)
            {
                var OrgCustomerID = OrgCustomer.OrganizationCustomerID;

            }
            foreach (SalesQuoteVoucher sov in SalesQuoteVouchers)
            {
                //var equipmentInfo = db.EquipmentInformation.FirstOrDefault(e => e.CustomerId == sov.OrganizationCustomerId && e.LicensePlate == sov.PlateNo);
                var acc = db.BankAccounts.Find(sov.BankAccountId);
                string accNo = acc != null ? $"{acc.AccountNumber} { acc.AccountName} {acc.BankBranch} Branch" : "";

                var lineItems =
                     db.LineItems.Where(li => li.Voucher.ToString() == sov.SalesQuoteVoucherID.ToString() && li.VoucherDefinitionID == "0").ToList();
                var serviceLineItems = db.ServiceLineItems.Where(l => l.VoucherId == sov.SalesQuoteVoucherID && l.VoucherType == CRMVoucherType.SalesQuote).ToList();
                decimal subTotal = 0;
                decimal totalAmount = 0;
                decimal totalTax = 0;
                decimal totalWithholding = 0;
                decimal VATWithholdingAmount = 0;
                string totalAmountInwords = string.Empty;
                if (lineItems.Any())
                {
                    totalAmount += lineItems.Sum(l => l.TotalAmount);
                    totalTax += lineItems.Sum(l => l.TaxAmount);
                    totalWithholding += lineItems.Sum(l => l.WithholdingAmount);
                    subTotal += lineItems.Sum(l => l.CalculatedCost);
                    VATWithholdingAmount += lineItems.Sum(l => l.WithholdTaxAmount);

                }
                if (serviceLineItems.Any())
                {
                    totalAmount += serviceLineItems.Sum(l => l.TotalAmount);
                    totalTax += serviceLineItems.Sum(l => l.TaxAmount);
                    totalWithholding += serviceLineItems.Sum(l => l.WithholdingAmount);
                    subTotal += serviceLineItems.Sum(l => l.SubTotal);
                    VATWithholdingAmount += serviceLineItems.Sum(l => l.WithholdTaxAmount);
                }

                //totalAmount = (int)lineItems.Sum(l => l.TotalAmount);

                totalAmountInwords = NumberExtensions.toWords((int)totalAmount) + " birr ";
                var afterDecimal = totalAmount - Math.Floor(totalAmount);
                var roundedAfterDecimal = (int)Math.Round(afterDecimal * 100);

                if (roundedAfterDecimal > 0 && roundedAfterDecimal < 100)
                {
                    totalAmountInwords += " and " + NumberExtensions.toWords(roundedAfterDecimal) + " cents";
                }
                #region create sales quote
                string CustomerType = "CASH";
                var Store = db.SalesStores.Where(x => x.SalesStoreId == sov.StoreID).FirstOrDefault();

                if (OrgCustomer.IsCredit)
                {
                    CustomerType = "CREDIT";
                }
                salesQuoteViewModel = new QuotePrintViewModel();
                salesQuoteViewModel.CustomerType = CustomerType;

                salesQuoteViewModel.BanckAccount = accNo;
                salesQuoteViewModel.PlateNo = sov.PlateNo;
                //salesQuoteViewModel.Equipment = equipmentInfo?.GroupName;
                salesQuoteViewModel.TINNo = tin;
                salesQuoteViewModel.VATNo = vat;

                salesQuoteViewModel.CustomerAddress = customeraddress;
                salesQuoteViewModel.GrandTotal = sov.GrandTotal;
                salesQuoteViewModel.DueDate = sov.DueDate;

                salesQuoteViewModel.QuoteNumber = sov.SalesQuoteVoucherID;
                salesQuoteViewModel.Status = sov.LastObjectState;
                salesQuoteViewModel.Consignee = sov.OrganizationCustomer.TradeName;
                salesQuoteViewModel.SalesPerson = sov.CreatedBy;
                salesQuoteViewModel.PaymentMethod = sov.PaymentMethod;
                salesQuoteViewModel.QuoteDate = sov.IssuedDate;
                salesQuoteViewModel.Store = Store.Description;
                salesQuoteViewModel.GrandTotal = totalAmount;

                salesQuoteViewModel.AmharicName = AmharicName;
                salesQuoteViewModel.Name = Name;
                salesQuoteViewModel.Address = Address;
                salesQuoteViewModel.LogoFileName = LogoFileName;

                salesQuoteViewModel.OnlineApprovedBy = sov.OnlineApprovedBy;
                salesQuoteViewModel.SendForApprovalBy = sov.SendForApprovalBy;
                salesQuoteViewModel.SendForApprovalTime = sov.SendForApprovalTime;
                salesQuoteViewModel.OnlineApprovedTime = sov.OnlineApprovedTime;

                salesQuoteViewModel.CheckedTime = sov.MiddleOnlineApprovedTime;

                salesQuoteViewModel.RequestedBy = sov.CreatedBy;
                salesQuoteViewModel.CheckedBy = sov.MiddleOnlineApprovedBy;
                salesQuoteViewModel.ApprovedBy = sov.OnlineApprovedBy;


                salesQuoteViewModel.SubTotal = subTotal;
                salesQuoteViewModel.TotalAmountInWords = totalAmountInwords;
                salesQuoteViewModel.TotalTax = totalTax;
                salesQuoteViewModel.TotalWithholding = totalWithholding;
                salesQuoteViewModel.TotalVATWithholdingAmount = VATWithholdingAmount;
                salesQuoteViewModel.NetPay = totalAmount - VATWithholdingAmount - totalWithholding;
                string netPayDisplay = "Net Pay";
                if(totalWithholding > 0 && VATWithholdingAmount > 0)
                {
                    netPayDisplay = "Net Pay (Less W tax & VAT)";
                }
                else if(totalWithholding > 0 && VATWithholdingAmount == 0)
                {
                    netPayDisplay = "Net Pay (Less W tax)";
                }
                else if(totalWithholding == 0 && VATWithholdingAmount > 0)
                {
                    netPayDisplay = "Net Pay (Less VAT)";
                }
                salesQuoteViewModel.NetPayDisplayText = netPayDisplay;

                #endregion
                foreach (LineItem l in lineItems)
                {
                    var lineItem = new QuotePrintLineItemViewModel
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
                    salesQuoteViewModel.LineItemList.Add(lineItem);
                }
                foreach (var service in serviceLineItems)
                {
                    var serviceRecord = new QuotePrintServiceLineItemViewModel
                    {
                        ServiceName = GetServiceName(service.ServiceTypeId),
                        UnitAmount = service.UnitAmount,
                        SubTotoal = service.SubTotal,
                        TaxAmount = service.TaxAmount,
                        WithholdingAmount = service.WithholdingAmount,
                        TotalAmount = service.TotalAmount
                    };
                    salesQuoteViewModel.ServiceLineItemList.Add(serviceRecord);
                }
                quoteList.Add(salesQuoteViewModel);
            }
            return quoteList;
        }
        private string GetServiceName(string id)
        {
            var name = "";
            var ser = db.RepairType.FirstOrDefault(l => l.RepairTypeID.ToString() == id);
            if(ser != null)
            {
                name = ser.Name;
            }
            return name;
        }
    }
}