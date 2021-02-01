using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{

    public class CashFlowYearReportParameterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class CashFlowYearViewModel
    {
        public CashFlowYearViewModel()
        {
            ContractDataList = new List<CashFlowContractDataViewModel>();
            FinancialSourceList = new List<CashFlowFinancialSorceViewModel>();
            DeductionList = new List<CashFlowDeductionViewModel>();
            NetOperationList = new List<CashFlowNetOperationViewModel>();
            ExpenseList = new List<CashFlowExpenseViewModel>();
            BalanceList = new List<CashFlowBalanceViewModel>();
        }
        public string Name { get; set; }
        public List<CashFlowContractDataViewModel> ContractDataList { get; set; }
        public List<CashFlowFinancialSorceViewModel> FinancialSourceList { get; set; }
        public List<CashFlowDeductionViewModel> DeductionList { get; set; }
        public List<CashFlowNetOperationViewModel> NetOperationList { get; set; }
        public List<CashFlowExpenseViewModel> ExpenseList { get; set; }
        public List<CashFlowBalanceViewModel> BalanceList { get; set; }

    }
    public class CashFlowBaseVM 
    {
        public string Month { get; set; }
        public DateTime MonthEnd { get; set; }
        public int Order { get; set; }
        public string AttributeName { get; set; }
        public decimal AttributeValue { get; set; }

        public decimal PlannedTotalAmount { get; set; }

    }
    public class CashFlowContractDataViewModel : CashFlowBaseVM
    {
        public bool IsMainContent { get; set; }
        public decimal ActualTotalAmount { get; set; }
        public decimal TCPPercentage { get; set; }
    }
    public class CashFlowFinancialSorceViewModel : CashFlowBaseVM
    {
    }
    public class CashFlowDeductionViewModel: CashFlowBaseVM
    {

    }
    public class CashFlowNetOperationViewModel : CashFlowBaseVM
    {
    }
    public class CashFlowExpenseViewModel : CashFlowBaseVM
    {
    }
    public class CashFlowBalanceViewModel : CashFlowBaseVM
    {

    }
}
