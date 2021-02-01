using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel.Print;
using ExceedERP.Reporting.SharedDataSources;
using ArInvoiceViewModel = ExceedERP.Reporting.Finance.ViewModel.Print.ArInvoiceViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.Print
{
    [DataObject]
    public class ArInvoiceOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ArInvoiceViewModel ArInvoiceHeader(int id)
        {
            var invoice = _db.ARInvoices.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ArInvoiceViewModel headerDs = new ArInvoiceViewModel();

            if (invoice != null)
            {
                headerDs.VoucherId = invoice.ArInvoiceId.ToString();
                headerDs.CustomerId = invoice.CustomerId;
                headerDs.CustomerName = invoice.CustomerName;
                headerDs.TransactionClass = invoice.TransactionClass;
                headerDs.Date = invoice.Date;
                headerDs.PaymentTerm = invoice.PaymentTerm;
                headerDs.DueDate = invoice.DueDate;
                headerDs.LineAmount = invoice.LineAmount;
                headerDs.TaxAmount = invoice.TaxAmount;
                headerDs.TotalAmount = invoice.TotalAmount;
                headerDs.AmountDue = invoice.AmountDue;
                headerDs.PreparedBy = invoice.CreatedBy;
                headerDs.ApprovedBy = invoice.OnlineApprovedBy;

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
        public List<ArInvoiceLinesViewModel> ArInvoiceLines(int id)
        {
            List<ArInvoiceLinesViewModel> lines = new List<ArInvoiceLinesViewModel>();
            var invoiceLines = _db.ARInvoiceLines.Where(x => x.ArInvoiceId == id).ToList();
            foreach (var line in invoiceLines)
            {
                var taxRate = _db.GLTaxRates.Find(line.GLTaxRateID);
                if (taxRate != null)
                {
                    ArInvoiceLinesViewModel record = new ArInvoiceLinesViewModel
                    {
                        Items = line.Items,
                        Description=line.Description,
                        AccountCode = line.AccountCode,
                        AccountDesc = line.AccountDesc,
                        Quantity = line.Quantity,
                        UnitPrice = line.UnitPrice,
                        SubTotal= line.SubTotal,
                        TaxType=taxRate.TaxName,
                        TaxAmount=line.TaxAmount,
                        TotalAmount= line.TotalAmount
                    };

                    lines.Add(record);
                }
            }
            return lines;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ArInvoiceDistribution> ArInvoiceDistributions(int id)
        {
            return _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == id).ToList();
        }
    }
}
