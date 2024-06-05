
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore
{
    public partial class CannibalizationDetail : Operations
    {
        public Guid CannibalizationDetailId { get; set; }
        public Guid CannibalizationId { get; set; }
        
         [DisplayName("  Part No")]
        [Required]
        public string Model { get; set; }
        public enum UnitofMeasurementenum { Set, Pcs }
        [DisplayName("Unit of Measurement")]
      
        public string UnitofMeasurement { get; set; }
        [DisplayName("Part Name")]
        [Required]
        public string PartName { get; set; }
        [DisplayName("Part Type")]
        public string PartType { get; set; }
        [DisplayName("WHPR No")]
        public string WHPRNo { get; set; }
        [Required]
        public  int  Quantity { get; set; }

        public string Status { get; set; }
        public string TakenBy { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }


    }
}