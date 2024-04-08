using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ExceedERP.Core.Domain.ArchiveManagement.Employee
{
    public class Attachment_Answer_LetterSentForEmployee
    {
        public int Attachment_Answer_LetterSentForEmployeeId { get; set; }
        public int EmployeeIncomingAnswerId { get; set; }
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
