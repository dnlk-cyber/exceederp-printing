using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public class GlSegmentLength : TrackUserSettingOperation
    {
        public int GlSegmentLengthId { get; set; }
        [Required]
        [Display(Name = "Account Length")]
        public int AccountLength { get; set; }
        [Required]
        [Display(Name = "Location Length")]
        public int LocationLength { get; set; }
        [Required]
        [Display(Name = "Cost Center Length")]
        public int CostCenterLength { get; set; }
        [Required]
        [Display(Name = "Sub Account Length")]
        public int SubAccountLength { get; set; }
    }
}
