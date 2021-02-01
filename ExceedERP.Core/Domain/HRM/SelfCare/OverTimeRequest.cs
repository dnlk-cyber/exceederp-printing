using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.SelfCare
{
    public class OverTimeRequest
    {
        [Key]
        public int OverTimeRequestId { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeRequest_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        public virtual MainCompanyStructure MainCompanyStructure { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeRequest_FromDate", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        public DateTime FromDate { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeRequest_FromDate", ResourceType = typeof(Localization.Resources))]
        public string FromDateAm { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeRequest_ToDate", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        public DateTime ToDate { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeRequest_ToDate", ResourceType = typeof(Localization.Resources))]
        public string ToDateAm { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeRequest_FromTime", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}")]

        public DateTime FromTime { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeRequest_ToTime", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}")]
        
        public DateTime ToTime { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeRequest_Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }

    }
}