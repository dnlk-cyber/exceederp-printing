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
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Fleet.Printing.ViewModel;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
namespace ExceedERP.Reporting.Fleet.Printing.DataSource
{
    [DataObject]
    public class EquipmentDemandDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EquipmentDistributionVewModel> GetEquipmentDemand(string id,string RUName,int employeeId,int demandCheckerEmployeeId)
        {
            List<EquipmentDistributionVewModel> EquipmentDistrbutionList = new List<EquipmentDistributionVewModel>();
            EquipmentDistributionVewModel EquipmentDistrbutionModel;
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
            var vehicleDemand = db.VehicleDemand.Where(x => x.VehicleDemandId.ToString() == id ).FirstOrDefault();
            var EmployeeName = db.Person_Employee.Where(x => x.EmployeeId == vehicleDemand.EmployeeId).FirstOrDefault();          
            if (vehicleDemand != null  )
            {
                string EquipmentSpecifcation = string.Empty;
                string EquipmentStatus = string.Empty;
                //int? RequestedQuantity = 0;
                var VehicleDemandApproval = db.VehicleDemandApproval.Where(x => x.VehicleDemandId == vehicleDemand.VehicleDemandId).FirstOrDefault();
                var Equipmentdemands = db.RequestedEquipments.Where(RequestedEquipment => RequestedEquipment.RequestNumber  == vehicleDemand.VehicleDemandId && RequestedEquipment.IsAssigned == false).ToList();                      
                foreach (var AssignedEquipment in Equipmentdemands)
                {
                    var EquipmentNamefleet = db.Fleets.Where(x => x.EquipmentIdentityID == AssignedEquipment.EquipmentIdentityID).FirstOrDefault();
                    var EquipmentName = db.EquipmentName.Where(x => x.EquipmentNameID == EquipmentNamefleet.EquipmentNameID).FirstOrDefault();
                    var EquipmentType = db.EquipmentType.Where(x => x.EquipmentTypeID== AssignedEquipment.EquipmentTypeID).Select(x => x.EquipmentTypeName).FirstOrDefault();
                    var EquipmentCategory = db.EquipmentIdentity.Where(x => x.EquipmentIdentityID == AssignedEquipment.EquipmentIdentityID).Select(x => x.EquipmentCategoryName).FirstOrDefault();
                    EquipmentDistrbutionModel = new EquipmentDistributionVewModel();
                    EquipmentDistrbutionModel.RequestQuantity = AssignedEquipment.Quantity;
                    EquipmentDistrbutionModel.RequestNumber = vehicleDemand.RequestNumber;
                    EquipmentDistrbutionModel.EquipmentStatus = EquipmentStatus;
                    EquipmentDistrbutionModel.RequestDesc = AssignedEquipment.RequestDesc;
                    EquipmentDistrbutionModel.NumberofPersonOrTon = AssignedEquipment.NumberofPersonOrTon;
                    EquipmentDistrbutionModel.Destination = AssignedEquipment.Destination;
                    EquipmentDistrbutionModel.NumberHourPerKm = AssignedEquipment.NumberHourPerKm;
                    EquipmentDistrbutionModel.FromBranchName = vehicleDemand.FromBranchName;
                    EquipmentDistrbutionModel.FromStructureName = vehicleDemand.FromStructureName;
                    EquipmentDistrbutionModel.StartingDate = AssignedEquipment.StartDate;
                    EquipmentDistrbutionModel.ToDate = AssignedEquipment.EndDate;                     
                    EquipmentDistrbutionModel.AmharicName = AmharicName;
                    EquipmentDistrbutionModel.Name = Name;
                    EquipmentDistrbutionModel.LogoFileName = LogoFileName;
                    EquipmentDistrbutionModel.Address = Address;
                    EquipmentDistrbutionModel.ReasonForTravel = vehicleDemand.ReasonForTravel;
                    EquipmentDistrbutionModel.RequestDate = vehicleDemand.RequestDate;
                    EquipmentDistrbutionModel.CheckedBySignature = SignatureHelper.GetUserSignature(RUName);
                    EquipmentDistrbutionModel.CreatedBySignature = SignatureHelper.GetUserSignature(vehicleDemand.CreatedBy);
                    EquipmentDistrbutionModel.ApproveddBySignature = SignatureHelper.GetUserSignature(VehicleDemandApproval.CreatedBy);
                    EquipmentDistrbutionModel.IsAssigned = vehicleDemand.IsAssigned;
                    EquipmentDistrbutionModel.NumberHourPerKm = AssignedEquipment.NumberHourPerKm;
                    EquipmentDistrbutionModel.Destination = AssignedEquipment.Destination;
                    EquipmentDistrbutionModel.Remark = vehicleDemand.Remark;
                    EquipmentDistrbutionModel.EquipmentName = EquipmentName.Name;
                    EquipmentDistrbutionModel.PlateNumber = EquipmentNamefleet.PlateNumber;
                    EquipmentDistrbutionModel.PlateNumber = EquipmentNamefleet.PlateNumber;
                    EquipmentDistrbutionModel.EquipmentType = EquipmentType;
                    if (employeeId != 0)
                    {
                        var employeer = db.Person_Employee.Find(employeeId);
                        EquipmentDistrbutionModel.demandApprovalBy = employeer != null ? employeer.FirstNameEnglish + " " + employeer.MiddleNameEnglish + "" + employeer.LastNameEnglish : "";
                        
                    }
                    EquipmentDistrbutionModel.demandApprovalDate = VehicleDemandApproval.DateCreated.Value.Date;
                    if (vehicleDemand.EmployeeId != 0)
                    {
                        var checkerEmployee = db.Person_Employee.Find(vehicleDemand.EmployeeId);
                        EquipmentDistrbutionModel.RequestedBy = checkerEmployee != null ? checkerEmployee.FirstNameEnglish + " " + checkerEmployee.MiddleNameEnglish + "" + checkerEmployee.LastNameEnglish : "";

                    }
                    EquipmentDistrbutionModel.RequestDate = vehicleDemand.DateCreated.Value.Date;
              
                    EquipmentDistrbutionModel.EquipmentCategory = EquipmentCategory;
                    EquipmentDistrbutionModel.ManufacturingYear = AssignedEquipment.ManufacturedYear;
                    EquipmentDistrbutionList.Add(EquipmentDistrbutionModel);
                }
            }
            return EquipmentDistrbutionList;
        }
    }
}

