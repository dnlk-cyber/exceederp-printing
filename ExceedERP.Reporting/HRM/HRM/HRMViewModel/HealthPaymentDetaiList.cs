using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class HealthPaymentDetaiList
    {
        public string FiscalYear { get; set; }
        public string Period { get; set; }
        public string TreatmentDate { get; set; }
        public string ReferalDate { get; set; }
        public string RecoredDate { get; set; }
        public string AllowedAmt{ get; set; }
        public  decimal UsedAmt { get; set; }
        public string RemainingAmt { get; set; }
        public string AllowedSum { get; set; }        
        public string RemainingSum { get; set; }
        public decimal UsedSum { get; set; }
    }
}
