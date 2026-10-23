using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetCategory : TrackUserSettingOperation
    {
        public int BudgetCategoryID { get; set; }
        [Required]
        public string BudgetCategoryCode { get; set; } //Added as requested
        [Required]
        public string Name { get; set; }
        [Display(Name = "Budget Type")]
        public string BudgetType { get; set; }
        [Required]
        [Display(Name = "From Account Code")]
        public string FromAccountCode { get; set; }
        [Required]
        [Display(Name = "To Account Code")]
        public string ToAccountCode { get; set; }
        [Display(Name = "Budget Control")]
        public int BudgetControlId { get; set; }
        //public string BudgetCategoryDescription { get; set; } // Added per request
        [ForeignKey("BudgetControlId")]
        public virtual BudgetControl BudgetControl { get; set; }
        public ICollection<BudgetCategoryDetail> budgetCategoryDetails { get; set; } //added 
    }
}
