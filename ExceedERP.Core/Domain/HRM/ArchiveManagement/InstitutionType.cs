using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.ArchiveManagement
{
    public class InstitutionType
    {
        [Key]
        public int InstitutionTypeId { get; set; }
        [Required ]
        [Display(Name = "HR_ArchiveManagement_InstitutionType_InstitutionName", ResourceType = typeof(Localization.Resources))]
        public string InstitutionName { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}