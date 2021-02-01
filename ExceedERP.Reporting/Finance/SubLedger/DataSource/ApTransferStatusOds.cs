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
    public class ApTransferStatusOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public Organization ApTransferHeader()
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
        public List<ApGlTransferViewModel> ApTransferToGl(string category, string status, string supplier, string dateFrom, string dateTo)
        {
            List<ApGlTransferViewModel> lines = new List<ApGlTransferViewModel>();

            var invoices = _db.APPaybleInvoices.Where(x => x.IsOnlineApproved&&x.IsVoid==false)
                .Select(x => new { x.InvoiceDate, x.SupplierId, x.APPaybleInvoiceDistribution, x.ApPaybleInvoicesId,x.IsOnlineTransferred }).AsQueryable();
            var payments = _db.APPayments.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Select(x => new { x.SupplierId, x.PaymentDate, x.ApPaymentDistribution, x.ApPaymentId,x.IsOnlineTransferred }).AsQueryable();
            var directPayments = _db.ApDirectPayments.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Select(x => new { x.SupplierId, x.PaymentDate, x.ApDirectPaymentDistribution, x.ApDirectPaymentId ,x.IsOnlineTransferred}).AsQueryable();
            var miscellaneousPayments = _db.APMiscellaneousPayments.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Select(x => new { x.SupplierId, x.PaymentDate, x.ApMiscellaneousPaymentDistribution, x.ApMiscellaneousPaymentId,x.IsOnlineTransferred }).AsQueryable();
            var pettyCash= _db.ApPettyCash.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Select(x => new { x.ApPettyCashId,x.Date,x.PettyCashDistributions, x.IsOnlineTransferred }).AsQueryable();
            var travelOrders = _db.ApTravelOrders.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Select(x => new { x.ApTravelOrderId,x.ApTravelOrderDistributions,x.RequestDate, x.IsOnlineTransferred }).AsQueryable();
            var expenseClaims = _db.ApTravelExpenseClaims.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Select(x => new { x.Distributions, x.ApTravelExpenseClaimId, x.ClaimDate, x.IsOnlineTransferred }).AsQueryable();

            if (!String.IsNullOrEmpty(dateFrom) && !String.IsNullOrEmpty(dateTo))
            {
                DateTime from = DateTime.Parse(dateFrom).Date;
                DateTime to = DateTime.Parse(dateTo).Date;
                invoices = invoices.Where(x => x.InvoiceDate >= from && x.InvoiceDate <= to);
                payments = payments.Where(x => x.PaymentDate >= from && x.PaymentDate <= to);
                miscellaneousPayments = miscellaneousPayments.Where(x => x.PaymentDate >= from && x.PaymentDate <= to);
                pettyCash = pettyCash.Where(x => x.Date >= from && x.Date <= to);


                if (String.IsNullOrEmpty(category) || category == "Invoice")
                {
                    #region Invoice

                    if (!String.IsNullOrEmpty(supplier))
                    {
                        invoices = invoices.Where(x => x.SupplierId == supplier);
                    }

                    if (!String.IsNullOrEmpty(status))
                    {
                        if (status == "Transferred to GL")
                        {
                            invoices = invoices.Where(x => x.IsOnlineTransferred);
                        }
                        else if (status == "Not Transferred to GL")
                        {
                            invoices = invoices.Where(x => x.IsOnlineTransferred == false);
                        }
                    }

                    var records = invoices.ToList();
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.APPaybleInvoiceDistribution)
                        {
                            var line = new ApGlTransferViewModel
                            {
                                TransactionNumber = record.ApPaybleInvoicesId.ToString(),
                                TransactionDate = record.InvoiceDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Reference = distribution.ApPaybleInvoices.Remark
                            };
                            lines.Add(line);

                        }

                    }

                    #endregion
                }

                if (String.IsNullOrEmpty(category) || category == "Payment")
                {
                    #region Payment

                   
                    if (!String.IsNullOrEmpty(supplier))
                    {
                        payments = payments.Where(x => x.SupplierId == supplier);
                    }

                    if (!String.IsNullOrEmpty(status))
                    {
                        if (status == "Transferred to GL")
                        {
                            payments = payments.Where(x => x.IsOnlineTransferred);
                        }
                        else if (status == "Not Transferred to GL")
                        {
                            payments = payments.Where(x => x.IsOnlineTransferred == false);
                        }
                    }

                    var records = payments.ToList();
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.ApPaymentDistribution)
                        {
                            var line = new ApGlTransferViewModel
                            {
                                TransactionNumber = record.ApPaymentId.ToString(),
                                TransactionDate = record.PaymentDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Reference = distribution.ApPayment.PaymentReference
                            };
                            lines.Add(line);

                        }

                    }

                    #endregion
                }

                if (String.IsNullOrEmpty(category) || category == "Direct Payment")
                {
                 
                    #region Payment direct


                    if (!String.IsNullOrEmpty(supplier))
                    {
                        directPayments = directPayments.Where(x => x.SupplierId == supplier);
                    }

                    if (!String.IsNullOrEmpty(status))
                    {
                        if (status == "Transferred to GL")
                        {
                            directPayments = directPayments.Where(x => x.IsOnlineTransferred);
                        }
                        else if (status == "Not Transferred to GL")
                        {
                            directPayments = directPayments.Where(x => x.IsOnlineTransferred == false);
                        }
                    }

                    var directrecords = directPayments.ToList();
                    foreach (var record in directrecords)
                    {
                        foreach (var distribution in record.ApDirectPaymentDistribution)
                        {
                            var line = new ApGlTransferViewModel
                            {
                                TransactionNumber = record.ApDirectPaymentId.ToString(),
                                TransactionDate = record.PaymentDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Reference = distribution.ApDirectPayment.PaymentReference
                            };
                            lines.Add(line);

                        }

                    }

                    #endregion
                }

                if (String.IsNullOrEmpty(category) || category == "Miscellaneous Payment")
                {
                    #region Miscellaneous Payment

                    if (!String.IsNullOrEmpty(status))
                    {
                        if (status == "Transferred to GL")
                        {
                            miscellaneousPayments = miscellaneousPayments.Where(x => x.IsOnlineTransferred);
                        }
                        else if (status == "Not Transferred to GL")
                        {
                            miscellaneousPayments = miscellaneousPayments.Where(x => x.IsOnlineTransferred == false);
                        }
                    }

                    var records = miscellaneousPayments.ToList();
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.ApMiscellaneousPaymentDistribution)
                        {
                            var line = new ApGlTransferViewModel
                            {
                                TransactionNumber = record.ApMiscellaneousPaymentId.ToString(),
                                TransactionDate = record.PaymentDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Reference = distribution.ApMiscellaneousPayment.PaymentReference
                            };
                            lines.Add(line);

                        }

                    }

                    #endregion
                }

                if (String.IsNullOrEmpty(category) || category == "Petty Cash")
                {
                    #region Petty Cash

                    if (!String.IsNullOrEmpty(status))
                    {
                        if (status == "Transferred to GL")
                        {
                            pettyCash = pettyCash.Where(x => x.IsOnlineTransferred);
                        }
                        else if (status == "Not Transferred to GL")
                        {
                            pettyCash = pettyCash.Where(x => x.IsOnlineTransferred == false);
                        }
                    }

                    var records = pettyCash.ToList();
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.PettyCashDistributions)
                        {
                            var line = new ApGlTransferViewModel
                            {
                                TransactionNumber = record.ApPettyCashId.ToString(),
                                TransactionDate = record.Date,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Reference = distribution.PettyCash.Reference
                            };
                            lines.Add(line);

                        }

                    }

                    #endregion
                }
                if (String.IsNullOrEmpty(category) || category == "Travel Order")
                {
                    #region Travel Order

                    if (!String.IsNullOrEmpty(status))
                    {
                        if (status == "Transferred to GL")
                        {
                            travelOrders = travelOrders.Where(x => x.IsOnlineTransferred);
                        }
                        else if (status == "Not Transferred to GL")
                        {
                            travelOrders = travelOrders.Where(x => x.IsOnlineTransferred == false);
                        }
                    }

                    var records = travelOrders.ToList();
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.ApTravelOrderDistributions)
                        {
                            var line = new ApGlTransferViewModel
                            {
                                TransactionNumber = record.ApTravelOrderId.ToString(),
                                TransactionDate = record.RequestDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Reference = distribution.TravelOrder.Reference
                            };
                            lines.Add(line);

                        }

                    }

                    #endregion
                }
                if (String.IsNullOrEmpty(category) || category == "Expense Claim")
                {
                    #region Expense Claim

                    if (!String.IsNullOrEmpty(status))
                    {
                        if (status == "Transferred to GL")
                        {
                            expenseClaims = expenseClaims.Where(x => x.IsOnlineTransferred);
                        }
                        else if (status == "Not Transferred to GL")
                        {
                            expenseClaims = expenseClaims.Where(x => x.IsOnlineTransferred == false);
                        }
                    }

                    var records = expenseClaims.ToList();
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.Distributions)
                        {
                            var line = new ApGlTransferViewModel
                            {
                                TransactionNumber = record.ApTravelExpenseClaimId.ToString(),
                                TransactionDate = record.ClaimDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Reference = distribution.ApTravelExpenseClaim.Reference
                            };
                            lines.Add(line);

                        }

                    }

                    #endregion
                }
            }

            return lines;
        }
    }
}
