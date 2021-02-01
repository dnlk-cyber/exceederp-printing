using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class LineCreditSettlement : Operations
    {
       public int LineCreditSettlementId { get; set; }
       public int CreditSettlementId { get; set; }
       public int InvoiceId { get; set; }

       public string CurrentAmmount { get; set; }

       public decimal SettelmentAmount { get; set; }
       public decimal RemainingAmount { get; set; }

        [ForeignKey("CreditSettlementId")]
       public virtual CreditSettlement CreditSettlement { get; set; }
    }
}
