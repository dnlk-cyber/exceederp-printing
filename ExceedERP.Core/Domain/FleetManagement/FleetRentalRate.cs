using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement
{
    public class FleetRentalRate
    {
        [Key]
        public int FleetRentalRateId { get; set; }
        public int BranchId { get; set; }
        [DataType(DataType.Date)]
        public DateTime Year { get; set; }
        public int GlFiscalYearId { get; set; } // GlFiscalYear
    }

    public class EquipmentRate 
    {
        [Key]
        public int EquipmentRateId { get; set; }
        [Display(Name ="Equipment Category")]
        public Guid EquipmentCatagoryId { get; set; }
        [Display(Name ="Equipment Type")]
        public Guid EquipmentTypeId { get; set; }
        [DataType(DataType.Date)]
        public DateTime ManufacturingYearFrom { get; set; }
        [DataType(DataType.Date)]
        public DateTime ManufacturingYearTo { get; set; }
        public string UoM { get; set; }
        public decimal? CapacityFrom { get; set; }
        public decimal? CapacityTo { get; set; }

        public decimal Rate { get; set; }
        public decimal IdleRate { get; set; }

        public int FleetRentalRateId { get; set; }
        public FleetRentalRate FleetRentalRate { get; set; }

        // DumpTruck
        public decimal DistanceFrom { get; set; }
        public decimal DistanceTo { get; set; }
        public DumpTruckTypeOfWork? DumpTruckTypeOfWork { get; set; }
       
        // Shower Truck
        public SprayingType SprayingType { get; set; }
        public string Remark { get; set; }

        public FleetRoadType? RoadType { get; set; }

        //Light-Vehicle
        public int NumberOfSeatsFrom { get; set; }
        public int NumberOfSeatsTo { get; set; }       
        public bool IsDump { get; set; }


    }

    public enum FleetRoadType
    {
        Asphalt,
        Gravel

    }
    public enum SprayingType
    {
        Gravity,
        Automatic
    }

    public enum DumpTruckTypeOfWork
    {
        Stone,
        Soil,
        Gravel,
        Quarry,
        Sand,
        Other
    }
}
