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
    public class ArJournalEntriesOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public Organization ArJournalHeader()
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
        
        public List<ArJournalEntriesCategoryViewModel> ArJournalEntriesGl(string category,string customer,string branchid,string dateFrom,string dateTo)
        {
     
            List<ArJournalEntriesCategoryViewModel> arJournalEntriesGl = new List<ArJournalEntriesCategoryViewModel>();
            var invoices = _db.ARInvoices.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Select(x => new { x.CustomerId, x.Date, x.ArInvoiceDistribution, x.OrgBranchName,x.ArInvoiceId,x.Reference }).AsQueryable();
            var collections = _db.ARStandardCollections.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Select(x => new { x.CustomerId, x.ReceiptDate, x.ARStandardCollectionDistribution, x.ArStandardCollectionId,x.OrgBranchName,x.ReceiptNumber,x.Reference }).AsQueryable();
            var miscelaneousCollections = _db.ARMiscelaneousCollections.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Select(x => new { x.CustomerId, x.ReceiptDate, x.ARMiscelaneousCollectionDistribution, x.ARMiscelaneousCollectionID,x.OrgBranchName,x.ReceiptNumber,x.Remark }).AsQueryable();
            if (!String.IsNullOrEmpty(dateFrom) && !String.IsNullOrEmpty(dateTo))
            {
                DateTime from = DateTime.Parse(dateFrom).Date;
                DateTime to = DateTime.Parse(dateTo).Date;

                if (String.IsNullOrEmpty(category) || category == "Invoice")
                {
                    #region Invoice

                    invoices = invoices.Where(x => x.Date >= from && x.Date <= to);
                    if (!String.IsNullOrEmpty(customer))
                    {
                        invoices = invoices.Where(x => x.CustomerId == customer);
                    }

                    if (!String.IsNullOrEmpty(branchid))
                    {
                        invoices = invoices.Where(x => x.OrgBranchName == branchid);
                    }

                    var records = invoices.ToList();
                    var arcategory = new ArJournalEntriesCategoryViewModel
                    {
                        Category = "Invoice",
                        ArJournalEntriesOdsViewModels = new List<ArJournalEntriesOdsViewModel>()
                    };
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.ArInvoiceDistribution)
                        {
                            var line = new ArJournalEntriesOdsViewModel
                            {
                                TransactionNumber = record.ArInvoiceId.ToString(),
                                TransactionDate = record.Date,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Reference = record.Reference
                            };

                            arcategory.ArJournalEntriesOdsViewModels.Add(line);

                        }

                    }
                    arJournalEntriesGl.Add(arcategory);

                    #endregion
                }

                if (String.IsNullOrEmpty(category) || category == "Collection")
                {
                    #region Collection

                    collections = collections.Where(x => x.ReceiptDate >= from && x.ReceiptDate <= to);
                    if (!String.IsNullOrEmpty(customer))
                    {
                        collections = collections.Where(x => x.CustomerId == customer);
                    }

                    if (!String.IsNullOrEmpty(branchid))
                    {
                        collections = collections.Where(x => x.OrgBranchName == branchid);
                    }


                    var records = collections.ToList();
                    var arcategory = new ArJournalEntriesCategoryViewModel
                    {
                        Category = "Collection",
                        ArJournalEntriesOdsViewModels = new List<ArJournalEntriesOdsViewModel>()
                    };
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.ARStandardCollectionDistribution)
                        {
                            var line = new ArJournalEntriesOdsViewModel
                            {

                                TransactionNumber = record.ArStandardCollectionId.ToString(),
                                TransactionDate = record.ReceiptDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Reference = record.Reference,
                                CollectionReference = record.ReceiptNumber
                            };
                            arcategory.ArJournalEntriesOdsViewModels.Add(line);

                        }

                    }
                    arJournalEntriesGl.Add(arcategory);

                    #endregion
                }
                if (String.IsNullOrEmpty(category) || category == "Miscellaneous Collection")
                {
                    #region Miscellaneous Collection

                    miscelaneousCollections =
                        miscelaneousCollections.Where(x => x.ReceiptDate >= from && x.ReceiptDate <= to);

                    if (!String.IsNullOrEmpty(customer))
                    {
                        miscelaneousCollections = miscelaneousCollections.Where(x => x.CustomerId == customer);
                    }

                    if (!String.IsNullOrEmpty(branchid))
                    {
                        miscelaneousCollections = miscelaneousCollections.Where(x => x.OrgBranchName == branchid);
                    }


                    var records = miscelaneousCollections.ToList();
                    var arcategory = new ArJournalEntriesCategoryViewModel
                    {
                        Category = "Miscellaneous Collection",
                        ArJournalEntriesOdsViewModels = new List<ArJournalEntriesOdsViewModel>()
                    };
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.ARMiscelaneousCollectionDistribution)
                        {
                            var line = new ArJournalEntriesOdsViewModel
                            {
                                TransactionNumber = record.ARMiscelaneousCollectionID.ToString(),
                                TransactionDate = record.ReceiptDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit,
                                Reference = record.Remark,
                                CollectionReference = record.ReceiptNumber
                            };
                            arcategory.ArJournalEntriesOdsViewModels.Add(line);

                        }

                    }
                    arJournalEntriesGl.Add(arcategory);

                    #endregion
                }
            }
            return arJournalEntriesGl;
        }
    }
}
