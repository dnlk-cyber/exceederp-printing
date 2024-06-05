using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public class ServiceCrew : TrackUserOperation
    {

      public  Guid ServiceCrewID { get; set; }
       [DisplayName("Service")]
       [Required]
      public Guid ServiceID { get; set; }
       public Guid MechanicCrewID { get; set; }
      public string ServiceName { get; set; }
      public string Type { get; set; }

        public bool IsActive { get; set; }

      [DisplayName("Hourly Wage")]
      public decimal HourlyWage { get; set; }
       

        [NotMapped]
      [DisplayName("Labour Cost")]
      public decimal? LabourCost { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

      public Nullable<DateTime> ActivatedDate { get; set; }

      public virtual MechanicCrew MechanicCrews { get; set; }


     
    }
}
