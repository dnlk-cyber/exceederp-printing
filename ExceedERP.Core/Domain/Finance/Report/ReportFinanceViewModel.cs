using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Report
{
    public class ReportFinanceViewModel
    {
    }

    public class InventoryTransactionViewModel
    {
        public string User { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public int ID { get; set; }
        public string Controller { get; set; }
    }
    public class SupplierListViewModel
    {
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }
        [Display(Name = "Supplier ID")]
        public string ID { get; set; }
        public string Account { get; set; }
        public string PhoneNo { get; set; }
        [Display(Name = "TIN Number")]
        public string TINNo { get; set; }
        [Display(Name = "Address")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }
        public string Country { get; set; }
        [Display(Name = "Supplier Type")]
        public string SupplierType { get; set; }
        [Display(Name = "Business Type")]
        public string BusinessType { get; set; }
        public string IsActive { get; set; }
    }
 
    public class TrialBalalnceViewModel
    {
        [Display(Name = "Account No.")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Name")]
        public string AccountDescription { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
    }
    public class ChartOfAccountViewModel
    {
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Description")]
        public string AccountDescription { get; set; }
        [Display(Name = "Parent")]
        public string Parent { get; set; }
        [Display(Name = "Allowing Budget")]
        public bool AllowingBudgeting { get; set; }
        [Display(Name = "Allowing Posting")]
        public bool AllowingPosting { get; set; }
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }
    }
    public class ChartOfAccountExportViewModel
    {
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Description")]
        public string AccountDescription { get; set; }
        [Display(Name = "Parent")]
        public string Parent { get; set; }
        [Display(Name = "Allowing Budget")]
        public string AllowingBudgeting { get; set; }
        [Display(Name = "Allowing Posting")]
        public string AllowingPosting { get; set; }
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }
    }
    public class AccountAnalysisViewModel
    {
        [Display(Name = "Journal Name")]
        public string JournalName { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Period")]
        public string Period { get; set; }
        [Display(Name = "Effective Date")]
        public string EffectiveDate { get; set; }
        [Display(Name = "Journal Reference")]
        public string JournalReference { get; set; }
        [Display(Name = "Category")]
        public string Category { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Description")]
        public string AccountDescription { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
    }
    public class CashFlowViewModel
    {
        public List<CashFlowCollectedViewModel> CashFlowCollected { get; set; }
        public List<CashFlowPaidViewModel> CashFlowPaid { get; set; }
    }
    public class CashFlowCollectedViewModel
    {
        public string Description { get; set; }
        public decimal Period1 { get; set; }
        public decimal Period2 { get; set; }
        public decimal Period3 { get; set; }
        public decimal Period4 { get; set; }
        public decimal Period5 { get; set; }
        public decimal Period6 { get; set; }
        public decimal Period7 { get; set; }
        public decimal Period8 { get; set; }
        public decimal Period9 { get; set; }
        public decimal Period10 { get; set; }
        public decimal Period11 { get; set; }
        public decimal Period12 { get; set; }
    }
    public class CashFlowPaidViewModel
    {
        public string Description { get; set; }
        public decimal Period1 { get; set; }
        public decimal Period2 { get; set; }
        public decimal Period3 { get; set; }
        public decimal Period4 { get; set; }
        public decimal Period5 { get; set; }
        public decimal Period6 { get; set; }
        public decimal Period7 { get; set; }
        public decimal Period8 { get; set; }
        public decimal Period9 { get; set; }
        public decimal Period10 { get; set; }
        public decimal Period11 { get; set; }
        public decimal Period12 { get; set; }
    }
    public class BalanceSheetViewModel
    {
        public decimal Cash1 { get; set; }
        public decimal Cash2 { get; set; }
        public decimal Cash3 { get; set; }
        public decimal Cash4 { get; set; }
        public decimal Cash5 { get; set; }
        public decimal TotalCash { get; set; }
        public decimal Debtors1 { get; set; }
        public decimal Debtors2 { get; set; }
        public decimal Debtors3 { get; set; }
        public decimal Debtors4 { get; set; }
        public decimal Debtors5 { get; set; }
        public decimal Debtors6 { get; set; }
        public decimal Debtors7 { get; set; }
        public decimal Debtors8 { get; set; }
        public decimal TotalDebtors { get; set; }
        public decimal Inventory1 { get; set; }
        public decimal Inventory2 { get; set; }
        public decimal Inventory3 { get; set; }
        public decimal TotalInventory { get; set; }
        public decimal TotalCurrentAssets { get; set; }
        public decimal FixedAsset1 { get; set; }
        public decimal FixedAsset2 { get; set; }
        public decimal FixedAsset3 { get; set; }
        public decimal FixedAsset4 { get; set; }
        public decimal FixedAsset5 { get; set; }
        public decimal FixedAsset6 { get; set; }
        public decimal FixedAsset7 { get; set; }
        public decimal FixedAsset8 { get; set; }
        public decimal FixedAsset9 { get; set; }
        public decimal FixedAsset10 { get; set; }
        public decimal FixedAsset11 { get; set; }
        public decimal FixedAsset12 { get; set; }
        public decimal FixedAsset13 { get; set; }
        public decimal FixedAsset14 { get; set; }
        public decimal FixedAsset15 { get; set; }
        public decimal FixedAsset16 { get; set; }
        public decimal TotalFixedAsset { get; set; }
        public decimal OtherAsset1 { get; set; }
        public decimal TotalOtherAsset { get; set; }
        public decimal TotalAssets { get; set; }
        public decimal CurrentLiability1 { get; set; }
        public decimal CurrentLiability2 { get; set; }
        public decimal CurrentLiability3 { get; set; }
        public decimal CurrentLiability4 { get; set; }
        public decimal CurrentLiability5 { get; set; }
        public decimal CurrentLiability6 { get; set; }
        public decimal CurrentLiability7 { get; set; }
        public decimal TotalCurrentLiability { get; set; }
        public decimal LongTermLiability { get; set; }
        public decimal TotalLongTermLiability { get; set; }
        public decimal TotalLiability { get; set; }
        public decimal Capital1 { get; set; }
        public decimal Capital2 { get; set; }
        public decimal Capital3 { get; set; }
        public decimal Capital4 { get; set; }
        public decimal Capital5 { get; set; }
        public decimal Capital6 { get; set; }
        public decimal Capital7 { get; set; }
        public decimal Capital8 { get; set; }
        public decimal TotalCapital { get; set; }
        public decimal TotalLiabilityAndEquitiy { get; set; }
    }
    public class IncomeStatementViewModel
    {
        public decimal Revenue1 { get; set; }
        public decimal Revenue2 { get; set; }
        public decimal Revenue3 { get; set; }
        public decimal Revenue4 { get; set; }
        public decimal Revenue5 { get; set; }
        public decimal Revenue6 { get; set; }
        public decimal Revenue7 { get; set; }
        public decimal Revenue8 { get; set; }
        public decimal Revenue9 { get; set; }
        public decimal Revenue10 { get; set; }
        public decimal Revenue11 { get; set; }
        public decimal Revenue12 { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal OperationalExpense1 { get; set; }
        public decimal OperationalExpense2 { get; set; }
        public decimal OperationalExpense3 { get; set; }
        public decimal OperationalExpense4 { get; set; }
        public decimal OperationalExpense5 { get; set; }
        public decimal OperationalExpense6 { get; set; }
        public decimal OperationalExpense7 { get; set; }
        public decimal OperationalExpense8 { get; set; }
        public decimal OperationalExpense9 { get; set; }
        public decimal OperationalExpense10 { get; set; }
        public decimal OperationalExpense11 { get; set; }
        public decimal OperationalExpense12 { get; set; }
        public decimal OperationalExpense13 { get; set; }
        public decimal OperationalExpense14 { get; set; }
        public decimal OperationalExpense15 { get; set; }
        public decimal OperationalExpense16 { get; set; }
        public decimal OperationalExpense17 { get; set; }
        public decimal TotalOperationalExpense { get; set; }
        public decimal AdminExpense1 { get; set; }
        public decimal AdminExpense2 { get; set; }
        public decimal AdminExpense3 { get; set; }
        public decimal AdminExpense4 { get; set; }
        public decimal AdminExpense5 { get; set; }
        public decimal AdminExpense6 { get; set; }
        public decimal AdminExpense7 { get; set; }
        public decimal AdminExpense8 { get; set; }
        public decimal AdminExpense9 { get; set; }
        public decimal AdminExpense10 { get; set; }
        public decimal AdminExpense11 { get; set; }
        public decimal AdminExpense12 { get; set; }
        public decimal AdminExpense13 { get; set; }
        public decimal AdminExpense14 { get; set; }
        public decimal TotalAdminExpense { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal TotalTaxableIncome { get; set; }
    }
    public class IncomeStatementByProjectViewModel
    {
        public string CostCenter { get; set; }
        public string SubAccount { get; set; }
        public string SubAccountDesc { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expense { get; set; }
        public decimal ProfitLoss { get; set; }
    }
    
    
}