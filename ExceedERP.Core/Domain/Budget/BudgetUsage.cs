using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetUsage : Operations
    {
        public int BudgetUsageID { get; set; }
        
        [Display(Name = "Budget Year")]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "Budget Period")]
        public int GLPeriodId { get; set; }
        public virtual GLPeriod Period { get; set; }
        [Display(Name = "Branch or Project")]

        public int BranchId { get; set; }
        [Display(Name = "Business Unit")]

        public int BusinessUnitId { get; set; }
        [Display(Name = "Employee")]
        public int ? EmployeeId { get; set; }
        public string ReasonOfPayment { get; set; }
        [Display(Name = "Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        public decimal Amount { get; set; }
        [Display(Name = "Net Amount")]
        public decimal NetAmount { get; set; }
        public virtual ICollection<BudgetUsageDetail> BudgetUsageDetails { get; set; }
        public virtual ICollection<BudgetUsageApproval> BudgetUsageApprovals { get; set; }

    }

    public class BudgetUsageDetail : TrackUserSettingOperation
    {
      
        public int BudgetUsageDetailId { get; set; }
        public int BudgetUsageID { get; set; }
        public virtual BudgetUsage BudgetUsage { get; set; }

        public int PlanCategoryId { get; set; }

        [Display(Name = "Budget Code")]
        public string BudgetCode { get; set; }
        [Required]
        [Display(Name = "Budget Item")]
        public string BudgetItem { get; set; }

        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }

        [Display(Name = "Description")]
        public string AccountDesc { get; set; }

        [Display(Name = "Usage Amount")]
        public decimal UsageAmount { get; set; }
        [Display(Name = "Cost Center")]
        public string CostCenter { get; set; }
        [Display(Name = "Work Unit")]
        public int? OrgStructureId { get; set; }
        [Display(Name = "Branch / Project")]
        public int? BranchId { get; set; }
        [Display(Name = "Business Unit")]
        public int BusinessUnitId { get; set; }
       
    }
}
