using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Finance.ViewModel.Print;
using ExceedERP.Reporting.SharedDataSources;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Finance.DataSource.Print
{
    [DataObject]
    public class ApPaymentOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ApPaymentViewModel ApPaymentHeader(int id)
        {
            var payment = _db.APPayments.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ApPaymentViewModel headerDs = new ApPaymentViewModel();

            if (payment != null)
            {
                headerDs.VoucherId = payment.ApPaymentId.ToString();
                var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == payment.SupplierId);
                if (supplier != null) headerDs.SupplierName = supplier.Name;
                headerDs.SupplierId = payment.SupplierId;
                headerDs.IsPrepayment = payment.IsPrepayment;
                headerDs.InvoiceNumber = payment.InvoiceNumber;
                headerDs.InvoiceAmount = payment.InvoiceAmount;
                headerDs.PaymentAmount = payment.PaymentAmount;
                headerDs.PrepaymentAmount = payment.PrepaymentAmount;
                headerDs.RemainingBalance = payment.RemainingBalance;
                headerDs.PaymentMethod = payment.PaymentMethod;
                headerDs.PaymentReference = payment.PaymentReference;
                headerDs.PaymentDate = payment.PaymentDate;
                headerDs.NetAmount = payment.NetAmount;
                headerDs.HasPrepayment = payment.HasPrepayment;
                headerDs.PrepaymentAmount = payment.PrepaymentAmount;
                headerDs.Remark = payment.Remark;
                headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(payment.CreatedBy));
                headerDs.PreparedDate = payment.DateCreated != null ? payment.DateCreated : null;

                headerDs.AccountantCheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(payment.SendForApprovalBy));
                headerDs.AccountantCheckedDate = payment.SendForApprovalTime != null ? payment.SendForApprovalTime : null;

                headerDs.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(payment.OnlineApprovedBy));
                headerDs.ApprovedDate = payment.OnlineApprovedTime;
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
        public List<AccountDistributionViewModel> ApPaymentDistribution(int id)
        {
            List<AccountDistributionViewModel> lines = new List<AccountDistributionViewModel>();
            var distributions = _db.APPaymentDistributions.Where(x => x.ApPaymentId == id).ToList();
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
