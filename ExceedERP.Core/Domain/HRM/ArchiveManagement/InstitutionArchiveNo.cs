using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.ArchiveManagement
{
    public class InstitutionArchiveNo
    {
        [Key]
        public int InstiArchiveNoId { get; set; }
        [Required]
        [Display(Name = "HR_ArchiveManagement_InstitutionArchiveNo_FileNo", ResourceType = typeof(Localization.Resources))]
        public string FileNo { get; set; }
        [Required]
        [Display(Name = "HR_ArchiveManagement_InstitutionType_InstitutionName", ResourceType = typeof(Localization.Resources))]
        public string InstitutionName { get; set; }
       // [Required ]
        [Display(Name = "HR_ArchiveManagement_InstitutionType_InstitutionTypeId", ResourceType = typeof(Localization.Resources))]
        public int? InstitutionTypeId { get; set; }
        [Required]
        [Display(Name = "HR_ArchiveManagement_InstitutionArchiveNo_ZoneId", ResourceType = typeof(Localization.Resources))]
        public int ZoneId { get; set; }
        //public virtual Zone Zone { get; set; }
        [Required]
        [Display(Name = "HR_ArchiveManagement_InstitutionArchiveNo_Shelve", ResourceType = typeof(Localization.Resources))]
        public string Shelve { get; set; }
        [Required]
        [Display(Name = "HR_ArchiveManagement_InstitutionArchiveNo_Row", ResourceType = typeof(Localization.Resources))]
        public string Row { get; set; }
        [Required]
        [Display(Name = "HR_ArchiveManagement_InstitutionArchiveNo_Folder", ResourceType = typeof(Localization.Resources))]
        public string Folder { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "Common_Period_BranchId", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        public string RoomNo { get; set; }
    }
}