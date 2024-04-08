using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ExceedERP.Core.Domain.ArchiveManagement.DepartmentToStaff
{
    public class Attachment_Answer_DepartmentToStaff
    {
        public int Attachment_Answer_DepartmentToStaffId { get; set; }
        public int Answer_DepartmentToStaffId { get; set; }
        [NotMapped]
        public string MasterId { get; set; }
        [NotMapped]
        public HttpPostedFileBase AnswerFile { get; set; }
        public string FilePath { get; set; }
        public string ShortPath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }
}
