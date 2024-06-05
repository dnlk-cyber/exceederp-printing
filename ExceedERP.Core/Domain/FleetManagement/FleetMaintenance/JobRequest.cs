using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public class JobRequest : Operations
    {
        [Key]
        [DisplayName(" Request Number ")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Num { get; set; }
        public int? RequestNumber { get; set; }
        public Guid JobRequestId { get; set; }
        public bool IsPM { get; set; }
        [Required]
        [DisplayName(" Equipment ")]
        public Guid FleetsID { get; set; }
        [DisplayName(" Equipment Name ")]
        public string EquipmentName { get; set; }
        public string Requester { get; set; }
        [DisplayName(" Operator Name ")]
        public string OperatorName { get; set; }
        [DisplayName(" Fuel Amount ")]
        public string FuelAmount { get; set; }
        public decimal Odometer { get; set; }
        [DisplayName(" Asset Num ")]
        public string AssetNum { get; set; }
        [DisplayName(" Project Name")]
        public string ProjectName { get; set; }
          public decimal LastKmHr { get; set; }
        [NotMapped]
        public string Model { get; set; }

        [NotMapped]
        public string EquipmentType { get; set; }

    }
}
