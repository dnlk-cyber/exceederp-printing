using ExceedERP.Core.Domain.Common;
using System.ComponentModel;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class SalarySetting: TrackUserOperation
    {
        public int SalarySettingId { get; set; }
        [DisplayName("Is Job Title Included In Salary ?")]
        public bool IsJobtitleIncludedInSalary { get; set; }
        public string Remark { get; set; }
    }
}
