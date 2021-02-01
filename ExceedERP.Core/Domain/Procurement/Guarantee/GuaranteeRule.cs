using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Settings;

namespace ExceedERP.Core.Domain.Procurement.Guarantee
{
    public class GuaranteeRule : TrackUserSettingOperation
    {
        public int GuaranteeRuleId { get; set; }
        [Required]
        [Display(Name = "Procurement Category")]
        public ProcurementCategory ProcurementCategory { get; set; }
        [Required]
        [Display(Name = "Guarantee Type")]
        public GuaranteeTypes GuaranteeType{ get; set; }
        [Required]
        [Display(Name = "Guarantee Mode")]
        public string GuaranteeMode { get; set; }
    }
}