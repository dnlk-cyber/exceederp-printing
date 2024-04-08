using System.Web.Mvc;

namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class EmployeeOutgoingAnswer
    {
        public int EmployeeOutgoingAnswerId { get; set; }
        public int EmployeeOutgoingId { get; set; }
        [AllowHtml]
        public string Answer { get; set; }     
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public bool IsAnswered { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }
}
