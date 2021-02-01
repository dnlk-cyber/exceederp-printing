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
    public class ArTransferStatusOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public Organization ArTransferHeader()
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
        public List<ArGlTransferViewModel> ArTransferToGl(string category,string status,string customer,string dateFrom,string dateTo)
        {
            List<ArGlTransferViewModel> lines = new List<ArGlTransferViewModel>();
            if (!String.IsNullOrEmpty(dateFrom) && !String.IsNullOrEmpty(dateTo))
            {
                DateTime from = DateTime.Parse(dateFrom).Date;
                DateTime to = DateTime.Parse(dateTo).Date;
                var invoices = _db.ARInvoices.Where(x => x.IsOnlineApproved&&x.IsVoid==false)
                    .Select(x => new { x.CustomerId, x.Date, x.ArInvoiceDistribution, x.OrgBranchName, x.ArInvoiceId, x.Reference,x.IsOnlineTransferred }).AsQueryable();
                var standardCollections = _db.ARStandardCollections.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Select(x => new { x.CustomerId, x.ReceiptDate, x.ARStandardCollectionDistribution, x.ArStandardCollectionId, x.OrgBranchName, x.ReceiptNumber, x.Reference,x.IsOnlineTransferred }).AsQueryable();
                var miscelaneousCollections = _db.ARMiscelaneousCollections.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Select(x => new { x.CustomerId, x.ReceiptDate, x.ARMiscelaneousCollectionDistribution, x.ARMiscelaneousCollectionID, x.OrgBranchName, x.ReceiptNumber, x.Remark,x.IsOnlineTransferred }).AsQueryable();
                if (String.IsNullOrEmpty(category) || category == "Invoice")
                {
                   
                    #region Invoice

                    invoices = invoices.Where(x => x.Date >= from && x.Date <= to);

                    if (!String.IsNullOrEmpty(customer))
                    {
                        standardCollections = standardCollections.Where(x => x.CustomerId == customer);
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
                        foreach (var distribution in record.ArInvoiceDistribution)
                        {
                            var line = new ArGlTransferViewModel
                            {
                                TransactionNumber = record.ArInvoiceId.ToString(),
                                TransactionDate = record.Date,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Reference = distribution.ArInvoice.Reference
                            };
                            lines.Add(line);

                        }

                    }

                    #endregion
                }

                if (String.IsNullOrEmpty(category) || category == "Collection")
                {
                    #region Collection

                    standardCollections =
                        standardCollections.Where(x => x.ReceiptDate >= from && x.ReceiptDate <= to);
                    if (!String.IsNullOrEmpty(customer))
                    {
                        standardCollections = standardCollections.Where(x => x.CustomerId == customer);
                    }
                    if (!String.IsNullOrEmpty(status))
                    {
                        if (status == "Transferred to GL")
                        {
                            standardCollections = standardCollections.Where(x => x.IsOnlineTransferred);
                        }
                        else if (status == "Not Transferred to GL")
                        {
                            standardCollections = standardCollections.Where(x => x.IsOnlineTransferred == false);
                        }
                    }

                    var records = standardCollections.ToList();
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.ARStandardCollectionDistribution)
                        {
                            var line = new ArGlTransferViewModel
                            {
                                TransactionNumber = record.ArStandardCollectionId.ToString(),
                                TransactionDate = record.ReceiptDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Reference = distribution.ArStandardCollection.Reference
                            };
                            lines.Add(line);

                        }

                    }

                    #endregion
                }
                if (String.IsNullOrEmpty(category) || category == "Miscellaneous Collection")
                {
                    #region Miscellaneous Collection

                    miscelaneousCollections =
                        miscelaneousCollections.Where(x => x.ReceiptDate >= from && x.ReceiptDate <= to);

                    if (!String.IsNullOrEmpty(status))
                    {
                        if (status == "Transferred to GL")
                        {
                            miscelaneousCollections = miscelaneousCollections.Where(x => x.IsOnlineTransferred);
                        }
                        else if (status == "Not Transferred to GL")
                        {
                            miscelaneousCollections =
                                miscelaneousCollections.Where(x => x.IsOnlineTransferred == false);
                        }
                    }

                    var records = miscelaneousCollections.ToList();
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.ARMiscelaneousCollectionDistribution)
                        {
                            var line = new ArGlTransferViewModel
                            {
                                TransactionNumber = record.ARMiscelaneousCollectionID.ToString(),
                                TransactionDate = record.ReceiptDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Reference = distribution.ARMiscelaneousCollection.Remark
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
