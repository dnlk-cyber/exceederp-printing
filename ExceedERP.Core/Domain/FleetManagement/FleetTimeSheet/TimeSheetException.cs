using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet{
   
    public class TimeSheetException: TrackUserSettingOperation
    {
        
         public int TimeSheetExceptionId { get; set; }
        public string Reason { get; set; }
        public string BranchId { get; set; }
        public int TimeSheetSettingId { get; set; }

    }
}
