using ExceedERP.Reporting.SharedDataSources;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Budget.Print.ViewModel
{
    public class BudgetRequestViewModel : Organization
    {
   
        public string Category { get; set; }
        public string Branch { get; set; }
        public int RequestNo { get; set; }
        public string WorkUnit  { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string LineType { get; set; }
        public string HeadOfficeBankNumber { get; set; }
        public string BranchBankNumber { get; set; }
        public string HeadOfficeBankName { get; set; }
        public string BranchBankName { get; set; }
        public string HeadOfficeBankAddress { get; set; }
        public string BranchBankAddress { get; set; }
        public decimal TotalAmount { get; set; }
        
   
    }
}
