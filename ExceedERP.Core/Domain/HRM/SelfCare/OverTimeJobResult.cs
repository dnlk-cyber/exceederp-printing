using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.SelfCare
{
    public class OverTimeJobResult
    {
        [Key]
        public int OverTimeJobResultId { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeJobResult_Date", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        public DateTime Date { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeJobResult_JobResult", ResourceType = typeof(Localization.Resources))]
        public string JobResult { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeJobResult_Cost", ResourceType = typeof(Localization.Resources))]
        public string Cost { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeJobResult_Benefit", ResourceType = typeof(Localization.Resources))]
        public string Benefit { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        
        public int OverTimeRequestId { get; set; }
        public virtual OverTimeRequest OverTimeRequest { get; set; }

    }
}