using System.Collections.Generic;


namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeePerfromanceWithPlanningVM
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string Departement { get; set; }
        public int OrgStructureId { get; set; }
        public decimal Planning { get; set; }
        public decimal PlanningFrom100 { get; set; }
        public decimal Difference { get; set; }
        public string Role { get; set; }      
        public string JobTitle { get; set; }      
        public decimal Total { get; set; }
        public decimal TotalWeight { get; set; }
        public string Rank { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public decimal TotalScoreInThePeriod { get; set; }
        public decimal Percent { get; set; }
        public decimal NumberOfMonthsServed { get; set; }
        public decimal NumberOfDayssServed { get; set; }
        public decimal DisplineDays { get; set; }
        public decimal LeaveTaken { get; set; }
        public decimal Bonus { get; set; }
        public string Level { get; set; }
        public string Remark { get; set; }
        public List<EvaluationWithPlanningPeiordVM> EvaluationWithPlanningLists { get; set; }
    }

    public class EvaluationWithPlanningPeiordVM
    {
        public string Period { get; set; }    
        public string Criteria { get; set; }
        public decimal CriteriaTotalWeight { get; set; }
        public decimal CriteriaTotalScore { get; set; }
        public List<EvaluationPlanningCriteriaVM> EvaluationWithPlanningCriteriaList { get; set; }
    }

    public class EvaluationPlanningCriteriaVM
    {
        public string Measurment { get; set; }
        public decimal Weight { get; set; }
        public decimal Score { get; set; }
    }
}