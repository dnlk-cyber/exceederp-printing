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
    public class ApFundTransferOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ApFundTransferVm ApFundTransferHeader(int id)
        {
            var fundTransfer = _db.FundTransfers.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ApFundTransferVm headerDs = new ApFundTransferVm();

            if (fundTransfer != null)
            {
                headerDs.VoucherId = fundTransfer.FundTransferId.ToString();              
                headerDs.Employee = EmployeeHelper.GetEmployeeEnglishName((int)fundTransfer.EmployeeId);
              
                headerDs.ReasonOfTransfer = fundTransfer.ReasonOfTransfer;
                headerDs.RequestDate = fundTransfer.RequestDate;
               
                headerDs.Branch = _db.Branch.Find(fundTransfer.BranchId)?.Name;
                headerDs.WorkUnit = _db.MainCompanyStructures.Find(fundTransfer.OrgStuructureId)?.Name;

                //department
                headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(fundTransfer.CreatedBy));
                headerDs.PreparedDate = fundTransfer.DateCreated != null ? fundTransfer.DateCreated : null;
                headerDs.PreparedBySignature = SignatureHelper.GetUserSignature(fundTransfer.CreatedBy);

                headerDs.DepartmentSecondApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(fundTransfer.DepartmentOnlineApprovedBy));
                headerDs.DepartmentSecondApprovedDate = fundTransfer.DepartmentOnlineApprovedTime != null ? fundTransfer.DepartmentOnlineApprovedTime : null;
                headerDs.DepartmentSecondApproverSignature = SignatureHelper.GetUserSignature(fundTransfer.DepartmentOnlineApprovedBy);
                //finance
                headerDs.AccountantCheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(fundTransfer.SendForApprovalBy));
                headerDs.AccountantCheckedDate =fundTransfer.SendForApprovalTime!=null? fundTransfer.SendForApprovalTime:null;
                headerDs.AccountantCheckerSignature = SignatureHelper.GetUserSignature(fundTransfer.SendForApprovalBy);

                headerDs.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(fundTransfer.OnlineApprovedBy));
                headerDs.ApprovedDate = fundTransfer.OnlineApprovedTime;
                headerDs.ApproverSignature = SignatureHelper.GetUserSignature(fundTransfer.OnlineApprovedBy);
                //budget
                headerDs.BudgetApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(fundTransfer.BudgetApprovedBy));
                headerDs.BudgetApprovedDate = fundTransfer.BudgetApprovedTime;
                headerDs.BudgetSignature = SignatureHelper.GetUserSignature(fundTransfer.BudgetApprovedBy);

                #region prepared checker approver               

                var paymentReqValidations = _db.FundTransferDepartmentValidations
                    .Where(x => x.FundTransferId == fundTransfer.FundTransferId).ToList();

                if (paymentReqValidations.Any())
                {
                    int maxid = paymentReqValidations.Max(x => x.FundTransferDepartmentValidationId);
                    var approver =
                        paymentReqValidations.FirstOrDefault(x => x.FundTransferDepartmentValidationId == maxid);
                    if (approver != null)
                    {
                        //headerDs.DepartmentSecondApprovedBy =
                        //    EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(approver.CreatedBy));
                        //headerDs.DepartmentSecondApprovedDate = approver.DateCreated != null ? approver.DateCreated : null;

                        var checker = paymentReqValidations.OrderByDescending(x => x.FundTransferDepartmentValidationId)
                            .FirstOrDefault(x => x.FundTransferDepartmentValidationId < maxid);

                        if (checker != null)
                        {
                            headerDs.DepartmentApprovedBy =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(checker.CreatedBy));
                            headerDs.DepartmentApprovedDate = checker.DateCreated != null ? checker.DateCreated : null;
                            headerDs.DepartmentApproverSignature = SignatureHelper.GetUserSignature(checker.CreatedBy);
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

                var items = _db.FundTransferDetails.Where(x => x.FundTransferId == id).ToList();
                if (items.Any())
                    headerDs.Total = items.Sum(x => x.Approved);
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
        public List<FundTransferDetailVm> ApFundTransferLines(int id)
        {
            List<FundTransferDetailVm> list = new List<FundTransferDetailVm>();
            var fundLines = _db.FundTransferDetails.Where(x => x.FundTransferId == id).ToList();
            foreach (var line in fundLines)
            {
                var entity = new FundTransferDetailVm
                {

                    Description = line.Description,
                    Requested = line.Requested,
                    Approved = line.Approved,
                   Remark = line.Remark
                };
                list.Add(entity);
            }
            return list;
        }
    }
}
