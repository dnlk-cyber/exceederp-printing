using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Budget
{
    public class PlanCategory : Operations
    {
     
        public int PlanCategoryID { get; set; }

        public string Name { get; set; }

        [Display(Name = "Budget Year")]
        [Required]
        public int GlFiscalYearId { get; set; }

        
         [Display(Name = "Initial Total")]
        public decimal GrandTotal { get; set; }
        
        [Display(Name = "Current Total")]
        public decimal CurrentTotal { get; set; }
     
        [Display(Name = "Progress % ")]
        public decimal Progress { get; set; }
        
        [Display(Name = "Actual GL  ")]
        public decimal ActualConsumption { get; set; }
 
        [Display(Name = "Budget Status")]
        public string BudgetStatus { get; set; }
 
        [Display(Name = "Budget Type")]
        public string BudgetType { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; } 
    }
}
