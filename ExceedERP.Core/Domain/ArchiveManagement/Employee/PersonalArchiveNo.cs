using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.Archive
{
    public class PersonalArchiveNo
    {
        [Key]
        public int PersonalArchiveNoId { get; set; }
        [Display(Name = "HR_ArchiveManagement_PersonalArchiveNo_Folder", ResourceType = typeof(Localization.Resources))]
        public string Folder { get; set; }       
        [Display(Name = "HR_ArchiveManagement_PersonalArchiveNo_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [NotMapped ]
        [Display(Name = "Common_PersonEmployee_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Display(Name = "HR_ArchiveManagement_PersonalArchiveNo_ZoneId", ResourceType = typeof(Localization.Resources))]
        public int ZoneId { get; set; }
        [Display(Name = "HR_ArchiveManagement_PersonalArchiveNo_Shelve", ResourceType = typeof(Localization.Resources))]
        public string Shelve { get; set; }
        [Display(Name = "HR_ArchiveManagement_PersonalArchiveNo_Row", ResourceType = typeof(Localization.Resources))]
        public string Row { get; set; }
        [Display(Name = "HR_ArchiveManagement_PersonalArchiveNo_FileNo", ResourceType = typeof(Localization.Resources))]
        public string FileNo { get; set; }      
        [Display(Name = "Common_Period_BranchId", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        public string RoomNo { get; set; }
        public string Volume { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}