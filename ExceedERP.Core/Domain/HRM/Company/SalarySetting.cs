using System.ComponentModel;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class SalarySetting
    {
        public int SalarySettingId { get; set; }
        [DisplayName("Is Job Title Included In Salary ?")]
        public bool IsJobtitleIncludedInSalary { get; set; }
        public string Remark { get; set; }
    }
}
