using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class TrialBalalnceViewModel
    {
        public string Year { get; set; }
        public string Period { get; set; }
        public List<TrialBalalnceLines> TrialBalalnceLines { get; set; }
    }

    public class TrialBalalnceLines
    {
        [Display(Name = "Account No.")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Name")]
        public string AccountDescription { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
    }
}
