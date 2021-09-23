using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetRequestTransferLog : Operations
    {


        public int BudgetRequestTransferLogId { get; set; }

     

   
        [Display(Name = "Budget Year")]
        public string BudgetYear { get; set; }
        [Required]
        [Display(Name = "Budget Category")]
        public string PlanCategoryId { get; set; }
             [Display(Name = " Branch / Project ")]
        public int BranchId { get; set; }

        public string WorkUnit { get; set; }

        [Display(Name = " From Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime FromDate { get; set; }


        [Display(Name = " To Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ToDate { get; set; }

        public decimal Amount { get; set; }
             [Display(Name = " Remark ")]
        public string Reason { get; set; }


             public int RequestNo { get; set; }
 
        [ForeignKey("RequestNo")]
        public virtual BudgetRequest BudgetRequest { get; set; }

    
    }
}
