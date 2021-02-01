using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
   public class EquipmentArrival : Operations
    {

       public Guid EquipmentArrivalId { get; set; }
       public Guid AssignmentId { get; set; }
       [DisplayName("Equipment")]
       public Guid FleetsId { get; set; }
        [DisplayName("Equipment Type")]
       public string EquipmentType { get; set; }
       public string Model { get; set; }
         [DisplayName("Asset No")]
       public string DCENo { get; set; }
       public string OwnedBy { get; set; }
       [DataType(DataType.Date)]
       [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
       public DateTime Date { get; set; }
         [DisplayName("KMHR Reading")]
       public decimal KMHRReading  { get; set; }
        [DisplayName("Means Of Travelling")]
       [Required]
       public string MeansOfTravelling { get; set; }
           [DisplayName("Operator")]
       public string OperatorName { get; set; }

        
       public virtual Fleets  Fleet { get; set;}
    }
}
