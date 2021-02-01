using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Procurements.Print.VM
{
    public class ForeignPurchaseCostVm : Organization
    {

        public string VoucherId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Payee { get; set; }
        public string CostType { get; set; }//reason
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal ForexAmount { get; set; }
        public bool Paid { get; set; }
        public string CreatedBy { get; set; }
        public string CreateDate { get; set; }
        public string Approver { get; set; }
        public string ApprovedDate { get; set; }
    }
}
