using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetControl : TrackUserOperation
    {
        [Key]
        public int BudgetControlId { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal ReserveAmount { get; set; }
        public decimal Percentage { get; set; }
        public string Remark { get; set; }
    }
}
