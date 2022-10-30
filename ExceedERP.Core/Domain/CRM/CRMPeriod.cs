using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public  class CRMPeriod : TrackUserOperation
    {
        [Key]
        public int CRMPeriodId { get; set; }
        [Required]
        //[Display(Name = "GLPeriod_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "From")]
        public DateTime DateFrom { get; set; }

        [Display(Name = "From")]
        public string DateFromAmharic { get; set; }

        [Display(Name = "To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; }

        [Display(Name = "To")]
        public string DateToAmharic { get; set; }
        [Required]

        public FiscalYearStatus Status { get; set; }
        public bool IsPermanentlyClosed { get; set; }
    }
}
