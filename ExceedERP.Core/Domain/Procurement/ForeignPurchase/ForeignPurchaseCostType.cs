using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.ForeignPurchase
{
    public class ForeignPurchaseCostType : TrackUserOperation
    {
        public int ForeignPurchaseCostTypeId { get; set; }
        [Display(Name = "Cost Name")]
        public string CostName { get; set; }

    }
}