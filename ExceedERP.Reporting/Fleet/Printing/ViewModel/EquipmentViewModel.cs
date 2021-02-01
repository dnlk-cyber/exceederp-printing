using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.Printing.ViewModel
{
    public class EquipmentViewModel : Organization
    {
         public string EquipmentName { get; set; }

        public string Type { get; set; }
         public string UnitNum { get; set; }

        public string Manufacturer { get; set; }

         public string ManufacturerYear { get; set; }

         public string Condition { get; set; }

        public string EngineModel { get; set; }
         public string OperatorName { get; set; }
   public string EngineManufacturer { get; set; }
         public string Location { get; set; }
         public string HistoricalCost { get; set; }

        public string Model { get; set; }

        public string Capacity { get; set; }
        public bool IsExternal { get; set; }
        public Guid RentId { get; set; }

        public string Color { get; set; }

         public string TyreSize { get; set; }
        public string Status { get; set; }

         public string PlateNumber { get; set; }
       
        public string EngineNumber { get; set; }
        public string ChasisNumber { get; set; }
         public decimal MinFuelStandard { get; set; }
         public decimal MaxFuelStandard { get; set; }
         public string Battery { get; set; }
         public string Tyre { get; set; }
         public decimal RentalRate { get; set; }

        public decimal IdealRentalRate { get; set; }
        public enum WayOfSupplyenum { Purchased, Donated, Transferred }
        public enum manufaturerenum { TOYOTA, NISSAN, SINO }
        public enum Colorenum { BLACK, WHITE, RED, BLUE, GREEN }
        public enum yearenum { }
      
        public string WayOfSupply { get; set; }
       public string SupplierCompany { get; set; }
         public decimal PreviousOdoMeter { get; set; }

         public decimal CurentOdoMeter { get; set; }
        public decimal LastMaintenanceOdometer { get; set; }
         public decimal PurchasingCost { get; set; }
         public Nullable<int> AnnualDepreciationCost { get; set; }
        public Nullable<int> DepreciationCost { get; set; }
          public decimal BookValue { get; set; }
    public string SerialNo { get; set; }
        public string Libre { get; set; }
        public enum TypeOfFuelenum { Benzene, Diesel }
        
        public string TypeOfFuel { get; set; }
         public string StateOfOdoMeter { get; set; }
        public enum StateOfOdoMeterenum { Functional, Malfucntioned }
        public string Attachment { get; set; }
         public string FleetPhoto { get; set; }
        public Nullable<System.DateTime> PurchasedDate { get; set; }
         public Nullable<System.DateTime> LastMaintained { get; set; }

    public string InsuranceCost { get; set; }
         public string WorkingCost { get; set; }

         public string IdealCost { get; set; }
         public string SpareRepairCost { get; set; }
         public string BatteryRepairCost { get; set; }
          public string OtherRepairCost { get; set; }
        public string TyreRepairCost { get; set; }
         public string OilRepairCost { get; set; }
         
        public string FuelConsumptionLevel { get; set; }
      public decimal FuelConsumptionRate { get; set; }
        public string RentTypeName { get; set; }
         public string Category { get; set; }
         public string SubCategory { get; set; }
    }






   
}
