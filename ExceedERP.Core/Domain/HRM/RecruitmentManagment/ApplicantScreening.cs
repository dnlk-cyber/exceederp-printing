using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class ApplicantScreening
    {
        public int ApplicantScreeningId { get; set; }
        public int ApplicantDetailId { get; set; }
        public int ApplicantScreenResultId { get; set; }
        public Gender Sex { get; set; }
        public int Age { get; set; }
        public string ApplicantName { get; set; }
        public string Address { get; set; }
        [NotMapped]
        public decimal TotalResult { get; set; }
        [NotMapped]
        public string PassOrFail { get; set; } 
     
        public bool IsProfileGenerated { get; set; }
    }
}
