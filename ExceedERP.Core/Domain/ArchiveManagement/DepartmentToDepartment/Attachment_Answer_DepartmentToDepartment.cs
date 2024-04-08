using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ExceedERP.Core.Domain.ArchiveManagement.DepartmentToDepartment
{
    public class Attachment_Answer_DepartmentToDepartment
    {
        public int Attachment_Answer_DepartmentToDepartmentId { get; set; }
        public int OutGoing_DepartmentToDepartment_AnswerId { get; set; }
        public string MasterId { get; set; }
        [NotMapped]
        public HttpPostedFileBase AnswerFile { get; set; }
        public string FilePath { get; set; }
        public string ShortPath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }
}
