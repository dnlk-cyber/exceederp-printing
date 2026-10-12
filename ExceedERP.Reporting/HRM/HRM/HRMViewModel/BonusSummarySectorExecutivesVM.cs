using System.Collections.Generic;


namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class BonusSummarySectorExecutivesVM
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Departement { get; set; }
        public string Role { get; set; }      
        public string JobTitle { get; set; } 
        public decimal FirstHalfEvaluationResultOf20 { get; set; }
        public decimal SecondHalfEvaluationResultOf20 { get; set; }
        public decimal AverageOf20 { get; set; }
        public decimal AverageCorpBScOutOf30 { get; set; }
        public decimal AverageSectorBScOutOf50 { get; set; }
        public decimal TotalScoreOutOf100 { get; set; }
        public decimal DatesServedInThisFiscalYear { get; set; }
        public decimal Bonus { get; set; }
        public string Step { get; set; }
        public string Remark { get; set; }
        public decimal TotalWeight { get; set; }
        public string Rank { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string Period { get; set; }
        public string Criteria { get; set; }
        public decimal CriteriaTotalWeight { get; set; }
        public decimal CriteriaTotalScore { get; set; }
        public string Measurment { get; set; }
        public decimal Weight { get; set; }
        public decimal Score { get; set; }
        public decimal TotalScoreInThePeriod { get; set; }
        public decimal Percent { get; set; }

    }

}