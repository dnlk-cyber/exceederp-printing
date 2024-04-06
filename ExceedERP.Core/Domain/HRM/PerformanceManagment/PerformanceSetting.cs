using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class PerformanceSetting: TrackUserSettingOperation
    {
        public int PerformanceSettingId { get; set; }
        public decimal Value { get; set; }
    }
}
