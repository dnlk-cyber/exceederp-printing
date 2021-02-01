
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Service : TrackUserOperation
    {
      
        public Guid ServiceID { get; set; }
        [DisplayName("Plan")]
        [Required]
        public Guid PlannerID { get; set; }
         [DisplayName("Repair Type")]
        public Guid RepairTypeID { get; set; }
        [DisplayName("Section")]
        [Required]
        public Guid SectionId { get; set; }

        [DisplayName("Inspection Name")]
        [Required]
        public string ServiceName { get; set; }
        public string ServiceCode { get; set; }
        public string Type { get; set; }
         [DisplayName(" STD Time (Minute) ")]
        public Nullable<decimal> STDTime { get; set; }
         public Nullable<decimal> LabourCost { get; set; }
        public string Priority { get; set; }
        [DisplayName("Date Interval Type")]
        [Required]
        public string DateIntervalType { get; set; }
        [DisplayName("Date Interval ")]
        [Required]
        public Nullable<int> DateInterval { get; set; }
        [DisplayName("Mileage Interval ")]
        [Required]
        public Nullable<int> MileageInterval { get; set; }
        public Nullable<DateTime> DateCreated { get; set; }
        public enum IsPmenum { Planned , UnPlanned  }
        [DisplayName(" Maintenance Type ")]
        [Required]
        public string IsPm { get; set; }
        public enum ServiceTypeenum { Outsourced , Internal , OtherCompany  }
        [DisplayName(" Service Type ")]
        [Required]
        public string ServiceType { get; set; }
        public string ApprovedBy { get; set; }
        public virtual Planner Planner { get; set; }

        public virtual RepairType RepairType { get; set; }
        [ForeignKey("SectionId")]
        public virtual WorkUnitSection WorkUnitSection { get; set; }
    }
}
