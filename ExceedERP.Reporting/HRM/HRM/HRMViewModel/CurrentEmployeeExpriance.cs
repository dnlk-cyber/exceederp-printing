using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class CurrentEmployeeExpriance
    {
       
        public string JobTitleName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string FromString { get; set; }
        public string ToString { get; set; }
        public string Year { get; set; }
        public decimal Salary { get; set; }
        public string Branch { get; set; }
        public string RecruitmentType { get; set; }
        public string IntExpSingle { get; set; }
        public int DYear { get; set; }
        public int DMonth { get; set; }
        public int DDays { get; set; }
    }
}