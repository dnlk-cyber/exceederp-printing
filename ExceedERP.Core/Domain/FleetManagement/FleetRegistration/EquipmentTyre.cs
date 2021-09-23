using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public class EquipmentTyre : TrackUserOperation
    {
        public Guid EquipmentTyreId { get; set; }
         
        public string StoreCode { get; set; }
        [Required]
        public string SerialNo { get; set; }
        public string Make { get; set; } 
        public string MakeCountry { get; set; }
        public string Type { get; set; }
        public string Tube { get; set; }
        public string Size { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OdoMeter { get; set; }
        public Guid fleetId { get; set; }
        public bool Isattached { get; set; }
        public string Condition { get; set; }

          
        public string Duration { get; set; }


    }
}
