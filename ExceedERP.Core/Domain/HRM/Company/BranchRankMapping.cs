using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SalaryStructure;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class BranchRankMapping : TrackUserOperation
    {
        [Key]
        public int BranchRankMappingId { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_Rank", ResourceType = typeof(Localization.Resources))]
        public int RankId { get; set; }
        public virtual Rank Rank{ get; set; }
        [Required]
        [Display(Name = "HR_Company_JobTitleMapping_BranchId", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        [Display(Name = "HR_SalaryStructure_Rank_MaximumSalary", ResourceType = typeof(Localization.Resources))]

        public decimal MaximumSalary { get; set; }
        [Display(Name = "Common_PersonEmployee_IsActive", ResourceType = typeof(Localization.Resources))]
        public bool IsActive { get; set; }

    }
}