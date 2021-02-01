using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public class GLPeriod : TrackUserOperation
    {
        public int GLPeriodId { get; set; }
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date ")]
        public DateTime DateFrom { get; set; }
        [Display(Name = "End Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; }
        [Required]
        
        public FiscalYearStatus Status { get; set; }
        public bool IsPermanentlyClosed { get; set; }
    }
}
