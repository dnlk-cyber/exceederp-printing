using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
   public class EquipmentTyreLog : TrackUserOperation
    {
       public Guid EquipmentTyreLogId { get; set; }
       public string Equipment { get; set; }
       public Guid FleetId { get; set; }
        public string Type { get; set; }
        public string StoreCode { get; set; }
        [Required]
        public string SerialNo { get; set; }
        public string Make { get; set; }
        public string MakeCountry { get; set; }
        public string ProjectName { get; set; }
        public string Size { get; set; }
        public decimal IntialOdoMeter { get; set; }
        public decimal FinalOdoMeter { get; set; }
     
        public decimal UnitPrice { get; set; }
          public bool Isattached { get; set; }
        public string Condition { get; set; }
        public string Reason { get; set; }
        public decimal Efficency { get; set; }
       [NotMapped]
        public decimal DiffrenceOdoMeter { get; set; }
        public virtual Fleets Fleet { get; set; }
    }
}
