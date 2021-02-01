using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.SubLedger.ViewModel;
using ExceedERP.Reporting.Finance.ViewModel.Print;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.SubLedger.DataSource
{
    [DataObject]
    public class ApJournalEntriesOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public Organization ApJournalHeader()
        {
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            Organization headerDs = new Organization();

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    _db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (headerDs.Address == null)
                        {
                            headerDs.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            headerDs.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            return headerDs;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        
        public List<ApJournalEntriesCategoryViewModel> ApJournalEntries(string category,string supplier,string dateFrom,string dateTo)
        {
            List<ApJournalEntriesCategoryViewModel> apJournalEntries = new List<ApJournalEntriesCategoryViewModel>();

            var invoices = _db.APPaybleInvoices.Where(x => x.IsOnlineApproved&&x.IsVoid==false)
                .Select(x=> new {x.InvoiceDate,x.SupplierId, x.SupplierName,x.APPaybleInvoiceDistribution,x.ApPaybleInvoicesId}).AsQueryable();
            var payments = _db.APPayments.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Select(x => new { x.SupplierId,x.SupplierName, x.PaymentDate,x.ApPaymentDistribution,x.ApPaymentId }).AsQueryable();
            var directpayments = _db.ApDirectPayments.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Select(x => new { x.SupplierId,x.SupplierName, x.PaymentDate, x.ApDirectPaymentDistribution, x.ApDirectPaymentId }).AsQueryable();
            var miscellaneousPayments = _db.APMiscellaneousPayments.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Select(x => new { x.SupplierId, x.SupplierName, x.PaymentDate, x.ApMiscellaneousPaymentDistribution, x.ApMiscellaneousPaymentId }).AsQueryable();

            if (!String.IsNullOrEmpty(dateFrom) && !String.IsNullOrEmpty(dateTo))
            {
                DateTime from = DateTime.Parse(dateFrom).Date;
                DateTime to = DateTime.Parse(dateTo).Date;
                if (String.IsNullOrEmpty(category) || category == "Invoice")
                {
                    #region Invoice

                    invoices = invoices.Where(x => x.InvoiceDate >= from && x.InvoiceDate <= to);

                    if (!String.IsNullOrEmpty(supplier))
                    {
                        invoices = invoices.Where(x => x.SupplierId == supplier);
                    }

                    var records = invoices.ToList();
                    var apcategory = new ApJournalEntriesCategoryViewModel
                    {
                        Category = "Invoice",
                        ApJournalEntriesOdsViewModels = new List<ApJournalEntriesOdsViewModel>()
                    };
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.APPaybleInvoiceDistribution)
                        {
                            var line = new ApJournalEntriesOdsViewModel
                            {

                                TransactionNumber = record.ApPaybleInvoicesId.ToString(),
                                TransactionDate = record.InvoiceDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Reference = distribution.ApPaybleInvoices.Remark,
                                Supplier=record.SupplierName
                            };

                            apcategory.ApJournalEntriesOdsViewModels.Add(line);

                        }

                    }

                    #endregion
                }

                if (String.IsNullOrEmpty(category) || category == "Payment")
                {
                    #region payment

                    payments = payments.Where(x => x.PaymentDate >= from && x.PaymentDate <= to);
                    if (!String.IsNullOrEmpty(supplier))
                    {
                        payments = payments.Where(x => x.SupplierId == supplier);
                    }
                    var records = payments.ToList();
                    var apcategory = new ApJournalEntriesCategoryViewModel
                    {
                        Category = "Payment",
                        ApJournalEntriesOdsViewModels = new List<ApJournalEntriesOdsViewModel>()
                    };
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.ApPaymentDistribution)
                        {
                            var line = new ApJournalEntriesOdsViewModel
                            {

                                TransactionNumber = record.ApPaymentId.ToString(),
                                TransactionDate = record.PaymentDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                PaymentReference = distribution.ApPayment.PaymentReference,
                                Reference = distribution.ApPayment.Remark,
                                Supplier = record.SupplierName
                            };
                            apcategory.ApJournalEntriesOdsViewModels.Add(line);

                        }

                    }
                    apJournalEntries.Add(apcategory);

                    #endregion
                }

                if (String.IsNullOrEmpty(category) || category == "Direct Payment")
                {
                    #region payment

                    directpayments = directpayments.Where(x => x.PaymentDate >= from && x.PaymentDate <= to);

                    if (!String.IsNullOrEmpty(supplier))
                    {
                        directpayments = directpayments.Where(x => x.SupplierId == supplier);
                    }


                    var records = directpayments.ToList();
                    var apcategory = new ApJournalEntriesCategoryViewModel
                    {
                        Category = "Direct Payment",
                        ApJournalEntriesOdsViewModels = new List<ApJournalEntriesOdsViewModel>()
                    };
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.ApDirectPaymentDistribution)
                        {
                            var line = new ApJournalEntriesOdsViewModel
                            {

                                TransactionNumber = record.ApDirectPaymentId.ToString(),
                                TransactionDate = record.PaymentDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                PaymentReference = distribution.ApDirectPayment.PaymentReference,
                                Reference = distribution.ApDirectPayment.Remark,
                                Supplier = record.SupplierName
                            };
                            apcategory.ApJournalEntriesOdsViewModels.Add(line);

                        }

                    }

                    apJournalEntries.Add(apcategory);

                    #endregion
                }
                if (String.IsNullOrEmpty(category) || category == "Miscellaneous Payment")
                {
                    #region Miscellaneous payment

                    miscellaneousPayments =miscellaneousPayments.Where(x => x.PaymentDate >= from && x.PaymentDate <= to);
                    var records = miscellaneousPayments.ToList();
                    var apcategory = new ApJournalEntriesCategoryViewModel
                    {
                        Category = "Miscellaneous Payment",
                        ApJournalEntriesOdsViewModels = new List<ApJournalEntriesOdsViewModel>()
                    };
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.ApMiscellaneousPaymentDistribution)
                        {
                            var line = new ApJournalEntriesOdsViewModel
                            {

                                TransactionNumber = record.ApMiscellaneousPaymentId.ToString(),
                                TransactionDate = record.PaymentDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                PaymentReference = distribution.ApMiscellaneousPayment.PaymentReference,
                                Reference = distribution.ApMiscellaneousPayment.Remark,
                                Supplier = record.SupplierName
                            };
                            apcategory.ApJournalEntriesOdsViewModels.Add(line);

                        }

                    }
                    apJournalEntries.Add(apcategory);

                    #endregion
                }
            }
            return apJournalEntries;
        }
    }
}
