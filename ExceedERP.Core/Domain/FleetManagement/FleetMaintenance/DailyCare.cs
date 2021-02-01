using ExceedERP.Core.Domain.Common;
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
        public partial  class DailyCare  : TrackUserSettingOperation
    {
            public Guid DailyCareId { get; set; }
            [DisplayName("Category")]
            public Guid DailyCareCategoryId { get; set; }
              [Required]
            public string Name { get; set; }
            public string Description { get; set; }

            [NotMapped]
            public string Category { get; set; }

            public virtual DailyCareCategory DailyCareCategory { get; set; }
          
              }
}
