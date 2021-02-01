using System.Collections.ObjectModel;

namespace ExceedERP.Reporting.Finance.ViewModel.GAAP
{
   public class CashFlowProjectionViewModel
    {
        public string Month1 { get; set; }
        public string Month2 { get; set; }
        public string Month3 { get; set; }
        public string Month4 { get; set; }
        public string Month5 { get; set; }
        public string Month6 { get; set; }
        public string Month7 { get; set; }
        public string Month8 { get; set; }
        public string Month9 { get; set; }
        public string Month10 { get; set; }
        public string Month11 { get; set; }
        public string Month12 { get; set; }
        public Collection<CashFlowProjectionLineVm> BegEndLines { get; set; }
        public Collection<CashFlowProjectionLineVm> Receipts { get; set; }
        public Collection<CashFlowProjectionLineVm> PayOuts { get; set; }
        public Collection<CashFlowProjectionLineVm> TotalCashAvailable { get; set; }
    }

    public class CashFlowProjectionLineVm
    {
        public string Description { get; set; }
        public decimal Beginning { get; set; }
        public decimal MonthOneValue { get; set; }
        public decimal MonthTwoValue { get; set; }
        public decimal MonthThreeValue { get; set; }
        public decimal MonthFourValue { get; set; }
        public decimal MonthFiveValue { get; set; }
        public decimal MonthSixValue { get; set; }
        public decimal MonthSevenValue { get; set; }
        public decimal MonthEightValue { get; set; }
        public decimal MonthNineValue { get; set; }
        public decimal MonthTenValue { get; set; }
        public decimal MonthElevenValue { get; set; }
        public decimal MonthTwelveValue { get; set; }

        //
        public decimal MonthOneEndValue { get; set; }
        public decimal MonthTwoEndValue { get; set; }
        public decimal MonthThreeEndValue { get; set; }
        public decimal MonthFourEndValue { get; set; }
        public decimal MonthFiveEndValue { get; set; }
        public decimal MonthSixEndValue { get; set; }
        public decimal MonthSevenEndValue { get; set; }
        public decimal MonthEightEndValue { get; set; }
        public decimal MonthNineEndValue { get; set; }
        public decimal MonthTenEndValue { get; set; }
        public decimal MonthElevenEndValue { get; set; }
        public decimal MonthTwelveEndValue { get; set; }
    }
}
