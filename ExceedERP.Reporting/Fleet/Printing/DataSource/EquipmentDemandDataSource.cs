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
    public class EquipmentDemandDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EquipmentDistributionVewModel> GetEquipmentDemand(string id )
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
 

            if(vehicleDemand != null  )
            {
                 string EquipmentSpecifcation = string.Empty;
                string EquipmentStatus = string.Empty;
                int? RequestedQuantity = 0;

            
                var Equipmentdemands = db.RequestedEquipments.Where(RequestedEquipment => RequestedEquipment.RequestNumber  == vehicleDemand.VehicleDemandId && RequestedEquipment.IsAssigned == false).ToList();

 
                
          
                foreach (var AssignedEquipment in Equipmentdemands)
                {

                    EquipmentDistrbutionModel = new EquipmentDistributionVewModel();



                    EquipmentDistrbutionModel.RequestQuamtity = AssignedEquipment.Quantity;
                    EquipmentDistrbutionModel.RequestNumber = vehicleDemand.RequestNumber;
                    EquipmentDistrbutionModel.EquipmentStatus = EquipmentStatus;
                    EquipmentDistrbutionModel.RequestDesc = AssignedEquipment.RequestDesc;
                    EquipmentDistrbutionModel.FromBranchName = vehicleDemand.FromBranchName;
                    EquipmentDistrbutionModel.FromStructureName = vehicleDemand.FromStructureName;
                    EquipmentDistrbutionModel.StartingDate = AssignedEquipment.StartDate;
                    EquipmentDistrbutionModel.ToDate = AssignedEquipment.EndDate;
               //     EquipmentDistrbutionModel.Type = Enum.GetName(vehicleDemand.Type.GetType(), vehicleDemand.);
                    EquipmentDistrbutionModel.CategoryRef = AssignedEquipment.EquipmentName;
                    


                    EquipmentDistrbutionModel.AmharicName = AmharicName;
                    EquipmentDistrbutionModel.Name = Name;
                    EquipmentDistrbutionModel.LogoFileName = LogoFileName;
                    EquipmentDistrbutionModel.Address = Address;

                    EquipmentDistrbutionList.Add(EquipmentDistrbutionModel);
                }
            }



            return EquipmentDistrbutionList;
        }

    }
}

