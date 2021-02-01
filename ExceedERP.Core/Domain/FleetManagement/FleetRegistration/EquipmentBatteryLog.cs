using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public class EquipmentBatteryLog : TrackUserOperation
    {
        public Guid EquipmentBatteryLogId { get; set; }
        public Guid EquipmentBatteryId { get; set; }
        public Guid FleetId { get; set; }
        public string Name { get; set; }
        public string StoreCode { get; set; }
        public string SerialNo { get; set; }
        public string Type { get; set; }
        public string Make { get; set; }
        public string MakeCountry { get; set; }
        public string Size { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProjectName { get; set; }
        public decimal IntialOdoMeter { get; set; }
        public decimal FinalOdoMeter { get; set; }
        public string Reason { get; set; }
         public bool Isattached { get; set; }
        public string Condition { get; set; }
        public decimal Efficency { get; set; }
        public virtual Fleets Fleet { get; set; }
    }
}
