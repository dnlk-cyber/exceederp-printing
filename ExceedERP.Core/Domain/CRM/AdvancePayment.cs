using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
    public class AdvancePayment : TrackUserSettingOperation
    {
        public int AdvancePaymentId { get; set; }
        public int Customer { get; set; }
        public decimal GrandTotal { get; set; }
        [ForeignKey("Customer")]
        public virtual OrganizationCustomer OrganizationCustomer { get; set; }
    }
}
