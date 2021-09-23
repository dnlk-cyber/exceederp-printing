using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetTransfer : Operations
    {
        public int BudgetTransferID { get; set; }
        [Display(Name = "From Branch")]
        public string FromBranch { get; set; }
        [Display(Name = "To Branch")]
        public string ToBranch { get; set; }
       
        public string BudgetGroup { get; set; }
    
        [Display(Name = "To Budget Year")]
        public  string BudgetYear { get; set; }
        [Required]
        [Display(Name = "Plan Category")]
        public int PlanCategoryId { get; set; }
        [Required]
        [Display(Name = "From Account Code")]
        public string FromAccountCode { get; set; }


        [Required]
        [Display(Name = " To Account Code")]
        public string ToAccountCode { get; set; }


          [Display(Name = "From Work Unit")]
        public string FromWorkUnit { get; set; }

         [Display(Name = "To Work Unit")]
        public string ToWorkUnit { get; set; }

       
        [Display(Name = "Month")]
        public string Month { get; set; }



        [Display(Name = "Description")]
        public string AccountDesc { get; set; }
        [Required]
        public string Unit { get; set; }
        [Required]
        [Display(Name = "Amount Transferred")]
        public decimal Transferred { get; set; }
  

        public virtual ICollection<BudgetPlanMonthly> BudgetMonthly { get; set; }
    }
}
