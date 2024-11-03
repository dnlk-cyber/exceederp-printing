using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class InternalCustomerPerformanceEvaluation
    {
        [Key]
        public int InternalCustomerPerformanceEvaluationId { get; set; }
        [Display(Name ="Number Of Evaluator's needed")]
        public int NumberOfEmployee { get; set; }
        [Display(Name="Work Unit")]
        public int OrgStructureId { get; set; }
        [Display(Name = "Is Confirmed")]
        public bool IsConfirmed { get; set; }
        public int PerformanceRequestPeriodId { get; set; }
        //InternalCustomer 
        //public virtual PerformanceRequestPeriod PerformanceRequest { get; set; }




    }
    public  class InternalCustomer
    {
        [Key]
        public int InternalCustomerId { get; set; }
        [Required]
        [Display(Name = "Evaluator's Name ")]
        public int EmployeeId { get; set; }
        [Display(Name = "To be Evaluted Name ")]
        public int? TobeEvalutedEmployeeId { get; set; }
        [Display(Name = "Is Confirmed")]
        public bool IsConfirmed { get; set; }
        public int InternalCustomerPerformanceEvaluationId { get; set; }
        public virtual InternalCustomerPerformanceEvaluation InternalCustomerEvaluation { get; set; }
    }
    public  class InternalCustomerDetail
    {
        public int InternalCustomerDetailId { get; set; }
        [Display(Name = "To be Evaluted Name ")]
        public int TobeEvalutedEmployeeId { get; set; }
        public int InternalCustomerId { get; set; }
        public InternalCustomer customer { get; set; }
    }
}
