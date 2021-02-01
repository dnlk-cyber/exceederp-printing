using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class PromotedEmployee
    {

        public int No { get; set; }
     
        public string RecruitmentType { get; set; }
        public string ApplicantName { get; set; }
        public string Perfromance { get; set; }
        public string TotalYear { get; set; }
        public string TotalMonth { get; set; }
        public string TotalDays { get; set; }
        public string Address { get; set; }
        public string Branch { get; set; }
        public decimal CurrentSalary { get; set; }
        public string Remark { get; set; }
        public List<EmployeeEducation> Educations { get; set; }
        public List<CurrentEmployeeExpriance> Expriance { get; set; }
        public List<OtherEmployeeExpriance> OtherExpriance { get; set; }
        public List<LeaveBalanceViewModel> LeaveBalance { get; set; }
        public int DYear { get; set; }
        public string IntExp { get; set; }
        public string ExtExp { get; set; }
       
        public int DMonth { get; set; }
        public int DDays { get; set; }
        public int OtherYear { get; set; }
        public int OtherMonth { get; set; }
        public int OtherDays { get; set; }
    }
}