using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Common
{
    public class PaymentMethod : TrackUserOperation
    {
        public int PaymentMethodID { get; set; }
        [Required]
        public string Method { get; set; }
        [Display(Name = "Branch")]
        public int? BranchId { get; set; }
        [Display(Name = "Business Unit")]
        public int? BusinessUnitId { get; set; }
        [Display(Name = "Apply to AR")]
        public bool ApplyToAr { get; set; }
        [Display(Name = "AR Account Code")]
        public string ArAccountCode { get; set; }
        [Display(Name = "Apply to AP")]
        public bool ApplyToAp { get; set; }
        [Display(Name = "AP Account Code")]
        public string ApAccountCode { get; set; }
        [Display(Name = "Enable ?")]
        public bool Enable { get; set; }
        public string Description { get; set; }
    }
}
