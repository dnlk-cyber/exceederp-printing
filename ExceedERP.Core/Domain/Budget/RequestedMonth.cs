using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Budget
{
    public class RequestedMonth : Operations
    {

        public int RequestedMonthID { get; set; }
        public int BudgetRequestID { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}
