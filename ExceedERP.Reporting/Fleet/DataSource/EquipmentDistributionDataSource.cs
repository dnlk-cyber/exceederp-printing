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
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.Fleet.View;
using ExceedERP.Core.Domain.Finance.GL;
using static ExceedERP.Core.Domain.FleetManagement.FleetResource.VehicleDistribution;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class EquipmentDistributionDataSource
    {

        private ExceedDbContext db = new ExceedDbContext();





        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetRequestStatus()
        {

            List<Helpers.Fleet.FleetBindingHelper.HelperObject> result = new List<Helpers.Fleet.FleetBindingHelper.HelperObject>();

            result.Add(new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = MaintenanceConst.Pending });
            result.Add(new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = MaintenanceConst.Finished });

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetRequestType()
        {

            List<Helpers.Fleet.FleetBindingHelper.HelperObject> result = new List<Helpers.Fleet.FleetBindingHelper.HelperObject>();

            result.Add(new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = MaintenanceConst.CompanyRequest });

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EquipmentDistrbutionViewModel> GetDistrubtionList(string Type, string Branch, string Structure, string status, DateTime? StartDate, DateTime? EndDate)
        {
            List<EquipmentDistrbutionViewModel> EquipmentDistrbutionList = new List<EquipmentDistrbutionViewModel>();
            EquipmentDistrbutionViewModel EquipmentDistrbutionModel;
            List<VehicleDistribution> VehicleDistributions = new List<VehicleDistribution>();
            #region for ShardedDataSource
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
                            a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            #endregion
            
            VehicleDistributions =  db.VehicleDistribution.ToList();
            
            if (StartDate.HasValue)
            {
                VehicleDistributions = VehicleDistributions.Where(x => x.DateCreated >= StartDate).ToList();
            }
            if (EndDate.HasValue)
            {
                VehicleDistributions = VehicleDistributions.Where(x => x.DateCreated <= EndDate).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Branch))
            {
                VehicleDistributions = VehicleDistributions.Where(x => x.FromBranchName == Branch).ToList();

            }

            if (!string.IsNullOrEmpty(Structure))
            {

                VehicleDistributions = VehicleDistributions.Where(x => x.FromStructureName == Structure).ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {

                VehicleDistributions = VehicleDistributions.Where(x => x.EquipmentStatus == status).ToList();
            }

            if (!string.IsNullOrEmpty(Type))
            {
                if (Type == MaintenanceConst.RentRequest)
                {
                    VehicleDistributions = VehicleDistributions.Where(x => x.Type == RequestType.Rent).ToList();
                }
                if (Type == MaintenanceConst.CompanyRequest)
                {
                    VehicleDistributions = VehicleDistributions.Where(x => x.Type == RequestType.Company).ToList();
                }
            }


            foreach (var VehicleDistribution in VehicleDistributions)
            {
                string EquipmentSpecifcation = string.Empty;
                string EquipmentStatus = string.Empty;
                int? RequestedQuantity = 0;
                List<AssignedEquipment> AssignedEquipments = new List<AssignedEquipment>();


                var requestedEquipment = db.RequestedEquipments.FirstOrDefault(x => x.RequestNumber == VehicleDistribution.RequestID);
                if(requestedEquipment != null)
                {
                    AssignedEquipments = db.AssignedEquipment.Where(x => x.VehicleDemandId == requestedEquipment.VehicleDemandId).ToList();
                    EquipmentStatus = requestedEquipment.EquipmentStatus;
                    RequestedQuantity = requestedEquipment.Quantity;
                }
               
                
                foreach (var AssignedEquipment in AssignedEquipments)
                {

                    EquipmentDistrbutionModel = new EquipmentDistrbutionViewModel
                    {
                        AssignedQuantity = AssignedEquipments.Count,
                        DistrubtionNumber = VehicleDistribution.DistrubtionNumber,
                        EquipmentStatus = EquipmentStatus,
                        FromBranchName = VehicleDistribution.FromBranchName,
                        FromStructureName = VehicleDistribution.FromStructureName,
                        StartingDate = AssignedEquipment.StartDate,
                        ToDate = AssignedEquipment.EndDate,
                        Type = Enum.GetName(VehicleDistribution.Type.GetType(), VehicleDistribution.Type),
                        //CategoryRef = AssignedEquipment.EquipmentName,
                        CategoryRef = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == AssignedEquipment.EquipmentNameID)?.Name ?? null,
                        //CategoryRef = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == requestedEquipment.EquipmentTypeID),
                        RequestQuamtity = RequestedQuantity,
                        AmharicName = AmharicName,
                        Name = Name,
                        LogoFileName = LogoFileName,
                        Address = Address
                    };
                    
                    EquipmentDistrbutionList.Add(EquipmentDistrbutionModel);
                }
            }

            if (!EquipmentDistrbutionList.Any())
            {
                EquipmentDistrbutionModel = new EquipmentDistrbutionViewModel
                {
                    AmharicName = AmharicName,
                    Name = Name,
                    LogoFileName = LogoFileName,
                    Address = Address
                };

                EquipmentDistrbutionList.Add(EquipmentDistrbutionModel);
            }

            return EquipmentDistrbutionList;
        }

    }
}
