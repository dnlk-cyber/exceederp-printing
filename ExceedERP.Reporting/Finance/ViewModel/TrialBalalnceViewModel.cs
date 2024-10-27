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
        public string NaturalAccount { get; set; }
        public string AccountCode { get; set; }
        public string AccountDescription { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
