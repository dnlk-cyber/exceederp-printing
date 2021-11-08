using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class InternalApplicantScreening
    {
        public int InternalApplicantScreeningId { get; set; }
        public int InternalApplicantScreeningResultId { get; set; }
        [Display(Name ="Employee")]
        public int EmployeeId { get; set; }
        [NotMapped]
        public string Department { get; set; }
        [NotMapped]
        public decimal TotalResult { get; set; }
        [NotMapped]
        public string PassOrFail { get; set; }


    }
}
