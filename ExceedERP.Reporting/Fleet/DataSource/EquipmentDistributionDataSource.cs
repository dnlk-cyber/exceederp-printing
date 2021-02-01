using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.Fleet.View;
using ExceedERP.Core.Domain.Finance.GL;

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
        public List<EquipmentDistrbutionViewModel> GetDistrubtionList(string Type, string Branch, string Structure,
             string status, DateTime StartingDate, DateTime EndingDate)
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



            VehicleDistributions = db.VehicleDistribution.Where(x => x.DateCreated > StartingDate && x.DateCreated < EndingDate).ToList();


            if (!string.IsNullOrWhiteSpace(Branch))
            {
                VehicleDistributions = VehicleDistributions.Where(x => x.FromBranchName == Branch && x.DateCreated > StartingDate && x.DateCreated < EndingDate).ToList();

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
                    VehicleDistributions = VehicleDistributions.Where(x => x.Type == ExceedERP.Core.Domain.FleetManagement.FleetResource.VehicleDistribution.RequestType.Rent).ToList();
                }
                if (Type == MaintenanceConst.CompanyRequest)
                {
                    VehicleDistributions = VehicleDistributions.Where(x => x.Type == ExceedERP.Core.Domain.FleetManagement.FleetResource.VehicleDistribution.RequestType.Company).ToList();
                }
            }


            foreach (var VehicleDistribution in VehicleDistributions)
            {
                string EquipmentSpecifcation = string.Empty;
                string EquipmentStatus = string.Empty;
                int? RequestedQuantity = 0;

                List<AssignedEquipment> AssignedEquipments = db.AssignedEquipment.Where(x => x.RequestNumber == VehicleDistribution.RequestID).ToList();

                RequestedEquipment demand = db.RequestedEquipments.Where(x => x.RequestNumber == VehicleDistribution.RequestID).FirstOrDefault();


                if (demand != null)
                {

                    EquipmentSpecifcation = demand.EquipmentName;
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

            if (!EquipmentDistrbutionList.Any())
            {
                EquipmentDistrbutionModel = new EquipmentDistrbutionViewModel();
                EquipmentDistrbutionModel.AmharicName = AmharicName;
                EquipmentDistrbutionModel.Name = Name;
                EquipmentDistrbutionModel.LogoFileName = LogoFileName;
                EquipmentDistrbutionModel.Address = Address;

                EquipmentDistrbutionList.Add(EquipmentDistrbutionModel);
            }

            return EquipmentDistrbutionList;
        }

    }
}
