using System.ComponentModel.DataAnnotations;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetUsage : Operations
    {
        public int BudgetUsageID { get; set; }
        
        [Display(Name = "Budget Year")]
        public string BudgetYear { get; set; }
    
        [Display(Name = "Plan Category")]
        public int PlanCategoryId { get; set; }
        [Required]
        [Display(Name = "Work Unit")]
        public string WorkUnit { get; set; }
        [Required]
        [Display(Name = "Budget Month")]
        public string BudgetMonth { get; set; }
              [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Description")]
        public string AccountDesc { get; set; }
        [Display(Name = "Account Type")]
        public GlAccountTypes AccountType { get; set; }
        [Required]
        public string Unit { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Monthly Actuals")]
        public decimal MonthlyActuals { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
