using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Budget.ViewModel
{
    public class FinancialStatementQuarterlyVm
    {

        public string Description { get; set; }
        public decimal BeginningActual { get; set; }
        public decimal First { get; set; }
        public decimal Second { get; set; }
        public decimal Third { get; set; }
        public decimal Fourth { get; set; }
        public decimal PlanFirst { get; set; }
        public decimal PlanSecond { get; set; }
        public decimal PlanThird { get; set; }
        public decimal PlanFourth { get; set; }
        public decimal PlanDiffFirst { get; set; }
        public decimal PlanDiffSecond { get; set; }
        public decimal PlanDiffThird { get; set; }
        public decimal PlanDiffFourth { get; set; }
        public decimal ActualDiffFirst { get; set; }
        public decimal ActualDiffSecond { get; set; }
        public decimal ActualDiffThird { get; set; }
        public decimal ActualDiffFourth { get; set; }
        public decimal DiffrenceFirst { get; set; }
        public decimal DiffrenceSecond { get; set; }
        public decimal DiffrenceThird { get; set; }
        public decimal DiffrenceFourth { get; set; }
        public decimal Total { get; set; }
        public decimal ActualTotal { get; set; }
        public decimal PlanTotal { get; set; }
        public decimal PercentageTotal { get; set; }
        public bool IsHeader { get; set; }//for fromatting
    }
}
