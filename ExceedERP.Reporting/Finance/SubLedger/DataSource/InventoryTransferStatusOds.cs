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
    public class InventoryTransferStatusOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public Organization InventoryTransferHeader()
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
        public List<InventoryGlTransferViewModel> InventoryTransferToGl(string category,string status,string storecode,string dateFrom,string dateTo)
        {
            List<InventoryGlTransferViewModel> lines = new List<InventoryGlTransferViewModel>();
            if (!String.IsNullOrEmpty(dateFrom) && !String.IsNullOrEmpty(dateTo))
            {
                DateTime from = DateTime.Parse(dateFrom);
                DateTime to = DateTime.Parse(dateTo);
                var goodreceive = _db.GoodReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Select(x => new { x.ReceiveDate,x.StoreCode,x.IsOnlineTransferred,x.GoodReceiveDistribution,x.GoodReceiveId }).AsQueryable();
                var issue = _db.Issues.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Select(x => new { x.IssueDate, x.StoreCode, x.IsOnlineTransferred, x.IssueDistribution, x.IssueID }).AsQueryable();
                var transfer = _db.ItemTransfers.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Select(x => new { x.TransferDate, x.FromStore, x.IsOnlineTransferred, x.ItemTransferDistribution, x.ItemTransferID }).AsQueryable();
                var returns = _db.StoreReturns.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Select(x => new { x.ReturnDate, x.StoreCode, x.IsOnlineTransferred, x.StoreReturnDistribution, x.StoreReturnID }).AsQueryable();
                if (String.IsNullOrEmpty(category) || category == "GRN")
                {
                    #region good receive


                    goodreceive = goodreceive.Where(x => x.ReceiveDate >= from && x.ReceiveDate <= to);

                    if (!String.IsNullOrEmpty(storecode))
                    {
                        goodreceive = goodreceive.Where(x => x.StoreCode == storecode);
                    }
                    if (!String.IsNullOrEmpty(status))
                    {
                        if (status == "Transferred to GL")
                        {
                            goodreceive = goodreceive.Where(x => x.IsOnlineTransferred);
                        }
                        else if (status == "Not Transferred to GL")
                        {
                            goodreceive = goodreceive.Where(x => x.IsOnlineTransferred == false);
                        }
                    }

                    var records = goodreceive.ToList();
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.GoodReceiveDistribution)
                        {
                            var line = new InventoryGlTransferViewModel
                            {
                                TransactionNumber = record.GoodReceiveId.ToString(),
                                TransactionDate = record.ReceiveDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit
                            };
                            lines.Add(line);

                        }

                    }

                    #endregion
                }
                if (String.IsNullOrEmpty(category) || category == "Issue")
                {
                    #region issue

                    issue = issue.Where(x => x.IssueDate >= from && x.IssueDate <= to);

                    if (!String.IsNullOrEmpty(storecode))
                    {
                        issue = issue.Where(x => x.StoreCode == storecode);
                    }
                    if (!String.IsNullOrEmpty(status))
                    {
                        if (status == "Transferred to GL")
                        {
                            issue = issue.Where(x => x.IsOnlineTransferred);
                        }
                        else if (status == "Not Transferred to GL")
                        {
                            issue = issue.Where(x => x.IsOnlineTransferred == false);
                        }
                    }

                    var records = issue.ToList();
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.IssueDistribution)
                        {
                            var line = new InventoryGlTransferViewModel
                            {
                                TransactionNumber = record.IssueID.ToString(),
                                TransactionDate = record.IssueDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit
                            };
                            lines.Add(line);

                        }

                    }

                    #endregion
                }
                if (String.IsNullOrEmpty(category) || category == "Transfer")
                {
                    #region transfer

                    transfer = transfer.Where(x => x.TransferDate >= from && x.TransferDate <= to);

                    if (!String.IsNullOrEmpty(storecode))
                    {
                        transfer = transfer.Where(x => x.FromStore == storecode);
                    }
                    if (!String.IsNullOrEmpty(status))
                    {
                        if (status == "Transferred to GL")
                        {
                            transfer = transfer.Where(x => x.IsOnlineTransferred);
                        }
                        else if (status == "Not Transferred to GL")
                        {
                            transfer = transfer.Where(x => x.IsOnlineTransferred == false);
                        }
                    }

                    var records = transfer.ToList();
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.ItemTransferDistribution)
                        {
                            var line = new InventoryGlTransferViewModel
                            {
                                TransactionNumber = record.ItemTransferID.ToString(),
                                TransactionDate = record.TransferDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit
                            };
                            lines.Add(line);

                        }

                    }

                    #endregion
                }
                if (String.IsNullOrEmpty(category) || category == "Store Return")
                {
                    #region return

                    returns = returns.Where(x => x.ReturnDate >= from && x.ReturnDate <= to);

                    if (!String.IsNullOrEmpty(storecode))
                    {
                        returns = returns.Where(x => x.StoreCode == storecode);
                    }
                    if (!String.IsNullOrEmpty(status))
                    {
                        if (status == "Transferred to GL")
                        {
                            returns = returns.Where(x => x.IsOnlineTransferred);
                        }
                        else if (status == "Not Transferred to GL")
                        {
                            returns = returns.Where(x => x.IsOnlineTransferred == false);
                        }
                    }

                    var records = returns.ToList();
                    foreach (var record in records)
                    {
                        foreach (var distribution in record.StoreReturnDistribution)
                        {
                            var line = new InventoryGlTransferViewModel
                            {
                                TransactionNumber = record.StoreReturnID.ToString(),
                                TransactionDate = record.ReturnDate,
                                AccountCode = distribution.AccountCode,
                                Debit = distribution.Debit,
                                Credit = distribution.Credit
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
