using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class MainCompanyStructure : TrackUserOperation 
    {

        [Key]
        public int OrgStructureId { get; set; }
        [Display(Name = "Common_Company_MainCompanyStructure_Name", ResourceType = typeof(Localization.Resources))]
        public string  Name { get; set; }
        [Required]      
        [Display(Name = "Common_Company_MainCompanyStructure_Type", ResourceType = typeof(Localization.Resources))]
        public int StructureId { get; set; }
        public virtual Structure Structure { get; set; }
        [Display(Name = "Common_Company_MainCompanyStructure_Parent", ResourceType = typeof(Localization.Resources))]
        public int Parent { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "Common_PersonEmployee_IsActive", ResourceType = typeof(Localization.Resources))]
        public bool IsActive { get; set; }

       
    }
}