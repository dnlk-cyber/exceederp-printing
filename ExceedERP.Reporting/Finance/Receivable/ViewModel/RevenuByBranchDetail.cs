using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Receivable.ViewModel
{
   public class RevenuByBranchDetail
    {
        public string Branch { get; set; }
        public decimal BeginningBalance { get; set; }
        public decimal DebitChange { get; set; }
        public decimal CreditChange { get; set; }
        public decimal NetChange { get; set; }
        public decimal EndingBalance { get; set; }
    }
}
