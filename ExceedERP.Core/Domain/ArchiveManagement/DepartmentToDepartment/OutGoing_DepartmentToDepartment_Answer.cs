using System.ComponentModel;
using System.Web.Mvc;

namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class OutGoing_DepartmentToDepartment_Answer
    {
        public int OutGoing_DepartmentToDepartment_AnswerId { get; set; }
        public int OutGoing_DepartmentToDepartmentId { get; set; }
        //  i created this field to pass master Grid id to popup through Default value
        //[NotMapped]
        //public string MasterId { get; set; }
        [AllowHtml]
        public string Answer { get; set; }
        [DisplayName("Department")]
        public int OrgStructureId { get; set; }     
        public bool IsAnswered { get; set; }
        public string DateAm { get; set; }
        public string Time { get; set; }
    }
}
