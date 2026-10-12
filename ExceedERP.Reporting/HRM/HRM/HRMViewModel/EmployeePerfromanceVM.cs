using System;
using System.Collections.Generic;


namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeePerfromanceVM
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string WorkArea { get; set; }
        public string Role { get; set; }
        public string JobTitle { get; set; }
        public string BusinessUnit { get; set; }
        public string FiscalYear { get; set; }
        public string Period { get; set; }
        // Evaluation Period
        public string EvaluationFrom { get; set; }
        public string EvaluationTo { get; set; }
        public string HireDate { get; set; }
        public string AssignmentDate { get; set; }
        public string Status { get; set; }

        // Score Summary
        public decimal TotalScore { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal TotalScoreInThePeriod { get; set; }
        public decimal MaximumPossibleScore { get; set; } = 400; // 20 KPIs × 20

        public int TotalKpiCount { get; set; } = 20;

        // Calculated Fields
        public decimal AverageScore { get; set; }
        public decimal Percent { get; set; }
        public string OverallRating { get; set; }   // Excellent / Very Good / etc.
        public string PerformanceRank { get; set; } // Optional ranking in department
        public List<PerDetailEvaluationMeasurementVM> EvaluationCriteriaList { get; set; }
        public List<PerformanceResultVM> PerformanceResults { get; set; }

    }


    public class PerDetailEvaluationMeasurementVM
    {
        public string Period { get; set; }
        public string Measurement { get; set; }
        public decimal Score4 { get; set; }
        public decimal Score8 { get; set; }
        public decimal Score12 { get; set; }
        public decimal Score16 { get; set; }
        public decimal Score20 { get; set; }
        public decimal TotalScore { get; set; }
        public decimal TotalWeight { get; set; }
        // Calculated Fields
        public decimal AverageScore { get; set; }
        public decimal Percent { get; set; }
        public string PeriodRating { get; set; }
        public List<PerDetailEvaluationCriteriaVM> EvaluationCriteriaList { get; set; }
    }

    public class PerDetailEvaluationCriteriaVM
    {
        public string Measurement { get; set; }
        public decimal Weight { get; set; }
        public decimal Score { get; set; }
        // Added fields
        public decimal MaxScore { get; set; } = 20;
        public decimal WeightedScore { get; set; }
        public decimal Percent { get; set; }
        public string Rating { get; set; } // Excellent, Very Good, Good, etc.
        public string Comment { get; set; }
    }
}