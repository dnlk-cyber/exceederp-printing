using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Budget.ViewModel
{
    public class FinancialStatementSingleQuarterVm
    {

        public string Description { get; set; }
        public string AccountDesc { get; set; }
        public string Branch { get; set; }
        public decimal Begining { get; set; }
        public decimal QuarterActual { get; set; }     
        public decimal QuarterPlan { get; set; }     
        public decimal QuarterVariance { get; set; }
        public decimal QuarterPercentage { get; set; }

        public decimal TotalActual { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal TotalVariance { get; set; }
        public decimal TotalPercentage { get; set; }

        public bool IsHeader { get; set; }

    }
}
