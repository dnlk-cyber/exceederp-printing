using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Budget
{
    public enum BudgetControlPeriod
    {
        Yearly,
        Monthly
    }
    public class BudgetControl : TrackUserOperation
    {
        [Key]
        public int BudgetControlId { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal ReserveAmount { get; set; }
        public decimal Percentage { get; set; }
        public BudgetControlPeriod ControlPeriod { get; set; }
        public string Remark { get; set; }
    }    

}
