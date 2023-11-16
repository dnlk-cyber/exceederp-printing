using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.Finance
{
    public enum FinanceSettingCategories
    {
        ENABLE_PR_BUDGET_CONTROL,
        ENABLE_PAYMENT_BUDGET_CONTROL,
        ENABLE_TELEBIRR_PAYMENT_METHOD,
        DISABLE_GRN_DISTRIBUTION,
        APPLY_CLEARING_ACCOUNT_GRN_DISTRIBUTION,
        ENABLE_PAYMENT_REQUEST_AUTHORIZATION,

    }


    public enum ApInvoiceTransactionType
    {
        Invoice,
        Prepayment
    }
    public enum InvoiceStatus
    {
        New,
        Open,
        Close
    }


    public class GlCostAllocationSetup : TrackUserOperation
    {
        public int GlCostAllocationSetupId { get; set; }
        [Display(Name = "Segment Type")]
        public int GLSegmentSetupId { get; set; }
        [Display(Name = "Segment Code")]
        public string SegmentCode { get; set; }
        [Display(Name = "Cost Center Code")]
        public string CostCenterCode { get; set; }
    }



    public class GlRetainedEarningAccount : TrackUserOperation
    {
        public int GlRetainedEarningAccountId { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Description")]
        public string AccountDesc { get; set; }
        [Display(Name = "Account Type")]
        public EarningTypes Type { get; set; }

    }

    public class EmployeeSubAccount : TrackUserOperation
    {
        public int EmployeeSubAccountId { get; set; }
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [Required]
        [Display(Name = "Sub Account Code")]
        public string SubAccountCode { get; set; }
        public bool Enable { get; set; }
    }

    public enum SubAccountType
    {
        Employee,
        Supplier,
        Customer,
        Branch
    }

    public class SubAccountRange : TrackUserOperation
    {
        public int SubAccountRangeId { get; set; }
        [Required]
        [Display(Name = "From")]
        public string From { get; set; }
        [Required]
        [Display(Name = "To")]
        public string To { get; set; }
        [Display(Name = "Sub Account Type")]
        public SubAccountType Type { get; set; }
    }

    //public class FinSetupCashFlowConfiguration : TrackUserSettingOperation
    //{
    //    public int FinSetupCashFlowConfigurationID { get; set; }
    //    [Required]
    //    public CashFlowCategory Category { get; set; }
    //    [Required]
    //    [Display(Name = "Name")]
    //    public string Name { get; set; }
    //    [Required]
    //    [Display(Name = "Account From")]
    //    public string AccountFrom { get; set; }
    //    [Required]
    //    [Display(Name = "Account To")]
    //    public string AccountTo { get; set; }
    //    [Display(Name = "Cost Center From")]
    //    public string CostCenterFrom { get; set; }
    //    [Display(Name = "Cost Center To")]
    //    public string CostCenterTo { get; set; }
    //    [Display(Name = "Sub Account From")]
    //    public string SubAccountFrom { get; set; }
    //    [Display(Name = "Sub Account To")]
    //    public string SubAccountTo { get; set; }
    //}
    //public class FinSetupIncomeStatementConfiguration : TrackUserSettingOperation
    //{
    //    public int FinSetupIncomeStatementConfigurationID { get; set; }
    //    [Required]
    //    public string Category { get; set; }
    //    [Required]
    //    [Display(Name = "Name")]
    //    public string Name { get; set; }
    //    [Required]
    //    [Display(Name = "Account From")]
    //    public string AccountFrom { get; set; }
    //    [Required]
    //    [Display(Name = "Account To")]
    //    public string AccountTo { get; set; }
    //    [Display(Name = "Cost Center From")]
    //    public string CostCenterFrom { get; set; }
    //    [Display(Name = "Cost Center To")]
    //    public string CostCenterTo { get; set; }
    //    [Display(Name = "Sub Account From")]
    //    public string SubAccountFrom { get; set; }
    //    [Display(Name = "Sub Account To")]
    //    public string SubAccountTo { get; set; }
    //}
    //public class FinSetupIncomeStatementByProjectConfiguration : TrackUserSettingOperation
    //{
    //    public int FinSetupIncomeStatementByProjectConfigurationID { get; set; }
    //    [Required]
    //    public string Category { get; set; }
    //    [Required]
    //    [Display(Name = "Name")]
    //    public string Name { get; set; }
    //    [Required]
    //    [Display(Name = "Account From")]
    //    public string AccountFrom { get; set; }
    //    [Required]
    //    [Display(Name = "Account To")]
    //    public string AccountTo { get; set; }
    //    [Display(Name = "Cost Center From")]
    //    public string CostCenterFrom { get; set; }
    //    [Display(Name = "Cost Center To")]
    //    public string CostCenterTo { get; set; }
    //    [Display(Name = "Sub Account From")]
    //    public string SubAccountFrom { get; set; }
    //    [Display(Name = "Sub Account To")]
    //    public string SubAccountTo { get; set; }
    //}
    //public class FinSetupBalanceSheetConfiguration : TrackUserSettingOperation
    //{
    //    public int FinSetupBalanceSheetConfigurationID { get; set; }
    //    [Required]
    //    public string Category { get; set; }
    //    [Display(Name = "Parent")]
    //    public string Parent { get; set; }
    //    [Required]
    //    [Display(Name = "Name")]
    //    public string Name { get; set; }
    //    [Display(Name = "Is Parent")]
    //    public bool IsParent { get; set; }
    //    [Display(Name = "Account From")]
    //    public string AccountFrom { get; set; }
    //    [Display(Name = "Account To")]
    //    public string AccountTo { get; set; }
    //    [Display(Name = "Accum. Dep.Account From")]
    //    public string DepAccountFrom { get; set; }
    //    [Display(Name = "Accum. Dep. Account To")]
    //    public string DepAccountTo { get; set; }

    //}

    public class FinanceSetting : TrackUserSettingOperation
    {
        public int FinanceSettingId { get; set; }
        [Required]
        [Display(Name = "Profit Tax Rate")]
        public decimal ProfitTaxRate { get; set; }
        public decimal ProfitTaxCalculatedRate { get; set; }
        [Required]
        [Display(Name = "Retained Earning")]
        public string RetainedEarningAccount { get; set; }
        [Required]
        [Display(Name = "Retention %")]
        public decimal RetentionPercentage { get; set; }
        [Required]
        [Display(Name = "Retention AP")]
        public string RetentionApAccount { get; set; }
        [Required]
        [Display(Name = "Retention AR")]
        public string RetentionArAccount { get; set; }
        [Display(Name = "Discount AR Account")]
        public string DiscountArAccount { get; set; }
        [Display(Name = "Bank Service Charge Expense Account")]
        public string BankServiceChargeExpense { get; set; }
    }

    public enum AgingType
    {
        AccountReceivable,
        AccountPayable
    }
    public class AgingPeriodSetup : TrackUserSettingOperation
    {
        public int AgingPeriodSetupId { get; set; }
        [Display(Name = "Aging Type")]
        public AgingType Type { get; set; }
        
        [Display(Name = "Period 1 -Days")]
        public int Period1 { get; set; }

        [Display(Name = "Period 2 -Days")]
        public int Period2 { get; set; }

        [Display(Name = "Period 3 -Days")]
        public int Period3 { get; set; }
       
    }

    public enum JournalSources
    {
        GeneralLedger,
        AR_Invoice,
        AR_Collection,
        AR_MiscCollection,
        AP_Invoice,
        AP_Payment,
        AP_PrePayment,
        AP_MiscPayment,
        AP_TravelOrder,
        AP_TravelExpenseClaim,
        AP_PettyCash,
        Payroll,
        Inventory,
        Sales,
        BankServiceCharge,
        AP_Direct_Payment,
        PPE_Monthly_Distribution,
        PPE_Annual_Distribution,
        BudgetTransfer,
        DirectPaymentSettlement,
        PPE_Receive,
        Stock_Adjustment,
        Stock_Receive,
        Stock_Issue,
        Stock_Issue_fuel,
        Stock_Transfer,
        Stock_Transfer_Receive,
        Stock_Return,
        Stock_SaleOrLend,
        Deposit,
        Credit_Settlement,
        TDS,
        AP_Payment_SubContract,
        AR_Collection_Construction,
    }

    public class JournalSourceMap : TrackUserSettingOperation
    {
        public int JournalSourceMapId { get; set; }
        public int GLJournalCategoriersID { get; set; }
        public virtual GLJournalCategoriers Category { get; set; }
        [Required]
        [Display(Name = "Source")]
        public JournalSources Source { get; set; }
        public string Remark { get; set; }
    }

    public class FinSetupCurrency : TrackUserSettingOperation
    {
        public int FinSetupCurrencyID { get; set; }
        [Required]
        [Display(Name = "Currency Code")]
        public string Code { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Issuing Territory")]
        public string Territory { get; set; }
        public bool Enable { get; set; }
    }
    public class FinSetupCurrencyExchange : TrackUserSettingOperation
    {
        public int FinSetupCurrencyExchangeID { get; set; }
        [Required]
        [Display(Name = "From Currency")]
        public string FromCurrency { get; set; }
        [Required]
        [Display(Name = "To Currency")]
        public string ToCurrency { get; set; }
        [Display(Name = "Start Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateFrom { get; set; }
        [Display(Name = "End Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; }
        [Display(Name = "Rate")]
        public decimal RateOne { get; set; }
        [Display(Name = "Reverse Rate")]
        public decimal RateTwo { get; set; }
    }
}
