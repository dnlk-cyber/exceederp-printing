using ExceedERP.Core.Domain.Common.HRM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
    public class NeedAssessmentEmployee
    {
        [Key]
        public int NeedAssessmentEmployeeId { get; set; }

        [Required]
        public int NeedAssessmentId { get; set; }

        public virtual NeedAssessment NeedAssessment { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public virtual Person_Employee Employee { get; set; }
    }
}
