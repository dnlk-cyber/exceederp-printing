using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Drawing;
using System.Linq;

namespace ExceedERP.Reporting.CRM.DataSource
{
    [DataObject]
    public class SalesDescriptionOds
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesDescriptionViewModel> GetSalesData(DateTime start, DateTime end, string receiptCategory, string branchId, string storeId)
        {
            List<SalesDescriptionViewModel> list = new List<SalesDescriptionViewModel>();

            //var temp = _db.CashSalesInvoices.Where(s => s.IsOnlineApproved).ToList();
            //var sales = new List<CashSalesInvoice>();
            var sales = _db.CashSalesInvoices.Where(c => !string.IsNullOrEmpty(c.OrganizationCustomer.TinNo)).Where(o =>
           (o.IsOnlineApproved && o.IsManual && DbFunctions.TruncateTime(o.Date) <= DbFunctions.TruncateTime(end) && DbFunctions.TruncateTime(o.Date) >= DbFunctions.TruncateTime(start)) ||
           (!o.IsManual && o.IsReceiptPrinted && o.InvoicePrintTime != null && DbFunctions.TruncateTime(o.InvoicePrintTime.Value) <= DbFunctions.TruncateTime(end) && DbFunctions.TruncateTime(o.InvoicePrintTime.Value) >= DbFunctions.TruncateTime(start))).ToList();


            if (receiptCategory != null)
            {
                switch (receiptCategory)
                {
                    case ReceiptCategory.CashSalesFiscalReceipt:
                        sales = sales.Where(s => s.PaymentMethod == PaymentMethod.CashPaymentMethod && s.IsManual == false).ToList();
                        break;
                    case ReceiptCategory.CashSalesManualReceipt:
                        sales = sales.Where(s => s.PaymentMethod == PaymentMethod.CashPaymentMethod && s.IsManual == true).ToList();
                        break;
                    case ReceiptCategory.CreditSalesFiscalReceipt:
                        sales = sales.Where(s => s.PaymentMethod == PaymentMethod.CreditPaymentMethod && s.IsManual == false).ToList();
                        break;
                    case ReceiptCategory.CreditSalesManualReceipt:
                        sales = sales.Where(s => s.PaymentMethod == PaymentMethod.CreditPaymentMethod && s.IsManual == true).ToList();
                        break;
                    default:
                        break;
                }
            }

            if (branchId != null)
            {
                int bId = int.Parse(branchId);
                sales = sales.Where(o => o.BranchId == bId).ToList();
            }
           
            if (storeId != null)
            {
                int sId = int.Parse(storeId);

                sales = sales.Where(s => s.StoreID == sId).ToList();
            }
            foreach (CashSalesInvoice inv in sales)
            {
                //string brachName = GetBranch(inv.BranchId);
                //string storeName = GetStore(inv.StoreID);

                var lineItems = _db.LineItems.Where(l => l.VoucherDefinitionID == "2" && l.Voucher == inv.CashSalesInvoiceID.ToString()).ToList();
                //if (!string.IsNullOrEmpty(categoryId))
                //{
                //    int.TryParse(categoryId, out int catId);
                //    lineItems = lineItems.Where(l => l.Product.CategoryID == categoryId).ToList();
                //}
                if (lineItems.Any())
                {
                    foreach (LineItem lineItem in lineItems)
                    {
                        var lineRecord = new SalesDescriptionViewModel
                        {
                            //ItemCategory = GetProductCategory(lineItem.Product.CategoryID),
                            ItemCode = lineItem.Product.ItemCode,
                            ItemName = lineItem.Product.Name,
                            //UoM = lineItem.Product.UOM,
                            Quantity = lineItem.Quantity,
                            Price = lineItem.UnitAmount,
                            TotalAmount = lineItem.TotalAmount,
                            //SubTotal = lineItem.CalculatedCost,
                            //Adds = 0,
                            //Discount = lineItem.Discount,
                            //TaxAmount = lineItem.TaxAmount,
                            //TotalAmount = lineItem.TotalAmount,

                            //CustomerName = inv.OrganizationCustomer.TradeName,
                            //CustomerAddress = "",
                            BuyerTin = inv.OrganizationCustomer.TinNo,
                            ReceiptDate = inv.Date,
                            ReceiptNumber = inv.InvoiceNumber,
                            FsInvoiceNumber = inv.FsInvoiceNo,
                            //Type = inv.PaymentMethod,
                            //SystemReference = inv.CashSalesInvoiceID.ToString(),
                            //DocumentReference = inv.ReceiptReference,
                            //FsInvoiceNumber = inv.FsInvoiceNo,
                            //SalesPerson = inv.InvoicePrintedBy,
                            //Branch = brachName,
                            //Shop = storeName,
                            //Remark = lineItem.Remark,
                        };
                        list.Add(lineRecord);
                    }
                }
            }
            return list;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public SalesDescriptionCompanyHeaderViewModel GetHeader()
        {

            SalesDescriptionCompanyHeaderViewModel viewModel = new SalesDescriptionCompanyHeaderViewModel();
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
            if (organization != null)
            {
                viewModel.CompanyName = organization.TradeName;
                viewModel.AmharicName = organization.AmharicName;

                var logo =
                    _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    viewModel.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
                var address =
                _db.Addresses.Where(
                    a =>
                        a.Reference == organization.MainCompanyId.ToString() &&
                        a.ReferenceType.ToLower() == "company").ToList();
                var phoneNumberAddress = address.FirstOrDefault(a => a.Category == Core.Domain.Common.AddressType.OFFICE_PHONE);

                var tinIdentification = _db.Identifications.FirstOrDefault(a =>
                            a.Type == Core.Domain.Common.IdentificationType.TIN &&
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company");

                if (tinIdentification != null)
                {
                    viewModel.TinNumber = tinIdentification.IdNumber;
                }
                if (phoneNumberAddress != null)
                {
                    viewModel.PhoneNumber = phoneNumberAddress.Value;
                }
            }
            return viewModel;

        }
    }

}
