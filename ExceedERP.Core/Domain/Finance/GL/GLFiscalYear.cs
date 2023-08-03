using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Finance.GL
{

    public enum FiscalYearStatus
    {
        FuturePeriod,
        Open,
        Close,
        PermanetlyClosed
    }
    public class GlFiscalYear : TrackUserOperation
    {
        public int GlFiscalYearId { get; set; }
        [Display(Name = "Year")]
        public string Name { get; set; }
        [Display(Name = "Start Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateFrom { get; set; }
        [Display(Name = "End Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; }
        [Display(Name = "GLPeriod_DateFrom", ResourceType = typeof(Localization.Resources))]
        public string DateFromAmharic { get; set; }
        [Display(Name = "GLPeriod_DateTo", ResourceType = typeof(Localization.Resources))]
        public string DateToAmharic { get; set; }
        [Required]
        public FiscalYearStatus Status { get; set; }
        public virtual ICollection<GLPeriod> GLPeriod { get; set; }
        public bool IsPermanentlyClosed { get; set; }
       
    }
}
