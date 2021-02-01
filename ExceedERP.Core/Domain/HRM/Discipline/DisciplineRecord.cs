using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Discipline
{
    public class DisciplineRecord
    {
        [Key]
        public int RecordId { get; set; }
        //[Required ]
        //[Display (Name ="Employee")]
        //public int EmployeeId { get; set; }
        [Required ]
        [Display(Name = "Measures")]
        public int MeasuresId { get; set; }
        [Required]
        [Display(Name = "Penalty")]
        public int DetailMeasuresId { get; set; }
        public virtual DisciplineMeasuresDetail DisciplineMe { get; set; }
        [Required ]
        public DateTime Date { get; set; }
        public string Remark { get; set; }


    }
}