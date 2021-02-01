using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class SiteTransferViewModel : Organization
    {
        public string No { get; set; }
        public string Equipment { get; set; }
        public Nullable<DateTime> StartingDate { get; set; }

        public Nullable<DateTime> ToDate { get; set; }
        public string InitialSite { get; set; }
        public string DestinationSite { get; set; }
        public string Description { get; set; }

        public string EquipmentType { get; set; }
         public string UnitNum { get; set; }
     
        public string Manufacturer { get; set; }
        public string ManufacturerYear { get; set; }
        public string Model { get; set; }
        public string Capacity { get; set; }
        public string Color { get; set; }
        public string TyreSize { get; set; }
        public string Status { get; set; }
        public string PlateNumber { get; set; }
        public string EngineNumber { get; set; }
        public string ChasisNumber { get; set; }
        public string Odometer { get; set; }
        public string MotorNo { get; set; }
        public string EquipmentCondition { get; set; }
        public string TransferedOperatorName { get; set; }
        public string TransferDuration { get; set; }
        public bool ByItSelf { get; set; }
        public bool ByAnotherVehicle { get; set; }
        public bool AmpereNew { get; set; }
        public bool AmpereOld { get; set; }
        public bool HydroilicFull { get; set; }
        public bool HydroilicHalf { get; set; }
        public bool HydroilicQuarter { get; set; }
        public bool FuelFull { get; set; }
        public bool FuelHalf { get; set; }
        public bool FuelQuarter { get; set; }
 
        public bool Vehicle_New { get; set; }
        public bool Vehicle_Old { get; set; }
        public bool Vehicle_Maintainable { get; set; }
        public bool Vehicle_NotMaintainable { get; set; }


        public string Type { get; set; }
        public string SerialNo { get; set; }
        public string DamagedPart { get; set; }
        public string MissedPart { get; set; }
        public string  PartName { get; set; }
       public string DescriptionPart { get; set; }
       public string Duration { get; set; }
       public   string OnlineApprovedBy { get; set; }

       public   string LoadedPlateNo { get; set; }
    }
}
