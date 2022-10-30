using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.Printing.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XAPI;

namespace ExceedERP.Reporting.CRM.Printing.DataSource
{
    [DataObject]
    public class PriceEstimationOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PriceEstimationPrintViewModel> GetSalesQuoteList(string quoteNumber)
        {
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            string vat = "";
            string tin = "";
            if (organization != null)
            {
                var tinIdentification = db.Identifications.FirstOrDefault(a =>
                            a.Type == Core.Domain.Common.IdentificationType.TIN &&
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company");
                var companyVat = db.Identifications.FirstOrDefault(a =>
                           a.Type == Core.Domain.Common.IdentificationType.VAT_NUMBER &&
                           a.Reference == organization.MainCompanyId.ToString() &&
                           a.ReferenceType.ToLower() == "company");
                if(tinIdentification != null)
                {
                    tin = tinIdentification.IdNumber;
                }
                if(companyVat != null)
                {
                    vat = companyVat.IdNumber;
                }
            }
            List<PriceEstimationPrintViewModel> list = new List<PriceEstimationPrintViewModel>();
            if (int.TryParse(quoteNumber, out int id))
            {

                var currentQuote = db.PriceEstimationQuoteVouchers.Find(id);
                if (currentQuote == null)
                {
                    return null;
                }
                var lineItems =
                     db.PriceEstimationQuoteVoucherLines.Where(li => li.PriceEstimationQuoteVoucherId == id).ToList();
                var serviceLineItems = db.PriceEstimationQuoteVoucherServices.Where(l => l.PriceEstimationQuoteVoucherId == id).ToList();
                decimal subTotal = 0;
                decimal totalAmount = 0;
                decimal totalTax = 0;
                decimal totalWithholding = 0;
                decimal totalVatWithholdingAmount = 0;
                string totalAmountInwords = string.Empty;
                if (lineItems.Any())
                {
                    totalAmount += lineItems.Sum(l => l.TotalAmount);
                    totalTax += lineItems.Sum(l => l.TaxAmount);
                    totalWithholding += lineItems.Sum(l => l.WithholdingAmount);
                    subTotal += lineItems.Sum(l => l.SubTotal);
                    totalVatWithholdingAmount += lineItems.Sum(l => l.WithholdTaxAmount);

                }
                if (serviceLineItems.Any())
                {
                    totalAmount += serviceLineItems.Sum(l => l.TotalAmount);
                    totalTax += serviceLineItems.Sum(l => l.TaxAmount);
                    totalWithholding += serviceLineItems.Sum(l => l.WithholdingAmount);
                    subTotal += serviceLineItems.Sum(l => l.SubTotal);
                    totalVatWithholdingAmount += serviceLineItems.Sum(l => l.WithholdTaxAmount);
                }

                totalAmountInwords = NumberExtensions.toWords((double)totalAmount) + " birr ";
                var afterDecimal = totalAmount - Math.Floor(totalAmount);
                var roundedAfterDecimal = (int)Math.Round(afterDecimal * 100);

                if (roundedAfterDecimal > 0 && roundedAfterDecimal < 100)
                {
                    totalAmountInwords += " and " + NumberExtensions.toWords(roundedAfterDecimal) + " cents";
                }
                #region create sales quote
                //var equipmentInfo = db.EquipmentInformation.FirstOrDefault(e => e.CustomerId == currentQuote.OrganizationCustomerId && e.LicensePlate == currentQuote.PlateNo);
                var acc = db.BankAccounts.Find(currentQuote.BankAccountId);
                string accNo = acc != null ? $"{acc.AccountNumber} { acc.AccountName} {acc.BankBranch} Branch" : "";
                var record = new PriceEstimationPrintViewModel
                {
                    Customer = GetCustomerName(currentQuote.OrganizationCustomerId),
                    Date = currentQuote.Date,
                    PlateNo = currentQuote.PlateNo,
                    //Equipment = equipmentInfo?.GroupName,
                    BanckAccount = accNo,
                    VATNo = vat,
                    TINNo = tin,
                    SubTotal = subTotal,
                    GrandTotal = totalAmount,
                    TotalInWords = totalAmountInwords,
                    TotalTax = totalTax,
                    TotalWithholding = totalWithholding,
                    NetPay = totalAmount - totalVatWithholdingAmount - totalWithholding,
                    QuoteNumber = currentQuote.PriceEstimationQuoteVoucherId.ToString("D8"),

                    PreparedBy = currentQuote.CreatedBy,
                    CheckedBy = currentQuote.MiddleOnlineApprovedBy,
                    ApprovedBy = currentQuote.OnlineApprovedBy
                };
                string netPayDisplay = "Net Pay";
                if (totalWithholding > 0 && totalVatWithholdingAmount > 0)
                {
                    netPayDisplay = "Net Pay (Less W tax & VAT)";
                }
                else if (totalWithholding > 0 && totalVatWithholdingAmount == 0)
                {
                    netPayDisplay = "Net Pay (Less W tax)";
                }
                else if (totalWithholding == 0 && totalVatWithholdingAmount > 0)
                {
                    netPayDisplay = "Net Pay (Less VAT)";
                }
                record.NetPayDisplayText = netPayDisplay;

                #endregion
                foreach (PriceEstimationQuoteVoucherLine l in lineItems)
                {
                    var lineItem = new PriceEstimationPrintLineItemViewModel
                    {
                        ProductName = GetProductName(l.ProductId),
                        Quantity = l.Quantity,
                        SellingPrice = l.UnitAmount,
                        SubTotoal = l.SubTotal,
                        TaxAmount = l.TaxAmount,
                        WithholdingAmount = l.WithholdingAmount,
                        TotalAmount = l.TotalAmount
                    };
                    record.LineItemList.Add(lineItem);
                }
                foreach (var service in serviceLineItems)
                {
                    var serviceRecord = new PriceEstimationPrintServiceLineItemViewModel
                    {
                        ServiceName = GetServiceName(service.ServiceTypeId),
                        UnitAmount = service.UnitAmount,
                        SubTotoal = service.SubTotal,
                        TaxAmount = service.TaxAmount,
                        WithholdingAmount = service.WithholdingAmount,
                        TotalAmount = service.TotalAmount
                    };
                    record.ServiceLineItemList.Add(serviceRecord);
                }
                list.Add(record);
            }
            return list;
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
        private string GetCustomerName(int id)
        {
            return db.OrganizationCustomers.Find(id)?.TradeName;
        }
        private string GetProductName(int id)
        {
            string name = "";
            var product = db.Products.Find(id);
            if (product != null)
            {
                name = $"{product.Name} {product.ItemCode}";
            }
            return name;
        }
    }
}