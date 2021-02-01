using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using System.Web.Util;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.Inventory;
using System;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.Construction
{
    public class ConstructionItem

    {
        [Key]
        [DisplayName("Item Number")]
        public int ItemNumber { get; set; }
        [DisplayName("Description")]
        public string ItemDescription { get; set; }

    }

    public class LabourRateIndexFactor
    {
        public int IndexFactorId { get; set; }
        public string Description { get; set; }
        public decimal NumberOfDays { get; set; }
        public decimal Index { get; set; }
    }
    public class BillOfQuantity
    {
        [Key]
        public int BillOfQuantityId { get; set; }

        [DisplayName("Type Of BOQ"), Required]
        public TypeOfConstruction BoqType { get; set; }
        [DisplayName("Bill Number")]

        public int BillNumber { get; set; }
        [DisplayName("Bill Description")]
        [StringLength(200)]
        public string BillDescription { get; set; }
        [DisplayName("Quantity")]
        public double Quanitiy { get; set; }
        [DisplayName("Unit Cost")]
        public decimal Cost { get; set; }
        [DisplayName("VAT")]
        public double VAT { get; set; }
        [DisplayName("Total Cost"), ScaffoldColumn(false)]
        public decimal TotalCost { get; set; }
        [DisplayName("Grand Total"), ScaffoldColumn(false)]
        public decimal GrandTotal { get; set; }

        [DisplayName("Cost Type")]
        public CostType CostType { get; set; }

    }

    public class BoQItem : ConstructionItem
    {
        [Key]
        public int BoQItemId { get; set; }
        [ScaffoldColumn(false)]
        public int ItemsId { get; set; }
        public int BillOfQuantityId { get; set; }
        public virtual BillOfQuantity BillOfQuantity { get; set; }

        [DisplayName("Unit Rate")]
        public string UnitRate { get; set; }
    }

    public class UnitRate
    {
        public int UnitRateId { get; set; }
        [Display(Name = "Diameter")]
        public string Diameter { get; set; }
        [Display(Name = "Unit Weight")]
        public decimal UnitWeight { get; set; }
        public string Remark { get; set; }


    }
    public class UnitOfMeasurement : TrackUserOperation
    {
        public int UnitOfMeasurementId { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Abbreviation")]
        public string Abbreviation { get; set; }
        [Display(Name = "TakeOff Sheet Type")]
        public TakeOffSheetType TakeOffSheetType { get; set; }
        public string Remark { get; set; }


    }
    public class WorkCategory : TrackUserOperation
    {
        [Key]
        public int WorkCategoryId { get; set; }

        [DisplayName("Work Category Name")]
        [StringLength(100)]
        public string Name { get; set; }
        [DisplayName("Description")]
        [StringLength(200)]
        public string Description { get; set; }
        [DisplayName("Structure Type")]
        public int ConfigurationId { get; set; }
        public virtual Configuration Configuration { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int Index { get; set; }

    }

    public class MapWorkItemCategory
    {
        [Key]
        public int MapWorkCategoryId { get; set; }
        public int LookupId { get; set; }
        //public virtual Lookup Lookup { get; set; }
        public string ItemCategoryCode { get; set; }

        //   public virtual ItemCategory ItemCategory { get; set; }
        public string Remark { get; set; }
    }
    public class DirectLabourCost
    {
        [Key]
        public int DirectLabourCostId { get; set; }
        public virtual WorkCategory WorkCategory { get; set; }


    }

    public class DirectCost
    {
        [Key]
        public int DirectCostId { get; set; }
        public virtual WorkCategory WorkCategory { get; set; }
        //  public virtual WorkItem WorkItem { get; set;  }
        public virtual ICollection<DirectLabourCost> DirectLabourCosts { get; set; }
        public virtual ICollection<IndirectLabourCost> IndirectLabourCosts { get; set; }
        public virtual ICollection<MaterialCost> MaterialCosts { get; set; }
        public virtual ICollection<EquipmentCost> EquipmentCosts { get; set; }

    }
    public class MaterialCost
    {
        [Key]
        public int MaterialCostId { get; set; }
        public int ItemNumber { get; set; }
        [Display(Name = "Unit")]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid  Number")]
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }

    public class Coefficient
    {
        [Key]
        public int CoefficientId { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public double Ratio { get; set; }
        public double Weight { get; set; }
        public string Shrinkage { get; set; }
        public double Wastage { get; set; }
        [Display(Name = "Unit")]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        public double Quantity { get; set; }

    }
    public class LabourCost
    {
        [Key]
        public int LabourCostId { get; set; }
        //[Display (Name ="Job Title")]
        //public int LookupId { get; set; }
        //public virtual Lookup Lookup { get; set; }
        //public decimal BasicSalary { get; set; }
        //public decimal IndexFactor { get; set; }
        //[Display (Name ="Birr/Month")]
        //public decimal GrosSalBirMon { get; set; }

        //[Display(Name = "(Ideal) Birr/day")]
        //public decimal GrosIdelBirrDay { get; set; }
        //[Display(Name = "(Actual)  Birr/day")]
        //public decimal GrosActualBirrDay { get; set; }
        //[Display(Name = "(Ideal) Birr/day")]
        //public decimal PerdimeIdelBirrDay { get; set; }
        //[Display(Name = "(Actual)  Birr/day")]
        //public decimal PerdimeActualBirrDay { get; set; }
        //[Display(Name = "Desert Allowance (Birr/day)")]
        //public decimal DesertAllowance { get; set; }

        //[Display(Name = "Birr/day")]
        //public decimal NetPerDay { get; set; }
        //[Display(Name = "Birr/Hour")]
        //public decimal NetPerHour { get; set; }
    }

    public class IndirectLabourCost
    {
        [Key]
        public int IndirectLabourCostId { get; set; }
        public int LabourNumber { get; set; }
        [Display(Name = "Unit")]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        public int Quantity { get; set; }
        public double UnitFactor { get; set; }
    }

    public class EquipmentCost
    {
        public int EquipmentCostId { get; set; }
        public string Equipment { get; set; }
        [Display(Name = "Unit")]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        public int Quanitity { get; set; }
        public string RentalCost { get; set; }

    }


    public class EquipmentMobilization
    {
        [Key]
        public int EquipmentMobilizationId { get; set; }
        public int ItemNumber { get; set; }
        [Display(Name = "Type Of Machinery")]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        public int Quantity { get; set; }
        public double Trip { get; set; }
        public double Distance { get; set; }
        public double Speed { get; set; }
        public double Houre { get; set; }
        public decimal RentalCost { get; set; }
        public decimal TotalCost { get; set; }
    }
    public class ProjectJobTitle : TrackUserOperation
    {
        [Key]
        public int ProjectJobTitleId { get; set; }

        //[Required]
        //[Display(Name = "HR_SalaryStructure_SalaryJobTitle_RankId", ResourceType = typeof(Localization.Resources))]
        //public int? RankId { get; set; }
        //public virtual Rank Rank { get; set; }

        ////[Required]
        //[Display(Name = "HR_SalaryStructure_SalaryJobTitle_StepId", ResourceType = typeof(Localization.Resources))]
        //public int? StepId { get; set; }
        //public virtual Step Step { get; set; }

        [Required]
        [Display(Name = "HR_SalaryStructure_SalaryJobTitle_JobTitle", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Display(Name = "Name")]
        public string JobTitle { get; set; }
        [Required]
        [Display(Name = "HR_SalaryStructure_SalaryJobTitle_Salary", ResourceType = typeof(Localization.Resources))]
        public decimal Salary { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }

    public class MaterialPriceIndex : TrackUserOperation
    {
        [Key]
        public int MaterialPriceIndexId { get; set; }

        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }

        [Required]
        [DisplayName("Item Name")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public string Uom { get; set; }
        public SourceType SourceType { get; set; }

        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }

        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }

        [DisplayName("Original Cost")]
        public decimal OrginalCost { get; set; }

        [DisplayName("GRN Cost")]
        public decimal OrginalCostGRN { get; set; }

        [Display(Name = "External Cost")]
        public decimal ExternalCost { get; set; }
        public decimal TransportCost { get; set; }
        //public decimal LoadingUnloading { get; set; }
        public decimal TotalCost { get; set; }
        public decimal ApprovedCost { get; set; }
        public string Remark { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Of Information")]
        public DateTime DateOfInformation { get; set; }
        public bool IsExpired { get; set; }

        //public virtual Branch Branch { get; set; }
        //public virtual GLPeriod Period { get; set; }

    }
    public class MaterialTransportationCost : TrackUserOperation
    {
        [Key]
        public int MaterialTransportationCostId { get; set; }

        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }

        [DisplayName("Item Name")]
        public string ItemCode { get; set; }

        [Display(Name ="UoM")]
        public string UoM { get; set; }
        //public decimal StandardKgPerUnitMeasure { get; set; }
        [Display(Name = "Track Capacity")]
        public decimal TrackCapacityPerTrip { get; set; }
        [Display(Name = "Track Speed (km/hr)")]
        public decimal AverageSpeedOfTrack { get; set; }
        [Display(Name = "Rental (including fuel)")]
        public decimal RentalCostWithFuel { get; set; }
        [Display(Name = "Perdiem Driver")]
        public decimal DriverPerdiem { get; set; }
        [Display(Name = "Perdiem Asst. Driver")]
        public decimal AssistantDriverPerdiem { get; set; }
        [Display(Name = "Distance From src to Site")]
        public decimal AverageDistanceFromSourceToSite { get; set; }
        [Display(Name = "Required Time (hr)")]
        public decimal RequiredTime { get; set; }
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        [Display(Name = "Cost Per Unit")]
        public decimal CostPerUnit { get; set; }
        [Display(Name = "Loading unloading (per unit)")]
        public decimal LoadingUnloadingCost { get; set; }
        [Display(Name = "Total Cost (per unit)")]
        public decimal TotalCostPerUnit { get; set; }

        //public decimal SingleTripToCover { get; set; }

        //public decimal TimeTakenToArriveInHours { get; set; }
        //public decimal TimeTakenToArriveInDays { get; set; }
        //public decimal TotalTimeTakenForRoundTrip { get; set; }

        //public decimal RentalRateOfTruckPerHour { get; set; }
        //public decimal RentalRateOfTruckPerTrip { get; set; }

        //public decimal FuelConsumptionPerKm { get; set; }
        //public decimal FuelPricePerLiter { get; set; }
        //public decimal FuelExpense { get; set; }

        //public decimal DriverSalary { get; set; }
        //public decimal DriverBenefit { get; set; }
        //public decimal AssistantSalary { get; set; }
        //public decimal AssistantBenefit { get; set; }
        //public decimal TotalSalaryCost { get; set; }
        //public decimal LoadingUnloadingUnitCost { get; set; }
        //public decimal LoadingUnloadingTotalCost { get; set; }
        //public decimal TotalTransportCost { get; set; }
        //public decimal TotalTransportCostPerUnitPerTrip { get; set; }
        [Display(Name ="Project/Location")]
        public int BranchId { get; set; }
    }
    //public class MaterialTransportCostComponent
    //{
    //    public int MaterialTransportCostComponentId { get; set; }
    //    public MaterialTransportCostAttriute Attribute { get; set; }
    //    public decimal Value { get; set; }
    //    public string Remark { get; set; }
    //    public int ProjectInformationId { get; set; }

    //}
    public class ConstructionItemCategory
    {
        [Key]
        public int ConstructionItemCategoryId { get; set; }
        [Display(Name="Item Category")]
        public string ItemCategoryCode { get; set; }
        public string Name { get; set; }
        public bool IsFuel { get; set; }
        public bool IsReinforcementBar { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        public string Remark { get; set; }

    }
    public class MonthlyFuelCost
    {
        [Key]
        [ScaffoldColumn(false)]
        public int MonthlyFuelCostId { get; set; }

        [Required]
        [Display(Name = "Fuel Type")]
        public MachineryFuelTypes FuelType { get; set; }

        [Required]
        [Display(Name = "Year")]
        public string Year { get; set; }

        [Required]
        [Display(Name = "Month")]
        public Months Month { get; set; }

        [Required]
        [Display(Name = "Price")]
        public decimal Price { get; set; }
    }

    public class YearViewModel
    {

        public string Title { get; set; }
        public string Description { get; set; }
    }
    public class LaborRateIndexFactorAttributeName
    {
        public int LaborRateIndexFactorAttributeNameId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class LaborRateIndexFactorAttribute
    {
        [Key]
        public int LaborRateIndexFactorAttributeId { get; set; }
        [Display(Name ="Description")]
        public int LaborRateIndexFactorAttributeNameId { get; set; }
        public string CalculationDescription { get; set; }
        public decimal? NumberOfDays { get; set; }
        public decimal? Index { get; set; }

        [Display(Name = "Approved Index")]
        public decimal? ApprovedIndex { get; set; }

        public virtual LaborRateIndexFactorAttributeName AttributeName { get; set; }

    }
}
