using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    class EVMViewModels
    {
    }
    //
    // earned value analysis by project
    //
    public class EearnedValueAnalysisProjectVM
    {
        public string Name { get; set; }

    }
    public class EarnedValuePerProjectViewModel
    {
        public int EarnedValuePerProjectId { get; set; }

        public string ProjectName { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }

        public decimal BAC { get; set; }
        public decimal PV { get; set; }
        public decimal EV { get; set; }
        public decimal AC { get; set; }
        public decimal PCP { get; set; }
        public decimal PCA { get; set; }
        public decimal CV { get; set; }
        public decimal CVPercentage { get; set; }
        public decimal CPI { get; set; }
        public decimal SV { get; set; }
        public decimal SVPercentage { get; set; }
        public decimal SPI { get; set; }
        public decimal CR { get; set; }
        public decimal TCPI { get; set; }
        public decimal TSPI { get; set; }
        public decimal EAC { get; set; }
        public decimal VAC { get; set; }
        public decimal ETC { get; set; }
        public decimal ISAC { get; set; }
    }

    public class EVMPerProjectYearlyAnalysisViewModel
    {
        public EVMPerProjectYearlyAnalysisViewModel()
        {
            MonthlyValueList = new List<EVMMonthlyValuesViewModel>();
        }
        public string ProjectName { get; set; }
        public string Year { get; set; }

        public List<EVMMonthlyValuesViewModel> MonthlyValueList { get; set; }
    }
    public class EVMMonthlyValuesViewModel
    {
        public string Month { get; set; }
        public int RecordOrder { get; set; }
        public decimal BAC { get; set; }
        public decimal BCWS { get; set; }
        public decimal BCWSCummulative { get; set; }
        public decimal BCWP { get; set; }
        public decimal BCWPCummulative { get; set; }
        public decimal ACWP { get; set; }
        public decimal ACWPCummulative { get; set; }
        public decimal PCP { get; set; }
        public decimal PCA { get; set; }
        public decimal CV { get; set; }
        public decimal CVPercentage { get; set; }
        public decimal CPI { get; set; }
        public decimal SV { get; set; }
        public decimal SVPercentage { get; set; }
        public decimal SPI { get; set; }
        public decimal CR { get; set; }
        public decimal TCPI { get; set; }
        public decimal TSPI { get; set; }
        public decimal EAC { get; set; }
        public decimal VAC { get; set; }
        public decimal ETC { get; set; }
    }

    public class EVMProjectGraphicalSummaryVM
    {
        public string ConcatnatedMonth { get; set; }
        public int Order { get; set; }
        public decimal BCWS { get; set; }
        public decimal BCWSCummulative { get; set; }
        public decimal BCWP { get; set; }
        public decimal BCWPCummulative { get; set; }
        public decimal ACWP { get; set; }
        public decimal ACWPCummulative { get; set; }
    }
    //
    // earned value analysis by Item
    //
    public class GLFiscalYearViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class EVMPeriodViewModel
    {
        public EVMPeriodViewModel()
        {
            EVMList = new List<EVMPerItemViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public List<EVMPerItemViewModel> EVMList { get; set; }
    }
    public class EVMPerItemViewModel
    {
        public string WorkDescription { get; set; }
        public string Unit { get; set; }
        public decimal BudgetedQuantity { get; set; }
        public decimal PlannedQuanity { get; set; }
        public decimal ExecutedQuantity { get; set; }
        public decimal EstimatedUnitPrice { get; set; }
        public decimal ActualUnitPrice { get; set; }
        public decimal BAC { get; set; }
        public decimal BCWS { get; set; }
        public decimal BCWP { get; set; }
        public decimal ACWP { get; set; }
        public decimal PCP { get; set; }
        public decimal PCA { get; set; }
        public decimal CV { get; set; }
        public decimal CVPercentage { get; set; }
        public decimal CPI { get; set; }
        public decimal SV { get; set; }
        public decimal SVPercentage { get; set; }
        public decimal SPI { get; set; }
        public decimal CR { get; set; }
        public decimal TCPI { get; set; }
        public decimal TSPI { get; set; }
        public decimal EAC { get; set; }
        public decimal VAC { get; set; }
        public decimal ETC { get; set; }
        public decimal ISAC { get; set; }
    }
    // just for report parameter only
    public class EVMPerItemPeriodViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
