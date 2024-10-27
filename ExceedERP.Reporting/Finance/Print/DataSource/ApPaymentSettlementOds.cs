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
    public class ApPaymentSettlementOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ApMiscPaymentViewModel PaymentSettlementHeader(int id)
        {
            var paymentRequest = _db.ApPaymentSettlements.Find(id);
            decimal netpayment = 0;
            var paymentlines = _db.ApPaymentSettlementLines.Where(x=>x.ApPaymentSettlementId==id);
            if(paymentlines.Any())
                netpayment = paymentlines.Sum(x => x.Amount);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ApMiscPaymentViewModel headerDs = new ApMiscPaymentViewModel();

            if (paymentRequest != null)
            {
                headerDs.VoucherId = paymentRequest.ApPaymentSettlementId.ToString();
                headerDs.DocTitile = "PAYMENT SETTLEMENT";
                headerDs.SupplierName = "";
                headerDs.NetPayable = netpayment;
                headerDs.PaymentReference = paymentRequest.Reference;
                headerDs.PaymentDate = paymentRequest.PaymentDate;
                headerDs.Remark = paymentRequest.Remark;

                //department
                headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.CreatedBy));
                headerDs.PreparedDate = paymentRequest.DateCreated != null ? paymentRequest.DateCreated : null;
                headerDs.PreparedBySignature = SignatureHelper.GetUserSignature(paymentRequest.CreatedBy);

                headerDs.DepartmentCheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.DepartmentSendForApprovalBy));
                headerDs.DepartmentCheckedDate = paymentRequest.DepartmentOnlineApprovedTime != null ? paymentRequest.DepartmentSendForApprovalTime : null;
                headerDs.DepartmentCheckerSignature = SignatureHelper.GetUserSignature(paymentRequest.DepartmentSendForApprovalBy);

                headerDs.DepartmentApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.DepartmentOnlineApprovedBy));
                headerDs.DepartmentApprovedDate = paymentRequest.DepartmentOnlineApprovedTime != null ? paymentRequest.DepartmentOnlineApprovedTime : null;
                headerDs.DepartmentApproverSignature = SignatureHelper.GetUserSignature(paymentRequest.DepartmentOnlineApprovedBy);
                //finance
                headerDs.AccountantCheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.SendForApprovalBy));
                headerDs.AccountantCheckedDate = paymentRequest.SendForApprovalTime != null ? paymentRequest.SendForApprovalTime : null;
                headerDs.AccountantCheckerSignature = SignatureHelper.GetUserSignature(paymentRequest.SendForApprovalBy);

                headerDs.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.OnlineApprovedBy));
                headerDs.ApprovedDate = paymentRequest.OnlineApprovedTime != null ? paymentRequest.OnlineApprovedTime : null;
                headerDs.ApproverSignature = SignatureHelper.GetUserSignature(paymentRequest.OnlineApprovedBy);
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
        public List<ApPaymentSettlementLine> ApPaymentSettlementLine(int id)
        {
            return _db.ApPaymentSettlementLines.Where(x => x.ApPaymentSettlementId == id).ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ApPaymentSettlementDistribution> ApPaymentSettlementDistribution(int id)
        {
            return _db.ApPaymentSettlementDistributions.Where(x => x.ApPaymentSettlementId == id).ToList();
        }
    }
}
