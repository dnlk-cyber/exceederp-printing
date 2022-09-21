using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL.Dynamic
{
    public class GLChartOfAccountSegment : TrackUserOperation
    {
        public int GLChartOfAccountSegmentId { get; set; }
        public int GLSegmentSetupId { get; set; }
        public virtual GLSegmentSetup GlSegmentSetup { get; set; }
        [Required]
        [Display(Name = "Values")]
        public string Values { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Parent")]
        public string HasParent { get; set; }
        [Display(Name = "Is Enabled?")]
        public bool Enable { get; set; }
        [Display(Name = "Is Parent?")]
        public bool Parent { get; set; }
        [Required]
        [Display(Name = "Allow Budgeting")]
        public bool AllowBudgeting { get; set; }
        [Display(Name = "Allow Posting")]
        public bool AllowPosting { get; set; }
        public GlCostCenterTypes Type { get; set; }
    }
}
