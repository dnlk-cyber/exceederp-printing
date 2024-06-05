using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
   public class HRFleetMapping : TrackUserSettingOperation
    {
       public Guid HRFleetMappingId { get; set; }
       [Required]
       [DisplayName("Job Title")]
       public int JobTitleCode { get; set; }
       public FleetPlacmentType PlacmentType { get; set; }
       public string Remark { get; set; }
    }
}
