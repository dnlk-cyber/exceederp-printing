using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.printing.JobCosting.Setting
{
    public class IdleTimeCategory :  TrackUserSettingOperation
    {
        public int IdleTimeCategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsControllable { get; set; }
        public string CategoryDescription { get; set; }

    }
}
