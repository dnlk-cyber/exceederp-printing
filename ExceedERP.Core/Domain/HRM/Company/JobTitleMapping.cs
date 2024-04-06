using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class JobTitleMapping:TrackUserOperation 
    {
        public int JobTitleMappingId { get; set; }
        [Required ]
        [Display(Name = "HR_Company_JobTitleMapping_OrgStructureID", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        public virtual MainCompanyStructure MainCompanyStructure { get; set; }
        [Required]
        [Display(Name = "HR_Company_JobTitleMapping_WorkUnitId", ResourceType = typeof(Localization.Resources))]
        public int WorkUnitId { get; set; }
        [Display(Name = "HR_Company_JobTitleMapping_JobTitle", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Display(Name = "HR_Company_JobTitleMapping_Quntity", ResourceType = typeof(Localization.Resources))]
        public int Quntity { get; set; }
        [Display(Name = "HR_Company_JobTitleMapping_PlanType", ResourceType = typeof(Localization.Resources))]
        public JobTitlePlan PlanType { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "BussinessUnit", ResourceType = typeof(Localization.Resources))]
        public int? BusinessUnitId { get; set; }
        [Display(Name = "Common_PersonEmployee_IsActive", ResourceType = typeof(Localization.Resources))]
        public bool IsActive { get; set; }
        public virtual ICollection<JobTitleMappingDetail> JobTitleMappingDetails { get; set; }
        [Display(Name = "HR_Company_JobTitleMapping_BranchId", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
    }
}