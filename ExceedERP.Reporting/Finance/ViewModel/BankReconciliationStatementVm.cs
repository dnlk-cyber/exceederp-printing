using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Finance.ViewModel
{
   
    public class BankReconciliationStatementVm
    {
        public BankReconciliationStatementVm()
        {
            EndingBalanceOfBankStatements=new List<EndingBalanceOfBankStatement>();
            Deposits=new List<BankReconciliationDeposit>();
            OutstandingPayments =new List<BankReconciliationOutstandingPayment>();
            DepositKnownByBanks =new List<BankReconciliationDepositByBankNotRecordedOnLedger>();
            WithdrawByBanks =new List<BankReconciliationWithdrawByBankNotRecordedOnLedger>();
        }

        public decimal EndingBalanceOfBankStatement { get; set; }
        public string EndingBalanceOfBankStatementPeriodName { get; set; }
        public decimal EndingBalanceOfLedgerBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal AdjustedBankBalance { get; set; }
        public decimal BalanceAsPerLedger { get; set; }
        public decimal BankServiceCharge { get; set; }
        public decimal AdjustedLedgerBalance { get; set; }
        public decimal UnreconciledDifference { get; set; }

        public ICollection<EndingBalanceOfBankStatement> EndingBalanceOfBankStatements { get; set; }

        public ICollection<BankReconciliationDeposit> Deposits { get; set; }
        public ICollection<BankReconciliationOutstandingPayment> OutstandingPayments { get; set; }
        public ICollection<BankReconciliationDepositByBankNotRecordedOnLedger> DepositKnownByBanks { get; set; }
        public ICollection<BankReconciliationWithdrawByBankNotRecordedOnLedger> WithdrawByBanks { get; set; }

       


    }

    public class EndingBalanceOfBankStatement
    {
        public string PeriodName { get; set; }
        public decimal EndingBalance { get; set; }

    }
    public class BankReconciliationDeposit
    {
        public BookTransactionType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public string Reference { get; set; }
        public string ReferenceDoc { get; set; }
        public string Source { get; set; }
        public decimal ActivityBalance { get; set; }
        public decimal TotalBalance { get; set; }

    }
    public class BankReconciliationOutstandingPayment
    {
    public BookTransactionType TransactionType { get; set; }
    public DateTime Date { get; set; }
    public string Reference { get; set; }
    public string ReferenceDoc { get; set; }
    public string Source { get; set; }
    public decimal ActivityBalance { get; set; }
    public decimal TotalBalance { get; set; }

    }
    public class BankReconciliationDepositByBankNotRecordedOnLedger
    {
        public BookTransactionType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public string Reference { get; set; }
        public string ReferenceDoc { get; set; }
        public string Source { get; set; }
        public decimal ActivityBalance { get; set; }
        public decimal TotalBalance { get; set; }

    }
    public class BankReconciliationWithdrawByBankNotRecordedOnLedger
    {
        public BookTransactionType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public string Reference { get; set; }
        public string ReferenceDoc { get; set; }
        public string Source { get; set; }
        public decimal ActivityBalance { get; set; }
        public decimal TotalBalance { get; set; }

    }


}
