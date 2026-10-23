using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetRequestDetail : Operations
    {
        public int BudgetRequestDetailId { get; set; }
        
        public string AccountCode  { get; set; }
        public string Description { get; set; }

        public int BudgetRequestId { get; set; }
        [ForeignKey("BudgetRequestId")]
        public BudgetRequest BudgetRequest { get; set; }

    }
}
