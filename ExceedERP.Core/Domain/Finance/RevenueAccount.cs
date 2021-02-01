using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance
{
    public class RevenueAccount : TrackUserOperation
    {
        public int RevenueAccountId { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "AR")]
        public bool ApplyToAr { get; set; }
        [Display(Name = "AP")]
        public bool ApplyToAp { get; set; }
        [Display(Name = "Inventory")]
        public bool ApplyToInventory { get; set; }
        [Display(Name = "AR Account Code")]
        public string ArAccountCode { get; set; }
        [Display(Name = "AP Account Code")]
        public string ApAccountCode { get; set; }
        [Display(Name = "Withholding Account Code")]
        public string InventoryAccountCode { get; set; }
        [Display(Name = "Enable ?")]
        public bool Enable { get; set; }
    }
}
