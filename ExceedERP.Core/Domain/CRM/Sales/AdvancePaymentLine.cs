using System.Collections.Generic;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Finance.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class AdvancePaymentLine : TrackUserSettingOperation
    {

        public AdvancePaymentLine()
        {

            this.AdvancePaymentLog = new HashSet<AdvancePaymentLog>();
        }
        public int AdvancePaymentLineId { get; set; }
        public int AdvancePaymentId { get; set; }
        public int RecieveMethod { get; set; }
         public decimal Value { get; set; }

         [ForeignKey("RecieveMethod")]
         public virtual PaymentMethod PaymentMethod { get; set; }
         public virtual AdvancePayment AdvancePayment { get; set; }
         public virtual ICollection<AdvancePaymentLog> AdvancePaymentLog { get; set; }
    }
}
