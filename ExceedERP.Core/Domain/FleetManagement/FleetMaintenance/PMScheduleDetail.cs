using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Finance.GL;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public class PMScheduleDetail : Operations
    {
        public int PMScheduleDetailId { get; set; }
        public int PMScheduleId { get; set; }
          [DisplayName("Equipment")]
        public Guid FleetsId { get; set; }
          [DisplayName("Month")]
        public int GLPeriodId { get; set; }
        public Weeks Weeks { get; set; }
          [DisplayName("Service")]
        public Guid ServiceId { get; set; } 
        [NotMapped]
          public string EquipmentType { get; set; }
        [NotMapped]
        public string Period { get; set; }
        [NotMapped]
        public string ServiceName { get; set; }
        [JsonIgnore]
         public virtual Fleets Fleets { get; set; }
          [JsonIgnore]
         public virtual GLPeriod GLPeriod { get; set; }
          [JsonIgnore]
         public virtual Service Service { get; set; }
     }


}
