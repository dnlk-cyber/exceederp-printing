using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;


namespace ExceedERP.Core.Domain.Finance.GL
{
    public class GLPeriod : TrackUserOperation
    {
        public int GLPeriodId { get; set; }
         [Display(Name = "GLPeriod_GlFiscalYearId", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        public int Index { get; set; }
        [Required]
        [Display(Name = "GLPeriod_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "GLPeriod_DateFrom", ResourceType = typeof(Localization.Resources))]
        public DateTime DateFrom { get; set; }
        [Display(Name = "GLPeriod_DateFrom", ResourceType = typeof(Localization.Resources))]
        public string DateFromAmharic { get; set; }
       [Display(Name = "GLPeriod_DateTo", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; }
          [Display(Name = "GLPeriod_DateTo", ResourceType = typeof(Localization.Resources))]
       public string DateToAmharic { get; set; }
        [Required]

        public FiscalYearStatus Status { get; set; }
        public bool IsPermanentlyClosed { get; set; }
        
    }
   
    
    
}
