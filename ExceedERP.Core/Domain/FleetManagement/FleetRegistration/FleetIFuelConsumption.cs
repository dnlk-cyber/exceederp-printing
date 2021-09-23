
using System;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class FleetIFuelConsumption : TrackUserOperation
    {

        public int FleetIFuelConsumptionId { get; set; }
        [NotMapped]
        public string EquipmentCategory { get; set; }
        public Guid EquipmentTypeId { get; set; }
        public decimal MinConsupmtion { get; set; }
        public decimal MaxConsupmtion { get; set; }
        [ForeignKey("EquipmentTypeId")]
        public virtual EquipmentType EquipmentType { get; set; }
    }
}
