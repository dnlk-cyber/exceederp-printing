namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeSummary
    {
        public string Criteria { get; set; }
        public string SummaryName { get; set; }
        public int NumberOfUnspecifiedGenderEmployee { get; set; }
        public int NumberOfMaleEmployee { get; set; }
        public int NumberOfFemaleEmployee { get; set; }
        public int NumberOfOtherEmployee { get; set; }
        public double TotalEmployee { get; set; }
        public int index { get; set; }
        
    }
   

    public class EmployeePromotionSummaryVM
    {
        
        public string PromationType { get; set; }
        public int NumberOfMaleEmployee { get; set; }
        public int NumberOfFemaleEmployee { get; set; }
        public double TotalEmployee { get; set; }
       
        
    }
}