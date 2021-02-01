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
        public List<EquipmentDistributionVewModel> GetEquipmentDistribution(string id )
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

          var vehicleDistribution = db.VehicleDistribution.Where(x => x.VehicleDistributionID.ToString() == id ).FirstOrDefault();
 

            if(vehicleDistribution != null  )
            {
                 string EquipmentSpecifcation = string.Empty;
                string EquipmentStatus = string.Empty;
                int? RequestedQuantity = 0;

            
                RequestedEquipment demand = db.RequestedEquipments.Where(x => x.RequestNumber == vehicleDistribution.RequestID).FirstOrDefault();

               var _AssignedEquipmentService = new AssignedEquipmentService();

                List<AssignedEquipment> AssignedEquipments = _AssignedEquipmentService.GetAll().Where(AssignedEquipment => AssignedEquipment.RequestNumber == vehicleDistribution.RequestID && AssignedEquipment.Remark != MaintenanceConst.R).Select(a => new AssignedEquipment { AssignedEquipmentID = a.AssignedEquipmentID, RequestNumber = a.RequestNumber, EquipmentID = a.EquipmentID, AssignedDate = a.AssignedDate, RequestDesc = a.Type + " -- " + a.EquipmentName + " -- " + a.Quantity, EquipmentName = a.EquipmentName, Type = a.Type, StartDate = a.StartDate, EndDate = a.EndDate, EquipmentStatus = a.EquipmentStatus, CategoryRef = a.CategoryRef, IsAssigned = a.IsAssigned }).ToList();

                if (demand != null)
                {

                    EquipmentSpecifcation = demand.EquipmentName;
                    EquipmentStatus = demand.EquipmentStatus;
                    RequestedQuantity = demand.Quantity;

                }
                
          
                foreach (var AssignedEquipment in AssignedEquipments)
                {

                    EquipmentDistrbutionModel = new EquipmentDistributionVewModel();



                    EquipmentDistrbutionModel.AssignedQuantity = AssignedEquipments.Count;
                    EquipmentDistrbutionModel.DistrubtionNumber = vehicleDistribution.DistrubtionNumber;
                    EquipmentDistrbutionModel.EquipmentStatus = AssignedEquipment.EquipmentName;
                    EquipmentDistrbutionModel.FromBranchName = vehicleDistribution.FromBranchName;
                    EquipmentDistrbutionModel.FromStructureName = vehicleDistribution.FromStructureName;
                    EquipmentDistrbutionModel.StartingDate = AssignedEquipment.StartDate;
                    EquipmentDistrbutionModel.ToDate = AssignedEquipment.EndDate;
               //     EquipmentDistrbutionModel.Type = Enum.GetName(vehicleDistribution.Type.GetType(), vehicleDistribution.);
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

