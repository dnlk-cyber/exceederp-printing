using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Finance.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class CreditSettlement : Operations
    {
       public int CreditSettlementId { get; set; }
        public int Customer { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public int RecieveMethod { get; set; }

        [NotMapped]
        public decimal GrandTotal { get; set; }

        [ForeignKey("Customer")]
        public virtual OrganizationCustomer OrganizationCustomer { get; set; }
        [ForeignKey("RecieveMethod")]
        public virtual PaymentMethod PaymentMethod { get; set; }

        public virtual ICollection<LineCreditSettlement> LineCreditSettlement { get; set; }
    }
}
