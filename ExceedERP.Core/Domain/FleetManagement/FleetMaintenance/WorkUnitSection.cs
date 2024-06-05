using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
   public class WorkUnitSection : TrackUserSettingOperation
    {
       public Guid WorkUnitSectionId { get; set; }
       [Required]
       public string Name { get; set; }
       public string Description { get; set; }
       public string Branch { get; set; }
    }
}
