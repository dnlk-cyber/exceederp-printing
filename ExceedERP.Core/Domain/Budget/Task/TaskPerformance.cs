using System.ComponentModel.DataAnnotations;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Budget.Task
{
    public class TaskPerformance : Operations
    {
        public int TaskPerformanceId { get; set; }

        [Required]
        [Display(Name = " Category ")]
        public int BudgetCategoryID { get; set; }
            [Required]
        [Display(Name = "Fiscal Year")]
        public int GlFiscalYearId { get; set; }
            [Required]
        [Display(Name = "  Period")]
        public int GLPeriodId { get; set; }
            [Required]
        [Display(Name = "Work Unit")]
        public string GLChartOfAccountCostCenterId { get; set; }

        [Required]
        public int Unit { get; set; }
            [Required]
        public decimal Amount { get; set; }
       [Required]
       [Display(Name = "Plan")]
        public int TaskPlanId { get; set; }
         
         
         public virtual GLPeriod GLPeriod { get; set; }
         [ForeignKey("Unit")]
        public virtual TaskUnit TaskUnit { get; set; }
        [ForeignKey("BudgetCategoryID")]
        public virtual TaskCategory TaskCategory { get; set; }
    }
}
