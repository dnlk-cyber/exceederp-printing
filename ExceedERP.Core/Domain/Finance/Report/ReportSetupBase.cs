using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.Report
{
    public enum IfrsStatementOfChangesInEquityTypes
    {
        PaidInCapital,
        LegalReserve,
        RevaluationReserve,
        OtherReserve,
        RetainedEarnings
    }
    public enum IfrsContinuedOpRevenueExpense
    {
        Revenue1,
        Revenue2,
        Revenue3,
        Expense1,
        Expense2,
        Expense3
    }

    public enum IfrsSofpAssetTypes
    {
        CurrentAsset,
        NonCurrentAsset
    }

    public enum IfrsSofpEquityAndLiablityTypes
    {
        CapitalAndReserves,
        CurrentLiability,
        NonCurrentLiability
    }


    public enum IfrsCashFlowTypes
    {
        ProfitBeforeTax,
        Adjustment,
        Increase_Decrease,
        Net
    }

    public enum IfrsCashFlowRule
    {
        ADD,
        DEDUCT,
        ADD_INCREASE,
        DEDUCT_INCREASE,
        ADD_DECREASE,
        DEDUCT_DECREASE,
    }

    //gaap

    public enum GaapReportType
    {
        GAAPOne,
        GAAPTwo
    }

    public enum CashFlowCategory
    {
        CashReceipt,
        CashPaidOut
    }

    public enum IncomeStatementRevenueCategory
    {
        OperationalRevenue,
        NonOperationalRevenue,
        OperatingIncome,
        OtherIncome
    }
    public enum IncomeStatementExpenseCategory
    {
        OperationalExpense,
        NonOperationalExpense,
        SellingAndDistributionExpense,
        CostOfGoodSold,
        AdministrationAndGeneralExpense,
    }

    public enum BalanceSheetCategory
    {
        Asset,
        Liability,
        Capital
    }


    public enum GaapAssetTypes
    {
        CurrentAsset,
        NonCurrentAsset,
        FixedAsset,
        FixedAssetAccumulated,
        OtherAsset
    }

    public enum GaapLiablityTypes
    {
        CurrentLiability,
        NonCurrentLiability,
        LongTermLiability
    }

    public enum GaapEquityTypes
    {
        CapitalAndReserves
    }


    public enum GaapCashFlowTypes
    {
        Revenue,
        Expense
    }
    public class ReportSetupBase: TrackUserSettingOperation
    {
        [Required]
        [Display(Name = "Account From")]
        public string AccountFrom { get; set; }
        [Required]
        [Display(Name = "Account To")]
        public string AccountTo { get; set; }
    }
}
