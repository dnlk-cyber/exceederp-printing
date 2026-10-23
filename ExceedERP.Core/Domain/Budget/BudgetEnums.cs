namespace ExceedERP.Core.Domain.Budget
{
    public  class BudgetEnums
    {

       public enum BudgetQuarter
        {
            First,
            Second,
            Third,
            Fourth
            
        }

        public enum BudgetReserveType
        {
            PO,
            PaymentRequest,
            TravelOrder,
            HealthPayment,
            TravelClaim,

        }
        public enum EncumbranceStatus
        {
            None = 0,
            PreEncumbered = 1,
            Encumbered = 2,
            Released = 3,
            Liquidated = 4    // actual expense posted and encumbrance cleared
        }

        public enum FundsCheckStatus
        {
            Pending = 0,      // not checked yet
            Passed = 1,       // funds available, allocation ok
            Warning = 2,      // funds insufficient but within tolerance
            Failed = 3        // funds not available and blocked
        }
    }
}
