using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetCategory : TrackUserSettingOperation
    {


        public int BudgetCategoryID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [Display(Name = "From Account Code")]
        public string FromAccountCode { get; set; }
        [Required]
        [Display(Name = "To Account Code")]
        public string ToAccountCode { get; set; }
        [Display(Name = "Budget Control")]
        public int BudgetControlId { get; set; }
       

        [ForeignKey("BudgetControlId")]
        public virtual BudgetControl BudgetControl { get; set; }
    }
}
