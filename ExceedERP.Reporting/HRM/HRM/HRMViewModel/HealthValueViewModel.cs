using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class HealthValueViewModel
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public int EmployeeId { get; set; }
        public string FiscalYear { get; set; }
        public string BenefitType { get; set; }
        public int FiscalYearId { get; set; }
        public DateTime YeearStartDate { get; set; }
        public string AllowedAmount { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public decimal AllowedSum { get; set; }
        public decimal UsedSum { get; set; }
        public decimal RemainingSum { get; set; }
        public IList<HealthValueDetailVM> HealthValueList { get; set; }

      
    }
    public class HealthValueDetailVM
    {
        public string TreatedDate { get; set; }
        public string RecoredDate { get; set; }
        public DateTime YeearStartDate { get; set; }
        public string ReferalDate { get; set; }
        public string FiscalYear { get; set; }
        public int FiscalYearId { get; set; }
        public string Month { get; set; }
        public string AllowedAmount { get; set; }
        public decimal UsedAmount { get; set; }
        public string RemainingAmount { get; set; }
        //////////////////////////////////
        public decimal AllowedSum { get; set; }
        public decimal UsedSum { get; set; }
        public decimal RemainingSum { get; set; }
    }

}
