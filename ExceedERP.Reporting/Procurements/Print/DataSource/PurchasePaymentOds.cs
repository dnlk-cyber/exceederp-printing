using ExceedERP.Core.Domain.Common;

using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.Procurements.Print.VM;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Procurements.Print.Print
{
    [DataObject]
    public class PurchasePaymentOds
    {
        ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public PurchasePaymentVM GetPurchasePayment(int id)
        {
            var purchasePayment = _db.PurchasePayments.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            PurchasePaymentVM headerDs = new PurchasePaymentVM();
            if (purchasePayment != null)
            {
                int branchid = purchasePayment.ForBranchId != null ? (int)purchasePayment.ForBranchId : purchasePayment.BranchId;
                headerDs.PurchasePaymentId = purchasePayment.PurchasePaymentId;                
                headerDs.Date = purchasePayment.Date;
                headerDs.SupplierName = purchasePayment.SupplierName;
                purchasePayment.SupplierId = purchasePayment.SupplierId;
                headerDs.SupplierInvoice = purchasePayment.SupplierInvoice;
                headerDs.PaymentAmount = purchasePayment.PaymentAmount;
                headerDs.ApplyPenality = purchasePayment.ApplyPenality;
                headerDs.PenalityAmount = purchasePayment.PenalityAmount;
                headerDs.NetPayable = purchasePayment.NetPayable;
                headerDs.AmountDue = purchasePayment.AmountDue;
                headerDs.Paid = purchasePayment.Paid? "Yes": "No";
                headerDs.WithOutGRN = purchasePayment.WithOutGRN;
                headerDs.PurchasePaymentType = purchasePayment.PurchasePaymentType.ToString();
                headerDs.Type = purchasePayment.Type;
                headerDs.PoOrContractNumber = purchasePayment.PoOrContractNumber;
                if (purchasePayment.ProcurementOfficer != null)
                    headerDs.ProcurementOfficer = EmployeeHelper.GetEmployeeEnglishName((int)purchasePayment.ProcurementOfficer);
                if (purchasePayment.ProcurementAnalyzer != null)
                    headerDs.ProcurementAnalyzer = EmployeeHelper.GetEmployeeEnglishName((int)purchasePayment.ProcurementAnalyzer);
                headerDs.PurposeOfPayment = purchasePayment.PurposeOfPayment;              
                headerDs.PaymentType = purchasePayment.PaymentType.ToString();
                headerDs.Branch = _db.Branch.Find(branchid)?.Name;

                #region prepared checker approver, budget approver

                headerDs.CreatedBy =
                     EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(purchasePayment.CreatedBy));
                if (purchasePayment.DateCreated != null)
                    headerDs.CreateDate = purchasePayment.DateCreated != null ? purchasePayment.DateCreated.ToString() : "";
                headerDs.CreatedBySignature = SignatureHelper.GetUserSignature(purchasePayment.CreatedBy);

                var purchasePaymentValidations = _db.PurchasePaymentValidations
                   .Where(x => x.PurchasePaymentId == purchasePayment.PurchasePaymentId).ToList();

                if (purchasePaymentValidations.Any())
                {
                    int maxid = purchasePaymentValidations.Max(x => x.PurchasePaymentValidationId);
                    var approver =
                        purchasePaymentValidations.FirstOrDefault(x => x.PurchasePaymentValidationId == maxid);
                    if (approver != null)
                    {
                        headerDs.Approver =
                            EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(approver.CreatedBy));
                        headerDs.ApprovedDate = approver.DateCreated != null ? approver.DateCreated.ToString() : "";
                        headerDs.ApproverSignature = SignatureHelper.GetUserSignature(approver.CreatedBy);

                        var checker = purchasePaymentValidations.OrderByDescending(x => x.PurchasePaymentValidationId)
                            .FirstOrDefault(x => x.PurchasePaymentValidationId < maxid);

                        if (checker != null)
                        {
                            headerDs.Checker =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(checker.CreatedBy));
                            headerDs.CheckedDate = checker.DateCreated != null ? checker.DateCreated.ToString() : "";
                            headerDs.CheckerSignature = SignatureHelper.GetUserSignature(checker.CreatedBy);
                        }
                        else
                        {
                            headerDs.Checker =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(purchasePayment.SendForApprovalBy));
                            headerDs.CheckedDate = purchasePayment.SendForApprovalTime != null ? purchasePayment.SendForApprovalTime.ToString() : "";
                            headerDs.CheckerSignature = SignatureHelper.GetUserSignature(purchasePayment.SendForApprovalBy);
                        }
                    }

                }

                headerDs.BudgetApprover =
                     EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(purchasePayment.BudgetApprovedBy));
                if (purchasePayment.BudgetApprovedTime != null)
                    headerDs.BudgetApprovedDate = purchasePayment.BudgetApprovedTime != null ? purchasePayment.BudgetApprovedTime: null;

                headerDs.BudgetSignature = SignatureHelper.GetUserSignature(purchasePayment.BudgetApprovedBy);
                //finance
                headerDs.AccountantCheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(purchasePayment.FinanceSendForApprovalBy));
                headerDs.AccountantCheckedDate = purchasePayment.FinanceSendForApprovalTime != null ? purchasePayment.FinanceSendForApprovalTime : null;
                headerDs.AccountantCheckerSignature = SignatureHelper.GetUserSignature(purchasePayment.FinanceSendForApprovalBy);

                headerDs.FinanceApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(purchasePayment.FinanceApprovedBy));
                headerDs.FinanceApprovedDate = purchasePayment.FinanceApprovedTime != null ? purchasePayment.FinanceApprovedTime:null;
                headerDs.FinanceApproverSignature = SignatureHelper.GetUserSignature(purchasePayment.FinanceApprovedBy);
                #endregion

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
      
    }
}
