using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using static ExceedERP.Core.Domain.Budget.BudgetEnums;

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
        public int PlanCategoryId { get; set; }
       
        [Display(Name = "Budget Year")]
        public int FiscalYearId { get; set; } 
       
        [Display(Name = "Budget Code")]
        public string BudgetCode { get; set; }
        [Required]
        [Display(Name = "Budget Item")]
        public string BudgetItem { get; set; }

        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }

        [Display(Name = "Description")]
        public string AccountDesc { get; set; }

        [Required]
        public int Unit { get; set; }

        [Display(Name = "Current Total")]
        public decimal CurrentYearlyBudget { get; set; }

        [Required]
        [Display(Name = "Initial Total")]
        public decimal TotalYearlyBudget { get; set; }

        [NotMapped]
        [Display(Name = "Progress ")]
        public decimal Progress { get; set; }

        [NotMapped]
        [Display(Name = "Budget Status")]
        public string BudgetStatus { get; set; }

        [ForeignKey("Unit")]
        public virtual FinSetupCurrency BudgetCurrency { get; set; }
        public  string CostCenter { get; set; }
        [Display(Name = "Work Unit")]
        public int ? OrgStructureId { get; set; }
        [Display(Name = "Branch / Project")]
        public int ? BranchId { get; set; }
        [Display(Name = "Business Unit")]
        public int  BusinessUnitId { get; set; }

        public virtual ICollection<BudgetPlanMonthly> BudgetPlanMonthly { get; set; }
        public virtual ICollection<BudgetPlanReserve> BudgetPlanReserves { get; set; }
    }

    public class BudgetPlanReserve : TrackUserSettingOperation
    {
        public int BudgetPlanReserveId { get; set; }
        public BudgetReserveType Type { get; set; }
        public int SourceId { get; set; }
        public decimal Amount { get; set; }
        public EncumbranceStatus ReserveStatus { get; set; }
        public virtual BudgetPlan BudgetPlan { get; set; }
        public int BudgetPlanID { get; set; }
    }
}
