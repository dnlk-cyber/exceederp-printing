using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class AdvancePayment : TrackUserSettingOperation
    {
        public int AdvancePaymentId { get; set; }
        public int Customer { get; set; }
        public decimal GrandTotal { get; set; }
        [ForeignKey("Customer")]
        public virtual OrganizationCustomer OrganizationCustomer { get; set; }
        [Display(Name ="Branch")]

        public int BranchId { get; set; }
    }
}
