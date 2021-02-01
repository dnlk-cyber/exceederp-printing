using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public class ServiceCrewWorkLog : Operations
    {
        [Key]
        public Guid ServiceCrewWorkLogId { get; set; }
        public Guid JobId { get; set; }
         [DisplayName("Service")]
        public Guid JobOrderDetailId { get; set; }

        [DisplayName("Mecanic")]
        public Guid MecanicId { get; set; }
        public int Date { get; set; }

        [DisplayName("  Morning From")]
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh-mm}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> WorkingHoursMorningFrom { get; set; }
        [DisplayName("  Morning To")]
          [Required]
        [DisplayFormat(DataFormatString = "{0:hh-mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public Nullable<DateTime> WorkingHoursMorningto { get; set; }
        [DisplayName("  Afternoon From ")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:hh-mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public Nullable<DateTime> WorkingHoursAfternoonFrom { get; set; }
        [DisplayName("  Afternoon To ")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:hh-mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public Nullable<DateTime> WorkingHoursAfternoonTo { get; set; }
        public virtual JobOrderDetail JobOrderDetail { get; set; }
        [ForeignKey("MecanicId")]
        public virtual MechanicCrew MechanicCrew { get; set; }
    }
}
