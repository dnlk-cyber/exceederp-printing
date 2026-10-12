using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public  class QuarterLength: TrackUserSettingOperation
    {
        [Key]
        public int QuarterLengthId { get; set; }
        [Range(1,12, ErrorMessage="The Length value should be between 1 and 12" )]
        [Display(Name = "HR_QuarterLength_Length", ResourceType = typeof(Localization.Resources))]
        public int Length { get; set; }
        public bool IsClosed { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Closed Date ")]
        public DateTime?  ClosedDate { get; set; }
        [Display(Name = "Closed By ")]
        public string  ClosedBy { get; set; }
        public string  Remark { get; set; }
        [Required]
        [Display(Name = "HR_QuarterLength_GlFiscalYearId", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
    }

    public class Quarter: TrackUserSettingOperation
    {
        [Key]
       public int QuarterId { get; set; }
        [Required]
        [Display(Name = "HR_Quarter_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        [Required]
        [Display(Name = "HR_Quarter_AmharicName", ResourceType = typeof(Localization.Resources))]
        public string AmharicName { get; set; }
        [Required]
        [Display(Name = "HR_Quarter_Sequence", ResourceType = typeof(Localization.Resources))]
        [Range(1, 12, ErrorMessage = "The Sequence value should be between 1 and 12")]
        public int Sequence { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public bool IsClosed { get; set; }
        [Display(Name = "Start Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateFrom { get; set; }
        [Display(Name = "End Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateTo { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Closed Date ")]
        public DateTime? ClosedDate { get; set; }
        [Display(Name = "Closed By ")]
        public string ClosedBy { get; set; }
        public int QuarterLengthId { get; set; }
        public virtual QuarterLength QuarterLength { get; set; }
    }
}
