using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class PeerToPeerPerformanceEvaluation
    {
        [Key]
        public int PeerToPeerPerformanceEvaluationId { get; set; }
        [Required]
        [Display(Name = "Evaluator's Name ")]
        public int EvaluatorId { get; set; }
        [Display(Name = "To Be Evaluated")]
        [Required]
        public int EmployeeId { get; set; }
        public int PerformanceRequestPeriodId { get; set; }
        public virtual PerformanceRequestPeriod PerformanceRequest { get;set;}

    }
}
