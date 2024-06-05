using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
   public class ExternalComponent : TrackUserSettingOperation
    {
       public Guid ExternalComponentId { get; set; }
       public Guid JobId { get; set; }
       [Required]
       [DisplayName("Component Name")]
       public string ComponentName { get; set; }
           [Required]
        [DisplayName("Enterprise Name")]
       public string EnterpriseName { get; set; }
          [DisplayName("Equipment Type")]
       public string EquipmentType { get; set; }
           [Required]
       public string Manufacturer { get; set; }
           [Required]
       public string Model { get; set; }
       public string ChasisNo { get; set; }
       public string  Code { get; set; }
       public string KMReading { get; set; }
           [Required]
         [DisplayName("Maintaing Company")]
       public string MaintaingCompany { get; set; }
   }
}
