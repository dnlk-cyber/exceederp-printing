using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class TerminationSummaryByProjectViewModel
    {
        public string ProjectName { get; set; }
        public string TerminationReason { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public int TotalEmployee => MaleCount + FemaleCount;
    }
}
