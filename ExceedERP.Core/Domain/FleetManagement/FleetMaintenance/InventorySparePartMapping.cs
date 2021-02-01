using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
   public class InventorySparePartMapping : TrackUserSettingOperation
    {
       public Guid InventorySparePartMappingId { get; set; }
       [DisplayName("Inventory Category")]
       public string InventoryCategoryId { get; set; }
       public SpareType SpareType { get; set; }
       public string Remark { get; set; }
    }
}
