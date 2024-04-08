using System.Web.Mvc;

namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class EmployeeIncomingAnswer
    {
        public int EmployeeIncomingAnswerId { get; set; }
        public int EmployeeIncomingLetterId { get; set; }
       // public int EmployeeLetterSenderId { get; set; }
        public int OrgStructureId { get; set; }
        [AllowHtml]
        public string Answer { get; set; }     
        public bool IsAnswered { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }
}
