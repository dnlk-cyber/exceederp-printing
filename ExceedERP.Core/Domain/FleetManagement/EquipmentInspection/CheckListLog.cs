using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.EquipmentInspection
{
   public class CheckListLog : Operations
    {
          public int CheckListLogId { get; set; }
          public string FleetId { get; set; }
          public string LineId { get; set; }
          public string LineName { get; set; }
          public string LineCode { get; set; }
          public string AnnualEquipmentInventoryInspectionId { get; set; }
          public string CategoryName { get; set; }
          public string Name { get; set; }
          public CheckListLogType Type { get; set; }
          public bool IsActive { get; set; }
          public bool IsAccessory { get; set; }
          public bool IsActiveInspection { get; set; }
    }
}
