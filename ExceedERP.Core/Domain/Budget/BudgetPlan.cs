using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Common.HRM;



namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetPlan : Operations
    {
        public BudgetPlan()
        {
            this.BudgetPlanMonthly = new HashSet<BudgetPlanMonthly>();
        }
        public int BudgetPlanID { get; set; }
        [Required]
        public int BudgetCategoryID { get; set; }
       
        [Display(Name = "Budget Year")]
        public string BudgetYear { get; set; }
         [Display(Name = "Branch / Project")]
        public int BranchId { get; set; }
        [Display(Name = " Work Unit")]
        public int GLChartOfAccountCostCenterId { get; set; }
        [Display(Name = " Work Unit")]
        public string WorkUnitName { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Description")]
        public string AccountDesc { get; set; }
           [Required]
        public int Unit { get; set; }
          [Display(Name = "Current Yearly Budget")]
        public decimal CurrentYearlyBudget { get; set; }
        [Required]
        [Display(Name = "Total Yearly Budget")]
        public decimal TotalYearlyBudget { get; set; }

        [NotMapped]
        [Display(Name = "Progress ")]
        public decimal Progress { get; set; }
        [NotMapped]
        [Display(Name = "Branch / Project")]
        public string BranchName { get; set; }
        [NotMapped]
        [Display(Name = "Budget Status")]
        public string BudgetStatus { get; set; }

        [ForeignKey("Unit")]
        public virtual BudgetCurrency BudgetCurrency { get; set; }
      
         [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; } 
        public virtual ICollection<BudgetPlanMonthly> BudgetPlanMonthly { get; set; }
    }
}
