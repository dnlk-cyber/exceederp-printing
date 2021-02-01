using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class Period:TrackUserOperation 
    {
        [Key]
        public int PeriodId { get; set; }
        [Required]
        [Display(Name = "Common_Period_PeriodName", ResourceType = typeof(Localization.Resources))]
        public string PeriodName { get; set; }
        [Required]
        [Display(Name = "Common_Period_PeriodType", ResourceType = typeof(Localization.Resources))]
        public PeriodType PeriodType { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Common_Period_StartDate", ResourceType = typeof(Localization.Resources))]
        public DateTime StartDate { get; set; }
        [Display(Name = "Common_Period_StartDate", ResourceType = typeof(Localization.Resources))]
        public string AmharicStartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Common_Period_EndDate", ResourceType = typeof(Localization.Resources))]
        public DateTime EndDate { get; set; }
     [Display(Name = "Common_Period_EndDate", ResourceType = typeof(Localization.Resources))]
        public string AmharicEndDate { get; set; }
        [Display(Name = "Common_Period_PeriodName", ResourceType = typeof(Localization.Resources))]
        public int? ParentId { get; set; }
        

        public int Index { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }


    }
}