using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeePerformanceVM
    {
        public string FullName { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string Jobtitle { get; set; }
        public string Year { get; set; }
        public string Period { get; set; }
        public string EvaluationCriteriaName { get; set; }
        public string EvaluationType { get; set; }
        public string Role { get; set; }
        public decimal Weight { get; set; }
        public decimal Result { get; set; }
        public string Approval { get; set; }
        public List<EmployeeMeasurementVM> EmployeeMeasurmentList { get; set; }

        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }
        public string Date { get; set; }
    }

    public class EmployeeMeasurementVM
    {
        public string MeasurmentName { get; set; }
        public decimal Weight { get; set; }
        public decimal Result { get; set; }
        public List<EmployeePerformanceScoreVM> EmployeePerformanceScoreVMList { get; set; }
    }
    public class EmployeePerformanceScoreVM
    {
        public string ActionName { get; set; }
        public decimal Weight { get; set; }
        //  public string IsChecked { get; set; }
        public decimal Score { get; set; }
    }
}
