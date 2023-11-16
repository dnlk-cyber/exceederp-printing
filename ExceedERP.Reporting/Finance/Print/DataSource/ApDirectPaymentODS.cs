using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM;
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
                if (!string.IsNullOrEmpty(payment.SupplierId))
                {
                    var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == payment.SupplierId);
                    if (supplier != null && !string.IsNullOrEmpty(supplier.Name))
                        headerDs.SupplierName = supplier.Name;
                }
                else
                {
                    headerDs.SupplierName = payment.SupplierName;
                }

                if (!String.IsNullOrWhiteSpace(payment.SourceId))
                {

                    #region requestor/payee

                    int did = int.Parse(payment.SourceId.Split('_')[1]);
                    string sourceType = payment.SourceId.Split('_')[0];
                    switch (sourceType)
                    {
                        case "LC":
                            var purchaseCost = _db.ForeignPurchaseCosts.Find(did);
                            if (purchaseCost != null)
                                headerDs.RequestorName = purchaseCost.Payee;
                            break;
                        case "PAYMENT-REQUEST":
                            var paymentRequest = _db.PaymentRequests.Find(did);
                            if (paymentRequest != null)
                            {
                                headerDs.RequestorName = EmployeeHelper.GetEmployeeEnglishName(paymentRequest.EmployeeId);
                            }
                            break;
                        case "TRAVEL-ORDER":
                            var travelOrder = _db.ApTravelOrders.Find(did);
                            if (travelOrder != null)
                            {
                                headerDs.RequestorName = EmployeeHelper.GetEmployeeEnglishName(travelOrder.EmployeeId);
                            }
                            break;
                        case "EXPENSE-CLAIM":
                            var claim = _db.ApTravelExpenseClaims.Find(did);
                            if (claim != null)
                            {
                                headerDs.RequestorName = EmployeeHelper.GetEmployeeEnglishName(claim.EmployeeId);
                            }
                            break;
                        case "FUND-TRANSFER":
                            var fund = _db.FundTransfers.Find(did);
                            if (fund != null)
                            {
                                headerDs.RequestorName = EmployeeHelper.GetEmployeeEnglishName(fund.EmployeeId);
                            }
                            break;

                    }
              #endregion

                }

                headerDs.VoucherId = payment.ApDirectPaymentId.ToString();
                headerDs.PaymentRequest = payment.SourceId;
                headerDs.DocTitile = "PAYMENT VOUCHER (Direct)";
                headerDs.NetPayable = netpayment;
                headerDs.PaymentReference = payment.PaymentReference;
                headerDs.PaymentMethod = payment.PaymentMethod;
                headerDs.PaymentDate = payment.PaymentDate;
                headerDs.Remark = payment.Remark;

                headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(payment.CreatedBy));
                headerDs.PreparedDate = payment.DateCreated != null ? payment.DateCreated : null;
                headerDs.PreparedBySignature = SignatureHelper.GetUserSignature(payment.CreatedBy);

                headerDs.AccountantCheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(payment.SendForApprovalBy));
                headerDs.AccountantCheckedDate = payment.SendForApprovalTime != null ? payment.SendForApprovalTime : null;
                headerDs.AccountantCheckerSignature = SignatureHelper.GetUserSignature(payment.SendForApprovalBy);

                headerDs.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(payment.OnlineApprovedBy));
                headerDs.ApprovedDate = payment.OnlineApprovedTime;
                headerDs.ApproverSignature = SignatureHelper.GetUserSignature(payment.OnlineApprovedBy);


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
                            a.ReferenceType == Core.Domain.Common.AttachmentType.LOGO);
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
