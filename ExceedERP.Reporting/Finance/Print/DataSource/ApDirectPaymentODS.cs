using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Finance.ViewModel.Print;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Finance.DataSource.Print
{
    [DataObject]
    public class ApDirectPaymentOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ApMiscPaymentViewModel DirectPaymentHeader(int id)
        {
            var payment = _db.ApDirectPayments.Find(id);
            decimal netpayment = 0;
            var paymentlines = _db.ApDirectPaymentLines.Where(x=>x.ApDirectPaymentId==id);
            if(paymentlines.Any())
                netpayment = paymentlines.Sum(x => x.NetPayable);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ApMiscPaymentViewModel headerDs = new ApMiscPaymentViewModel();

            if (payment != null)
            {
                headerDs.VoucherId = payment.ApDirectPaymentId.ToString();
                headerDs.PaymentRequest = payment.SourceId;
                headerDs.DocTitile = "PAYMENT VOUCHER";
                headerDs.SupplierName = payment.SupplierName;
                headerDs.NetPayable = netpayment;
                headerDs.PaymentReference = payment.PaymentReference;
                headerDs.PaymentMethod = payment.PaymentMethod;
                headerDs.PaymentDate = payment.PaymentDate;
                headerDs.Remark = payment.Remark;
                headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(payment.CreatedBy));
                headerDs.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(payment.OnlineApprovedBy));
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
        public List<ApDirectPaymentLine> ApDirectPaymentLine(int id)
        {
            return _db.ApDirectPaymentLines.Where(x => x.ApDirectPaymentId == id).ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ApDirectPaymentDistribution> ApDirectPaymentDistribution(int id)
        {
            return _db.ApDirectPaymentDistributions.Where(x => x.ApDirectPaymentId == id).ToList();
        }
    }
}
