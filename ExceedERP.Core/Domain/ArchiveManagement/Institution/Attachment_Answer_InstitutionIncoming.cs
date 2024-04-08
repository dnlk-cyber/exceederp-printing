using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ExceedERP.Core.Domain.ArchiveManagement.Institution
{
    public class Attachment_Answer_InstitutionIncoming
    {
        public int Attachment_Answer_InstitutionIncomingId { get; set; }
        public int InstitutionIncomingAnswerId { get; set; }
        [NotMapped]
        public string MasterId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        [NotMapped]
        public HttpPostedFileBase FileAnswer { get; set; }
    }
}
