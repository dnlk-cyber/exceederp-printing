using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetPlanMonthly : Operations
    {
        public int BudgetPlanMonthlyID { get; set; }
        public int BudgetDefineID { get; set; }
      
        [Display(Name = "Budget Month")]
        public int GLPeriodId { get; set; }
      
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Description")]
        public string AccountDesc { get; set; }
        public string Unit { get; set; }
        [Display(Name = "Monthly Budget")]
        public decimal MonthlyBudget { get; set; }
        [Display(Name = "Transfered Budget")]
        public decimal TransferedBudget { get; set; }
        [Display(Name = "Revised Budget")]
        public decimal RevisedBudget { get; set; }
        [Display(Name = "Allocated Budget")]
        public decimal AllocatedBudget { get; set; }
        [Display(Name = "Current Budget")]
        public decimal CurrentBudget { get; set; }

        [NotMapped]
        [Display(Name = "Progress ")]
        public decimal Progress { get; set; }

        [NotMapped]
        [Display(Name = "Budget Status")]
        public string BudgetStatus { get; set; }
        [ForeignKey("BudgetDefineID")]
        public virtual BudgetPlan BudgetPlan { get; set; }
        public virtual GLPeriod GLPeriod { get; set; }
     
     
      
    }
}
