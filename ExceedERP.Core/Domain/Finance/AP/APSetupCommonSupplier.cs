using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class ApSetupCommonSupplier : TrackUserSettingOperation
    {
        public int ApSetupCommonSupplierId { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
    }
}
