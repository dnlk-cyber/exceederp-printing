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

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class EquipmentReturnDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EquipmentDistrbutionViewModel> GetEquipmentReturns(string Branch, string Structure, DateTime startingDate,
DateTime endingDate)
        {
            List<EquipmentDistrbutionViewModel> EquipmentDistrbutionList = new List<EquipmentDistrbutionViewModel>();
            EquipmentDistrbutionViewModel EquipmentDistrbutionModel;



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

            List<VehicleDistribution> VehicleDistributions = db.VehicleDistribution.Where(x => x.FromBranchName == Branch && x.DateCreated > startingDate && x.DateCreated < endingDate).ToList();

            if (!string.IsNullOrEmpty(Structure))
            {

                VehicleDistributions = VehicleDistributions.Where(x => x.FromStructureName == Structure).ToList();
            }



            foreach (var VehicleDistribution in VehicleDistributions)
            {
                 string EquipmentSpecifcation = string.Empty;
                string EquipmentStatus = string.Empty;
                int? RequestedQuantity = 0;

                List<AssignedEquipment> AssignedEquipments = db.AssignedEquipment.Where(x => x.RequestNumber == VehicleDistribution.RequestID && x.IsAssigned == false).ToList();

                RequestedEquipment demand = db.RequestedEquipments.Where(x => x.RequestNumber == VehicleDistribution.RequestID).FirstOrDefault();


                if (demand != null)
                {

                    //EquipmentSpecifcation = demand.EquipmentName;
                    EquipmentStatus = demand.EquipmentStatus;
                    RequestedQuantity = demand.Quantity;

                }
                
          
                foreach (var AssignedEquipment in AssignedEquipments)
                {

                    EquipmentDistrbutionModel = new EquipmentDistrbutionViewModel();



                    EquipmentDistrbutionModel.AssignedQuantity = AssignedEquipments.Count;
                    EquipmentDistrbutionModel.DistrubtionNumber = VehicleDistribution.DistrubtionNumber;
                    EquipmentDistrbutionModel.EquipmentStatus = EquipmentStatus;
                    EquipmentDistrbutionModel.FromBranchName = VehicleDistribution.FromBranchName;
                    EquipmentDistrbutionModel.FromStructureName = VehicleDistribution.FromStructureName;
                    EquipmentDistrbutionModel.StartingDate = AssignedEquipment.StartDate;
                    EquipmentDistrbutionModel.ToDate = AssignedEquipment.EndDate;
                    EquipmentDistrbutionModel.Type = Enum.GetName(VehicleDistribution.Type.GetType(), VehicleDistribution.Type);
                    EquipmentDistrbutionModel.CategoryRef = EquipmentSpecifcation;
                    EquipmentDistrbutionModel.RequestQuamtity = RequestedQuantity;
                  


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

