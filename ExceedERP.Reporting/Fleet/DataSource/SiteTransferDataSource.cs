using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class SiteTransferDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Helpers.Fleet.FleetBindingHelper.HelperObject> GetIntial()
        {

            var result = db.SitesVehicleTransfer.Select(s => new Helpers.Fleet.FleetBindingHelper.HelperObject
            {
                Name = s.InitialSite,
                Value = s.SitesVehicleTransferID.ToString()
            }).ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Helpers.Fleet.FleetBindingHelper.HelperObject> GetDestination()
        {

            var result = db.SitesVehicleTransfer.Select(s => new Helpers.Fleet.FleetBindingHelper.HelperObject
            {
                Name = s.DestinationSite,
                Value = s.SitesVehicleTransferID.ToString()
            }).ToList();

            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SiteTransferViewModel> GetSiteTransfer(string FromSite, string ToSite, DateTime startingDate,
            DateTime endingDate)
        {
            List<SiteTransferViewModel> SiteTransferList = new List<SiteTransferViewModel>();
            SiteTransferViewModel SiteTransferModel;
            List<SitesVehicleTransfer> SiteTransfers = db.SitesVehicleTransfer.ToList();



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

            if (!string.IsNullOrEmpty(FromSite))
            {

                SiteTransfers = SiteTransfers.Where(x => x.InitialSite == FromSite).ToList();
            }

            if (!string.IsNullOrEmpty(FromSite) && !string.IsNullOrEmpty(ToSite))
            {

                SiteTransfers = SiteTransfers.Where(x => x.InitialSite == FromSite && x.DestinationSite == ToSite).ToList();
            }

            foreach (var SiteTransfer in SiteTransfers)
            {
                string Equipment =string.Empty;
                Fleets fleet =db.Fleets.Where(x=>x.FleetsID == SiteTransfer.FleetsID).FirstOrDefault();
 
                if(fleet !=null)
                {
                    Equipment = fleet.PlateNumber +" - "+fleet.EquipmentName;
                }
                SiteTransferModel = new SiteTransferViewModel();


                SiteTransferModel.Equipment = Equipment;
                SiteTransferModel.Description = SiteTransfer.Description;
                SiteTransferModel.InitialSite = SiteTransfer.InitialSite;
                SiteTransferModel.DestinationSite = SiteTransfer.DestinationSite;
                SiteTransferModel.StartingDate = SiteTransfer.StartingDate;
                SiteTransferModel.ToDate = SiteTransfer.ToDate;
                SiteTransferModel.EquipmentType = SiteTransfer.Fleet.EquipmentType.EquipmentTypeName;
                SiteTransferModel.Model = SiteTransfer.Fleet.Model;
                SiteTransferModel.PlateNumber = SiteTransfer.Fleet.PlateNumber;
                SiteTransferModel.UnitNum = SiteTransfer.Fleet.UnitNum;
                SiteTransferModel.Odometer = SiteTransfer.Fleet.CurentOdoMeter.ToString();


                SiteTransferModel.AmharicName = AmharicName;
                SiteTransferModel.Name = Name;
                SiteTransferModel.LogoFileName = LogoFileName;
                SiteTransferModel.Address = Address;

                SiteTransferList.Add(SiteTransferModel);
            }

            return SiteTransferList;
        }
    }
}
