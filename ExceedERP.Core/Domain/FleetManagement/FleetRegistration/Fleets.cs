using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class Fleets : Operations
    {
      
        public Fleets()
        {
            this.Accessories = new HashSet<Accessories>();
            this.AnnualEquipmentInventoryInspections = new HashSet<AnnualEquipmentInventoryInspection>();
            this.DisposalandReplacementRecommendations = new HashSet<DisposalandReplacementRecommendation>();
            this.EquipmentPlanandEvaluations = new HashSet<EquipmentPlanandEvaluations>();
            this.TireReturns = new HashSet<TireReturn>();
            this.TireIssues = new HashSet<TireIssue>();
            this.Fuels = new HashSet<Fuel>();
            this.Insurances = new HashSet<FleetInsurance>();
            this.Operators = new HashSet<Operator>();
    }
        [ScaffoldColumn(false)]
        public Guid FleetsID { get; set; }
   
        [DisplayName("  Equipment Category ")]
        [Required]
        public Guid EquipmentIdentityID { get; set; }
        [DisplayName("  Equipment Type ")]
        [Required]
        public Guid EquipmentTypeID { get; set; }
        [DisplayName("  Maintenance Plan ")]
        public Guid PlannerID { get; set; }

        [DisplayName(" Equipment Name ")]
        public string EquipmentName { get; set; }
        [NotMapped]
        [DisplayName(" Sub Type ")]
        public string SubType { get; set; }
        public string Type { get; set; }
        [Required]
        [DisplayName("Asset Code No")]
        public string UnitNum { get; set; }

        public string Manufacturer { get; set; }

        [DisplayName(" Manufacturer Year")]
        public string ManufacturerYear { get; set; }

        [DisplayName(" Condition ")]
        public string Condition { get; set; }

        [DisplayName(" Reading Type ")]
        public ReadingType ReadingType { get; set; }
        [DisplayName(" Engine Model ")]
        public string EngineModel { get; set; }
        [DisplayName(" Operator Name ")]
        public string OperatorName { get; set; }

        [DisplayName(" Engine Manufacturer ")]
        public string EngineManufacturer { get; set; }
        [DisplayName(" Location ")]
        public string Location { get; set; }
        [DisplayName(" Historical Cost ")]
        public string HistoricalCost { get; set; }

        public string Model { get; set; }
        public bool IsReturned { get; set; }
        public string Capacity { get; set; }
        public bool IsExternal { get; set; }
        [DisplayName("Rent Type")]
        public RentType RentType { get; set; }
        public Guid RentId { get; set; }

        public string Color { get; set; }

        [DisplayName("  Tire Size")]
        public string TyreSize { get; set; }
        public string Status { get; set; }

        [DisplayName("Plate Number")]
        public string PlateNumber { get; set; }
        [DisplayName(" Engine Serial No")]

        public string EngineNumber { get; set; }
        [DisplayName(" IFRS Condition ")]
        public string IFRSCondition { get; set; }
        [DisplayName(" Chassis Number ")]

        public string ChasisNumber { get; set; }
        [DisplayName("Min Standard Consumption ")]
        public decimal MinFuelStandard { get; set; }
        [DisplayName("Max Standard Consumption ")]
        public decimal MaxFuelStandard { get; set; }
        [DisplayName(" Battery ")]
        public string Battery { get; set; }
        [DisplayName(" Tyre ")]
        public string Tyre { get; set; }
        [DisplayName(" Rental Rate  ")]
        public decimal RentalRate { get; set; }

        [DisplayName("Ideal Rental Rate  ")]
        public decimal IdealRentalRate { get; set; }
        public enum WayOfSupplyenum { Purchased, Donated, Transferred }
        public enum manufaturerenum { TOYOTA, NISSAN, SINO }
        public enum Colorenum { BLACK, WHITE, RED, BLUE, GREEN }
        public enum yearenum { }
        [DisplayName(" Way Of Supply  ")]

        public string WayOfSupply { get; set; }
        
[DisplayName(" Supplier Company  ")]
        public string SupplierCompany { get; set; }
        [DisplayName(" Previous OdoMeter   ")]
        public decimal PreviousOdoMeter { get; set; }

        [DisplayName(" Current Hour/OdoMeter   ")]
        public decimal CurentOdoMeter { get; set; }
        public decimal LastMaintenanceOdometer { get; set; }
        [DisplayName("Purchasing Cost   ")]
        public decimal PurchasingCost { get; set; }
        [DisplayName(" Depreciation Cost    ")]
        public Nullable<int> AnnualDepreciationCost { get; set; }
        [DisplayName(" Depreciation Cost")]
        public Nullable<int> DepreciationCost { get; set; }
        [DisplayName(" Book Value  ")]
        public decimal BookValue { get; set; }

        [DisplayName(" Serial Number   ")]
        public string SerialNo { get; set; }
          [DisplayName(" Libre No  ")]
        public string Libre { get; set; }
        public enum TypeOfFuelenum { Benzene, Diesel }
        [DisplayName(" Type Of Fuel    ")]

        public string TypeOfFuel { get; set; }
        [DisplayName(" State Of OdoMeter    ")]
        public string StateOfOdoMeter { get; set; }
        public enum StateOfOdoMeterenum { Functional, Malfucntioned }
        public string Attachment { get; set; }
        [DisplayName(" ")]
        public string FleetPhoto { get; set; }
        [DataType(DataType.Date)]
        [DisplayName(" Acquired Date ")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> PurchasedDate { get; set; }
        [DisplayName(" Last Maintained ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> LastMaintained { get; set; }

        [NotMapped]
        [DisplayName("Petty Cash")]
        public string PettyCash { get; set; }
        [NotMapped]
        [DisplayName("  Insurance Cost ")]
        public string InsuranceCost { get; set; }
        [NotMapped]
        [DisplayName("  Working Cost ")]
        public string WorkingCost { get; set; }

        [NotMapped]
        [DisplayName("  Ideal Cost ")]
        public string IdealCost { get; set; }
        [NotMapped]
        [DisplayName(" Spare Repair Cost ")]
        public string SpareRepairCost { get; set; }
        [NotMapped]
        [DisplayName(" Battery Repair Cost ")]
        public string BatteryRepairCost { get; set; }
        [NotMapped]
        [DisplayName(" Other Repair Cost ")]
        public string OtherRepairCost { get; set; }
        [NotMapped]
        [DisplayName(" Tyre Repair Cost ")]
        public string TyreRepairCost { get; set; }
        [NotMapped]
        [DisplayName(" Oil Repair Cost ")]
        public string OilRepairCost { get; set; }
        [NotMapped]
        [DisplayName(" Fuel Consumption ")]
        public string FuelConsumptionLevel { get; set; }

        [NotMapped]
        [DisplayName(" Fuel Consumption Rate ")]
        public decimal FuelConsumptionRate { get; set; }
        [NotMapped]
        [DisplayName(" Rent Type")]
        public string RentTypeName { get; set; }
        [NotMapped]
        public string Category { get; set; }
     
        [NotMapped]
        [DisplayName("Type")]
        public string SubCategory { get; set; }
        [NotMapped]
        [DisplayName("Status")]
        public string ApprovalStatus { get; set; }
        public virtual EquipmentIdentity EquipmentIdentity { get; set; }
        public virtual EquipmentType EquipmentType { get; set; }
        public virtual ICollection<FleetInsurance> Insurances { get; set; }

        public virtual ICollection<Operator> Operators { get; set; }
        public virtual ICollection<Accessories> Accessories { get; set; }
        public virtual ICollection<AnnualEquipmentInventoryInspection> AnnualEquipmentInventoryInspections { get; set; }
        public virtual ICollection<DisposalandReplacementRecommendation> DisposalandReplacementRecommendations { get; set; }
        public virtual ICollection<EquipmentPlanandEvaluations> EquipmentPlanandEvaluations { get; set; }
        public virtual ICollection<TireIssue> TireIssues { get; set; }
        public virtual ICollection<TireReturn> TireReturns { get; set; }
        public virtual ICollection<Fuel> Fuels { get; set; }

    }
}