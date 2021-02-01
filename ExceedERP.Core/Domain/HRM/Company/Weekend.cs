using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class Weekend : TrackUserOperation
    {
        [Key]
        public int WeekendId { get; set; }
        [Display(Name = "HR_Company_Weekend_IsHalfDay", ResourceType = typeof(Localization.Resources))]
        public bool IsHalfDay { get; set; }
        [Display(Name = "HR_Company_Weekend_Name", ResourceType = typeof(Localization.Resources))]
        public WeekendName Name { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }
}