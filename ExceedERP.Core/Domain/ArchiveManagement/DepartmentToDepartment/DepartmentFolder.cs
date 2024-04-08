using System.ComponentModel;

namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class DepartmentFolder
    {
        public int DepartmentFolderId { get; set; }
        public int Owner_OrgStructureId { get; set; }
        /// if we choose the folder structured by Department in the selection setting, we use this field , we don't use Department and Subject type at the same time        [DisplayName("Department")]
        [DisplayName("Department")]
        public int? OrgStructureId { get; set; }
        /// if we choose the folder structured by Subject Type in the selection setting, we use this field , we don't use Department and Subject type at the same time
        [DisplayName("Subject Type")]
        public int? Department_SubjectTypeId { get; set; }
        public string Room { get; set; }
        public string Shelve { get; set; }
        public string Row { get; set; }
        public string Folder { get; set; }
        public string Volume { get; set; }
        public string FileNo { get; set; }
    }
}
