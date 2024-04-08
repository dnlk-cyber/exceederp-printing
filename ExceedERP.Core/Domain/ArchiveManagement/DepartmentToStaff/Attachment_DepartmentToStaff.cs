namespace ExceedERP.Core.Domain.ArchiveManagement.DepartmentToStaff
{
    public class Attachment_DepartmentToStaff
    {
        public int Attachment_DepartmentToStaffId { get; set; }
        public int OutGoing_DepartmentToStaffId { get; set; }
        public string FilePath { get; set; }
        public string ShortPath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }
}
