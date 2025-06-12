using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.CRM.Printing.ViewModel;
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
	public class SalesInvoiceObjectDataSource
	{

		private readonly ExceedDbContext db = new ExceedDbContext();


		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<SalesInvoicePrintViewMode> GetSalesinvoiceList(string invoiceNumber)
		{
			List<SalesInvoicePrintViewMode> list = new List<SalesInvoicePrintViewMode>();
			if (string.IsNullOrEmpty(invoiceNumber))
			{
				return list;
			}
			int.TryParse(invoiceNumber, out int id);
			var record = db.CashSalesInvoices.Find(id);
			if (record is null || !record.IsOnlineApproved || record.IsDeleted)
			{
				return list;
			}

			string customerAddress = null;

			string AmharicName = "";
			string Name = "";

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

			var mainCompany = db.Companies.FirstOrDefault(x => x.Category == "0");

			if (mainCompany != null)
			{
				Name = mainCompany.TradeName;
				AmharicName = mainCompany.AmharicName;
				dateOfRegistration = mainCompany.DateCreated?.ToString("dd/MM/yyyy");
				var address =
					db.Addresses.Where(
						a =>
							a.Reference == mainCompany.MainCompanyId.ToString() &&
							a.ReferenceType.ToLower() == "company").ToList();

				var tinIdentification = db.Identifications.FirstOrDefault(a =>
							a.Type == Core.Domain.Common.IdentificationType.TIN &&
							a.Reference == mainCompany.MainCompanyId.ToString() &&
							a.ReferenceType.ToLower() == "company");
				var companyVatReg = db.Identifications.FirstOrDefault(a =>
						   a.Type == Core.Domain.Common.IdentificationType.VAT_NUMBER &&
						   a.Reference == mainCompany.MainCompanyId.ToString() &&
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
					db.Attachments.FirstOrDefault(a => a.Reference == mainCompany.MainCompanyId.ToString() &&
							a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
				if (logo != null)
				{
					LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
				}
			}

			var customer = db.OrganizationCustomers.Find(record.OrganizationCustomerId);
			if (customer != null)
			{
				List<Address> customerAddresses = db.Addresses
					.AsNoTracking()
					.Where(x => x.Reference == customer.OrganizationCustomerID.ToString() 
						&& x.ReferenceType == "Organization_Customer")
					.ToList();
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
			}

			var selectedCustomerName = "";
			var selectedCustomerTin = "";
			var currentCustomer = db.OrganizationCustomers.Find(record.OrganizationCustomerId);
			if (currentCustomer != null)
			{
				selectedCustomerName = currentCustomer.TradeName;
				selectedCustomerTin = currentCustomer.TinNo;
			}
			var paymentTerm = string.Empty;
			var invoicePaymentTerm = db.PaymentTerms.Find(record.PaymentTermsID);

			if (invoicePaymentTerm != null)
			{
				paymentTerm = invoicePaymentTerm.PaymentTermsDays + " days";
			}

			List<LineItem> lineItems = db.LineItems
				.AsNoTracking()
				.Where(li => li.Voucher.ToString() == record.CashSalesInvoiceID.ToString() && li.VoucherDefinitionID == "2")
				.ToList();
			List<ServiceLineItem> serviceLineItems = db.ServiceLineItems
				.AsNoTracking()
				.Where(li => li.VoucherId == record.CashSalesInvoiceID && li.VoucherType == CRMVoucherType.SalesInvoice)
				.ToList();
            List<AdditionalCostLog> additionalCosts = db.AdditionalCostLogs
				.AsNoTracking()
				.Where(li => li.VoucherId == record.CashSalesInvoiceID && li.VoucherType == VoucherType.SalesInvoice)
				.ToList();

			decimal totalAmount = 
                lineItems.Sum(l => l.TotalAmount) 
                + serviceLineItems.Sum(l => l.TotalAmount)
                + additionalCosts.Sum(l => l.TotalAmount);
			decimal totalTax =
                 lineItems.Sum(l => l.TaxAmount)
                 + serviceLineItems.Sum(l => l.TaxAmount)
                 + additionalCosts.Sum(a => a.TaxAmount);
			decimal totalWithholding =
                lineItems.Sum(l => l.WithholdingAmount)
                + serviceLineItems.Sum(l => l.WithholdingAmount);
			decimal VATWithholdingAmount =
                lineItems.Sum(l => l.WithholdTaxAmount)
                + serviceLineItems.Sum(l => l.WithholdTaxAmount);

			var totalAmountInwords = string.Empty;

			totalAmountInwords = NumberExtensions.toWords((int)totalAmount) + " birr ";
			var afterDecimal = totalAmount - Math.Floor(totalAmount);
			var roundedAfterDecimal = (int)Math.Round(afterDecimal * 100);

			if (roundedAfterDecimal > 0 && roundedAfterDecimal < 100)
			{
				totalAmountInwords += " and " + NumberExtensions.toWords(roundedAfterDecimal) + " cents";
			}
			var branchName = "";
			var invoiceBranch = db.Branch.FirstOrDefault(b => b.BranchId == record.BranchId);
			if (invoiceBranch != null)
			{
				branchName = invoiceBranch.Name;
			}

			string CustomerType = "CASH";

			if (customer.IsCredit)
			{
				CustomerType = "CREDIT";
			}
			var store = db.SalesStores.Find(record.StoreID);
			var invoiceVm = new SalesInvoicePrintViewMode
			{
				Consignee = selectedCustomerName,
				Branch = branchName,
				CustomerType = CustomerType,
				CustomerAddress = customerAddress,
				CustomerTinNumber = selectedCustomerTin,
				InvoiceNumber = record.CashSalesInvoiceID,
				PaymentTerm = paymentTerm,
				Remark = record.PosPrintType == PosPrintType.CRM ? record.Remark : "",
				Store = store?.Description,
				OrderNumber = record.Source == SalesInvoiceSource.SalesOrder ? "Sales Order # " + record.References : record.References,
				ReceiptReference = record.ReceiptReference,
				FsInvoiceNumber = record.FsInvoiceNo,
				SalesPerson = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.CreatedBy)),
				PaymentMethod = record.PaymentMethod,
				InvoiceDate = record.IssuedDate.ToString("dd/MM/yyyy"),
				Status = record.LastObjectState,
				OrderDate = DateTime.Now,
				TotalTax = totalTax,
				TotalAmountInWords = totalAmountInwords,
				OnlineApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.OnlineApprovedBy)),
				SendForApprovalBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.SendForApprovalBy)),
				SendForApprovalTime = record.SendForApprovalTime,
				OnlineApprovedTime = record.OnlineApprovedTime,
				PrintStatus = record.IsReceiptPrinted ? "Printed" : "Not Printed",
				AmharicName = AmharicName,
				Name = Name,
				LogoFileName = LogoFileName,
				SubCity = subCity,
				HouseNumber = houseNumber,
				PhoneNumber = phoneNumber,
				PoBox = poBox,
				DateOfRegistration = dateOfRegistration,
				VatRegNumber = vatRegNumber,
				TinNumber = tinNumber,
				RequestedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.CreatedBy)),
				CheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.SendForApprovalBy)),
				ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.OnlineApprovedBy)),
				TotalAmount = totalAmount,
				TotalWithholding = totalWithholding,
				TotalVATWithholdingAmount = VATWithholdingAmount,
				NetPay = totalAmount - VATWithholdingAmount - totalWithholding,
			};
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
			invoiceVm.NetPayDisplayText = netPayDisplay;

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
					VatWithholdingAmount = l.WithholdTaxAmount,
					TotalAmount = l.TotalAmount
				};
				invoiceVm.LineItemList.Add(lineItem);
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
				invoiceVm.ServiceLineItemList.Add(serviceRecord);
			}
            foreach (var additionalCost in additionalCosts)
			{
				var serviceRecord = new SalesInvoicePrintServiceLineItemViewModel
				{
					ServiceName = additionalCost.AdditionalCost.Name,
					UnitAmount = additionalCost.CostAmount,
					SubTotoal = additionalCost.CostAmount,
					TaxAmount = additionalCost.TaxAmount,
					WithholdingAmount = 0,
					TotalAmount = additionalCost.TotalAmount
				};
				invoiceVm.ServiceLineItemList.Add(serviceRecord);
			}
			list.Add(invoiceVm);

			return list;
		}

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesInvoicePrintViewMode> GetSalesinvoiceForExternal(string invoiceNumber)
        {
            List<SalesInvoicePrintViewMode> list = new List<SalesInvoicePrintViewMode>();
            if (string.IsNullOrEmpty(invoiceNumber))
            {
                return list;
            }
            int.TryParse(invoiceNumber, out int id);
            var record = db.CashSalesInvoices.Find(id);
            if (record is null || !record.IsOnlineApproved || record.IsDeleted)
            {
                return list;
            }

            string customerAddress = null;

            string AmharicName = "";
            string Name = "";

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

            var mainCompany = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (mainCompany != null)
            {
                Name = mainCompany.TradeName;
                AmharicName = mainCompany.AmharicName;
                dateOfRegistration = mainCompany.DateCreated?.ToString("dd/MM/yyyy");
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == mainCompany.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();

                var tinIdentification = db.Identifications.FirstOrDefault(a =>
                            a.Type == Core.Domain.Common.IdentificationType.TIN &&
                            a.Reference == mainCompany.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company");
                var companyVatReg = db.Identifications.FirstOrDefault(a =>
                           a.Type == Core.Domain.Common.IdentificationType.VAT_NUMBER &&
                           a.Reference == mainCompany.MainCompanyId.ToString() &&
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
                    db.Attachments.FirstOrDefault(a => a.Reference == mainCompany.MainCompanyId.ToString() &&
                            a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            var customer = db.OrganizationCustomers.Find(record.OrganizationCustomerId);
            if (customer != null)
            {
                List<Address> customerAddresses = db.Addresses
                    .AsNoTracking()
                    .Where(x => x.Reference == customer.OrganizationCustomerID.ToString()
                        && x.ReferenceType == "Organization_Customer")
                    .ToList();
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
            }

            var selectedCustomerName = "";
            var selectedCustomerTin = "";
            var currentCustomer = db.OrganizationCustomers.Find(record.OrganizationCustomerId);
            if (currentCustomer != null)
            {
                selectedCustomerName = currentCustomer.TradeName;
                selectedCustomerTin = currentCustomer.TinNo;
            }
            var paymentTerm = string.Empty;
            var invoicePaymentTerm = db.PaymentTerms.Find(record.PaymentTermsID);

            if (invoicePaymentTerm != null)
            {
                paymentTerm = invoicePaymentTerm.PaymentTermsDays + " days";
            }

            List<LineItemExternal> lineItems = db.LineItemExternal
                .AsNoTracking()
                .Where(li => li.VoucherId == record.CashSalesInvoiceID
                    && li.VoucherType == CRMVoucherType.SalesInvoice)
                .ToList();
            
            decimal totalAmount = 0;
            decimal totalTax = 0;
            decimal totalWithholding = 0;
            decimal VATWithholdingAmount = 0;
            var totalAmountInwords = string.Empty;
            
                totalAmount += lineItems.Sum(l => l.TotalAmount);
                totalTax += lineItems.Sum(l => l.TaxAmount);
                totalWithholding += lineItems.Sum(l => l.WithholdingAmount);
                VATWithholdingAmount += lineItems.Sum(l => l.WithholdTaxAmount);
            
            totalAmountInwords = NumberExtensions.toWords((int)totalAmount) + " birr ";
            var afterDecimal = totalAmount - Math.Floor(totalAmount);
            var roundedAfterDecimal = (int)Math.Round(afterDecimal * 100);

            if (roundedAfterDecimal > 0 && roundedAfterDecimal < 100)
            {
                totalAmountInwords += " and " + NumberExtensions.toWords(roundedAfterDecimal) + " cents";
            }
            var branchName = "";
            var invoiceBranch = db.Branch.FirstOrDefault(b => b.BranchId == record.BranchId);
            if (invoiceBranch != null)
            {
                branchName = invoiceBranch.Name;
            }

            string CustomerType = "CASH";

            if (customer.IsCredit)
            {
                CustomerType = "CREDIT";
            }
            var store = db.SalesStores.Find(record.StoreID);
            var invoiceVm = new SalesInvoicePrintViewMode
            {
                Consignee = selectedCustomerName,
                Branch = branchName,
                CustomerType = CustomerType,
                CustomerAddress = customerAddress,
                CustomerTinNumber = selectedCustomerTin,
                InvoiceNumber = record.CashSalesInvoiceID,
                PaymentTerm = paymentTerm,
                Remark = record.PosPrintType == PosPrintType.CRM ? record.Remark : "",
                Store = store?.Description,
                OrderNumber = record.Source == SalesInvoiceSource.SalesOrder ? "Sales Order # " + record.References : record.References,
                ReceiptReference = record.ReceiptReference,
                FsInvoiceNumber = record.FsInvoiceNo,
                SalesPerson = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.CreatedBy)),
                PaymentMethod = record.PaymentMethod,
                InvoiceDate = record.IssuedDate.ToString("dd/MM/yyyy"),
                Status = record.LastObjectState,
                OrderDate = DateTime.Now,
                TotalTax = totalTax,
                TotalAmountInWords = totalAmountInwords,
                OnlineApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.OnlineApprovedBy)),
                SendForApprovalBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.SendForApprovalBy)),
                SendForApprovalTime = record.SendForApprovalTime,
                OnlineApprovedTime = record.OnlineApprovedTime,
                PrintStatus = record.IsReceiptPrinted ? "Printed" : "Not Printed",
                AmharicName = AmharicName,
                Name = Name,
                LogoFileName = LogoFileName,
                SubCity = subCity,
                HouseNumber = houseNumber,
                PhoneNumber = phoneNumber,
                PoBox = poBox,
                DateOfRegistration = dateOfRegistration,
                VatRegNumber = vatRegNumber,
                TinNumber = tinNumber,
                RequestedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.CreatedBy)),
                CheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.SendForApprovalBy)),
                ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.OnlineApprovedBy)),
                TotalAmount = totalAmount,
                TotalWithholding = totalWithholding,
                TotalVATWithholdingAmount = VATWithholdingAmount,
                NetPay = totalAmount - VATWithholdingAmount - totalWithholding,
            };
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
            invoiceVm.NetPayDisplayText = netPayDisplay;

            foreach (LineItemExternal l in lineItems)
            {
                var lineItem = new SalesInvoicePrintLineItemViewModel
                {
                    ProductName = $"{l.ProductOrServiceDescription}",
                    UOM = l.UoM,
                    Quantity = l.Quantity,
                    SellingPrice = l.UnitPrice,
                    SubTotoal = l.Subtotal,
                    TaxAmount = l.TaxAmount,
                    WithholdingAmount = l.WithholdingAmount,
                    VatWithholdingAmount = l.WithholdTaxAmount,
                    TotalAmount = l.TotalAmount
                };
                invoiceVm.LineItemList.Add(lineItem);
            }
            list.Add(invoiceVm);

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
	}
}





