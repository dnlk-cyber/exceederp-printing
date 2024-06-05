
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
    public partial class FleetReturnItem : Operations
    {
        [ScaffoldColumn(false)]
        public int FleetReturnItemId { get; set; }
        public int FleetReturnId { get; set; } 
       public string IdentifcationNo { get; set; }

       public string CostCode { get; set; }
       public string Description { get; set; }
       public string PartNo { get; set; }
       public string Location { get; set; }
       public string Requested { get; set; }

       public string Recieved { get; set; }
       public string Unit { get; set; }
       public decimal UnitPriceBirr { get; set; }

       public decimal UnitPriceCent { get; set; }
       public decimal TotalPriceCent { get; set; }
       public decimal TotalPriceBirr { get; set; }
    
    
    }
}
