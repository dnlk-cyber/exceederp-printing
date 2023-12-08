using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
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
    public class SalesOrderObjectDataSource : List<OrderStatus>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesOrderPrintViewModel> GetSalesOrderList(string salesOrderNumber)
        {
            List<SalesOrderPrintViewModel> salesOrderViewList = new List<SalesOrderPrintViewModel>();
            SalesOrderPrintViewModel SalesOrderViewModel;
            List<SalesOrderVoucher> SalesOrderVouchers = new List<SalesOrderVoucher>();
            var product = db.Products.ToList();
            var orgCustomer = db.OrganizationCustomers.ToList();

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

                SalesOrderVouchers = db.SalesOrderVouchers.Where(so => so.SalesOrderVoucherID.ToString() == salesOrderNumber).ToList();

            }

            if (SalesOrderVouchers.Any())
            {
                string address = "";
                string tradeName = "";

                tradeName = SalesOrderVouchers.FirstOrDefault().OrganizationCustomer.TradeName;

                var OrgCustomer = orgCustomer.Where(x => x.TradeName == tradeName).FirstOrDefault();
                if (OrgCustomer != null)
                {
                    var OrgCustomerID = OrgCustomer.OrganizationCustomerID;
                }
                foreach (SalesOrderVoucher sov in SalesOrderVouchers)
                {
                    List<LineItem> lineItems = db.LineItems.Where(li => li.Voucher == sov.SalesOrderVoucherID.ToString() && li.VoucherDefinitionID == "1").ToList();
                    List<ServiceLineItem> serviceLineItems = db.ServiceLineItems.Where(li => li.VoucherId == sov.SalesOrderVoucherID && li.VoucherType == CRMVoucherType.SalesOrder).ToList();
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

                    string CustomerType = "CASH";

                    if (OrgCustomer.IsCredit)
                    {
                        CustomerType = "CREDIT";
                    }

                    SalesOrderViewModel = new SalesOrderPrintViewModel();
                    SalesOrderViewModel.CustomerType = CustomerType;
                    SalesOrderViewModel.OrderNumber = sov.SalesOrderVoucherID.ToString();
                    SalesOrderViewModel.GrandTotal = sov.GrandTotal;
                    SalesOrderViewModel.OrderDate = sov.IssuedDate.ToString("dd/MM/yyyy");
                    SalesOrderViewModel.CustomerAddress = address;
                    SalesOrderViewModel.QuoteNumber = sov.References;
                    SalesOrderViewModel.Status = sov.LastObjectState;
                    SalesOrderViewModel.Consignee = sov.OrganizationCustomer.TradeName;
                    SalesOrderViewModel.SalesPerson = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(sov.CreatedBy));
                    SalesOrderViewModel.PaymentMethod = sov.PaymentMethod;
                    SalesOrderViewModel.OnlineApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(sov.OnlineApprovedBy));
                    SalesOrderViewModel.SendForApprovalBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(sov.SendForApprovalBy));
                    SalesOrderViewModel.SendForApprovalTime = sov.SendForApprovalTime;
                    SalesOrderViewModel.OnlineApprovedTime = sov.OnlineApprovedTime;
                    SalesOrderViewModel.ReceiptReference = sov.ReceiptReference;

                    SalesOrderViewModel.AmharicName = AmharicName;
                    SalesOrderViewModel.Name = Name;
                    SalesOrderViewModel.Address = Address;
                    SalesOrderViewModel.LogoFileName = LogoFileName;


                    SalesOrderViewModel.RequestedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(sov.CreatedBy));
                    SalesOrderViewModel.CheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(sov.OnlineApprovedBy));
                    SalesOrderViewModel.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(sov.OnlineApprovedBy));

                    SalesOrderViewModel.TotalAmountInWords = totalAmountInwords;
                    SalesOrderViewModel.GrandTotal = totalAmount;
                    SalesOrderViewModel.TotalTax = totalTax;
                    SalesOrderViewModel.TotalWithholding = totalWithholding;
                    SalesOrderViewModel.TotalVATWithholdingAmount = VATWithholdingAmount;
                    SalesOrderViewModel.NetPay = totalAmount - VATWithholdingAmount - totalWithholding;
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
                    SalesOrderViewModel.NetPayDisplayText = netPayDisplay;


                    foreach (LineItem l in lineItems)
                    {
                        var lineItem = new SalesOrderPrintLineItemViewModel
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
                        SalesOrderViewModel.LineItemList.Add(lineItem);
                    }
                    foreach (var service in serviceLineItems)
                    {
                        var serviceRecord = new SalesOrderPrintServiceLineItemViewModel
                        {
                            ServiceName = GetServiceName(service.ServiceTypeId),
                            UnitAmount = service.UnitAmount,
                            SubTotoal = service.SubTotal,
                            TaxAmount = service.TaxAmount,
                            WithholdingAmount = service.WithholdingAmount,
                            TotalAmount = service.TotalAmount
                        };
                        SalesOrderViewModel.ServiceLineItemList.Add(serviceRecord);
                    }
                    salesOrderViewList.Add(SalesOrderViewModel);
                }
            }
            return salesOrderViewList;
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







