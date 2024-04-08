using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Archive
{
    public class InstitutionArchiveNo
    {
        [Key]
        public int InstiArchiveNoId { get; set; }
        [Display(Name = "HR_ArchiveManagement_InstitutionArchiveNo_FileNo", ResourceType = typeof(Localization.Resources))]
        public string FileNo { get; set; }
        [Display(Name = "HR_ArchiveManagement_InstitutionType_InstitutionTypeId", ResourceType = typeof(Localization.Resources))]
        public int? InstitutionTypeId { get; set; }
        [Display(Name = "HR_ArchiveManagement_InstitutionArchiveNo_ZoneId", ResourceType = typeof(Localization.Resources))]
        public int ZoneId { get; set; }
        [Display(Name = "HR_ArchiveManagement_InstitutionArchiveNo_Shelve", ResourceType = typeof(Localization.Resources))]
        public string Shelve { get; set; }
        [Display(Name = "HR_ArchiveManagement_InstitutionArchiveNo_Row", ResourceType = typeof(Localization.Resources))]
        public string Row { get; set; }
        [Display(Name = "HR_ArchiveManagement_InstitutionArchiveNo_Folder", ResourceType = typeof(Localization.Resources))]
        public string Folder { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "Common_Period_BranchId", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        public string RoomNo { get; set; }
        public string Volume { get; set; }
        [DisplayName("Subject Type")]
        public int SubjectTypeId { get; set; }
    }
}