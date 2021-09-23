using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public partial class DailyCareDetailLog : TrackUserSettingOperation
    {
        public Guid DailyCareDetailLogId { get; set; }
        public Guid DailyCareLogId { get; set; }
           [DisplayName("Category")]
        public Guid DailyCareCategoryId { get; set; }
           [DisplayName("Care")]
        public Guid DailyCareId { get; set; }

        public string Description { get; set; }
        public string IsActive { get; set; }
  
        [NotMapped]
        public string Category { get; set; }
        [NotMapped]
        public string Care { get; set; }

        public virtual DailyCareLog DailyCareLog { get; set; }
        public virtual DailyCare DailyCare { get; set; }

    }

   
}
