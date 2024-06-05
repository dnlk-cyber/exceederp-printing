using System;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel;
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Core.Domain.Maintenance
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
