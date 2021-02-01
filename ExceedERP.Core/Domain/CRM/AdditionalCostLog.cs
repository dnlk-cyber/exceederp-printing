using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
   public class AdditionalCostLog
    {
     public int  AdditionalCostLogId {get; set;}
     public int LineItemId { get; set; }
     public int AdditionalCostId { get; set; }
     public decimal Value { get; set; }

     public virtual AdditionalCost AdditionalCost { get; set; }
     public virtual LineItem LineItem { get; set; }

    }
}
