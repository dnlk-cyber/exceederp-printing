using ExceedERP.Core.Domain.Common;
using System;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public class EquipmentLocation : TrackUserSettingOperation
    {
        public Guid EquipmentLocationId { get; set; }
        public Guid FleetId { get; set; }
        public string Name { get; set; }
        public string Condition { get; set; }
        public decimal Odometer { get; set; }
        
    }
}
