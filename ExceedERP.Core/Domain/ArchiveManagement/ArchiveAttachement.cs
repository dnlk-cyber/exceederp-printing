namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class ArchiveAttachement
    {
        public int ArchiveAttachementId { get; set; }
        public int InstitutionIncomingLetterId { get; set; }
        public ArchiveCategory ArchiveCategory { get; set; }
        public LetterType LetterType { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
    }
}
