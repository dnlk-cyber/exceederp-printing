using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public class EquipmentBattery : TrackUserOperation
    {
       public Guid EquipmentBatteryId { get; set; }
       [Required]
       public string StoreCode { get; set; }
       [Required]
       public string SerialNo { get; set; }
       public string Make { get; set; }
       public string MakeCountry { get; set; }
       public string Type { get; set; }
       public string Condition { get; set; }
       public decimal Odometer { get; set; }
       public decimal UnitPrice { get; set; }
       public Guid fleetId { get; set; }  
       public bool Isattached { get; set; }
 
    }

}
