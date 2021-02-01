using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL.Dynamic
{
    public class GLSegmentCrossValidation : TrackUserSettingOperation
    {
        public int GLSegmentCrossValidationId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Display(Name = "Enable?")]
        public bool Enabled { get; set; }
        [Display(Name = "Error Message")]
        public string ErrMsg { get; set; }
        [Display(Name = "Error Segment")]
        public string ErrorSegment { get; set; }
        [Display(Name = "From")]
        public string ErrorSegmentFrom { get; set; }
        [Display(Name = "To")]
        public string ErrorSegmentTo { get; set; }
        public virtual ICollection<GLCrossValidationRuleElement> GlCrossValidationRuleElements { get; set; }
    }
    public class GLCrossValidationRuleElement : TrackUserSettingOperation
    {
        public int GLCrossValidationRuleElementId { get; set; }
        public int GLSegmentCrossValidationId { get; set; }
        public virtual GLSegmentCrossValidation GlSegmentCrossValidation { get; set; }
        [Required]
        [Display(Name = "Segment Type")]
        public string SegmentType { get; set; }
        [Display(Name = "From")]
        public string From { get; set; }
        [Display(Name = "To")]
        public string To { get; set; }
    }
}
