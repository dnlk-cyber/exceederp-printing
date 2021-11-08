using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
   public class EmployeesInAssessment
    {
        public int EmployeesInAssessmentId { get; set; }       
        public int AssesmentId { get; set; }
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [Display(Name = "Jobtitle")]
        public int JobtitleId { get; set; }
    }
}
