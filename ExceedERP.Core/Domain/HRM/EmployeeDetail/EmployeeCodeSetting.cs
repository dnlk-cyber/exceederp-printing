using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class EmployeeCodeSetting
    {
        public int EmployeeCodeSettingId { get; set; }
        public EmployeeIDAbbreviationType Type { get; set; }
        public int Index { get; set; }
        public string Remark { get; set; }
    }
}
