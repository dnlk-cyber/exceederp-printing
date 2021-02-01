

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class SocieVeiwModel
    {
        public string CategoryDescription { get; set; }
        public string Notes { get; set; }
        public decimal PaidInCapital { get; set; }
        public decimal LegalReserve { get; set; }
        public decimal RevaluationReserve { get; set; }
        public decimal OtherReserve { get; set; }
        public decimal RetainedEarning { get; set; }
        public decimal Total { get; set; }
        public bool IsHeader { get; set; }
    }
}
