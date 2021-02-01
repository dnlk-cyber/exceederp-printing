using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class RetirementSetting
    {
        public int RetirementSettingId { get; set; }
        public RetireSetting RetireSetting { get; set; }
        public double CurrentValue { get; set; }
    }
}
