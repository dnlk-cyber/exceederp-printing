using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
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
    public class EquipmentDistributionDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EquipmentDistributionVewModel> GetEquipmentDistribution(string Id,int employeeId, int checkerEmployeeId)
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
            var vehicleDistribution = db.VehicleDistribution.Where(x => x.VehicleDistributionID.ToString() == Id).FirstOrDefault(); // c
            var EmployeeName = db.Person_Employee.Where(x => x.EmployeeId == vehicleDistribution.EmployeeId).First();
            var EquipmentDistributionApproval = db.VehicleDistributionApproval.Where(x => x.VehicleDistributionId == vehicleDistribution.VehicleDistributionID).FirstOrDefault();
            if (vehicleDistribution != null)
            {
                string EquipmentSpecifcation = string.Empty;
                string EquipmentStatus = string.Empty;
                int? RequestedQuantity = 0;

                var requestedEquipments = db.RequestedEquipments.FirstOrDefault(x => x.RequestNumber == vehicleDistribution.RequestID);
                var assignedEquipments  = db.AssignedEquipment.Where(x => x.RequestedEquipmentId == requestedEquipments.RequestedEquipmentId);
                
                if (requestedEquipments != null)
                {
                    foreach (var assignedEqupment in assignedEquipments)
                    {
                        var additionalInfos = db.AssignedEquipmentAdditionalInfos.FirstOrDefault(x => x.AssignedEquipmentId == assignedEqupment.AssignedEquipmentID);
                        var fleet = db.Fleets.Where(x => x.FleetsID == assignedEqupment.FleetId).FirstOrDefault();
                        var OperatorId = db.AssignedEquipment.Where(x=>x.FleetId==fleet.FleetsID).Select(x=> x.OperatorId).FirstOrDefault();
                        var Odometer = db.AssignedEquipment.FirstOrDefault(x => x.RequestedEquipmentId == assignedEqupment.RequestedEquipmentId);
                        var EquipmentName = db.EquipmentName.Where(x => x.EquipmentNameID == fleet.EquipmentNameID).FirstOrDefault();

                        var EquipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == requestedEquipments.EquipmentTypeID);
                        var EquipmentCategory = db.EquipmentIdentity.Where(x => x.EquipmentIdentityID == requestedEquipments.EquipmentIdentityID).Select(x => x.EquipmentCategoryName).FirstOrDefault();

                        EquipmentDistrbutionModel = new EquipmentDistributionVewModel();
                        EquipmentDistrbutionModel.AssignedQuantity = assignedEquipments.Count();
                        EquipmentDistrbutionModel.DistrubtionNumber = vehicleDistribution.RequestNumber;
                        EquipmentDistrbutionModel.EquipmentStatus = assignedEqupment.EquipmentStatus;
                        EquipmentDistrbutionModel.FromBranchName = vehicleDistribution.FromBranchName;
                        EquipmentDistrbutionModel.FromStructureName = vehicleDistribution.FromStructureName;
                       
                        if (EmployeeName != null)
                        {
                            EquipmentDistrbutionModel.EmployeeName = EmployeeName.FirstNameEnglish + " " + EmployeeName.MiddleNameEnglish + " " + EmployeeName.LastNameEnglish;
                        }
                        EquipmentDistrbutionModel.ReasonForTravel = vehicleDistribution.ReasonForTravel;
                       // var bankid=
                       // var employeeBank = db.BankReferences
                       //.Where(x => x.BankId == bankid)
                       //.Where(x => x.Reference == payrolldata.EmployeeId.ToString())
                       //.Where(x => x.Type == BankAccountType.PAYROLL).FirstOrDefault();
                       // if (employeeBank != null)
                       // {
                       //     EmployeeInfo.BankAccount = employeeBank.AccountNumber;
                       // }
                       // else
                       // {
                       //     EmployeeInfo.BankAccount = "";
                       // }
                       // EquipmentDistrbutionModel.AccountNumber=
                        //EquipmentDistrbutionModel.IsAssigned = vehicleDistribution.IsAssigned;
                        EquipmentDistrbutionModel.StartingDate = assignedEqupment.StartDate;
                        EquipmentDistrbutionModel.ToDate = assignedEqupment.EndDate;
                        EquipmentDistrbutionModel.CategoryRef = EquipmentSpecifcation;
                        EquipmentDistrbutionModel.RequestQuantity = RequestedQuantity;
                        EquipmentDistrbutionModel.AmharicName = AmharicName;
                        EquipmentDistrbutionModel.Name = Name;
                        EquipmentDistrbutionModel.LogoFileName = LogoFileName;
                        EquipmentDistrbutionModel.Address = Address;
                        EquipmentDistrbutionModel.EquipmentName = EquipmentName.Name; //
                        EquipmentDistrbutionModel.EquipmentType = EquipmentType.EquipmentTypeName; //
                        EquipmentDistrbutionModel.PlateNumber = fleet.PlateNumber; //
                        EquipmentDistrbutionModel.Destination = requestedEquipments.Destination;
                        EquipmentDistrbutionModel.NumberHourPerKm = assignedEqupment.NumberHourPerKm;
                        EquipmentDistrbutionModel.ManufacturingYear = requestedEquipments.ManufacturedYear;
                        EquipmentDistrbutionModel.Referance = vehicleDistribution.Reference;
                        EquipmentDistrbutionModel.NumberofPersonOrTon = requestedEquipments.NumberofPersonOrTon;
                        EquipmentDistrbutionModel.Remark = vehicleDistribution.Remarks;
                        EquipmentDistrbutionModel.RequestDate = vehicleDistribution.RequestDate;
                        EquipmentDistrbutionModel.PreparedBy = vehicleDistribution.CreatedBy;
                        EquipmentDistrbutionModel.CreatedBySignature = SignatureHelper.GetUserSignature(assignedEqupment.CreatedBy);
                        EquipmentDistrbutionModel.ApproveddBySignature = SignatureHelper.GetUserSignature(assignedEqupment.OnlineApprovedBy);
                        EquipmentDistrbutionModel.CheckedBySignature = null;

                        // Additional Info
                        if (additionalInfos != null)
                        {
                            EquipmentDistrbutionModel.KmTravelledPerLitre = additionalInfos.KmTravelledPerLitre;
                            EquipmentDistrbutionModel.KmTravelledTotal = additionalInfos.KmTravelledTotal;
                            EquipmentDistrbutionModel.KmInterSite = additionalInfos.KmInterSite;
                            EquipmentDistrbutionModel.KmGrandTotal = additionalInfos.KmGrandTotal;
                            EquipmentDistrbutionModel.ReqFuelInLiter = additionalInfos.ReqFuelInLiter;
                            EquipmentDistrbutionModel.TotalInLitre = additionalInfos.TotalInLitre;
                            EquipmentDistrbutionModel.FromDeppot = additionalInfos.FromDeppot;
                            EquipmentDistrbutionModel.InCash = additionalInfos.InCash;
                            EquipmentDistrbutionModel.FromProject = additionalInfos.FromProject;
                            EquipmentDistrbutionModel.OilReqForEngine = additionalInfos.OilReqForEngine;
                            EquipmentDistrbutionModel.OilReqForOther = additionalInfos.OilReqForOther;
                            EquipmentDistrbutionModel.GreaseReq = additionalInfos.GreaseReq;
                            EquipmentDistrbutionModel.TotalBirr = additionalInfos.TotalBirr;
                            EquipmentDistrbutionModel.TotalPerdium = additionalInfos.TotalPerdium;
                        }
                        

                        if(Odometer != null)
                        {
                            EquipmentDistrbutionModel.StartOdometer = Odometer.StartOdometr;
                            EquipmentDistrbutionModel.EndOdometer = Odometer.EndOdometer;
                            EquipmentDistrbutionModel.NetOdometer = Odometer.NetOdometer;
                        }
                        
                        if (employeeId != 0)
                        {
                            var employeer = db.Person_Employee.Find(employeeId);
                            EquipmentDistrbutionModel.ApprovedBy = employeer != null ? employeer.FirstNameEnglish + " " + employeer.MiddleNameEnglish+""+employeer.LastNameEnglish : "";
                        }
                        if (checkerEmployeeId != 0)
                        {
                            var checkerEmployee = db.Person_Employee.Find(employeeId);
                            EquipmentDistrbutionModel.CheckedBy = checkerEmployee != null ? checkerEmployee.FirstNameEnglish + " " + checkerEmployee.MiddleNameEnglish + "" + checkerEmployee.LastNameEnglish : "";
                        }
                        //EquipmentDistrbutionModel.CheckedBy = vehicleDistribution.CreatedBy;
                       if (OperatorId != null){
                            EquipmentDistrbutionModel.OperatorName = db.Operator
                                .Where(x => x.OperatorID == OperatorId)
                                .Select(x => x.FullName).FirstOrDefault(); }
                        
                        EquipmentDistrbutionModel.RequestDate = vehicleDistribution.DateCreated.Value.Date;
                        EquipmentDistrbutionModel.DistributionApprovalDate = EquipmentDistributionApproval.DateCreated.Value.Date;
                        EquipmentDistrbutionList.Add(EquipmentDistrbutionModel);
                    }
                }
            }
            return EquipmentDistrbutionList;
        }

    }
}

