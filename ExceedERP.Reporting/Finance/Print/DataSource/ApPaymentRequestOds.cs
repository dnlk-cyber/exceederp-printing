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
    public class ApPaymentRequestOds
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

                //department
                headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.CreatedBy));
                headerDs.PreparedDate = paymentRequest.DateCreated != null ? paymentRequest.DateCreated : null;

                headerDs.DepartmentSecondApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.DepartmentOnlineApprovedBy));
                headerDs.DepartmentSecondApprovedDate = paymentRequest.DepartmentOnlineApprovedTime != null ? paymentRequest.DepartmentOnlineApprovedTime : null;

                //finance
                headerDs.AccountantCheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.SendForApprovalBy));
                headerDs.AccountantCheckedDate =paymentRequest.SendForApprovalTime!=null? paymentRequest.SendForApprovalTime:null;
                
                headerDs.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.OnlineApprovedBy));
                headerDs.ApprovedDate = paymentRequest.OnlineApprovedTime;
                //budget
                headerDs.BudgetApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(paymentRequest.BudgetApprovedBy));
                headerDs.BudgetApprovedDate = paymentRequest.BudgetApprovedTime;

                #region prepared checker approver               

                var paymentReqValidations = _db.PaymentRequestDepartmentValidations
                    .Where(x => x.PaymentRequestId == paymentRequest.PaymentRequestId).ToList();

                if (paymentReqValidations.Any())
                {
                    int maxid = paymentReqValidations.Max(x => x.PaymentRequestDepartmentValidationId);
                    var approver =
                        paymentReqValidations.FirstOrDefault(x => x.PaymentRequestDepartmentValidationId == maxid);
                    if (approver != null)
                    {
                        //headerDs.DepartmentSecondApprovedBy =
                        //    EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(approver.CreatedBy));
                        //headerDs.DepartmentSecondApprovedDate = approver.DateCreated != null ? approver.DateCreated : null;

                        var checker = paymentReqValidations.OrderByDescending(x => x.PaymentRequestDepartmentValidationId)
                            .FirstOrDefault(x => x.PaymentRequestDepartmentValidationId < maxid);

                        if (checker != null)
                        {
                            headerDs.DepartmentApprovedBy =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(checker.CreatedBy));
                            headerDs.DepartmentApprovedDate = checker.DateCreated != null ? checker.DateCreated : null;
                        }
                        //else
                        //{
                        //    headerDs.DepartmentApprovedBy =
                        //        EmployeeHelper.GetEmployeeEnglishName(
                        //            UserHelper.GetUserEmployeeId(paymentRequest.DepartmentSendForApprovalBy));
                        //    headerDs.DepartmentApprovedDate = paymentRequest.DepartmentSendForApprovalTime != null ? paymentRequest.DepartmentSendForApprovalTime : null;

                        //}
                    }

                }

                #endregion

                var items = _db.PaymentRequestLines.Where(x => x.PaymentRequestId == id).ToList();
                if (items.Any())
                    headerDs.Total = items.Sum(x => x.TotalAmount);
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
                    ItemName =  _db.ApSetupItems.Find(int.Parse(line.ItemName))?.Name,
                    Description = line.Description,
                    Amount = line.Amount,
                    TaxName = _db.GLTaxRates.Find(line.GLTaxRateID)?.TaxName,
                    TaxAmount = line.TaxAmount,
                    TotalAmount = line.TotalAmount,
                    Remark = line.Remark
                };
                list.Add(entity);
            }
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FundTransferLineVm> ApFundTransferLines(int id)
        {
            List<FundTransferLineVm> list = new List<FundTransferLineVm>();
            var fundLines = _db.FundTransferLines.Where(x => x.PaymentRequestId == id).ToList();
            foreach (var line in fundLines)
            {
                var entity = new FundTransferLineVm
                {
                    BranchName = line.BranchId!=null ?_db.Branch.Find(line.BranchId)?.Name:"",
                    Material = line.Material,
                    FuelAndLubricant = line.FuelAndLubricant,
                    ManPower = line.ManPower,
                    Machinery = line.Machinery,
                    CompensationAndSeverance = line.CompensationAndSeverance,
                    FoodAndPerDiem = line.FoodAndPerDiem,
                    TotalCost = line.TotalCost,
                    TotalApproved = line.TotalApproved,
                    TotalPermited = line.TotalPermited,
                    Remark = line.Remark
                };
                list.Add(entity);
            }
            return list;
        }
    }
}
