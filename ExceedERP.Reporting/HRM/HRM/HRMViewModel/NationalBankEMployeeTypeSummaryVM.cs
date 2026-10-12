using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class NationalBankEMployeeTypeSummaryVM
    {

        public string Region { get; set; }
        public string PeriodEnding { get; set; }
       
        public List<EmployeeSummaryFlat> EmployeeSummaries { get; set; }
    }
    public class EmployeeSummaryFlat
    {
        public string Criteria { get; set; }

        // Employed During Quarter
        public int EDQ_PermanentMale { get; set; }
        public int EDQ_PermanentFemale { get; set; }
        public int EDQ_PermanentTotal => EDQ_PermanentMale + EDQ_PermanentFemale;
        public int EDQ_TemporaryMale { get; set; }
        public int EDQ_TemporaryFemale { get; set; }
        public int EDQTemporaryTotal => EDQ_TemporaryMale + EDQ_TemporaryFemale;
        public int EDQTotal => EDQ_PermanentMale + EDQ_PermanentFemale+ EDQ_TemporaryMale + EDQ_TemporaryFemale;

        // Total At Quarter End
        public int TQE_PermanentMale { get; set; }
        public int TQE_PermanentFemale { get; set; }
        public int TQE_PermanentTotal => TQE_PermanentMale + TQE_PermanentFemale;

        public int TQE_TemporaryMale { get; set; }
        public int TQE_TemporaryFemale { get; set; }
        public int TQETemporaryTotal => TQE_TemporaryMale + TQE_TemporaryFemale;
        public int TQETotal => TQE_PermanentMale + TQE_PermanentFemale + TQE_TemporaryMale + TQE_TemporaryFemale;
        public int GTotal => EDQ_PermanentMale + EDQ_PermanentFemale + EDQ_TemporaryMale + EDQ_TemporaryFemale+ TQE_PermanentMale + TQE_PermanentFemale + TQE_TemporaryMale + TQE_TemporaryFemale;
    }

}
