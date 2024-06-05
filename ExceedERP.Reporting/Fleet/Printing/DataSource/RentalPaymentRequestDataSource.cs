using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.Printing.ViewModel;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
namespace ExceedERP.Reporting.Fleet.Printing.DataSource
{
    [DataObject]
    public class RentalPaymentRequestDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RentalPaymentRequestViewModel> SendRentalPaymentRequest(int id, string rentalPaymentRequesterId, string rentalPaymentApproverId, string preparedById, string departmentApprovalById)
        {
            List<RentalPaymentRequestViewModel> RentalPaymentRequestList = new List<RentalPaymentRequestViewModel>();
            RentalPaymentRequestViewModel RentalPaymentRequestModel;
            string AmharicName = "";
            string Name = "";
            string Address = "";
            Image LogoFileName = null;
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                var address =
                   db.Addresses.Where(
                     a =>
                       a.Reference == organization.MainCompanyId.ToString() &&
                       a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {
                        if (Address == null)
                        {
                            Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            Address += ", " + a.Category + "-" + a.Value;
                        }
                    }
                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                       a.ReferenceType ==
                       ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            var rentalPaymentRequest = db.RentalPaymentRequests.Where(x => x.RentalPaymentRequestId == id ).FirstOrDefault();
            var departmentName = db.MainCompanyStructures.Where(x => x.OrgStructureId == rentalPaymentRequest.OrgStructureId).FirstOrDefault();
            var branchName = db.Branch.Where(x => x.BranchId == rentalPaymentRequest.BranchId).FirstOrDefault();
            var employeeFullName = db.Person_Employee.Where(x => x.EmployeeId == rentalPaymentRequest.EmployeeId).FirstOrDefault();
            var supplierName = db.ProcurementSuppliers.Where(x => x.ProcurementSupplierID == rentalPaymentRequest.ProcurementSupplierID).FirstOrDefault();
            if (rentalPaymentRequest != null  )
            {
                var rentalPaymentRequestLines = db.RentalPaymentRequestLines.Where(x => x.RentalPaymentRequestId  == rentalPaymentRequest.RentalPaymentRequestId).ToList();
                foreach (var rentalPaymentRequestLine in rentalPaymentRequestLines)
                {
                    var equipmentName = db.EquipmentName.Where(x => x.EquipmentNameID == rentalPaymentRequestLine.EquipmentNameId).FirstOrDefault();
                    var equipmentType = db.EquipmentType.Where(x => x.EquipmentTypeID == rentalPaymentRequestLine.EquipmnetTypeId).FirstOrDefault();
                    var period = db.GLPeriods.Where(x => x.GLPeriodId == rentalPaymentRequest.GLPeriodId).FirstOrDefault();
                    RentalPaymentRequestModel = new RentalPaymentRequestViewModel();
                    RentalPaymentRequestModel.Id = rentalPaymentRequest.RentalPaymentRequestId;
                    if (employeeFullName != null)
                    {
                        RentalPaymentRequestModel.EmployeeName = employeeFullName.FirstNameEnglish + " " + employeeFullName.MiddleNameEnglish + " " + employeeFullName.LastNameEnglish;
                    }   
                    if (departmentName != null)
                    {
                        RentalPaymentRequestModel.Departnment = departmentName.Name;
                    }
                    if (branchName != null)
                    {
                        RentalPaymentRequestModel.Branch = branchName.Name;
                    }
                    if (supplierName != null)
                    {
                        RentalPaymentRequestModel.SupplierName = supplierName.Name;
                    }
                    RentalPaymentRequestModel.ReasonForPayment = rentalPaymentRequest.ReasonofPayment;
                    RentalPaymentRequestModel.RequestDate = rentalPaymentRequest.RequestDate;
                    RentalPaymentRequestModel.Refference = rentalPaymentRequest.Refference;
                    if (equipmentType != null)
                    {
                        RentalPaymentRequestModel.EquipmentType = equipmentType.EquipmentTypeName;
                    }
                    if (equipmentName != null)
                    {
                        RentalPaymentRequestModel.EquipmentName = equipmentName.Name;
                    }
                    if (period != null)
                    {
                        RentalPaymentRequestModel.Period = period.Name ;
                    }
                    RentalPaymentRequestModel.Amount = rentalPaymentRequestLine.Amount;
                    RentalPaymentRequestModel.Tax = rentalPaymentRequestLine.TaxAmount;
                    RentalPaymentRequestModel.SubTotalAmount = rentalPaymentRequestLine.SubTotalAmount;
                    RentalPaymentRequestModel.FuelAmount = rentalPaymentRequestLine.FuelAmount;
                    RentalPaymentRequestModel.Commusion = rentalPaymentRequestLine.Commission;
                    RentalPaymentRequestModel.AdvancedAmount = rentalPaymentRequestLine.AdvancedPayment;
                    RentalPaymentRequestModel.TotalAmount = rentalPaymentRequestLine.TotalAmount;
                    RentalPaymentRequestModel.DurationFrom = rentalPaymentRequest.DurationFrom;
                    RentalPaymentRequestModel.DurationTo = rentalPaymentRequest.DurationTo;
                    
                    if (!string.IsNullOrWhiteSpace(preparedById))
                    {
                        int.TryParse(preparedById, out int pId);
                        var preparedPerson = db.Person_Employee.Find(pId);
                        if (preparedPerson != null)
                            RentalPaymentRequestModel.PreparedBy = preparedPerson.FirstNameEnglish + " " + preparedPerson.MiddleNameEnglish + " " + preparedPerson.LastNameEnglish; ;
                    }
                    if (!string.IsNullOrWhiteSpace(rentalPaymentRequesterId))
                    {
                        int.TryParse(rentalPaymentRequesterId, out int rpId);
                        var budgetApprovedBy = db.Person_Employee.Find(rpId);
                        if(budgetApprovedBy != null)
                            RentalPaymentRequestModel.BudgetApprovalBy = budgetApprovedBy.FirstNameEnglish + " " + budgetApprovedBy.MiddleNameEnglish + " " + budgetApprovedBy.LastNameEnglish; ;
                    }
                    if (!string.IsNullOrWhiteSpace(departmentApprovalById))
                    {
                        int.TryParse(departmentApprovalById, out int dAId);
                        var departmentApprovedBy = db.Person_Employee.Find(dAId);
                        if(departmentApprovedBy != null)
                            RentalPaymentRequestModel.DepartmentApprovedBy = departmentApprovedBy.FirstNameEnglish + " " + departmentApprovedBy.MiddleNameEnglish + " " + departmentApprovedBy.LastNameEnglish; ;
                    }
                    if (!string.IsNullOrWhiteSpace(rentalPaymentApproverId))
                    {
                        int.TryParse(rentalPaymentApproverId, out int rPAId);
                        var onlinePaymentRequestApprovedBy = db.Person_Employee.Find(rPAId);
                        if(onlinePaymentRequestApprovedBy != null)
                            RentalPaymentRequestModel.PreparedBy = onlinePaymentRequestApprovedBy.FirstNameEnglish + " " + onlinePaymentRequestApprovedBy.MiddleNameEnglish + " " + onlinePaymentRequestApprovedBy.LastNameEnglish; ;
                    }
                    RentalPaymentRequestModel.ApprovedDate = rentalPaymentRequest.DateCreated.Value.Date;
                    RentalPaymentRequestList.Add(RentalPaymentRequestModel);
                }
            }
            return RentalPaymentRequestList;
        }
    }
}

