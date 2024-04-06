namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class EmployeeFolderAttachment
    {
        public int EmployeeFolderAttachmentId { get; set; }
        public int PersonalArchiveNoId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }
}
