using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    public class PerformanceEvaluationSummaryVM
    {
        public string ProjectName { get; set; }
        public decimal ContractAmount { get; set; }
        public decimal AttentionWeight { get; set; }
        public int ValueOrder { get; set; }
        public string ComponentName { get; set; }
        public decimal Value { get; set; }
    }
}
