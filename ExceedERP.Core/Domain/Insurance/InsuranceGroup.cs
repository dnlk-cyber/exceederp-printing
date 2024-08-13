using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.Insurance
{
    public class InsuranceGroup
    {
        [Key]
        public int InsuranceGroupId { get; set; }
        [Display(Name = "HR_HealthAndInsurance_InsuranceGroup_GroupName", ResourceType = typeof(Localization.Resources))]
        public int InsuranceCategoryId { get; set; }
       
        //[Required ]
        [Display(Name = "HR_HealthAndInsurance_InsuranceGroup_GroupName", ResourceType = typeof(Localization.Resources))]
        public string GroupName { get; set; }
        [Required]
        [Display(Name = "HR_HealthAndInsurance_InsuranceGroup_OrganizationId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        public virtual MainCompanyStructure MainCompanyStructure { get; set; }
        [Display(Name = "HR_HealthAndInsurance_InsuranceGroup_JobTitle", ResourceType = typeof(Localization.Resources))]
        [Required ]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        ////[NotMapped ]
        [Display(Name = "HR_HealthAndInsurance_InsuranceGroup_JobTitle", ResourceType = typeof(Localization.Resources))]
        public string JobTitleName { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [NotMapped]
        public string Name { get; set; }



    }
}