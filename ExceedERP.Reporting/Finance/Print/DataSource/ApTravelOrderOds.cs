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
    public class ApTravelOrderOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ApTravelOrderVm ApTravelOrderHeader(int id)
        {
            var apTravelOrder = _db.ApTravelOrders.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ApTravelOrderVm headerDs = new ApTravelOrderVm();

            if (apTravelOrder != null)
            {
                headerDs.VoucherId = apTravelOrder.ApTravelOrderId.ToString();
                headerDs.EmployeeName = EmployeeHelper.GetEmployeeEnglishName((int)apTravelOrder.EmployeeId);
                headerDs.WorkPlace = EmployeeHelper.GetEmployeePlacementBranchName((int)apTravelOrder.EmployeeId);
                headerDs.Salary = EmployeeHelper.GetEmployeeSalary((int)apTravelOrder.EmployeeId);
                headerDs.Department = EmployeeHelper.GetEmployeeWorkUnit((int)apTravelOrder.EmployeeId);
                headerDs.PurposeOfTravel = apTravelOrder.PurposeOfTravel;
                headerDs.RequestDate = apTravelOrder.RequestDate;
                headerDs.AdvanceAmount = apTravelOrder.AdvanceAmount;
                headerDs.Reference = apTravelOrder.Reference;
                headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(apTravelOrder.CreatedBy));
                var departmentApproval = _db.RequestesStatus
                    .Where(x => x.ReferenceType == "TravelOrder")
                    .Where(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL)
                    .FirstOrDefault(x => x.RequestId == apTravelOrder.ApTravelOrderId);
                headerDs.DepartmentApprovedBy= EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(departmentApproval?.ChangedBy));
                headerDs.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(apTravelOrder.OnlineApprovedBy));
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
        public List<ApTravelAdvanceLineVm> ApTravelAdvanceLines(int id)
        {
            List<ApTravelAdvanceLineVm> list = new List<ApTravelAdvanceLineVm>();
            var requestLines = _db.ApTravelOrderAdvancePayments.Where(x => x.ApTravelOrderId == id).ToList();
            foreach (var line in requestLines)
            {
                var entity = new ApTravelAdvanceLineVm
                {
                    Description = line.Description,
                    UoM = line.UoM,
                    UnitPrice = line.UnitPrice,
                    Quantity = line.Quantity,
                    Total = line.Total,
                    PaymentMethod = line.PaymentMethod
                };
                list.Add(entity);
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ApTravelDepartureLineVm> ApTravelDepartureLines(int id)
        {
            List<ApTravelDepartureLineVm> list = new List<ApTravelDepartureLineVm>();
            var apTravelOrderDepartures = _db.ApTravelOrderDepartures.Where(x => x.ApTravelOrderId == id).ToList();
            foreach (var line in apTravelOrderDepartures)
            {
                var entity = new ApTravelDepartureLineVm
                {
                    Type = line.Type.ToString(),
                    From = line.From,
                    To = line.To,
                    Date = line.Date,
                    Time = line.Time,
                    TransportMode = line.TransportMode,
                    Remark = line.Remark
                };
                list.Add(entity);
            }
            return list;
        }
    }
}
