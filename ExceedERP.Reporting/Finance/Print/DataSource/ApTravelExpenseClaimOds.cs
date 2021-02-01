
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Finance.ViewModel.Print;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Finance.DataSource.Print
{
    [DataObject]
    public class ApTravelExpenseClaimOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ApTravelExpenseClaimVm ApTravelExpenseClaimHeader(int id)
        {
            var travelExpenseClaim = _db.ApTravelExpenseClaims.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ApTravelExpenseClaimVm headerDs = new ApTravelExpenseClaimVm();

            if (travelExpenseClaim != null)
            {
                headerDs.VoucherId = travelExpenseClaim.ApTravelExpenseClaimId.ToString();
                headerDs.EmployeeName = EmployeeHelper.GetEmployeeEnglishName((int)travelExpenseClaim.EmployeeId);
                headerDs.WorkPlace = EmployeeHelper.GetEmployeePlacementBranchName((int)travelExpenseClaim.EmployeeId);
                headerDs.Salary = EmployeeHelper.GetEmployeeSalary((int)travelExpenseClaim.EmployeeId);
                headerDs.Department = EmployeeHelper.GetEmployeeWorkUnit((int)travelExpenseClaim.EmployeeId);
                headerDs.PurposeOfTravel = travelExpenseClaim.PurposeOfTravel;
                headerDs.ApTravelOrderId = travelExpenseClaim.ApTravelOrderId.ToString();
                headerDs.ClaimDate = travelExpenseClaim.ClaimDate;
                headerDs.Reference = travelExpenseClaim.Reference;
                headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(travelExpenseClaim.CreatedBy));
                var departmentApproval = _db.RequestesStatus
                    .Where(x => x.ReferenceType == "TravelExpenseClaim")
                    .Where(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL)
                    .FirstOrDefault(x => x.RequestId == travelExpenseClaim.ApTravelExpenseClaimId);
                headerDs.DepartmentApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(departmentApproval?.ChangedBy));
                headerDs.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(travelExpenseClaim.OnlineApprovedBy));
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
        public List<ApTravelExpenseExpenseLineVm> ApTravelExpenseLines(int id)
        {
            List<ApTravelExpenseExpenseLineVm> list = new List<ApTravelExpenseExpenseLineVm>();
            var requestLines = _db.ApTravelExpenseExpenses.Where(x => x.ApTravelExpenseClaimId == id).ToList();
            foreach (var line in requestLines)
            {
                var entity = new ApTravelExpenseExpenseLineVm
                {
                    Type=line.Type,
                    Description = line.Description,
                    UoM = line.UoM,
                    UnitPrice = line.UnitPrice,
                    Quantity = line.Quantity,
                    Total = line.Total
                };
                list.Add(entity);
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ApTravelPaymentLineVm> ApTravelPaymentLines(int id)
        {
            List<ApTravelPaymentLineVm> list = new List<ApTravelPaymentLineVm>();
            var travelExpensePayments = _db.ApTravelExpensePayments.Where(x => x.ApTravelExpenseClaimId == id).ToList();
            foreach (var line in travelExpensePayments)
            {
                var entity = new ApTravelPaymentLineVm
                {
                    Description = line.Description,
                    AccountCode = line.AccountCode,
                    Amount = line.Amount,
                    Reference = line.Reference,
                    PaymentMethod = line.PaymentMethod
                };
                list.Add(entity);
            }
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ApTravelExpenseRefundLineVm> ApTravelRefundLines(int id)
        {
            List<ApTravelExpenseRefundLineVm> list = new List<ApTravelExpenseRefundLineVm>();
            var travelExpensePayments = _db.ApTravelExpenseRefunds.Where(x => x.ApTravelExpenseClaimId == id).ToList();
            foreach (var line in travelExpensePayments)
            {
                var entity = new ApTravelExpenseRefundLineVm
                {
                    Description = line.Description,
                    AccountCode = line.AccountCode,
                    Amount = line.Amount,
                    Reference = line.Reference,
                    PaymentMethod = line.PaymentMethod
                };
                list.Add(entity);
            }
            return list;
        }
    }
}
