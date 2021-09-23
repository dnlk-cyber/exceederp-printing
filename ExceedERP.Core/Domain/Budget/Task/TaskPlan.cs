using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;



namespace ExceedERP.Core.Domain.Budget.Task
{
    public class TaskPlan : Operations
    {
        public int TaskPlanId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = " Category ")]
        public int BudgetCategoryID { get; set; }
       
        [Display(Name = "Fiscal Year")]
        public int GlFiscalYearId { get; set; }

        [Display(Name = " Work Unit")]
        public string GLChartOfAccountCostCenterId { get; set; }

           [Required]
        public int Unit { get; set; }
          
        public decimal Amount { get; set; }
        //[Required]
        //[Display(Name = "Total Yearly Budget")]
        //public decimal TotalYearlyBudget { get; set; }


        [NotMapped]
        [Display(Name = "Progress ")]
        public decimal Progress { get; set; }

        //[NotMapped]
        //[Display(Name = "Budget Status")]
        //public string BudgetStatus { get; set; }
         
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [ForeignKey("Unit")]
        public virtual TaskUnit TaskUnit { get; set; }
        [ForeignKey("BudgetCategoryID")]
        public virtual TaskCategory TaskCategory { get; set; }
          }
}
