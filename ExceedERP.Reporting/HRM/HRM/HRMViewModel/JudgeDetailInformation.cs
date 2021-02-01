using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class JudgeDetailInformation
    {
        public string FullName { get; set; }
        public Gender Gender { get; set; }
        public int Age { get; set; }
        public string EduLevel { get; set; }
        public string DateOfBirth { get; set; }
        public string HireDate { get; set; }
        public string Nation { get; set; }
        public MartialStatus MartialStatus { get; set; }
        public string PensionDate { get; set; }
        public int Expiriance { get; set; }
        public string PensionNo { get; set; }
    }
}