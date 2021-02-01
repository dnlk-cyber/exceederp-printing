using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Budget.GAAP
{

    public class BudgetSheetCategory : TrackUserOperation
    {
        [Key]
        public int BalanceSheetCategoryId { get; set; }
        public Enums.BalanceSheetType BalanceSheetType { get; set; }
             [Required]
        public string Name { get; set; }
        public string Remark { get; set; }
    }
}
