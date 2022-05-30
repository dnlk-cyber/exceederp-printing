using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
