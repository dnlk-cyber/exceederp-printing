using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeMasterViewModel
    {
        public string EmployeeCode { get; set; }
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
       // public string EduLevel { get; set; }
        public string DateOfBirth { get; set; }
        public string RetirementDate { get; set; }
        public string HireDate { get; set; }
        public string Nation { get; set; }
        public string MartialStatus { get; set; }
        public string PensionDate { get; set; }
        public string Expiriance { get; set; }
        public string PensionNo { get; set; }
        // public string EducationField { get; set; }
        public List<EmployeeEductaionList> Education { get; set; }
        public decimal Salary { get; set; }
        public string JobTitle { get; set; }
        public string IsDirect { get; set; }
        public string Step { get; set; }
        public string Rank { get; set; }
        public string Religion { get; set; }
        public string InternalExpriance { get; set; }
        public string ExpTotalSum { get; set; }
        public decimal TotalLeave { get; set; }
        public string BranchName { get; set; }
        public string StructureName { get; set; }
        public string ResponsibilityLevel { get; set; }
        public string EmployementType { get; set; }
        public string Employement { get; set; }
        public string Performance { get; set; }
        public List<Training> Training { get; set; }
        public List<TrainingExt> TrainingExt { get; set; }
        public List<Languages> Languages { get; set; }
        public List<EmployeePerformances> EmployeePerformances { get; set; }
        public List<Discipline> Discipline { get; set; }

    }
    public class Training
    {
        public string Name { get; set; }
    }
    public class TrainingExt
    {
        public string Name { get; set; }
    }
    public class Languages
    {
        public string Name { get; set; }
    }
    public class EmployeePerformances
    {
        public string Type { get; set; }
        public string Result { get; set; }
        public string EvaluationCriteria { get; set; }
    }
    public class Discipline
    {
        public string MeasureRank { get; set; }
        public string MeasureTaken { get; set; }
        public int Month { get; set; }
        public string DecisionDate { get; set; }
        public decimal Amount { get; set; }
    }
}