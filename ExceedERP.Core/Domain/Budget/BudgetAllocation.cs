 using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetAllocation : Operations
    {
        public int BudgetAllocationID { get; set; }
        public int Branch { get; set; }
        [Display(Name = "Budget Group")]
        public string BudgetGroup { get; set; }

        [Display(Name = "Budget Year")]
        public string BudgetYear { get; set; }
        [Required]
        [Display(Name = "Plan Category")]
        public int PlanCategoryId { get; set; }
        [Display(Name = "Account Code ")]
        public string AccountCode { get; set; }

        [Display(Name = "Work Unit")]
        public string WorkUnit { get; set; }

    
        public string Month { get; set; }

      
        public decimal Amount { get; set; }

    }
}
