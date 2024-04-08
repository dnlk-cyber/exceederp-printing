using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace ExceedERP.Core.Domain.ArchiveManagement.DepartmentToStaff
{
    public class Answer_DepartmentToStaff
    {
        public int Answer_DepartmentToStaffId { get; set; }
        public int OutGoing_DepartmentToStaffId { get; set; }
        [NotMapped]
        public string MasterId { get; set; }
        [AllowHtml]
        public string Answer { get; set; }
        [DisplayName("Employee")]
        public int EmployeeId { get; set; }
        public bool IsAnswered { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        public string DateEt { get; set; }
        public string Time { get; set; }
    }
}
