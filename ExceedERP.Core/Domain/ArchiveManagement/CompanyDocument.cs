namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class CompanyDocument
    {
        public int CompanyDocumentId { get; set; }
        public int CompanyDocumentTypeId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
    }
}
