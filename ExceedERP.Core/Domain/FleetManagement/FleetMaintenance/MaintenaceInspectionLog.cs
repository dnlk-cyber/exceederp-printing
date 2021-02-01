using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
   public class MaintenaceInspectionLog : TrackUserOperation
    {
       public Guid MaintenaceInspectionLogId { get; set; }
       public Guid JobId { get; set; }
       public string Name { get; set; }
       public bool IsActive { get; set; }
       public bool IsAccessory { get; set; }
    }
}
