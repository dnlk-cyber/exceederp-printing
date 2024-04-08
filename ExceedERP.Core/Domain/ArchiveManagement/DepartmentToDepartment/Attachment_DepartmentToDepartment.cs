namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class Attachment_DepartmentToDepartment
    {
        public int Attachment_DepartmentToDepartmentId { get; set; }
        public int OutGoing_DepartmentToDepartmentId { get; set; }
        public string FilePath { get; set; }
        public string ShortPath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }
}
