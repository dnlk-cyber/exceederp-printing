namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class AttachedLettersForStaff
    {
        public int AttachedLettersForStaffId { get; set; }
        public int LetterForMultipleStaffId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
    }
}
