using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
   public class FuelConsumptionAlertLevel : TrackUserSettingOperation
    {

       public Guid FuelConsumptionAlertLevelId { get; set; }
       [Required]
       public string Name  { get; set; }
       [Required]
       [DisplayName("From Value")]
       public int FromValue { get; set; }
       [Required]
       [DisplayName("To Value")]
       public int ToValue { get; set; }

    }

}
