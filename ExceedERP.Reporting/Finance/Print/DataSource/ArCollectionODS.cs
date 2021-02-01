using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel.Print;
using ExceedERP.Reporting.SharedDataSources;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.Finance.DataSource.Print
{
    [DataObject]
    public class ArCollectionOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ArCollectionViewModel ArCollectionHeader(int id)
        {
            var collection = _db.ARStandardCollections.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ArCollectionViewModel headerDs = new ArCollectionViewModel();

            if (collection != null)
            {
                headerDs.VoucherId = collection.ArStandardCollectionId.ToString();
                var customer = _db.ARCustomerProfiles.FirstOrDefault(x => x.CustomerId == collection.CustomerId);
                if (customer != null) headerDs.CustomerName = customer.CompanyName;
                headerDs.CustomerId = collection.CustomerId;
                headerDs.ReceiptAmount = collection.ARStandardCollectionLines.Sum(x=>x.NetAmount);
                headerDs.ReceiptNumber = collection.ReceiptNumber;
                headerDs.ReceiptDate = collection.ReceiptDate;
                headerDs.ReceiptMethod = collection.ReceiptMethod;
                headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(collection.CreatedBy)); 
                headerDs.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(collection.IsOnlineTransferredBy));
            }

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
        public List<ArCollectionLinesViewModel> ApCollectionLines(int id)
        {
            List<ArCollectionLinesViewModel> lines = new List<ArCollectionLinesViewModel>();
            var collectionLines = _db.ARStandardCollectionLines.Where(x => x.ArStandardCollectionId == id).ToList();
            foreach (var line in collectionLines)
            {
                ArCollectionLinesViewModel record = new ArCollectionLinesViewModel
                {
                    IsPrepayment = line.IsPrepayment,
                    InvoiceNumber = line.InvoiceNumber,
                    InstallementNo = line.InstallmentNo,
                    ApplyAmount = line.ApplyAmount,
                    BalanceDue = line.BalanceDue,
                    ApplyDate = line.ApplyDate,
                    TaxType = line.TaxType,
                    TaxAmount = line.TaxAmount,
                    WithholdingType = line.WithholdingType,
                    WithholdingAmount = line.WithholdingAmount,
                    WithholdTax = line.WithholdTax,
                    WithholdTaxAmount = line.WithholdTaxAmount,
                    NetAmount = line.NetAmount,
                    HasPrepayment = line.HasPrepayment,
                    PrepaymentAmount = line.PrepaymentAmount
                };

                lines.Add(record);

            }
            return lines;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AccountDistributionViewModel> ArCollectionDistribution(int id)
        {
            List<AccountDistributionViewModel> lines = new List<AccountDistributionViewModel>();
            var distributions = _db.ARStandardCollectionDistributions.Where(x => x.ArStandardCollectionId == id).ToList();
            foreach (var distribution in distributions)
            {
                AccountDistributionViewModel record = new AccountDistributionViewModel
                {
                    AccountCode = distribution.AccountCode,
                    AccountDesc = distribution.AccountDesc,
                    Debit = distribution.Debit,
                    Credit = distribution.Credit
                };

                lines.Add(record);

            }
            return lines;
        }
    }
}
