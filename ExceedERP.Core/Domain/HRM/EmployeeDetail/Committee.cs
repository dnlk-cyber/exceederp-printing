using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using System.Collections.Generic;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class Committee:TrackUserOperation 
    {
        [Key ]
        public int CommitteeId { get; set; }
        [Display(Name = "EmployeeDetail_Committee_IsActive", ResourceType = typeof(Localization.Resources))]
        public bool IsActive { get; set;}
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_VaccancyCode", ResourceType = typeof(Localization.Resources))]
        public string VaccancyCode { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }
        [Required]
        [Display(Name = "EmployeeDetail_Committee_CommitteeType", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public ICollection<CommitteeMember> Members { get; set; }
        public ICollection<CommitteMinute> CommitteMinute { get; set; }

    }
}