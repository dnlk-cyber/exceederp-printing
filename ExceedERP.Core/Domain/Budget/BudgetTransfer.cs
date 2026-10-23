using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetTransfer : Operations
    {
        public int BudgetTransferID { get; set; }
        [Display(Name = "Budget Year")]
        [Required]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "Category")]
        public int PlanCategoryId { get; set; }
        //from
        [Display(Name = "From Cost Center")]
        public string FromCostCenter { get; set; }
        [Display(Name = "From Work Unit")]
        public int? FromOrgStructureId { get; set; }
        [Display(Name = "From Branch / Project")]
        public int FromBranchId { get; set; }
        [Display(Name = "From Business Unit")]
        public int? FromBusinessUnitId { get; set; }
        [Display(Name = "From Period")]
        public int FromGlPeriodId { get; set; }
        [Required]
        [Display(Name = "From Account Code")]
        public string FromAccountCode { get; set; }

        //to
        [Display(Name = "To Cost Center")]
        public string ToCostCenter { get; set; }
        [Display(Name = "From Work Unit")]
        public int? ToOrgStructureId { get; set; }
        [Display(Name = "To Branch / Project")]
        public int ToBranchId { get; set; }
        [Display(Name = "To Business Unit")]
        public int? ToBusinessUnitId { get; set; }
        [Required]
        [Display(Name = "To Account Code")]
        public string ToAccountCode { get; set; }       
        [Display(Name = "To Period")]
        public int ? ToGlPeriodId { get; set; }
        [Display(Name = "Equal to All Periods")]
        public bool  EqualToAllPeriods { get; set; }

        [Display(Name = "Amount Transferred")]
        public decimal Transferred { get; set; }

       
    }
}
