using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class ServiceCrew : TrackUserOperation
    {

        public Guid ServiceCrewID { get; set; }
        [DisplayName("Service")]
        [Required]
        public Guid ServiceID { get; set; }
        [Display(Name ="Crew Name")]
        public Guid MechanicCrewID { get; set; }
        [Display(Name = "Mechanic Type")]
        public MechanicType MechanicType { get; set; }

        public string ServiceName { get; set; }
        public string Type { get; set; }

        public bool IsActive { get; set; }

        [DisplayName("Hourly Wage")]
        public decimal HourlyWage { get; set; }

        public decimal TotalLabourHours { get; set; }
        //[NotMapped]
        [DisplayName("Labour Cost")]
        public decimal LabourCost { get; set; }
        [DisplayName("TotalLabour Cost")]
        public decimal TotalLabourCost { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public Nullable<DateTime> ActivatedDate { get; set; }

        public virtual MechanicCrew MechanicCrews { get; set; }



    }
}
