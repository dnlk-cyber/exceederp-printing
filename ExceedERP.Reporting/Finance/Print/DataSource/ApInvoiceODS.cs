using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Finance.ViewModel.Print;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Finance.DataSource.Print
{
    [DataObject]
    public class ApInvoiceOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ApInvoiceViewModel ApInvoiceHeader(int id)
        {
            var invoice = _db.APPaybleInvoices.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ApInvoiceViewModel headerDs = new ApInvoiceViewModel();

            if (invoice != null)
            {
                headerDs.VoucherId = invoice.ApPaybleInvoicesId.ToString();
                var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == invoice.SupplierId);
                if (supplier != null) headerDs.SupplierName = supplier.Name;
                headerDs.SupplierId = invoice.SupplierId;
                if (invoice.EmployeeId != null)
                    headerDs.Purchaser = EmployeeHelper.GetEmployeeEnglishName((int) invoice.EmployeeId);
                headerDs.PoNum = invoice.PoNum;
                headerDs.GrnSrn = invoice.Grn;
                headerDs.PrNo = invoice.PrNo;
                headerDs.PurposeOfPayment = invoice.PurposeOfPayment;
                headerDs.InvoiceDate = invoice.InvoiceDate;
                headerDs.InvoiceNumber = invoice.InvoiceNumber;
                headerDs.PaymentTerms = invoice.PaymentTerms;
                headerDs.InvoiceAmount = invoice.InvoiceAmount;
                headerDs.AdvanceAmount = invoice.AdvanceAmount;
                headerDs.TaxAmount = invoice.TaxAmount;
                headerDs.WithholdingAmount = invoice.WithholdingAmount;
                headerDs.Subtotal = invoice.Subtotal;
                headerDs.WithholdTaxAmount = invoice.WithholdTaxAmount;
                headerDs.NetPayable = invoice.NetPayable;
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
        public List<ApInvoiceLinesViewModel> ApInvoiceLines(int id)
        {
            List<ApInvoiceLinesViewModel> lines = new List<ApInvoiceLinesViewModel>();
            var invoiceLines = _db.APPaybleInvoicesLines.Where(x => x.ApPaybleInvoicesId == id).ToList();
            foreach (var line in invoiceLines)
            {
                ApInvoiceLinesViewModel record = new ApInvoiceLinesViewModel
                {
                    ItemName = line.Name,
                    Description=line.Description,
                    AccountCode = line.AccountCode,
                    AccountDesc = line.AccountDesc,
                    InvoiceAmount = line.InvoiceAmount,
                    AdvanceAmount = line.AdvanceAmount,
                    NetPayable=line.NetPayable,
                    ReceiptNo=line.ReceiptNo,
                    TaxType=line.TaxType,
                    TaxAmount=line.TaxAmount,
                    WithholdingType=line.WithholdingType,
                    WithholdingAmount=line.WithholdingAmount,
                    WithholdTax=line.WithholdTax,
                    WithholdTaxAmount=line.WithholdTaxAmount,
                    PoNo=line.PoNo
                };

                lines.Add(record);

            }
            return lines;
        }
    }
}
