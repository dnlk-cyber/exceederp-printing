using System.ComponentModel;
using System.Web.Mvc;

namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class InstitutionIncomingAnswer
    {
        public int InstitutionIncomingAnswerId { get; set; }
        public int InstitutionIncomingId { get; set; }
        public int LetterSenderId { get; set; }
        [DisplayName("Department")]
        public int OrgStructureId { get; set; }
        [AllowHtml]
        public string Answer { get; set; }     
        public bool IsAnswered { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }
}
