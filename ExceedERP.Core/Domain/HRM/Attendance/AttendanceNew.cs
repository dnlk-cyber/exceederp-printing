using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.Attendance
{
   public class AttendanceNew
    {
        public int AttendanceNewId { get; set; }
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        [Display(Name = "Date")]
        public string DateAm { get; set; }
        [Display(Name = "Morning")]
        public bool IsPresentMorning { get; set; }
        [Display(Name = "Afternoon")]
        public bool IsPresentAfternoon { get; set; }
        public int GlFisicalYearId { get; set; }
        public int GlPeriodId { get; set; }
        [NotMapped]
        [Display(Name = "Department")]
        public int OrgStructureId { get; set; }
        public bool IsDepartmentApproved { get; set; }
        public int DepartmentApproverId { get; set; }
        public bool IsApproved { get; set; }
        public int ApproverEmpId { get; set; }
        [NotMapped]
        public GenerationType IsGenerated { get; set; }
    }

    public enum GenerationType
    {
        New = 0,
        Existing = 1,
        Fail = 2,
        Future = 3
    }
}
