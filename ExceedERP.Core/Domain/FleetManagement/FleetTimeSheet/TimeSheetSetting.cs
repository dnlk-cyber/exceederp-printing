using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet
{
   public class TimeSheetSetting:TrackUserSettingOperation
    {
        public int TimeSheetSettingId { get; set; }
        public bool IsSet { get; set; }
        public int SettedHours { get; set; }
    }
}
