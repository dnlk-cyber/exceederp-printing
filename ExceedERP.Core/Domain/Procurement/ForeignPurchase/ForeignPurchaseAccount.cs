using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.ForeignPurchase
{
    public class ForeignPurchaseAccount : TrackUserOperation
    {
        public int ForeignPurchaseAccountId { get; set; }
        public int ForeignPurchaseId { get; set; }
        public virtual ForeignPurchase ForeignPurchase { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
    }
}