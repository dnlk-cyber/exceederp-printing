using System.Collections.Generic;


namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeBscPerfromanceVM
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string Departement { get; set; }
        public string JobTitle { get; set; }      
        public decimal Total { get; set; }
        public string Rank { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public decimal TotalScoreInThePeriod { get; set; }
        public decimal Percent { get; set; }
        public List<EvaluationPeiordVM> EvaluationPeiordList { get; set; }
    }

    public class EvaluationPeiordVM
    {
        public string Period { get; set; }    
        public decimal CriteriaTotalScore { get; set; }
        public List<EvaluationCriteriaVM> EvaluationCriteriaList { get; set; }
    }

    public class EvaluationCriteriaVM
    {
        public string Measurment { get; set; }
        public decimal Weight { get; set; }
        public decimal Score { get; set; }
    }
}