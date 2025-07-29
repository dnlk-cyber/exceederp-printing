using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Helpers.Fleet;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using System.IO;
using System.Web;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using static ExceedERP.Core.Domain.Global.GlobalEnum;

namespace ExceedERP.Reporting.Fleet.Printing.DataSource
{
    [DataObject]
    public class SiteTransferDataSource
    {
        ExceedDbContext db = new ExceedDbContext();





        [DataObjectMethod(DataObjectMethodType.Select)]
        public SiteTransferViewModel GetSiteTransfer(string Id)
        {
            IFleetService _FleetService = new FleetService();
            ISitesVehicleTransferService _SitesVehicleTransferService = new SitesVehicleTransferService();
            List<SiteTransferViewModel> FleetList = new List<SiteTransferViewModel>();
            SiteTransferViewModel SiteTransferViewModel;

            #region SharedResource




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

            var SiteTransfer = _SitesVehicleTransferService.GetById(Guid.Parse(Id));
            var SiteTransferParts = db.SiteVechicleTransferDetails.Where(x => x.SiteVechicleTransferId.ToString() == Id).ToList();
            var SiteTransferTyres = db.SiteVechicleTransferTyreDetailes.Where(x => x.SiteVechicleTransferId.ToString() == Id).ToList();


            foreach (var SiteTransferPart in SiteTransferParts)
            {
                if (SiteTransfer != null)
                {

                    SiteTransferViewModel = new SiteTransferViewModel();
                      SiteTransferViewModel.No = SiteTransfer.Num.ToString();
                      SiteTransferViewModel.LoadedPlateNo = SiteTransfer.LoadedPlateNo.ToString();
                      SiteTransferViewModel.OnlineApprovedBy = SiteTransfer.OnlineApprovedBy.ToString();
                      SiteTransferViewModel.Duration =( SiteTransfer.ToDate.GetValueOrDefault().Day - SiteTransfer.StartingDate.GetValueOrDefault().Day).ToString();
                    SiteTransferViewModel.InitialSite = SiteTransfer.InitialSite;
                    SiteTransferViewModel.DestinationSite = SiteTransfer.DestinationSite;
                    SiteTransferViewModel.StartingDate = SiteTransfer.StartingDate;
                    SiteTransferViewModel.ToDate = SiteTransfer.ToDate;
                    SiteTransferViewModel.Description = SiteTransfer.Description;
                    SiteTransferViewModel.EquipmentType = SiteTransfer.Fleet.EquipmentType.EquipmentTypeName;

                    if (SiteTransfer.BatteryStatus == BatteryStatus.New)
                        SiteTransferViewModel.AmpereNew = true;
                    if (SiteTransfer.BatteryStatus == BatteryStatus.Old)
                        SiteTransferViewModel.AmpereOld = true;
                    if (SiteTransfer.TransferMode == Core.Domain.FleetManagement.TransferMode.Loaded)
                        SiteTransferViewModel.ByAnotherVehicle = true;
                    if (SiteTransfer.TransferMode == Core.Domain.FleetManagement.TransferMode.ByItSelf)
                        SiteTransferViewModel.ByItSelf = true;


                    SiteTransferViewModel.Capacity = SiteTransfer.Fleet.Capacity;
                    SiteTransferViewModel.ChasisNumber = SiteTransfer.Fleet.ChasisNumber;
                    SiteTransferViewModel.EngineNumber = SiteTransfer.Fleet.EngineNumber;
                    if (SiteTransfer.FuelAmount == Core.Domain.FleetManagement.FuelAmount.Full)
                        SiteTransferViewModel.FuelFull = true;
                    if (SiteTransfer.FuelAmount == Core.Domain.FleetManagement.FuelAmount.Half)
                        SiteTransferViewModel.FuelHalf = true;

                    if (SiteTransfer.FuelAmount == Core.Domain.FleetManagement.FuelAmount.Quarter)
                        SiteTransferViewModel.FuelQuarter = true;
                    SiteTransferViewModel.HydroilicFull = true;
                    if (SiteTransfer.Hydrolic == Core.Domain.FleetManagement.HydrolicAmount.Half)
                        SiteTransferViewModel.HydroilicHalf = true;
                    if (SiteTransfer.Hydrolic == Core.Domain.FleetManagement.HydrolicAmount.Quarter)
                        SiteTransferViewModel.HydroilicQuarter = true;
                    SiteTransferViewModel.Model = SiteTransfer.Fleet.Model;
                    SiteTransferViewModel.MotorNo = SiteTransfer.Fleet.EngineModel;
                    SiteTransferViewModel.Manufacturer = SiteTransfer.Fleet.Manufacturer;
                    SiteTransferViewModel.ManufacturerYear = SiteTransfer.Fleet.ManufacturerYear;
                    SiteTransferViewModel.TransferedOperatorName = SiteTransfer.Fleet.OperatorName;
            


                    SiteTransferViewModel.Odometer = SiteTransfer.DestinationSite;
                    SiteTransferViewModel.PlateNumber = SiteTransfer.Fleet.PlateNumber;
                    SiteTransferViewModel.Duration = (SiteTransfer.ToDate - SiteTransfer.StartingDate).ToString();
                    SiteTransferViewModel.TransferedOperatorName = SiteTransfer.TransferedOperator;
                    SiteTransferViewModel.UnitNum = SiteTransfer.Fleet.UnitNum;
                    if (SiteTransfer.EquipmentCondition == Core.Domain.FleetManagement.EquipmentCondition.Maintainable)
                        SiteTransferViewModel.Vehicle_Maintainable = true;
                    if (SiteTransfer.EquipmentCondition == Core.Domain.FleetManagement.EquipmentCondition.New)
                        SiteTransferViewModel.Vehicle_New = true;
                    if (SiteTransfer.EquipmentCondition == Core.Domain.FleetManagement.EquipmentCondition.Not_Maintainable)
                        SiteTransferViewModel.Vehicle_NotMaintainable = true;

                    if (SiteTransfer.EquipmentCondition == Core.Domain.FleetManagement.EquipmentCondition.Old)
                        SiteTransferViewModel.Vehicle_Old = true;

                    SiteTransferViewModel.AmharicName = AmharicName;
                    SiteTransferViewModel.Name = Name;
                    SiteTransferViewModel.Address = Address;
                    SiteTransferViewModel.LogoFileName = LogoFileName;

                    SiteTransferViewModel.SerialNo = SiteTransferPart.SerialNo;
                    SiteTransferViewModel.DamagedPart = SiteTransferPart.DamagedParts;
                    SiteTransferViewModel.MissedPart = SiteTransferPart.MissingParts;
                    SiteTransferViewModel.Type ="Part";
                    SiteTransferViewModel.Description = SiteTransferPart.Description;

                    return SiteTransferViewModel;

                }
            }
            if (!SiteTransferTyres.Any())
            {
                SiteTransferTyres.Add(new SiteVechicleTransferTyreDetail() { });
            }
            foreach (var SiteTransferPart in SiteTransferTyres)
            { 
            if (SiteTransfer != null)
            {

                SiteTransferViewModel = new SiteTransferViewModel();
                //   SiteTransferViewModel.Equipment = SiteTransfer.Fleet.UnitNum + " " + SiteTransfer.Fleet.;
                SiteTransferViewModel.No = SiteTransfer.Num.ToString();
                SiteTransferViewModel.LoadedPlateNo = SiteTransfer.LoadedPlateNo;
                SiteTransferViewModel.OnlineApprovedBy = SiteTransfer.OnlineApprovedBy.ToString();
                if (SiteTransfer.ToDate != null && SiteTransfer.StartingDate != null)
                {
                    SiteTransferViewModel.Duration = (SiteTransfer.ToDate.GetValueOrDefault().Day - SiteTransfer.StartingDate.GetValueOrDefault().Day).ToString();
                }
                SiteTransferViewModel.InitialSite = SiteTransfer.InitialSite;
                SiteTransferViewModel.DestinationSite = SiteTransfer.DestinationSite;
                SiteTransferViewModel.StartingDate = SiteTransfer.StartingDate;
                SiteTransferViewModel.ToDate = SiteTransfer.ToDate;
                SiteTransferViewModel.Description = SiteTransfer.Description;
                SiteTransferViewModel.EquipmentType = SiteTransfer.Fleet.EquipmentType.EquipmentTypeName; 
                SiteTransferViewModel.InitialSite = SiteTransfer.InitialSite;
                SiteTransferViewModel.DestinationSite = SiteTransfer.DestinationSite;
                SiteTransferViewModel.StartingDate = SiteTransfer.StartingDate;
                SiteTransferViewModel.ToDate = SiteTransfer.ToDate;
                SiteTransferViewModel.Description = SiteTransfer.Description;


                if (SiteTransfer.BatteryStatus == BatteryStatus.New)
                    SiteTransferViewModel.AmpereNew = true;
                if (SiteTransfer.BatteryStatus == BatteryStatus.Old)
                    SiteTransferViewModel.AmpereOld = true;
                if (SiteTransfer.TransferMode == Core.Domain.FleetManagement.TransferMode.Loaded)
                    SiteTransferViewModel.ByAnotherVehicle = true;
                if (SiteTransfer.TransferMode == Core.Domain.FleetManagement.TransferMode.ByItSelf)
                    SiteTransferViewModel.ByItSelf = true;


                SiteTransferViewModel.Capacity = SiteTransfer.Fleet.Capacity;
                SiteTransferViewModel.ChasisNumber = SiteTransfer.Fleet.ChasisNumber;
                SiteTransferViewModel.EngineNumber = SiteTransfer.Fleet.EngineNumber;
                 if (SiteTransfer.FuelAmount == Core.Domain.FleetManagement.FuelAmount.Full)
                    SiteTransferViewModel.FuelFull = true;
                if (SiteTransfer.FuelAmount == Core.Domain.FleetManagement.FuelAmount.Half)
                    SiteTransferViewModel.FuelHalf = true;

                if (SiteTransfer.FuelAmount == Core.Domain.FleetManagement.FuelAmount.Quarter)
                    SiteTransferViewModel.FuelQuarter = true;
                SiteTransferViewModel.HydroilicFull = true;
                if (SiteTransfer.Hydrolic == Core.Domain.FleetManagement.HydrolicAmount.Half)
                    SiteTransferViewModel.HydroilicHalf = true;
                if (SiteTransfer.Hydrolic == Core.Domain.FleetManagement.HydrolicAmount.Quarter)
                    SiteTransferViewModel.HydroilicQuarter = true;
                SiteTransferViewModel.Model = SiteTransfer.Fleet.Model;
                SiteTransferViewModel.MotorNo = SiteTransfer.Fleet.EngineModel;

                SiteTransferViewModel.EquipmentType = SiteTransfer.Fleet.EquipmentType.EquipmentTypeName;

                SiteTransferViewModel.Manufacturer = SiteTransfer.Fleet.Manufacturer;
                SiteTransferViewModel.ManufacturerYear = SiteTransfer.Fleet.ManufacturerYear;
                SiteTransferViewModel.TransferedOperatorName = SiteTransfer.Fleet.OperatorName;
                SiteTransferViewModel.SerialNo = SiteTransferPart.SerialNo;
                SiteTransferViewModel.DamagedPart = "------";
                SiteTransferViewModel.MissedPart = "------";
                SiteTransferViewModel.Type = "Tyre";
                SiteTransferViewModel.Description = SiteTransferPart.Description;



                SiteTransferViewModel.Odometer = SiteTransfer.DestinationSite;
                SiteTransferViewModel.PlateNumber = SiteTransfer.Fleet.PlateNumber;
                SiteTransferViewModel.TransferDuration = (SiteTransfer.ToDate - SiteTransfer.StartingDate).ToString();
                SiteTransferViewModel.TransferedOperatorName = SiteTransfer.TransferedOperator;
                SiteTransferViewModel.UnitNum = SiteTransfer.Fleet.UnitNum;
                if (SiteTransfer.EquipmentCondition == Core.Domain.FleetManagement.EquipmentCondition.Maintainable)
                    SiteTransferViewModel.Vehicle_Maintainable = true;
                if (SiteTransfer.EquipmentCondition == Core.Domain.FleetManagement.EquipmentCondition.New)
                    SiteTransferViewModel.Vehicle_New = true;
                if (SiteTransfer.EquipmentCondition == Core.Domain.FleetManagement.EquipmentCondition.Not_Maintainable)
                    SiteTransferViewModel.Vehicle_NotMaintainable = true;

                if (SiteTransfer.EquipmentCondition == Core.Domain.FleetManagement.EquipmentCondition.Old)
                    SiteTransferViewModel.Vehicle_Old = true;

                SiteTransferViewModel.AmharicName = AmharicName;
                SiteTransferViewModel.Name = Name;
                SiteTransferViewModel.Address = Address;
                SiteTransferViewModel.LogoFileName = LogoFileName;

                return SiteTransferViewModel;

            }
        }
            return new SiteTransferViewModel();
        }

    }
}
