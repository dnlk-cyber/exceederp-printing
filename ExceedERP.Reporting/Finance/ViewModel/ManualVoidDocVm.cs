using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class ManualVoidDocVm
    {
    
        public string JournalName { get; set; }
        public string Description { get; set; }
        public string FiscalYear { get; set; }
        public string PeriodName { get; set; }
        public string CategoryName { get; set; }
        public string SourcesName { get; set; }
        [Display(Name = "Journal Reference")]
        public string JournalNumber { get; set; }
        public DateTime EffectiveDate { get; set; }
        
        public string Remark { get; set; }
    }
}
