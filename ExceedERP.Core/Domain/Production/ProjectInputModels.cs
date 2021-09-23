using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Production
{
    public class ProductionExternalEquipment : TrackUserOperation
    {
        [Key]
        public int ProductionExternalEquipmentId { get; set; }
        [Display(Name = "Equipment Name")]
        public Guid FleetsID { get; set; }

        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Unit Factory")]
        public decimal UnitFactory { get; set; }

        [Required]
        [Display(Name = "Rental Cost")]
        public decimal RentalCost { get; set; }

        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }

        public virtual Fleets Fleet { get; set; }

    }
    public class ProductionInternalEquipment : TrackUserOperation
    {
        public int ProductionInternalEquipmentId { get; set; }
        [Required]
        [Display(Name = "Type of Equipment")]
        public Guid FleetsID { get; set; }
        //
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Display(Name = "Capacity (HP)")]
        public string Capacity { get; set; }

        [Display(Name = "Purchase cost")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid  Number")]
        public decimal PurchaseCost { get; set; }

        [Display(Name = "Maximum Life (years)")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid  Number")]
        public decimal MaximumLife { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid  Number")]
        [Display(Name = "Hours Used Per Year")]
        public int HoursUsedPerYear { get; set; }

        [Display(Name = "Depereciation Cost")]
        public decimal DepCostPerHour { get; set; }

        [Display(Name = "Investment Cost")]
        public decimal InvestmentCost { get; set; }

        [Display(Name = "Total Owning Cost")]
        public decimal TotalOwningCost { get; set; }

        [Display(Name = "Fuel Cost")]
        public decimal FuelCost { get; set; }

        [Display(Name = "Lub. Cost")]
        public decimal LubCost { get; set; }

        [Display(Name = "Maint. & Repair")]
        public decimal MaintAndRepair { get; set; }

        [Display(Name = "Tyre Cost")]
        public decimal TyreCost { get; set; }

        [Display(Name = "Total Operating Cost")]
        public decimal TotalOperatingCost { get; set; }

        [Display(Name = "Rate /hour")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Index for Sever Condition")]
        public decimal IndexForSeverCondition { get; set; }

        [Display(Name = "Rate /hour (without fuel)")]
        public decimal RatePerHourWithoutFuel { get; set; }

        public string Remark { get; set; }
        public virtual Fleets Fleet { get; set; }

    }
    //
    // Summary :
    //      direct labor input
    public class ProductionLabor
    {
        [Key]
        public int ProductionLaborId { get; set; }

    }
    public class ProductionMaterial
    {
        [Key]
        public int ProductionMaterialId { get; set; }
    }

    //public class 
}
