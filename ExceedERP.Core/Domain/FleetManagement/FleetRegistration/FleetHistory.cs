
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class FleetHistory : Operations
    {
        [ScaffoldColumn(false)]
       public Guid FleetHistoryID { get; set; }
        public Guid FleetsID { get; set; } 
       public string FuelStandard { get; set; }

             [DisplayName("Rental Rate  ")]
       public Nullable<decimal> RentalRate { get; set; }
        [DisplayName("Annual Depreciation Cost ")]
       public Nullable<int> AnnualDepreciationCost { get; set; }

      [ForeignKey("FleetsID")]
       public virtual Fleets Fleets { get; set; }
    }
}
