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
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.Print
{
    [DataObject]
    public class ApPaymentRequestNewOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ApPaymentRequestVm ApPaymentRequestrHeader(int id)
        {
            var paymentRequest = _db.PaymentRequests.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ApPaymentRequestVm headerDs = new ApPaymentRequestVm();

            if (paymentRequest != null)
            {
                headerDs.VoucherId = paymentRequest.PaymentRequestId.ToString();
                if(paymentRequest.EmployeeId!=null)
                     headerDs.Employee = EmployeeHelper.GetEmployeeEnglishName((int)paymentRequest.EmployeeId);
                if (paymentRequest.Purchaser != null)
                    headerDs.Purchaser = EmployeeHelper.GetEmployeeEnglishName((int)paymentRequest.Purchaser);
                headerDs.ReasonOfPayment = paymentRequest.ReasonOfPayment;
                headerDs.RequestDate = paymentRequest.RequestDate;

                headerDs.Supplier = paymentRequest.Supplier;
                headerDs.InvoiceNo = paymentRequest.InvoiceNo;
                headerDs.PurchaseRequestNo = paymentRequest.PurchaseRequestNo;
                headerDs.PONo = paymentRequest.PONo;
                headerDs.GRN = paymentRequest.GRN;
                headerDs.Branch = _db.Branch.Find(paymentRequest.BranchId)?.Name;
                headerDs.WorkUnit = _db.MainCompanyStructures.Find(paymentRequest.OrgStuructureId)?.Name;

                var lines = _db.PaymentRequestLines.Where(x => x.PaymentRequestId == id).ToList();
                if (lines.Any())
                    headerDs.Total = lines.Sum(x=>x.TotalAmount);

                //department
                headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.CreatedBy));
                headerDs.PreparedDate = paymentRequest.DateCreated != null ? paymentRequest.DateCreated : null;
                headerDs.PreparedBySignature = SignatureHelper.GetUserSignature(paymentRequest.CreatedBy);

                headerDs.DepartmentSecondApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.DepartmentOnlineApprovedBy));
                headerDs.DepartmentSecondApprovedDate = paymentRequest.DepartmentOnlineApprovedTime != null ? paymentRequest.DepartmentOnlineApprovedTime : null;
                headerDs.DepartmentSecondApproverSignature = SignatureHelper.GetUserSignature(paymentRequest.DepartmentOnlineApprovedBy);
                //finance
                headerDs.AccountantCheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.SendForApprovalBy));
                headerDs.AccountantCheckedDate = paymentRequest.SendForApprovalTime != null ? paymentRequest.SendForApprovalTime : null;
                headerDs.AccountantCheckerSignature = SignatureHelper.GetUserSignature(paymentRequest.SendForApprovalBy);

                headerDs.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.OnlineApprovedBy));
                headerDs.ApprovedDate = paymentRequest.OnlineApprovedTime;
                headerDs.ApproverSignature = SignatureHelper.GetUserSignature(paymentRequest.OnlineApprovedBy);
                //budget
                headerDs.BudgetApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.BudgetApprovedBy));
                headerDs.BudgetApprovedDate = paymentRequest.BudgetApprovedTime != null ? paymentRequest.BudgetApprovedTime : null;
                headerDs.BudgetSignature = SignatureHelper.GetUserSignature(paymentRequest.BudgetApprovedBy);
                //authorize
                headerDs.AuthorizedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.AuthorizedBy));
                headerDs.AuthorizedDate = paymentRequest.AuthorizedTime != null ? paymentRequest.AuthorizedTime : null;
                headerDs.AuthorizedSignature = SignatureHelper.GetUserSignature(paymentRequest.AuthorizedBy);
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
        public List<ApPaymentRequestLineVm> ApPaymentRequestLines(int id)
        {
            List<ApPaymentRequestLineVm> list = new List<ApPaymentRequestLineVm>();
            var requestLines = _db.PaymentRequestLines.Where(x => x.PaymentRequestId == id).ToList();
            foreach (var line in requestLines)
            {
                var entity = new ApPaymentRequestLineVm
                {
                    ItemName = "",// _db.ApSetupItems.Find(int.Parse(line.ItemName))?.Name,
                    Description = line.Description,
                    Amount = line.Amount,
                    TaxName = "",//_db.GLTaxRates.Find(line.GLTaxRateID)?.TaxName,
                    TaxAmount = line.TaxAmount,
                    TotalAmount = line.TotalAmount,
                    Remark = line.Remark
                };
                list.Add(entity);
            }
            return list;
        }

      
    }
}
