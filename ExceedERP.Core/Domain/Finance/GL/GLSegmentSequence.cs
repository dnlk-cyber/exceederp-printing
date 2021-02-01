using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public class GlSegmentSequence : TrackUserSettingOperation
    {
        public int GlSegmentSequenceId { get; set; }
        [Required]
        [Display(Name = "First Segment")]
        public GlSegmentTypes FirstSegment { get; set; }
        [Required]
        [Display(Name = "Second Segment")]
        public GlSegmentTypes SecondSegment { get; set; }
        [Required]
        [Display(Name = "Third Segment")]
        public GlSegmentTypes ThirdSegment { get; set; }
        [Required]
        [Display(Name = "Fourth Segment")]
        public GlSegmentTypes FourthSegment { get; set; }
    }
}
