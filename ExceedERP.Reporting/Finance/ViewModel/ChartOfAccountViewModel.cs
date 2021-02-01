using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class ChartOfAccountViewModel
    {
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Description")]
        public string AccountDescription { get; set; }
        [Display(Name = "Parent")]
        public string Parent { get; set; }
        [Display(Name = "Allowing Budget")]
        public string AllowingBudgeting { get; set; }
        [Display(Name = "Allowing Posting")]
        public string AllowingPosting { get; set; }
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }
    }
}
