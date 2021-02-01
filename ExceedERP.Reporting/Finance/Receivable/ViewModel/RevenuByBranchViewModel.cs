using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Receivable.ViewModel
{
    public class RevenuByBranchViewModel
    {
        public string AccountNo { get; set; }
        public string ServiceType { get; set; }
        public decimal TotalBeginningBalance { get; set; }
        public decimal TotalDebitChange { get; set; }
        public decimal TotalCreditChange { get; set; }
        public decimal TotalNetChange { get; set; }
        public decimal TotalEndingBalance { get; set; }
        public List<RevenuByBranchDetail> RevenuByBranchList { get; set; }
    }
}
