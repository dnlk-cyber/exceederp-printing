using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class InternalCustomerEmployeePerformance
    {
        public int InternalCustomerEmployeePerformanceId { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public int GlFiscalYearId { get; set; }
        [Required]
        public int GlPeriodId { get; set; }
        [Required]
        public int EvaluationCriteriaTypeId { get; set; }
        [Display(Name = "Weight")]
        public decimal Weight { get; set; }
        [Display(Name = "Result")]
        public decimal Result { get; set; }
        [NotMapped]
        public string Grade { get; set; }
        public bool IsApproved { get; set; }
        public string Remark { get; set; }
    }
}
