namespace ExceedERP.Core.Domain.ArchiveManagement.Setting
{
    public class DepartmentOrSubjectTypeSelectionForFolderCreation
    {
        public int DepartmentOrSubjectTypeSelectionForFolderCreationId { get; set; }
        public int Owner_OrgStructureId { get; set; }
        public DepartmentOrSubjectType DepartmentOrSubjectType { get; set; } 
        public string Remark { get; set; }
    }
}
