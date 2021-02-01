using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL.Dynamic
{
    public enum DynamicSegmentTypes
    {
        Other,
        NaturalAccount,
        SubAccount,
        CostCenter,
        Location,
        BusinessUnit
    }
    public class GLSegmentSetup : TrackUserSettingOperation
    {
        public int GLSegmentSetupId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [Display(Name = "Segment Length")]
        public int SegmentLength { get; set; }
        [Required]
        [Display(Name = "Column")]
        public int Sequence { get; set; }
        public bool Displayed { get; set; }
        public bool Enabled { get; set; }
        public DynamicSegmentTypes SegmentType { get; set; }
    }
}
