using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Budget.Task
{
    public class TaskPlanMonthly : Operations
    {
        public int TaskPlanMonthlyId { get; set; }
        public int BudgetDefineID { get; set; }
        //[Required]
        [Display(Name = " Month ")]
        public int GLPeriodId { get; set; }
      
        
        [Display(Name = " Amount ")]
        public decimal MonthlyBudget { get; set; }
       

        [NotMapped]
        [Display(Name = "Progress ")]
        public decimal Progress { get; set; }

        [NotMapped]
        [Display(Name = "Budget Status")]
        public string BudgetStatus { get; set; }

        public virtual GLPeriod GLPeriod { get; set; }
     
     
      
    }
}
