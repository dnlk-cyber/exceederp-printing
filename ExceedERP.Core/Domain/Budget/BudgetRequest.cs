using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetRequest : OperationWithSecondValidation
    {

        public BudgetRequest()
        {
            this.BudgetRequestLine = new HashSet<BudgetRequestLine>();
        }
        public int BudgetRequestID { get; set; }


        public int FiscalYearId { get; set; }
        [Display(Name = "Budget Group")]
        public string BudgetGroup { get; set; }

        [Display(Name = "Budget Year")]
        public string BudgetYear { get; set; }
        [Required]
        [Display(Name = "Category")]
        public string PlanCategoryId { get; set; }
             [Display(Name = "Branch / Project")]
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

           

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        public virtual ICollection<BudgetRequestLine> BudgetRequestLine { get; set; }
    }
}
